using System;
using System.Linq;
using System.Drawing;
using EnsoulSharp;
using EnsoulSharp.Common;
using Color = SharpDX.Color;
using HitChance = EnsoulSharp.Common.HitChance;
using Orbwalking = EnsoulSharp.Common.Orbwalking;
using Prediction = EnsoulSharp.Common.Prediction;
using PredictionInput = EnsoulSharp.Common.PredictionInput;
using SebbyLib;
using SPrediction;

namespace Blank_Kha_Zix
{
    class BlankKhaZix
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        protected static Orbwalking.Orbwalker Orbwalker;
        protected static Menu Config;
        protected bool _isMidAir;
        private static AIHeroClient Player;
        private static string ChampionName = "Khazix";

        protected bool BoolEvolvedQ, BoolEvolvedW, BoolEvolvedE;

        protected string[] HighChamps =
        {
            "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
            "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "Leblanc",
            "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon",
            "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath",
            "Zed", "Ziggs", "Kindred", "Jhin"
        };

        protected AIBaseClient KhaETrail, KhaELand;
        static void Main(string[] args)
        {
            var khaXiz = new BlankKhaZix();
        }

        public BlankKhaZix()
        {
            CustomEvents.Game.OnGameLoad += GameOnGameLoad;
        }

        public Items.Item Hydra { get; private set; }
        public Items.Item Tiamat { get; private set; }
        public Items.Item Blade { get; private set; }
        public Items.Item Bilgewater { get; private set; }
        public Items.Item Youmuu { get; private set; }
        public Items.Item TitanicHydra { get; private set; }
        public SpellSlot IgniteSlot { get; private set; }
        public SpellSlot SmiteSlot { get; private set; }
        public Spell Ignite { get; private set; }
        public Spell Smite { get; private set; }

        private void GameOnGameLoad(EventArgs args)
        {
            if (Player.CharacterName != ChampionName)
                return;

            #region Spells && Items

            IgniteSlot = Player.GetSpellSlot("summonerdot");
            var smite = Player.Spellbook.Spells.FirstOrDefault(x => x.Name.ToLower().Contains("smite"));
            Q = new Spell(SpellSlot.Q, 325f);
            W = new Spell(SpellSlot.W, 1000f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 0);
            W.SetSkillshot(0.225f, 80f, 825f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 100f, 1000f, false, SkillshotType.SkillshotCircle);
            if (IgniteSlot != SpellSlot.Unknown)
                Ignite = new Spell(IgniteSlot, 550f);
            if ((smite != null) && (smite.Slot != SpellSlot.Unknown))
                Smite = new Spell(smite.Slot, 500f, TargetSelector.DamageType.True);

            Hydra = new Items.Item(3074, 225f);
            Tiamat = new Items.Item(3077, 225f);
            Blade = new Items.Item(3153, 450f);
            Bilgewater = new Items.Item(3144, 450f);
            Youmuu = new Items.Item(3142, 185f);
            TitanicHydra = new Items.Item(3748, 225f);

            #endregion

            #region Config

            Config = new Menu("Blank Kha'Zix", "Blank Kha'Zix", true).SetFontStyle(FontStyle.Bold, Color.Crimson);

            var orbwalkerMenu = Config.AddSubMenu(new Menu(":: Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            var targetSelectorMenu = Config.AddSubMenu(new Menu(":: Target Selector", "TargetSelector"));

            TargetSelector.AddToMenu(targetSelectorMenu);

            #endregion

            var AssassinManagerMenu = Config.AddSubMenu(new Menu(":: Assassin Manager", "AssassinManager"));
            AssassinManagerMenu.AddItem(
                new MenuItem("AssassinateTarget", "Assassinate Target").SetValue(new KeyBind("T".ToCharArray()[0],
                    KeyBindType.Press))).Permashow(true, "Assassinating?");
            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValid))
                if (HighChamps.Contains(enemy.CharacterName))
                    AssassinManagerMenu.AddItem(
                        new MenuItem("AssassinManager." + enemy.CharacterName,
                                "Assassination (Priority Main): " + enemy.CharacterName).SetValue(new Slider(5, 0, 5))
                            .SetFontStyle(FontStyle.Bold, Color.Chartreuse));
                else
                    AssassinManagerMenu.AddItem(
                        new MenuItem("AssassinManager." + enemy.CharacterName,
                                "Assassination (Priority): " + enemy.CharacterName).SetValue(new Slider(3, 0, 5))
                            .SetFontStyle(FontStyle.Bold, Color.Crimson));

            var comboMenu = Config.AddSubMenu(new Menu(":: Combo", "Combo"));
            comboMenu.AddItem(new MenuItem("ComboUseQ", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("ComboUseW", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("ComboUseE", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("DontAAInInvisible", "Don't AA while Invisible?").SetValue(true));
            comboMenu.AddItem(
                new MenuItem("ComboDontEUnderTurret", "Don't E (Jump) under Turret?").SetValue(true));
            comboMenu.AddItem(
                new MenuItem("ComboMinimumREnemies", "Minimum Enemies in E Range Before Casting R").SetValue(
                    new Slider(2, 1, 5)));
            comboMenu.AddItem(
                new MenuItem("ComboUseR", "Use R").SetValue(true));

            var laneClearMenu = Config.AddSubMenu(new Menu(":: LaneClear", "LaneClear"));
            laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q").SetValue(true));
            laneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W").SetValue(true));
            laneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E").SetValue(false));
            laneClearMenu.AddItem(new MenuItem("LaneClearItems", "Use Hydra/Tiamat to LaneClear?").SetValue(true));
            laneClearMenu.AddItem(
                new MenuItem("LaneClearManaManager", "LaneClear Mana Manager").SetValue(new Slider(50, 0, 100)));
            laneClearMenu.AddItem(
                new MenuItem("MinimumEMinions", "Minimum Minions To Hit Using E?").SetValue(new Slider(3, 0, 10)));

            var jungleClearMenu = Config.AddSubMenu(new Menu(":: JungleClear", "JungleClear"));
            jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q").SetValue(true));
            jungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W").SetValue(true));
            jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E").SetValue(false));
            jungleClearMenu.AddItem(new MenuItem("JungleClearItems", "Use Hydra/Tiamat to JungleClear?").SetValue(true));
            jungleClearMenu.AddItem(
                new MenuItem("JungleClearDontEQRange", "Don't Use E if Mobs are in Q Range?").SetValue(true));
            jungleClearMenu.AddItem(
                new MenuItem("JungleClearManaManager", "JungleClear Mana Manager").SetValue(new Slider(50, 0, 100)));
            jungleClearMenu.AddItem(
                new MenuItem("MinimumEJungleMobs", "Minimum Mobs in E before Jumping?").SetValue(new Slider(2, 1, 4)));

            var lastHitMenu = Config.AddSubMenu(new Menu(":: LastHit", "LastHit"));
            lastHitMenu.AddItem(new MenuItem("LastHitQ", "Use Q").SetValue(true));
            lastHitMenu.AddItem(new MenuItem("LastHitW", "Use W").SetValue(false));
            lastHitMenu.AddItem(
                new MenuItem("LastHitManaManager", "LastHit Mana Manager Mana Manager").SetValue(new Slider(50, 0, 100)));

            var harassMenu = Config.AddSubMenu(new Menu(":: Harass", "Harass"));
            harassMenu.AddItem(new MenuItem("HarassQ", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("HarassW", "Use W").SetValue(true));
            harassMenu.AddItem(
                new MenuItem("HarassManaManager", "Harass Mana Manager Mana Manager").SetValue(new Slider(50, 0, 100)));

            var killStealMenu = Config.AddSubMenu(new Menu(":: Killsteal", "Killsteal"));
            killStealMenu.AddItem(new MenuItem("EnableKS", "Enable Killsteal?").SetValue(true));
            killStealMenu.AddItem(new MenuItem("KSQ", "KS with Q?").SetValue(true));
            killStealMenu.AddItem(new MenuItem("UnavailableService",
                "KS with Ignite/Smite is currently (Temporary) Unavailable."));
            killStealMenu.AddItem(new MenuItem("KSIgnite", "KS with Ignite").SetValue(false));
            killStealMenu.AddItem(new MenuItem("KSSmite", "KS with Smite").SetValue(false));

            var drawingMenu = Config.AddSubMenu(new Menu(":: Drawings", "Drawings"));
            drawingMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range").SetValue(true));
            drawingMenu.AddItem(new MenuItem("DrawW", "Draw W Range").SetValue(false));
            drawingMenu.AddItem(new MenuItem("DrawE", "Draw E Range").SetValue(true));
            drawingMenu.AddItem(
                new MenuItem("DrawIsolated", "Draw Isolated?").SetValue(true));
            drawingMenu.AddItem(new MenuItem("DrawIsMidAirDebug", "Draw isMidAir (Debug)").SetValue(false));

            var miscMenu = Config.AddSubMenu(new Menu(":: Misc", "Misc"));
            miscMenu.AddItem(
                new MenuItem("HitChance", "Hit Chance").SetValue(new StringList(new[] { "Medium", "High", "Very High" }, 1)));

            var drawdamage = new Menu(":: Draw Damage", "drawdamage");
            {
                var dmgAfterShave =
                    new MenuItem("SurvivorKhaZix.DrawComboDamage", "Draw Damage on Enemy's HP Bar").SetValue(true);
                var drawFill =
                    new MenuItem("SurvivorKhaZix.DrawColour", "Fill Color", true).SetValue(
                        new Circle(true, System.Drawing.Color.Chartreuse));
                drawdamage.AddItem(drawFill);
                drawdamage.AddItem(dmgAfterShave);
                DrawDamage.DamageToUnit = CalculateDamage;
                DrawDamage.Enabled = dmgAfterShave.GetValue<bool>();
                DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
                DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;
                dmgAfterShave.ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                    };

                drawFill.ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                    DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };
            }

            #region Subs
            Game.OnTick += GameOnUpdate;
            Drawing.OnDraw += DrawingOnOnDraw;
            GameObject.OnCreate += GameObjectOnOnCreate;
            GameObject.OnDelete += GameObjectOnOnDelete;
            #endregion
        }

        private void GameObjectOnOnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Khazix_Base_E_WeaponTrails.troy")
            {
                KhaETrail = null;
                _isMidAir = false;
            }
            if (sender.Name == "Khazix_Base_E_Land.troy")
            {
                KhaELand = null;
                _isMidAir = false;
            }
        }

        private void GameObjectOnOnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Khazix_Base_E_WeaponTrails.troy")
            {
                KhaETrail = sender as AIBaseClient;
                _isMidAir = true;
            }
            if (sender.Name == "Khazix_Base_E_Land.troy")
            {
                KhaELand = sender as AIBaseClient;
                _isMidAir = false;
            }
        }

        private AIHeroClient AssassinManager()
        {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range)))
                {
                    #region Target Acquire

                    AIHeroClient mostPriority = null;
                    switch (Config.Item("AssassinManager." + enemy.CharacterName).GetValue<Slider>().Value)
                    {
                        case 5:
                            {
                                if (Config.Item("AssassinManager." + enemy.CharacterName).GetValue<Slider>().Value == 5)
                                    mostPriority = enemy;
                                break;
                            }
                        case 4:
                            {
                                if (Config.Item("AssassinManager." + enemy.CharacterName).GetValue<Slider>().Value == 4)
                                    mostPriority = enemy;
                                break;
                            }
                        case 3:
                            {
                                if (Config.Item("AssassinManager." + enemy.CharacterName).GetValue<Slider>().Value == 3)
                                    mostPriority = enemy;
                                break;
                            }
                        case 2:
                            {
                                if (Config.Item("AssassinManager." + enemy.CharacterName).GetValue<Slider>().Value == 2)
                                    mostPriority = enemy;
                                break;
                            }
                        case 1:
                            {
                                if (Config.Item("AssassinManager." + enemy.CharacterName).GetValue<Slider>().Value == 1)
                                    mostPriority = enemy;
                                break;
                            }
                        default:
                            mostPriority = null;
                            break;
                    }

                    #endregion

                    if ((mostPriority != null) || mostPriority.IsValidTarget())
                        return mostPriority;
                }
            return null;
        }

        private void SebbySpell(Spell w, AIBaseClient target)
        {
            var aoe2 = false;

            if (w.Type == SkillshotType.SkillshotCircle)
            {
                aoe2 = true;
            }

            if ((w.Width > 80) && !w.Collision)
                aoe2 = true;

            var predInput2 = new PredictionInput
            {
                Aoe = aoe2,
                Collision = w.Collision,
                Speed = w.Speed,
                Delay = w.Delay,
                Range = w.Range,
                From = Player.Position,
                Radius = w.Width,
                Unit = target,
            };
            var poutput2 = Prediction.GetPrediction(predInput2);

            if (OktwCommon.CollisionYasuo(Player.Position, poutput2.CastPosition))
                return;

            if ((w.Speed != float.MaxValue) && OktwCommon.CollisionYasuo(Player.Position, poutput2.CastPosition))
                return;

            if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 0)
            {
                if (poutput2.Hitchance >= HitChance.Medium)
                    w.Cast(poutput2.CastPosition);
            }
            else if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 1)
            {
                if (poutput2.Hitchance >= HitChance.High)
                    w.Cast(poutput2.CastPosition);
            }
            else if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 2)
            {
                if (poutput2.Hitchance >= HitChance.VeryHigh)
                    w.Cast(poutput2.CastPosition);
            }
        }

        private void DrawingOnOnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Config.Item("DrawQ").GetValue<bool>() && Q.IsReady())
                Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Crimson);
            if (Config.Item("DrawW").GetValue<bool>() && W.IsReady())
                Render.Circle.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Aqua);
            if (Config.Item("DrawE").GetValue<bool>() && E.IsReady())
                Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Chartreuse);
            if (!Config.Item("DrawIsolated").GetValue<bool>())
                return;

            foreach (
                var enemy in
                HeroManager.Enemies.Where(
                    x => IsIsolated(x) && x.IsValidTarget() && (x.Distance(Player.Position) < 3000)))
            {
                var drawPos = Drawing.WorldToScreen(enemy.Position);
            }
        }

        private void EvolvedSpells()
        {
            if (!BoolEvolvedQ && Player.HasBuff("khazixqevo"))
            {
                Q.Range = 375;
                BoolEvolvedQ = true;
            }
            if (!BoolEvolvedW && Player.HasBuff("khazixwevo"))
            {
                W.Width = 103f;
                BoolEvolvedW = true;
            }

            if (!BoolEvolvedE && Player.HasBuff("khazixeevo"))
            {
                E.Range = 900;
                BoolEvolvedE = true;
            }
        }

        private bool IsInvisible()
        {
            return Player.HasBuff("khazixrstealth");
        }

        private void AssassinateProgram(AIHeroClient target)
        {
            if ((target == null) || !target.IsValidTarget())
                return;

            if (E.Instance.IsReady() && !Q.IsInRange(target))
                E.Cast(target.Position);
            if (Q.Instance.IsReady())
                Q.CastOnUnit(target);
            if ((_isMidAir && target.IsValidTarget(Hydra.Range)) ||
                target.IsValidTarget(Tiamat.Range) ||
                target.IsValidTarget(TitanicHydra.Range))
            {
                if (Hydra.IsReady())
                    Hydra.Cast();
                if (TitanicHydra.IsReady())
                    TitanicHydra.Cast();
                if (Tiamat.IsReady())
                    Tiamat.Cast();
            }

            if (Youmuu.IsReady() && target.IsValidTarget(Player.AttackRange + 400))
                Youmuu.Cast();
            if (Hydra.IsReady() && target.IsValidTarget(Hydra.Range))
                Hydra.Cast();
            if (TitanicHydra.IsReady() && target.IsValidTarget(TitanicHydra.Range))
                TitanicHydra.Cast();
            if (Tiamat.IsReady() && target.IsValidTarget(Tiamat.Range))
                Tiamat.Cast();
        }

        private void GameOnUpdate(EventArgs args)
        {
            EvolvedSpells();
            if (Player.IsDead || Player.IsRecalling())
                return;

            Orbwalker.SetAttack(!IsInvisible());
            KillStealCheck();
            if (Config.Item("AssassinateTarget").GetValue<KeyBind>().Active)
            {
                var target = AssassinManager();
                if ((target == null) || !target.IsValidTarget())
                {
                    Orbwalking.MoveTo(Game.CursorPosCenter);
                    return;
                }
                if (target.IsValidTarget(E.Range))
                {
                    Orbwalking.Orbwalk(target, Game.CursorPosCenter);
                    AssassinateProgram(target);
                }
            }
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    JungleClear();
                    LaneClear();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    HarassMode();
                    break;
            }
        }

        private void KillStealCheck()
        {
            if (Config.Item("EnableKS").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if ((target == null) || !target.IsValidTarget())
                    return;

                if (Config.Item("KSQ").GetValue<bool>() && Q.Instance.IsReady() &&
                    (target.Health < GetRealQDamage(target) + OktwCommon.GetIncomingDamage(target)))
                    Q.CastOnUnit(target);
            }
        }

        private void Combo()
        {
            var useQ = Config.Item("ComboUseQ").GetValue<bool>();
            var useW = Config.Item("ComboUseW").GetValue<bool>();
            var useE = Config.Item("ComboUseE").GetValue<bool>();
            var useR = Config.Item("ComboUseR").GetValue<bool>();
            var dontEUnderTurret = Config.Item("ComboDontEUnderTurret").GetValue<bool>();
            var comboMinimumREnemies = Config.Item("ComboMinimumREnemies").GetValue<Slider>().Value;

            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if ((target == null) || !target.IsValidTarget())
                return;

            if (useR && (Player.CountEnemiesInRange(E.Range) >= comboMinimumREnemies) && R.Instance.IsReady())
                R.Cast();

            if (useE && E.Instance.IsReady() && !Q.IsInRange(target))
            {
                if (target.UnderTurret() && dontEUnderTurret)
                    return;
                E.Cast(target.Position);
            }
            if (useQ && Q.Instance.IsReady())
                Q.CastOnUnit(target);
            if ((_isMidAir && target.IsValidTarget(Hydra.Range)) || target.IsValidTarget(Tiamat.Range) ||
                target.IsValidTarget(TitanicHydra.Range))
            {
                if (Hydra.IsReady())
                    Hydra.Cast();
                if (TitanicHydra.IsReady())
                    TitanicHydra.Cast();
                if (Tiamat.IsReady())
                    Tiamat.Cast();
            }
            if (useW && W.Instance.IsReady())
                SebbySpell(W, target);

            if (Youmuu.IsReady() && target.IsValidTarget(Player.AttackRange + 400))
                Youmuu.Cast();
            if (Hydra.IsReady() && target.IsValidTarget(Hydra.Range))
                Hydra.Cast();
            if (TitanicHydra.IsReady() && target.IsValidTarget(TitanicHydra.Range))
                TitanicHydra.Cast();
            if (Tiamat.IsReady() && target.IsValidTarget(Tiamat.Range))
                Tiamat.Cast();
        }

        private void HarassMode()
        {
            var useQ = Config.Item("HarassQ").GetValue<bool>();
            var useW = Config.Item("HarassW").GetValue<bool>();
            var harassManaManager = Config.Item("HarassManaManager").GetValue<Slider>().Value;

            if (Player.ManaPercent < harassManaManager)
                return;

            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

            if ((target == null) || !target.IsValidTarget())
                return;

            if (useQ && Q.Instance.IsReady() && target.IsValidTarget(Q.Range))
                Q.CastOnUnit(target);

            if (useW && W.Instance.IsReady() && target.IsValidTarget(W.Range))
                SebbySpell(W, target);
        }

        private void LastHit()
        {
            var useQ = Config.Item("LastHitQ").GetValue<bool>();
            var useW = Config.Item("LastHitW").GetValue<bool>();
            var lastHitManaManager = Config.Item("LastHitManaManager").GetValue<Slider>().Value;

            if (Player.ManaPercent < lastHitManaManager)
                return;

            var minion = Cache.GetMinions(Player.Position, W.Range, SebbyLib.MinionTeam.Enemy).FirstOrDefault();
            if ((minion == null) || !minion.IsValidTarget())
                return;

            if (useQ && Q.Instance.IsReady() && (minion.Health < Q.GetDamage(minion)))
                Q.CastOnUnit(minion);

            if (useW && W.Instance.IsReady() && (minion.Health < W.GetDamage(minion)))
                W.Cast(minion.Position);
        }

        private void LaneClear()
        {
            var useQ = Config.Item("LaneClearQ").GetValue<bool>();
            var useW = Config.Item("LaneClearW").GetValue<bool>();
            var useE = Config.Item("LaneClearE").GetValue<bool>();
            var useItems = Config.Item("LaneClearItems").GetValue<bool>();
            var laneClearManaManager = Config.Item("LaneClearManaManager").GetValue<Slider>().Value;
            var minimumEMinions = Config.Item("MinimumEMinions").GetValue<Slider>().Value;

            var minionsq =
                Cache.GetMinions(Player.Position, Q.Range, SebbyLib.MinionTeam.Enemy)
                    .OrderByDescending(x => x.Distance(Player.Position))
                    .FirstOrDefault();

            if (useItems)
            {
                if (Hydra.IsReady() && minionsq.IsValidTarget(Hydra.Range))
                    Hydra.Cast();

                if (Tiamat.IsReady() && minionsq.IsValidTarget(Tiamat.Range))
                    Tiamat.Cast();
            }

            if (Player.ManaPercent < laneClearManaManager)
                return;

            var minionselist = Cache.GetMinions(Player.Position, E.Range, SebbyLib.MinionTeam.Enemy);
            var minionsw = minionselist.OrderByDescending(x => x.Distance(Player.Position)).FirstOrDefault();
            var minionse = E.GetCircularFarmLocation(minionselist);

            if (useQ && Q.Instance.IsReady() && minionsq.IsValidTarget() && (minionsq != null) &&
                (minionsq.Health < GetRealQDamage(minionsq)))
                Q.CastOnUnit(minionsq);

            if (useW && W.Instance.IsReady() && minionsw.IsValidTarget() && (minionsw != null) &&
                (minionsw.Health < W.GetDamage(minionsw)))
                W.Cast(minionsw.Position);

            if ((minionse.MinionsHit >= minimumEMinions) && useE &&
                E.Instance.IsReady())
                E.Cast(minionse.Position);
        }

        private void JungleClear()
        {
            var useQ = Config.Item("JungleClearQ").GetValue<bool>();
            var useW = Config.Item("JungleClearW").GetValue<bool>();
            var useE = Config.Item("JungleClearE").GetValue<bool>();
            var useItems = Config.Item("JungleClearItems").GetValue<bool>();
            var jungleClearManaManager = Config.Item("JungleClearManaManager").GetValue<Slider>().Value;
            var minimumEJungleMobs = Config.Item("MinimumEJungleMobs").GetValue<Slider>().Value;

            var junglemobsq =
                Cache.GetMinions(Player.Position, Q.Range, SebbyLib.MinionTeam.Neutral)
                    .OrderByDescending(x => x.MaxHealth)
                    .FirstOrDefault();

            if (useItems)
            {
                if (Hydra.IsReady() && junglemobsq.IsValidTarget(Hydra.Range))
                    Hydra.Cast();

                if (Tiamat.IsReady() && junglemobsq.IsValidTarget(Tiamat.Range))
                    Tiamat.Cast();
            }

            if (Player.ManaPercent < jungleClearManaManager)
                return;

            var minionselist = Cache.GetMinions(Player.Position, E.Range, SebbyLib.MinionTeam.Neutral);
            var minionsw = minionselist.OrderByDescending(x => x.MaxHealth).FirstOrDefault();
            var minionse = E.GetCircularFarmLocation(minionselist);
            if (minionsw == null)
                return;

            if (Config.Item("JungleClearDontEQRange").GetValue<bool>() && (Player.Distance(minionse.Position) > Q.Range))
            {
                if ((minionse.MinionsHit >= minimumEJungleMobs) && useE && E.Instance.IsReady())
                    E.Cast(minionse.Position);
            }
            else if (!Config.Item("JungleClearDontEQRange").GetValue<bool>())
            {
                if ((minionse.MinionsHit >= minimumEJungleMobs) && useE && E.Instance.IsReady())
                    E.Cast(minionse.Position);
            }

            if (junglemobsq.IsValidTarget() && useQ && Q.Instance.IsReady())
                Q.CastOnUnit(junglemobsq);

            if (minionsw.IsValidTarget() && useW && W.Instance.IsReady())
                W.Cast(minionsw.Position);
        }

        private bool IsIsolated(AIBaseClient enemy)
        {
            return
                !ObjectManager.Get<AIBaseClient>()
                    .Any(
                        x =>
                            (x.NetworkId != enemy.NetworkId) && (x.Team == enemy.Team) && (x.Distance(enemy) <= 450) &&
                            ((x.Type == GameObjectType.AIHeroClient) || (x.Type == GameObjectType.AIMinionClient) ||
                             (x.Type == GameObjectType.AITurretClient)));
        }

        private double GetRealQDamage(AIBaseClient enemy)
        {
            if (Q.Range < 326)
                return 0.984 * Player.GetSpellDamage(enemy, SpellSlot.Q, 3);
            if (Q.Range > 325)
                return 0.984 * Player.GetSpellDamage(enemy, SpellSlot.Q, 3);
            return 0;
        }

        private float CalculateDamage(AIBaseClient enemy)
        {
            double damage = 0;

            if (Q.Instance.IsReady())
                damage += GetRealQDamage(enemy);

            if (E.Instance.IsReady())
                damage += E.GetDamage(enemy);

            if (W.Instance.IsReady())
                damage += W.GetDamage(enemy);

            if (Tiamat.IsReady())
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Tiamat);

            else if (Hydra.IsReady())
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.RavenousHydra);

            return (float)damage;
        }


    }
}

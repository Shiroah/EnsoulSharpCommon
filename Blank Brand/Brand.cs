namespace Blank_Brand
{
    using System;
    using System.Linq;
    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    using EnsoulSharp.SDK.Prediction;
    using EnsoulSharp.SDK.Utility;
    using Color = System.Drawing.Color;

    internal class Brand
    {
        private static Menu MainMenu;
        private static Spell Q, W, E, R;

        public static void OnLoad()
        {
            Q = new Spell(SpellSlot.Q, 1050f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 625f);
            R = new Spell(SpellSlot.R, 750f);

            Q.SetSkillshot(0.2f, 50f, 1550f, true, true, SkillshotType.Line);
            W.SetSkillshot(0.5f, 250, float.MaxValue, false, true, SkillshotType.Circle);

            MainMenu = new Menu(ObjectManager.Player.CharacterName, "Blank Brand", true);

            var combo = new Menu("Combo", "Combo Settings")
            {
                MenuWrapper.Combo.Q,
                MenuWrapper.Combo.Qstun,
                MenuWrapper.Combo.W,
                MenuWrapper.Combo.E,
                MenuWrapper.Combo.R
            };
            MainMenu.Add(combo);

            var harass = new Menu("Harass", "Harass/Poke Settings")
            {
                MenuWrapper.Harass.Q,
                MenuWrapper.Harass.W,
                MenuWrapper.Harass.E
            };
            MainMenu.Add(harass);

            var laneclear = new Menu("Laneclear", "Laneclearing Settings")
            {
                MenuWrapper.Laneclear.W,
                MenuWrapper.Laneclear.E,
                MenuWrapper.Laneclear.Elasthit
            };
            MainMenu.Add(laneclear);

            var jungleclear = new Menu("Jungle", "Jungle Clear Settings")
            {
                MenuWrapper.Jungleclear.Q,
                MenuWrapper.Jungleclear.W,
                MenuWrapper.Jungleclear.E
            };
            MainMenu.Add(jungleclear);

            var killsteal = new Menu("KillSteal", "KillSteal Settings")
            {
                MenuWrapper.Killsteal.Q,
                MenuWrapper.Killsteal.W,
                MenuWrapper.Killsteal.E,
                MenuWrapper.Killsteal.R
            };
            MainMenu.Add(killsteal);

            var draw = new Menu("Drawing", "Drawing Settings")
            {
                MenuWrapper.Drawing.Q,
                MenuWrapper.Drawing.W,
                MenuWrapper.Drawing.E,
                MenuWrapper.Drawing.R,
                MenuWrapper.Drawing.OnlyReady
            };
            MainMenu.Add(draw);

            var misc = new Menu("Misc", "Misc Settings")
            {
                MenuWrapper.Misc.Rppl
            };
            MainMenu.Add(misc);

            MainMenu.Attach();

            Game.OnUpdate += OnUpdate;
        }

        private static void Combo()
        {
            var qtarget = TargetSelector.GetTarget(Q.Range);
            if (qtarget == null || !qtarget.IsValidTarget(Q.Range))
            {
                return;
            }

            var wtarget = TargetSelector.GetTarget(W.Range);
            if (wtarget == null || !wtarget.IsValidTarget(W.Range))
            {
                return;
            }

            if (MenuWrapper.Combo.Q.Enabled && Q.IsReady() && !MenuWrapper.Combo.Qstun.Enabled)
            {
                var qPred = Q.GetPrediction(qtarget, false, -1, CollisionObjects.Minions | CollisionObjects.YasuoWall | CollisionObjects.Heroes);
                if (qPred.Hitchance >= HitChance.High)
                {
                    Q.Cast(qPred.CastPosition);
                }
            }

            if (MenuWrapper.Combo.Qstun.Enabled && Q.IsReady())
            {
                var qPred = Q.GetPrediction(qtarget, false, -1, CollisionObjects.Minions | CollisionObjects.YasuoWall | CollisionObjects.Heroes);
                if (qPred.Hitchance >= HitChance.High)
                foreach (var t in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(E.Range) && x.HasBuff("brandablaze"))) ;
                {
                    Q.Cast(qPred.CastPosition);
                }
            }

            if (MenuWrapper.Combo.W.Enabled && W.IsReady())
            {
                var wPred = W.GetPrediction(wtarget, false, 0);
                if (wPred.Hitchance >= HitChance.High)
                {
                    W.Cast(wPred.CastPosition);
                }
            }

            if (MenuWrapper.Combo.E.Enabled && E.IsReady())
            {
                E.Cast();
            }

            if (MenuWrapper.Combo.R.Enabled && R.IsReady())
            {
                R.Cast();
            }
        }
        private static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.IsRecalling())
            {
                return;
            }

            Orbwalker.AttackState = true;
            Orbwalker.MovementState = true;

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;
            }

        }
    }
}

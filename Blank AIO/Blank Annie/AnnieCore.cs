using EnsoulSharp;
using EnsoulSharp.Common;
using KoreanAnnie.Common;
using System.Linq;

namespace KoreanAnnie
{
    internal class AnnieCore : CommonOrbwalkImplementation
    {
        #region Constructors and Destructors

        public AnnieCore(Annie annie)
        {
            Champion = annie;
            this.annie = annie;
            spells = annie.Spells;

            Game.OnUpdate += UseSkills;
            Orbwalking.BeforeAttack += DisableAAToFarm;
        }

        #endregion Constructors and Destructors

        #region Fields

        private readonly Annie annie;

        private readonly CommonSpells spells;

        #endregion Fields

        #region Public Methods and Operators

        private bool CanAttack(AIHeroClient enemy)
        {
            if (enemy == null) return false;
            if (annie.GetParamBool("combo" + enemy.CharacterName.ToLowerInvariant()))
                return true;
            if (enemy.HealthPercent < 30)
                return true;
            if (annie.Player.GetEnemiesInRange(1100f).Count == 1)
                return true;
            return !HeroManager.Enemies.Where(x => x.Distance(Champion.Player) < 1100f && !x.IsDead && !x.IsZombie)
                .Any(x => x.CharacterName != enemy.CharacterName &&
                          annie.GetParamBool("combo" + x.CharacterName.ToLowerInvariant()));
        }

        public override void ComboMode()
        {
            var target = TargetSelector.GetTarget(spells.MaxRangeCombo, TargetSelector.DamageType.Magical);

            if (target == null || !CanAttack(target)) return;

            if (annie.GetParamBool("usertocombo") && spells.R.IsReady() && spells.R.CanCast() &&
                target.IsValidTarget(spells.R.Range) && !spells.CheckOverkill(target))
            {
                var minEnemiesToR = annie.GetParamSlider("minenemiestor");

                if (minEnemiesToR == 1 && annie.CheckStun())
                    spells.R.Cast(target.Position);
                else
                    foreach (var pred in ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(spells.R.Range))
                        .Select(x => spells.R.GetPrediction(x, true)).Where(pred =>
                            pred.Hitchance >= HitChance.High && pred.AoeTargetsHitCount >= minEnemiesToR))
                        spells.R.Cast(pred.CastPosition);
            }

            if (!annie.GetParamBool("supportmode") && spells.R.GetDamage(target) > target.Health + 50f &&
                spells.R.IsReady() && spells.R.CanCast() && spells.R.CanCast(target) &&
                !spells.CheckOverkill(target)) spells.R.Cast(target.Position);
            if (spells.W.IsReady() && annie.GetParamBool("usewtocombo") && target.IsValidTarget(spells.W.Range))
                spells.W.Cast(target.Position);
            if (spells.Q.IsReady() && annie.GetParamBool("useqtocombo") && target.IsValidTarget(spells.Q.Range))
                spells.Q.Cast(target);
        }

        public override void Ultimate()
        {
            spells.R.CastOnBestTarget();
        }

        #endregion Public Methods and Operators

        #region Methods

        protected override void HarasMode()
        {
            LastHitMode();
            Haras();
        }

        protected override void LaneClearMode()
        {
            if (!annie.SaveStun() && annie.CanFarm())
            {
                var manaLimitReached = annie.Player.ManaPercent < annie.GetParamSlider("manalimittolaneclear");

                if (annie.GetParamBool("useqtolaneclear") && spells.Q.IsReady())
                {
                    if (annie.GetParamBool("saveqtofarm"))
                    {
                        QFarmLogic();
                    }
                    else if (!manaLimitReached)
                    {
                        var minions = MinionManager.GetMinions(annie.Player.Position, spells.Q.Range, MinionTypes.All,
                            MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);

                        if (minions != null && minions.Count > 0) spells.Q.Cast(minions[0]);
                    }
                }

                if (!manaLimitReached)
                    if (annie.GetParamBool("usewtolaneclear") && spells.W.IsReady())
                    {
                        var minions = MinionManager.GetMinions(annie.Player.Position, spells.W.Range);

                        var wFarmLocation = spells.W.GetCircularFarmLocation(minions, spells.W.Width);

                        if (wFarmLocation.MinionsHit >= annie.GetParamSlider("minminionstow"))
                            spells.W.Cast(wFarmLocation.Position);
                    }
            }

            if (annie.GetParamBool("harasonlaneclear")) Haras();
        }

        protected override void LastHitMode()
        {
            if (!annie.SaveStun()) QFarmLogic();
        }

        private void DisableAAToFarm(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target is AIMinionClient &&
                (Champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear ||
                 Champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit ||
                 Champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed) && !annie.SaveStun() &&
                annie.CanFarm() && annie.GetParamBool("useqtofarm") &&
                annie.Player.Mana > annie.Spells.Q.Instance.ManaCost &&
                Champion.Spells.Q.IsKillable((AIBaseClient)args.Target) &&
                Champion.Spells.Q.Instance.CooldownExpires - Game.Time < 0.3f) args.Process = false;
        }

        private void Haras()
        {
            if (annie.Player.ManaPercent < annie.GetParamSlider("manalimittoharas")) return;

            var target = TargetSelector.GetTarget(spells.MaxRangeHaras, TargetSelector.DamageType.Magical);

            if (!CanAttack(target)) return;

            if (spells.Q.IsReady() && annie.GetParamBool("useqtoharas") && target.IsValidTarget(spells.Q.Range))
                spells.Q.Cast(target);

            if (spells.W.IsReady() && annie.GetParamBool("usewtoharas") && target.IsValidTarget(spells.W.Range))
                spells.W.Cast(target.Position);
        }

        private void QFarmLogic()
        {
            if (annie.SaveStun() || !annie.CanFarm() || !spells.Q.IsReady() ||
                !annie.GetParamBool("useqtofarm")) return;

            spells.Q.Cast(MinionManager
                .GetMinions(annie.Player.Position, spells.Q.Range, MinionTypes.All, MinionTeam.NotAlly,
                    MinionOrderTypes.MaxHealth).Where(x => spells.Q.IsKillable(x)).FirstOrDefault(minion =>
                    spells.Q.GetDamage(minion) > HealthPrediction.GetHealthPrediction(minion,
                        (int)(annie.Player.Distance(minion) / spells.Q.Speed) * 1000, (int)spells.Q.Delay * 1000)));
        }

        #endregion Methods
    }
}
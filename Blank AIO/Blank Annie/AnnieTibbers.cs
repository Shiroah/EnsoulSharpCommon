using EnsoulSharp;
using EnsoulSharp.Common;
using KoreanAnnie.Common;
using System;
using System.Linq;

namespace KoreanAnnie
{
    internal class AnnieTibbers
    {
        #region Constants

        private const float TibbersRange = 1500f;

        #endregion Constants

        #region Constructors and Destructors

        public AnnieTibbers(Annie annie)
        {
            this.annie = annie;

            GameObject.OnCreate += NewTibbers;
            GameObject.OnDelete += DeleteTibbers;
            Game.OnUpdate += ControlTibbers;
            Orbwalking.OnAttack += AttackTurrent;
            Game.OnUpdate += FlashTibbersLogic;
        }

        #endregion Constructors and Destructors

        #region Public Properties

        public AIBaseClient Tibbers { get; private set; }

        #endregion Public Properties

        #region Fields

        private readonly Annie annie;

        private AIBaseClient currentTarget;

        #endregion Fields

        #region Methods

        private static AIBaseClient GetBaronOrDragon()
        {
            var legendaryMonster = ObjectManager.Get<AIBaseClient>().Where(obj =>
                (obj.CharacterName.ToLowerInvariant() == "sru_dragon" ||
                 obj.CharacterName.ToLowerInvariant() == "sru_baron") && obj.IsVisible && obj.HealthPercent < 100 &&
                obj.HealthPercent > 0).ToList();

            return legendaryMonster.Count > 0 ? legendaryMonster[0] : null;
        }

        private AIBaseClient GetChampion()
        {
            var champ = TargetSelector.GetTarget(Tibbers, TibbersRange, TargetSelector.DamageType.Magical);

            return champ;
        }

        private static AIBaseClient GetJungleMob()
        {
            var jungleMob = ObjectManager.Get<AIBaseClient>().Where(obj =>
                (obj.CharacterName.ToLowerInvariant() == "sru_blue" ||
                 obj.CharacterName.ToLowerInvariant() == "sru_gromp" ||
                 obj.CharacterName.ToLowerInvariant() == "sru_murkwolf" ||
                 obj.CharacterName.ToLowerInvariant() == "sru_razorbeak" ||
                 obj.CharacterName.ToLowerInvariant() == "sru_red" ||
                 obj.CharacterName.ToLowerInvariant() == "sru_krug") && obj.IsVisible && obj.HealthPercent < 100 &&
                obj.HealthPercent > 0 && obj.IsVisible).ToList();

            return jungleMob.Count > 0 ? jungleMob[0] : null;
        }

        private static bool IsTibbers(GameObject sender)
        {
            return sender != null && sender.IsValid && sender.Name.ToLowerInvariant().Equals("tibbers") &&
                   sender.IsAlly;
        }

        private void AttackTurrent(AttackableUnit unit, AttackableUnit target)
        {
            if (Tibbers != null && Tibbers.IsValid && unit.IsMe && target is AITurretClient)
                currentTarget = (AIBaseClient)target;
        }

        private void ControlTibbers(EventArgs args)
        {
            if (Tibbers == null || !Tibbers.IsValid || !annie.GetParamBool("autotibbers")) return;

            var target = FindTarget();

            if (target != null)
            {
                //Method bugged == plz fix the Common
                //annie.EloBuddy.Player.IssueOrder(
                //    Tibbers.Distance(target.Position) > 200 ? GameObjectOrder.MovePet : GameObjectOrder.AutoAttackPet,
                //    target);

                if (Tibbers.Distance(target.Position) > 200)
                    Player.IssueOrder(GameObjectOrder.MoveTo, target);
                else
                    Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }
        }

        private void DeleteTibbers(GameObject sender, EventArgs args)
        {
            if (IsTibbers(sender)) Tibbers = null;
        }

        private AIBaseClient FindTarget()
        {
            var target = GetChampion();

            if (target != null) return target;

            target = GetBaronOrDragon();

            if (target != null) return target;

            target = GetJungleMob();

            if (target != null) return target;

            target = GetMinion();

            if (target != null) return target;

            if (currentTarget != null && currentTarget.IsValidTarget(annie.Player.AttackRange + 200f))
                return currentTarget;
            currentTarget = null;

            return annie.Player;
        }

        private void FlashTibbersLogic(EventArgs args)
        {
            if (!annie.GetParamKeyBind("flashtibbers")) return;

            if (annie.Spells.R.IsReady() && FlashSpell.IsReady(annie.Player) && annie.CheckStun())
            {
                var minToCast = annie.GetParamSlider("minenemiestoflashr");

                if (minToCast > 1)
                {
                    foreach (var pred in ObjectManager.Get<AIHeroClient>()
                        .Where(x => x.IsValidTarget(annie.Spells.RFlash.Range))
                        .Select(x => annie.Spells.RFlash.GetPrediction(x, true)).Where(pred =>
                            pred.Hitchance >= HitChance.High && pred.AoeTargetsHitCount >= minToCast))
                    {
                        var pred1 = pred;
                        annie.Player.Spellbook.CastSpell(FlashSpell.Slot(annie.Player), pred1.CastPosition);
                        Utility.DelayAction.Add(10, () => annie.Spells.R.Cast(pred1.CastPosition));
                    }
                }
                else
                {
                    var target = TargetSelector.GetTarget(annie.Spells.RFlash.Range, TargetSelector.DamageType.Magical);
                    if (target != null)
                    {
                        annie.Player.Spellbook.CastSpell(FlashSpell.Slot(annie.Player), target.Position);
                        Utility.DelayAction.Add(50, () => annie.Spells.R.Cast(target.Position));
                    }
                }
            }

            if (annie.GetParamBool("orbwalktoflashtibbers"))
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPosCenter);
            annie.AnnieOrbwalker.ComboMode();
        }

        private AIBaseClient GetMinion()
        {
            var minion = MinionManager.GetMinions(Tibbers.Position, TibbersRange)
                .OrderBy(x => x.Distance(Tibbers.Position)).ToList();

            return minion.Count > 0 ? minion[0] : null;
        }

        private void NewTibbers(GameObject sender, EventArgs args)
        {
            if (IsTibbers(sender)) Tibbers = (AIBaseClient)sender;
        }

        #endregion Methods
    }
}
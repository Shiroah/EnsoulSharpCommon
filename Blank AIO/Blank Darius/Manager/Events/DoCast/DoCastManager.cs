using EnsoulSharp;
using EnsoulSharp.Common;
using Flowers_Darius.Manager.Spells;
using myCommon;
using System.Linq;

namespace Flowers_Darius.Manager.Events
{
    internal class DoCastManager : Logic
    {
        internal static void Init(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || Args.SData == null || !Orbwalking.IsAutoAttack(Args.SData.Name) ||
                Args.Target == null) return;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
                Args.Target.Type == GameObjectType.AIHeroClient)
            {
                SpellManager.CastItem();

                var target = (AIHeroClient)Args.Target;

                if (target != null && !target.IsDead && !target.IsZombie)
                    if (Menu.GetBool("ComboW") && W.IsReady() && Orbwalking.InAutoAttackRange(target))
                    {
                        W.Cast();
                        Orbwalker.ForceTarget(target);
                    }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed &&
                Args.Target.Type == GameObjectType.AIHeroClient)
            {
                SpellManager.CastItem();

                var target = (AIHeroClient)Args.Target;

                if (target != null && !target.IsDead && !target.IsZombie)
                    if (Menu.GetBool("HarassW") && W.IsReady() && Orbwalking.InAutoAttackRange(target))
                    {
                        W.Cast();
                        Orbwalker.ForceTarget(target);
                    }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear(Args);
                JungleClear(Args);
            }
        }

        private static void LaneClear(AIBaseClientProcessSpellCastEventArgs Args)
        {
            if (Args.SData == null || !Orbwalking.IsAutoAttack(Args.SData.Name) || Args.Target == null ||
                Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear || !Args.Target.IsEnemy ||
                !ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearMana")) || !ManaManager.SpellFarm)
                return;

            if (Menu.GetBool("LaneClearW") && W.IsReady())
            {
                if (Args.Target.Type == GameObjectType.AITurretClient || Args.Target.Type == GameObjectType.obj_Turret)
                {
                    if (!Args.Target.IsDead) W.Cast();
                }
                else
                {
                    var minion = (AIMinionClient)Args.Target;

                    if (minion != null && minion.Health <= DamageCalculate.GetWDamage(minion))
                    {
                        W.Cast();
                        Orbwalker.ForceTarget(minion);
                    }
                }
            }
        }

        private static void JungleClear(AIBaseClientProcessSpellCastEventArgs Args)
        {
            if (Args.SData == null || !Orbwalking.IsAutoAttack(Args.SData.Name) || Args.Target == null ||
                Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear ||
                Args.Target.Type != GameObjectType.AIMinionClient ||
                !ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) || !ManaManager.SpellFarm)
                return;

            var mobs = MinionManager.GetMinions(Orbwalking.GetRealAutoAttackRange(Me), MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var mob = mobs.FirstOrDefault();

            if (mob != null)
            {
                SpellManager.CastItem();

                if (Menu.GetBool("JungleClearW") && W.IsReady() && Orbwalking.InAutoAttackRange(Me)) W.Cast(true);
            }
        }
    }
}
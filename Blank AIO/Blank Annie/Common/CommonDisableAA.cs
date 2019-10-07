using EnsoulSharp;
using EnsoulSharp.Common;

namespace KoreanAnnie.Common
{
    internal class CommonDisableAA
    {
        private readonly ICommonChampion champion;

        public CommonDisableAA(ICommonChampion champion)
        {
            this.champion = champion;

            Orbwalking.BeforeAttack += CancelAA;
        }

        private CommonDisableAAMode Mode =>
            (CommonDisableAAMode)KoreanUtils.GetParamStringList(champion.MainMenu, "disableaa");

        private bool CanUseAA()
        {
            var canHit = true;

            if (KoreanUtils.GetParam(champion.MainMenu, "supportmode") != null)
                if (KoreanUtils.GetParamBool(champion.MainMenu, "supportmode") &&
                    champion.Player.CountAlliesInRange(1500f) > 1)
                    canHit = false;
            return canHit;
        }

        private void CancelAA(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target == null) return;

            if (champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                switch (Mode)
                {
                    case CommonDisableAAMode.Always:
                        args.Process = false;
                        break;

                    case CommonDisableAAMode.Never:
                        args.Process = true;
                        break;

                    case CommonDisableAAMode.SomeSkillReady:
                        if (champion.Spells.SomeSkillReady()) args.Process = false;
                        break;

                    case CommonDisableAAMode.HarasComboReady:
                        if (champion.Spells.HarasReady()) args.Process = false;
                        break;

                    case CommonDisableAAMode.FullComboReady:
                        if (champion.Spells.ComboReady()) args.Process = false;
                        break;
                }
            }
            else
            {
                if (args.Target is AIBaseClient && ((AIBaseClient)args.Target).IsMinion && !CanUseAA())
                    args.Process = false;
            }
        }
    }
}
using ElLeeSin.Components;
using ElLeeSin.Components.SpellManagers;
using ElLeeSin.Utilities;
using EnsoulSharp;
using EnsoulSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElLeeSin.Kurisu
{
    internal class Bubba
    {
        public List<AIHeroClient> HeroesOnSegment = new List<AIHeroClient>();
        public AIHeroClient HeroToKick;
        public Vector2 KickPos;
        public AIHeroClient TargetHero;
    }

    internal class BubbaSharp
    {
        public static void Drawing_OnDraw(EventArgs args)
        {
            if (BubbaFat != null && MyMenu.Menu.Item("bSharpOn").GetValue<KeyBind>().Active)
            {
                var kickLineMenu = MyMenu.Menu.Item("drawKickLine").GetValue<Circle>();
                if (kickLineMenu.Active)
                {
                    var start = Drawing.WorldToScreen(BubbaFat.KickPos.To3D());
                    var end = Drawing.WorldToScreen(BubbaFat.TargetHero.Position);

                    Drawing.DrawLine(start, end, 6, kickLineMenu.Color);
                }

                var kickPosMenu = MyMenu.Menu.Item("drawKickPos").GetValue<Circle>();
                if (kickPosMenu.Active) Render.Circle.DrawCircle(BubbaFat.KickPos.To3D(), 100f, kickPosMenu.Color, 6);

                var kickTargetMenu = MyMenu.Menu.Item("drawKickTarget").GetValue<Circle>();
                if (kickTargetMenu.Active)
                    Render.Circle.DrawCircle(BubbaFat.TargetHero.Position, 100f, kickTargetMenu.Color, 3);
            }
        }

        public static void BubbKushGo(AIHeroClient target)
        {
            var posChecked = 0;
            var maxPosToCheck = 50;
            var posRadius = 50;
            var radiusIndex = 0;

            var bubba = new Bubba();
            var bubbaList = new List<Bubba>();

            while (posChecked < maxPosToCheck)
            {
                radiusIndex++;
                var curRadius = radiusIndex * 2 * posRadius;
                var curCurcleChecks = (int)Math.Ceiling(2 * Math.PI * curRadius / (2 * (double)posRadius));

                for (var i = 1; i < curCurcleChecks; i++)
                {
                    posChecked++;

                    var cRadians = 0x2 * Math.PI / (curCurcleChecks - 1) * i;
                    var startPos = new Vector2((float)Math.Floor(target.Position.X + curRadius *
                                                                  Math.Cos(cRadians)),
                        (float)Math.Floor(target.Position.Y + curRadius * Math.Sin(cRadians)));

                    var endPos = startPos.Extend(target.Position.To2D(), 1000f);
                    var targetProj = target.Position.To2D().ProjectOn(startPos, endPos);

                    foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget()))
                        if (hero.NetworkId != target.NetworkId && hero.Distance(targetProj.SegmentPoint) <= 1000)
                        {
                            var mPos = Prediction.GetPrediction(hero, 250 + Game.Ping / 2).UnitPosition.To2D();
                            var mProj = mPos.ProjectOn(startPos, endPos);
                            if (mProj.IsOnSegment &&
                                mProj.SegmentPoint.Distance(hero.Position) <= hero.BoundingRadius + 100)
                                if (bubba.HeroesOnSegment.Contains(hero) == false)
                                {
                                    bubba.HeroToKick = hero;
                                    bubba.TargetHero = target;
                                    bubba.KickPos = hero.Position.To2D().Extend(startPos, -(hero.BoundingRadius + 35));
                                    bubba.HeroesOnSegment.Add(hero);
                                }
                        }

                    bubbaList.Add(bubba);

                    BubbaFat =
                        bubbaList.Where(x => x.HeroesOnSegment.Count > 0)
                            .OrderByDescending(x => x.HeroesOnSegment.Count)
                            .ThenByDescending(x => x.HeroToKick.MaxHealth).FirstOrDefault();

                    if (BubbaFat != null)
                    {
                        if (!LeeSin.spells[LeeSin.Spells.R].IsReady()) return;

                        if (BubbaFat.KickPos.To3D().IsValid()
                            && BubbaFat.KickPos.Distance(ObjectManager.Player) < LeeSin.spells[LeeSin.Spells.W].Range
                            && Wardmanager.FindBestWardItem() != null
                            && Misc.IsWOne)
                        {
                            Wardmanager.WardJump(BubbaFat.KickPos.To3D(), false, true, true);
                            LeeSin.spells[LeeSin.Spells.R].CastOnUnit(BubbaFat.HeroToKick);
                        }

                        if (MyMenu.Menu.Item("bubbaflash").IsActive() && Wardmanager.FindBestWardItem() == null &&
                            BubbaFat.KickPos.To3D().IsValid()
                            && BubbaFat.KickPos.Distance(ObjectManager.Player) < 425)
                            if (ObjectManager.Player.Spellbook.CanUseSpell(LeeSin.flashSlot) == SpellState.Ready &&
                                Wardmanager.LastWard + 1000 < Environment.TickCount)
                            {
                                ObjectManager.Player.Spellbook.CastSpell(LeeSin.flashSlot, BubbaFat.KickPos.To3D());
                                LeeSin.spells[LeeSin.Spells.R].CastOnUnit(BubbaFat.HeroToKick);
                            }
                    }
                }
            }
        }

        #region Static Fields

        internal static Bubba BubbaFat;
        internal static SpellSlot Flash;

        #endregion Static Fields
    }
}
using ElLeeSin.Utilities;
using EnsoulSharp;
using EnsoulSharp.Common;
using SharpDX;
using System;
using System.Linq;

namespace ElLeeSin.Components.SpellManagers
{
    internal class Wardmanager
    {
        #region Public Methods and Operators

        public static Vector2 JumpPos;

        public static bool reCheckWard = true;

        public static int Wcasttime;

        public static Vector3 lastWardPos;

        public static float LastWard;

        public static bool castWardAgain = true;

        public enum WCastStage
        {
            First,

            Second,

            Cooldown
        }

        private static SpellDataInstClient GetItemSpell(InventorySlot invSlot)
        {
            return ObjectManager.Player.Spellbook.Spells.FirstOrDefault(spell => (int)spell.Slot == invSlot.Slot + 4);
        }

        public static InventorySlot FindBestWardItem()
        {
            try
            {
                var slot = Items.GetWardSlot();
                if (slot == default(InventorySlot)) return null;

                var sdi = GetItemSpell(slot);
                if (sdi != default(SpellDataInstClient) && sdi.State == SpellState.Ready) return slot;
                return slot;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        public static void WardJump(
            Vector3 pos,
            bool m2M = true,
            bool maxRange = false,
            bool reqinMaxRange = false,
            bool minions = true,
            bool champions = true)
        {
            if (Misc.WStage != WCastStage.First) return;

            var basePos = EnsoulSharp.Common.Geometry.To2D(ObjectManager.Player.Position);
            var newPos = EnsoulSharp.Common.Geometry.To2D(pos) -
                         EnsoulSharp.Common.Geometry.To2D(ObjectManager.Player.Position);

            if (JumpPos == new Vector2())
            {
                if (reqinMaxRange)
                    JumpPos = EnsoulSharp.Common.Geometry.To2D(pos);
                else if (maxRange || EnsoulSharp.Common.Geometry.Distance(ObjectManager.Player, pos) > 590)
                    JumpPos = basePos + EnsoulSharp.Common.Geometry.Normalized(newPos) * 590;
                else
                    JumpPos = basePos + EnsoulSharp.Common.Geometry.Normalized(newPos) *
                              EnsoulSharp.Common.Geometry.Distance(ObjectManager.Player, pos);
            }

            if (JumpPos != new Vector2() && reCheckWard)
            {
                reCheckWard = false;
                Utility.DelayAction.Add(
                    20,
                    () =>
                    {
                        if (JumpPos != new Vector2())
                        {
                            JumpPos = new Vector2();
                            reCheckWard = true;
                        }
                    });
            }

            if (m2M) Misc.Orbwalk(pos);
            if (!LeeSin.spells[LeeSin.Spells.W].IsReady() || Misc.WStage != WCastStage.First
                                                          || reqinMaxRange &&
                                                          EnsoulSharp.Common.Geometry.Distance(ObjectManager.Player,
                                                              pos) > LeeSin.spells[LeeSin.Spells.W].Range)
                return;

            if (minions || champions)
            {
                if (champions)
                {
                    var wardJumpableChampion =
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                x =>
                                    x.IsAlly && EnsoulSharp.Common.Geometry.Distance(x, ObjectManager.Player) <
                                             LeeSin.spells[LeeSin.Spells.W].Range
                                             && EnsoulSharp.Common.Geometry.Distance(x, pos) < 200 && !x.IsMe)
                            .OrderByDescending(i => EnsoulSharp.Common.Geometry.Distance(i, ObjectManager.Player))
                            .ToList()
                            .FirstOrDefault();

                    if (wardJumpableChampion != null && Misc.WStage == WCastStage.First)
                    {
                        if (500 >= Utils.TickCount - Wcasttime || Misc.WStage != WCastStage.First) return;

                        LeeSin.CastW(wardJumpableChampion);
                        return;
                    }
                }

                if (minions)
                {
                    var wardJumpableMinion =
                        ObjectManager.Get<AIMinionClient>()
                            .Where(
                                m =>
                                    m.IsAlly && EnsoulSharp.Common.Geometry.Distance(m, ObjectManager.Player) <
                                             LeeSin.spells[LeeSin.Spells.W].Range
                                             && EnsoulSharp.Common.Geometry.Distance(m, pos) < 200 &&
                                             !m.Name.ToLower().Contains("ward"))
                            .OrderByDescending(i => EnsoulSharp.Common.Geometry.Distance(i, ObjectManager.Player))
                            .ToList()
                            .FirstOrDefault();

                    if (wardJumpableMinion != null && Misc.WStage == WCastStage.First)
                    {
                        if (500 >= Utils.TickCount - Wcasttime || Misc.WStage != WCastStage.First) return;

                        LeeSin.CastW(wardJumpableMinion);
                        return;
                    }
                }
            }

            var isWard = false;

            var wardObject =
                ObjectManager.Get<AIBaseClient>()
                    .Where(o => o.IsAlly && o.Name.ToLower().Contains("ward") &&
                                EnsoulSharp.Common.Geometry.Distance(o, JumpPos) < 200)
                    .ToList()
                    .FirstOrDefault();

            if (wardObject != null)
            {
                isWard = true;
                if (500 >= Utils.TickCount - Wcasttime || Misc.WStage != WCastStage.First) return;

                LeeSin.CastW(wardObject);
            }

            if (!isWard && castWardAgain)
            {
                var ward = FindBestWardItem();
                if (ward == null) return;

                if (LeeSin.spells[LeeSin.Spells.W].IsReady() && Misc.IsWOne && LastWard + 400 < Utils.TickCount)
                {
                    ObjectManager.Player.Spellbook.CastSpell(ward.SpellSlot, EnsoulSharp.Common.Geometry.To3D(JumpPos));
                    lastWardPos = EnsoulSharp.Common.Geometry.To3D(JumpPos);
                    LastWard = Utils.TickCount;
                }
            }
        }

        public static void WardjumpToMouse()
        {
            WardJump(
                Game.CursorPosRaw,
                Misc.GetMenuItem("ElLeeSin.Wardjump.Mouse"),
                false,
                false,
                Misc.GetMenuItem("ElLeeSin.Wardjump.Minions"),
                Misc.GetMenuItem("ElLeeSin.Wardjump.Champions"));
        }

        #endregion Public Methods and Operators
    }
}
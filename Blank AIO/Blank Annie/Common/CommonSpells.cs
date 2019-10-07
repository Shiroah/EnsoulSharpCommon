using EnsoulSharp;
using EnsoulSharp.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KoreanAnnie.Common
{
    internal class CommonSpells : IEnumerable
    {
        #region Constructors and Destructors

        public CommonSpells(ICommonChampion player)
        {
            champion = player;

            spellList = new List<CommonSpell>();
        }

        #endregion Constructors and Destructors

        #region Fields

        private readonly ICommonChampion champion;

        private readonly List<CommonSpell> spellList;

        private CommonSpell rflash;

        #endregion Fields

        #region Public Properties

        public CommonSpell E
        {
            get { return spellList.First(x => x.Slot == SpellSlot.E); }
        }

        public float MaxRangeCombo
        {
            get
            {
                var range = 0f;
                if (spellList != null && spellList.Count > 0)
                {
                    var ultimate = spellList.First(x => x.Slot == SpellSlot.R);
                    if (ultimate != null && ultimate.UseOnCombo && ultimate.IsReady() && ultimate.CanCast())
                    {
                        range = ultimate.Range;
                    }
                    else
                    {
                        var spells = spellList
                            .Where(x => x.Slot != SpellSlot.R && x.UseOnCombo && x.IsReady() && x.CanCast()).ToList();
                        if (spells.Count > 0)
                            foreach (var spell in spells)
                                range = Math.Max(spell.Range, range);
                    }
                }

                return range;
            }
        }

        public float MaxRangeHaras
        {
            get
            {
                var range = 0f;

                if (spellList != null && spellList.Count > 0)
                {
                    var spells = spellList.Where(x => x.UseOnHaras && x.IsReady() && x.CanCast()).ToList();
                    if (spells.Count > 0)
                        foreach (var spell in spells)
                            range = Math.Max(spell.Range, range);
                }

                return range;
            }
        }

        public CommonSpell Q
        {
            get { return spellList.First(x => x.Slot == SpellSlot.Q); }
        }

        public CommonSpell R
        {
            get { return spellList.First(x => x.Slot == SpellSlot.R); }
        }

        public CommonSpell RFlash
        {
            get
            {
                if (rflash == null && R != null)
                {
                    rflash = new CommonSpell(R.Slot, R.Range + 390, R.DamageType);

                    if (R.IsSkillshot)
                        rflash.SetSkillshot(R.Delay, R.Width, R.Speed, R.Collision, R.Type, R.From, R.RangeCheckFrom);
                }

                return rflash;
            }
        }

        public CommonSpell W
        {
            get { return spellList.First(x => x.Slot == SpellSlot.W); }
        }

        #endregion Public Properties

        #region Public Methods and Operators

        public void AddSpell(CommonSpell spell)
        {
            var slot = char.MinValue;

            switch (spell.Slot)
            {
                case SpellSlot.Q:
                    slot = 'q';
                    break;

                case SpellSlot.W:
                    slot = 'w';
                    break;

                case SpellSlot.E:
                    slot = 'e';
                    break;

                case SpellSlot.R:
                    slot = 'r';
                    break;
            }

            spell.UseOnComboMenu = KoreanUtils.GetParam(champion.MainMenu, string.Format("use{0}tocombo", slot));
            spell.UseOnHarasMenu = KoreanUtils.GetParam(champion.MainMenu, string.Format("use{0}toharas", slot));
            spell.UseOnLaneClearMenu =
                KoreanUtils.GetParam(champion.MainMenu, string.Format("use{0}tolaneclear", slot));
            spellList.Add(spell);
        }

        public bool CheckOverkill(AIHeroClient target)
        {
            var totalDamage = 0f;

            if (R.IsReady() && R.CanCast() && target.IsValidTarget(R.Range))
            {
                var spells = spellList.Where(x =>
                    x.CanCast() && x.IsReady() && target.IsValidTarget(x.Range) && x.IsKillable(target) &&
                    x.Slot != SpellSlot.R).ToList();

                if (spells.Count > 0) totalDamage = spells.Sum(spell => spell.GetDamage(target));
            }

            return totalDamage > target.Health;
        }

        public bool ComboReady()
        {
            return spellList.Count(x => x.UseOnCombo && x.IsReady() && x.CanCast()) ==
                   spellList.Count(x => x.UseOnCombo);
        }

        public IEnumerator GetEnumerator()
        {
            return spellList.GetEnumerator();
        }

        public bool HarasReady()
        {
            return spellList.Count(x => x.UseOnHaras && x.IsReady() && x.CanCast()) ==
                   spellList.Count(x => x.UseOnHaras);
        }

        public float MaxComboDamage(AIHeroClient target)
        {
            var damage = 0f;

            if (spellList != null && spellList.Count > 0)
            {
                var spells = spellList.Where(x => x.UseOnCombo && x.IsReady() && x.CanCast()).ToList();
                if (spells.Count > 0) damage = spells.Sum(spell => spell.GetDamage(target));
            }

            return damage;
        }

        public void RemoveSpell(CommonSpell spell)
        {
            spellList.Remove(spell);
        }

        public bool SomeSkillReady()
        {
            return spellList.Count(x => x.Range > 0 && x.IsReady() && x.CanCast()) > 0;
        }

        #endregion Public Methods and Operators
    }
}
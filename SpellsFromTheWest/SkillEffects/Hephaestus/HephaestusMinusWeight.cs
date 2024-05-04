using Config;
using GameData.Domains.CombatSkill;
using GameData.Domains.Map;
using GameData.Domains.SpecialEffect.SpellsFromTheWest.Hephaestus;
using SpellsFromTheWestBackend.SkillEffects.Hephaestus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameData.Domains.SpecialEffect.SpellsFromTheWest.Hephaestus
{
    internal class HephaestusMinusWeight : HephaestusBoilerPlateCuiPo
    {
        public HephaestusMinusWeight() { }
        public HephaestusMinusWeight(CombatSkillKey skillKey, sbyte direction) : base(skillKey, 94116, direction) { }
        public override bool DoTemplateReforgeDirect(WeaponItem weaponConfig)
        {
            if (weaponConfig.BaseWeight <= 16) return false;

            typeof(WeaponItem).GetField("BaseWeight").SetValue(weaponConfig, (int)(weaponConfig.BaseWeight - 15));
            return true;
        }

        public override bool DoTemplateReforgeIndirect(WeaponItem weaponConfig)
        {
            typeof(WeaponItem).GetField("BaseWeight").SetValue(weaponConfig, (int)(weaponConfig.BaseWeight - 15));

            return true;
        }

        public override string GetIdentifierDirect()
        {
            return "C";
        }

        public override string GetIdentifierIndirect()
        {
            return "c";
        }

        public override List<Tuple<string, Type>> GetInfluencedFields()
        {
            return new List<Tuple<string, Type>> { Tuple.Create("BaseWeight", typeof(int))};
        }
    }
}

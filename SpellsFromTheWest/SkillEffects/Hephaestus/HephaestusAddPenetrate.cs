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
    internal class HephaestusAddPenetrate : HephaestusBoilerPlateCuiPo
    {
        public HephaestusAddPenetrate() { }
        public HephaestusAddPenetrate(CombatSkillKey skillKey, sbyte direction) : base(skillKey, 94115, direction) { }
        public override bool DoTemplateReforgeDirect(WeaponItem weaponConfig)
        {
            typeof(WeaponItem).GetField("BasePenetrationFactor").SetValue(weaponConfig, (short)(weaponConfig.BasePenetrationFactor + 10));
            return true;
        }

        public override bool DoTemplateReforgeIndirect(WeaponItem weaponConfig)
        {
            if (weaponConfig.BasePenetrationFactor < 17) return false;
            typeof(WeaponItem).GetField("BasePenetrationFactor").SetValue(weaponConfig, (short)(weaponConfig.BasePenetrationFactor - 10));

            return true;
        }

        public override string GetIdentifierDirect()
        {
            return "B";
        }

        public override string GetIdentifierIndirect()
        {
            return "b";
        }

        public override List<Tuple<string, Type>> GetInfluencedFields()
        {
            return new List<Tuple<string, Type>> { Tuple.Create("BasePenetrationFactor", typeof(short))};
        }
    }
}

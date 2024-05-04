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
    internal class HephaestusAddPersue : HephaestusBoilerPlateCuiPo
    {
        public HephaestusAddPersue() { }
        public HephaestusAddPersue(CombatSkillKey skillKey, sbyte direction) : base(skillKey, 94117, direction) { }
        public override bool DoTemplateReforgeDirect(WeaponItem weaponConfig)
        {
            typeof(WeaponItem).GetField("PursueAttackFactor").SetValue(weaponConfig, (short)(weaponConfig.PursueAttackFactor + 5));
            return true;
        }

        public override bool DoTemplateReforgeIndirect(WeaponItem weaponConfig)
        {
            if (weaponConfig.PursueAttackFactor < 6) return false;
            typeof(WeaponItem).GetField("PursueAttackFactor").SetValue(weaponConfig, (short)(weaponConfig.PursueAttackFactor - 5));

            return true;
        }

        public override string GetIdentifierDirect()
        {
            return "D";
        }

        public override string GetIdentifierIndirect()
        {
            return "d";
        }

        public override List<Tuple<string, Type>> GetInfluencedFields()
        {
            return new List<Tuple<string, Type>> { Tuple.Create("PursueAttackFactor", typeof(short))};
        }
    }
}

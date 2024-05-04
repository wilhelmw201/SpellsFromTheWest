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
    internal class HephaestusAddPojia : HephaestusBoilerPlateCuiPo
    {
        public HephaestusAddPojia() { }
        public HephaestusAddPojia(CombatSkillKey skillKey, sbyte direction) : base(skillKey, 94118, direction) { }
        public override bool DoTemplateReforgeDirect(WeaponItem weaponConfig)
        {
            typeof(WeaponItem).GetField("BaseEquipmentAttack").SetValue(weaponConfig, (short)(weaponConfig.BaseEquipmentAttack + 100));
            return true;
        }

        public override bool DoTemplateReforgeIndirect(WeaponItem weaponConfig)
        {
            if (weaponConfig.PursueAttackFactor < 101) return false;
            typeof(WeaponItem).GetField("BaseEquipmentAttack").SetValue(weaponConfig, (short)(weaponConfig.BaseEquipmentAttack - 100));

            return true;
        }

        public override string GetIdentifierDirect()
        {
            return "E";
        }

        public override string GetIdentifierIndirect()
        {
            return "d";
        }

        public override List<Tuple<string, Type>> GetInfluencedFields()
        {
            return new List<Tuple<string, Type>> { Tuple.Create("BaseEquipmentAttack", typeof(short))};
        }
    }
}

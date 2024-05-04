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
    internal class HephaestusAddStance : HephaestusBoilerPlateCuiPo
    {
        public HephaestusAddStance() { }
        public HephaestusAddStance(CombatSkillKey skillKey, sbyte direction) : base(skillKey, 94120, direction) { }
        public override bool DoTemplateReforgeDirect(WeaponItem weaponConfig)
        {

            typeof(WeaponItem).GetField("StanceIncrement").SetValue(weaponConfig, (short)(weaponConfig.StanceIncrement + 20));
            return true;
        }

        public override bool DoTemplateReforgeIndirect(WeaponItem weaponConfig)
        {
            if (weaponConfig.StanceIncrement <= 30) return false;
            typeof(WeaponItem).GetField("StanceIncrement").SetValue(weaponConfig, (short)(weaponConfig.StanceIncrement - 20));

            return true;
        }

        public override string GetIdentifierDirect()
        {
            return "G";
        }

        public override string GetIdentifierIndirect()
        {
            return "g";
        }

        public override List<Tuple<string, Type>> GetInfluencedFields()
        {
            return new List<Tuple<string, Type>> { Tuple.Create("StanceIncrement", typeof(short))};
        }
    }
}

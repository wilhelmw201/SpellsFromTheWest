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
    internal class HephaestusAddBianzhao : HephaestusBoilerPlateCuiPo
    {
        public HephaestusAddBianzhao() { }
        public HephaestusAddBianzhao(CombatSkillKey skillKey, sbyte direction) : base(skillKey, 94119, direction) { }
        public override bool DoTemplateReforgeDirect(WeaponItem weaponConfig)
        {
            typeof(WeaponItem).GetField("ChangeTrickPercent").SetValue(weaponConfig, (short)(weaponConfig.ChangeTrickPercent + 16));
            return true;
        }

        public override bool DoTemplateReforgeIndirect(WeaponItem weaponConfig)
        {
            if (weaponConfig.PursueAttackFactor < 17) return false;
            typeof(WeaponItem).GetField("ChangeTrickPercent").SetValue(weaponConfig, (short)(weaponConfig.ChangeTrickPercent - 16));

            return true;
        }

        public override string GetIdentifierDirect()
        {
            return "F";
        }

        public override string GetIdentifierIndirect()
        {
            return "f";
        }

        public override List<Tuple<string, Type>> GetInfluencedFields()
        {
            return new List<Tuple<string, Type>> { Tuple.Create("ChangeTrickPercent", typeof(short))};
        }
    }
}

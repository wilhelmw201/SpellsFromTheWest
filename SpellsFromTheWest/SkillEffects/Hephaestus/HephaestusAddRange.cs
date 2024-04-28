using Config;
using GameData.Domains.CombatSkill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellsFromTheWestBackend.SkillEffects.Hephaestus
{
    internal class HephaestusAddRange : HephaestusBoilerPlateCuiPo
    {

        public HephaestusAddRange(CombatSkillKey skillKey) : base(skillKey, 94112, -1) { }
        public override bool DoTemplateReforgeDirect(WeaponItem weaponConfig)
        {
            if (HephaestusCommon.GetReforgeTimes(weaponConfig.TemplateId) >= 10) return false;
            
            typeof(WeaponItem).GetField("MinDistance").SetValue(weaponConfig, Math.Clamp(weaponConfig.MinDistance - 2, (short)2, (short)12));
            typeof(WeaponItem).GetField("MaxDistance").SetValue(weaponConfig, Math.Clamp(weaponConfig.MaxDistance + 2, (short)2, (short)12));

            return true;
        }

        public override bool DoTemplateReforgeIndirect(WeaponItem weaponConfig)
        {
            if (HephaestusCommon.GetReforgeTimes(weaponConfig.TemplateId) >= 10) return false;
                    
            if (weaponConfig.MaxDistance - weaponConfig.MinDistance < 10) return false;

            typeof(WeaponItem).GetField("MaxDistance").SetValue(weaponConfig, Math.Clamp(weaponConfig.MaxDistance - 2, (short)2, (short)12));
            typeof(WeaponItem).GetField("MinDistance").SetValue(weaponConfig, Math.Clamp(weaponConfig.MinDistance + 2, (short)2, (short)12));

            return true;
        }

        public override string GetIdentifierDirect()
        {
            return "A";
        }

        public override string GetIdentifierIndirect()
        {
            return "A";
        }

        public override List<string> GetInfluencedFields()
        {
            return new List<string>() { "MinDistance", "MaxDistance" };
        }
    }
}

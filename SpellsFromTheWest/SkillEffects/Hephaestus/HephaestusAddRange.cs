using Config;
using GameData.Domains.CombatSkill;
using GameData.Domains.SpecialEffect.SpellsFromTheWest.Hephaestus;
using SpellsFromTheWestBackend.SkillEffects.Hephaestus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameData.Domains.SpecialEffect.SpellsFromTheWest.Hephaestus
{
    internal class HephaestusAddRange : HephaestusBoilerPlateCuiPo
    {
        public HephaestusAddRange() { }
        public HephaestusAddRange(CombatSkillKey skillKey, sbyte direction) : base(skillKey, 94112, direction) { }
        
        // distance is 20 to 120
        public override bool DoTemplateReforgeDirect(WeaponItem weaponConfig)
        {
            var proposedMinDist = weaponConfig.MinDistance - 2;
            var proposedMaxDist = weaponConfig.MaxDistance + 2;
            int adjustment = 0;
            if (proposedMinDist < 20)
            {
                adjustment = 20 - proposedMinDist;
            }
            if (proposedMaxDist > 120)
            {
                if (adjustment != 0) { return false; } // out of range.
                adjustment = 120 - proposedMaxDist;
            }
            int finalMinDist = (proposedMinDist + adjustment);
            int finalMaxDist = (proposedMaxDist + adjustment);


            typeof(WeaponItem).GetField("MinDistance").SetValue(weaponConfig, (short)finalMinDist);
            typeof(WeaponItem).GetField("MaxDistance").SetValue(weaponConfig, (short)finalMaxDist);
            return true;
        }

        public override bool DoTemplateReforgeIndirect(WeaponItem weaponConfig)
        {
            if (weaponConfig.MaxDistance - weaponConfig.MinDistance <= 15) return false;
            typeof(WeaponItem).GetField("MaxDistance").SetValue(weaponConfig, (short)Math.Clamp(weaponConfig.MaxDistance - 2, (short)20, (short)120));
            typeof(WeaponItem).GetField("MinDistance").SetValue(weaponConfig, (short)Math.Clamp(weaponConfig.MinDistance + 2, (short)20, (short)120));

            return true;
        }

        public override string GetIdentifierDirect()
        {
            return "A";
        }

        public override string GetIdentifierIndirect()
        {
            return "a";
        }

        public override List<Tuple<string, Type>> GetInfluencedFields()
        {
            
            return new List<Tuple<string, Type>>() { Tuple.Create("MinDistance", typeof(short)), Tuple.Create("MaxDistance", typeof(short))};
        }
    }
}

using Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpellsFromTheWestBackend
{
    internal class CreateCombatSkillObjHelper
    {
        public static List<short> CombatSkillTemplateIds = new List<short>();

        public static void RegisterSkillItem(CombatSkillItem itm)
        {
            CombatSkillTemplateIds.Add(itm.TemplateId);
        }
    }
}

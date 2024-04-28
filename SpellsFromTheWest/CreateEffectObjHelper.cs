using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellsFromTheWestBackend
{
    internal class CreateEffectObjHelper
    {
        public static void RegisterSkillItem(int templateId, string type)
        {
            mySkillItems.TryAdd(templateId, Type.GetType("GameData.Domains.SpecialEffect." + type));
            mySkillNames.TryAdd(templateId, type); 
        }
        public static System.Type GetSkillTypeFromChangedId(int templateId) // looks like 9xxxx
        {
            if (mySkillItems.ContainsKey(templateId))
            {
                return mySkillItems[templateId];
            }
            return null;
        }
        static Dictionary<int, System.Type> mySkillItems = new Dictionary<int, System.Type>();
        static Dictionary<int, string> mySkillNames = new Dictionary<int, string>();
    }
}

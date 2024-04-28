using Config;
using GameData.Domains;
using SpellsFromTheWest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellsFromTheWestBackend.SkillEffects.Hephaestus
{
    internal class HephaestusCommon
    {
        public static void OnSaveLoaded()
        {
            string savedData;
            if (!DomainManager.Mod.TryGet(SpellsFromTheWestBackendPlugin.GetModIdStr(), "HephData", true, out savedData)) return;
            //HephaestusSerializer.LoadReforgeStatesToConfig(savedData);

        }
        public static void OnUnloading()
        {
            string savedData;
            if (!DomainManager.Mod.TryGet(SpellsFromTheWestBackendPlugin.GetModIdStr(), "HephData", true, out savedData)) return;
            //HephaestusSerializer.LoadOriginalStatesToConfig(savedData);
        }

        public static int GetReforgeTimes(short weaponTemplateId)
        {
            return 0;
        }

        public static void PurgeReforgeSkill(string reforgeIdentifier)
        {

        }

        public static bool TryReforgeWeapon(string reforgeIdentifier, short weaponTemplateId)
        {
            // returns true if can reforge.
            return false;
        }
    }
}

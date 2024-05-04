using GameData.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaiwuModdingLib.Core.Plugin;

namespace SpellsFromTheWest
{
    [PluginConfig("SpellsFromTheWestFrontendPlugin", "wilhelm", "1.0")]
    public class SpellsFromTheWestFrontendPlugin : TaiwuRemakePlugin
    {
        Harmony harmony;
        public override void Dispose()
        {
            if (harmony != null)
            {
                harmony.UnpatchSelf();
            }
        }

        public override void Initialize()
        {
            AdaptableLog.Info($"Load SpellsFromTheWest Frontend. Current Directory {Directory.GetCurrentDirectory()}");
            harmony = Harmony.CreateAndPatchAll(typeof(SpellsFromTheWestFrontendPlugin));
            // removed due to bug
            //AddCharacterFeature.DoAdd();
            AddConfig.DoAppend();


        }


    }
}

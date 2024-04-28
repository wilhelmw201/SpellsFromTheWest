﻿using GameData.Domains.Item;
using GameData.Utilities;
using HarmonyLib;
using System;
using System.IO;
using TaiwuModdingLib.Core.Plugin;
using GameData.Domains;
using GameData;
using GameData.ArchiveData;
using GameData.Common;
using GameData.Domains.World;
using GameData.Domains.SpecialEffect;
using GameData.Domains.CombatSkill;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.ComponentModel;
using GameData.Domains.Character;
using SpellsFromTheWestBackend;
using GameData.Domains.Combat;
using System.Linq;
using System.Threading;
using GameData.Domains.Building;
using GameData.DomainEvents;
using System.Runtime.CompilerServices;
using Config;
using GameData.Domains.Mod;
using System.Xml.Linq;
using GameData.Domains.Taiwu;
namespace SpellsFromTheWest
{

    [PluginConfig("SpellsFromTheWestBackendPlugin", "wilhelm", "1.0")]
    public class SpellsFromTheWestBackendPlugin : TaiwuRemakePlugin
    {
        Harmony harmony;
        static int replaceChance = 0;
        static bool learnAll = false;
        public override void Dispose()
        {
            if (harmony != null)
            {
                harmony.UnpatchSelf();
            }
        }

        public override void Initialize()
        {
            AdaptableLog.Info($"Load SpellsFromTheWest Backend. Current Directory {Directory.GetCurrentDirectory()}");
            harmony = Harmony.CreateAndPatchAll(typeof(SpellsFromTheWestBackendPlugin));
            AddConfig.DoAppend();
            harmony.PatchAll(typeof(PatchAddTranspiler1));
            harmony.PatchAll(typeof(PatchAddTranspiler2));
            harmony.PatchAll(typeof(DefenseSkillEndedTranspiler));
            thisModIdStr = base.ModIdStr;

            DomainManager.Mod.GetSetting(base.ModIdStr, "ReplaceChance", ref replaceChance);
            DomainManager.Mod.GetSetting(base.ModIdStr, "LearnEverything", ref learnAll);
            /*
            Thread.Sleep(5000);
            Utils.DebugBreak();
            FieldInfo fieldInfo = typeof(Config.Weapon).GetField("_dataArray", BindingFlags.NonPublic | BindingFlags.Instance);
            List<Config.WeaponItem> allWeapons = (List<Config.WeaponItem>)fieldInfo.GetValue(Config.Weapon.Instance);
            

            typeof(WeaponItem).GetField("BaseWeight").SetValue(allWeapons[188], 3000);
            Utils.DebugBreak();*/

        }
        static  string thisModIdStr;
        public static string GetModIdStr()
        {
            return thisModIdStr;
            /*
            if (thisModIdStr != null) return thisModIdStr;

            Type type = typeof(ModDomain);
            FieldInfo fieldInfo = type.GetField("ModInfoDict", BindingFlags.NonPublic | BindingFlags.Static);
            Dictionary<string, ModInfo> modInfoDict = (Dictionary<string, ModInfo>)fieldInfo.GetValue(null);

            foreach (var modInfo in modInfoDict)
            {
                if (modInfo.Value.Title == "神话功法")
                {
                    thisModIdStr = modInfo.Key;
                }
                return thisModIdStr;
            }
            throw new KeyNotFoundException("神话功法：无法找到本mod的modIdStr");*/
        }

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(GameData.Domains.Information.InformationDomain), "ProcessAdvanceMonth")]
        //public static void ProcessAdvanceMonth_Post()
        //{
        //    var Taiwu = DomainManager.Taiwu.GetTaiwu();
        //    var context = GameData.Common.DataContextManager.GetCurrentThreadDataContext();
        //    ItemKey testBook = DomainManager.Item.CreateSkillBook(context,
        //            templateId: 1234, completePagesCount: 2, lostPagesCount: 0);
        //    Taiwu.AddInventoryItem(context, testBook, 1);
        //}
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData.Domains.CombatSkill.CombatSkillDomain), "InitializeOnInitializeGameDataModule")]
        public static void InitializeOnInitializeGameDataModule_Post()
        {
            AdaptableLog.Info("Attempting EquipAddPropertyDict resize.");

            // Fix for GetCharPropertyBonus out of bound error
            try
            {
                Array.Resize(ref CombatSkillDomain.EquipAddPropertyDict, 32767);
                AdaptableLog.Info(String.Format("EquipAddPropertyDict size: {0}", CombatSkillDomain.EquipAddPropertyDict.Length));
            }
            catch
            {
                AdaptableLog.Info("EquipAddPropertyDict resize failed.");
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorldDomain), "AdvanceMonth")]
        public static void WorldDomain_AdvanceMonth_PrePatch(DataContext context)
        {
            var Taiwu = DomainManager.Taiwu.GetTaiwu();
            ItemKey testBook = DomainManager.Item.CreateSkillBook(context,
                    templateId: 4101, completePagesCount: 2, lostPagesCount: 0);
            Taiwu.AddInventoryItem(context, testBook, 1);
            
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData.Domains.SpecialEffect.SpecialEffectType), "CreateEffectObj")]
        public static bool CreateEffectObj_PreFix(int type, ref SpecialEffectBase __result)
        {
            if (type < 90000) return true;
            Type effType = CreateEffectObjHelper.GetSkillTypeFromChangedId(type);
            if (effType == null) return true;
            __result = (SpecialEffectBase)Activator.CreateInstance(effType);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CombatDomain), "ClearAffectingDefenseSkill")]
        public static bool ClearAffectingDefenseSkill_PreFix(DataContext context, CombatCharacter character)
        {
            NewEvents.RaiseDefenseSkillEnding(context, character);
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData.DomainEvents.Events), "RaiseCombatEnd")]
        public static void Combatend_Postfix()
        {
            NewEvents.ClearDefenseSkillEndingHandlers();
        }

        /* no longer needed as we are patching CreateItem
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildingDomain), "GetWestRandomItemByGarde")]
        public static bool GetWestRandomItemByGarde_PreFix(DataContext context, sbyte grade, ItemKey __result)
        {
            __result = new ItemKey();
            if (replaceChance <= 0) { return true; }
            if (context.Random.CheckPercentProb(replaceChance))
            {
                Utils.DebugBreak();
                int templateId = CreateSkillBookObjHelper.GetRandomBookTemplateId(context, grade);
                //__result = DomainManager.Item.CreateSkillBook(context, (short)templateId);
                __result = new ItemKey(ItemSubType.GetType((short)1001), 0, (short)templateId, -1);
                return false;
            }
            return true;
        }*/

        /* causes merchants to not have any items :(
         * [HarmonyPrefix]
         [HarmonyPatch(typeof(ItemDomain), "CreateMisc")]
         public static bool CreateMisc_PreFix(DataContext context, short templateId, ItemKey __result)
         {

             __result = new ItemKey();

             if (replaceChance <= 0) { return true; }
             Utils.DebugBreak();
             var template = Config.Misc.Instance[templateId];
             if (template.ItemSubType == 1203 && context.Random.CheckPercentProb(replaceChance))
             {
                 Utils.DebugBreak();
                 int newtemplateId = CreateSkillBookObjHelper.GetRandomBookTemplateId(context, template.TemplateId);
                 __result = DomainManager.Item.CreateSkillBook(context, (short)newtemplateId);
                 return false;
             }
             return true;
         }*/
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemDomain), "CreateItem")]
        public static bool CreateItem(DataContext context, ref sbyte itemType, ref short templateId, ItemKey __result)
        {

            __result = new ItemKey();

            if (replaceChance <= 0 || itemType != 12 || !(templateId <= 72 && templateId >= 28)) { return true; }
            var template = Config.Misc.Instance[templateId];
            if (context.Random.CheckPercentProb(replaceChance))
            {
                Utils.DebugBreak();

                short newTemplateId = (short)CreateSkillBookObjHelper.GetRandomBookTemplateId(context, template.Grade);
                if (newTemplateId > 0)
                {

                    itemType = 10;
                    templateId = (short)newTemplateId;

                }
            }
            return true;
        }
        /*
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemDomain), "CreateMisc")]
        public static bool CreateMisc_PreFix(DataContext context, short templateId, ItemKey __result)
        {

            __result = new ItemKey();

            if (replaceChance <= 0) { return true; }
            Utils.DebugBreak();
            var template = Config.Misc.Instance[templateId];
            if (template.ItemSubType == 1203 && context.Random.CheckPercentProb(replaceChance))
            {
                Utils.DebugBreak();
                int newtemplateId = CreateSkillBookObjHelper.GetRandomBookTemplateId(context, template.TemplateId);
                __result = DomainManager.Item.CreateSkillBook(context, (short)newtemplateId);
                return false;
            }
            return true;
        }*/
        public override void OnLoadedArchiveData()
        {
            DataContext context = DataContextManager.GetCurrentThreadDataContext();
            base.OnEnterNewWorld();
            Utils.DebugBreak();
            if (learnAll)
            {
                var taiwuId = DomainManager.Taiwu.GetTaiwuCharId();
                foreach (short templateId in CreateCombatSkillObjHelper.CombatSkillTemplateIds)
                {
                    TaiwuCombatSkill skill;
                    if (!DomainManager.Taiwu.TryGetElement_CombatSkills(templateId, out skill))
                    {
                        DomainManager.Taiwu.TaiwuLearnCombatSkill(context, templateId, ushort.MaxValue);
                        DomainManager.Taiwu.TryGetElement_CombatSkills(templateId, out skill);
                    }
                    var element_CombatSkills = 
                        DomainManager.CombatSkill.GetElement_CombatSkills(new CombatSkillKey(
                           taiwuId, templateId));
                    element_CombatSkills.SetPracticeLevel(100, context);


                }
            }
        }
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(Tester), "Assert")]
        //public static bool Assert_PreFix()
        //{

        //    return false;

        //}
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(SpecialEffectDomain), "Add", new Type[]
        //{
        //    typeof(DataContext),
        //    typeof(int),
        //    typeof(string)
        //})]
        //public static bool FixAdd(DataContext context, int charId, string effectName, SpecialEffectDomain __instance, out long __result)
        //{
        //    if (!effectName.StartsWith("SpellsFromTheWest"))
        //    {
        //        __result = 0;
        //        return true;
        //    }

        //    Type type = AccessTools.TypeByName(effectName);
        //    if (type == null)
        //    {
        //        throw new Exception("Cannot find type '" + effectName + "'.");
        //    }
        //    SpecialEffectBase specialEffectBase = (SpecialEffectBase)Activator.CreateInstance(type, new object[]
        //    {
        //        charId
        //    });
        //    __instance.Add(context, specialEffectBase);
        //    __result = specialEffectBase.Id;
        //    return false;
        //}
        //// Token: 0x06000009 RID: 9 RVA: 0x00002284 File Offset: 0x00000484
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(SpecialEffectDomain), "Add", new Type[]
        //{
        //    typeof(DataContext),
        //    typeof(int),
        //    typeof(short),
        //    typeof(sbyte),
        //    typeof(sbyte)
        //})]
        //public static bool FixAdd(DataContext context, int charId, short skillTemplateId, sbyte effectActiveType, 
        //    sbyte direction, SpecialEffectDomain __instance)
        //{
        //    return true;
        //}

        static public Type MyGetType(Type currentType, string NeededType)
        {
            if (currentType != null) return currentType; // gettype was successful
            // try to return our own type.
            Utils.MyLog("Using our own type for " + NeededType);
            return Type.GetType(NeededType);
        }
        }


    [HarmonyPatch(typeof(SpecialEffectDomain), "Add", new Type[]
    {
            typeof(DataContext),
            typeof(int),
            typeof(short),
            typeof(sbyte),
            typeof(sbyte)
    })]
    class PatchAddTranspiler2
    {
        public static IEnumerable<CodeInstruction> Transpiler(MethodBase __originalMethod, IEnumerable<CodeInstruction> instructions)
        {
            AdaptableLog.Info("Transpiling Add(5)");
            /*
            var InstrEnumerator = instructions.GetEnumerator();
            while (InstrEnumerator != null && InstrEnumerator.MoveNext())
            {
                try
                {
                    throw new NotImplementedException();
                }
                catch { };
                var currentOp = InstrEnumerator.Current;
                
                AdaptableLog.Info(String.Format("opcode {0} operand {1} typeOperand {2}",
                    currentOp.opcode,
                    currentOp.operand == null ? "null" : currentOp.operand,
                    currentOp.operand == null ? "null" : currentOp.operand.GetType()));
                yield return currentOp;
            }*/
            
            var InstrEnumerator = instructions.GetEnumerator();

            while (InstrEnumerator.MoveNext())
            {
                var currentOp = InstrEnumerator.Current;
                if (currentOp.opcode == OpCodes.Call && currentOp.operand is MethodInfo
                    && ((System.Reflection.MethodInfo)currentOp.operand).Name == "GetType")
                {
                    break;
                }
                else
                {
                    yield return currentOp;
                }
            }
            yield return InstrEnumerator.Current; // IL_00A7: call  class [System.Runtime]System.Type [System.Runtime]System.Type::GetType(string)
            InstrEnumerator.MoveNext();
            yield return InstrEnumerator.Current; // IL_00AC: stloc.s   specialEffectType
            yield return new CodeInstruction(OpCodes.Ldloc_S, 6); // load currenttype
            yield return new CodeInstruction(OpCodes.Ldloc_S, 5); // load neededtype
            yield return new CodeInstruction(OpCodes.Call, typeof(SpellsFromTheWestBackendPlugin).GetMethod("MyGetType"));
            yield return new CodeInstruction(OpCodes.Stloc_S, 6); // save currenttype

            while (InstrEnumerator.MoveNext())
            {
                var currentOp = InstrEnumerator.Current;
                yield return currentOp;
            }
        }
    }



    [HarmonyPatch(typeof(SpecialEffectDomain), "Add", new Type[]
    {
            typeof(DataContext),
            typeof(int),
            typeof(string)
    })]
    class PatchAddTranspiler1
    {
        public static IEnumerable<CodeInstruction> Transpiler(MethodBase __originalMethod, IEnumerable<CodeInstruction> instructions)
        {
            AdaptableLog.Info("Transpiling Add(3)");

            var InstrEnumerator = instructions.GetEnumerator();


            
            while (InstrEnumerator.MoveNext())
            {
                var currentOp = InstrEnumerator.Current;
                if (currentOp.opcode == OpCodes.Call && currentOp.operand is MethodInfo && 
                    ((System.Reflection.MethodInfo)currentOp.operand).Name == "GetType")
                {
                    break;
                }
                else
                {
                    yield return currentOp;
                }
            }
            yield return InstrEnumerator.Current; // IL_000E: call  class [System.Runtime]System.Type [System.Runtime]System.Type::GetType(string)
            InstrEnumerator.MoveNext();
            yield return InstrEnumerator.Current; // IL_0013: stloc.1
            yield return new CodeInstruction(OpCodes.Ldloc_1);
            yield return new CodeInstruction(OpCodes.Ldloc_0);
            yield return new CodeInstruction(OpCodes.Call, typeof(SpellsFromTheWestBackendPlugin).GetMethod("MyGetType"));
            yield return new CodeInstruction(OpCodes.Stloc_1);

            while (InstrEnumerator.MoveNext())
            {
                var currentOp = InstrEnumerator.Current;
                yield return currentOp;
            }
        }
    }

    [HarmonyPatch(typeof(CombatCharacterStateBase), "TimeUpdateMainDefend")]
    class DefenseSkillEndedTranspiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(MethodBase __originalMethod, IEnumerable<CodeInstruction> instructions)
        {
            AdaptableLog.Info("Transpiling TimeUpdateMainDefend");
            Utils.DebugBreak();

            List<CodeInstruction> instructionsList = instructions.ToList();

     

            int i = 0;
            for (; i < instructionsList.Count; i++)
            {
                if (instructionsList[i].opcode == OpCodes.Ldarg_1 &&
                    instructionsList[i+1].opcode == OpCodes.Ldc_I4_M1 &&
                    instructionsList[i+2].opcode == OpCodes.Ldarg_0 &&
                    instructionsList[i+3].opcode == OpCodes.Callvirt)
                {

                    break;
                }
                else
                {
                    yield return instructionsList[i];
                }
            }
            // We now call NewEvents:: DefenseSkillEndingPatch(DataContext context,CombatCharacter combatChar)
            // to do this we push in this order: context, combatChar,
            yield return new CodeInstruction(OpCodes.Ldarg_0); // load context
            yield return new CodeInstruction(OpCodes.Ldarg_1); // load combatchar
            yield return new CodeInstruction(OpCodes.Call, typeof(NewEvents).GetMethod("RaiseDefenseSkillEnding"));

            // return rest of the function including the four detected lines above
            for (; i < instructionsList.Count; i++)
            {
                yield return instructionsList[i];
            }
        }
    }

}

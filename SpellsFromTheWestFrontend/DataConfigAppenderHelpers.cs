// #define DEBUGGING

using Config.ConfigCells.Character;
using Config;
using GameData.Domains.Item;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GameData.Utilities;
using System.Reflection;

namespace SpellsFromTheWest
{
    internal class DataConfigAppenderHelpers
    {
        public static void AddSkillBreakGridList(CombatSkillItem Item)
        {
            var Grade = Item.Grade;
            var Type = Item.Type;
            var fieldInfo = typeof(Config.CombatSkill).GetField("_dataArray", BindingFlags.NonPublic | BindingFlags.Instance);
            List<CombatSkillItem> ItemList = (List<CombatSkillItem>)fieldInfo.GetValue(Config.CombatSkill.Instance);
            var fieldInfo1 = typeof(Config.SkillBreakGridList).GetField("_dataArray", BindingFlags.NonPublic | BindingFlags.Instance);
            List<SkillBreakGridListItem> SkillBreakGridListList = (List<SkillBreakGridListItem>)fieldInfo1.GetValue(Config.SkillBreakGridList.Instance);
            CombatSkillItem ReferenceSkill = ItemList.First(item => item.Grade == Grade && item.Type == Type);
            SkillBreakGridListItem ReferenceSkillBreakGridListItem = SkillBreakGridListList.First(item => item.TemplateId == ReferenceSkill.TemplateId);
            SkillBreakGridListItem CopiedGradeList = new SkillBreakGridListItem(Item.TemplateId, ReferenceSkillBreakGridListItem.BreakGridListJust,
                ReferenceSkillBreakGridListItem.BreakGridListKind, ReferenceSkillBreakGridListItem.BreakGridListEven,
                ReferenceSkillBreakGridListItem.BreakGridListRebel, ReferenceSkillBreakGridListItem.BreakGridListEgoistic);
            SkillBreakGridList.Instance.AddExtraItem((Item.TemplateId).ToString(), Item.Name, CopiedGradeList);

        }
        public static void AddSpecialEffectItemToConfig(string TemplateId, string Name, object Item)
        {
            SpecialEffect.Instance.AddExtraItem(TemplateId, Name, Item);
        }
        public static void AddCombatSkillItemToConfig(string TemplateId, string Name, object Item)
        {
            CombatSkill.Instance.AddExtraItem(TemplateId, Name, Item);
            AddSkillBreakGridList((CombatSkillItem)Item);
        }
        public static void AddSkillBookToConfig(string TemplateId, string Name, object Item)
        {
            SkillBook.Instance.AddExtraItem(TemplateId, Name, Item);
        }
        public static List<Dictionary<string, string>> ParseCSV(String csvContent)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            using (StringReader stringReader = new StringReader(csvContent))
            using (TextFieldParser parser = new TextFieldParser(stringReader))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                // Read the first line to get keys
                string[] keys = parser.ReadFields();

                // Read remaining lines and create dictionary entries
                while (!parser.EndOfData)
                {
                    Dictionary<string, string> csvDict = new Dictionary<string, string>();

                    string[] values = parser.ReadFields();

                    // Create dictionary entry with keys and values
                    for (int i = 0; i < keys.Length; i++)
                    {
                        string key = keys[i];
                        string value = i < values.Length ? values[i] : string.Empty; // Handle if values are missing
                        csvDict[key] = value;
                    }
                    result.Add(csvDict);
                }
            }

            return result;
        }


        static public string ParseString(string input)
        {
            return input.Length > 0 ? input : null;
        }
        static List<Dictionary<string, string>> ParseStringToDictList(string input)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

            int i = 0;
            while (i < input.Length)
            {
                // Find the start of a dictionary
                int start = input.IndexOf('{', i);
                if (start == -1)
                    break;

                // Find the end of the dictionary
                int end = input.IndexOf('}', start);
                if (end == -1)
                    break;

                // Extract the dictionary substring
                string dictStr = input.Substring(start, end - start + 1);

                // Parse the dictionary string
                Dictionary<string, string> dict = ParseDictionary(dictStr);

                // Add the parsed dictionary to the result list
                result.Add(dict);

                // Move to the next position
                i = end + 1;
            }

            return result;
        }

        static Dictionary<string, string> ParseDictionary(string dictStr)
        {
            dictStr = dictStr.Trim('{', '}');

            Dictionary<string, string> dict = new Dictionary<string, string>();

            // Split the dictionary string by commas
            string[] entries = dictStr.Split(',');

            foreach (string entry in entries)
            {
                // Split each entry by colon
                string[] keyValue = entry.Split(':');

                // Trim whitespace from key and value
                string key = keyValue[0].Trim(' ', '\'', '"');
                string value = (keyValue[1].Trim(' ', '\'', '"'));

                // Add key-value pair to the dictionary
                dict.Add(key, value);
            }

            return dict;
        }

        public static List<Dictionary<string, string>> parseListOfDict(String input)
        {
            if (string.IsNullOrEmpty(input) || input.Length < 5) return new List<Dictionary<string, string>>();
#if DEBUGGING
            AdaptableLog.Info("Parsing: " + input);
#endif
            // Replace single quotes with double quotes because JSON.NET requires double quotes for keys and string values
            input = input.Replace("'", "\"");

            // Deserialize the JSON string into a list of dictionaries with string keys
            return ParseStringToDictList(input);

        }
        public static PoisonsAndLevels parsePoisonsAndLevels(String input)
        {
            if (string.IsNullOrEmpty(input)) return new PoisonsAndLevels(new short[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
#if DEBUGGING
            AdaptableLog.Info("Parsing: " + input);
#endif
            // using the original representation of the game will generate an error here.
            // instead, pass in an array with 12 integers. (first in pair is value, second in pair is level 1/2/3.
            try
            {
                return new PoisonsAndLevels(parseArrShort(input));
            }
            catch (Exception e)
            {
                AdaptableLog.Warning("Parse poison failed for input=" + input);
                return new PoisonsAndLevels(new short[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            }
        }
        public static List<PropertyAndValue> parseListPropertyValue(String input)
        {
            if (string.IsNullOrEmpty(input)) return new List<PropertyAndValue> { };
#if DEBUGGING
            AdaptableLog.Info("Parsing: " + input);
#endif
            List<Dictionary<string, string>> inputDict = parseListOfDict(input);

            return new List<PropertyAndValue>(inputDict.Select(e => new PropertyAndValue(short.Parse(e["PropertyId"]), short.Parse(e["Value"]))));
        }
        public static List<sbyte> parseListSByte(String input)
        {
            if (string.IsNullOrEmpty(input)) return new List<sbyte> { };
#if DEBUGGING
            AdaptableLog.Info("Parsing: " + input);
#endif
            // e.g. [0, 0, 0, 0, 0]
            string[] elements = input.Trim('[', ']').Split(',');

            // Filter out empty strings if any
            elements = elements.Where(e => !string.IsNullOrWhiteSpace(e)).ToArray();

            List<sbyte> result = elements.Select(e => sbyte.Parse(e.Trim())).ToList();
            return result;
        }
        public static List<NeedTrick> parseListNeedTrick(String input)
        {
#if DEBUGGING
            AdaptableLog.Info("Parsing: " + input);
#endif
            if (string.IsNullOrEmpty(input)) return new List<NeedTrick> { };
            List<Dictionary<string, string>> inputDict = parseListOfDict(input);

            return new List<NeedTrick>(inputDict.Select(e => new NeedTrick(sbyte.Parse(e["TrickType"]), byte.Parse(e["NeedCount"]))));

        }
        public static int[] parseArrInt(String input)
        {
            if (string.IsNullOrEmpty(input)) return new int[] { };
#if DEBUGGING
            AdaptableLog.Info("Parsing: " + input);
#endif
            // e.g. [0, 0, 0, 0, 0]
            string[] elements = input.Trim('[', ']').Split(',');

            // Filter out empty strings if any
            elements = elements.Where(e => !string.IsNullOrWhiteSpace(e)).ToArray();

            int[] result = elements.Select(e => int.Parse(e.Trim())).ToArray();
            return result;
        }
        public static short[] parseArrShort(String input)
        {
            if (string.IsNullOrEmpty(input)) return new short[] { };
#if DEBUGGING
            AdaptableLog.Info("Parsing: " + input);
#endif
            // e.g. [0, 0, 0, 0, 0]
            string[] elements = input.Trim('[', ']').Split(',');

            // Filter out empty strings if any
            elements = elements.Where(e => !string.IsNullOrWhiteSpace(e)).ToArray();

            short[] result = elements.Select(e => short.Parse(e.Trim())).ToArray();
            return result;
        }
        public static String[] parseArrString(String input)
        {
            if (string.IsNullOrEmpty(input)) { return new String[] { }; }
#if DEBUGGING
            AdaptableLog.Info("Parsing: " + input);
#endif
            string[] elements = input.Trim('[', ']').Split(',');

            // Filter out empty strings if any
            elements = elements.Where(e => !string.IsNullOrWhiteSpace(e)).ToArray();

            string[] result = elements.Select(e => (e.Trim())).ToArray();
            return result;

        }
        public static sbyte[] parseArrSByte(String input)
        {
            if (string.IsNullOrEmpty(input)) return new sbyte[] { };
#if DEBUGGING
            AdaptableLog.Info("Parsing: " + input);
#endif
            // e.g. [0, 0, 0, 0, 0]
            string[] elements = input.Trim('[', ']').Split(',');

            // Filter out empty strings if any
            elements = elements.Where(e => !string.IsNullOrWhiteSpace(e)).ToArray();

            sbyte[] result = elements.Select(e => sbyte.Parse(e.Trim())).ToArray();
            return result;

        }
        public static SkillBookItem CreateSkillBookItem(short TemplateId, string Name, sbyte ItemType, short ItemSubType,
            sbyte Grade, string Icon, string Desc, bool Transferable, bool Stackable, bool Wagerable, bool Refinable,
            bool Poisonable, bool Repairable, short MaxDurability, int BaseWeight, int BaseValue, int BasePrice,
            sbyte BaseHappinessChange, int BaseFavorabilityChange, sbyte GiftLevel, sbyte DropRate, sbyte ResourceType,
            short PreservationDuration, sbyte LifeSkillType, short LifeSkillTemplateId, sbyte CombatSkillType,
            short CombatSkillTemplateId, short LegacyPoint, List<short> ReferenceBooksWithBonus)
        {
            // the false is AllowRandonCreate
            SkillBookItem skillBookItem = new SkillBookItem(TemplateId, 0, ItemType, ItemSubType, Grade, 
                Icon, 1, Transferable, Stackable, Wagerable, Refinable, Poisonable, Repairable, MaxDurability, 
                BaseWeight, BaseValue, BasePrice, BaseHappinessChange, BaseFavorabilityChange, GiftLevel, false, DropRate, 
                ResourceType, PreservationDuration, LifeSkillType, LifeSkillTemplateId, CombatSkillType, CombatSkillTemplateId, 
                LegacyPoint, ReferenceBooksWithBonus);
            
            typeof(SkillBookItem).GetField("Name").SetValue(skillBookItem, Name);
            typeof(SkillBookItem).GetField("Desc").SetValue(skillBookItem, Desc);
            return skillBookItem;
        }
        public static CombatSkillItem CreateCombatSkillItem(short TemplateId, string Name, sbyte Grade, string Desc,
              string Icon, sbyte EquipType, sbyte Type, sbyte GridCost, sbyte SectId, sbyte FiveElements, short BookId,
              bool CanObtainByAdventure, bool IsNonPublic, sbyte OrderIdInSect, List<PropertyAndValue> UsingRequirement,
              int DirectEffectID, int ReverseEffectID, byte InheritAttainmentAdiitionRate, sbyte PracticeType, string BreakStart,
              string BreakEnd, bool GoneMadInnerInjury, List<sbyte> GoneMadInjuredPart, sbyte GoneMadInjuryValue,
              short GoneMadQiDisorder, short TotalObtainableNeili, short ObtainedNeiliPerLoop, sbyte DestTypeWhileLooping,
              sbyte TransferTypeWhileLooping, sbyte FiveElementChangePerLoop, sbyte[] SpecificGrids, sbyte GenericGrid,
              string AssetFileName, string PrepareAnimation, string CastAnimation, string CastParticle, string CastPetAnimation,
              string CastPetParticle, short[] DistanceWhenFourStepAnimation, string CastSoundEffect, string PlayerCastBossSkillPrepareAni,
              string PlayerCastBossSkillAni, string PlayerCastBossSkillParticle, string PlayerCastBossSkillSound,
              short[] PlayerCastBossSkillDistance, int PrepareTotalProgress, List<sbyte> NeedBodyPartTypes, short MobilityCost,
              sbyte BreathStanceTotalCost, sbyte BaseInnerRatio, sbyte InnerRatioChangeRange, short Penetrate,
              short DistanceAdditionWhenCast, List<NeedTrick> TrickCost, sbyte WeaponDurableCost, sbyte WugCost,
              short MostFittingWeaponID, sbyte[] InjuryPartAtkRateDistribution, short TotalHit, sbyte[] PerHitDamageRateDistribution,
              bool HasAtkAcupointEffect, PoisonsAndLevels Poisons, sbyte EquipmentBreakOdds, sbyte AddWugType,
              short[] AddBreakBodyFeature, short AddMoveSpeedOnCast, short[] AddHitOnCast, sbyte MobilityReduceSpeed,
              short MoveCostMobility, sbyte MaxJumpDistance, sbyte JumpPrepareFrame, bool CanPartlyJump, string[] JumpAni,
              string[] JumpParticle, short JumpChangeDistanceFrame, short JumpChangeDistanceDuration, sbyte ScoreBonusType,
              short ScoreBonus, short AddOuterPenetrateResistOnCast, short AddInnerPenetrateResistOnCast,
              short[] AddAvoidOnCast, short FightBackDamage, short BounceRateOfOuterInjury, short BounceRateOfInnerInjury,
              short ContinuousFrames, short BounceDistance, int RecoverBlockPercent, string DefendAnimation, string DefendParticle,
              string DefendSound, string FightBackAnimation, string FightBackParticle, string FightBackSound, List<PropertyAndValue> PropertyAddList,
              int[] OuterDamageSteps, int[] InnerDamageSteps, int FatalDamageStep, int MindDamageStep)
        {
            CombatSkillItem combatSkillItem = new CombatSkillItem(TemplateId, 0, Grade, 1, Icon, EquipType, Type, ECombatSkillSubType.Invalid,
                GridCost, SectId, FiveElements, BookId, CanObtainByAdventure, IsNonPublic, OrderIdInSect, 
                UsingRequirement, DirectEffectID, ReverseEffectID, InheritAttainmentAdiitionRate, PracticeType,
                2, 3, GoneMadInnerInjury, GoneMadInjuredPart, GoneMadInjuryValue, GoneMadQiDisorder,
                TotalObtainableNeili, ObtainedNeiliPerLoop, DestTypeWhileLooping, TransferTypeWhileLooping,
                FiveElementChangePerLoop, SpecificGrids, GenericGrid, AssetFileName, PrepareAnimation,
                CastAnimation, CastParticle, CastPetAnimation, CastPetParticle, DistanceWhenFourStepAnimation,
                CastSoundEffect, PlayerCastBossSkillPrepareAni, PlayerCastBossSkillAni, PlayerCastBossSkillParticle,
                PlayerCastBossSkillSound, PlayerCastBossSkillDistance, PrepareTotalProgress, NeedBodyPartTypes,
                MobilityCost, BreathStanceTotalCost, BaseInnerRatio, InnerRatioChangeRange, Penetrate,
                DistanceAdditionWhenCast, TrickCost, WeaponDurableCost, WugCost, MostFittingWeaponID,
                InjuryPartAtkRateDistribution, TotalHit, PerHitDamageRateDistribution, HasAtkAcupointEffect,
                Poisons, EquipmentBreakOdds, AddWugType, AddBreakBodyFeature, AddMoveSpeedOnCast, AddHitOnCast,
                MobilityReduceSpeed, MoveCostMobility, MaxJumpDistance, JumpPrepareFrame, CanPartlyJump, JumpAni,
                JumpParticle, JumpChangeDistanceFrame, JumpChangeDistanceDuration, ScoreBonusType, ScoreBonus,
                AddOuterPenetrateResistOnCast, AddInnerPenetrateResistOnCast, AddAvoidOnCast, FightBackDamage,
                BounceRateOfOuterInjury, BounceRateOfInnerInjury, ContinuousFrames, BounceDistance, RecoverBlockPercent,
                DefendAnimation, DefendParticle, DefendSound, FightBackAnimation, FightBackParticle, FightBackSound,
                PropertyAddList, OuterDamageSteps, InnerDamageSteps, FatalDamageStep, MindDamageStep);
            typeof(CombatSkillItem).GetField("Name").SetValue(combatSkillItem, Name);
            typeof(CombatSkillItem).GetField("Desc").SetValue(combatSkillItem, Desc);
            typeof(CombatSkillItem).GetField("BreakStart").SetValue(combatSkillItem, BreakStart);
            typeof(CombatSkillItem).GetField("BreakEnd").SetValue(combatSkillItem, BreakEnd);
            return combatSkillItem;
        }

        public static SpecialEffectItem CreateSpecialEffectItem(short TemplateId, sbyte EffectActiveType, short MinEffectCount, short MaxEffectCount, sbyte RequireAttackPower, sbyte AiCostNeiliAllocationChanceDelayFrame, ESpecialEffectAiCostNeiliAllocationType AiCostNeiliAllocationType, ESpecialEffectJumpType JumpType, int[] AffectRequirePower, string Name, short SkillTemplateId, string[] ShortDesc, string[] Desc, string[] PlayerCastBossSkillDesc, string ClassName)
        {
            SpecialEffectItem specialEffectItem = new SpecialEffectItem(TemplateId, EffectActiveType, MinEffectCount, MaxEffectCount, RequireAttackPower, AiCostNeiliAllocationChanceDelayFrame, AiCostNeiliAllocationType, JumpType, AffectRequirePower, 0, SkillTemplateId, new int[0], new int[]
            {
                1
            }, new int[0], ClassName);
            typeof(SpecialEffectItem).GetField("Name").SetValue(specialEffectItem, Name);
            typeof(SpecialEffectItem).GetField("ShortDesc").SetValue(specialEffectItem, ShortDesc);
            typeof(SpecialEffectItem).GetField("Desc").SetValue(specialEffectItem, Desc);
            typeof(SpecialEffectItem).GetField("PlayerCastBossSkillDesc").SetValue(specialEffectItem, PlayerCastBossSkillDesc);
            return specialEffectItem;
        }


    }
}

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
using System.Reflection;
using GameData.Utilities;



namespace SpellsFromTheWest
{
    internal class DataConfigAppenderHelpers
    {
        public static void AddSpecialEffectItemToConfig(string TemplateId, string Name, object Item)
        {
            FieldInfo fieldInfo = typeof(Config.SpecialEffect).GetField("_extraDataMap", BindingFlags.NonPublic | BindingFlags.Instance);
            Dictionary<int, SpecialEffectItem> ItemList = (Dictionary<int, SpecialEffectItem>)fieldInfo.GetValue(Config.SpecialEffect.Instance);
            ItemList[int.Parse(TemplateId)] = (SpecialEffectItem)Item;
        }
        public static void AddCombatSkillItemToConfig(string TemplateId, string Name, object Item)
        {
            FieldInfo fieldInfo = typeof(Config.CombatSkill).GetField("_extraDataMap", BindingFlags.NonPublic | BindingFlags.Instance);
            Dictionary<int, CombatSkillItem> ItemList = (Dictionary<int, CombatSkillItem>)fieldInfo.GetValue(Config.CombatSkill.Instance);
            ItemList[int.Parse(TemplateId)] = (CombatSkillItem)Item;
        }
        public static void AddSkillBookToConfig(string TemplateId, string Name, object Item)
        {
            FieldInfo fieldInfo = typeof(Config.SkillBook).GetField("_extraDataMap", BindingFlags.NonPublic | BindingFlags.Instance);
            Dictionary<int, SkillBookItem> ItemList = (Dictionary<int, SkillBookItem>)fieldInfo.GetValue(Config.SkillBook.Instance);
            ItemList[int.Parse(TemplateId)] = (SkillBookItem)Item;
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
            if (string.IsNullOrEmpty(input)) return null;
#if DEBUGGING
            AdaptableLog.Info("Parsing: " + input);
#endif
            List<Dictionary<string, string>> inputDict = parseListOfDict(input);

            return new List<PropertyAndValue>(inputDict.Select(e => new PropertyAndValue(short.Parse(e["PropertyId"]), short.Parse(e["Value"]))));
        }
        public static List<sbyte> parseListSByte(String input)
        {
            if (string.IsNullOrEmpty(input)) return null;
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
            if (string.IsNullOrEmpty(input)) return null;
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
            if (string.IsNullOrEmpty(input)) return null;
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
            SkillBookItem MySkillBookItem = new SkillBookItem();
            typeof(SkillBookItem).GetField("Name").SetValue(MySkillBookItem, Name);
            typeof(SkillBookItem).GetField("Desc").SetValue(MySkillBookItem, Desc);
            typeof(SkillBookItem).GetField("TemplateId").SetValue(MySkillBookItem, TemplateId);
            typeof(SkillBookItem).GetField("Name").SetValue(MySkillBookItem, Name);
            typeof(SkillBookItem).GetField("ItemType").SetValue(MySkillBookItem, ItemType);
            typeof(SkillBookItem).GetField("ItemSubType").SetValue(MySkillBookItem, ItemSubType);
            typeof(SkillBookItem).GetField("Grade").SetValue(MySkillBookItem, Grade);
            typeof(SkillBookItem).GetField("Icon").SetValue(MySkillBookItem, Icon);
            typeof(SkillBookItem).GetField("Desc").SetValue(MySkillBookItem, Desc);
            typeof(SkillBookItem).GetField("Transferable").SetValue(MySkillBookItem, Transferable);
            typeof(SkillBookItem).GetField("Stackable").SetValue(MySkillBookItem, Stackable);
            typeof(SkillBookItem).GetField("Wagerable").SetValue(MySkillBookItem, Wagerable);
            typeof(SkillBookItem).GetField("Refinable").SetValue(MySkillBookItem, Refinable);
            typeof(SkillBookItem).GetField("Poisonable").SetValue(MySkillBookItem, Poisonable);
            typeof(SkillBookItem).GetField("Repairable").SetValue(MySkillBookItem, Repairable);
            typeof(SkillBookItem).GetField("MaxDurability").SetValue(MySkillBookItem, MaxDurability);
            typeof(SkillBookItem).GetField("BaseWeight").SetValue(MySkillBookItem, BaseWeight);
            typeof(SkillBookItem).GetField("BaseValue").SetValue(MySkillBookItem, BaseValue);
            typeof(SkillBookItem).GetField("BasePrice").SetValue(MySkillBookItem, BasePrice);
            typeof(SkillBookItem).GetField("BaseHappinessChange").SetValue(MySkillBookItem, BaseHappinessChange);
            typeof(SkillBookItem).GetField("BaseFavorabilityChange").SetValue(MySkillBookItem, BaseFavorabilityChange);
            typeof(SkillBookItem).GetField("GiftLevel").SetValue(MySkillBookItem, GiftLevel);
            typeof(SkillBookItem).GetField("DropRate").SetValue(MySkillBookItem, DropRate);
            typeof(SkillBookItem).GetField("ResourceType").SetValue(MySkillBookItem, ResourceType);
            typeof(SkillBookItem).GetField("PreservationDuration").SetValue(MySkillBookItem, PreservationDuration);
            typeof(SkillBookItem).GetField("LifeSkillType").SetValue(MySkillBookItem, LifeSkillType);
            typeof(SkillBookItem).GetField("LifeSkillTemplateId").SetValue(MySkillBookItem, LifeSkillTemplateId);
            typeof(SkillBookItem).GetField("CombatSkillType").SetValue(MySkillBookItem, CombatSkillType);
            typeof(SkillBookItem).GetField("CombatSkillTemplateId").SetValue(MySkillBookItem, CombatSkillTemplateId);
            typeof(SkillBookItem).GetField("LegacyPoint").SetValue(MySkillBookItem, LegacyPoint);
            typeof(SkillBookItem).GetField("ReferenceBooksWithBonus").SetValue(MySkillBookItem, ReferenceBooksWithBonus);
            return MySkillBookItem;
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
            CombatSkillItem MyCombatSkillItem = new CombatSkillItem();

            typeof(CombatSkillItem).GetField("TemplateId").SetValue(MyCombatSkillItem, TemplateId);
            typeof(CombatSkillItem).GetField("Name").SetValue(MyCombatSkillItem, Name);
            typeof(CombatSkillItem).GetField("Grade").SetValue(MyCombatSkillItem, Grade);
            typeof(CombatSkillItem).GetField("Desc").SetValue(MyCombatSkillItem, Desc);
            typeof(CombatSkillItem).GetField("Icon").SetValue(MyCombatSkillItem, Icon);
            typeof(CombatSkillItem).GetField("EquipType").SetValue(MyCombatSkillItem, EquipType);
            typeof(CombatSkillItem).GetField("Type").SetValue(MyCombatSkillItem, Type);
            typeof(CombatSkillItem).GetField("GridCost").SetValue(MyCombatSkillItem, GridCost);
            typeof(CombatSkillItem).GetField("SectId").SetValue(MyCombatSkillItem, SectId);
            typeof(CombatSkillItem).GetField("FiveElements").SetValue(MyCombatSkillItem, FiveElements);
            typeof(CombatSkillItem).GetField("BookId").SetValue(MyCombatSkillItem, BookId);
            typeof(CombatSkillItem).GetField("CanObtainByAdventure").SetValue(MyCombatSkillItem, CanObtainByAdventure);
            typeof(CombatSkillItem).GetField("IsNonPublic").SetValue(MyCombatSkillItem, IsNonPublic);
            typeof(CombatSkillItem).GetField("OrderIdInSect").SetValue(MyCombatSkillItem, OrderIdInSect);
            typeof(CombatSkillItem).GetField("UsingRequirement").SetValue(MyCombatSkillItem, UsingRequirement);
            typeof(CombatSkillItem).GetField("DirectEffectID").SetValue(MyCombatSkillItem, DirectEffectID);
            typeof(CombatSkillItem).GetField("ReverseEffectID").SetValue(MyCombatSkillItem, ReverseEffectID);
            typeof(CombatSkillItem).GetField("InheritAttainmentAdiitionRate").SetValue(MyCombatSkillItem, InheritAttainmentAdiitionRate);
            typeof(CombatSkillItem).GetField("PracticeType").SetValue(MyCombatSkillItem, PracticeType);
            typeof(CombatSkillItem).GetField("BreakStart").SetValue(MyCombatSkillItem, BreakStart);
            typeof(CombatSkillItem).GetField("BreakEnd").SetValue(MyCombatSkillItem, BreakEnd);
            typeof(CombatSkillItem).GetField("GoneMadInnerInjury").SetValue(MyCombatSkillItem, GoneMadInnerInjury);
            typeof(CombatSkillItem).GetField("GoneMadInjuredPart").SetValue(MyCombatSkillItem, GoneMadInjuredPart);
            typeof(CombatSkillItem).GetField("GoneMadInjuryValue").SetValue(MyCombatSkillItem, GoneMadInjuryValue);
            typeof(CombatSkillItem).GetField("GoneMadQiDisorder").SetValue(MyCombatSkillItem, GoneMadQiDisorder);
            typeof(CombatSkillItem).GetField("TotalObtainableNeili").SetValue(MyCombatSkillItem, TotalObtainableNeili);
            typeof(CombatSkillItem).GetField("ObtainedNeiliPerLoop").SetValue(MyCombatSkillItem, ObtainedNeiliPerLoop);
            typeof(CombatSkillItem).GetField("DestTypeWhileLooping").SetValue(MyCombatSkillItem, DestTypeWhileLooping);
            typeof(CombatSkillItem).GetField("TransferTypeWhileLooping").SetValue(MyCombatSkillItem, TransferTypeWhileLooping);
            typeof(CombatSkillItem).GetField("FiveElementChangePerLoop").SetValue(MyCombatSkillItem, FiveElementChangePerLoop);
            typeof(CombatSkillItem).GetField("SpecificGrids").SetValue(MyCombatSkillItem, SpecificGrids);
            typeof(CombatSkillItem).GetField("GenericGrid").SetValue(MyCombatSkillItem, GenericGrid);
            typeof(CombatSkillItem).GetField("AssetFileName").SetValue(MyCombatSkillItem, AssetFileName);
            typeof(CombatSkillItem).GetField("PrepareAnimation").SetValue(MyCombatSkillItem, PrepareAnimation);
            typeof(CombatSkillItem).GetField("CastAnimation").SetValue(MyCombatSkillItem, CastAnimation);
            typeof(CombatSkillItem).GetField("CastParticle").SetValue(MyCombatSkillItem, CastParticle);
            typeof(CombatSkillItem).GetField("CastPetAnimation").SetValue(MyCombatSkillItem, CastPetAnimation);
            typeof(CombatSkillItem).GetField("CastPetParticle").SetValue(MyCombatSkillItem, CastPetParticle);
            typeof(CombatSkillItem).GetField("DistanceWhenFourStepAnimation").SetValue(MyCombatSkillItem, DistanceWhenFourStepAnimation);
            typeof(CombatSkillItem).GetField("CastSoundEffect").SetValue(MyCombatSkillItem, CastSoundEffect);
            typeof(CombatSkillItem).GetField("PlayerCastBossSkillPrepareAni").SetValue(MyCombatSkillItem, PlayerCastBossSkillPrepareAni);
            typeof(CombatSkillItem).GetField("PlayerCastBossSkillAni").SetValue(MyCombatSkillItem, PlayerCastBossSkillAni);
            typeof(CombatSkillItem).GetField("PlayerCastBossSkillParticle").SetValue(MyCombatSkillItem, PlayerCastBossSkillParticle);
            typeof(CombatSkillItem).GetField("PlayerCastBossSkillSound").SetValue(MyCombatSkillItem, PlayerCastBossSkillSound);
            typeof(CombatSkillItem).GetField("PlayerCastBossSkillDistance").SetValue(MyCombatSkillItem, PlayerCastBossSkillDistance);
            typeof(CombatSkillItem).GetField("PrepareTotalProgress").SetValue(MyCombatSkillItem, PrepareTotalProgress);
            typeof(CombatSkillItem).GetField("NeedBodyPartTypes").SetValue(MyCombatSkillItem, NeedBodyPartTypes);
            typeof(CombatSkillItem).GetField("MobilityCost").SetValue(MyCombatSkillItem, MobilityCost);
            typeof(CombatSkillItem).GetField("BreathStanceTotalCost").SetValue(MyCombatSkillItem, BreathStanceTotalCost);
            typeof(CombatSkillItem).GetField("BaseInnerRatio").SetValue(MyCombatSkillItem, BaseInnerRatio);
            typeof(CombatSkillItem).GetField("InnerRatioChangeRange").SetValue(MyCombatSkillItem, InnerRatioChangeRange);
            typeof(CombatSkillItem).GetField("Penetrate").SetValue(MyCombatSkillItem, Penetrate);
            typeof(CombatSkillItem).GetField("DistanceAdditionWhenCast").SetValue(MyCombatSkillItem, DistanceAdditionWhenCast);
            typeof(CombatSkillItem).GetField("TrickCost").SetValue(MyCombatSkillItem, TrickCost);
            typeof(CombatSkillItem).GetField("WeaponDurableCost").SetValue(MyCombatSkillItem, WeaponDurableCost);
            typeof(CombatSkillItem).GetField("WugCost").SetValue(MyCombatSkillItem, WugCost);
            typeof(CombatSkillItem).GetField("MostFittingWeaponID").SetValue(MyCombatSkillItem, MostFittingWeaponID);
            typeof(CombatSkillItem).GetField("InjuryPartAtkRateDistribution").SetValue(MyCombatSkillItem, InjuryPartAtkRateDistribution);
            typeof(CombatSkillItem).GetField("TotalHit").SetValue(MyCombatSkillItem, TotalHit);
            typeof(CombatSkillItem).GetField("PerHitDamageRateDistribution").SetValue(MyCombatSkillItem, PerHitDamageRateDistribution);
            typeof(CombatSkillItem).GetField("HasAtkAcupointEffect").SetValue(MyCombatSkillItem, HasAtkAcupointEffect);
            typeof(CombatSkillItem).GetField("Poisons").SetValue(MyCombatSkillItem, Poisons);
            typeof(CombatSkillItem).GetField("EquipmentBreakOdds").SetValue(MyCombatSkillItem, EquipmentBreakOdds);
            typeof(CombatSkillItem).GetField("AddWugType").SetValue(MyCombatSkillItem, AddWugType);
            typeof(CombatSkillItem).GetField("AddBreakBodyFeature").SetValue(MyCombatSkillItem, AddBreakBodyFeature);
            typeof(CombatSkillItem).GetField("AddMoveSpeedOnCast").SetValue(MyCombatSkillItem, AddMoveSpeedOnCast);
            typeof(CombatSkillItem).GetField("AddHitOnCast").SetValue(MyCombatSkillItem, AddHitOnCast);
            typeof(CombatSkillItem).GetField("MobilityReduceSpeed").SetValue(MyCombatSkillItem, MobilityReduceSpeed);
            typeof(CombatSkillItem).GetField("MoveCostMobility").SetValue(MyCombatSkillItem, MoveCostMobility);
            typeof(CombatSkillItem).GetField("MaxJumpDistance").SetValue(MyCombatSkillItem, MaxJumpDistance);
            typeof(CombatSkillItem).GetField("JumpPrepareFrame").SetValue(MyCombatSkillItem, JumpPrepareFrame);
            typeof(CombatSkillItem).GetField("CanPartlyJump").SetValue(MyCombatSkillItem, CanPartlyJump);
            typeof(CombatSkillItem).GetField("JumpAni").SetValue(MyCombatSkillItem, JumpAni);
            typeof(CombatSkillItem).GetField("JumpParticle").SetValue(MyCombatSkillItem, JumpParticle);
            typeof(CombatSkillItem).GetField("JumpChangeDistanceFrame").SetValue(MyCombatSkillItem, JumpChangeDistanceFrame);
            typeof(CombatSkillItem).GetField("JumpChangeDistanceDuration").SetValue(MyCombatSkillItem, JumpChangeDistanceDuration);
            typeof(CombatSkillItem).GetField("ScoreBonusType").SetValue(MyCombatSkillItem, ScoreBonusType);
            typeof(CombatSkillItem).GetField("ScoreBonus").SetValue(MyCombatSkillItem, ScoreBonus);
            typeof(CombatSkillItem).GetField("AddOuterPenetrateResistOnCast").SetValue(MyCombatSkillItem, AddOuterPenetrateResistOnCast);
            typeof(CombatSkillItem).GetField("AddInnerPenetrateResistOnCast").SetValue(MyCombatSkillItem, AddInnerPenetrateResistOnCast);
            typeof(CombatSkillItem).GetField("AddAvoidOnCast").SetValue(MyCombatSkillItem, AddAvoidOnCast);
            typeof(CombatSkillItem).GetField("FightBackDamage").SetValue(MyCombatSkillItem, FightBackDamage);
            typeof(CombatSkillItem).GetField("BounceRateOfOuterInjury").SetValue(MyCombatSkillItem, BounceRateOfOuterInjury);
            typeof(CombatSkillItem).GetField("BounceRateOfInnerInjury").SetValue(MyCombatSkillItem, BounceRateOfInnerInjury);
            typeof(CombatSkillItem).GetField("ContinuousFrames").SetValue(MyCombatSkillItem, ContinuousFrames);
            typeof(CombatSkillItem).GetField("BounceDistance").SetValue(MyCombatSkillItem, BounceDistance);
            typeof(CombatSkillItem).GetField("RecoverBlockPercent").SetValue(MyCombatSkillItem, RecoverBlockPercent);
            typeof(CombatSkillItem).GetField("DefendAnimation").SetValue(MyCombatSkillItem, DefendAnimation);
            typeof(CombatSkillItem).GetField("DefendParticle").SetValue(MyCombatSkillItem, DefendParticle);
            typeof(CombatSkillItem).GetField("DefendSound").SetValue(MyCombatSkillItem, DefendSound);
            typeof(CombatSkillItem).GetField("FightBackAnimation").SetValue(MyCombatSkillItem, FightBackAnimation);
            typeof(CombatSkillItem).GetField("FightBackParticle").SetValue(MyCombatSkillItem, FightBackParticle);
            typeof(CombatSkillItem).GetField("FightBackSound").SetValue(MyCombatSkillItem, FightBackSound);
            typeof(CombatSkillItem).GetField("PropertyAddList").SetValue(MyCombatSkillItem, PropertyAddList);
            typeof(CombatSkillItem).GetField("OuterDamageSteps").SetValue(MyCombatSkillItem, OuterDamageSteps);
            typeof(CombatSkillItem).GetField("InnerDamageSteps").SetValue(MyCombatSkillItem, InnerDamageSteps);
            typeof(CombatSkillItem).GetField("FatalDamageStep").SetValue(MyCombatSkillItem, FatalDamageStep);
            typeof(CombatSkillItem).GetField("MindDamageStep").SetValue(MyCombatSkillItem, MindDamageStep);
            return MyCombatSkillItem;
        }

        public static SpecialEffectItem CreateSpecialEffectItem(short TemplateId, sbyte EffectActiveType, short MinEffectCount, short MaxEffectCount, sbyte RequireAttackPower, sbyte AiCostNeiliAllocationChanceDelayFrame, ESpecialEffectAiCostNeiliAllocationType AiCostNeiliAllocationType, ESpecialEffectJumpType JumpType, int[] AffectRequirePower, string Name, short SkillTemplateId, string[] ShortDesc, string[] Desc, string[] PlayerCastBossSkillDesc, string ClassName)
        {
            SpecialEffectItem MySpecialEffectItem = new SpecialEffectItem();


            typeof(SpecialEffectItem).GetField("TemplateId").SetValue(MySpecialEffectItem, TemplateId);
            typeof(SpecialEffectItem).GetField("EffectActiveType").SetValue(MySpecialEffectItem, EffectActiveType);
            typeof(SpecialEffectItem).GetField("MinEffectCount").SetValue(MySpecialEffectItem, MinEffectCount);
            typeof(SpecialEffectItem).GetField("MaxEffectCount").SetValue(MySpecialEffectItem, MaxEffectCount);
            typeof(SpecialEffectItem).GetField("RequireAttackPower").SetValue(MySpecialEffectItem, RequireAttackPower);
            typeof(SpecialEffectItem).GetField("AiCostNeiliAllocationChanceDelayFrame").SetValue(MySpecialEffectItem, AiCostNeiliAllocationChanceDelayFrame);
            typeof(SpecialEffectItem).GetField("AiCostNeiliAllocationType").SetValue(MySpecialEffectItem, AiCostNeiliAllocationType);
            typeof(SpecialEffectItem).GetField("JumpType").SetValue(MySpecialEffectItem, JumpType);
            typeof(SpecialEffectItem).GetField("AffectRequirePower").SetValue(MySpecialEffectItem, AffectRequirePower);
            typeof(SpecialEffectItem).GetField("Name").SetValue(MySpecialEffectItem, Name);
            typeof(SpecialEffectItem).GetField("SkillTemplateId").SetValue(MySpecialEffectItem, SkillTemplateId);
            typeof(SpecialEffectItem).GetField("ShortDesc").SetValue(MySpecialEffectItem, ShortDesc);
            typeof(SpecialEffectItem).GetField("Desc").SetValue(MySpecialEffectItem, Desc);
            typeof(SpecialEffectItem).GetField("PlayerCastBossSkillDesc").SetValue(MySpecialEffectItem, PlayerCastBossSkillDesc);
            typeof(SpecialEffectItem).GetField("ClassName").SetValue(MySpecialEffectItem, ClassName);

            return MySpecialEffectItem;
        }


    }
}

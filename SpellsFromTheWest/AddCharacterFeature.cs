using Config;
using Config.ConfigCells.Character;
using GameData.Domains.SpecialEffect.SpellsFromTheWest.Misc;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpellsFromTheWestBackend
{
    internal class AddCharacterFeature
    {
        public static void DoAdd()
        {
            STD.TemplateHet =
            AddSingleCharacterFeature(Name: "难言之隐",  Desc: "受难以言说的病症折磨…",
                featureMedals: new FeatureMedals[3] {
                    new FeatureMedals("dec"),
                    new FeatureMedals("dec"),
                    new FeatureMedals() },
                CanBeExchanged: true,
                Strength: -10, Dexterity: -10, Concentration: -10,
                HitRateStrength: -10, HitRateSpeed: -10, HitRateTechnique: -10,
                AvoidRateStrength: -10, AvoidRateSpeed: -10, AvoidRateTechnique: -10,
                MoveSpeed: -10, ResistOfColdPoison: -20, ResistOfGloomyPoison: -20, ResistOfHotPoison: -20, ResistOfRedPoison: -20,
                ResistOfIllusoryPoison: -20, ResistOfRottenPoison: -20);
            STD.TemplateHomo =
            AddSingleCharacterFeature(Name: "难言之隐",  Desc: "受难以言说的病症折磨…",
                featureMedals: new FeatureMedals[3] {
                    new FeatureMedals("dec"),
                    new FeatureMedals("dec"),
                    new FeatureMedals() },
                CanBeExchanged: true,
                Strength: -10, Dexterity: -10, Concentration: -10,
                HitRateStrength: -10, HitRateSpeed: -10, HitRateTechnique: -10,
                AvoidRateStrength: -10, AvoidRateSpeed: -10, AvoidRateTechnique: -10,
                MoveSpeed: -10, ResistOfColdPoison: -20, ResistOfGloomyPoison: -20, ResistOfHotPoison: -20, ResistOfRedPoison: -20,
                ResistOfIllusoryPoison: -20, ResistOfRottenPoison: -20);
        }
        public static short AddSingleCharacterFeature(
            string Name,
            string Desc,
            FeatureMedals[] featureMedals,
            ECharacterFeatureType Type = ECharacterFeatureType.Special,
            bool Hidden = false,
            sbyte Level = 0,
            ECharacterFeatureInfectedType infectedType = ECharacterFeatureInfectedType.NotInfected,
            bool CanBeModified = false,
            bool CanBeExchanged = false,
            bool Mergeable = false,
            bool Basic = false,
            bool Inscribable = false,
            bool SoulTransform = false,
            bool CanCrossArchive = false,
            bool InheritableThroughSamsara = false,
            
            sbyte Gender = -1,
            sbyte CandidateGroupId = -1,
            short FavorabilityIncrementFactor = 100,
            short FavorabilityDerementFactor = 100,
            short AdoreMultiplePeopleChanceFactor = 100,
            sbyte[] CombatSkillPowerBonuses = null,
            sbyte[] CombatSkillSlotBonuses = null,

            short Strength = 0,
            short Vitality = 0,
            short Dexterity = 0,
            short Energy = 0,
            short Intelligence = 0,
            short Concentration = 0,
            short Fertility = 0,
            short HitRateStrength = 0,
            short HitRateTechnique = 0,
            short HitRateSpeed = 0,
            short HitRateMind = 0,
            short PenetrateOfOuter = 0,
            int AvoidRateStrength = 0,
            int AvoidRateTechnique = 0,
            int AvoidRateSpeed = 0,
            int AvoidRateMind = 0,
            int PenetrateResistOfOuter = 0,
            int PenetrateResistOfInner = 0,
            short RecoveryOfStance = 0,
            short RecoveryOfBreath = 0,
            short MoveSpeed = 0,
            short RecoveryOfFlaw = 0,
            short CastSpeed = 0,
            short RecoveryOfBlockedAcupoint = 0,
            short WeaponSwitchSpeed = 0,
            short AttackSpeed = 0,
            short InnerRatio = 0,
            short RecoveryOfQiDisorder = 0,
            int ResistOfHotPoison = 0,
            int ResistOfGloomyPoison = 0,
            int ResistOfColdPoison = 0,
            int ResistOfRedPoison = 0,
            int ResistOfRottenPoison = 0,
            int ResistOfIllusoryPoison = 0
        )
        {
            if (CombatSkillPowerBonuses == null)
            {
                CombatSkillPowerBonuses = new sbyte[5] {0,0,0,0,0};
            }
            if (CombatSkillSlotBonuses == null)
            {
                CombatSkillSlotBonuses = new sbyte[5] { 0, 0, 0, 0, 0 };

            }
            var result = new Config.CharacterFeatureItem();
            typeof(CharacterFeatureItem).GetField("Name").SetValue(result, Name);
            typeof(CharacterFeatureItem).GetField("Desc").SetValue(result, Desc);
            typeof(CharacterFeatureItem).GetField("Type").SetValue(result, Type);
            typeof(CharacterFeatureItem).GetField("FeatureMedals").SetValue(result, featureMedals);
            typeof(CharacterFeatureItem).GetField("Hidden").SetValue(result, Hidden);
            typeof(CharacterFeatureItem).GetField("Level").SetValue(result, Level);
            typeof(CharacterFeatureItem).GetField("InfectedType").SetValue(result, infectedType);
            typeof(CharacterFeatureItem).GetField("CanBeModified").SetValue(result, CanBeModified);
            typeof(CharacterFeatureItem).GetField("CanBeExchanged").SetValue(result, CanBeExchanged);
            typeof(CharacterFeatureItem).GetField("Mergeable").SetValue(result, Mergeable);
            typeof(CharacterFeatureItem).GetField("Basic").SetValue(result, Basic);
            typeof(CharacterFeatureItem).GetField("Inscribable").SetValue(result, Inscribable);
            typeof(CharacterFeatureItem).GetField("SoulTransform").SetValue(result, SoulTransform);
            typeof(CharacterFeatureItem).GetField("CanCrossArchive").SetValue(result, CanCrossArchive);
            typeof(CharacterFeatureItem).GetField("InheritableThroughSamsara").SetValue(result, InheritableThroughSamsara);
            typeof(CharacterFeatureItem).GetField("Gender").SetValue(result, Gender);
            typeof(CharacterFeatureItem).GetField("CandidateGroupId").SetValue(result, CandidateGroupId);
            typeof(CharacterFeatureItem).GetField("FavorabilityIncrementFactor").SetValue(result, FavorabilityIncrementFactor);
            typeof(CharacterFeatureItem).GetField("FavorabilityDecrementFactor").SetValue(result, FavorabilityDerementFactor);
            typeof(CharacterFeatureItem).GetField("AdoreMultiplePeopleChanceFactor").SetValue(result, AdoreMultiplePeopleChanceFactor);
            typeof(CharacterFeatureItem).GetField("CombatSkillPowerBonuses").SetValue(result, CombatSkillPowerBonuses);
            typeof(CharacterFeatureItem).GetField("CombatSkillSlotBonuses").SetValue(result, CombatSkillSlotBonuses);
            typeof(CharacterFeatureItem).GetField("Strength").SetValue(result, Strength);
            typeof(CharacterFeatureItem).GetField("Vitality").SetValue(result, Vitality);
            typeof(CharacterFeatureItem).GetField("Dexterity").SetValue(result, Dexterity);
            typeof(CharacterFeatureItem).GetField("Energy").SetValue(result, Energy);
            typeof(CharacterFeatureItem).GetField("Intelligence").SetValue(result, Intelligence);
            typeof(CharacterFeatureItem).GetField("Concentration").SetValue(result, Concentration);
            typeof(CharacterFeatureItem).GetField("Fertility").SetValue(result, Fertility);
            typeof(CharacterFeatureItem).GetField("HitRateStrength").SetValue(result, HitRateStrength);
            typeof(CharacterFeatureItem).GetField("HitRateTechnique").SetValue(result, HitRateTechnique);
            typeof(CharacterFeatureItem).GetField("HitRateSpeed").SetValue(result, HitRateSpeed);
            typeof(CharacterFeatureItem).GetField("HitRateMind").SetValue(result, HitRateMind);
            typeof(CharacterFeatureItem).GetField("PenetrateOfOuter").SetValue(result, PenetrateOfOuter);
            typeof(CharacterFeatureItem).GetField("AvoidRateStrength").SetValue(result, AvoidRateStrength);
            typeof(CharacterFeatureItem).GetField("AvoidRateTechnique").SetValue(result, AvoidRateTechnique);
            typeof(CharacterFeatureItem).GetField("AvoidRateSpeed").SetValue(result, AvoidRateSpeed);
            typeof(CharacterFeatureItem).GetField("AvoidRateMind").SetValue(result, AvoidRateMind);
            typeof(CharacterFeatureItem).GetField("PenetrateResistOfOuter").SetValue(result, PenetrateResistOfOuter);
            typeof(CharacterFeatureItem).GetField("PenetrateResistOfInner").SetValue(result, PenetrateResistOfInner);
            typeof(CharacterFeatureItem).GetField("RecoveryOfStance").SetValue(result, RecoveryOfStance);
            typeof(CharacterFeatureItem).GetField("RecoveryOfBreath").SetValue(result, RecoveryOfBreath);
            typeof(CharacterFeatureItem).GetField("MoveSpeed").SetValue(result, MoveSpeed);
            typeof(CharacterFeatureItem).GetField("RecoveryOfFlaw").SetValue(result, RecoveryOfFlaw);
            typeof(CharacterFeatureItem).GetField("CastSpeed").SetValue(result, CastSpeed);
            typeof(CharacterFeatureItem).GetField("RecoveryOfBlockedAcupoint").SetValue(result, RecoveryOfBlockedAcupoint);
            typeof(CharacterFeatureItem).GetField("WeaponSwitchSpeed").SetValue(result, WeaponSwitchSpeed);
            typeof(CharacterFeatureItem).GetField("AttackSpeed").SetValue(result, AttackSpeed);
            typeof(CharacterFeatureItem).GetField("InnerRatio").SetValue(result, InnerRatio);
            typeof(CharacterFeatureItem).GetField("RecoveryOfQiDisorder").SetValue(result, RecoveryOfQiDisorder);
            typeof(CharacterFeatureItem).GetField("ResistOfHotPoison").SetValue(result, ResistOfHotPoison);
            typeof(CharacterFeatureItem).GetField("ResistOfGloomyPoison").SetValue(result, ResistOfGloomyPoison);
            typeof(CharacterFeatureItem).GetField("ResistOfColdPoison").SetValue(result, ResistOfColdPoison);
            typeof(CharacterFeatureItem).GetField("ResistOfRedPoison").SetValue(result, ResistOfRedPoison);
            typeof(CharacterFeatureItem).GetField("ResistOfRottenPoison").SetValue(result, ResistOfRottenPoison);
            typeof(CharacterFeatureItem).GetField("ResistOfIllusoryPoison").SetValue(result, ResistOfIllusoryPoison);

            FieldInfo fieldInfo = typeof(Config.CharacterFeature).GetField("_dataArray", BindingFlags.NonPublic | BindingFlags.Instance);
            List<CharacterFeatureItem> ItemList = (List <CharacterFeatureItem>) fieldInfo.GetValue(Config.CharacterFeature.Instance);
            short TemplateId = (short)ItemList.Count;            
            typeof(CharacterFeatureItem).GetField("TemplateId").SetValue(result, TemplateId);

            ItemList.Add(result);
            return TemplateId;
        }
    }
}

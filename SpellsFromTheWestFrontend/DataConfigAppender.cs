using Config;
using Config.ConfigCells.Character;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item;
using GameData.Domains.Taiwu.LifeSkillCombat.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using Newtonsoft.Json;
using Microsoft.VisualBasic.FileIO;
using System.IO;
namespace SpellsFromTheWest
{
    internal class DataConfigAppender
    {
        public static void CreateAndAppendSpecialEffectItemFromStrings(string SkillTemplateId,
            string TemplateId, string EffectActiveType, string MinEffectCount, string MaxEffectCount, string Name, string ShortDesc,
            string Desc, string PlayerCastBossSkillDesc, string ClassName,
            string RequireAttackPower = "-1", string AiCostNeiliAllocationChanceDelayFrame = "-1",
            string AiCostNeiliAllocationType = "-1",
            string JumpType = "-1",
            string AffectRequirePower = "[]")
        {
            
            SpecialEffectItem Item = DataConfigAppenderHelpers.CreateSpecialEffectItem(
                SkillTemplateId: short.Parse(SkillTemplateId),
                TemplateId: short.Parse(TemplateId),
                EffectActiveType: sbyte.Parse(EffectActiveType),
                MinEffectCount: short.Parse(MinEffectCount),
                MaxEffectCount: short.Parse(MaxEffectCount),
                Name: (Name),
                ShortDesc: DataConfigAppenderHelpers.parseArrString(ShortDesc),
                Desc: DataConfigAppenderHelpers.parseArrString(Desc),
                PlayerCastBossSkillDesc: DataConfigAppenderHelpers.parseArrString(PlayerCastBossSkillDesc),
                ClassName: DataConfigAppenderHelpers.ParseString(ClassName),
                RequireAttackPower: sbyte.Parse(RequireAttackPower),
                AiCostNeiliAllocationChanceDelayFrame: sbyte.Parse(AiCostNeiliAllocationChanceDelayFrame),
                AiCostNeiliAllocationType: (ESpecialEffectAiCostNeiliAllocationType)Int32.Parse(AiCostNeiliAllocationType),
                JumpType: (ESpecialEffectJumpType)Int32.Parse(JumpType),
                AffectRequirePower: DataConfigAppenderHelpers.parseArrInt(AffectRequirePower)
            );
            DataConfigAppenderHelpers.AddSpecialEffectItemToConfig(TemplateId, Name+TemplateId, Item);
        }

        public static void CreateAndAppendCombatSkillItemFromStrings(string TemplateId, string Name, string Grade, string Desc,
            string Icon, string EquipType, string Type, string GridCost, string SectId, string FiveElements, string BookId,
            string CanObtainByAdventure, string IsNonPublic, string UsingRequirement,
            string DirectEffectID, string ReverseEffectID, string InheritAttainmentAdiitionRate, string PracticeType, string BreakStart,
            string BreakEnd, string GoneMadInnerInjury, string GoneMadInjuredPart, string GoneMadInjuryValue,
            string GoneMadQiDisorder, string TotalObtainableNeili, string ObtainedNeiliPerLoop, string DestTypeWhileLooping,
            string TransferTypeWhileLooping, string FiveElementChangePerLoop, string SpecificGrids, string GenericGrid,
            string AssetFileName, string PrepareAnimation, string CastAnimation, string CastParticle, string CastPetAnimation,
            string CastPetParticle, string DistanceWhenFourStepAnimation, string CastSoundEffect, string PlayerCastBossSkillPrepareAni,
            string PlayerCastBossSkillAni, string PlayerCastBossSkillParticle, string PlayerCastBossSkillSound,
            string PlayerCastBossSkillDistance, string PrepareTotalProgress, string NeedBodyPartTypes, string MobilityCost,
            string BreathStanceTotalCost, string BaseInnerRatio, string InnerRatioChangeRange, string Penetrate,
            string DistanceAdditionWhenCast, string TrickCost, string WeaponDurableCost, string WugCost,
            string MostFittingWeaponID, string InjuryPartAtkRateDistribution, string TotalHit, string PerHitDamageRateDistribution,
            string HasAtkAcupointEffect, string Poisons, string EquipmentBreakOdds, string AddWugType,
            string AddBreakBodyFeature, string AddMoveSpeedOnCast, string AddHitOnCast, string MobilityReduceSpeed,
            string MoveCostMobility, string MaxJumpDistance, string JumpPrepareFrame, string CanPartlyJump, string JumpAni,
            string JumpParticle, string JumpChangeDistanceFrame, string JumpChangeDistanceDuration, string ScoreBonusType,
            string ScoreBonus, string AddOuterPenetrateResistOnCast, string AddInnerPenetrateResistOnCast,
            string AddAvoidOnCast, string FightBackDamage, string BounceRateOfOuterInjury, string BounceRateOfInnerInjury,
            string ContinuousFrames, string BounceDistance, string DefendAnimation, string DefendParticle,
            string DefendSound, string FightBackAnimation, string FightBackParticle, string FightBackSound, string PropertyAddList,
            string OuterDamageSteps = null, string InnerDamageSteps = null, string FatalDamageStep = null, string MindDamageStep = null,
            string RecoverBlockPercent = null, string OrderIdInSect = null)
        {


            var item = DataConfigAppenderHelpers.CreateCombatSkillItem(
                TemplateId: short.Parse(TemplateId),
                Name: Name,
                Grade: sbyte.Parse(Grade),
                Desc: Desc,
                Icon: Icon,
                EquipType: sbyte.Parse(EquipType),
                Type: sbyte.Parse(Type),
                GridCost: sbyte.Parse(GridCost),
                SectId: sbyte.Parse(SectId),
                FiveElements: sbyte.Parse(FiveElements),
                BookId: short.Parse(BookId),
                CanObtainByAdventure: Boolean.Parse(CanObtainByAdventure),
                IsNonPublic: Boolean.Parse(IsNonPublic),
                UsingRequirement: DataConfigAppenderHelpers.parseListPropertyValue(UsingRequirement),
                DirectEffectID: int.Parse(DirectEffectID),
                ReverseEffectID: int.Parse(ReverseEffectID),
                InheritAttainmentAdiitionRate: byte.Parse(InheritAttainmentAdiitionRate),
                PracticeType: sbyte.Parse(PracticeType),
                BreakStart: DataConfigAppenderHelpers.ParseString(BreakStart),
                BreakEnd: DataConfigAppenderHelpers.ParseString(BreakEnd),
                GoneMadInnerInjury: bool.Parse(GoneMadInnerInjury),
                GoneMadInjuredPart: DataConfigAppenderHelpers.parseListSByte(GoneMadInjuredPart),
                GoneMadInjuryValue: sbyte.Parse(GoneMadInjuryValue),
                GoneMadQiDisorder: short.Parse(GoneMadQiDisorder),
                TotalObtainableNeili: short.Parse(TotalObtainableNeili),
                ObtainedNeiliPerLoop: short.Parse(ObtainedNeiliPerLoop),
                DestTypeWhileLooping: sbyte.Parse(DestTypeWhileLooping),
                TransferTypeWhileLooping: sbyte.Parse(TransferTypeWhileLooping),
                FiveElementChangePerLoop: sbyte.Parse(FiveElementChangePerLoop),
                SpecificGrids: DataConfigAppenderHelpers.parseArrSByte(SpecificGrids),
                GenericGrid: sbyte.Parse(GenericGrid),
                AssetFileName: DataConfigAppenderHelpers.ParseString(AssetFileName),
                PrepareAnimation: DataConfigAppenderHelpers.ParseString(PrepareAnimation),
                CastAnimation: DataConfigAppenderHelpers.ParseString(CastAnimation),
                CastParticle: DataConfigAppenderHelpers.ParseString(CastParticle),
                CastPetAnimation: DataConfigAppenderHelpers.ParseString(CastPetAnimation),
                CastPetParticle: DataConfigAppenderHelpers.ParseString(CastPetParticle),
                DistanceWhenFourStepAnimation: DataConfigAppenderHelpers.parseArrShort(DistanceWhenFourStepAnimation),
                CastSoundEffect: DataConfigAppenderHelpers.ParseString(CastSoundEffect),
                PlayerCastBossSkillPrepareAni: DataConfigAppenderHelpers.ParseString(PlayerCastBossSkillPrepareAni),
                PlayerCastBossSkillAni: DataConfigAppenderHelpers.ParseString(PlayerCastBossSkillAni),
                PlayerCastBossSkillParticle: DataConfigAppenderHelpers.ParseString(PlayerCastBossSkillParticle),
                PlayerCastBossSkillSound: DataConfigAppenderHelpers.ParseString(PlayerCastBossSkillSound),
                PlayerCastBossSkillDistance: DataConfigAppenderHelpers.parseArrShort(DataConfigAppenderHelpers.ParseString(PlayerCastBossSkillDistance)),
                PrepareTotalProgress: int.Parse(PrepareTotalProgress),
                NeedBodyPartTypes: DataConfigAppenderHelpers.parseListSByte(NeedBodyPartTypes),
                MobilityCost: short.Parse(MobilityCost),
                BreathStanceTotalCost: sbyte.Parse(BreathStanceTotalCost),
                BaseInnerRatio: sbyte.Parse(BaseInnerRatio),
                InnerRatioChangeRange: sbyte.Parse(InnerRatioChangeRange),
                Penetrate: short.Parse(Penetrate),
                DistanceAdditionWhenCast: short.Parse(DistanceAdditionWhenCast),
                TrickCost: DataConfigAppenderHelpers.parseListNeedTrick(TrickCost),
                WeaponDurableCost: sbyte.Parse(WeaponDurableCost),
                WugCost: sbyte.Parse(WugCost),
                MostFittingWeaponID: short.Parse(MostFittingWeaponID),
                InjuryPartAtkRateDistribution: DataConfigAppenderHelpers.parseArrSByte(InjuryPartAtkRateDistribution),
                TotalHit: short.Parse(TotalHit),
                PerHitDamageRateDistribution: DataConfigAppenderHelpers.parseArrSByte(PerHitDamageRateDistribution),
                HasAtkAcupointEffect: bool.Parse(HasAtkAcupointEffect),
                Poisons: DataConfigAppenderHelpers.parsePoisonsAndLevels(Poisons),
                EquipmentBreakOdds: sbyte.Parse(EquipmentBreakOdds),
                AddWugType: sbyte.Parse(AddWugType),
                AddBreakBodyFeature: DataConfigAppenderHelpers.parseArrShort(AddBreakBodyFeature),
                AddMoveSpeedOnCast: short.Parse(AddMoveSpeedOnCast),
                AddHitOnCast: DataConfigAppenderHelpers.parseArrShort(AddHitOnCast),
                MobilityReduceSpeed: sbyte.Parse(MobilityReduceSpeed),
                MoveCostMobility: short.Parse(MoveCostMobility),
                MaxJumpDistance: sbyte.Parse(MaxJumpDistance),
                JumpPrepareFrame: sbyte.Parse(JumpPrepareFrame),
                CanPartlyJump: bool.Parse(CanPartlyJump),
                JumpAni: DataConfigAppenderHelpers.parseArrString(JumpAni),
                JumpParticle: DataConfigAppenderHelpers.parseArrString(JumpParticle),
                JumpChangeDistanceFrame: short.Parse(JumpChangeDistanceFrame),
                JumpChangeDistanceDuration: short.Parse(JumpChangeDistanceDuration),
                ScoreBonusType: sbyte.Parse(ScoreBonusType),
                ScoreBonus: short.Parse(ScoreBonus),
                AddOuterPenetrateResistOnCast: short.Parse(AddOuterPenetrateResistOnCast),
                AddInnerPenetrateResistOnCast: short.Parse(AddInnerPenetrateResistOnCast),
                AddAvoidOnCast: DataConfigAppenderHelpers.parseArrShort(AddAvoidOnCast),
                FightBackDamage: short.Parse(FightBackDamage),
                BounceRateOfOuterInjury: short.Parse(BounceRateOfOuterInjury),
                BounceRateOfInnerInjury: short.Parse(BounceRateOfInnerInjury),
                ContinuousFrames: short.Parse(ContinuousFrames),
                BounceDistance: short.Parse(BounceDistance),
                DefendAnimation: DataConfigAppenderHelpers.ParseString(DefendAnimation),
                DefendParticle: DataConfigAppenderHelpers.ParseString(DefendParticle),
                DefendSound: DataConfigAppenderHelpers.ParseString(DefendSound),
                FightBackAnimation: DataConfigAppenderHelpers.ParseString(FightBackAnimation),
                FightBackParticle: DataConfigAppenderHelpers.ParseString(FightBackParticle),
                FightBackSound: DataConfigAppenderHelpers.ParseString(FightBackSound),
                PropertyAddList: DataConfigAppenderHelpers.parseListPropertyValue(PropertyAddList),
                OuterDamageSteps: OuterDamageSteps == null ? new int[] { 0, 0, 0, 0, 0, 0, 0 } : DataConfigAppenderHelpers.parseArrInt(OuterDamageSteps),
                InnerDamageSteps: InnerDamageSteps == null ? new int[] { 0, 0, 0, 0, 0, 0, 0 } : DataConfigAppenderHelpers.parseArrInt(InnerDamageSteps),
                FatalDamageStep: FatalDamageStep == null ? 0 : int.Parse(FatalDamageStep),
                MindDamageStep: MindDamageStep == null ? 0 : int.Parse(MindDamageStep),

                RecoverBlockPercent: RecoverBlockPercent == null ? 0 : int.Parse(RecoverBlockPercent),
                OrderIdInSect: OrderIdInSect == null ? (sbyte)0 : sbyte.Parse(OrderIdInSect)

                );
            DataConfigAppenderHelpers.AddCombatSkillItemToConfig(TemplateId, Name, item);
        }




        public static void CreateAndAppendSkillBookItemFromStrings(string TemplateId, string Name, string ItemType, string ItemSubType,
            string Grade, string Icon, string Desc, string Transferable, string Stackable, string Wagerable, string Refinable,
            string Poisonable, string Repairable, string MaxDurability, string BaseWeight, string BaseValue, string BasePrice,
            string HappinessChange, string FavorabilityChange, string GiftLevel, string DropRate, string ResourceType,
            string PreservationDuration, string LifeSkillType, string LifeSkillTemplateId, string CombatSkillType,
            string CombatSkillTemplateId, string LegacyPoint, string ReferenceBooksWithBonus)
        {
            var Item = DataConfigAppenderHelpers.CreateSkillBookItem(

                TemplateId: short.Parse(TemplateId),
                Name: (Name),
                ItemType: sbyte.Parse(ItemType),
                ItemSubType: short.Parse(ItemSubType),
                Grade: sbyte.Parse(Grade),
                Icon: (Icon),
                Desc: (Desc),
                Transferable: bool.Parse(Transferable),
                Stackable: bool.Parse(Stackable),
                Wagerable: bool.Parse(Wagerable),
                Refinable: bool.Parse(Refinable),
                Poisonable: bool.Parse(Poisonable),
                Repairable: bool.Parse(Repairable),
                MaxDurability: short.Parse(MaxDurability),
                BaseWeight: int.Parse(BaseWeight),
                BaseValue: int.Parse(BaseValue),
                BasePrice: int.Parse(BasePrice),
                BaseHappinessChange: sbyte.Parse(HappinessChange),
                BaseFavorabilityChange: int.Parse(FavorabilityChange),
                GiftLevel: sbyte.Parse(GiftLevel),
                DropRate: sbyte.Parse(DropRate),
                ResourceType: sbyte.Parse(ResourceType),
                PreservationDuration: short.Parse(PreservationDuration),
                LifeSkillType: sbyte.Parse(LifeSkillType),
                LifeSkillTemplateId: short.Parse(LifeSkillTemplateId),
                CombatSkillType: sbyte.Parse(CombatSkillType),
                CombatSkillTemplateId: short.Parse(CombatSkillTemplateId),
                LegacyPoint: short.Parse(LegacyPoint),
                ReferenceBooksWithBonus: DataConfigAppenderHelpers.parseArrShort(ReferenceBooksWithBonus).ToList()
                );
            DataConfigAppenderHelpers.AddSkillBookToConfig(TemplateId, Name, Item);
        }

    }
}

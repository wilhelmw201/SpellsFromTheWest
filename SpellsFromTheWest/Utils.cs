using GameData.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
{0: 'Strength', 1: 'Dexterity', 2: 'Concentration', 3: 'Vitality', 4: 'Energy', 5: 'Intelligence', 
6: 'HitRateStrength', 7: 'HitRateTechnique', 8: 'HitRateSpeed', 9: 'HitRateMind', 10: 'PenetrateOfOuter', 
11: 'PenetrateOfInner', 12: 'AvoidRateStrength', 13: 'AvoidRateTechnique', 14: 'AvoidRateSpeed', 
15: 'AvoidRateMind', 16: 'PenetrateResistOfOuter', 17: 'PenetrateResistOfInner', 18: 'RecoveryOfStance', 
19: 'RecoveryOfBreath', 20: 'MoveSpeed', 21: 'RecoveryOfFlaw', 22: 'CastSpeed', 23: 'RecoveryOfBlockedAcupoint', 
24: 'WeaponSwitchSpeed', 25: 'AttackSpeed', 26: 'InnerRatio', 27: 'RecoveryOfQiDisorder', 28: 'ResistOfHotPoison', 
29: 'ResistOfGloomyPoison', 30: 'ResistOfColdPoison', 31: 'ResistOfRedPoison', 32: 'ResistOfRottenPoison', 
33: 'ResistOfIllusoryPoison', 34: 'QualificationMusic', 35: 'QualificationChess', 36: 'QualificationPoem',
37: 'QualificationPainting', 38: 'QualificationMath', 39: 'QualificationAppraisal', 40: 'QualificationForging', 
41: 'QualificationWoodworking', 42: 'QualificationMedicine', 43: 'QualificationToxicology', 
44: 'QualificationWeaving', 45: 'QualificationJade', 46: 'QualificationTaoism', 47: 'QualificationBuddhism', 
48: 'QualificationCooking', 49: 'QualificationEclectic', 50: 'AttainmentMusic', 51: 'AttainmentChess', 
52: 'AttainmentPoem', 53: 'AttainmentPainting', 54: 'AttainmentMath', 55: 'AttainmentAppraisal', 
56: 'AttainmentForging', 57: 'AttainmentWoodworking', 58: 'AttainmentMedicine', 59: 'AttainmentToxicology', 
60: 'AttainmentWeaving', 61: 'AttainmentJade', 62: 'AttainmentTaoism', 63: 'AttainmentBuddhism', 
64: 'AttainmentCooking', 65: 'AttainmentEclectic', 66: 'QualificationNeigong', 67: 'QualificationPosing', 
68: 'QualificationStunt', 69: 'QualificationFistAndPalm', 70: 'QualificationFinger', 71: 'QualificationLeg', 
72: 'QualificationThrow', 73: 'QualificationSword', 74: 'QualificationBlade', 75: 'QualificationPolearm', 
76: 'QualificationSpecial', 77: 'QualificationWhip', 78: 'QualificationControllableShot', 79: 'QualificationCombatMusic', 
80: 'AttainmentNeigong', 81: 'AttainmentPosing', 82: 'AttainmentStunt', 83: 'AttainmentFistAndPalm', 
84: 'AttainmentFinger', 85: 'AttainmentLeg', 86: 'AttainmentThrow', 87: 'AttainmentSword', 
88: 'AttainmentBlade', 89: 'AttainmentPolearm', 90: 'AttainmentSpecial', 91: 'AttainmentWhip', 
92: 'AttainmentControllableShot', 93: 'AttainmentCombatMusic', 94: 'PersonalityCalm', 
95: 'PersonalityClever', 96: 'PersonalityEnthusiastic', 97: 'PersonalityBrave', 
98: 'PersonalityFirm', 99: 'PersonalityLucky', 100: 'PersonalityPerceptive', 
101: 'Attraction', 102: 'Fertility', 103: 'HobbyChangingPeriod', 104: 'MaxHealth', 105: 'MaxNeili', 106: 'Count'}
 */

namespace SpellsFromTheWestBackend
{
    internal class Utils
    {
        static Boolean _debugging = true;
        public static void MyLog(string message) 
        { 
            if (_debugging)
            {
                AdaptableLog.Info(message);
            }
        }
        public static void DebugBreak()
        {
            if (!_debugging) return;
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e) { }
        }

 

    }
}

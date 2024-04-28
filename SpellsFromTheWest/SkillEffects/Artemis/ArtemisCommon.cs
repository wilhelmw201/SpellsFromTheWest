using Config;
using GameData.Common;
using GameData.DomainEvents;
using GameData.Domains;
using GameData.Domains.Combat;
using GameData.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SpellsFromTheWestBackend.SkillEffects.Artemis;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace SpellsFromTheWestBackend.SkillEffects.Artemis
{
    internal class ArtemisCommon
    {


        public delegate void OnArtemisHuntComplete(DataContext context, int attackerId, int defenderId, sbyte bodyPart);
        private static OnArtemisHuntComplete _ArtemisHuntCompleteCallback;
        private static float _difficultyMultiplier = 1;
        private static int _huntCount = 1;
        public static void SetHuntParameter(float difficultyMultiplier=-1, int huntCount=-1)
        {
            if(_difficultyMultiplier > 0)
            {
                _difficultyMultiplier = difficultyMultiplier;
            }
            if(_huntCount > 0)
            {
                _huntCount = huntCount;
            }
        }
        public static void ArtemisCommonOnCombatStart(DataContext context)
        {
            Utils.DebugBreak();
            // maybe its better to patch the combat start function...
            Events.OnAddDirectDamageValue gameDmgDelegate = (Events.OnAddDirectDamageValue)(typeof(Events).GetField("_handlersAddDirectDamageValue"
                , BindingFlags.NonPublic | BindingFlags.Static).GetValue(null));
            bool shouldAdd = true;
            if (gameDmgDelegate != null)
            {
                foreach (var registeredCb in gameDmgDelegate.GetInvocationList())
                {
                    if (registeredCb.Method.Name == "ArtemisOnAddDirectDamageValue")
                    {
                        shouldAdd = false;
                        break;
                    }
                }
            }

            if (shouldAdd)
            {
                Events.RegisterHandler_AddDirectDamageValue(ArtemisOnAddDirectDamageValue);
                Utils.MyLog("Artemis common combat start register done");
            }
            else
            {
                Utils.MyLog("Artemis common combat start register skipped");
            }
        }
        public static void ArtemisCommonRegisterOnHuntComplete(OnArtemisHuntComplete handler)
        {
            bool shouldAdd = true;

            if (_ArtemisHuntCompleteCallback != null)
            {
                foreach (var registeredCb in _ArtemisHuntCompleteCallback.GetInvocationList())
                {
                    if (registeredCb.Method== handler.Method && registeredCb.Target == handler.Target)
                    {
                        shouldAdd = false;
                        break;
                    }
                }
            }
            if ( shouldAdd)
            {
                _ArtemisHuntCompleteCallback += handler;
            }
            Utils.MyLog("Register Artemis Hunt cb. Current len" + _ArtemisHuntCompleteCallback.GetInvocationList().Length);
        }
        public static void ArtemisCommonUnregisterOnHuntComplete(OnArtemisHuntComplete handler)
        {
            try
            {
                _ArtemisHuntCompleteCallback -= handler;
                Utils.MyLog("UnRegister Artemis Hunt cb. Current len" + _ArtemisHuntCompleteCallback.GetInvocationList().Length);
            }
            catch (Exception e)
            {

            }
        }

        public static void CreateHuntOnPart(DataContext context, int bodyPart, int difficulty)
        {
            DomainManager.Combat.AddSkillEffect(context, DomainManager.Combat.GetCombatCharacter(false),
                new SkillEffectKey((short)(4000 + bodyPart), true),
                (short)difficulty, (short)difficulty, true);
        }

        public static void CreateNewHunt(DataContext context, bool forceCreate = false, int lastHuntBodyPart = -1) 
        {
            int huntDifficulty = DomainManager.Combat.GetCombatCharacter(false).GetCharacter().GetConsummateLevel()*25+300;
            huntDifficulty = (int)(huntDifficulty * _difficultyMultiplier);
            List<int> bodyParts = new List<int> { 0, 1, 2, 3, 4, 5, 6}; // shuffle this, this is our try create hunt order.
            Shuffle(bodyParts, context);

            // newBodyPart = context.Random.NextInt() % 2 + 3; // DEBUG

            var ongoingHunts = GetCurrentHunts(context);
            int needHuntCount;
            if (ongoingHunts == null || ongoingHunts.Count == 0) {
                needHuntCount = _huntCount;
            }
            else
            {
                needHuntCount = _huntCount - ongoingHunts.Count;
            }
            for (int i = 0; i < bodyParts.Count; i++) 
            {
                if (lastHuntBodyPart == bodyParts[i]) { continue; }
                if (needHuntCount == 0) { break; }
                CreateHuntOnPart(context, bodyParts[i], huntDifficulty);
                needHuntCount--;
            }

        }
        public static List<KeyValuePair<SkillEffectKey, short>> GetCurrentHunts(DataContext context, bool isTaiwu=false)
        {
            var combatChar = DomainManager.Combat.GetCombatCharacter(isTaiwu);
            SkillEffectCollection effectCollection = combatChar.GetSkillEffectCollection();
            if (effectCollection.EffectDict == null) { return new List<KeyValuePair<SkillEffectKey, short>>(); };
            List<KeyValuePair<SkillEffectKey, short>> result = new List<KeyValuePair<SkillEffectKey, short>>();
            foreach (var effect in effectCollection.EffectDict)
            {
                if (effect.Key.SkillId >= 4000 && effect.Key.SkillId <= 4006)
                {
                    result.Add(new KeyValuePair<SkillEffectKey, short>(effect.Key, effect.Value));
                }
            }

            return result;
        }
        public static void ArtemisOnAddDirectDamageValue(DataContext context, int attackerId, int defenderId, 
            sbyte bodyPart, bool isInner, int damageValue, short combatSkillId)
        {
            // only implemented for Taiwu.
            if (defenderId == DomainManager.Taiwu.GetTaiwuCharId()) { return; }
            var combatChar = DomainManager.Combat.GetCombatCharacter(false); // always fetch enemy

            var currentHunts = GetCurrentHunts(context);
            if (currentHunts == null || currentHunts.Count == 0)
            {
                return;
            }
            var correctHunt = currentHunts.FirstOrDefault(kvp => kvp.Key.SkillId - 4000 == bodyPart);
            if (correctHunt.Value <= 0) { return; }
            var huntEffectValue = correctHunt.Value;
            // check if the hit was at the correct BodyPart
            Utils.MyLog(String.Format("Got Hunt {0} count={1}, dmg={2}", correctHunt.Key.SkillId, huntEffectValue, damageValue));


            if (damageValue < huntEffectValue) 
            {
                // This event is not raised. Maybe we still need to raise it?
                DomainManager.Combat.ChangeSkillEffectCount(context, combatChar, correctHunt.Key, (short)(-(short)damageValue), false, false);
            }
            else
            {
                // Artemis listeners listen to this event to check if the hunt is complete.
                // Listening to RemoveSkillEffect might not be a good idea due to Xiaoyuyang.
                DomainManager.Combat.ChangeSkillEffectCount(context, combatChar, correctHunt.Key, (short)(-(short)huntEffectValue), true, false);
                _ArtemisHuntCompleteCallback(context, attackerId, defenderId, bodyPart);

                CreateNewHunt(context, false, bodyPart);
            }
        }

        static void Shuffle<T>(List<T> list, DataContext cxt)
        {
            
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = (int)(cxt.Random.NextUInt() % (n + 1));
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}

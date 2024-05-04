using Config;
using GameData.Domains;
using GameData.Domains.Combat;
using GameData.Domains.SpecialEffect.SpellsFromTheWest.Hephaestus;
using GameData.Utilities;
using SpellsFromTheWest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpellsFromTheWestBackend.SkillEffects.Hephaestus.Common
{
    internal class HephaestusCommon
    {
        public static void OnSaveLoaded()
        {
            string savedData;
            if (!DomainManager.Mod.TryGet(SpellsFromTheWestBackendPlugin.GetModIdStr(), "Hephdata", true, out savedData)) return;
            if (savedData.Length < 3) return;
            AdaptableLog.Info("Heph Reforge archive data:");
            AdaptableLog.Info(savedData);
            reforgeStates = new HephReforgeCollection(savedData);
            foreach (var weaponPair in reforgeStates.data)
            {
                var weaponTemplate = weaponPair.Value;
                var result = ReapplyReforgeFor(Config.Weapon.Instance[weaponTemplate.templateId], weaponTemplate, weaponTemplate.reforgeStatus);
                PushIntoConfig(result);

            }

        }
        // avoid polluting other saves.
        public static void OnUnloading()
        {
            foreach (var weapon in reforgeStates.GetKeys())
            {
                WeaponItem cleanTemplate = GetCleanTemplate((short)weapon);
                PushIntoConfig(cleanTemplate);
            }
        }
        public static bool CanReforge(short weaponId)
        {
            var grade = Config.Weapon.Instance[weaponId].Grade;
            var reforgeTimes = GetReforgeTimes(weaponId);
            if (reforgeTimes + grade < 10)
            {
                return true;
            }
            return false;
        }

        public static int GetReforgeTimes(short weaponTemplateId)
        {
            var newTemplate = reforgeStates.Get(weaponTemplateId);
            if (newTemplate == null) return 0;
            int result = 0;
            foreach(var pair in newTemplate.reforgeStatus)
            {
                result += pair.Value;
            }
            return result;
        }
        public static void PurgeReforgeSkill(string reforgeIdentifier)
        {
            foreach (var weaponId in reforgeStates.GetKeys())
            {
                PurgeReforgeSkillSingle((short)weaponId, reforgeIdentifier);
            }
        }
        public static void PurgeReforgeSkillSingle(short templateId, string reforgeIdentifier)
        {
            WeaponItem dirtyTemplate = Config.Weapon.Instance[templateId];
            var weaponReforgeData = reforgeStates.Get(templateId);
            if (weaponReforgeData.reforgeStatus.ContainsKey(reforgeIdentifier))
            {
                var newReforgeStatus = new Dictionary<string, int>(weaponReforgeData.reforgeStatus);
                newReforgeStatus.Remove(reforgeIdentifier);
                var newTemplate = ReapplyReforgeFor(dirtyTemplate, weaponReforgeData, newReforgeStatus);
                PushIntoConfigAndSavefile(newTemplate, weaponReforgeData);
            }
        }

        private static WeaponItem ReapplyReforgeFor(Config.WeaponItem dirtyOrCleanTemplate, HephaestusReforgeState weaponData, Dictionary<string, int> newReforgeState)
        {
            // this takes weaponData, and reforges the weapon such that the reforge state is as described in
            // newReforgeState (old reforge state in weaponData is overwritten)
            // i.e. this edits weapondata.
            // it does not edit the template, the return value is a copy.
            WeaponItem cleanTemplate = GetCleanTemplate(dirtyOrCleanTemplate.TemplateId);

            foreach (var item in newReforgeState)
            {
                var reforger = allReforgeFuncs[item.Key];

                for (int i = 0; i < item.Value; i++)
                {
                    reforger(cleanTemplate);
                }
            }

            weaponData.reforgeStatus = newReforgeState;
            return cleanTemplate;
        }
        private static void PushIntoConfig(Config.WeaponItem template)
        {
            // FIXME what about extra data array?
            var weaponArr = (List<WeaponItem>)typeof(Config.Weapon).GetField("_dataArray", BindingFlags.NonPublic|BindingFlags.Instance).GetValue(Config.Weapon.Instance);
            weaponArr[template.TemplateId] = template;
        }
        private static void PushIntoConfigAndSavefile(Config.WeaponItem template, HephaestusReforgeState weaponReforgeData)
        {
            PushIntoConfig(template);
            // re-serialize
            reforgeStates.Set(weaponReforgeData);
            var saveString = reforgeStates.ToString();
            var context = GameData.Common.DataContextManager.GetCurrentThreadDataContext();
            DomainManager.Mod.SetString(context, SpellsFromTheWestBackendPlugin.GetModIdStr(), "Hephdata", true, saveString);
        }

        public static WeaponItem GetCleanTemplate(short weaponTemplateId)
        {
            var state = reforgeStates.Get(weaponTemplateId);
            if (state == null)
            {
                return Config.Weapon.Instance.GetItem(weaponTemplateId);
            }
            WeaponItem result = Utils.DeepClone(Config.Weapon.Instance.GetItem(weaponTemplateId));
            foreach (var pair in state.fieldBackups)
            {
                var field = typeof(WeaponItem).GetField(pair.Key);
                if (field.FieldType == typeof(short))
                {
                    field.SetValue(result, Convert.ToInt16(pair.Value));
                }
                else if (field.FieldType == typeof(int))
                {
                    field.SetValue(result, Convert.ToInt32(pair.Value));
                }
                else if (field.FieldType == typeof(sbyte))
                {
                    field.SetValue(result, Convert.ToSByte(pair.Value));
                }
                else
                {
                    throw new NotImplementedException("Not implemented for" + field.GetType());
                }
            }
            return result;

        }

        public static bool TryReforgeWeapon(string reforgeIdentifier, short weaponTemplateId)
        {
            if (GetReforgeTimes(weaponTemplateId) >= 10) return false;

            // reforge ok, start.
            var weaponReforgeData = reforgeStates.Get(weaponTemplateId);
            if (weaponReforgeData == null)
            {
                weaponReforgeData = new HephaestusReforgeState();
                weaponReforgeData.fieldBackups["TemplateId"] = weaponTemplateId;
            }

            // first, back up template.
            var affectedDatas = allAffectedDataFuncs[reforgeIdentifier]();
            foreach (var item in affectedDatas)
            {
                if (!weaponReforgeData.fieldBackups.ContainsKey(item.Item1))
                {
                    // do backup
                    weaponReforgeData.fieldBackups[item.Item1] =
                        typeof(WeaponItem).GetField(item.Item1).GetValue(Config.Weapon.Instance.GetItem(weaponTemplateId));
                }
            }
            
            // copy old status and add 1 to our field
            var newReforgeStatus = new Dictionary<string, int>(weaponReforgeData.reforgeStatus);
            // edit the storage
            if (newReforgeStatus.ContainsKey(reforgeIdentifier))
            {
                newReforgeStatus[reforgeIdentifier] += 1;
            }
            else
            {
                newReforgeStatus[reforgeIdentifier] = 1;
            }

            var reforgedTemplate = ReapplyReforgeFor(Config.Weapon.Instance.GetItem(weaponTemplateId), weaponReforgeData, newReforgeStatus);
            PushIntoConfigAndSavefile(reforgedTemplate, weaponReforgeData);

            return true; 
        }

        public static void InitReforgeGongFa()
        {
            allReforgeFuncs = new Dictionary<string, DoTemplateReforgeFunc> ();
            allAffectedDataFuncs = new Dictionary<string, AffecedDataFunc> ();
            foreach (var item in allReforgeGongFas)
            {
                allReforgeFuncs.Add(item.GetIdentifierDirect(), item.DoTemplateReforgeDirect);
                allReforgeFuncs.Add(item.GetIdentifierIndirect(), item.DoTemplateReforgeIndirect);
                allAffectedDataFuncs.Add(item.GetIdentifierDirect(), item.GetInfluencedFields);
            }
            
        }
        delegate bool DoTemplateReforgeFunc(WeaponItem weaponConfig);
        delegate List<Tuple<string, Type>> AffecedDataFunc();

        static HephReforgeCollection reforgeStates = new HephReforgeCollection();
        static List<HephaestusBoilerPlateCuiPo> allReforgeGongFas = new List<HephaestusBoilerPlateCuiPo>
        {
            new HephaestusAddRange(),
            new HephaestusAddPenetrate(),
        };
        static Dictionary<string, DoTemplateReforgeFunc> allReforgeFuncs = null;
        static Dictionary<string, AffecedDataFunc> allAffectedDataFuncs = null;

    }
}

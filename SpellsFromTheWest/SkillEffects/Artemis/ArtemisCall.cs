using GameData.Common;
using GameData.DomainEvents;
using GameData.Domains;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;
using GameData.Domains.SpecialEffect.CombatSkill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpellsFromTheWestBackend.SkillEffects.Artemis;
using GameData.Domains.SpecialEffect.CombatSkill.Common.Assist;
using GameData.Domains.SpecialEffect.CombatSkill.Emeipai.DefenseAndAssist;
using SpellsFromTheWestBackend;

namespace GameData.Domains.SpecialEffect.SpellsFromTheWest.Artemis
{
    internal class ArtemisCall : AssistSkillBase
    {
        public ArtemisCall() { }
        public ArtemisCall(CombatSkillKey skillKey) : base(skillKey, 94109) { }

        public unsafe override void OnEnable(DataContext context)
        {
            base.OnEnable(context);

            this.AffectDatas = new Dictionary<AffectedDataKey, EDataModifyType>();
			this.AffectDatas.Add(new AffectedDataKey(base.CharacterId, 243, base.SkillTemplateId, -1, -1, -1), EDataModifyType.Custom);
            
            ArtemisCommon.ArtemisCommonRegisterOnHuntComplete(this.OnArtemisHuntComplete);
            Events.RegisterHandler_AddDirectDamageValue(new Events.OnAddDirectDamageValue(this.OnAddDirectDmg));
            NewEvents.RegisterHandler_OnDefenseSkillEnding(this.OnDefenseSkillEnding);
            Events.RegisterHandler_CombatStateMachineUpdateEnd(new Events.OnCombatStateMachineUpdateEnd(this.OnStateMachineUpdateEnd));

        }
        private unsafe void OnStateMachineUpdateEnd(DataContext context, CombatCharacter combatChar)
        {
            DomainManager.Combat.SilenceSkill(context, DomainManager.Combat.GetCombatCharacter(true), this.SkillTemplateId, -1, -1);
            Events.UnRegisterHandler_CombatStateMachineUpdateEnd(new Events.OnCombatStateMachineUpdateEnd(this.OnStateMachineUpdateEnd));

        }
        public override unsafe void OnDisable(DataContext context)
        {
            base.OnDisable(context);
            Events.UnRegisterHandler_AddDirectDamageValue(new Events.OnAddDirectDamageValue(this.OnAddDirectDmg));

        }
        private void OnDefenseSkillEnding(DataContext context, CombatCharacter combatChar)
        {
            if (combatChar.GetAffectingDefendSkillId() != this.SkillTemplateId) { return; }
            this._huntCompleted = 0;
            DomainManager.Combat.SilenceSkill(context, combatChar, this.SkillTemplateId, -1, -1);
        }
        private void OnAddDirectDmg(DataContext context, int attackerId, int defenderId,
                sbyte bodyPart, bool isInner, int damageValue, short combatSkillId)
        {
            if (defenderId == DomainManager.Taiwu.GetTaiwuCharId()) { return; }
            var combatChar = DomainManager.Combat.GetCombatCharacter(false); // always fetch enemy
            var casterChar = DomainManager.Combat.GetCombatCharacter(true); // always fetch taiwu
            if (casterChar.GetAffectingDefendSkillId() != this.SkillTemplateId) { return; }
            this.ShowSpecialEffectTips(0);
            if (IsDirect)
            {
                var currentHunts = ArtemisCommon.GetCurrentHunts(context);
                if (currentHunts == null || currentHunts.Count == 0) { return; }
                foreach (var hunt in currentHunts)
                {
                   
                    sbyte huntTgt = (sbyte)(hunt.Key.SkillId - 4000);
                    if (huntTgt == bodyPart) { continue; }
                    DomainManager.Combat.AddInjuryDamageValue(context, DomainManager.Combat.GetCombatCharacter(true), DomainManager.Combat.GetCombatCharacter(false),
                        huntTgt, isInner ? 0 : damageValue, isInner ? damageValue : 0, this.SkillTemplateId);
                }
            }
            else
            {
                if (context.Random.NextInt() % 2 == 0) { return; }
                for (sbyte i = 0; i < 7; i++)
                {
                    if (i == bodyPart) { continue; }
                    DomainManager.Combat.AddInjuryDamageValue(context, DomainManager.Combat.GetCombatCharacter(true), DomainManager.Combat.GetCombatCharacter(false),
                        i, isInner ? 0 : damageValue, isInner ? damageValue : 0, this.SkillTemplateId);
                }
            }



        }
        /*private void CostBreathAndStance(DataContext context, int charId, bool isAlly, int costBreath, int costStance, short skillId)
        {
            if (charId == base.CharacterId && skillId == this.SkillTemplateId && costBreath == 0 && costStance == 0)
            {
                _poweredUp = true;
                this.ShowSpecialEffectTips(0);
            }
            else
            {
                _poweredUp = false;
            }

            if (charId == base.CharacterId && skillId == this.SkillTemplateId)
            {
                _costPercent = 100;
            }

        }

        private void OnPrepareSkillBegin(DataContext context, int charId, bool isAlly, short skillId)
        {
            if (charId == base.CharacterId && skillId == this.SkillTemplateId)
            {
            }
        }
        */
        public void OnArtemisHuntComplete(DataContext context, int attackerId, int defenderId, sbyte bodyPart)
        {
            _huntCompleted += 1;
            if (_huntCompleted >= 3)
            {
                DomainManager.Combat.ClearSkillCd(context, 
                    DomainManager.Combat.GetCombatCharacter(true), // TODO implement for enemy
                    this.SkillTemplateId);
            }
        }



        public override bool GetModifiedValue(AffectedDataKey dataKey, bool dataValue)
        {
            if (dataKey.CharId != base.CharacterId || dataKey.FieldId != AffectedDataHelper.FieldIds.CanCastInDefend) return dataValue;
            var combatChar = DomainManager.Combat.GetCombatCharacter(true);
            if (combatChar.GetAffectingDefendSkillId() != this.SkillTemplateId) return dataValue;
            return true;
        }

        int _huntCompleted = 0;
        
    }
}

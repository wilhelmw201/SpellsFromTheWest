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
using GameData.Domains.Item;
using GameData.Utilities;

namespace GameData.Domains.SpecialEffect.SpellsFromTheWest.Artemis
{
    internal class ArtemisFlourish : CombatSkillEffectBase
    {
        public ArtemisFlourish() { }
        public ArtemisFlourish(CombatSkillKey skillKey) : base(skillKey, 94109, -1) { }

        public unsafe override void OnEnable(DataContext context)
        {
            base.OnEnable(context);

            Events.RegisterHandler_CombatStateMachineUpdateEnd(new Events.OnCombatStateMachineUpdateEnd(this.OnStateMachineUpdateEnd));
            Events.RegisterHandler_CastSkillEnd(new Events.OnCastSkillEnd(this.OnCastSkillEnd));
            Events.RegisterHandler_AttackSkillAttackEnd(new Events.OnAttackSkillAttackEnd(this.OnSkillAttackEnd));
            Events.RegisterHandler_SkillEffectChange(new Events.OnSkillEffectChange(this.OnSkillEffectChange));

        }
        private void OnSkillEffectChange(DataContext context, int charId, SkillEffectKey key, short oldCount, short newCount, bool removed)
        {
            bool flag = removed && this.IsSrcSkillPerformed && charId == base.CharacterId && key.SkillId == base.SkillTemplateId && key.IsDirect == base.IsDirect;
            if (flag)
            {
                base.RemoveSelf(context);
            }
        }
        private unsafe void OnStateMachineUpdateEnd(DataContext context, CombatCharacter combatChar)
        {
            if (!IsDirect) return;
            if (base.CombatChar != combatChar || DomainManager.Combat.Pause || this.EffectCount == 0) return; 

            this._frameCounter++;
            if (this._frameCounter >= 60)
            {
                ReduceEffectCount(1);
                this._frameCounter = 0;
            }
        }
        // Token: 0x06004390 RID: 17296 RVA: 0x00222A10 File Offset: 0x00220C10
        private void OnCastSkillEnd(DataContext context, int charId, bool isAlly, short skillId, sbyte power, bool interrupted)
        {
            bool flag = !this.IsSrcSkillPerformed;
            if (flag)
            {
                bool flag2 = skillId == base.SkillTemplateId && charId == base.CharacterId;
                if (flag2)
                {
                    if (IsDirect)
                    {
                        int effCount = 10 * power / 100; // max power is 100% (SHI CHENG)
                        base.AddEffectCount(effCount);
                    }
                    else
                    {
                        base.AddMaxEffectCount(true);
                    }
                }
            }

        }

        private void OnSkillAttackEnd(DataContext context, CombatCharacter attacker, CombatCharacter defender, short skillId, int index, bool hit)
        {
            base.CombatChar.GetAttackSkillPower();
            /*

            bool flag = attacker != base.CombatChar || skillId != base.SkillTemplateId || index != 3 || !base.CombatCharPowerMatchAffectRequire(0);
            if (!flag)
            {
                List<sbyte> bodyPartRandomPool = ObjectPool<List<sbyte>>.Instance.Get();
                bodyPartRandomPool.Clear();
                for (sbyte part = 0; part < 7; part += 1)
                {
                    bool flag2 = part != base.CombatChar.SkillAttackBodyPart;
                    if (flag2)
                    {
                        bodyPartRandomPool.Add(part);
                    }
                }
                for (int i = 0; i < (int)this.AttackExtraPartCount; i++)
                {
                    int partIndex = context.Random.Next(0, bodyPartRandomPool.Count);
                    DomainManager.Combat.DoSkillHit(attacker, defender, base.SkillTemplateId, bodyPartRandomPool[partIndex], attacker.SkillHitType[attacker.SkillFinalAttackHitIndex]);
                    bodyPartRandomPool.RemoveAt(partIndex);
                }
                ObjectPool<List<sbyte>>.Instance.Return(bodyPartRandomPool);
                base.ShowSpecialEffectTips(0);
            }*/

            if (attacker != base.CombatChar || index != 3 || !hit) { return; }
            if (this.EffectCount <= 0) { return; }
            var currentHunts = ArtemisCommon.GetCurrentHunts(context);
            foreach ( var hunt in currentHunts )
            {
                DomainManager.Combat.DoSkillHit(attacker, defender, base.SkillTemplateId, (sbyte)(hunt.Key.SkillId - 4000), attacker.SkillHitType[attacker.SkillFinalAttackHitIndex]);
            }
            if (!IsDirect)
            {
                ReduceEffectCount();
            }
            base.ShowSpecialEffectTips();
        }


        int _frameCounter = 0;
    }
    
}

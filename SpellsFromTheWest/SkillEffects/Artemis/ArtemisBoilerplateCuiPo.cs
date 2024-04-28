using GameData.Common;
using GameData.DomainEvents;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;
using GameData.Domains.SpecialEffect.CombatSkill;
using GameData.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellsFromTheWestBackend.SkillEffects.Artemis
{

    internal abstract class ArtemisBoilerplateCuiPo : CombatSkillEffectBase
    {
        public ArtemisBoilerplateCuiPo() { }
        public ArtemisBoilerplateCuiPo(CombatSkillKey skillKey, int a, sbyte b) : base(skillKey, a, b) { }

        public unsafe override void OnEnable(DataContext context)
        {
            base.OnEnable(context);
            ArtemisCommon.ArtemisCommonOnCombatStart(context);
            ArtemisCommon.ArtemisCommonRegisterOnHuntComplete(this.OnArtemisHuntComplete);
            Events.RegisterHandler_CastAttackSkillBegin(new Events.OnCastAttackSkillBegin(this.OnCastAttackSkillBegin));
        }
        public unsafe override void OnDisable(DataContext context)
        {
            base.OnDisable(context);
            ArtemisCommon.ArtemisCommonUnregisterOnHuntComplete(this.OnArtemisHuntComplete);
            Events.UnRegisterHandler_CastAttackSkillBegin(new Events.OnCastAttackSkillBegin(this.OnCastAttackSkillBegin));
        }
        private void OnCastAttackSkillBegin(DataContext context, CombatCharacter attacker, CombatCharacter defender, short skillId)
        {
            if (skillId != base.SkillTemplateId || attacker != base.CombatChar) { return; }
            ArtemisCommon.CreateNewHunt(context, false);
        }
        public abstract void OnArtemisHuntComplete(DataContext context, int attackerId, int defenderId, sbyte bodyPart);

    }

    
}

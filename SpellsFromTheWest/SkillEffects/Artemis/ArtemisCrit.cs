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
using SpellsFromTheWestBackend.SkillEffects.Artemis;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using GameData.Domains.SpecialEffect.CombatSkill.Common.Assist;

namespace GameData.Domains.SpecialEffect.SpellsFromTheWest.Artemis

{
    internal class ArtemisCrit : AssistSkillBase
    {
        public ArtemisCrit() { }
        public ArtemisCrit(CombatSkillKey skillKey) : base(skillKey, 94113) { }

        public unsafe override void OnEnable(DataContext context)
        {
            base.OnEnable(context);
            ArtemisCommon.ArtemisCommonRegisterOnNewHunt(OnArtemisNewHunt);
            ArtemisCommon.ArtemisCommonRegisterOnHuntComplete(OnArtemisHuntComplete);
        }

        public unsafe override void OnDisable(DataContext context)
        {
            base.OnDisable(context);
            ArtemisCommon.ArtemisCommonUnregisterOnNewHunt(OnArtemisNewHunt);
            ArtemisCommon.ArtemisCommonUnregisterOnHuntComplete(OnArtemisHuntComplete);
        }
        public void OnArtemisNewHunt(DataContext context, CombatCharacter attacker, CombatCharacter defender, sbyte bodyPart)
        {
            if (IsDirect)
            {
                DomainManager.Combat.AddFlaw(context, defender, 2, this.SkillKey, bodyPart, 1);
                ShowSpecialEffectTips(0);
            }
            else
            {
                ShowSpecialEffectTips(0);
                DomainManager.Combat.AddAcupoint(context, defender, 2, this.SkillKey, bodyPart, 2);
            }
        }
        public void OnArtemisHuntComplete(DataContext context, int attackerId, int defenderId, sbyte bodyPart)
        {
            var defender = DomainManager.Combat.GetCombatCharacter(false);
            if (IsDirect)
            {
                ShowSpecialEffectTips(0);
                DomainManager.Combat.AddAcupoint(context, defender, 2, this.SkillKey, bodyPart, 1);
            }
            else
            {
                ShowSpecialEffectTips(0);
                DomainManager.Combat.AddFlaw(context, defender, 2, this.SkillKey, bodyPart, 2);
            }
        }



    }
}

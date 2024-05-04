using GameData.Common;
using GameData.DomainEvents;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameData.Domains.SpecialEffect.SpellsFromTheWest.Misc
{
    internal class STD : CombatSkill.CombatSkillEffectBase
    {
        public STD()
        {
        }

        public STD(CombatSkillKey skillKey) : base(skillKey, 4114, -1)
        {
        }

        public override void OnEnable(DataContext context)
        {
            Events.RegisterHandler_AttackSkillAttackEnd(new Events.OnAttackSkillAttackEnd(this.OnSkillAttackEnd));
        }
        public override void OnDisable(DataContext context)
        {
            Events.UnRegisterHandler_AttackSkillAttackEnd(new Events.OnAttackSkillAttackEnd(this.OnSkillAttackEnd));
        }
        private void OnSkillAttackEnd(DataContext context, CombatCharacter attacker, CombatCharacter defender, short skillId, int index, bool hit)
        {
            if (skillId != this.SkillTemplateId || index != 3 || attacker != base.CombatChar) { return; }
            int chance = (int)(base.CombatChar.GetAttackSkillPower() * 0.9);
            if (context.Random.Next(100) <= chance) 
            {
                ShowSpecialEffectTips();
                if (defender.GetCharacter().GetFeatureIds().Contains(TemplateHet)) return;
                if (defender.GetCharacter().GetFeatureIds().Contains(TemplateHomo)) return;
                defender.GetCharacter().AddFeature(context, this.IsDirect ? (short)TemplateHet : (short)TemplateHomo);
            }

        }

        public static short TemplateHet = -1;
        public static short TemplateHomo = -1;

    }
}

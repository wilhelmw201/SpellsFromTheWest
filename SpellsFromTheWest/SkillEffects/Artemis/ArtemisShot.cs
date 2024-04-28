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

namespace GameData.Domains.SpecialEffect.SpellsFromTheWest.Artemis
{
    internal class ArtemisShot : ArtemisBoilerplateCuiPo
    {
        public ArtemisShot() { }
        public ArtemisShot(CombatSkillKey skillKey) : base(skillKey, 94103, -1) { }

        public override void OnArtemisHuntComplete(DataContext context, int attackerId, int defenderId, sbyte bodyPart)
        {
            if (defenderId == DomainManager.Taiwu.GetTaiwuCharId()) return; // not implemented
            if (base.IsDirect)
            {
                var enemyChar = DomainManager.Combat.GetCombatCharacter(false);

                base.ChangeStanceValue(context, enemyChar, -(int)(4000*0.5));
            }
            else
            {
                var ourChar = DomainManager.Combat.GetCombatCharacter(true);
                base.ChangeStanceValue(context, ourChar, (int)(4000 * 0.3));
            }
            base.ShowSpecialEffectTips(0);
        }
    }
}

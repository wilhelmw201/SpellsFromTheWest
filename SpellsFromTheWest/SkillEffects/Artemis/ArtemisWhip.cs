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

namespace GameData.Domains.SpecialEffect.SpellsFromTheWest.Artemis

{
    internal class ArtemisWhip : ArtemisBoilerplateCuiPo
    {
        public ArtemisWhip() { }
        public ArtemisWhip(CombatSkillKey skillKey) : base(skillKey, 94110, -1) { }

        public override void OnArtemisHuntComplete(DataContext context, int attackerId, int defenderId, sbyte bodyPart)
        {
            if (defenderId == DomainManager.Taiwu.GetTaiwuCharId()) return; // not implemented
            if (base.IsDirect)
            {
                var enemyChar = DomainManager.Combat.GetCombatCharacter(false);
                base.ChangeMobilityValue(context, enemyChar, -(int)(3000 * .5));
                base.ChangeSkillMobility(context, enemyChar, -(int)(3000 * .5));
            }
            else
            {
                var ourChar = DomainManager.Combat.GetCombatCharacter(true);
                base.ChangeMobilityValue(context, ourChar, (int)(3000*.3));
                base.ChangeSkillMobility(context, ourChar, (int)(3000 * .3));
            }
            base.ShowSpecialEffectTips(0);
        }
    }
}

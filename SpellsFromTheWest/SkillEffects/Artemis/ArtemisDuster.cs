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
    internal class ArtemisDuster : ArtemisBoilerplateCuiPo
    {
        public ArtemisDuster() { }
        public ArtemisDuster(CombatSkillKey skillKey) : base(skillKey, 94110, -1) { }

        public override void OnArtemisHuntComplete(DataContext context, int attackerId, int defenderId, sbyte bodyPart)
        {
            if (defenderId == DomainManager.Taiwu.GetTaiwuCharId()) return; // not implemented
            if (base.IsDirect)
            {
                var tgtchar = DomainManager.Combat.GetCombatCharacter(false).GetCharacter();
                tgtchar.ChangeDisorderOfQi(context, 70);
            }
            else
            {
                var tgtchar = DomainManager.Combat.GetCombatCharacter(true).GetCharacter();
                tgtchar.ChangeDisorderOfQi(context, -70);

            }
            base.ShowSpecialEffectTips(0);
        }
    }
}

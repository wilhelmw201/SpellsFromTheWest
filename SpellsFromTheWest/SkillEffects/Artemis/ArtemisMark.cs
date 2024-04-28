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
    internal class ArtemisMark : AssistSkillBase
    {
        public ArtemisMark() { }
        public ArtemisMark(CombatSkillKey skillKey) : base(skillKey, 94107) { }

        public unsafe override void OnEnable(DataContext context)
        {
            base.OnEnable(context);
            if (IsDirect)
            {
                ArtemisCommon.SetHuntParameter(huntCount: 2, difficultyMultiplier: (float)1.2);
            }
            else
            {
                ArtemisCommon.SetHuntParameter(huntCount: 3, difficultyMultiplier: 2);

            }
        }
     
        public unsafe override void OnDisable(DataContext context) { 
            base.OnDisable(context);
            ArtemisCommon.SetHuntParameter(huntCount: 1, difficultyMultiplier: 1);

        }

    }
}

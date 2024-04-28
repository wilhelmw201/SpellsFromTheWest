using GameData.Common;
using GameData.DomainEvents;
using GameData.Domains;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;
using GameData.Domains.SpecialEffect.CombatSkill;
using GameData.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpellsFromTheWestBackend.SkillEffects.Artemis;

namespace GameData.Domains.SpecialEffect.SpellsFromTheWest.Artemis

{
    internal class ArtemisFinger : ArtemisBoilerplateCuiPo
    {
        public ArtemisFinger() { }
        public ArtemisFinger(CombatSkillKey skillKey) : base(skillKey, 94106, -1) { }

        public override void OnArtemisHuntComplete(DataContext context, int attackerId, int defenderId, sbyte bodyPart)
        {
            if (defenderId == DomainManager.Taiwu.GetTaiwuCharId()) return; // not implemented
            if (base.IsDirect)
            {
                var enemyChar = DomainManager.Combat.GetCombatCharacter(false);
                enemyChar.SetNormalAttackCd(new IntPair((int)120, (int)120), context);

            }
            else
            {
                var ourChar = DomainManager.Combat.GetCombatCharacter(true);
                DomainManager.Combat.ChangeAttackPrepareValue(context, ourChar, (int)(GlobalConfig.Instance.AttackPrepareValueCostUnit));
            }
            base.ShowSpecialEffectTips(0);
        }
    }
}

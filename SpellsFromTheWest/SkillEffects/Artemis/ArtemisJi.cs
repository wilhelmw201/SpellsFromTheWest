using GameData.Common;
using GameData.DomainEvents;
using GameData.Domains;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;
using GameData.Domains.SpecialEffect.CombatSkill;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using SpellsFromTheWestBackend.SkillEffects.Artemis;
using System.Threading.Tasks;
namespace GameData.Domains.SpecialEffect.SpellsFromTheWest.Artemis

{
    internal class ArtemisJi : ArtemisBoilerplateCuiPo
    {
        static List<sbyte> FindMostFrequentNumbers(List<sbyte> numbers)
        {
            var groupedNumbers = numbers.GroupBy(x => x)
                                        .Select(group => new { Number = group.Key, Count = group.Count() });

            int maxCount = groupedNumbers.Max(g => g.Count);

            List<sbyte> mostFrequentNumbers = groupedNumbers.Where(g => g.Count == maxCount)
                                                           .Select(g => g.Number)
                                                           .ToList();

            return mostFrequentNumbers;
        }

        public ArtemisJi() { }
        public ArtemisJi(CombatSkillKey skillKey) : base(skillKey, 94105, -1) { }

        public override void OnArtemisHuntComplete(DataContext context, int attackerId, int defenderId, sbyte bodyPart)
        {
            if (defenderId == DomainManager.Taiwu.GetTaiwuCharId()) return; // not implemented
            if (base.IsDirect)
            {
                var enemyChar = DomainManager.Combat.GetCombatCharacter(false);
                int counter = 0;
                sbyte[] tricksToRemove = new sbyte[3] {-1, -1, -1};
                foreach (sbyte trickType in enemyChar.GetTricks().Tricks.Values)
                {
                    bool flag = DomainManager.Combat.IsTrickUsable(enemyChar, trickType);
                    if (flag)
                    {
                        tricksToRemove[counter] = trickType;
                        counter++;
                        if (counter == 3) { break; }
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    if (tricksToRemove[i] < 0)
                    {
                        break;
                    }
                    DomainManager.Combat.RemoveTrick(context, enemyChar, trickType: tricksToRemove[i], count: 1, removedByAlly: false);

                }
            }
            else
            {
                var ourChar = DomainManager.Combat.GetCombatCharacter(true);
                var currentWeapon = ourChar.GetWeapons()[ourChar.GetUsingWeaponIndex()];
                var weaponTemplate = Config.Weapon.Instance.GetItem(currentWeapon.TemplateId);
                List<sbyte> frequentTricks = FindMostFrequentNumbers(weaponTemplate.Tricks);
                DomainManager.Combat.AddTrick(context, ourChar, frequentTricks[context.Random.NextInt() % frequentTricks.Count]);
                DomainManager.Combat.AddTrick(context, ourChar, frequentTricks[context.Random.NextInt() % frequentTricks.Count]);

            }
            base.ShowSpecialEffectTips(0);
        }
    }
}

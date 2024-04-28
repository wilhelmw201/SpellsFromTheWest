using Config;
using GameData.Domains.SpecialEffect.CombatSkill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameData.Domains.CombatSkill;
using GameData.Common;
using GameData.DomainEvents;
using GameData.Domains.Character;
using GameData.Domains.SpecialEffect;
using GameData.Domains;
using Character = GameData.Domains.Character.Character;
using GameData.Utilities;
using GameData.Domains.SpecialEffect.CombatSkill.Common.Attack;
namespace GameData.Domains.SpecialEffect.SpellsFromTheWest.Misc
{

    public class QianKunYiZhi : PowerUpOnCast
    {
        public QianKunYiZhi() { }
        public QianKunYiZhi(CombatSkillKey skillKey): base(skillKey, 94101)
		{
		}
        public override void OnEnable(DataContext context)
        {
            //Events.RegisterHandler_PrepareSkillBegin(new Events.OnPrepareSkillBegin(this.OnPrepareSkillBegin));
            //Events.RegisterHandler_CastSkillEnd(new Events.OnCastSkillEnd(this.OnCastSkillEnd));
            doWork(context);
            base.OnEnable(context);
        }
        private void doWork(DataContext context)
        {
            var caster = DomainManager.Combat.GetCombatCharacter(base.CombatChar.IsAlly, true).GetCharacter();

            if (caster.GetId() == GameData.Domains.DomainManager.Taiwu.GetTaiwuCharId())
            {
                int price;
                if (base.IsDirect)
                {
                    if (caster.GetResource(6) > 5000)
                    {
                        price = 5000;
                    }
                    else
                    {
                        price = caster.GetResource(6);
                    }
                }
                else
                {
                    price = (int)(caster.GetResource(6) * 0.03);
                }
                caster.ChangeResource(context, 6, -price);

                if (base.IsDirect)
                {
                    this.PowerUpValue = price / 50;
                }
                else
                {
                    this.PowerUpValue = price / 75;
                }
            }
            else
            {
                this.PowerUpValue = 100;
            }          
	
        }
    }
}
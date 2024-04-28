using GameData.Common;
using GameData.DomainEvents;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;
using GameData.Domains.SpecialEffect.CombatSkill;
using GameData.Domains.SpecialEffect;
using GameData.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Config;

namespace SpellsFromTheWestBackend.SkillEffects.Hephaestus
{
    // Token: 0x020007B4 RID: 1972
    public abstract class HephaestusBoilerPlateCuiPo : CombatSkillEffectBase
    {
        public abstract List<string> GetInfluencedFields();
        public abstract string GetIdentifierDirect();
        public abstract string GetIdentifierIndirect();

        public abstract bool DoTemplateReforgeDirect(WeaponItem weaponConfig);
        public abstract bool DoTemplateReforgeIndirect(WeaponItem weaponConfig);

        public HephaestusBoilerPlateCuiPo() { }
        public HephaestusBoilerPlateCuiPo(CombatSkillKey skillKey, int a, sbyte b) : base(skillKey, a, b) { }

        public override void OnEnable(DataContext context)
        {
            Events.RegisterHandler_CastSkillEnd(new Events.OnCastSkillEnd(this.OnCastSkillEnd));
        }

        // Token: 0x060045C6 RID: 17862 RVA: 0x0022D2A3 File Offset: 0x0022B4A3
        public override void OnDisable(DataContext context)
        {
            Events.UnRegisterHandler_CastSkillEnd(new Events.OnCastSkillEnd(this.OnCastSkillEnd));
            HephaestusCommon.PurgeReforgeSkill(IsDirect ? GetIdentifierDirect() : GetIdentifierIndirect());
        }
   
        // Token: 0x060045C8 RID: 17864 RVA: 0x0022D33C File Offset: 0x0022B53C
        private void OnCastSkillEnd(DataContext context, int charId, bool isAlly, short skillId, sbyte power, bool interrupted)
        {
            if (base.CombatChar.GetCharacter().GetId() != DomainManager.Taiwu.GetTaiwuCharId()) { return; }
            CombatCharacter enemyChar = DomainManager.Combat.GetCombatCharacter(false);
            if (DomainManager.Combat.IsCharacterFallen(enemyChar))
            {
                CombatCharacter ourChar = DomainManager.Combat.GetCombatCharacter(true);
                var holdingWeaponId = ourChar.GetWeapons()[ourChar.GetUsingWeaponIndex()].TemplateId;
                bool forgeResult = HephaestusCommon.TryReforgeWeapon(IsDirect ? GetIdentifierDirect() : GetIdentifierIndirect(), holdingWeaponId);
                if (forgeResult)
                {
                    ShowSpecialEffectTips(0);
                }
            }
        }

    }
}

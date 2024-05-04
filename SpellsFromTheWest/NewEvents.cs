using GameData.Common;
using GameData.Domains.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpellsFromTheWestBackend
{
    internal class NewEvents
    {
        public delegate void OnDefenseSkillEnded(DataContext context, CombatCharacter combatChar);

        public static void RegisterHandler_OnDefenseSkillEnding(OnDefenseSkillEnded fcn)
        {
            _onDefenseSkillEndHandlers += fcn;
        }

        public static void UnRegisterHandler_OnDefenseSkillEnding(OnDefenseSkillEnded fcn)
        {
            _onDefenseSkillEndHandlers -= fcn;
        }

        public static void RaiseDefenseSkillEnding(DataContext context, CombatCharacter combatChar)
        {
            if (_onDefenseSkillEndHandlers != null)
            _onDefenseSkillEndHandlers(context, combatChar);
        }

        public static void ClearDefenseSkillEndingHandlers()
        {
            _onDefenseSkillEndHandlers = null;
        }

        private static OnDefenseSkillEnded _onDefenseSkillEndHandlers = null;
    }
}

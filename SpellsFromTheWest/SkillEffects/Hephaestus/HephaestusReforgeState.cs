using Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// format of the serializer:
//
// each weapon item is a string of the following format (as an example)
// TemplateId=1234,MinDistance=20,MaxDistance=50,BasePenetrationFactor=100:A=3,B=1,C=1,d=4
// The stuff at the front is the backup of the original template. the characters at the end are the modifications.
// weapons are seperated with |
//

namespace SpellsFromTheWestBackend.SkillEffects.Hephaestus
{
    internal class HephaestusReforgeState
    {
        public short templateId;
        public Dictionary<string, object> fieldBackups;
        public Dictionary<string, int> reforgeStatus;

        public static HephaestusReforgeState Deserialize(string input)
        {
            throw new NotImplementedException();
            HephaestusReforgeState result = new HephaestusReforgeState();
            var split1 = input.Split(':');
            (string backups, string reforges) = (split1[0], split1[1]);
            var allBackupStats = backups.Split(',');
            var allReforges = reforges.Split(",");

            foreach (var reforge in allReforges)
            {
                var split2 = reforge.Split("=");
                (string key, string val) = (split2[0], split2[1]);
                result.reforgeStatus.Add(key, int.Parse(val));
            }

            foreach (var backup in allBackupStats)
            {
                var split2 = backup.Split("=");
                (string key, string val) = (split2[0], split2[1]);
                result.fieldBackups.Add(key, tryParse(key, val));
            }
        }

        static object tryParse(string key, string val)
        {
            return int.Parse(val);
        }


    }
}

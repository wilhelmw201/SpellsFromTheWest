using Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// format of the serializer:
//
// each weapon item is a string of the following format (as an example)
// TemplateId=1234,MinDistance=20,MaxDistance=50,BasePenetrationFactor=100:A=3,B=1,C=1,d=4
// The stuff at the front is the backup of the original template. the characters at the end are the modifications.
// weapons are seperated with |
//

namespace SpellsFromTheWestBackend.SkillEffects.Hephaestus.Common
{
    internal class HephaestusReforgeState
    {
        public short templateId { get { return (Convert.ToInt16(fieldBackups["TemplateId"])); } }
        public Dictionary<string, object> fieldBackups = new Dictionary<string, object>();
        public Dictionary<string, int> reforgeStatus = new Dictionary<string, int>();

        public static void Test()
        {
            Thread.Sleep(2000);
            Utils.DebugBreak();
            string testStr = "TemplateId=1234,MinDistance=20,MaxDistance=50,BasePenetrationFactor=100:A=3,B=1,C=1,d=4";
            var deser = Deserialize(testStr);
            string result = Serialize(deser);
            if (result != testStr)
            {
                throw new Exception("test failed");
            }

        }

        public HephaestusReforgeState() { }
        public HephaestusReforgeState(string input)
        {
            var split1 = input.Split(':');
            (string backups, string reforges) = (split1[0], split1[1]);
            var allBackupStats = backups.Split(',');
            var allReforges = reforges.Split(",");

            foreach (var reforge in allReforges)
            {
                var split2 = reforge.Split("=");
                (string key, string val) = (split2[0], split2[1]);
                reforgeStatus.Add(key, int.Parse(val));
            }

            foreach (var backup in allBackupStats)
            {
                var split2 = backup.Split("=");
                (string key, string val) = (split2[0], split2[1]);
                fieldBackups.Add(key, tryParse(key, val));
            }
        }

        public override string ToString()
        {
            return Serialize(this);
        }

        public static string Serialize(HephaestusReforgeState state)
        {
            List<string> sb = new List<string>();
            foreach (var field in state.fieldBackups)
            {
                sb.Add(",");
                sb.Add(field.Key);
                sb.Add("=");
                sb.Add(myToStr(field.Key, field.Value));
            }
            sb.Add(":");
            foreach (var field in state.reforgeStatus)
            {
                sb.Add(string.Format("{0}={1}", field.Key, field.Value.ToString()));
                sb.Add(",");
            }
            return string.Join("", sb.GetRange(1, sb.Count - 2));
        }

        public static HephaestusReforgeState Deserialize(string input)
        {
            return new HephaestusReforgeState(input);
        }

        // this is for non string keys, although i do not plan to use any atm...
        static object tryParse(string key, string val)
        {
            return int.Parse(myToStr(key, val));
        }
        static string myToStr(string key, object val)
        {
            return (val).ToString();
        }


    }
}

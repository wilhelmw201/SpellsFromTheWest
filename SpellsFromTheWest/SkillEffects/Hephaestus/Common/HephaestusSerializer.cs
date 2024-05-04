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


namespace SpellsFromTheWestBackend.SkillEffects.Hephaestus.Common
{
    internal class HephaestusSerializer
    {
        static public string SerializeReforgeStates(List<HephaestusReforgeState> states)
        {
            var result = new List<string>();
            foreach (var state in states)
            {
                // do Trim data size: if reforge status is empty dont save. 
                if (state.reforgeStatus.Count == 0) { continue;}
                
                result.Add(HephaestusReforgeState.Serialize(state));
            }
            return string.Join("|", result);
        }
        static public List<HephaestusReforgeState> DeserializeReforgeStates(string savedArg)
        {
            var result = new List<HephaestusReforgeState>();
            foreach (var saved in savedArg.Split('|'))
            {
                result.Add(HephaestusReforgeState.Deserialize(saved));
            }
            return result;
        }

    }
}

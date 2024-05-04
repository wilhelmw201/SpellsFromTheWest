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
    internal class HephReforgeCollection
    {
        public HephReforgeCollection() { }
        public HephReforgeCollection(string input)
        {
            foreach (var item in input.Split("|"))
            {
                var reforgeItem = new HephaestusReforgeState(item);
                data.Add(reforgeItem.templateId, reforgeItem);
            }
        }

        public HephaestusReforgeState Get(short templateId)
        {
            if (data.ContainsKey(templateId))
            {
                return data[templateId];
            }
            return null;
        }

        public IEnumerable<int> GetKeys()
        {
            foreach (var item in data)
            {
                yield return item.Key;
            }
        }

        public void Set (HephaestusReforgeState value)
        {
            data[value.templateId] = value;
        }

        public override string ToString()
        {
            List<string> list = new List<string>(); 
            foreach(var item in data)
            {
                list.Add(item.Value.ToString());
            }
            return string.Join("|", list);
        }
        public Dictionary<short, HephaestusReforgeState> data = new Dictionary<short, HephaestusReforgeState>();
    }
}

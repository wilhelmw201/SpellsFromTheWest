using Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellsFromTheWestBackend
{
    internal class CreateSkillBookObjHelper
    {
        static Dictionary<int, List<int>> createdBookPerGrade = new Dictionary<int, List<int>> ();

        public static void CreatedBook(SkillBookItem item)
        {
            if (!createdBookPerGrade.ContainsKey(item.Grade))
            {
                createdBookPerGrade.Add(item.Grade, new List<int>());
            }
            createdBookPerGrade[item.Grade].Add(item.TemplateId);
        }

        public static int GetRandomBookTemplateId(GameData.Common.DataContext context , int grade)
        {
            if (!createdBookPerGrade.ContainsKey(grade)) { return -1; }
            return createdBookPerGrade[grade][
                    context.Random.NextInt() % createdBookPerGrade[grade].Count
                    ];
        }
    }
}

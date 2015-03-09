using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
   public class Idea: Content
   {
       public string authorship { get; set; }
       public string authorshipPolicy { get; set; }
       public int commentCount { get; set; }
       public int score { get; set; }
       public string stage { get; set; }
       public List<Person> users { get; set; }
       public int voteCount { get; set; }
       public Boolean voted { get; set; }
    }
}

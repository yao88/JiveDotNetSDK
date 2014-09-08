using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
     public class Discussion: Content
     {
         public string answer { get; set; }
         public List<string> helpful { get; set; }
         public OnBehalfOf onBehalfOf { get; set; }
         public Boolean question { get; set; }
         public string resolved { get; set; }
         public Boolean restrictReplies 
    }
}

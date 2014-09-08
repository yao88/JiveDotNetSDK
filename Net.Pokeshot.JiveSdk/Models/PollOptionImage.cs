using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Pokeshot.JiveSdk.Models
{
   public class PollOptionImage
   {
       public int followerCount { get; set; }
       public Image image { get; set; }
       public int likeCount { get; set; }
       public string option { get; set; }
       public DateTime published {get;set;}
       public Object recources {get;set;}
       public List<string> tags { get; set; }
       public DateTime úpdated { get; set; }
        
    }
}

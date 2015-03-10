using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
     public class Attachment
     {
         public string contentType { get; set; }
         public Boolean doUpload { get; set; }
         public string id { get; set; }
         public string name { get; set; }
         public Object recources { get; set; }
         public int size { get; set; }
         public string url { get; set; }
          
    }
}

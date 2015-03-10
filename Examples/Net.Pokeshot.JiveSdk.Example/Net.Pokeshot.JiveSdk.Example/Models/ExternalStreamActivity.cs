using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
   public class ExternalStreamActivity: Content {
       public Object action {get;set;}
       public Object actor {get;set;}
       public string externalID {get;set;}
       public int externalStreamID {get;set;}
       public Object jive {get;set;}
       public Object @object {get;set;}
       public Object properties {get;set;}
      
    
    }
}

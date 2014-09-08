using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class Poll: Content 
        public List<String> options {get;set;}
        public List<PollOptionImage> optionsImages {get;set;}
        public List<Person> users {get;set;}
        public int voteCount {get;set;}
        public List<string> votes {get;set;}
        
    {
    }
}

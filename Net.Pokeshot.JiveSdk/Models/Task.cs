using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class Task: Content
        public Boolean completed {get;set;}
        public DateTime dueDate {get;set;}
        public string owner {get;set;}
        public string parentTask {get;set;}
        public List<string> subTasks {get;set;}
        
    {
    }
}

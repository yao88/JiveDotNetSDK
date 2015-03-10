using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models.Dto
{
    public class JiveDEAActivity
    {
        public JiveDEAPaging paging { get; set; }
        public List<JiveDEAActivityInstance> list { get; set; }
    }
}

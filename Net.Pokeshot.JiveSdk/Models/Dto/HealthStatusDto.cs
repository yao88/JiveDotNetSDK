using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models.Dto
{
    public class HealthStatusDto
    {
        public string status { get; set; }
        public string lastUpdate { get; set; }
        public List<string> messages { get; set; }
        public List<string> resources { get; set; }
    }
}

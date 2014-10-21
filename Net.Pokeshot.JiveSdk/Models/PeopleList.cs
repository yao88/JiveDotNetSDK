using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class PeopleList
    {
        public int itemsPerPage { get; set; }
        public List<Person> list { get; set; }
    }
}

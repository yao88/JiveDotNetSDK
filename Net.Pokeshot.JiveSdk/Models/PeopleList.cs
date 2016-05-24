using System.Collections.Generic;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class PeopleList
    {
        public int itemsPerPage { get; set; }
        public JiveLinkResource links { get; set; }
        public List<Person> list { get; set; }
    }
    
    public class JiveLinkResource
    {
        public string next { get; set; }
    }    
}

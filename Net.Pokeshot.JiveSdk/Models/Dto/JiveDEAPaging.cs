using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Pokeshot.JiveSdk.Models.Dto
{
    public class JiveDEAPaging
    {
        public string next { get; set; }
        public string previous { get; set; }
        public int itemsPerPage { get; set; }
        public int totalCount { get; set; }
        public int currentPage { get; set; }
        public int totalPages { get; set; }
    }
}

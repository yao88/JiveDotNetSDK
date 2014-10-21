using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class ImageList
    {
        public int itemsPerPage { get; set; }
        public List<Image> list { get; set; }
    }
}

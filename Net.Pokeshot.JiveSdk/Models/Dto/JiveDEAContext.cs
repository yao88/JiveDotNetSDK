using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Pokeshot.JiveSdk.Models.Dto
{
    public class JiveDEAContext
    {
        public JiveDEAService service { get; set; }
        public JiveDEAJvm jvm { get; set; }
        public JiveDEAWeb web { get; set; }

    }
}

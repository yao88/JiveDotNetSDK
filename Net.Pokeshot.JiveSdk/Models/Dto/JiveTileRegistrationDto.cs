using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models.Dto
{
    [DataContract]
    public class JiveTileRegistrationDto
    {
        [DataMember]
        public string guid { get; set; }
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public JiveTileRegistrationConfigDto config { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string jiveUrl { get; set; }
        [DataMember]
        public string tenantId { get; set; }
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public string parent { get; set; }
        [DataMember]
        public string placeUri { get; set; }
        [DataMember]
        public string code { get; set; }
    }

  
    [DataContract]
    public class JiveTileRegistrationConfigDto
    {
        [DataMember]
        public string parent { get; set; }
   

    }
}

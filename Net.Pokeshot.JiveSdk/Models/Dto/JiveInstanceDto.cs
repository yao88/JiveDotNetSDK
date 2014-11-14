using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models.Dto
{
    [DataContract]
    public class JiveInstanceDto
    {
        [Required]
        [DataMember(IsRequired = true)]
        public string name { get; set; }
        [Required]
        [DataMember(IsRequired = true)]
        public string jiveUrl { get; set; }

        [DataMember]
        public string version { get; set; }
        [Required]
        [DataMember(IsRequired = true)]
        public string communityLanguage { get; set; }
    }
}

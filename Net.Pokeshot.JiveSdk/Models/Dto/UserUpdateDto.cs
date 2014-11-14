using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models.Dto
{
    [DataContract]
    public class UserUpdateDto
    {
        [DataMember]
        public string displayName { get; set; }
        [DataMember]
        public string firstName { get; set; }
        [DataMember]
        public string lastName { get; set; }
        [DataMember]
        public string emailAddress { get; set; }
        [DataMember]
        public int shortId { get; set; }
    
    }
}

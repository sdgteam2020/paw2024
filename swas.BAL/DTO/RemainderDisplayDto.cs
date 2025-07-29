using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace swas.BAL.DTO
{
    public class RemainderDisplayDto
    {
      

        [JsonPropertyName("projectId")]
        public int projid { get; set; }
        public int Psmid { get; set; }
        public string ProjName { get; set; }
        public string Sponsor { get; set; }
        public string Domain { get; set; }
        public string FromUnit { get; set; }
        public string ToUnit { get; set; }
        public string Remarks { get; set; }
      
      
        public string userDetails { get; set; }
        public string TouserDetails { get; set; }
        public string SentOn { get; set; }
        public string ReadOn { get; set; }
        public string unitName { get; set; }
        public string EncyID { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    [Table("trnRemainder")]
    public class trnRemainder
    {
        [Key]
        public int RemainderId { get; set; }

        
        public int psmid { get; set; }


        public int? FromUnitId { get; set; }
        public int? Tounitid { get; set; }
        public bool IsRemainder { get; set; }
        public string? SendDate { get; set; }
        public string? ReadDate { get; set; }
        public string? UserDetails { get; set; }
        public string? Remarks { get; set; }

    }
}

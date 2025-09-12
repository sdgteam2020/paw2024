using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    public class tbl_ProjStakeHolderCcMov
    {
        [Key]
        public int PsmCcId { get; set; }

        [ForeignKey("tbl_ProjStakeHolderMov")]
        public int PsmId { get; set; }

        [ForeignKey("tbl_Projects")]
        public int ProjId { get; set; }

        public int ToCcUnitId { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }
        public bool IsRead { get; set; }
        public DateTime ReadDate { get; set; }

        public string? UserDetails { get; set; }
    }

}

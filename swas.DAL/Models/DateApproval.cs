using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace swas.DAL.Models
{
    [Table("DateApproval")]
    public class DateApproval
        {
        public int Id { get; set; }

        public int? ProjId { get; set; }
        public int? UnitId { get; set; }

        public DateTime? Request_Date { get; set; }
        public DateTime? DDGIT_Approval_dat { get; set; }

        public bool? UserRequest { get; set; }
        public bool? DDGIT_approval { get; set; }

        public string User { get; set; }
        public bool? IsRead { get; set; }
        public int? RequestType { get; set; }

        [NotMapped]
        public string ProjName { get; set; }

       
        [NotMapped]
        public string UnitName { get; set; }


        //[NotMapped]
        //public string? Sponsor { get; set; }

        [NotMapped]
        public string? Remarks { get; set; }
        
        [NotMapped]
        public string? EncyID { get; set; }



    }

}


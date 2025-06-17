using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    public class LegacyHistory
    {
        [Key]
        public int HistoryId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public int? UnitId { get; set; }

        public int? FromUnit { get; set; }

        [Required]
        [StringLength(50)]
        public string ActionBy { get; set; }

        [Required]
        public ActionTypeEnum ActionType { get; set; }

        [StringLength(255)]
        public string Remarks { get; set; }

        public DateTime ActionDate { get; set; } = DateTime.Now;


        public string Userdetails { get; set; }




        public enum ActionTypeEnum
        {
            RequestSent = 1,
            Approved = 2,
            Rejected = 3,
           UnApprove =4
           
            // Add more as needed
        }

        [NotMapped]
        public string ProjectName { get; set; }

        [NotMapped]
        public string ToUnitName { get; set; }

        [NotMapped]
        public string FromunitName { get; set; }
        [NotMapped]
        public string ActionTypeText { get; set; }


    }
}

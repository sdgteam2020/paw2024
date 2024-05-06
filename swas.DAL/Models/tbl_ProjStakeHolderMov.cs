using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace swas.DAL.Models
{
	///Created and Reviewed by : Sub Maj Sanal
	///Reviewed Date : 31 Jul 23
	///Tested By :- 
	///Tested Date : 
	///Start
	public class tbl_ProjStakeHolderMov
	{
        [Key]
        public int PsmId { get; set; }

        [ForeignKey("tbl_Projects")]
        public int ProjId { get; set; }

        [ForeignKey("tbl_mStakeHolder")]
        public int StakeHolderId { get; set; }

        [ForeignKey("tbl_mStages")]
        public int StageId { get; set; }

        [ForeignKey("tbl_mStatus")]
        public int StatusId { get; set; }

        [ForeignKey("tbl_mActions")]
        public int ActionId { get; set; }

        [StringLength(200)]
        [Column(TypeName = "varchar(200)")]
        [Display(Name = "Remarks")]
        public string AddRemarks { get; set; }

        public int CurrentStakeHolderId { get; set; }

        [Display(Name = "Entry Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? TimeStamp { get; set; }

        public int ToStakeHolderId { get; set; }

        public int FromStakeHolderId { get; set; }

        public int CommentId { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        [Display(Name = "Edit/Delete By")]
        public int? EditDeleteBy { get; set; }
        [Display(Name = "Edit/Delete Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EditDeleteDate { get; set; }

        public int? UpdatedByUserId { get; set; }
        [Display(Name = "Date of Update")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateTimeOfUpdate { get; set; }

        [NotMapped]
        [StringLength(200)]
        [Column(TypeName = "varchar(200)")]
        [Display(Name = "Comments")]
        public string? Comments { get; set; }
        public DateTime? TostackholderDt { get; set; }

        public DateTime? ActionDt { get; set; }
        public int? ActionCde { get; set; }
        public int? grouppsmid { get; set; }

        [NotMapped]
        public string Attpath { get; set; }

        [NotMapped]
        public string? ProjCode { get; set; }
        [NotMapped]
        public string? AttRemarks { get; set; }

        [NotMapped]
        public int? Psmidex { get; set; }


    }


}

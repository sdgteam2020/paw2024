﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace swas.DAL.Models
{
	///Created and Reviewed by : Sub Maj Sanal
	///Reviewed Date : 31 Jul 23
	///Tested By :- 
	///Tested Date : 
	///Start
	public class tbl_mActions
	{
		[Key]
        [Display(Name = "Action")]
        public int ActionsId { get; set; }
		[StringLength(200)]
		[Column(TypeName = "varchar(200)")]
		[Display(Name = "Actions")]
		[Required]
		public string ? Actions { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsActive { get; set; }
		[Display(Name = "Edit/Delete By")]
		public string? EditDeleteBy { get; set; }
		[Display(Name = "Edit/Delete Date")]
		[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
		public DateTime? EditDeleteDate { get; set; }

        public string? ActionDesc { get; set; }
        public string? UpdatedByUserId { get; set; }
		[Display(Name = "Date of Update")]
		[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
		public DateTime? DateTimeOfUpdate { get; set; }
		public bool? InitiaalID { get; set; } = false;
		public bool? FininshID { get; set; } = false;

		public int StagesId { get; set; }

		public int TimeLimit { get; set; }

		public int StatusId { get; set;}
        public int? Actionseq { get; set; }

		public bool? ObsnFlag { get; set; } = false ;

		public int? StatCompId { get; set; }
		

	}


}

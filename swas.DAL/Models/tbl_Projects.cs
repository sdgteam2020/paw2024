using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace swas.DAL.Models
{

    public class tbl_Projects
    {
        // ─── Primary Key ─────────────────────────────────────────────────────────────

        [Key]
        
        public int ProjId { get; set; }

        // ─── Core Identity ────────────────────────────────────────────────────────────

        // Form: data_maxlength="200", required
        [Required(ErrorMessage = "Project Name is required.")]
        [Column(TypeName = "varchar(1201)")]
        [StringLength(1201, ErrorMessage = "Project Name cannot exceed 200 Words.")]
        [Display(Name = "Project Name")]
        public string? ProjName { get; set; }

        // Form: readonly input, populated by sponsor lookup — no regex, just length
        [Column(TypeName = "nvarchar(201)")]
        [StringLength(201, ErrorMessage = "Sponsor name cannot exceed 200 characters.")]
        public string? Sponsor { get; set; }

        // System-assigned from Logins.unitid — no user input, no validation needed
        [ForeignKey("tbl_mUnitBranch")]
        public int StakeHolderId { get; set; }

        // System-managed — no user input
       
        public int CurrentPslmId { get; set; }

        // Not present in form as editable field — optional, light format check only
        [StringLength(51, ErrorMessage = "Project Code cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z0-9\-_/]*$",
            ErrorMessage = "Project Code may only contain letters, digits, hyphens, underscores, and forward slashes.")]
        public string? ProjCode { get; set; }

        // ─── Dates ───────────────────────────────────────────────────────────────────

        // Form: type="date", required
        [Required(ErrorMessage = "Project Start Date is required.")]
        [Display(Name = "Project Start Date")]
        [DataType(DataType.Date)]
        public DateTime? InitiatedDate { get; set; }

        // Form: type="date", required
        [Required(ErrorMessage = "Project Completion Date is required.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Project Completion Date")]
        [DataType(DataType.Date)]
        public DateTime? CompletionDate { get; set; }

        // System-set (DateTime.Now) — no user input
        [Display(Name = "Edit/Delete Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? EditDeleteDate { get; set; }

        // System-set — no user input
        [Display(Name = "Date of Update")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? DateTimeOfUpdate { get; set; }

        // Radio button: value="1" (On) or value="0" (Off)
        public int Date_type { get; set; }

        // ─── Status / Flags ───────────────────────────────────────────────────────────

        // Form: DropDownListFor from ViewBag.WhitelistOptions, required
        // "Re-Vetted" must be included — business logic branches on it
        [Required(ErrorMessage = "Whitelisted status is required.")]
        [Column(TypeName = "varchar(200)")]
        [Display(Name = "Already Whitelisted")]
        [RegularExpression(@"^(Yes|No|Re-Vetted)$",
            ErrorMessage = "Whitelisted must be one of: Yes, No, Re-Vetted.")]
        public string? IsWhitelisted { get; set; }

        // System-set — no user input
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public bool IsProcess { get; set; }
        public bool IsSubmited { get; set; }

        // Form: DropDownListFor from ViewBag.IsAI_ML, required
        [Required(ErrorMessage = "Please indicate whether this is an AI/ML project.")]
        [Display(Name = "AI/ML Project?")]
        public bool? Is_AI_ML { get; set; }

        // ─── Contact Info ─────────────────────────────────────────────────────────────

        // Form: type="number", data_maxlength="10", required
        // Digits only, max 10 digits — matches form data_maxlength="10"
        [Required(ErrorMessage = "Mobile Number is required.")]
        [MaxLength(10)]
        [Display(Name = "Mobile Number (Tele No)")]
        [RegularExpression(@"^\d{1,10}$",
            ErrorMessage = "Mobile Number must be numeric and cannot exceed 10 digits.")]
        public string? MobileNo { get; set; }

        // Form: type="number", data_maxlength="5", required
        [Required(ErrorMessage = "Ascon Number is required.")]
        [Display(Name = "Ascon No")]
        [Range(1, 99999, ErrorMessage = "Ascon Number must be a positive number up to 5 digits.")]
        public int? AsconNo { get; set; }

        // ─── Classification & Type ────────────────────────────────────────────────────

        // Form: DropDownListFor from ViewBag.SecurityClassifications, required
        [Required(ErrorMessage = "Security Classification is required.")]
        [MaxLength(100)]
        [Display(Name = "Security Classification")]
       
        public string? Security_Classification { get; set; }

        // Form: DropDownListFor from ViewBag.TypeofSWOption, required
        [Required(ErrorMessage = "Type of Software is required.")]
        [Display(Name = "Type of Software")]
        public string? TypeofSW { get; set; }

        // Form: DropDownListFor from ViewBag.BeingDevpInhouseOption, required
        [Required(ErrorMessage = "Development Approach is required.")]
        [Display(Name = "Development Approach (In-house / Outsourced)")]
        public string? BeingDevpInhouse { get; set; }

        // Not an editable form field in this view — no required, no regex
        [Display(Name = "Deployment Mode")]
        public string? Deplytype { get; set; }

        // Not an editable form field in this view — no required, no regex
        [Display(Name = "Hosted On (ADN/Internet)")]
        public string? Hostedon { get; set; }

        // Not present in this form view
        [Display(Name = "Endorsement by Head of Dept")]
        public string? EndorsmentbyHeadof { get; set; }

        // ─── Foreign Keys ─────────────────────────────────────────────────────────────

        // Form: ddlApptype select, required
        [Required(ErrorMessage = "Application Type is required.")]
        [Display(Name = "Application Type")]
        [ForeignKey("mAppType")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid Application Type must be selected.")]
        public int? Apptype { get; set; }

        // Form: ddlHostTypeID select, required
        [Required(ErrorMessage = "Host Type is required.")]
        [Display(Name = "Hosted On")]
        [ForeignKey("mHostType")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid Host Type must be selected.")]
        public int? HostTypeID { get; set; }

        // ─── Audit Fields ─────────────────────────────────────────────────────────────

        // System-set — no user input
        [Display(Name = "Edit/Delete By")]
        public int? EditDeleteBy { get; set; }

        // System-set — no user input
        [Display(Name = "Updated by")]
        public int? UpdatedByUserId { get; set; }

        // ─── Financial ────────────────────────────────────────────────────────────────

        // Form: data_maxlength="50", required
        // Allows numeric with optional decimals, up to 50 chars as shown in form
        [Required(ErrorMessage = "Likely Cost is required.")]
        [Display(Name = "Likely Cost (₹)")]
        [StringLength(301, ErrorMessage = "Envisaged Cost cannot exceed 50 words.")]
        //[RegularExpression(@"^\d{1,47}(\.\d{1,2})?$",
        //    ErrorMessage = "Envisaged Cost must be a valid numeric amount (e.g. 150000 or 150000.50).")]
        public string EnvisagedCost { get; set; } = string.Empty;

        // ─── Technical Details ────────────────────────────────────────────────────────

        // Form: TextAreaFor, data_maxlength="500", required
        [Required(ErrorMessage = "Aim is required.")]
        [Display(Name = "Aim")]
        [StringLength(3001, ErrorMessage = "Aim cannot exceed 500 Words.")]
        public string? Aim { get; set; } 
        [Required(ErrorMessage = "Puropose is required.")]
        [Display(Name = "Puropose")]
        [StringLength(601, ErrorMessage = "Puropose cannot exceed 100 Words.")]
        public string? Puropose { get; set; } 
        [Required(ErrorMessage = "Scope is required.")]
        [Display(Name = "Scope")]
        [StringLength(601, ErrorMessage = "Scope cannot exceed 100 Words.")]
        public string? Scope { get; set; }

        // Not present in this form view as required — optional
        [Display(Name = "IT infra required")]
        [StringLength(201, ErrorMessage = "IT Infra details cannot exceed 200 characters.")]
        public string? HQandITinfraReqd { get; set; }

        // Not present in this form view as required — optional
        [Display(Name = "Brief details of content of the proposed SW application")]
        [StringLength(501, ErrorMessage = "Content of SW Application cannot exceed 501 characters.")]
        public string? ContentofSWApp { get; set; }

        // Not present in this form view as required — optional
        [Display(Name = "Detailed justification")]
        [StringLength(1801, ErrorMessage = "Justification cannot exceed 100 Words.")]
        public string? ReqmtJustification { get; set; }

        // Not present in this form view as required — optional
        [Display(Name = "Usability of proposed application by other arms/services/org/est")]
        [StringLength(501, ErrorMessage = "Usability details cannot exceed 501 characters.")]
        public string? UsabilityofProposedAppln { get; set; }

        // Not present in this form view as required — optional
        [Display(Name = "Details of user base")]
        [StringLength(501, ErrorMessage = "User base details cannot exceed 501 Words.")]
        public string? DetlsofUserBase { get; set; }

        // Not present in this form view as required — optional
        [Display(Name = "Brief details of proposed network and bandwidth requirements")]
        [StringLength(601, ErrorMessage = "Network/Bandwidth details cannot exceed 50 Words.")]
        public string? NWBandWidthReqmt { get; set; } 




         [Display(Name = "Additional_Ports")]
        [StringLength(601, ErrorMessage = "Additional_Ports details cannot exceed 501 characters.")]
        public string? Additional_Ports { get; set; } 

         [Display(Name = "Concurrent_users")]
        [StringLength(301, ErrorMessage = "Concurrent_users details cannot exceed 50 Words.")]
        public string? Concurrent_users { get; set; } 
         [Display(Name = "API_Initegration")]
        [StringLength(301, ErrorMessage = "API_Initegrationdetails cannot exceed 50 characters.")]
        public string? API_Initegration { get; set; } 


         [Display(Name = "Mission_Critical_appl_justif")]
        [StringLength(301, ErrorMessage = "Mission_Critical_appl_justif details cannot exceed 50 Words.")]
        public string? Mission_Critical_appl_justif { get; set; } 
        
        
     

        // Not present in this form view as required — optional
        [Display(Name = "Projected date of completion incl. broad timelines")]
        [StringLength(501, ErrorMessage = "Timeline details cannot exceed 501 characters.")]
        public string? MajTimeLines { get; set; }

        // Form: TextAreaFor, data_maxlength="500", required
        [Required(ErrorMessage = "Software Platform & Tech Stack is required.")]
        [Display(Name = "Software Platform & Tech Stack (incl. OS Dependencies)")]
        [StringLength(601, ErrorMessage = "Tech Stack details cannot exceed 50 words.")]
        public string? TechStackProposed { get; set; }

        // Not present in this form view as required — optional
        [Display(Name = "Brief details of proposed data security measures incl. backup")]
        [StringLength(501, ErrorMessage = "Data Security details cannot exceed 501 characters.")]
        public string? DataSecurity_backup { get; set; }

        // Form: data_maxlength="200", required
        [Required(ErrorMessage = "Detailed justification is required.")]
        [Column(TypeName = "varchar(601)")]
        [Display(Name = "Detailed justification")]
        [StringLength(601, ErrorMessage = "Initial Remark cannot exceed 100 Words.")]
        public string? InitialRemark { get; set; }

        // Form: TextAreaFor, data_maxlength="500", required
        [Required(ErrorMessage = "No of Virtaul Machines required at data centre for hosting is required.")]
        [Display(Name = "No of Virtaul Machines required at data centre for hosting")]
        [StringLength(301, ErrorMessage = "OS details cannot exceed 50 Words.")]
        public string? Detlsof_OS { get; set; }

        // Form: TextBoxFor, data_maxlength="50", required
        [Required(ErrorMessage = "Proposed Database Engine is required.")]
        [Display(Name = "Proposed Database Engine")]
        [StringLength(121, ErrorMessage = "DB Engine cannot exceed 20 Words.")]
        public string? ProposedDB_Engine { get; set; }

        // Form: TextAreaFor, data_maxlength="200", required
        // Note: form binds to DetlsofSw_Architecture but label says "Detls of proposed architecture"
        [Required(ErrorMessage = "Details of proposed architecture is required.")]
        [Display(Name = "Details of SW Architecture and COTS SW proposed")]
        [StringLength(201, ErrorMessage = "SW Architecture details cannot exceed 200 characters.")]
        public string? DetlsofSw_Architecture { get; set; }

        // Not present in this form view as required — optional
        [Display(Name = "Details of proposed architecture")]
        [StringLength(501, ErrorMessage = "Proposed Architecture details cannot exceed 500 characters.")]
        public string? DetlsofProposed_Architecture { get; set; }

        // Not present in this form view as required — optional
        [Display(Name = "Brief details of proposed utilisation of PKI and IAM")]
        [StringLength(501, ErrorMessage = "PKI/IAM details cannot exceed 501 characters.")]
        public string? DetlsPki_IAM { get; set; }

        // Not present in this form view as required — optional
        [Display(Name = "Technology dependencies (if any)")]
        [StringLength(501, ErrorMessage = "Technology dependencies cannot exceed 501 characters.")]
        public string? Technology_dependencies { get; set; }

        // Not present in this form view as required — optional
        [Display(Name = "Database requirements")]
        [StringLength(501, ErrorMessage = "Database requirements cannot exceed 501 characters.")]
        public string? Database_reqmts { get; set; }

        // Not present in this form view as required — optional
        [Display(Name = "Enhancement/upgradation incl. patch management and SW update procedure")]
        [StringLength(2000, ErrorMessage = "Enhancement/Upgradation details cannot exceed 2000 characters.")]
        public string? Enhancement_upgradation { get; set; }

        // Not present in this form view as required — optional
        [Display(Name = "Details of licensing (if any)")]
        [StringLength(1000, ErrorMessage = "Licensing details cannot exceed 1000 characters.")]
        public string? Details_licensing { get; set; }

        // Form: TextAreaFor, data_maxlength="200", required
       
       

        // Form: TextAreaFor, data_maxlength="200", required
        [Required(ErrorMessage = "Operation system of hosting environment is required.")]
        [MaxLength(301)]
        [Display(Name = "Operation system of hosting environment")]
        [StringLength(301, ErrorMessage = "OS of hosting environment cannot exceed 50 Words.")]
        public string? operation_system_hosting_env { get; set; }

        // ─── [NotMapped] — view/DTO helpers ──────────────────────────────────────────

        [NotMapped]
        public int? CurrentStakeHolderId { get; set; }

        [NotMapped]
        public string? StakeHolder { get; set; }

        [NotMapped]
        public string? FwdtoUser { get; set; }

        [NotMapped]
        [Display(Name = "Fwd On")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FwdtoDate { get; set; }

        [NotMapped]
        public string? RecdFmUser { get; set; }

        [NotMapped]
        public string? FwdBy { get; set; }

        [NotMapped]
        public string? Status { get; set; }

        [NotMapped]
        public string? Comments { get; set; }

        [NotMapped]
        public string? UserID { get; set; }

        [NotMapped]
        public string? UploadedFile { get; set; }

        [NotMapped]
        public string? RegdUserID { get; set; }

        [NotMapped]
        public int? TotalDays { get; set; }

        [NotMapped]
        public string? AdRemarks { get; set; }

        [NotMapped]
        public string? Stages { get; set; }

        [NotMapped]
        public string? Action { get; set; }

        [NotMapped]
        public DateTime? ActionDt { get; set; }

        [NotMapped]
        public int? ActionCde { get; set; }

        [NotMapped]
        public int? AttCnt { get; set; }

        [NotMapped]
        public string? EncyID { get; set; }

        [NotMapped]
        public string? ActFileName { get; set; }

        [NotMapped]
        public int? ActionId { get; set; }

        [NotMapped]
        public string? EncyPsmID { get; set; }

        [NotMapped]
        public string? BlogComment { get; set; }

        [NotMapped]
        public int? PsmIds { get; set; }

        [NotMapped]
        public int? StageId { get; set; }

        [NotMapped]
        public int? OldPsmid { get; set; }
    }
}

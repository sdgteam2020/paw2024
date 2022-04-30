using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace swas.DAL.Models
{

    public class ChartModel
    {
        [Key]
        public int serno { get; set; }

        [Required(ErrorMessage = "App Type is required")]
        [StringLength(100, ErrorMessage = "App Type cannot exceed 100 characters")]
        public string? AppType { get; set; }

        [Required(ErrorMessage = "Month Start is required")]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Invalid date format (yyyy-MM-dd)")]
        public string? MonthStart { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be positive")]
        public int? AppTypeCount { get; set; }

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? AppDesc { get; set; }

        [Required(ErrorMessage = "Month Name is required")]
        [StringLength(20)]
        public string? MonthName { get; set; }
    }

    public class ChartModelS
    {
        [Key]
        public int serno { get; set; }

        [Required(ErrorMessage = "Unit ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Unit ID")]
        public int? unitid { get; set; }

        [Required]
        [StringLength(20)]
        public string? MonthNames { get; set; }

        [Required]
        [StringLength(30)]
        public string? MonthNameYr { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "TotalIn must be >= 0")]
        public int? TotalIn { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "TotalOut must be >= 0")]
        public int? TotalOut { get; set; }

        [Required(ErrorMessage = "Unit name is required")]
        [StringLength(150)]
        public string? unitname { get; set; }
    }
}

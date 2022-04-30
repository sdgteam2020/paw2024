using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{

    public class AttachmentModel
    {
        [Required(ErrorMessage = "File is required")]

        public IFormFile File { get; set; }

        [Required(ErrorMessage = "Remarks is required")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Remarks must be between 5 and 500 characters")]
        public string Remarks { get; set; }
    }
}

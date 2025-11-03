using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    public class AttachmentModel
    {
        public IFormFile File { get; set; }
        public string Remarks { get; set; }
    }

}

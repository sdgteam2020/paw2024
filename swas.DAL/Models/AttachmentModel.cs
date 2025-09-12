using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace swas.DAL.Models
{


    public class AttachmentModel
    {
        public IFormFile File { get; set; }
        public string Remarks { get; set; }
    }


}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    public class tbl_mCertificateContent
    {
        [Key]
        public int ContentId { get; set; }
      
        public int CertificateId { get; set; }

        public int SubStage { get; set; }

        public string? ContentTitle { get; set; }

        public string? ContentText { get; set; }

        public string ContentType { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public tbl_mCertificate Certificate { get; set; } = null!;
    }
    public enum CertificateContentType
    {
        TableRow = 1,
        Paragraph = 2,
        Header = 3
    }


}

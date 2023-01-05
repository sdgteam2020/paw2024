using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    public class tbl_mCertificate
    {
        [Key]
        public int CertificateId { get; set; }
        public string? CertificateName { get; set; }
        public int? Statusid { get; set; }
        public ICollection<tbl_mCertificateContent> CertificateContents { get; set; }
      = new List<tbl_mCertificateContent>();
    }
}

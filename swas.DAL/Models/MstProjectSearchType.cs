using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    [Table("MstProjectSearchType")]
    public class MstProjectSearchType
    {
        [Key]
        public int ProjectSearchTypeId { get; set; }

        public string ProjectSearchTypeName { get; set; } = string.Empty;

        public string ProjectSearchTypeCode { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    public class TrnUnitStatusMapping
    {
        [Key]
        public int UnitStatusMappingId { get; set; }
        public int UnitId { get; set; }
        public int StatusId { get; set; }
    }
}

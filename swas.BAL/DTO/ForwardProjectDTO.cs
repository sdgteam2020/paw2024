using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.DTO
{
    public class ForwardProjectDTO
    {
        public int ProjId { get; set; }
        public int StatusActionsMappingId { get; set; }
        public string Remarks { get; set; }
        public int ToUnitId { get; set; }
        public int[] CcId { get; set; }
    }
}

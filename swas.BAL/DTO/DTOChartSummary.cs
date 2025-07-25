using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.DTO
{
    public class DTOChartSummary
    {
        public string? Name { get; set; }
        public int Total { get; set; }
    }
    public class DTOChartSummarylist
    {
        public List<DTOChartSummary>? ProjectStatus { get; set; }
        public List<DTOChartSummary>? ApprovedProjects { get; set; }
        public List<DTOChartSummary>? WhitelistedProjects { get; set; }
        public List<DTOChartSummary>? TotalProjects { get; set; }
       
    }
}

using System.ComponentModel.DataAnnotations;

namespace swas.DAL.Models
{
    public class mActionmapping
    {
        [Key]
        public int MappingId { get; set; }
        public int UnitId { get; set; }
        public int? Statusid { get; set; }
        public int? MandatoryActionId { get; set; }
        public int? NextStage { get; set; }
        public int? NextStatus { get; set; }
        public int? DefaultFwdUnitId { get; set; }
        public int? HideStatus { get; set; }
    }
}

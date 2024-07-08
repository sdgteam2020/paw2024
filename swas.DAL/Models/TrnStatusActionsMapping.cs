using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    public class TrnStatusActionsMapping
    {
        [Key]
        public int StatusActionsMappingId { get; set; }
        public int StatusId { get; set; }
        public int ActionsId { get; set; }
    }
}

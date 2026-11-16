using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    public class MAppointment
    {
        [Key]
        public int ApptId { get; set; }

        public string AppointmentName { get; set; }
        public bool IsActive { get; set; }
        public int Updatedby { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}

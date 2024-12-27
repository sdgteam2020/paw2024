using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    public class mWhiteListedHeader
    {
        [Key]
        public int Id { get; set; }
        public string Header { get; set; }
    }
}

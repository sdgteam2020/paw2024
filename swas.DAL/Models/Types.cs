using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace swas.DAL.Models
{
    public class Types
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("Name", TypeName = "varchar(200)")]
        public string Name { get; set; }

        public int IsActive { get; set; }

    }
    [NotMapped]
    [AllowAnonymous]
    public class AddlTask
    {
        [NotMapped]
        [Column("Id")]
        public string Id { get; set; }

        [NotMapped]
        [Column("Name", TypeName = "varchar(200)")]
        public string Name { get; set; }
        [NotMapped]
        public int IsActive { get; set; }
        [NotMapped]
        [Column("Unit", TypeName = "varchar(250)")]
        public string Unitname { get; set; }

    }
}

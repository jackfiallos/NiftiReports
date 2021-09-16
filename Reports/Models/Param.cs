using System;
using System.ComponentModel.DataAnnotations;

namespace Reports.Models
{
    public class Param
    {
        [Key]
        [Required]
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string type { get; set; }

        public string value { get; set; }

        public string defaults { get; set; }

        public string itemId { get; set; }
    }
}

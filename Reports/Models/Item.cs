using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Reports.Models
{
    public class Item
    {
        [Key]
        [Required]
        public string id { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        public string description { get; set; }

        public string query { get; set; }

        public string parentId { get; set; }

        [NotMapped]
        public int children { get; set; }
    }
}

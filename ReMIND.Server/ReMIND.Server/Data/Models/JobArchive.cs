using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ReMIND.Server.Data
{
    public class JobArchive
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public DateTime Deadline { get; set; } 

        [Required]
        public DateTime LastModified { get; set; }

        public DateTime? Finished { get; set; } 

        [Required]
        public string Name { get; set; }

        [Required]
        public string Contact { get; set; }

        [Required]
        [Range(0,8)]
        public int JobWeight { get; set; }

        public string JobGroupName { get; set; }

        public string JobTagName { get; set; }
        
        [ForeignKey("PersonID")]
        public int PersonID { get; set; }
        [JsonIgnore]
        public Person Person { get; set; }

        public int tdID { get; set; }
        public string TeamName { get; set; }        
    }
}
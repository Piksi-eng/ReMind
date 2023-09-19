using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReMIND.Server.Data
{
    public class Job
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public DateTime Deadline { get; set; } 

        [Required]
        public DateTime LastModified { get; set; }

        [Required]
        public string Name { get; set; }        

        [Required]
        public string Description { get; set; }

        [Required]
        public RecurringType RecurringType { get; set; }

        [Required]
        public int Multiplier { get; set; }

        public string Contact { get; set; }

        [Required]
        [Range(0,8)]
        public int JobWeight { get; set; }

        [Required]
        public bool isDone { get; set; }

        [Required]
        public bool isRead { get; set; }

        [ForeignKey("JobTagID")]
        public int? JobTagID { get; set; }
        [JsonIgnore]
        public JobTag JobTag { get; set; }

        [ForeignKey("PersonID")]
        public int PersonID { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Person Person { get; set; }

        [ForeignKey("TeamID")]
        public int TeamID { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Team Team { get; set; }

        [ForeignKey("JobGroupID")]
        public int? JobGroupID { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobGroup JobGroup { get; set; }
    }
}
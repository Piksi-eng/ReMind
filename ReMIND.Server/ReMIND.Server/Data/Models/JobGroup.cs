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
    public class JobGroup
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public int CreatorID { get; set; }

        [Required]
        public string Description { get; set; }

        public bool isRead { get; set; }

        public bool didAdminCreate { get; set; }

        public int Counter { get; set; }

        public int TeamID { get; set; }
        [JsonIgnore]
        public Team Team { get; set; }

        [JsonIgnore]
        public List<Job> Job { get; set; }

    }
}
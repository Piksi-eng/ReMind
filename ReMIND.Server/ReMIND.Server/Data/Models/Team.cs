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
    public class Team
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [JsonIgnore]
        public List<TeamLink> TeamLink { get; set; }

        [JsonIgnore]
        public List<JobGroup> JobGroup { get; set; }

        [JsonIgnore]
        public List<Job> Job { get; set; }
    }
}
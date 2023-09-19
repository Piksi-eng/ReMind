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
    public class Person
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]*$")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [RegularExpression(@"^[0-9]*$")]
        public string Phone { get; set; }

        [Required]
        public string Password { get; set; }

        public string SessionID { get; set; }

        public DateTime LastActive{ get; set; }

        public bool IsEmployed { get; set; }

        [JsonIgnore]
        public List<TeamLink> TeamLink { get; set; }

        [JsonIgnore]
        public List<Job> Job { get; set; }

        [JsonIgnore]
        public List<JobArchive> JobArchive { get; set; }
    }
}
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
    public class TeamLink
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public TitleType Title { get; set; }

        [ForeignKey("PersonID")]
        public int PersonID { get; set; }
        [JsonIgnore]
        public Person Person { get; set; }

        [ForeignKey("TeamID")]
        public int TeamID { get; set; }
        [JsonIgnore]
        public Team Team { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RepoLayer
{
    public class Label : ILabel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Label name is required")]
        [Column("LabelName",TypeName ="VARCHAR(50)")]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<NoteLabel>? NoteLabels { get; set; }
    }
}
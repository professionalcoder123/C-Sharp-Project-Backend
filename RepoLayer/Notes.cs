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
    public class Notes : INotes
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Note title is required")]
        [Column("NoteTitle", TypeName ="VARCHAR(50)")]
        public string Title { get; set; }

        [Column("NoteDescription",TypeName ="VARCHAR(500)")]
        public string Description { get; set; }

        [Column("NoteColor",TypeName ="VARCHAR(50)")]
        public string Color { get; set; }

        [Column("IsNoteArchived")]
        public bool IsArchive { get; set; }

        [Column("IsNoteInTrash")]
        public bool IsTrash { get; set; }

        [Required(ErrorMessage ="User id is required")]
        [ForeignKey("UserId")]
        public int UserId { get; set; }

        [JsonIgnore]
        public virtual UserRegistration User { get; set; }

        [JsonIgnore]
        public virtual ICollection<NoteLabel> NoteLabels { get; set; } = new List<NoteLabel>();
    }
}
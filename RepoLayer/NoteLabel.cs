using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer
{
    public class NoteLabel : INoteLabel
    {
        [ForeignKey("Note")]
        public int NoteId { get; set; }

        [ForeignKey("Label")]
        public int LabelId { get; set; }

        public virtual Notes Note { get; set; }

        public virtual Label Label { get; set; }
    }
}
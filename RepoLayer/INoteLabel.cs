using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer
{
    public interface INoteLabel
    {
        public int NoteId { get; set; }

        public int LabelId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer
{
    public interface INotes
    {
        int Id { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string Color { get; set; }
        bool IsArchive { get; set; }
        bool IsTrash { get; set;}
        int UserId { get; set; }
    }
}
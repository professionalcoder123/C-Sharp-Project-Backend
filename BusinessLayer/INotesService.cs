using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer;
using RepoLayer;

namespace BusinessLayer
{
    public interface INotesService
    {
        public List<Notes> GetAllNotes();
        public NoteModel GetNoteById(int id);
        public void AddNote(NoteModel note, int id);
        public void UpdateNote(int userId, int noteId, NoteModel note);
        public void UpdateLabelsForNote(int noteId, int userId, List<int> labelIds);
        public void DeleteNote(int id);
    }
}
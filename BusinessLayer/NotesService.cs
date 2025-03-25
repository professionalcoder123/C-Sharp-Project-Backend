using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModelLayer;
using RepoLayer;

namespace BusinessLayer
{
    public class NotesService : INotesService
    {
        private readonly UserDBContext context;

        public NotesService(UserDBContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddNote(NoteModel note, int id)
        {
            if (note == null)
                throw new ArgumentNullException(nameof(note), "Note data cannot be null!");

            Notes newNote = new Notes
            {

                Title = note.Title,
                Description = note.Description,
                //Color = "#FFFFFF",
                //IsArchive = false,
                //IsTrash = false,
                UserId= id
            };

            context.Notes.Add(newNote);
            context.SaveChanges();
        }

        public void DeleteNote(int id)
        {
            var note = context.Notes.Find(id);
            if (note == null)
            {
                throw new KeyNotFoundException("Note not found!");
            }
            context.Notes.Remove(note);
            context.SaveChanges();
        }

        public List<Notes> GetAllNotes()
        {
            return context.Notes
            .Include(n => n.NoteLabels) // Ensure labels are included
            .Select(n => new Notes
            {
                Id = n.Id,
                Title = n.Title,
                Description = n.Description,
                Color = n.Color,
                IsArchive = n.IsArchive,
                IsTrash = n.IsTrash,
                UserId = n.UserId, // Foreign key
                NoteLabels = n.NoteLabels // Keep full label objects
            })
            .ToList();
        }

        public NoteModel GetNoteById(int id)
        {
            try
            {
                var note = context.Notes
                    .Include(n => n.NoteLabels)
                    .FirstOrDefault(n => n.Id == id);

                if (note == null)
                    throw new Exception("Note not found.");

                return new NoteModel
                {
                    //Id = note.Id,
                    Title = note.Title,
                    Description = note.Description,
                    //Color = note.Color,
                    //IsArchive = note.IsArchive,
                    //IsTrash = note.IsTrash,
                    //UserId = note.UserId,
                    //LabelIds = note.NoteLabels.Select(l => l.LabelId).ToList()
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching the note.", ex);
            }
        }

        public void UpdateNote(int userId, int noteId, NoteModel updatedNote)
        {
            var existingNote = context.Notes.FirstOrDefault(n => n.Id == noteId && n.UserId == userId);

            if (existingNote == null)
            {
                throw new KeyNotFoundException("Note not found or you don't have permission to update it.");
            }

            existingNote.Title = updatedNote.Title;
            existingNote.Description = updatedNote.Description;
            //existingNote.Color = updatedNote.Color;
            //existingNote.IsArchive = updatedNote.IsArchive;
            //existingNote.IsTrash = updatedNote.IsTrash;

            context.SaveChanges();
        }

        public void UpdateLabelsForNote(int noteId, int userId, List<int> labelIds)
        {
            var existingNote = context.Notes.Include(n => n.NoteLabels)
                                            .FirstOrDefault(n => n.Id == noteId && n.UserId == userId);

            if (existingNote == null)
            {
                throw new KeyNotFoundException("Note not found or you don't have permission to modify labels.");
            }

            if (existingNote.NoteLabels != null && existingNote.NoteLabels.Any())
            {
                context.NoteLabels.RemoveRange(existingNote.NoteLabels);
            }

            if (labelIds != null && labelIds.Any())
            {
                foreach (var labelId in labelIds)
                {
                    var label = context.Labels.Find(labelId);
                    if (label != null)
                    {
                        context.NoteLabels.Add(new NoteLabel { NoteId = noteId, LabelId = labelId });
                    }
                }
            }

            context.SaveChanges();
        }
    }
}
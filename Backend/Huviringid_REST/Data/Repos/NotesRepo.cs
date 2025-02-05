using Huviringid_REST.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace Huviringid_REST.Data.Repos
{
    public class NotesRepo (DataContext context) {
        private readonly DataContext context = context;

        public async Task<Note> SaveNoteToDb(Note note) {
            context.Add(note);
            await context.SaveChangesAsync();
            return note;
        }

        public async Task<List<Note>> GetAllNotes() {
            IQueryable<Note> query = context.Notes;
            return await query.ToListAsync();
        }
        
        public async Task<Note?> GetNoteById(int id)
        {
            return await context.Notes.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<bool> NoteExistsInDb(int id) => await context.Notes.AnyAsync(x => x.Id == id);

        public async Task<bool> DeleteNoteById(int id){
            Note? noteInDb = await GetNoteById(id);
            if(noteInDb is null) {
                return false;
            }
            context.Remove(noteInDb);
            int changesCount = await context.SaveChangesAsync();

            return changesCount == 1;
        }

    }
}
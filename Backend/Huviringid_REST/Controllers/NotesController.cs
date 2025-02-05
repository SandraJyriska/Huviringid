using Huviringid_REST.Data.Repos;
using Huviringid_REST.Models.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Huviringid_REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController (NotesRepo repo) : ControllerBase
    {
        private readonly NotesRepo repo = repo;

        /// <summary>Leiab kõik märked</summary>
        /// <returns>Märked</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            var result = await repo.GetAllNotes();
            return Ok(result);
        }

        /// <summary>Leiab märke id järgi</summary>
        /// <param name="id">Märke id</param>
        /// <returns>Märge</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNote(int id) {
            var note = await repo.GetNoteById(id);
            if (note == null) {
                return NotFound();
            }
            return Ok(note);
        }

        /// <summary>Loob uue märke</summary>
        /// <param name="note">Märge</param>
        /// <returns>Loodud märge</returns>
        [HttpPost]
        public async Task<IActionResult> SaveNote([FromBody] Note note) {
            var noteExists = await repo.NoteExistsInDb(note.Id);
            if (noteExists) {
                return Conflict();
            }
            var result = await repo.SaveNoteToDb(note);
            return CreatedAtAction(nameof(SaveNote), new {note.Id}, result);
        }
        
        /// <summary>Kustutab märke id järgi</summary>
        /// <param name="id">Märke id</param>
        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var noteExists = await repo.NoteExistsInDb(id);
            if (!noteExists)
            {
                return NotFound();
            }

            var result = await repo.DeleteNoteById(id);
            if (result)
            {
                return NoContent();
            }
            return BadRequest("Kustutamine ebaõnnestus.");
        }
    }
}
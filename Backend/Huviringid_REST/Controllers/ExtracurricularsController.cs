
using Huviringid_REST.Data.Repos;
using Huviringid_REST.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace Huviringid_REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExtracurricularsController(ExtracurricularsRepo repo) : ControllerBase
    {
        private readonly ExtracurricularsRepo repo = repo; 

        /// <summary>Leiab kõik huviringid</summary>
        /// <returns>Huviringide nimekiri</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            var result = await repo.GetAllExtracurriculars();
            return Ok(result);
        }

        /// <summary>Leiab kõik huviringid, kuhu õpilane on vastu võetud</summary>
        /// <param name="studentId">Õpilase ID</param>
        /// <returns>Huviringide nimekiri</returns>
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetExtracurricularsByStudent(int studentId)
        {
            var extracurriculars = await repo.GetExtracurricularsByStudentId(studentId);
            return Ok(extracurriculars);
        }

        /// <summary>Leiab huviringid, kuhu õpilane pole veel registreerinud</summary>
        /// <param name="studentId">Õpilase ID</param>
        /// <returns>Nimekiri huviringidest</returns>
        [HttpGet("Student/{studentId}/Available")]
        public async Task<IActionResult> GetAvailableExtracurricularsForStudent(int studentId)
        {
            var extracurriculars = await repo.GetAvailableExtracurricularsForStudent(studentId);
            return Ok(extracurriculars);
        }
        
        /// <summary>Leiab huviringi ID järgi</summary>
        /// <param name="id">Huviringi ID</param>
        /// <returns>Huviring</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExtracurricular(int id) {
            var extracurricular = await repo.GetExtracurricularById(id);
            if (extracurricular == null) {
                return NotFound();
            }
            return Ok(extracurricular);
        }

        /// <summary>Leiab konkreetse huviringi toimumise kuupäevad</summary>
        /// <param name="extracurricularId">Huviringi id</param>
        /// <returns>Kuupäevade nimekiri</returns>
        [HttpGet("{extracurricularId}/lesson-dates")]
        public async Task<IActionResult> GetExtracurricularDates(int extracurricularId)
        {
            var dates =  await repo.GetExtracurricularDates(extracurricularId);
            return Ok(dates); 
        }

        /// <summary>Leiab õpetajale kuuluvad huviringid</summary>
        /// <param name="teacherId">Õpetaja id</param>
        /// <returns>Huviringide nimekiri</returns>
        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetExtracurricularsByTeacher(int teacherId)
        {
            var extracurriculars = await repo.GetExtracurricularsByTeacherId(teacherId);
            return Ok(extracurriculars);
        } 

        /// <summary>Loob uue huviringi</summary>
        /// <param name="extracurricular">Huviring</param>
        /// <returns>Loodud huviring</returns>
        [HttpPost]
        public async Task<IActionResult> SaveExtracurricular([FromBody] Extracurricular extracurricular) {
            var extracurricularExists = await repo.ExtracurricularExistsInDb(extracurricular.Id);
            if (extracurricularExists) {
                return Conflict();
            }
            var result = await repo.SaveExtracurricularToDb(extracurricular);
            return CreatedAtAction(nameof(SaveExtracurricular), new {extracurricular.Id}, result);
        }

        /// <summary>Kontrollib, kas õpetajal juba on selle nimega huviring</summary>
        /// <param name="teacherId">Õpetaja id</param>
        /// /// <param name="name">Huviringi nimi</param>
        /// <returns>True või false</returns>
        [HttpGet("teachers/{teacherId}/{name}")]
        public async Task<bool> ExtracurricularExists(string name, int teacherId)
        {
            var extracurriculars = await repo.GetExtracurricularsByTeacherId(teacherId);
            var extracurricularExists = extracurriculars.Any(x => x.Name == name);
            return extracurricularExists;
        } 

    }
}
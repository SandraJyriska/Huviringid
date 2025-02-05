
using Huviringid_REST.Data.Repos;
using Huviringid_REST.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace Huviringid_REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeachersController(TeachersRepo repo): ControllerBase
    {
        private readonly TeachersRepo repo = repo;

        /// <summary>Leiab kõik õpetajad</summary>
        /// <returns>Õpetajate nimekiri</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            var result = await repo.GetAllTeachers();
            return Ok(result);
        }

        /// <summary>Loob uue õpetaja</summary>
        /// <param name="teacher">Õpetaja</param>
        /// <returns>Loodud õpetaja</returns>
        [HttpPost]
        public async Task<IActionResult> SaveTeacher([FromBody] Teacher teacher) {
            var teacherExists = await repo.TeacherExistsInDb(teacher.Id);
            if (teacherExists) {
                return Conflict();
            }
            var result = await repo.SaveTeacherToDb(teacher);
            return CreatedAtAction(nameof(SaveTeacher), new {teacher.Id}, result);
        }

        /// <summary>Uuendab konkreetse õpetaja andmeid</summary>
        /// <param name="id">Õpetaja id</param>
        /// <param name="updatedTeacher">Õpetaja objekt</param>
        /// <returns>204, kui kõik läks edukalt.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(int id, [FromBody] Teacher updatedTeacher)
        {
            if (id != updatedTeacher.Id)
            {
                return BadRequest("Student ID mismatch.");
            }

            bool updateResult = await repo.UpdateTeacherAsync(id, updatedTeacher);

            if (!updateResult)
            {
                return NotFound();
            }

            return NoContent(); // Tagastab 204 No Content, kui kõik läks hästi
        }

        /// <summary>Leiab kõik huviringi õpetajad</summary>
        /// <param name="extracurricularId">Huviringi ID</param>
        /// <returns>Õpetajate nimekiri</returns>
        [HttpGet("{extracurricularId}/Teachers")]
        public async Task<ActionResult<List<Teacher>>> GetTeachersForExtracurricular(int extracurricularId)
        {
            var teachers = await repo.GetTeachersForExtracurricular(extracurricularId);

            if (teachers == null || teachers.Count == 0)
            {
                return NotFound("No teachers found for this extracurricular activity.");
            }

            return Ok(teachers);
        }

        /// <summary>Leiab õpetaja tema id järgi</summary>
        /// <param name="id">Õpetaja id</param>
        /// <returns>Õpetaja</returns>
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetTeacherById(int id) 
        {
            var result = await repo.GetTeacherById(id);
            return Ok(result);
        }

        /// <summary>Leiab konkreetse õpetaja sisse logitud kasutaja id järgi</summary>
        /// <param name="userId">Kasutaja id</param>
        /// <returns>Õpetaja</returns>
        [HttpGet("Users/{userId}")]
        public async Task<IActionResult> GetTeacherByUserId(int userId) 
        {
            var result = await repo.GetTeacherByUserId(userId);
            return Ok(result);
        }
    }
}
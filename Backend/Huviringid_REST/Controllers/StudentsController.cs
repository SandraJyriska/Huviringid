using Huviringid_REST.Data.Repos;
using Huviringid_REST.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace Huviringid_REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController(StudentsRepo repo) : ControllerBase
    {
        private readonly StudentsRepo repo = repo;

        /// <summary>Leiab kõik õpilased</summary>
        /// <returns>Õpilaste nimekiri</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            var result = await repo.GetAllStudents();
            return Ok(result);
        }

        /// <summary>Leiab konkreetse õpilase sisse logitud kasutaja id järgi</summary>
        /// <param name="userId">Kasutaja id</param>
        /// <returns>Õpilane</returns>
        [HttpGet("Users/{userId}")]
        public async Task<IActionResult> GetStudentByUserId(int userId) 
        {
            var result = await repo.GetStudentByUserId(userId);
            return Ok(result);
        }

        /// <summary>Loob uue õpilase</summary>
        /// <param name="student">Õpilane</param>
        /// <returns>Loodud õpilane</returns>
        [HttpPost]
        public async Task<IActionResult> SaveStudent([FromBody] Student student) {
            var studentExists = await repo.StudentExistsInDb(student.Id);
            if (studentExists) {
                return Conflict();
            }
            var result = await repo.SaveStudentToDb(student);
            return CreatedAtAction(nameof(SaveStudent), new {student.Id}, result);
        }

        /// <summary>Uuendab konkreetse õpilase andmeid</summary>
        /// <param name="id">Õpilase id</param>
        /// <param name="updatedStudent">Õpilase objekt</param>
        /// <returns>204, kui kõik läks edukalt.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] Student updatedStudent)
        {
            if (id != updatedStudent.Id)
            {
                return BadRequest("Student ID mismatch.");
            }

            bool updateResult = await repo.UpdateStudentAsync(id, updatedStudent);

            if (!updateResult)
            {
                return NotFound();
            }

            return NoContent(); // Tagastab 204 No Content, kui kõik läks hästi
        }

        /// <summary>Leiab õpilased, kelle antud huviringi registreerimise staatus on "pending"</summary>
        /// <param name="extracurricularId">Huviringi id</param>
        /// <returns>Õpilaste nimekiri</returns>
        [HttpGet("Extracurricular/{extracurricularId}/PendingStudents")]
        public async Task<IActionResult> GetPendingStudents(int extracurricularId){
            var filteredStudents = await repo.FilterPendingStudents(extracurricularId);
            return Ok(filteredStudents);
        }

        /// <summary>Leiab kõik õpilased, kes antud huviringi on vastu võetud</summary>
        /// <param name="extracurricularId">Huviringi id</param>
        /// <returns>Õpilaste nimekiri</returns>
        [HttpGet("Extracurricular/{extracurricularId}/ApprovedStudents")]
        public async Task<ActionResult> GetApprovedStudentsForExtracurricular(int extracurricularId){
            var filteredStudents = await repo.FilterApprovedStudentsForExtracurricular(extracurricularId);
            return Ok(filteredStudents);
        }

        /// <summary>Leiab konkreetse õpilase puudumiste arvu konkreetsest huviringist</summary>
        /// <param name="studentId">Õpilase id</param>
        /// /// <param name="extracurricularId">Huviringi id</param>
        /// <returns>Puudumiste arv</returns>
        [HttpGet("{studentId}/Extracurricular/{extracurricularId}/absences")]
        public async Task<IActionResult> GetAbsencesForStudentInExtracurricular(int studentId, int extracurricularId)
        {
            var absenceCount = await repo.GetAbsenceCountForStudentAsync(studentId, extracurricularId);
            return Ok(absenceCount);
        }

        /// <summary>Kontrollib, kas antud isikukoodiga õpilane juba eksisteerib</summary>
        /// <param name="personalId">Isikukood</param>
        /// <returns>True või false</returns>
        [HttpGet("PersonalId-Check/{personalId}")]
        public async Task<bool> PersonalIdExists(string personalId) 
        {
            var result = await repo.GetAllStudents();
            var personalIdExists = result.Any(x => x.PersonalId == personalId);
            return personalIdExists;
        }

    }
}

using Huviringid_REST.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace Huviringid_REST.Data.Repos
{
    public class StudentsRepo(DataContext context)
    {
        private readonly DataContext context = context;

        public async Task<Student> SaveStudentToDb(Student student) {

            var existingStudent = await context.Students
                .FirstOrDefaultAsync(s => s.PersonalId == student.PersonalId);

            if (existingStudent != null)
            {
                return existingStudent;
            }

            context.Add(student);
            await context.SaveChangesAsync();
            return student;
        }

        public async Task<Student?> GetStudentByUserId(int userId) {

            var student = await context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            return student != null ? student : null;
        }

        public async Task<List<Student>> GetAllStudents() {
            IQueryable<Student> query = context.Students.AsQueryable();
            return await query.ToListAsync();
        }

/*         public async Task<bool> UpdateStudent(int id, Student student) {
            bool isIdsMatch = id == student.Id;
            bool studentExists = await StudentExistsInDb(id);

            if(!isIdsMatch || !studentExists) {
                return false;
            }

            context.Update(student);
            int updatedRecordsCount = await context.SaveChangesAsync();
            return updatedRecordsCount == 1;
        } */

        public async Task<bool> UpdateStudentAsync(int id, Student updatedStudent)
        {
            var existingStudent = await context.Students.FindAsync(id);
            
            bool isIdsMatch = id == existingStudent?.Id;
            bool studentExists = await StudentExistsInDb(id);

            if (!isIdsMatch || !studentExists || existingStudent == null)
            {
                return false;
            }

            // Uuenda olemasoleva `Student` andmed
            existingStudent.FirstName = updatedStudent.FirstName;
            existingStudent.LastName = updatedStudent.LastName;
            existingStudent.Email = updatedStudent.Email;
            existingStudent.PhoneNr = updatedStudent.PhoneNr;
            existingStudent.UserId = updatedStudent.UserId;

            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<List<Student>> FilterPendingStudents(int extracurricularId){
            var students = await context.Students.ToListAsync();
            var registrations = await context.Registrations
                .Where(reg => reg.ExtracurricularId == extracurricularId && reg.Status == "Pending")
                .ToListAsync();

            var filteredStudents = students
                .Where(student => registrations.Any(reg => reg.StudentId == student.Id))
                .ToList();

            return filteredStudents;
        }

        public async Task<List<Student>> FilterApprovedStudentsForExtracurricular(int extracurricularId){
            var registrations = await context.Registrations
                .Where(reg => reg.ExtracurricularId == extracurricularId && reg.Status == "Approved")
                .Select(reg => reg.StudentId)
                .Distinct()
                .ToListAsync();

            var students = await context.Students
                .Where(student => registrations.Contains(student.Id))
                .ToListAsync();

            return students;
        }

        public async Task<int> GetAbsenceCountForStudentAsync(int studentId, int extracurricularId)
        {
            return await context.Notes
                .Where(n => n.StudentId == studentId && 
                            n.ExtracurricularId == extracurricularId && 
                            n.NoteType == "Puudumine")
                .CountAsync();
        }

        public async Task<bool> StudentExistsInDb(int id) => await context.Students.AnyAsync(x => x.Id == id);
    }
}

using Huviringid_REST.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace Huviringid_REST.Data.Repos
{
    public class TeachersRepo(DataContext context)
    {
        private readonly DataContext context = context;

        public async Task<List<Teacher>> GetAllTeachers() {
            IQueryable<Teacher> query = context.Teachers.AsQueryable();
            return await query.ToListAsync();
        }
        public async Task<Teacher> SaveTeacherToDb(Teacher teacher) {
            context.Add(teacher);
            await context.SaveChangesAsync();
            return teacher;
        }
        public async Task<List<Teacher>> GetTeachersForExtracurricular(int extracurricularId) {
            var extracurricular = await context.Extracurriculars.FindAsync(extracurricularId);
    
            if (extracurricular == null || extracurricular.TeacherIds == null || !extracurricular.TeacherIds.Any())
            {
                return new List<Teacher>();
            }

            var teachers = await context.Teachers.Where(t => extracurricular.TeacherIds.Contains(t.Id)).ToListAsync();

            return teachers;
        }
        public async Task<Teacher?> GetTeacherById(int id) {

            var teacher = await context.Teachers.FirstOrDefaultAsync(s => s.Id == id);
            return teacher != null ? teacher : null;
        }

        public async Task<Teacher?> GetTeacherByUserId(int userId) {

            var teacher = await context.Teachers.FirstOrDefaultAsync(s => s.UserId == userId);
            return teacher != null ? teacher : null;
        }

        public async Task<bool> UpdateTeacherAsync(int id, Teacher updatedTeacher)
        {
            var existingTeacher = await context.Teachers.FindAsync(id);
            
            bool isIdsMatch = id == existingTeacher?.Id;
            bool teacherExists = await TeacherExistsInDb(id);

            if (!isIdsMatch || !teacherExists || existingTeacher == null)
            {
                return false;
            }

            // Uuenda olemasoleva `Teacher` andmed
            existingTeacher.Name = updatedTeacher.Name;
            existingTeacher.Email = updatedTeacher.Email;
            existingTeacher.UserId = updatedTeacher.UserId;

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

        public async Task<bool> TeacherExistsInDb(int id) => await context.Teachers.AnyAsync(x => x.Id == id);
    }
}
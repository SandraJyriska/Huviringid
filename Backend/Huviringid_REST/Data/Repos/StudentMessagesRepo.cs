using Huviringid_REST.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace Huviringid_REST.Data.Repos
{
    public class StudentMessagesRepo(DataContext context) {
        private readonly DataContext context = context;

        public async Task<StudentMessage> SaveStudentMessageToDb(StudentMessage studentMessage) {
            context.Add(studentMessage);
            await context.SaveChangesAsync();
            return studentMessage;
        }

        public async Task<List<StudentMessage>> GetAllStudentMessages() {
            IQueryable<StudentMessage> query = context.StudentMessages.AsQueryable();
            return await query.ToListAsync();
        }
        
        public async Task<StudentMessage?> GetStudentMessageById(int id) => await context.StudentMessages.FindAsync(id);
        public async Task<bool> StudentMessageExistsInDb(int id) => await context.StudentMessages.AnyAsync(x => x.Id == id);

        public async Task<bool> DeleteStudentMessageById(int id) {

            StudentMessage? StudentMessageInDb = await GetStudentMessageById(id);

            if(StudentMessageInDb is null) {
                return false;
            }

            context.Remove(StudentMessageInDb);
            int changesCount = await context.SaveChangesAsync();
            return changesCount == 1;
        }
        public async Task<List<StudentMessage>> GetMessagesByStudentId(int studentId)
        {
            return await context.StudentMessages
                .Where(m => m.StudentIds != null && m.StudentIds.Contains(studentId))
                .ToListAsync();
        }
    }
}

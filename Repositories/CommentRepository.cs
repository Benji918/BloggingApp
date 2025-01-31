using BloggingApp.Models;
using Microsoft.EntityFrameworkCore;
using BloggingApp.Data;

namespace BloggingApp.Repositories
{
    public class CommentRepository : IRepository<CommentsModel>
    {
        private readonly ApplicationDbcontext _context;
        public CommentRepository(ApplicationDbcontext context)
        {
            _context = context;
        }
        public async Task AddAsync(CommentsModel entity)
        {
            await _context.Comments.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<CommentsModel>> GetAllAsync()
        {
            return await _context.Comments.OrderByDescending(comment => comment.CreatedDate).
                         ToListAsync();
        }
        public async Task<CommentsModel> GetByIdAsync(int id)
        {
            var comment = _context.Comments.Find(id);
            if (comment == null)
            {
                throw new KeyNotFoundException(nameof(comment));
            }
            return comment;
        }
        public async Task UpdateAsync(CommentsModel entity)
        {
            _context.Comments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public Task<string> UploadImageAsync(IFormFile imageFile, string DestinationPath)
        {
            throw new NotImplementedException();
        }
    }
}

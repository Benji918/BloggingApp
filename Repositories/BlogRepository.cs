using BloggingApp.Data;
using BloggingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BloggingApp.Repositories
{
    public class BlogRepository : IRepository<BlogModel>
    {
        private readonly ApplicationDbcontext _context;

        public BlogRepository(ApplicationDbcontext context)
        {
            _context = context;
        }

        public async Task AddAsync(BlogModel entity)
        {
            await _context.Blogs.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);

            if (blog == null)
            {
                throw new ArgumentNullException(nameof(blog));
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BlogModel>> GetAllAsync()
        {
            return await _context.Blogs.ToListAsync();
        }

        public async Task<BlogModel> GetByIdAsync(int id)
        {
            var blog = _context.Blogs.Find(id);

            if (blog == null)
            {
                throw new KeyNotFoundException(nameof(blog));
            }

            return blog;
        }

        public async Task UpdateAsync(BlogModel entity)
        {
            _context.Blogs.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile, string DestinationPath)
        {

            // Ensure the destination directory exists
            if (!Directory.Exists(DestinationPath))
            {
                Directory.CreateDirectory(DestinationPath);
            }

            //Generate a unique filename to prevent conflits
            var fileName = $"{Path.GetRandomFileName()}{Path.GetExtension(imageFile.FileName)}";

            //Combine destination path with filename
            var filePath = Path.Combine(DestinationPath, fileName);

            //Upload image
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return filePath;


        }
    }
}

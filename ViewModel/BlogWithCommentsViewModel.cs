using BloggingApp.Models;

namespace BloggingApp.ViewModel
{
    public class BlogWithCommentsViewModel
    {

        public IEnumerable<BlogModel> Blogs { get; set; } = new List<BlogModel>();
        public IEnumerable<CommentsModel> Comments { get; set; } = new List<CommentsModel>();

    }
}

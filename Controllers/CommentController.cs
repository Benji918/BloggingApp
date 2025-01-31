using Microsoft.AspNetCore.Mvc;
using BloggingApp.Models;
using BloggingApp.Data;
using BloggingApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BloggingApp.Controllers
{
    public class CommentController : Controller
    {
        private readonly IRepository<CommentsModel> _repository;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentController(IRepository<CommentsModel> repository, UserManager<IdentityUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<CommentsModel> allComments = await _repository.GetAllAsync();
            return Ok(allComments);
        }

        [Authorize]
        [HttpGet("create/{blogId}")]
        public IActionResult Create(int blogId) // Add blogId as a parameter
        {
            if (blogId <= 0)
            {
                return BadRequest("Invalid blog ID.");
            }

            // Pass the blogId to the view
            ViewBag.BlogId = blogId;
            return View();
        }


        [Authorize]
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentsModel comment)
        {

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            // Set the User and Blog properties
            comment.User = currentUser;
            comment.UserId = currentUser.Id;
            comment.BlogId = ViewBag.BlogId;

            // Clear and revalidate the ModelState
            ModelState.Clear();
            TryValidateModel(comment);

            if (!ModelState.IsValid)
            {
                // Log all the errors for the ModelState
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Error for {key}: {error.ErrorMessage}");
                    }
                }
                ViewBag.BlogId = comment.BlogId;
                return View(comment);
            }

            // Add the comment to the database
            try
            {
                TempData["Message"] = "Comment added successfully!";
                await _repository.AddAsync(comment);
                return RedirectToAction(actionName: "Index", controllerName: "Blog");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while adding the comment. {ex.Message}");
                ViewBag.BlogId = comment.BlogId;
                return View(comment);
            }

        }

        //[HttpGet("GetCommentsByBlogId/{blogId}")]
        //public async Task<IActionResult> GetCommentsByBlogId(int blogId)
        //{
        //    var comments = await _repository.GetByIdAsync(blogId);
        //    return Ok(comments);
        //}
    }
}

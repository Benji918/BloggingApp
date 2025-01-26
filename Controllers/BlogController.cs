using Microsoft.AspNetCore.Mvc;
using BloggingApp.Data;
using BloggingApp.Models;
using Microsoft.AspNetCore.Identity;
using BloggingApp.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace BloggingApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly IRepository<BlogModel> _repository;
        private readonly UserManager<IdentityUser> _userManager;

        public BlogController(IRepository<BlogModel> repository, UserManager<IdentityUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            IEnumerable<BlogModel> blogs = await _repository.GetAllAsync();
            return View(blogs);

        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogModel blogModel, IFormFile ImageUrl)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized("UserId not found");
            }

            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            if (currentUser == null)
            {
                return Unauthorized("User not found");
            }

            //Handle the image upload
            if (ImageUrl != null && ImageUrl.Length > 0)
            {
                try
                {
                    //Ensure the image is uploaded to the correct folder
                    string DestinationPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "blogs");

                    if (!Directory.Exists(DestinationPath))
                    {
                        Directory.CreateDirectory(DestinationPath);
                    }

                    var imageUrlString = await _repository.UploadImageAsync(ImageUrl, DestinationPath);

                    blogModel.ImageUrl = $"/blogs/{Path.GetFileName(imageUrlString)}"; // Save the relative URL
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ImageUrl", ex.Message);
                }

            }

            blogModel.UserId = currentUserId;
            blogModel.User = currentUser;

            // Clear the validation error for UserId and revalidate
            ModelState.ClearValidationState(nameof(blogModel.UserId));
            ModelState.ClearValidationState(nameof(blogModel.User));
            ModelState.ClearValidationState(nameof(blogModel.ImageUrl));
            TryValidateModel(blogModel);

            if (ModelState.IsValid)
            {
                await _repository.AddAsync(blogModel);
                return RedirectToAction(nameof(Index));
            }

            //Log all th errors for the ModelState
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"Error for {key}: {error.ErrorMessage}");
                }
            }

            return View(blogModel);
        }

        [HttpGet]
        public IActionResult Update()
        {
            return View();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPut]
        public async Task<IActionResult> Update(BlogModel blogModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User Not Found!");
            }

            await _repository.UpdateAsync(blogModel);
            return View(blogModel);
        }
    }
}

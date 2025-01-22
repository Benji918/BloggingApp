using Microsoft.AspNetCore.Mvc;
using BloggingApp.Data;
using BloggingApp.Models;
using Microsoft.AspNetCore.Identity;
using BloggingApp.Repositories;

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

        public IActionResult Index()
        {
            return View();
        }

        //public async Task<IActionResult> Index()
        //{
        //    return NoContent();
        //}
    }
}

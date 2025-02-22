using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseAndMasterpiece.Interfaces;
using MuseAndMasterpiece.Models;
using MuseAndMasterpiece.Services;

namespace MuseAndMasterpiece.Controllers
{
    public class CategoryPageController : Controller
    {
        private readonly IArtistService _artistService;
        private readonly IArtworkService _artworkService;
        private readonly ICategoryService _categoryService;

        // dependency injection of service interface
        public CategoryPageController(IArtistService ArtistService, IArtworkService ArtworkService, ICategoryService CategoryService)
        {
            _artistService = ArtistService;
            _artworkService = ArtworkService;
            _categoryService = CategoryService;
        }


        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: CategoryPage/ListCategories
        [HttpGet("ListCategories")]
        public async Task<IActionResult> List()
        {
            IEnumerable<CategoryDto?> CategoryDtos = await _categoryService.ListCategories();
            return View(CategoryDtos);
        }




        // GET: CategoryPage/Details/{id}
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            CategoryDto? categoryDto = await _categoryService.FindCategory(id);
            if (categoryDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = [$"Category with ID {id} not found."] });
            }
            return View(categoryDto);
        }




        // GET: CategoryPage/AddCategory
        [HttpGet("AddCategory")]
        [Authorize]
        public IActionResult Add()
        {
            return View(new AddCategoryDto());
        }



        // POST: CategoryPage/AddCategory
        [HttpPost("AddCategory")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Add(AddCategoryDto categoryDto)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.AddCategory(categoryDto);
                return RedirectToAction("List");
            }

            return View(categoryDto);
        }







        // GET: CategoryPage/EditCategory/{id}
        [HttpGet("EditCategory/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.FindCategory(id);

            if (category == null)
            {
                return View("Error", new ErrorViewModel { Errors = [$"Category with ID {id} not found."] });
            }

            var updateCategoryDto = new UpdateCategoryDto
            {
                CategoryId = category.CategoryId,
                CName = category.CName
            };

            return View(updateCategoryDto);
        }


        // POST: CategoryPage/EditCategory/{id}
        [HttpPost("EditCategory/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, UpdateCategoryDto updateCategoryDto)
        {
            if (id != updateCategoryDto.CategoryId)
            {
                return View("Error", new ErrorViewModel { Errors = ["Invalid category ID"] });
            }

            if (ModelState.IsValid)
            {
                var serviceResponse = await _categoryService.UpdateCategory(id, updateCategoryDto);

                if (serviceResponse.Status == ServiceResponse.ServiceStatus.Error)
                {
                    return View("Error", new ErrorViewModel { Errors = serviceResponse.Messages });
                }

                return RedirectToAction("List");
            }

            return View(updateCategoryDto);
        }






        // GET: CategoryPage/ConfirmDelete/{id}
        [HttpGet("Category/ConfirmDelete/{id}")]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var category = await _categoryService.FindCategory(id);

            if (category == null)
            {
                return View("Error", new ErrorViewModel { Errors = [$"Category with ID {id} not found."] });
            }

            return View("ConfirmDelete", category);
        }

        // POST: CategoryPage/Delete/{id}
        [HttpPost("Category/Delete/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _categoryService.DeleteCategory(id);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }

            return RedirectToAction("List");
        }





    }
}

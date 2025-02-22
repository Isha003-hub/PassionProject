using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MuseAndMasterpiece.Interfaces;
using MuseAndMasterpiece.Models;

namespace MuseAndMasterpiece.Controllers
{
    [Route("ArtworkPage")]
    public class ArtworkPageController : Controller
    {

        private readonly IArtistService _artistService;
        private readonly IArtworkService _artworkService;
        private readonly ICategoryService _categoryService;

        // dependency injection of service interface
        public ArtworkPageController(IArtistService ArtistService, IArtworkService ArtworkService, ICategoryService CategoryService)
        {
            _artistService = ArtistService;
            _artworkService = ArtworkService;
            _categoryService = CategoryService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        [HttpGet("ListArtworks")]
        public async Task<IActionResult> List()
        {
            IEnumerable<ArtworkDto?> artworkDtos = await _artworkService.ListArtworks();
            return View(artworkDtos);
        }

        [HttpGet("ArtworkDetails/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            ArtworkDto? artworkDto = await _artworkService.FindArtwork(id);

            if (artworkDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Artwork not found"] });
            }

            return View(artworkDto);
        }

       

        [HttpGet("AddArtwork")]
        [Authorize]
        public async Task<IActionResult> Add()
        {
            var artists = await _artistService.ListArtists();
            var categories = await _categoryService.ListCategories();

            var viewModel = new AddArtworkDto
            {
                // Initializing Artists and Categories dropdown lists
                Artists = artists.Select(a => new SelectListItem { Value = a.ArtistId.ToString(), Text = a.Name }).ToList(),
                Categories = categories.Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.CName }).ToList()
            };

            return View(viewModel);
        }

        // POST: ArtworkPage/AddArtwork
        [HttpPost("AddArtwork")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Add(AddArtworkDto artworkDto)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns if validation fails
                artworkDto.Artists = (await _artistService.ListArtists())
                    .Select(a => new SelectListItem { Value = a.ArtistId.ToString(), Text = a.Name }).ToList();
                artworkDto.Categories = (await _categoryService.ListCategories())
                    .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.CName }).ToList();

                return View(artworkDto);
            }

            await _artworkService.AddArtwork(artworkDto);
            return RedirectToAction("List");
        }

        
        // GET: ArtworkPage/Edit/{id}
        [HttpGet("EditArtwork/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var artwork = await _artworkService.FindArtwork(id);
            if (artwork == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Artwork not found"] });
            }

            var viewModel = new UpdateArtworkDto
            {
                ArtWorkId = artwork.ArtWorkId,
                Title = artwork.Title,
                Description = artwork.Description,
                DatePosted = artwork.DatePosted,
                ArtistId = artwork.ArtistId,
                CategoryId = artwork.CategoryId,
                Artists = (await _artistService.ListArtists())
                    .Select(a => new SelectListItem { Value = a.ArtistId.ToString(), Text = a.Name }).ToList(),
                Categories = (await _categoryService.ListCategories())
                    .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.CName }).ToList()
            };

            return View(viewModel);
        }

        // POST: ArtworkPage/Edit/{id}
        [HttpPost("EditArtwork/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, UpdateArtworkDto viewModel)
        {
            if (id != viewModel.ArtWorkId)
            {
                return View("Error", new ErrorViewModel { Errors = ["Artwork ID mismatch"] });
            }

            if (!ModelState.IsValid)
            {
                viewModel.Artists = (await _artistService.ListArtists())
                    .Select(a => new SelectListItem { Value = a.ArtistId.ToString(), Text = a.Name }).ToList();
                viewModel.Categories = (await _categoryService.ListCategories())
                    .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.CName }).ToList();
                return View(viewModel);
            }

            var response = await _artworkService.UpdateArtwork(id, viewModel);

            if (response.Status == ServiceResponse.ServiceStatus.Error || response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }

            return RedirectToAction("Details", new { id });
        }







        [HttpGet("ConfirmDelete/{id}")]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            ArtworkDto? artworkDto = await _artworkService.FindArtwork(id);
            if (artworkDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Artwork not found"] });
            }
            return View(artworkDto);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _artworkService.DeleteArtwork(id);
            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List");
            }
            return View("Error", new ErrorViewModel { Errors = response.Messages });
        }


    }
}

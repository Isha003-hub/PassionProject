using Microsoft.AspNetCore.Mvc;
using MuseAndMasterpiece.Interfaces;
using MuseAndMasterpiece.Services;
using MuseAndMasterpiece.Models;
using Microsoft.AspNetCore.Authorization;

namespace MuseAndMasterpiece.Controllers
{
    public class ArtistPageController : Controller
    {

        private readonly IArtistService _artistService;
        private readonly IArtworkService _artworkService;
        private readonly ICategoryService _categoryService;

        // dependency injection of service interface
        public ArtistPageController(IArtistService ArtistService, IArtworkService ArtworkService, ICategoryService CategoryService)
        {
            _artistService = ArtistService;
            _artworkService = ArtworkService;
            _categoryService = CategoryService;
        }




        // Show List of Artists on Index page 
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: ArtistPage/ListArtists
        [HttpGet("ListArtists")]
        public async Task<IActionResult> List()
        {
            IEnumerable<ArtistDto?> ArtistDtos = await _artistService.ListArtists();
            return View(ArtistDtos);
        }




        [HttpGet("ArtistDetails/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            ArtistDto? artistDto = await _artistService.FindArtist(id);

            if (artistDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find artist"] });
            }
            else
            {
              
                return View(artistDto);
            }
        }




        // GET: ArtistPage/AddArtist
        [HttpGet("AddArtist")]
        [Authorize]
        public IActionResult Add()
        {
            return View(new AddArtistDto());
        }

        // POST: ArtistPage/AddArtist
        [HttpPost("AddArtist")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Add(AddArtistDto artistDto)
        {
            if (ModelState.IsValid)
            {
                await _artistService.AddArtist(artistDto);
                return RedirectToAction("List");
            }

            return View(artistDto);
        }

        // GET: ArtistPage/EditArtist/{id}
        [HttpGet("EditArtist/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            ArtistDto? artistDto = await _artistService.FindArtist(id);

            if (artistDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Artist not found"] });
            }

            var updateArtistDto = new UpdateArtistDto
            {
                ArtistId = artistDto.ArtistId,
                Name = artistDto.Name
            };

            return View(updateArtistDto);
        }

        // POST: ArtistPage/EditArtist/{id}
        [HttpPost("EditArtist/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, UpdateArtistDto updateArtistDto)
        {
            if (id != updateArtistDto.ArtistId)
            {
                return View("Error", new ErrorViewModel() { Errors = [ "Invalid artist ID"]});
            }

            if (ModelState.IsValid)
            {
                var serviceResponse = await _artistService.UpdateArtist(id, updateArtistDto);

                if (serviceResponse.Status == ServiceResponse.ServiceStatus.Error)
                {
                    return View("Error", new ErrorViewModel() { Errors = serviceResponse.Messages });
                }

                return RedirectToAction("Details", new { id });
            }

            return View(updateArtistDto);
        }

        // GET: ArtistPage/DeleteArtist/{id}
        [HttpGet("Artist/ConfirmDelete/{id}")]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            ArtistDto? artistDto = await _artistService.FindArtist(id);

            if (artistDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors =["Artist not found" ] });
            }

            return View("ConfirmDelete", artistDto);
        }

        // POST: ArtistPage/DeleteArtist/{id}
        [HttpPost("Artist/Delete/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _artistService.DeleteArtist(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "ArtistPage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

    }
}

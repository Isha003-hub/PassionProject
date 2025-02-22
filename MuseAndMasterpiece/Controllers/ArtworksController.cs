using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuseAndMasterpiece.Data;
using MuseAndMasterpiece.Models;
using NuGet.DependencyResolver;
using MuseAndMasterpiece.Interfaces;
using MuseAndMasterpiece.Services;
using Microsoft.AspNetCore.Authorization;

namespace MuseAndMasterpiece.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtworksController : ControllerBase
    {
        private readonly IArtworkService _artworkservice;

        public ArtworksController(IArtworkService context)
        {
            _artworkservice = context;
        }

        /// <summary>
        /// Returns a list of Artworks including the Name of Artist who created the Artwork and Category it belongs.
        /// </summary>
        /// <returns>
        /// 200 Ok
        /// List of Artworks including ID, Title, ArtistName and CategoryName
        /// </returns>
        /// <example>
        /// GET: api/Artworks/List -> [{ArtworkId: 1, Title: "Elegant Script", ArtistName : "Alice Smith", CategoryName : "Portrait"},{....},{....}]
        /// </example>
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<ArtworkDto>>> ListArtworks()
        {
            IEnumerable<ArtworkDto> artworkDtos = await _artworkservice.ListArtworks();
            return Ok(artworkDtos);
        }



        /// <summary>
        /// Return a Artwork specified by it's {id}
        /// </summary>
        /// /// <param name="id">Artwork's id</param>
        /// <returns>
        /// 200 Ok
        /// ArtworkDto : It includes Artwork's ID, Title, ArtistName and CategoryName.
        /// or
        /// 404 Not Found when there is no Artwork for that {id}
        /// </returns>
        /// <example>
        /// GET: api/Artworks/Find/1 -> {ArtworkId: 1, Title: "Elegant Script", ArtistName : "Alice Smith", CategoryName : "Portrait"}

        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<ArtworkDto>> FindArtwork(int id)
        {
            var artwork = await _artworkservice.FindArtwork(id);

            if (artwork == null)
            {
                return NotFound($"Artwork with {id} doesn't exist");
            }
            else
            {
                return Ok(artwork);
            }
        }

        /// <summary>
        /// It updates an Artwork
        /// </summary>
        /// <param name="id">The ID of Artwork which we want to update</param>
        /// <param name="ArtworkDto">The required information to update the Artwork</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>       
        [HttpPut(template: "Update/{id}")]
        [Authorize]
        public async Task<ServiceResponse> UpdateArtwork(int id, UpdateArtworkDto updateartworkDto)
        {
            if (id != updateartworkDto.ArtWorkId)
            {
                return new ServiceResponse
                {
                    Status = ServiceResponse.ServiceStatus.Error,
                    Messages = new List<string> { "Artwork ID mismatch." }
                };
            }

            ServiceResponse response = await _artworkservice.UpdateArtwork(id, updateartworkDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return new ServiceResponse
                {
                    Status = ServiceResponse.ServiceStatus.NotFound,
                    Messages = new List<string> { "Artwork not found." }
                };
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return new ServiceResponse
                {
                    Status = ServiceResponse.ServiceStatus.Error,
                    Messages = new List<string> { "An unexpected error occurred while updating the artwork." }
                };
            }

            return new ServiceResponse
            {
                Status = ServiceResponse.ServiceStatus.Updated,
                Messages = new List<string> { $"Artwork with ID {id} updated successfully." }
            };
        }



        /// <summary>
        /// Adds an Artwork 
        /// </summary>
        /// <remarks>
        /// We add an Artwork with the necessary fields in an AddUpdArtworkDto
        /// </remarks>
        /// <param name="AddUpdArtworkDto">The required information to add the Artwork</param
        /// <returns>
        /// 201 Created or 404 Not Found
        /// </returns>
        /// <example>
        /// api/Artworks/Add -> Add the Artwork
        /// </example>
        [HttpPost(template: "Add")]
        [Authorize]
        public async Task<ServiceResponse> AddArtwork(AddArtworkDto addartworkDto)
        {
            ServiceResponse response = await _artworkservice.AddArtwork(addartworkDto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return new ServiceResponse
                {
                    Status = ServiceResponse.ServiceStatus.Error,
                    Messages = new List<string> { "An unexpected error occurred while adding the artwork." }
                };
            }

            return new ServiceResponse
            {
                Status = ServiceResponse.ServiceStatus.Created,
                CreatedId = response.CreatedId,
                Messages = new List<string> { $"Artwork added successfully with ID {response.CreatedId}" }
            };
        }


        /// <summary>
        /// Delete a Artwork specified by it's {id}
        /// </summary>
        /// <param name="id">The id of the Artwork we want to delete</param>
        /// <returns>
        /// 201 No Content or 404 Not Found
        /// </returns>
        /// <example>
        /// api/Artworks/Delete/{id} -> Deletes the Artwork associated with {id}
        /// </example>

        [HttpDelete(template: "Delete/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteArtwork(int id)
        {
            ServiceResponse response = await _artworkservice.DeleteArtwork(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Artwork not found." });
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while deleting the artwork." });
            }

            return Ok(new { message = $"Artwork with ID {id} deleted successfully." });
        }


        /// <summary>
        /// Retrieves a list of artwork titles based on the specified category ID.
        /// </summary>
        /// <param name="categoryId">The ID of the category to filter artworks.</param>
        /// <returns>
        /// 200 OK - A list of artwork titles.
        /// 404 Not Found - If no artworks exist for the specified category.
        /// </returns>
        /// <example>
        /// GET: api/Artworks/category/1 -> ["Sunset Glow", "Ocean Breeze", "Mountain Serenity"]
        /// </example>

        //[HttpGet("category/{categoryId}")]
        //public async Task<IActionResult> GetArtworkTitlesByCategory(int categoryId)
        //{
        //    var artworkTitles = await _artworkservice.GetArtworkTitlesByCategory(categoryId);

        //    if (!artworkTitles.Any())
        //    {
        //        return NotFound(new { message = "No artworks found for this category." });
        //    }

        //    return Ok(artworkTitles);
        //}



       

    }
}

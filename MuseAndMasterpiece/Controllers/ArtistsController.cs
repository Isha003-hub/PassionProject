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
    public class ArtistsController : ControllerBase
    {
        private readonly IArtistService _artistservice;

        public ArtistsController(IArtistService context)
        {
            _artistservice = context;
        }

        /// <summary>
        /// Returns a list of Artists including the Number of Artworks they posted.
        /// </summary>
        /// <returns>
        /// 200 Ok
        /// List of Artists including ID, Name, Bio, Total no. of Artworks they posted, Titles of all the Artworks they posted.
        /// </returns>
        /// <example>
        /// GET: api/Artists/List -> [{ArtistId: 1, Name: "Alice Smith", Bio: "Alice is a contemporary abstract artist.", TotalArtworks: 2, ArtworksTitle:["Elegant Script","Modern Calligraphy"]},{....},{....}]
        /// </example>
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<ArtistDto>>> ListArtists()
        {
            IEnumerable<ArtistDto> TeacherDtos = await _artistservice.ListArtists();

            // return 200 OK with TeacherDtos
            return Ok(TeacherDtos);

        }


        /// <summary>
        /// Return a Artist specified by it's {id}
        /// </summary>
        /// /// <param name="id">Artist's id</param>
        /// <returns>
        /// 200 Ok
        /// ArtistDto : It includes Artist's ID, Name, Total no. of Artworks they posted, Titles of all the Artworks they posted.
        /// or
        /// 404 Not Found when there is no Artist for that {id}
        /// </returns>
        /// <example>
        /// GET: api/Artists/Find/1 -> {ArtistId: 1, Name: "Alice Smith", Bio: "Alice is a contemporary abstract artist.", TotalArtworks: 2, ArtworksTitle:["Elegant Script","Modern Calligraphy"]}
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<ArtistDto>> FindArtist(int id)
        {
            var artist = await _artistservice.FindArtist(id);

            if (artist == null)
            {
                return NotFound($"Artist with {id} doesn't exist");
            }
            else
            {
                return Ok(artist);
            }


        }

        /// <summary>
        /// It updates an Artist
        /// </summary>
        /// <param name="id">The ID of Artist which we want to update</param>
        /// <param name="ArtistDto">The required information to update the Artist</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>       
        [HttpPut(template: "Update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateArtist(int id, UpdateArtistDto updateartistDto)
        {
            if (id != updateartistDto.ArtistId)
            {
                return BadRequest(new { message = "Artist ID mismatch." });
            }

            ServiceResponse response = await _artistservice.UpdateArtist(id, updateartistDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Artist not found." });
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while updating the artist." });
            }

            return Ok(new { message = $"Artist with ID {id} updated successfully." });
        }



        /// <summary>
        /// Adds an Artist 
        /// </summary>
        /// <remarks>
        /// We add an Artist with the necessary fields in an AddUpdArtistDto
        /// </remarks>
        /// <param name="AddUpdArtistDto">The required information to add the Artist</param
        /// <returns>
        /// 201 Created or 404 Not Found
        /// </returns>
        /// <example>
        /// api/Artists/Add -> Add the Artist
        /// </example>
        [HttpPost(template: "Add")]
        [Authorize]
        public async Task<ActionResult<Artist>> AddArtist(AddArtistDto addartistDto)
        {
            ServiceResponse response = await _artistservice.AddArtist(addartistDto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while adding the artist." });
            }

            return CreatedAtAction("FindArtist", new { id = response.CreatedId }, new
            {
                message = $"Artist added successfully with ID {response.CreatedId}",
                artistId = response.CreatedId
            });
        }


        /// <summary>
        /// Delete a Artist specified by it's {id}
        /// </summary>
        /// <param name="id">The id of the Artist we want to delete</param>
        /// <returns>
        /// 201 No Content or 404 Not Found
        /// </returns>
        /// <example>
        /// api/Artists/Delete/{id} -> Deletes the Artist associated with {id}
        /// </example>
        [HttpDelete(template: "Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            ServiceResponse response = await _artistservice.DeleteArtist(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Artist not found." });
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while deleting the artist." });
            }

            return Ok(new { message = $"Artist with ID {id} deleted successfully." });
        }



    }
}

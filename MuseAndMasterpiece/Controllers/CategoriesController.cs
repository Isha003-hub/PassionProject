using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuseAndMasterpiece.Data;
using MuseAndMasterpiece.Data.Migrations;
using MuseAndMasterpiece.Models;
using NuGet.DependencyResolver;
using MuseAndMasterpiece.Interfaces;
using MuseAndMasterpiece.Services;
using Microsoft.AspNetCore.Authorization;

namespace MuseAndMasterpiece.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryservice;

        public CategoriesController(ICategoryService context)
        {
            _categoryservice = context;
        }

        /// <summary>
        /// Returns a list of Categories including the total number of Artworks it holds and the list of all Artwork Titles it holds.
        /// </summary>
        /// <returns>
        /// 200 Ok
        /// List of Categories including ID, Name of Category, Total Artworks it holds and all the Artwork Titles.
        /// </returns>
        /// <example>
        /// GET: api/Categories/List -> [{CategoryId: 1, CName: "Calligraphy", TotalArtworks: 2, ArtworkTitles: ["Elegant Script","Modern Calligraphy"]},{....},{....}]
        /// </example>
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> ListCategories()
        {
            IEnumerable<CategoryDto> categoryDtos = await _categoryservice.ListCategories();
            return Ok(categoryDtos);
        }


        /// <summary>
        /// Return a Category specified by it's {id}
        /// </summary>
        /// /// <param name="id">Category's id</param>
        /// <returns>
        /// 200 Ok
        /// CategoryDto : It includes ID, Name of Category, Total Artworks it holds and all the Artwork Titles.
        /// or
        /// 404 Not Found when there is no Category for that {id}
        /// </returns>
        /// <example>
        /// GET: api/Categories/Find/1 -> {CategoryId: 1, CName: "Calligraphy", TotalArtworks: 2, ArtworkTitles: ["Elegant Script","Modern Calligraphy"]}
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<CategoryDto>> FindCategory(int id)
        {
            var category = await _categoryservice.FindCategory(id);

            if (category == null)
            {
                return NotFound($"Category with {id} doesn't exist");
            }
            else
            {
                return Ok(category);
            }
        }


        /// <summary>
        /// It updates an Category
        /// </summary>
        /// <param name="id">The ID of Category which we want to update</param>
        /// <param name="CategoryDto">The required information to update the Category</param>
        /// <returns>
        /// 400 Bad Request or 404 Not Found or 204 No Content
        /// </returns>       
        [HttpPut(template: "Update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updatecategoryDto)
        {
            if (id != updatecategoryDto.CategoryId)
            {
                return BadRequest(new { message = "Category ID mismatch." });
            }

            ServiceResponse response = await _categoryservice.UpdateCategory(id, updatecategoryDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Category not found." });
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while updating the category." });
            }

            return Ok(new { message = $"Category with ID {id} updated successfully." });
        }


        /// <summary>
        /// Adds an Category 
        /// </summary>
        /// <remarks>
        /// We add an Category with the necessary fields in an AddUpdCategoryDto
        /// </remarks>
        /// <param name="AddUpdCategoryDto">The required information to add the Category</param
        /// <returns>
        /// 201 Created or 404 Not Found
        /// </returns>
        /// <example>
        /// api/Categories/Add -> Add the Category
        /// </example>

        [HttpPost(template: "Add")]
        [Authorize]
        public async Task<ActionResult<Category>> AddCategory(AddCategoryDto addcategoryDto)
        {
            ServiceResponse response = await _categoryservice.AddCategory(addcategoryDto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while adding the category." });
            }

            return CreatedAtAction("FindCategory", new { id = response.CreatedId }, new
            {
                message = $"Category added successfully with ID {response.CreatedId}",
                categoryId = response.CreatedId
            });
        }


        /// <summary>
        /// Delete a Category specified by it's {id}
        /// </summary>
        /// <param name="id">The id of the Category we want to delete</param>
        /// <returns>
        /// 201 No Content or 404 Not Found
        /// </returns>
        /// <example>
        /// api/Categories/Delete/{id} -> Deletes the Category associated with {id}
        /// </example>
        [HttpDelete(template: "Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            ServiceResponse response = await _categoryservice.DeleteCategory(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Category not found." });
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while deleting the category." });
            }

            return Ok(new { message = $"Category with ID {id} deleted successfully." });
        }


    }
}

using MuseAndMasterpiece.Data;
using MuseAndMasterpiece.Interfaces;
using MuseAndMasterpiece.Models;
using Microsoft.EntityFrameworkCore;
using MuseAndMasterpiece.Data;
using System;
using Microsoft.AspNetCore.Mvc;

namespace MuseAndMasterpiece.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Returns a list of Categories including the total number of Artworks it holds and the list of all Artwork Titles it holds.
        /// </summary>
        /// <returns>
        /// 200 Ok
        /// List of Categories including ID, Name of Category, Total Artworks it holds and all the Artwork Titles.
        /// </returns>
        public async Task<IEnumerable<CategoryDto>> ListCategories()
        {
            List<Category> Categories = await _context.Categories
                .Include(c => c.Artworks)
                .ToListAsync();

            List<CategoryDto> CategoryDtos = new List<CategoryDto>();

            foreach (Category Category in Categories)
            {
                CategoryDtos.Add(new CategoryDto()
                {
                    CategoryId = Category.CategoryId,
                    CName = Category.CName,
                    DateCreated = Category.DateCreated,
                    TotalArtworks = Category.Artworks.Count(),
                    ArtworksTitle = Category.Artworks != null ? Category.Artworks.Select(a => a.Title).ToList() : new List<string>()

                });

            }
            // return CategoryDtos
            return CategoryDtos;
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
        public async Task<CategoryDto> FindCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Artworks)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return null;
            }

            CategoryDto categoryDto = new CategoryDto()
            {
                CategoryId = category.CategoryId,
                CName = category.CName,
                DateCreated = category.DateCreated,
                TotalArtworks = category.Artworks.Count(),
                ArtworksTitle = category.Artworks != null ? category.Artworks.Select(a => a.Title).ToList() : new List<string>()

            };


            return categoryDto;
        }

        /// <summary>
        /// It updates an Category
        /// </summary>
        /// <param name="id">The ID of Category which we want to update</param>
        /// <param name="CategoryDto">The required information to update the Category</param>
        /// <returns>
        /// set status to updated when update is done successfully
        /// </returns>       
        public async Task<ServiceResponse> UpdateCategory(int id, UpdateCategoryDto updatecategoryDto)
        {
            ServiceResponse serviceResponse = new();

            if (id != updatecategoryDto.CategoryId)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Category ID mismatch.");
                return serviceResponse;
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Category not found.");
                return serviceResponse;
            }

            // Update only the necessary fields
            category.CName = updatecategoryDto.CName;
            category.DateCreated = updatecategoryDto.DateCreated;

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("An error occurred while updating the category.");
                return serviceResponse;
            }

            // Set status as Updated
            serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            return serviceResponse;
        }



        /// <summary>
        /// Adds an Category 
        /// </summary>
        /// <remarks>
        /// We add an Category with the necessary fields in an AddUpdCategoryDto
        /// </remarks>
        /// <param name="AddUpdCategoryDto">The required information to add the Category</param
        /// <returns>
        ///  set status to created when category is added successfully
        /// </returns>
        public async Task<ServiceResponse> AddCategory(AddCategoryDto addcategoryDto)
        {
            ServiceResponse serviceResponse = new();

            Category category = new Category()
            {
                CName = addcategoryDto.CName,
                DateCreated = addcategoryDto.DateCreated
            };

            try
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the category.");
                serviceResponse.Messages.Add(ex.Message);
                return serviceResponse;
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = category.CategoryId;
            return serviceResponse;
        }


        /// <summary>
        /// Delete a Category specified by it's {id}
        /// </summary>
        /// <param name="id">The id of the Category we want to delete</param>
        /// <returns>
        /// set status to deleted when category is deleted successfully
        /// </returns>
        public async Task<ServiceResponse> DeleteCategory(int id)
        {
            ServiceResponse serviceResponse = new();

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Category cannot be deleted because it does not exist.");
                return serviceResponse;
            }

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error encountered while deleting the category.");
                return serviceResponse;
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }


    }
}

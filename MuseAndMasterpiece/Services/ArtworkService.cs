using MuseAndMasterpiece.Data;
using MuseAndMasterpiece.Interfaces;
using MuseAndMasterpiece.Models;
using Microsoft.EntityFrameworkCore;
using MuseAndMasterpiece.Data;
using System;
using Microsoft.AspNetCore.Mvc;

namespace MuseAndMasterpiece.Services
{
    public class ArtworkService : IArtworkService
    {
        private readonly ApplicationDbContext _context;

        public ArtworkService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of Artworks including the Name of Artist who created the Artwork and Category it belongs.
        /// </summary>
        /// <returns>
        /// List of Artworks including ID, Title, ArtistName and CategoryName
        /// </returns>
        public async Task<IEnumerable<ArtworkDto>> ListArtworks()
        {
            List<Artwork> Artworks = await _context.Artworks
                .Include(a => a.Category)
                .Include(a => a.Artist)
                .ToListAsync();

            List<ArtworkDto> ArtworkDtos = new List<ArtworkDto>();

            foreach (Artwork Artwork in Artworks)
            {
                ArtworkDtos.Add(new ArtworkDto()
                {
                    ArtWorkId = Artwork.ArtWorkId,
                    Title = Artwork.Title,
                    DatePosted = Artwork.DatePosted,
                    ArtistName = Artwork.Artist.Name,
                    CategoryName = Artwork.Category.CName

                });

            }
            // return ArtworkDtos
            return ArtworkDtos;
        }


        /// <summary>
        /// Return a Artwork specified by it's {id}
        /// </summary>
        /// /// <param name="id">Artwork's id</param>
        /// <returns>
        /// ArtworkDto : It includes Artwork's ID, Title, ArtistName and CategoryName.
        /// or
        /// null when there is no Artwork for that {id}
        /// </returns>
        public async Task<ArtworkDto> FindArtwork(int id)
        {
            var artwork = await _context.Artworks
                .Include(a => a.Category)
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(a => a.ArtWorkId == id);

            if (artwork == null)
            {
                return null;
            }

            ArtworkDto artistDto = new ArtworkDto()
            {
                ArtWorkId = artwork.ArtWorkId,
                Title = artwork.Title,
                DatePosted = artwork.DatePosted,
                ArtistName = artwork.Artist.Name,
                CategoryName = artwork.Category.CName

            };


            return artistDto;
        }

        /// <summary>
        /// It updates an Artwork
        /// </summary>
        /// <param name="id">The ID of Artwork which we want to update</param>
        /// <param name="ArtworkDto">The required information to update the Artwork</param>
        /// <returns>
        /// set status to updated if artwork is updated successfully
        /// </returns>       
        public async Task<ServiceResponse> UpdateArtwork(int id, UpdateArtworkDto updateartworkDto)
        {
            ServiceResponse serviceResponse = new();

            if (id != updateartworkDto.ArtWorkId)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Artwork ID mismatch.");
                return serviceResponse;
            }

            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Artwork not found.");
                return serviceResponse;
            }

            artwork.Title = updateartworkDto.Title;
            artwork.DatePosted = updateartworkDto.DatePosted;
            artwork.Description = updateartworkDto.Description;
            artwork.CategoryId = updateartworkDto.CategoryId;
            artwork.ArtistId = updateartworkDto.ArtistId;

            _context.Entry(artwork).State = EntityState.Modified;

            try
            {
                
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("An error occurred updating the artwork.");
                return serviceResponse;
            }

            // Set status as Updated
            serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            serviceResponse.Messages.Add($"Artwork with ID {id} updated successfully.");
            return serviceResponse;
        }



        /// <summary>
        /// Adds an Artwork 
        /// </summary>
        /// <remarks>
        /// We add an Artwork with the necessary fields in an AddUpdArtworkDto
        /// </remarks>
        /// <param name="AddArtworkDto">The required information to add the Artwork</param
        /// <returns>
        /// set status to created if artwork is added successfully
        /// </returns>

        public async Task<ServiceResponse> AddArtwork(AddArtworkDto addartworkDto)
        {
            ServiceResponse serviceResponse = new();

            Artwork artwork = new Artwork()
            {
                Title = addartworkDto.Title,
                DatePosted = addartworkDto.DatePosted,
                Description = addartworkDto.Description,
                CategoryId = addartworkDto.CategoryId,
                ArtistId = addartworkDto.ArtistId,
            };

            try
            {
                _context.Artworks.Add(artwork);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the Artwork.");
                serviceResponse.Messages.Add(ex.Message);
                return serviceResponse;
            }

            // Set status as Created
            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = artwork.ArtWorkId;
            return serviceResponse;
        }


        /// <summary>
        /// Delete a Artwork specified by it's {id}
        /// </summary>
        /// <param name="id">The id of the Artwork we want to delete</param>
        /// <returns>
        /// set status to deleted if artwork is deleted successfully
        /// </returns>
        public async Task<ServiceResponse> DeleteArtwork(int id)
        {
            ServiceResponse serviceResponse = new();

            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Artwork cannot be deleted because it does not exist.");
                return serviceResponse;
            }

            try
            {
                _context.Artworks.Remove(artwork);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error encountered while deleting the artwork");
                serviceResponse.Messages.Add(ex.Message);
                return serviceResponse;
            }

            // Set status as Deleted
            serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }





        /// <summary>
        /// Links an artwork to a specified category.
        /// </summary>
        /// <param name="artworkId">The ID of the artwork to be linked.</param>
        /// <param name="categoryId">The ID of the category to link the artwork to.</param>
        /// <returns>
        /// 200 OK if successfully linked.
        /// 404 Not Found if the artwork or category does not exist.
        /// </returns>

        public async Task<ServiceResponse> LinkArtworkToCategory(int artworkId, int categoryId)
        {
            ServiceResponse response = new();

            // Fetch artwork from the database
            var artwork = await _context.Artworks.FindAsync(artworkId);
            if (artwork == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Artwork not found.");
                return response;
            }

            // Fetch category from the database
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Category not found.");
                return response;
            }

            // Check if the artwork is already linked to the category
            if (artwork.CategoryId == categoryId)
            {
                response.Status = ServiceResponse.ServiceStatus.AlreadyExists;
                response.Messages.Add("Artwork is already linked to this category.");
                return response;
            }

            // Link the artwork to the category
            artwork.CategoryId = categoryId;
            _context.Entry(artwork).State = EntityState.Modified;

            try
            {
                // Save the changes to the database
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Updated;
                response.Messages.Add($"Artwork with ID {artworkId} successfully linked to Category with ID {categoryId}.");
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add($"An error occurred while linking the artwork to the category: {ex.Message}");
            }

            return response;
        }




        /// <summary>
        /// Unlinks an artwork from its current category and assigns it to a default category.
        /// </summary>
        /// <param name="artworkId">The ID of the artwork to be unlinked.</param>
        /// <returns>
        /// 200 OK if successfully unlinked and moved.
        /// 404 Not Found if the artwork does not exist.
        /// 400 Bad Request if the default category is missing.
        /// </returns>
        public async Task<ServiceResponse> UnlinkArtworkFromCategory(int artworkId, int categoryId)
        {
            ServiceResponse response = new();

            // Retrieve the category and include its artworks
            var category = await _context.Categories
                .Include(c => c.Artworks)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

            if (category == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Category not found.");
                return response;
            }

            // Find the artwork linked to this category
            var artwork = category.Artworks.FirstOrDefault(a => a.ArtWorkId == artworkId);
            if (artwork == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotLinked;
                response.Messages.Add("Artwork is not linked to this category.");
                return response;
            }

            // Remove the artwork from the category
            category.Artworks.Remove(artwork);
            await _context.SaveChangesAsync();

            // Set status to Updated (artwork was successfully unlinked)
            response.Status = ServiceResponse.ServiceStatus.Updated;
            response.Messages.Add($"Artwork {artworkId} successfully unlinked from Category {categoryId}.");
            return response;
        }

    }
}

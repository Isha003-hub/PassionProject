using MuseAndMasterpiece.Data;
using MuseAndMasterpiece.Interfaces;
using MuseAndMasterpiece.Models;
using Microsoft.EntityFrameworkCore;
using MuseAndMasterpiece.Data;
using System;
using Microsoft.AspNetCore.Mvc;

namespace MuseAndMasterpiece.Services
{
    public class ArtistService : IArtistService
    {
        private readonly ApplicationDbContext _context;

        public ArtistService(ApplicationDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Returns a list of Artists including the Number of Artworks they posted.
        /// </summary>
        /// <returns>
        /// List of Artists including ID, Name, Bio, Total no. of Artworks they posted, Titles of all the Artworks they posted.
        /// </returns>
        public async Task<IEnumerable<ArtistDto>> ListArtists()
        {
            List<Artist> Artists = await _context.Artists
                .Include(a => a.Artworks)
                .ToListAsync();

            List<ArtistDto> ArtistDtos = new List<ArtistDto>();

            foreach (Artist Artist in Artists)
            {
                ArtistDtos.Add(new ArtistDto()
                {
                    ArtistId = Artist.ArtistId,
                    Name = Artist.Name,
                    Bio = Artist.Bio,
                    TotalArtworks = Artist.Artworks?.Count() ?? 0,
                    ArtworksTitle = Artist.Artworks != null ? Artist.Artworks.Select(a => a.Title).ToList() : new List<string>()

                });

            }
            // return ArtistDtos
            return ArtistDtos;
        }


        /// <summary>
        /// Return a Artist specified by it's {id}
        /// </summary>
        /// /// <param name="id">Artist's id</param>
        /// <returns>
        /// ArtistDto : It includes Artist's ID, Name, Total no. of Artworks they posted, Titles of all the Artworks they posted.
        /// or
        /// null when there is no Artist for that {id}
        /// </returns>
        public async Task<ArtistDto> FindArtist(int id)
        {
            var artist = await _context.Artists
                .Include(a => a.Artworks)
                .FirstOrDefaultAsync(a => a.ArtistId == id);

            if (artist == null)
            {
                return null;
            }

            ArtistDto artistDto = new ArtistDto()
            {
                ArtistId = artist.ArtistId,
                Name = artist.Name,
                Bio = artist.Bio,
                TotalArtworks = artist.Artworks?.Count() ?? 0,
                ArtworksTitle = artist.Artworks != null ? artist.Artworks.Select(a => a.Title).ToList() : new List<string>()

            };


            return artistDto;
        }

        /// <summary>
        /// It updates an Artist
        /// </summary>
        /// <param name="id">The ID of Artist which we want to update</param>
        /// <param name="ArtistDto">The required information to update the Artist</param>
        /// <returns>
        /// set status as updated when artist is updated successfully
        /// </returns>       
        public async Task<ServiceResponse> UpdateArtist(int id, UpdateArtistDto updateartistDto)
        {
            ServiceResponse serviceResponse = new();

            var artist = await _context.Artists.FindAsync(id);

            if (artist == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Artist not found.");
                return serviceResponse;
            }


            artist.Name = updateartistDto.Name;
            artist.Bio = updateartistDto.Bio;
            artist.Email = updateartistDto.Email;

            _context.Entry(artist).State = EntityState.Modified;

            try
            {
               
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("An error occurred updating the record.");
                return serviceResponse;
            }

           
            serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            return serviceResponse;
        }


        /// <summary>
        /// Adds an Artist 
        /// </summary>
        /// <remarks>
        /// We add an Artist with the necessary fields in an AddUpdArtistDto
        /// </remarks>
        /// <param name="AddUpdArtistDto">The required information to add the Artist</param
        /// <returns>
        ///  set status as created when artist is added successfully
        /// </returns>
        public async Task<ServiceResponse> AddArtist(AddArtistDto addartistDto)
        {
            ServiceResponse serviceResponse = new();

            Artist artist = new Artist()
            {
                Name = addartistDto.Name,
                Bio = addartistDto.Bio,
                Email = addartistDto.Email
            };

            try
            {
                _context.Artists.Add(artist);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the Artist.");
                serviceResponse.Messages.Add(ex.Message);
                return serviceResponse;
            }

            // Set status as Created
            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = artist.ArtistId;
            return serviceResponse;
        }


        /// <summary>
        /// Delete a Artist specified by it's {id}
        /// </summary>
        /// <param name="id">The id of the Artist we want to delete</param>
        /// <returns>
        /// set status to deleted when artist is deleted successfully
        /// </returns>
        public async Task<ServiceResponse> DeleteArtist(int id)
        {
            ServiceResponse serviceResponse = new();

            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Artist cannot be deleted because it does not exist.");
                return serviceResponse;
            }

            try
            {
                _context.Artists.Remove(artist);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error encountered while deleting the artist.");
                serviceResponse.Messages.Add(ex.Message);
                return serviceResponse;
            }

            // Set status as Deleted
            serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }


    }
}

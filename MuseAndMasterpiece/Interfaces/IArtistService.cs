using MuseAndMasterpiece.Models;
using Microsoft.AspNetCore.Mvc;

namespace MuseAndMasterpiece.Interfaces
{
    public interface IArtistService
    {
        Task<IEnumerable<ArtistDto>> ListArtists();

        Task<ArtistDto> FindArtist(int id);

        Task<ServiceResponse> UpdateArtist(int id, UpdateArtistDto updateartistDto);

        Task<ServiceResponse> AddArtist(AddArtistDto addartistDto);

        Task<ServiceResponse> DeleteArtist(int id);
    }
}

using MuseAndMasterpiece.Models;
using Microsoft.AspNetCore.Mvc;

namespace MuseAndMasterpiece.Interfaces
{
    public interface IArtworkService
    {
        Task<IEnumerable<ArtworkDto>> ListArtworks();
        Task<ArtworkDto> FindArtwork(int id);
        Task<ServiceResponse> UpdateArtwork(int id, UpdateArtworkDto updateartworkDto);
        Task<ServiceResponse> AddArtwork(AddArtworkDto addartworkDto);
        Task<ServiceResponse> DeleteArtwork(int id);
        Task<ServiceResponse> LinkArtworkToCategory(int artworkId, int categoryId);
        Task<ServiceResponse> UnlinkArtworkFromCategory(int artworkId, int categoryId);
    }
}

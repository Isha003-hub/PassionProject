using MuseAndMasterpiece.Models;
using Microsoft.AspNetCore.Mvc;

namespace MuseAndMasterpiece.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> ListCategories();

        Task<CategoryDto> FindCategory(int id);

        Task<ServiceResponse> UpdateCategory(int id, UpdateCategoryDto updatecategoryDto);

        Task<ServiceResponse> AddCategory(AddCategoryDto addcategoryDto);

        Task<ServiceResponse> DeleteCategory(int id);

    }
}

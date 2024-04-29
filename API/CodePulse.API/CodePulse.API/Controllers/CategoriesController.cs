using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace CodePulse.API.Controllers;

// api/categories
[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository categoryRepository;

    public CategoriesController(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    // GET: /api/categories
    [HttpGet]
    public async Task<IActionResult> GetAllCategories(
        [FromQuery] string? query, 
        [FromQuery] string? sortBy, 
        [FromQuery] string? sortDirection,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize
        ) {
        var categories = await categoryRepository.GetAllAsync(query, sortBy, sortDirection, pageNumber, pageSize);

        // map domain model to dto
        var response = new List<CategoryDTO>();
        foreach (var category in categories)
        {
            response.Add(new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle,
            });
        }

        return Ok(response);
    }

    // GET: /api/categories/{id}
    [HttpGet]
    [Route("{id:Guid}")]
    public async Task<IActionResult> GetCategoryById([FromRoute] Guid id) { 
        var existingCategory = await categoryRepository.GetByIdAsync(id);

        if (existingCategory is null) { 
            return NotFound();
        }

        var response = new CategoryDTO
        {
            Id = existingCategory.Id,
            Name = existingCategory.Name,
            UrlHandle = existingCategory.UrlHandle,
        };

        return Ok(response);
    }

    // GET: /api/categories/count
    [HttpGet]
    [Route("count")]
    public async Task<IActionResult> GetCategoriesTotal()
    {
        var count = await categoryRepository.GetCount();

        return Ok(count);
    }

    // POST: /api/categories
    [HttpPost]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> CreateCategory(CreateCategoryRequestDTO request) {
        // map DTO to domain model
        var category = new Category
        {
            Name = request.Name,
            UrlHandle = request.UrlHandle,
        };

        await categoryRepository.CreateAsync(category);

        // domain model to dto
        var response = new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            UrlHandle = category.UrlHandle,
        };

        return Ok(category);
    }


    // PUT: /api/categories/{id}
    [HttpPut]
    [Route("{id:Guid}")]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> EditCategory([FromRoute] Guid id, UpdateCategoryRequestDTO request)
    {
        // convert dto to domain model
        var category = new Category
        {
            Id = id,
            Name = request.Name,
            UrlHandle = request.UrlHandle,
        };

        category = await categoryRepository.UpdateAsync(category);

        if (category is null) { return NotFound(); }

        // convert to dto
        var response = new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            UrlHandle = category.UrlHandle,
        };

        return Ok(response);
    }

    // DELETE: /api/categories/{id}
    [HttpDelete]
    [Route("{id:Guid}")]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id) {
        var category = await categoryRepository.DeleteAsync(id);

        if (category is null) { return NotFound(); };

        // convert domain to dto
        var response = new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            UrlHandle = category.UrlHandle,
        };

        return Ok(response);
    }
}

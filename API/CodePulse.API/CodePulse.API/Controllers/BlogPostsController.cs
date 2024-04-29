using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BlogPostsController : ControllerBase
{
    private readonly IBlogPostRepository blogPostRepository;
    private readonly ICategoryRepository categoryRepository;

    public BlogPostsController(IBlogPostRepository blogPostRepository, ICategoryRepository categoryRepository)
    {
        this.blogPostRepository = blogPostRepository;
        this.categoryRepository = categoryRepository;
    }

    // GET: /api/blogposts
    [HttpGet]
    public async Task<IActionResult> GetAllBlogPosts()
    {
        var blogPosts = await blogPostRepository.GetAllAsync();

        // convert domain to dto
        var response = new List<BlogPostDTO>();
        foreach (var blogpost in blogPosts)
        {
            response.Add(new BlogPostDTO
            {
                Id = blogpost.Id,
                Title = blogpost.Title,
                ShortDescription = blogpost.ShortDescription,
                Content = blogpost.Content,
                FeaturedImageUrl = blogpost.FeaturedImageUrl,
                UrlHandle = blogpost.UrlHandle,
                PublishedDate = blogpost.PublishedDate,
                Author = blogpost.Author,
                IsVisible = blogpost.IsVisible,
                Categories = blogpost.Categories.Select(x => new CategoryDTO { Id = x.Id, Name = x.Name, UrlHandle = x.UrlHandle }).ToList()
            });
        }

        return Ok(response);
    }

    // GET: /api/blogposts/{id}
    [HttpGet]
    [Route("{id:Guid}")]
    public async Task<IActionResult> GetBlogPostById([FromRoute] Guid id) { 
        var blogPost = await blogPostRepository.GetByIdAsync(id);

        if (blogPost is null) {
            return NotFound();
        }

        var response = new BlogPostDTO
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
            ShortDescription = blogPost.ShortDescription,
            Content = blogPost.Content,
            FeaturedImageUrl = blogPost.FeaturedImageUrl,
            UrlHandle = blogPost.UrlHandle,
            PublishedDate = blogPost.PublishedDate,
            Author = blogPost.Author,
            IsVisible = blogPost.IsVisible,
            Categories = blogPost.Categories.Select(x => new CategoryDTO { Id = x.Id, Name = x.Name, UrlHandle = x.UrlHandle }).ToList()
        };

        return Ok(response);
    }

    // GET: /api/blogposts/{urlhandle}
    [HttpGet]
    [Route("{urlHandle}")]
    public async Task<IActionResult> GetBlogPostByUrlHandle([FromRoute] string urlHandle) {
        var blogPost = await blogPostRepository.GetByUrlHandleAsync(urlHandle);
        if (blogPost is null) {  return NotFound(); }

        var response = new BlogPostDTO
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
            ShortDescription = blogPost.ShortDescription,
            Content = blogPost.Content,
            FeaturedImageUrl = blogPost.FeaturedImageUrl,
            UrlHandle = blogPost.UrlHandle,
            PublishedDate = blogPost.PublishedDate,
            Author = blogPost.Author,
            IsVisible = blogPost.IsVisible,
            Categories = blogPost.Categories.Select(x => new CategoryDTO { Id = x.Id, Name = x.Name, UrlHandle = x.UrlHandle }).ToList()
        };

        return Ok(response);
    }

    // POST: /api/blogposts
    [HttpPost]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostRequestDTO request)
    {
        // convert dto to model
        var blogpost = new BlogPost
        {
            Title = request.Title,
            ShortDescription = request.ShortDescription,
            Content = request.Content,
            FeaturedImageUrl = request.FeaturedImageUrl,
            UrlHandle = request.UrlHandle,
            PublishedDate = request.PublishedDate,
            Author = request.Author,
            IsVisible = request.IsVisible,
            Categories = new List<Category>()
        };

        foreach (var categoryId in request.Categories)
        {
            var existingCategory = await categoryRepository.GetByIdAsync(categoryId);

            if (existingCategory is not null) { 
                blogpost.Categories.Add(existingCategory);
            }
        }

        blogpost = await blogPostRepository.CreateAsync(blogpost);

        // convert model to dto
        var response = new BlogPostDTO
        {
            Id = blogpost.Id,
            Title = blogpost.Title,
            ShortDescription = blogpost.ShortDescription,
            Content = blogpost.Content,
            FeaturedImageUrl = blogpost.FeaturedImageUrl,
            UrlHandle = blogpost.UrlHandle,
            PublishedDate = blogpost.PublishedDate,
            Author = blogpost.Author,
            IsVisible = blogpost.IsVisible,
            Categories = blogpost.Categories.Select(x => new CategoryDTO { Id = x.Id, Name = x.Name, UrlHandle = x.UrlHandle }).ToList()
        };

        return Ok(response);
    }

    // PUT: /api/blogposts/{id}
    [HttpPut]
    [Route("{id:Guid}")]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> UpdateBlogPost([FromRoute] Guid id, UpdateBlogPostRequestDTO request)
    {
        // convert dto to domain model
        var blogPost = new BlogPost
        {
            Id = id,
            Title = request.Title,
            ShortDescription = request.ShortDescription,
            Content = request.Content,
            FeaturedImageUrl = request.FeaturedImageUrl,
            UrlHandle = request.UrlHandle,
            PublishedDate = request.PublishedDate,
            Author = request.Author,
            IsVisible = request.IsVisible,
            Categories = new List<Category>()
        };

        // loop over categories
        foreach (var categoryGuid in request.Categories)
        {
            var existingCategory = await categoryRepository.GetByIdAsync(categoryGuid);

            if (existingCategory != null)
            {
                blogPost.Categories.Add(existingCategory);
            }
        }

        // repo to update
        var updatedBlogPost = await blogPostRepository.UpdateAsync(blogPost);

        if (updatedBlogPost == null)
        {
            return NotFound();
        }

        // covert to dto
        var response = new BlogPostDTO
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
            ShortDescription = blogPost.ShortDescription,
            Content = blogPost.Content,
            FeaturedImageUrl = blogPost.FeaturedImageUrl,
            UrlHandle = blogPost.UrlHandle,
            PublishedDate = blogPost.PublishedDate,
            Author = blogPost.Author,
            IsVisible = blogPost.IsVisible,
            Categories = blogPost.Categories.Select(x => new CategoryDTO { Id = x.Id, Name = x.Name, UrlHandle = x.UrlHandle }).ToList()
        };

        return Ok(response);

    }

    // DELETE: /api/blogposts/{id}
    [HttpDelete]
    [Route("{id:Guid}")]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> DeleteBlogPost([FromRoute] Guid id)
    {
        var deletedBlogPost = await blogPostRepository.DeleteAsync(id);
        if (deletedBlogPost == null) { return NotFound(); }

        var response = new BlogPostDTO
        {
            Id = deletedBlogPost.Id,
            Title = deletedBlogPost.Title,
            ShortDescription = deletedBlogPost.ShortDescription,
            Content = deletedBlogPost.Content,
            FeaturedImageUrl = deletedBlogPost.FeaturedImageUrl,
            UrlHandle = deletedBlogPost.UrlHandle,
            PublishedDate = deletedBlogPost.PublishedDate,
            Author = deletedBlogPost.Author,
            IsVisible = deletedBlogPost.IsVisible,
        };

        return Ok(response);
    }
}

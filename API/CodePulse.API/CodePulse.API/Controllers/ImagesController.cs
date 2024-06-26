﻿using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace CodePulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImagesController : ControllerBase
{
    private readonly IImageRepository imageRepository;

    public ImagesController(IImageRepository imageRepository)
    {
        this.imageRepository = imageRepository;
    }

    // GET: /api/images
    [HttpGet]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> GetAllImages() 
    {
        // images repo for all images
        var images = await imageRepository.GetAll();

        // convert to DTO
        var response = new List<BlogImageDTO>();

        foreach (var image in images)
        {
            response.Add(new BlogImageDTO
            {
                Id = image.Id,
                Title = image.Title,
                DateCreated = image.DateCreated,
                FileExtension = image.FileExtension,
                FileName = image.FileName,
                Url = image.Url,

            });
        }

        return Ok(response);
    }


    // POST: /api/images
    [HttpPost]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] string fileName, [FromForm] string title) 
    { 
        ValidateFileUpload(file);

        if (ModelState.IsValid)
        {
            var blogImage = new BlogImage
            {
                FileExtension = Path.GetExtension(file.FileName).ToLower(),
                FileName = fileName,
                Title = title,
                DateCreated = DateTime.Now,
            };

            blogImage = await imageRepository.Upload(file, blogImage);

            // convert to dto
            var response = new BlogImageDTO
            {
                Id = blogImage.Id,
                Title = blogImage.Title,
                DateCreated = blogImage.DateCreated,
                FileExtension = blogImage.FileExtension,
                FileName = blogImage.FileName,
                Url = blogImage.Url,
            };

            return Ok(response);
        }

        return BadRequest(ModelState);
    }
    private void ValidateFileUpload(IFormFile file) {
        var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

        if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower())) {
            ModelState.AddModelError("file", "Unsupported file format");
        }

        if (file.Length > 10485760)
        {
            ModelState.AddModelError("file", "File size cannot be more than 10MB");
        }
    }
}

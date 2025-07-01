using ImageService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageService.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly IFileService _svc;
    public ImageController(IFileService svc) => _svc = svc;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("list")]
    public async Task<IActionResult> List()
        => Ok(await _svc.ListAsync(UserId));

    [HttpPost("upload")]
    [RequestSizeLimit(5_000_000)]  // 5 MB
    public async Task<IActionResult> Upload(IFormFile file)
        => Ok(await _svc.UploadAsync(UserId, file));
}
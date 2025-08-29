using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BlogApi.Services;
using BlogApi.DTOs;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly PostService _postService;

        public PostsController(PostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _postService.GetAllPostsAsync();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);

            if (post == null)
                return NotFound();

            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] PostCreateUpdateDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (userId == 0)
                return Unauthorized();

            var post = await _postService.CreatePostAsync(userId, dto);
            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] PostCreateUpdateDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (userId == 0)
                return Unauthorized();

            var post = await _postService.UpdatePostAsync(id, userId, dto);

            if (post == null)
                return NotFound();

            return Ok(post);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (userId == 0)
                return Unauthorized();

            var result = await _postService.DeletePostAsync(id, userId);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("paged")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPostsPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var (posts, totalCount) = await _postService.GetPostsPagedAsync(page, pageSize, search);

            return Ok(new
            {
                Posts = posts,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }
    }
}
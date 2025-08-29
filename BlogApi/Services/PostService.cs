using AutoMapper;
using BlogApi.Data;
using BlogApi.DTOs;
using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class PostService 
{
	private readonly AppDbContext _context;
	private readonly IMapper _mapper;

	public PostService(AppDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
		
	}

	public async Task<List<PostResponseDto>> GetAllPostsAsync()
	{
		var posts = await _context.Posts
			.Include(p => p.User)
			.OrderByDescending(p => p.CreatedAt)
			.ToListAsync();

		return _mapper.Map<List<PostResponseDto>>(posts);
	}

	public async Task<PostResponseDto?> GetPostByIdAsync(int id)
	{
		var post = await _context.Posts
			.Include(p => p.User)
			.FirstOrDefaultAsync(p => p.Id == id);

		if (post == null) return null;

		return _mapper.Map<PostResponseDto>(post);
	}

	public async Task<PostResponseDto> CreatePostAsync(int userId, PostCreateUpdateDto dto)
	{
		var post = _mapper.Map<Post>(dto);
		post.UserId = userId;
		post.CreatedAt = DateTime.UtcNow;
		post.UpdatedAt = DateTime.UtcNow;

		_context.Posts.Add(post);
		await _context.SaveChangesAsync();

		// Recarregar o post com dados do usuário
		await _context.Entry(post)
			.Reference(p => p.User)
			.LoadAsync();

		return _mapper.Map<PostResponseDto>(post);
	}

	public async Task<PostResponseDto?> UpdatePostAsync(int postId, int userId, PostCreateUpdateDto dto)
	{
		var post = await _context.Posts
			.Include(p => p.User)
			.FirstOrDefaultAsync(p => p.Id == postId);

		if (post == null || post.UserId != userId)
			return null;

		// Atualizar propriedades
		post.Title = dto.Title;
		post.Content = dto.Content;
		post.UpdatedAt = DateTime.UtcNow;

		_context.Posts.Update(post);
		await _context.SaveChangesAsync();

		return _mapper.Map<PostResponseDto>(post);
	}

	public async Task<bool> DeletePostAsync(int postId, int userId)
	{
		var post = await _context.Posts
			.FirstOrDefaultAsync(p => p.Id == postId);

		if (post == null || post.UserId != userId)
			return false;

		_context.Posts.Remove(post);
		await _context.SaveChangesAsync();

		return true;
	}

	public async Task<(List<PostResponseDto> Posts, int TotalCount)>
		GetPostsPagedAsync(int page, int pageSize, string? search)
	{
		var query = _context.Posts
			.Include(p => p.User)
			.AsQueryable();

		if (!string.IsNullOrEmpty(search))
		{
			query = query.Where(p =>
				p.Title.Contains(search) ||
				p.Content.Contains(search));
		}

		var totalCount = await query.CountAsync();

		var posts = await query
			.OrderByDescending(p => p.CreatedAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		var postDtos = _mapper.Map<List<PostResponseDto>>(posts);

		return (postDtos, totalCount);
	}
}
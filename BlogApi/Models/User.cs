using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BlogApi.Models;
public class User 
{
	[key]
	public int Id { get; set; }
    [Required]
	[MaxLength(100)]
	public string Name { get; set; }
	[Required]
    public string Email { get; set; }

	public string PasswordHash { get; set; }



}

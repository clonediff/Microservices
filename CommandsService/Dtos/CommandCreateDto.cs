using System.ComponentModel.DataAnnotations;

namespace CommandsService.Dtos;

public class CommandCreateDto
{
    [Required]
    public string HowTo { get; set; } = default!;
    
    [Required]
    public string CommandLine { get; set; } = default!;
}

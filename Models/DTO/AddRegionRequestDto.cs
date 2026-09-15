using System;
using System.ComponentModel.DataAnnotations;

namespace NZWalks.Models.DTO;

public class AddRegionRequestDto
{
    [Required]
    [MinLength(3, ErrorMessage = " Character must of 3 length")]
    [MaxLength(3, ErrorMessage = "Character must of 3 length")]
    public string Code { get; set; }

    [Required]
    [MaxLength(100, ErrorMessage = "Name has to be a maximum of 100 char")]
    public String Name { get; set; }

    public String? RegionImageUrl { get; set; }  

}

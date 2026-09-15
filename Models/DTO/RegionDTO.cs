using System;

namespace NZWalks.Models.DTO;

public class RegionDTO
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public String Name { get; set; }
    public String? RegionImageUrl { get; set; }

}

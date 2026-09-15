namespace NZWalks.Models.DTO
{
    public class WalkDTO
    {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double LengthInKm  { get; set; }

    public string? WalkImageUrl { get; set; }

    // **** Since we have added navigation thing below we can remove this now.******//

    // public Guid DifficultyId { get; set; }

    // public Guid RegionId { get; set; }  

    // WE are using navigation so we are doing this for that.
    public RegionDTO Region {get; set;}

    public DifficultyDTO Difficulty {get; set;}
    
    }
}
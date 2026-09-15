using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Region;
using NZWalks.Models;
using NZWalks.Models.Domain;

namespace NZWalks.Data;

public class NZClassDbContext : DbContext
{
    public NZClassDbContext(DbContextOptions<NZClassDbContext> dbContextOptions): base(dbContextOptions) // the <NZClassDbContext> thsi added here to differentiate between two DbContext class (if not then it will throw error)
    {
        
    }

    public DbSet<Difficulty> Difficulties { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<Walk> Walks { get; set; }

    public DbSet<Image> Images { get; set; } 

    // Seeding the data

    protected override void OnModelCreating(ModelBuilder modelBuilder) // for seeding the data we have to add this
    {
        base.OnModelCreating(modelBuilder);

        // Seed the data for difficulties
        // Easy, Medium, Hard

        var difficulties = new List<Difficulty>()
        {
            new Difficulty()
            {
                Id = Guid.Parse("091cf56f-20d8-4ac5-bb6b-fdfa27f8298a"),
                Name = "Easy"
            },
            new Difficulty()
            {
                Id = Guid.Parse("902c9903-2943-4ae9-a72d-509f3701e986"),
                Name = "Medium"
            },
            new Difficulty()
            {
                Id = Guid.Parse("1e40a925-2839-41d8-9d17-197516d7a896"),
                Name = "Hard"
            },
        };

        // Seed difficulties to the database

        modelBuilder.Entity<Difficulty>().HasData(difficulties);

        // seed data for Regons

        var regions = new List<Region>()
        {
            new Region()
            {
                Id = Guid.Parse("1a4b16bc-1583-4926-b6a6-f67ff77ae121"),
                Name = "Aukland",
                Code = "AKL",
                RegionImageUrl = "https://www.bing.com/images/search?view=detailV2&ccid=k2GBXexR&id=ED09A1F6B9ED70C2D41776F50A6A3FB70006D3D1&thid=OIP.k2GBXexRk18uOvYBqS5TDwHaDm&mediaurl=https%3a%2f%2fwallpaperaccess.com%2ffull%2f1606832.jpg&exph=1778&expw=3663&q=aukland+image&FORM=IRPRST&ck=991C5B7C7B46F85174A8C2AC582F6543&selectedIndex=2&itb=0"
            },
             new Region
                {
                    Id = Guid.Parse("6884f7d7-ad1f-4101-8df3-7a6fa7387d81"),
                    Name = "Northland",
                    Code = "NTL",
                    RegionImageUrl = null
                },
                new Region
                {
                    Id = Guid.Parse("14ceba71-4b51-4777-9b17-46602cf66153"),
                    Name = "Bay Of Plenty",
                    Code = "BOP",
                    RegionImageUrl = null
                },
                new Region 
                {
                    Id = Guid.Parse("cfa06ed2-bf65-4b65-93ed-c9d286ddb0de"),
                    Name = "Wellington",
                    Code = "WGN",
                    RegionImageUrl = "https://images.pexels.com/photos/4350631/pexels-photo-4350631.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"
                },
                new Region                         
                {
                    Id = Guid.Parse("906cb139-415a-4bbb-a174-1a1faf9fb1f6"),
                    Name = "Nelson",
                    Code = "NSN",
                    RegionImageUrl = "https://images.pexels.com/photos/13918194/pexels-photo-13918194.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"
                },
                new Region
                {
                    Id = Guid.Parse("f077a22e-4248-4bf6-b564-c7cf4e250263"),
                    Name = "Southland",
                    Code = "STL",
                    RegionImageUrl = null
                }
        };

            // Seed Region to the database

            modelBuilder.Entity<Region>().HasData(regions);
    }

}

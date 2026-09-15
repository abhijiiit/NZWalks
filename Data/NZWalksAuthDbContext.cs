using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;

namespace NZWalks;

public class NZWalksAuthDbContext : IdentityDbContext
{
    public NZWalksAuthDbContext(DbContextOptions<NZWalksAuthDbContext> options) : base(options) // the <NZWalksAuthDbContext> thsi is added to diffrentiate the DbContext class.
    {
        
    }

    // seeding roles data 

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // seed the data

        var readerRoleId = "d2af1a9b-a33d-49a1-a4ca-a8980d94dd97";
        var writerRoleId = "0340cc28-b899-4816-ac7a-285b82bf88cc";

        var roles = new List<IdentityRole>
        {
           new IdentityRole
           {
               Id = readerRoleId,
               ConcurrencyStamp = readerRoleId,
               Name = "Reader",
               NormalizedName = "Reader".ToUpper()
           },

           new IdentityRole
           {
               Id = writerRoleId,
               ConcurrencyStamp = writerRoleId,
               Name = "Writer",
               NormalizedName = "Writer".ToUpper()
           } 
        };

        // seed data to database

        builder.Entity<IdentityRole>().HasData(roles);
    }
}

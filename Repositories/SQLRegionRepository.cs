using System;
using Microsoft.EntityFrameworkCore;
using NZWalks.Data;
using NZWalks.Models.Domain;

namespace NZWalks.Repositories;

public class SQLRegionRepository : IRegionRepository
{
    private readonly NZClassDbContext dbContext;

    public SQLRegionRepository(NZClassDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Region> CreateAsync(Region region)
    {
        await dbContext.Regions.AddAsync(region);
        await dbContext.SaveChangesAsync();
        return region;
    }

    public async Task<Region?> DeleteAysnc(Guid id)
    {
        var existRegion=await dbContext.Regions.FirstOrDefaultAsync(x=>x.Id==id);

        if (existRegion == null)
        {
            return null;                                          
        }         
        
        dbContext.Regions.Remove(existRegion);  /// remove does not have RemoveAsync() method so we dont use await
        await dbContext.SaveChangesAsync(); 
        return existRegion;                                         
    }                                       

    public async Task<List<Region>> GetAllAsync()                                   
    {
        return await dbContext.Regions.ToListAsync();         
    }                                          

    public async Task<Region?> GetByIdAsync(Guid id)     
    {
        return await dbContext.Regions.FirstOrDefaultAsync(x=>x.Id==id);
    }

    public async Task<Region> UpdateAsync(Guid id, Region region)
    {
        var existRegion= await dbContext.Regions.FirstOrDefaultAsync(x=>x.Id==id);

        if (existRegion == null)
        {
            return null;                                                                                                                                                                                                                                                                                                                                               
        }  

        existRegion.Name=region.Name;                                                 
        existRegion.Code=region.Code;                                                 
        existRegion.RegionImageUrl=region.RegionImageUrl;

        await dbContext.SaveChangesAsync();     

        return existRegion;
    }
}

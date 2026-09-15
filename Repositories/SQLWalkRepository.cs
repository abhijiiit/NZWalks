using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using NZWalks.Data;
using NZWalks.Models.Domain;

namespace NZWalks.Repositories
{
    public class SQLWalkRepository : IwalkRepository
    {
        private readonly NZClassDbContext dbContext;

        public SQLWalkRepository(NZClassDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Walk> CreateAysnc(Walk walk)
        {
            await dbContext.Walks.AddAsync(walk);
            await dbContext.SaveChangesAsync();
            return walk;
        }

        public async Task<Walk?> DeleteAsync(Guid id)
        {
            var existWalk = await dbContext.Walks.FirstOrDefaultAsync(x=>x.Id==id);

            if (existWalk == null)
            {
                return null;
            }

            dbContext.Walks.Remove(existWalk); // remove doest not have async mthod so we dont use await

            await dbContext.SaveChangesAsync();
            return existWalk;
        }
                 
        public async Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null, 
        string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000) // in parameter it is = null parameter is optional and default vaule is null
        {
            var walks = dbContext.Walks.Include("Difficulty").Include("Region").AsQueryable();

            // Filtering
            if(string.IsNullOrWhiteSpace(filterOn)==false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if(filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase)) // 
                {
                    walks = walks.Where(x => x.Name.Contains(filterQuery));
                }
            }

            // Sorting                      
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if(sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    // if(isAscending==true)
                    // {
                    //     walks = walks.OrderBy(x => x.Name);
                    // }
                    // else
                    // {
                    //     walks = walks.OrderByDescending(x => x.Name);
                    // }
  
                    //************* Same above thing in one line using ternary operator*****

                    walks = isAscending ? walks.OrderBy(x=> x.Name) : walks.OrderByDescending(x => x.Name);
                }
                if(sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase)) // Ordinal we use bcoz it ignore the unneccesary case (LENGTH, LeNGth, length all are valid only) without this we have to write Length Exact
                {
                    // if(isAscending==true)
                    // {
                    //     walks = walks.OrderBy(x=>x.LengthInKm);
                    // }
                    // else
                    // {
                    //     walks = walks.OrderByDescending(x => x.LengthInKm);
                    // }

                    //************* Same above thing in one line using ternary operator*****

                    walks = isAscending ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm);
                }
            }

            // Pagination

            int skipResults = (pageNumber - 1) * pageSize;

            return await walks.Skip(skipResults).Take(pageSize).ToListAsync(); 
            //***** EXAMPLE **********//

            // pgno = 1                                      // pgno = 2
            //pgsize = 1                                     // pgsize = 1
            //skipres = (1-1)*1 = 0                          // skipres = (2-1)*1 = 1
            // so Skip 0 result and take 1(pgsize) next result    // skip 1 result take 1(pgsize) next result
            // return await dbContext.Walks.Include("Difficulty").Include("Region").ToListAsync(); // we commented this bcoz we will be implimenting filtering,pagination,sorting
        }

        public async Task<Walk?> GetByIdAsync(Guid id)
        {
            return await dbContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x=>x.Id==id);

        }

        public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
        {
            var existWalk = await dbContext.Walks.FirstOrDefaultAsync(x=>x.Id==id);

            if (existWalk == null)
            {
                return null;     // here we are not returning NotFound() bcoz this is not the IActionResult one (this actionn accept HTTP response)
            }

            existWalk.Name=walk.Name;
            existWalk.Description=walk.Description;
            existWalk.LengthInKm=walk.LengthInKm;
            existWalk.WalkImageUrl=walk.WalkImageUrl;
            existWalk.RegionId=walk.RegionId;
            existWalk.DifficultyId=walk.DifficultyId;


            await dbContext.SaveChangesAsync();

            return existWalk;  // here we returning existWalk which will have Id also, if we have just returned walk then it will not have Id bcoz walk is updateWalkRequestDto which doest not have Id(so this is the reason we have to update id when we are mapping domain back to dto in controller)
        }
    }
}
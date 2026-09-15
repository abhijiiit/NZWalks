using System;
using System.Runtime.InteropServices;
using NZWalks.Models.Domain;

namespace NZWalks.Repositories;

public interface IRegionRepository
{
        Task<List<Region>> GetAllAsync();

        Task<Region?>GetByIdAsync(Guid id);

        Task<Region>CreateAsync(Region region);

        Task<Region?>UpdateAsync(Guid id, Region region);

        Task<Region?>DeleteAysnc(Guid id); 

}

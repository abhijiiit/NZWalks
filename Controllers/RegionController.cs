using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using NZWalks.CustomActionFilters;
using NZWalks.Data;
using NZWalks.Models.Domain;
using NZWalks.Models.DTO;
using NZWalks.Repositories;                                                                                                              

namespace NZWalks.Controllers
{
    [Route("api/[controller]")]                                                                                                                                                                                                                                                                                                                                                                                                       
    [ApiController]                                                                                     
      
    public class RegionController : ControllerBase                                                                        
    {
        private readonly NZClassDbContext dbContext;            
        private readonly IRegionRepository regionRepository;   
        private readonly IMapper mapper;
        private readonly ILogger<RegionController> logger;

        public RegionController(NZClassDbContext dbContext, IRegionRepository regionRepository, IMapper mapper, ILogger<RegionController> logger)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository; 
            this.mapper = mapper;
            this.logger = logger;      
        }  
        //Get all regions 
        // http://localhost:5038/api/region
        [HttpGet]
       // [Authorize(Roles ="Reader")]
        public async Task<IActionResult> GetAll()
        {
           // try{                               

                // throw new Exception("This is a custum Exception");

            // Get data from database - Domain model
            //var regionsDomain = await dbContext.Regions.ToListAsync(); // before using repository directly accseing the data from database
           
           var regionsDomain = await regionRepository.GetAllAsync();  // good practice no use of db context with the help IRegionRepository


           // logger.LogInformation($"finished GetAllRegions request with data: {JsonSerializer.Serialize(regionsDomain)}");

            //***** Mapping ****** 1st way ****//
            //Map domain model to DTOs.

            // var regionsDto=new List<RegionDTO>();
            
            // foreach(var regionDomain in regionsDomain)
            // {
            //     regionsDto.Add(new RegionDTO()
            //     {
            //         Id=regionDomain.Id,
            //         Code=regionDomain.Code,
            //         Name=regionDomain.Name,
            //         RegionImageUrl=regionDomain.RegionImageUrl
            //     });
            // }

            // ****** Mapping **** 2nd Way *****//
            var regionsDto = mapper.Map<List<RegionDTO>>(regionsDomain);

            // Return DTOs to client
            return Ok(regionsDto);
            
            // *** we can also use ***//
            // return Ok(mapper.Map<List<RegionDTO>>(regionsDomail)) // this will minimize the lines of code we will have only 2 lines
        }
        //catch (Exception ex)
        //     {
        //         logger.LogError(ex, ex.Message); 
        //         throw;            
        //     }
        // }  

        // get single region by id
        // http://localhost:5038/api/region/{id}
        [HttpGet]
        [Route("{id:guid}")]
        [Authorize(Roles ="Reader")] 
        public async Task<IActionResult> GetById([FromRoute] Guid id)

        {
            //var regions=dbContext.Regions.Find(id);  // we can use this to find only id(primary key)
            
           // var regionsDomain=await dbContext.Regions.FirstOrDefaultAsync(x=>x.Id==id); // using this we cand all the properties of region.
                                                                                         // directlt using the dbcontext

            var regionsDomain = await regionRepository.GetByIdAsync(id);
             
             if (regionsDomain == null)
            {
                return NotFound();
            }

            //  // after getting domain -- Map it to DTO 
            // 1st Way ***

            //  var regionDto=new RegionDTO
            //  {
            //         Id=regionsDomain.Id,         
            //         Code=regionsDomain.Code,    
            //         Name=regionsDomain.Name,
            //         RegionImageUrl=regionsDomain.RegionImageUrl
            //  }; 

            // ***Mapping 2nd Way using automapper **//

            var regionDto = mapper.Map<RegionDTO>(regionsDomain);
            


            //return DTO to client 

          return Ok(regionDto);   // At the place of regionDto we can directly write the mapper here
        }
        
        // We use post to create 
        // http://localhost:5038/api/region 
        [HttpPost]
        [ValidateModel]  // by using this which is created by us only we can stop using if(ModelState.IsValid) this thing 
        [Authorize(Roles ="Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)// here we use frombody bcoz body will be provided by the client(what we have add).
        {

                // Map DTO to Domain model
            // **** 1st way

            // var regionDomainModel=new Region                
            // {
            //     Code=addRegionRequestDto.Code,       
            //     Name=addRegionRequestDto.Name,
            //     RegionImageUrl=addRegionRequestDto.RegionImageUrl
            // };

            // ******Mapping 2nd Way *******//

            var regionDomainModel = mapper.Map<Region>(addRegionRequestDto); 

            // use domain model to create region in database
            // await dbContext.Regions.AddAsync(regionDomainModel);
            // await dbContext.SaveChangesAsync(); // save changes and refleft in database //// without using repository

            regionDomainModel = await regionRepository.CreateAsync(regionDomainModel); // after using the repository
            
            // map domain model back to dto
            // ***** 1st Way

            // var regionDto=new RegionDTO
            // {
            //     Id=regionDomainModel.Id,                      
            //     Code=regionDomainModel.Code,   
            //     Name=regionDomainModel.Name,
            //     RegionImageUrl=regionDomainModel.RegionImageUrl
            // };


            // Mapping **** 2nd Way *****//

            var regionDto = mapper.Map<RegionDTO>(regionDomainModel);


            return CreatedAtAction(nameof(GetById), new {id=regionDto.Id}, regionDto);
                
        }
         
         //Update region
        //http://localhost:5038/api/region/{id}    
        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]  // by me for my custom validation
        [Authorize(Roles ="Writer")]          
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
                // Check if region exists or not first
            //var regionDomainModel=await dbContext.Regions.FirstOrDefaultAsync(x=>x.Id==id); // before using repository

            // for using repository we will here have to map DTO to domain
            // ****** Mapping 1st way

            // var regionDomainModel= new Region
            // {
            //     Code=updateRegionRequestDto.Code,
            //     Name=updateRegionRequestDto.Name,
            //     RegionImageUrl=updateRegionRequestDto.RegionImageUrl
            // };

            // Mapping 2nd way

            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);


            regionDomainModel=await regionRepository.UpdateAsync(id,regionDomainModel);

            if (regionDomainModel == null)
            {
                return NotFound(); 
            } 



            //     // Map dto to domain model
            // regionDomainModel.Code=updateRegionRequestDto.Code;
            // regionDomainModel.Name=updateRegionRequestDto.Name;                           // THIS WE IMPLEMENTED IN REPOSITORY SO NO NEED HERE 
            // regionDomainModel.RegionImageUrl=updateRegionRequestDto.RegionImageUrl;

            // // directly we save .
            // await dbContext.SaveChangesAsync();





            // map domain model back to dto
            // Mapping 1st Way
            // var regionDto=new RegionDTO
            // {
            //     Id=regionDomainModel.Id,
            //     Code=regionDomainModel.Code,
            //     Name=regionDomainModel.Name,
            //     RegionImageUrl=regionDomainModel.RegionImageUrl
            // };

            // Mapping 2nd Way

            var regionDto = mapper.Map<RegionDTO>(regionDomainModel);

            return Ok(regionDto);
                
            

        } 

        // Detele Region
        // http://localhost:5038/api/region/{id}
        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Writer,Reader")]    
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            //var regionDomainModel=await dbContext.Regions.FirstOrDefaultAsync(x=>x.Id==id);// without using repository

            var regionDomainModel=await regionRepository.DeleteAysnc(id);

            if (regionDomainModel == null)
            {
                return NotFound();                                                                            
            }



            // dbContext.Regions.Remove(regionDomainModel);
            // await dbContext.SaveChangesAsync();                    // REMOVED AFTER WE USE REPOSITORY



            // map Model to DTO
            // Mapping 1st way
            // var return1=new RegionDTO
            // {
            //     Id=regionDomainModel.Id,
            //     Code=regionDomainModel.Code,
            //     Name=regionDomainModel.Name,
            //     RegionImageUrl=regionDomainModel.RegionImageUrl
            // };

            // mapping 2nd way

            var return1 = mapper.Map<RegionDTO>(regionDomainModel);


            return Ok(return1);
        }
}
}
        
       
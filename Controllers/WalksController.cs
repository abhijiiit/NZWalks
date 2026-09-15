using AutoMapper;
// using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.CustomActionFilters;
using NZWalks.Models.Domain;
using NZWalks.Models.DTO;
using NZWalks.Repositories;

namespace NZWalks.Controllerss
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IwalkRepository iwalkRepository;

        public WalksController(IMapper mapper, IwalkRepository iwalkRepository)
        {
            this.mapper = mapper;
            this.iwalkRepository = iwalkRepository; 
        }
        // Create Walk
        // Post method
        [HttpPost]
        [ValidateModel]  
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
                // Map addWalkRequestDto DTO to Walk Domain model
            var walkDomainModel = mapper.Map<Walk>(addWalkRequestDto);

            await iwalkRepository.CreateAysnc(walkDomainModel);  // saved in database

            // Map Domain Model to Walk DTO

            var walkDto = mapper.Map<WalkDTO>(walkDomainModel);

            return Ok(walkDto);
            

        }
                                                  

        // Get all for walk
        // Get method- /api/walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAscending=true  
        [HttpGet]                                                                                                                        

        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery, 
        [FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)

        {
            
            var walksDomainModel = await iwalkRepository.GetAllAsync(filterOn,filterQuery,sortBy,isAscending ?? true, pageNumber, pageSize); // we use ?? true ---> beacuse in GetAllAsync methods parameter for isAscending is expecting some value so here we cannot just pass the null value 

            // Create a new exception

            //throw new Exception("This is a new exception");

            // Map domain to DTO

            var walksDto = mapper.Map<List<WalkDTO>>(walksDomainModel);

            return Ok(walksDto);                        
        }
    

        

        // Get walk by id
        // GET: /api/walks/{id}
        
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walkDomainModel = await iwalkRepository.GetByIdAsync(id);

            if (walkDomainModel == null)
            {
                return NotFound();
            }

            // Map Model to Dto

            var walkDto = mapper.Map<WalkDTO>(walkDomainModel);

            return Ok(walkDto);
        }

        // Update walk by id
        // PUT: /api/walks/{id}

        [HttpPut]    
        [Route("{id:Guid}")]                         
        [ValidateModel]                                                                                                                                      

        public async Task<IActionResult> Update ([FromRoute] Guid id, [FromBody] UpdateWalkRequestDto updateWalkRequestDto)
        {
            
            // Map dto to domain first
            var walkDomainModel = mapper.Map<Walk>(updateWalkRequestDto);  // for update purpose first we have to map the data which is goinng to be updated
                                                                            // Since we are using Walk model so Id will also be there but in Empty form here
           
            walkDomainModel = await iwalkRepository.UpdateAsync(id,walkDomainModel); // here repository returns existing walk with id also so our empty id will be filled here and we will have Id
                                                                                    // Same domain is updated here so we are storing it in same walkDomainModel

            if (walkDomainModel == null)
            {
                return NotFound();
            }

            // Map domain to dto.

            var walkDto = mapper.Map<WalkDTO>(walkDomainModel);   // we get result including id here
            return Ok(walkDto);
           
        }
    
        // Detelet by id
        // DELETE: /api/walks/{id}
        
        [HttpDelete]
        [Route("{id:guid}")] 

        public async Task<IActionResult> Delete ([FromRoute] Guid id)
        {
            var walkDomainModel = await iwalkRepository.DeleteAsync(id);

            if(walkDomainModel == null)
            {
                return NotFound();
            }

            // map the domain into dto

            var walkDto = mapper.Map<WalkDTO>(walkDomainModel);
            return Ok(walkDto);
        }
    }
}

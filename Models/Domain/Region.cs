using System;

namespace NZWalks.Models.Domain;

public class Region
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public String Name { get; set; }  
    public String? RegionImageUrl { get; set; }   /// we can pass null value for this property because not all regions will have an image url, so we make it nullable by adding ? after the type
}     
         
             
                                                                                    
                                                                
using NZWalks.Data;

namespace NZWalks.Repositories;

public class LocalImageRepository : IImageRepository
{
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly NZClassDbContext dbContext;

    public LocalImageRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, NZClassDbContext dbContext)
    {
        this.webHostEnvironment = webHostEnvironment;
        this.httpContextAccessor = httpContextAccessor;
        this.dbContext = dbContext;
    }
    public async Task<Image> Upload(Image image)
    {
        var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Images", $"{image.FileName}{image.FileExtention}");

        //Upload Image to Local Path
        using var stream = new FileStream(localFilePath, FileMode.Create); // Reads the file stream for localFilePath location & it knows we have to create File we have used in parameter
        await image.File.CopyToAsync(stream); // Copying // by this line we should have an image inside image folder

        /// https://localhost:1234/images/images.jpg
        
        var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtention}";

        image.FilePath = urlFilePath;

        // add the images in database

        await dbContext.Images.AddAsync(image);

        // Save image

        await dbContext.SaveChangesAsync();

        return image;                           

    }
}

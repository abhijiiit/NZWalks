using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NZWalks.Controllers
{
    //http://localhost:5038/api/students
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllStudents()
        {
            var students = new List<string>
            {
                "John Doe",
                "Jane Smith",
                "Michael Johnson"
            };

            return Ok(students);
        }
    }
}

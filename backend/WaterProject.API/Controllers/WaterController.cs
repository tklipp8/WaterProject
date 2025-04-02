using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WaterProject.API.Data;

namespace WaterProject.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WaterController : ControllerBase
    {
        private WaterDbContext _waterContext;
        public WaterController(WaterDbContext temp) => _waterContext = temp;

        [HttpGet("AllProjects")]
        public IActionResult GetProjects(int pageSize = 10, int pageNum = 1, [FromQuery] List<string>? projectTypes = null)
        {
            Console.WriteLine($"GetProjects called with pageSize={pageSize}, pageNum={pageNum}, projectTypes={string.Join(",", projectTypes ?? new List<string>())}");
            
            string? favProjType = Request.Cookies["FavoriteProjectType"];
            Console.WriteLine("~~~~~~~~~~COOKIE~~~~~~~~~~\n" + favProjType);

            HttpContext.Response.Cookies.Append("FavoriteProjectType", "Protected Spring", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddMinutes(1),
            });

            var query = _waterContext.Projects.AsQueryable();
            
            // Log the total count before filtering
            var totalBeforeFilter = query.Count();
            Console.WriteLine($"Total projects before filtering: {totalBeforeFilter}");
            
            if (projectTypes != null && projectTypes.Count > 0)
            {
                Console.WriteLine($"Filtering by project types: {string.Join(", ", projectTypes)}");
                query = query.Where(p => projectTypes.Contains(p.ProjectType));
            }

            var totalNumProjects = query.Count();
            Console.WriteLine($"Total projects after filtering: {totalNumProjects}");

            // Ensure pageSize is at least 1
            pageSize = Math.Max(1, pageSize);
            
            var projects = query
                .Skip((pageNum-1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            Console.WriteLine($"Returning {projects.Count} projects for page {pageNum} with page size {pageSize}");

            var result = new
            {
                Projects = projects,
                TotalNumProjects = totalNumProjects
            };

            return Ok(result);
        }

        [HttpGet("GetProjectTypes")]
        public IActionResult GetProjectTypes()
        {
            var projectTypes = _waterContext.Projects
                .Select(p => p.ProjectType)
                .Distinct()
                .ToList();
            return Ok(projectTypes);
        }

        [HttpPost("AddProject")]
        public IActionResult AddProject([FromBody] Project newProject)
        {
            _waterContext.Projects.Add(newProject);
            _waterContext.SaveChanges();
            return Ok(newProject);
        }

        [HttpPut("Update/{projectId}")]
        public IActionResult UpdateProject(int projectId, [FromBody] Project updatedProject)
        {
            var existingProject = _waterContext.Projects.Find(projectId);
            if (existingProject == null)
            {
                return NotFound($"Project with ID {projectId} not found.");
            }

            // Update the existing project with new values
            existingProject.ProjectName = updatedProject.ProjectName;
            existingProject.ProjectType = updatedProject.ProjectType;
            existingProject.ProjectRegionalProgram = updatedProject.ProjectRegionalProgram;
            existingProject.ProjectImpact = updatedProject.ProjectImpact;
            existingProject.ProjectPhase = updatedProject.ProjectPhase;
            existingProject.ProjectFunctionalityStatus = updatedProject.ProjectFunctionalityStatus;

            _waterContext.Update(existingProject);
            _waterContext.SaveChanges();
            return Ok(existingProject);
        }

        [HttpDelete("DeleteProject/{projectId}")]
        public IActionResult DeleteProject(int projectId)
        {
            var project = _waterContext.Projects.Find(projectId);
            if (project == null)
            {
                return NotFound($"Project with ID {projectId} not found.");
            }

            _waterContext.Projects.Remove(project);
            _waterContext.SaveChanges();

            return NoContent();
        }

        [HttpGet("Test")]
        public IActionResult Test()
        {
            return Ok(new { message = "API is working", timestamp = DateTime.Now });
        }
    }
}

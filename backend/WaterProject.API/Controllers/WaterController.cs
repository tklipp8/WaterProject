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
        public IActionResult GetProjects(int pageSize, int pageNum = 1, [FromQuery] List<string>? projectTypes = null)
        {
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

            if (projectTypes != null)
            {
                query = query.Where(p => projectTypes.Contains(p.ProjectType));
            }

            var totalNumProjects = query.Count();

            var something = query
                .Skip((pageNum-1) * pageSize)
                .Take(pageSize)
                .ToList();



            var someObject = new
            {
                Projects = something,
                TotalNumProjects = totalNumProjects
            };

            return Ok(someObject);
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
    }
}

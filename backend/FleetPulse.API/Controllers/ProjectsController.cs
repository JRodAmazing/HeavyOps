using Microsoft.AspNetCore.Mvc;
using FleetPulse.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FleetPulse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private static List<Project> projects = new()
        {
            new Project
            {
                Id = "proj_roadwork_2025",
                Name = "Highway Expansion - Route 45",
                Location = "North Dallas, TX",
                Description = "4-mile highway widening project",
                ProjectManager = "John Davis",
                Budget = 500000,
                TotalSpent = 125000,
                Status = "active",
                StartDate = new DateTime(2025, 10, 1),
                EndDate = new DateTime(2026, 6, 30)
            },
            new Project
            {
                Id = "proj_commercial_2025",
                Name = "Commercial Site Prep",
                Location = "Frisco, TX",
                Description = "Land clearing and foundation prep",
                ProjectManager = "Sarah Chen",
                Budget = 250000,
                TotalSpent = 75000,
                Status = "active",
                StartDate = new DateTime(2025, 9, 15),
                EndDate = new DateTime(2026, 2, 28)
            }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Project>> GetAllProjects()
        {
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public ActionResult<Project> GetProjectById(string id)
        {
            var project = projects.FirstOrDefault(p => p.Id == id);
            if (project == null)
                return NotFound(new { message = $"Project {id} not found" });
            return Ok(project);
        }

        [HttpPost]
        public ActionResult<Project> CreateProject([FromBody] Project newProject)
        {
            if (string.IsNullOrEmpty(newProject.Name))
                return BadRequest(new { message = "Project name is required" });

            newProject.Id = Guid.NewGuid().ToString();
            newProject.CreatedAt = DateTime.UtcNow;
            newProject.UpdatedAt = DateTime.UtcNow;
            projects.Add(newProject);

            return CreatedAtAction(nameof(GetProjectById), new { id = newProject.Id }, newProject);
        }

        [HttpPut("{id}")]
        public ActionResult<Project> UpdateProject(string id, [FromBody] Project updatedProject)
        {
            var project = projects.FirstOrDefault(p => p.Id == id);
            if (project == null)
                return NotFound(new { message = $"Project {id} not found" });

            project.Name = updatedProject.Name ?? project.Name;
            project.Location = updatedProject.Location ?? project.Location;
            project.Description = updatedProject.Description ?? project.Description;
            project.Budget = updatedProject.Budget > 0 ? updatedProject.Budget : project.Budget;
            project.Status = updatedProject.Status ?? project.Status;
            project.UpdatedAt = DateTime.UtcNow;

            return Ok(project);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteProject(string id)
        {
            var project = projects.FirstOrDefault(p => p.Id == id);
            if (project == null)
                return NotFound(new { message = $"Project {id} not found" });

            projects.Remove(project);
            return Ok(new { message = $"Project {id} deleted" });
        }
    }
}

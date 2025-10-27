using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FleetPulse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IntegrationsController : ControllerBase
    {
        private static List<object> hcssJobs = new()
        {
            new
            {
                jobId = "HCSS_001",
                jobName = "Highway Expansion - Route 45",
                bidAmount = 1250000,
                estimatedCost = 875000,
                jobStatus = "in_progress",
                location = "North Dallas, TX",
                startDate = new DateTime(2025, 10, 1),
                syncedAt = DateTime.UtcNow
            }
        };

        private static List<object> procoreProjects = new()
        {
            new
            {
                projectId = "PROCORE_001",
                projectName = "Highway Expansion - Route 45",
                owner = "TxDOT",
                budget = 5000000,
                spent = 1250000,
                status = "Active"
            }
        };

        private static List<object> quickbooksTransactions = new()
        {
            new
            {
                transactionId = "QB_TXN_001",
                date = new DateTime(2025, 10, 15),
                description = "Equipment Rental - CAT320",
                amount = 85000,
                category = "Equipment"
            }
        };

        [HttpGet("hcss/sync")]
        public ActionResult<object> SyncHCSSJobs()
        {
            return Ok(new
            {
                status = "success",
                message = "HCSS jobs synced successfully",
                jobsCount = hcssJobs.Count,
                jobs = hcssJobs,
                syncedAt = DateTime.UtcNow
            });
        }

        [HttpGet("procore/sync")]
        public ActionResult<object> SyncProcoreProjects()
        {
            return Ok(new
            {
                status = "success",
                message = "Procore projects synced successfully",
                projectsCount = procoreProjects.Count,
                projects = procoreProjects,
                syncedAt = DateTime.UtcNow
            });
        }

        [HttpGet("quickbooks/sync")]
        public ActionResult<object> SyncQuickBooksTransactions()
        {
            var totalAmount = quickbooksTransactions.Sum(t => ((dynamic)t).amount);

            return Ok(new
            {
                status = "success",
                message = "QuickBooks transactions synced successfully",
                transactionCount = quickbooksTransactions.Count,
                totalAmount = totalAmount,
                transactions = quickbooksTransactions,
                syncedAt = DateTime.UtcNow
            });
        }

        [HttpGet("status")]
        public ActionResult<object> GetIntegrationStatus()
        {
            return Ok(new
            {
                integrations = new
                {
                    hcss = new { status = "connected", lastSync = DateTime.UtcNow },
                    procore = new { status = "connected", lastSync = DateTime.UtcNow },
                    quickbooks = new { status = "connected", lastSync = DateTime.UtcNow }
                },
                overallStatus = "all_connected"
            });
        }
    }
}
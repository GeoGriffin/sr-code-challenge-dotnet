using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using challenge.Services;
using challenge.Models;

namespace code_challenge.Controllers
{
    [Route("api/compensation")]
    public class CompensationController : Controller
    {
        private readonly ILogger _logger;
        private readonly ICompensationService _compensationService;

        public CompensationController(ILogger<CompensationController> logger, ICompensationService compensationService)
        {
            _logger = logger;
            _compensationService = compensationService;
        }

        /// <summary>
        /// List all compensation records
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllCompensations()
        {
            _logger.LogDebug($"Received compensation get request for all employees");

            var compensations = _compensationService.GetAllCompensations();

            if (compensations == null)
                return NotFound();

            return Ok(compensations);
        }

        /// <summary>
        /// Create new compensation record
        /// </summary>
        /// <param name="compensation">The complete compensation record to create</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateCompensation([FromBody] Compensation compensation)
        {
            _logger.LogDebug($"Received compensation create request for '{compensation.EmployeeId}'");

            _compensationService.Create(compensation);

            return CreatedAtRoute("getCompensationById", new { id = compensation.EmployeeId }, compensation);
        }

        /// <summary>
        /// Get a single compensation record
        /// </summary>
        /// <param name="id">The id of the compensation being listed</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "getCompensationById")]
        public IActionResult GetCompensationById(String id)
        {
            _logger.LogDebug($"Received compensation get request for '{id}'");

            var compensation = _compensationService.GetById(id);

            if (compensation == null)
                return NotFound();

            return Ok(compensation);
        }

        /// <summary>
        /// Update a single compensation record
        /// </summary>
        /// <param name="id">The id of the compensation to update</param>
        /// <param name="newCompensation">The payload containing the complete updated compensation information</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult ReplaceCompensation(String id, [FromBody]Compensation newCompensation)
        {
            _logger.LogDebug($"Recieved compensation update request for '{id}'");

            var existingCompensation = _compensationService.GetById(id);
            if (existingCompensation == null)
                return NotFound();

            _compensationService.Replace(existingCompensation, newCompensation);

            return Ok(newCompensation);
        }
    }
}
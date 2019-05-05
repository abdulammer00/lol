﻿using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    // Inherits from ControllerBase (and not Controller) which means this controller
    // does not have any View support
    //[Authorize] -- We are now removing this attribute since we are globally specifying
    // authorization in the the Startup.cs class:
    //services.AddMvc(options => {
    //            var policy = new AuthorizationPolicyBuilder()
    //                // This is going to require authentication globally
    //                .RequireAuthenticatedUser()
    //                .Build();
    //options.Filters.Add(new AuthorizeFilter(policy));
    //        })
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _context;

        public ValuesController(DataContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin, Moderator")]
        // GET: api/Values
        [HttpGet]
        public IEnumerable<Value> GetValues()
        {
            return _context.Values;
        }

        [Authorize(Roles = "Member")]
        // GET: api/Values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var value = await _context.Values.FindAsync(id);

            if (value == null)
            {
                return NotFound();
            }

            return Ok(value);
        }

        // PUT: api/Values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutValue([FromRoute] int id, [FromBody] Value value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != value.Id)
            {
                return BadRequest();
            }

            _context.Entry(value).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ValueExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Values
        [HttpPost]
        public async Task<IActionResult> PostValue([FromBody] Value value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Values.Add(value);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetValue", new { id = value.Id }, value);
        }

        // DELETE: api/Values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteValue([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var value = await _context.Values.FindAsync(id);
            if (value == null)
            {
                return NotFound();
            }

            _context.Values.Remove(value);
            await _context.SaveChangesAsync();

            return Ok(value);
        }

        private bool ValueExists(int id)
        {
            return _context.Values.Any(e => e.Id == id);
        }
    }
}
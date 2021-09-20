using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMSite6.Models;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace MMSite6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsAPIController : ControllerBase
    {
        private readonly MMSite6Context _context;

        public DocumentsAPIController(MMSite6Context context)
        {
            _context = context;
        }
		[Authorize(Roles = "User, Admin")]
		// GET: api/DocumentsAPI
		[HttpGet]
        public async Task<ActionResult<IEnumerable<Document>>> Getdocument()
        {
            return await _context.document.ToListAsync();
        }
		[Authorize(Roles = "User, Admin")]
		// GET: api/DocumentsAPI/5
		[HttpGet("{id}")]
        public async Task<ActionResult<Document>> GetDocument(int id)
        {
            var document = await _context.document.FindAsync(id);

            if (document == null)
            {
                return NotFound();
            }

            return document;
        }
		[Authorize(Roles = "Admin")]
		// PUT: api/DocumentsAPI/5
		[HttpPut("{id}")]
        public async Task<IActionResult> PutDocument(int id, Document document)
        {
            if (id != document.fileId)
            {
                return BadRequest();
            }

            _context.Entry(document).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentExists(id))
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
		[Authorize(Roles = "Admin")]
		// POST: api/DocumentsAPI
		[HttpPost]
        public async Task<ActionResult<Document>> PostDocument(Document document)
        {
            _context.document.Add(document);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDocument", new { id = document.fileId }, document);
        }
		[Authorize(Roles = "Admin")]
		// DELETE: api/DocumentsAPI/5
		[HttpDelete("{id}")]
        public async Task<ActionResult<Document>> DeleteDocument(int id)
        {
            var document = await _context.document.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            _context.document.Remove(document);
            await _context.SaveChangesAsync();

			return Redirect("https://mitchellmeasures.azurewebsites.net/");
		}

        private bool DocumentExists(int id)
        {
            return _context.document.Any(e => e.fileId == id);
        }
    }
}

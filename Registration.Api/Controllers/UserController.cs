using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Registration.Api.Models;
using Registration.Api.Services;

namespace Registration.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        const string USERADD_EVENTTYPE = "User.Added";
        const string USERUPDATE_EVENTTYPE = "User.Updated";
        const string USERDELETE_EVENTTYPE = "User.Removed";

        private readonly RegistrationContext _context;
        private readonly IMessageService _msgService;

        public UserController(RegistrationContext context, IMessageService messageService)
        {
            _context = context;
            _msgService = messageService;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("findByEmail")]
        public ActionResult<IEnumerable<User>> FindByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("email");
            
            return Ok(_context.Users.Where(u => u.UserRegEmail.Equals(email)).AsEnumerable());
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            user.ModifiedBy = Request.HttpContext.User.Identity.Name;
            user.ModifiedDate = DateTime.UtcNow;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                await _msgService.SendMessageAsync(new Message { Subject = "User Updated", EventType = USERUPDATE_EVENTTYPE, Data = new { id = user.Id, email = user.UserRegEmail } });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            var currentUser = GetCurrentUser();
            var currentDate = DateTime.UtcNow;

            user.CreatedBy = currentUser;
            user.CreatedDate = currentDate;
            user.ModifiedBy = currentUser;
            user.ModifiedDate = currentDate;
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            await _msgService.SendMessageAsync(new Message { Subject = "New User Registered", EventType = USERADD_EVENTTYPE, Data = new { id = user.Id, email = user.UserRegEmail }});

            return CreatedAtAction("GetUser", new { id = user.Id}, user);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            await _msgService.SendMessageAsync(new Message { Subject = "User Removed", EventType = USERDELETE_EVENTTYPE, Data = new { id = user.Id, email = user.UserRegEmail } });

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private string GetCurrentUser()
        {
            return Request.HttpContext.User.Identity.Name;
        }
    }
}

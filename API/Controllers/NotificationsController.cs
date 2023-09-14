using AutoMapper;
using DAL.BLModels;
using DAL.DALModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using NuGet.Protocol.Plugins;
using System.Runtime.Intrinsics.X86;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly DwaMoviesContext _db;
        private readonly IMapper _mapper;

        public NotificationsController(DwaMoviesContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications()
        {
            var dbNotifications = await _db.Notifications.ToListAsync();
            var respNotifications = _mapper.Map<IEnumerable<BLNotification>>(dbNotifications);
            return Ok(respNotifications);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(int id)
        {
            var dbNotification = await _db.Notifications.FindAsync(id);
            if (dbNotification == null)
            {
                return NotFound();
            }
            var respNotification = _mapper.Map<BLNotification>(dbNotification);
            return Ok(respNotification);
        }
        [HttpPost]
        public async Task<ActionResult<Notification>> PostNotification(BLNotification notification)
        {
            var dbNotification = _mapper.Map<Notification>(notification);
            dbNotification.CreatedAt = DateTime.Now;
            dbNotification.SentAt = null;

            _db.Notifications.Add(dbNotification);
            await _db.SaveChangesAsync();
            return CreatedAtAction("GetNotification", new { id = dbNotification.Id }, notification);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotification(int id, BLNotification notification)
        {
            var dbNotification = _db.Notifications.FindAsync(id);
            if (dbNotification == null)
            {
                return NotFound();
            }

            dbNotification.Result.ReceiverEmail = notification.ReceiverEmail;
            dbNotification.Result.Subject = notification.Subject;
            dbNotification.Result.Body = notification.Body;
            
            
            await _db.SaveChangesAsync();


            return Ok(notification);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var dbNotification = await _db.Notifications.FindAsync(id);
            if (dbNotification == null)
            {
                return NotFound();
            }
            var respNotification = _mapper.Map<BLNotification>(dbNotification);
            _db.Notifications.Remove(dbNotification);
            await _db.SaveChangesAsync();

            return Ok($"Deleted notification with id {id}");
        }
        
        [HttpPost("send")]
        public async Task<IActionResult> SendNotifications()
        {
            var dbNotifications = await _db.Notifications.Where(n => n.SentAt == null).ToListAsync();

            foreach (var dbNotification in dbNotifications)
            {
                try
                {
                    var apiKey = "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";
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress("petar.markota@gmail.com", "Petar");
                    var subject = dbNotification.Subject;

                    var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dbNotification.ReceiverEmail);

                    var toName = user != null ? user.FirstName : "User";
                    var to = new EmailAddress(dbNotification.ReceiverEmail, toName);
                    var plainTextContent = dbNotification.Body;
                    var htmlContent = dbNotification.Body;
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                    var response = await client.SendEmailAsync(msg);
                    dbNotification.SentAt = DateTime.Now;
                }



                catch (Exception e)
                {
                    await Console.Out.WriteLineAsync(e.Message);
                    continue;
                }
            }

            await _db.SaveChangesAsync();
            var respNotifications = _mapper.Map<IEnumerable<BLNotification>>(dbNotifications);
            return Ok(respNotifications);
        }

        
        [HttpGet("unsent")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetUnsentNotifications()
        {
            var dbNotifications = await _db.Notifications.Where(n => n.SentAt == null).ToListAsync();
            var respNotifications = _mapper.Map<IEnumerable<BLNotification>>(dbNotifications);
            return Ok(respNotifications);
        }
        [HttpGet("unsent/count")]
        public async Task<ActionResult<int>> GetUnsentNotificationCount()
        {
            var dbNotifications = await _db.Notifications.Where(n => n.SentAt == null).ToListAsync();
            return Ok(dbNotifications.Count);
        }
    }
}

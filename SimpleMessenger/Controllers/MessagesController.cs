using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleMessenger.Data;
using SimpleMessenger.Models;

namespace SimpleMessenger.Controllers
{
    public class MessagesController : Controller
    {
        private readonly MessengerContext _context;

        public MessagesController(MessengerContext context)
        {
            _context = context;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            var messengerContext = _context.Messages.Include(m => m.Conversation).Include(m => m.User);
            return View(await messengerContext.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Conversation)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create(int? id)
        {
            ViewData["ConversationID"] = new SelectList(_context.Conversations, "ID", "ID");
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID");
            ViewBag.replyId = id;
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Body,Created,UserID,ConversationID,IsActive")] Message message, int? replyId)
        {   
            Message message2 = _context.Messages.First(x=>x.ID==replyId);
            if (ModelState.IsValid)
            {
                message.Reply = message2.Body;
                _context.Add(message);
                await _context.SaveChangesAsync();
                return Redirect("~/Conversations/Details/" + message.ConversationID);
            }
            ViewData["ConversationID"] = new SelectList(_context.Conversations, "ID", "ID", message.ConversationID);
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", message.UserID);
            return View(message);
        }


        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOwn(string body, int conversationId)
        {
            var currentUser = _context.Users.Include(x => x.UserConversations).FirstOrDefault(x => x.Email == HttpContext.User.Identity.Name);

            Message message = new Message { Body = body, ConversationID = conversationId, UserID = currentUser.ID, Created = DateTime.Now, IsActive = true };
            Console.WriteLine(message);
            _context.Messages.Add(message);
            _context.SaveChanges();
            return Redirect("~/Conversations/Details/" + conversationId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReply(string body, int replyId)
        {
            var currentUser = _context.Users.Include(x => x.UserConversations).FirstOrDefault(x => x.Email == HttpContext.User.Identity.Name);
            var message2 = _context.Messages.First(x => x.ID == replyId);
            Message message = new Message { Body = body, ConversationID = message2.ConversationID, UserID = currentUser.ID, Created = DateTime.Now, IsActive = true, Reply = message2.Body };
            Console.WriteLine(message);
            _context.Messages.Add(message);
            _context.SaveChanges();
            return Redirect("~/Conversations/Details/" + message2.ConversationID);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["ConversationID"] = new SelectList(_context.Conversations, "ID", "ID", message.ConversationID);
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", message.UserID);
            return View(message);
        }

        
        public async Task<IActionResult> DeleteForAll(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            message.IsActive = false;
            _context.Remove(message);
            await _context.SaveChangesAsync();
            return Redirect("~/Conversations/Details/" + message.ConversationID);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> DeleteForMe(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            message.IsActive = false;
            _context.Update(message);
            await _context.SaveChangesAsync();

            return Redirect("~/Conversations/Details/" + message.ConversationID);
        }




        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Body,Reply,Created,UserID,ConversationID,IsActive")] Message message)
        {
            if (id != message.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("~/Conversations/Details/" + message.ConversationID);
            }
            ViewData["ConversationID"] = new SelectList(_context.Conversations, "ID", "ID", message.ConversationID);
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", message.UserID);
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Conversation)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.ID == id);
        }
    }
}

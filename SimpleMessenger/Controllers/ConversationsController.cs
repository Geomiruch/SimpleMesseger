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
    public class ConversationsController : Controller
    {
        private readonly MessengerContext _context;

        public ConversationsController(MessengerContext context)
        {
            _context = context;
        }

        // GET: Conversations
        public async Task<IActionResult> Index()
        {
            List<Conversation> conversations = new List<Conversation>();
            var currentUser = _context.Users.Include(x => x.UserConversations).FirstOrDefault(x => x.Email == HttpContext.User.Identity.Name);

            foreach (var c in _context.Conversations.Include(x => x.UserConversations).Include(x=>x.ConversationType).Include(x=>x.Messages).ToList())
            {
                if (c.UserConversations.FirstOrDefault(x => x.UserID == currentUser.ID) != null)
                {
                    if (c.ConversationTypeID == 2)
                    {
                        int userID = c.UserConversations.First(x => x.UserID != currentUser.ID).UserID;
                        var User = _context.Users.First(x => x.ID == userID);
                        c.Name = User.FirstName + " " + User.LastName;
                    }
                    conversations.Add(c);
                }    
            }

            return View(conversations);
        }

        // GET: Conversations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conversation = await _context.Conversations.Include(x => x.UserConversations).Include(x => x.Messages)
                .FirstOrDefaultAsync(m => m.ID == id);

            foreach(Message m in conversation.Messages)
            {
                m.User = _context.Users.First(x => x.ID == m.UserID);
            }

            if (conversation == null)
            {
                return NotFound();
            }

            //todo sort messages...

            var currentUser = _context.Users.Include(x => x.UserConversations).FirstOrDefault(x => x.Email == HttpContext.User.Identity.Name);

            if (conversation.ConversationTypeID == 2)
            {
                int userID = conversation.UserConversations.First(x => x.UserID != currentUser.ID).UserID;
                var User = _context.Users.First(x => x.ID == userID);
                conversation.Name = User.FirstName + " " + User.LastName;
            }

            ViewBag.UserID = currentUser.ID;
            
            return View(conversation);
        }

        // GET: Conversations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Conversations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] Conversation conversation)
        {
            Conversation c = new Conversation { Name = conversation.Name, ConversationTypeID = 1 };
            if (ModelState.IsValid)
            {
                _context.Add(c);
                await _context.SaveChangesAsync();
                var currentUser = _context.Users.Include(x => x.UserConversations).FirstOrDefault(x => x.Email == HttpContext.User.Identity.Name);
                _context.UserConversations.Add(new UserConversation { ConversationID = c.ID, UserID = currentUser.ID });
                await _context.SaveChangesAsync();
                return Redirect("~/Conversations/Details/" + c.ID);
            }
            return Redirect("~/Conversations/Details/" + c.ID);
        }

        // GET: Conversations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conversation = await _context.Conversations.FindAsync(id);
            if (conversation == null)
            {
                return NotFound();
            }
            return View(conversation);
        }

        // POST: Conversations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] Conversation conversation)
        {
            if (id != conversation.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(conversation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConversationExists(conversation.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(conversation);
        }

        // GET: Conversations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(m => m.ID == id);
            if (conversation == null)
            {
                return NotFound();
            }

            return View(conversation);
        }

        // POST: Conversations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var conversation = await _context.Conversations.FindAsync(id);
            _context.Conversations.Remove(conversation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConversationExists(int id)
        {
            return _context.Conversations.Any(e => e.ID == id);
        }
    }
}

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
    public class UserConversationsController : Controller
    {
        private readonly MessengerContext _context;

        public UserConversationsController(MessengerContext context)
        {
            _context = context;
        }

        // GET: UserConversations
        public async Task<IActionResult> Index()
        {
            var messengerContext = _context.UserConversations.Include(u => u.Conversation).Include(u => u.User);
            return View(await messengerContext.ToListAsync());
        }

        // GET: UserConversations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userConversation = await _context.UserConversations
                .Include(u => u.Conversation)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (userConversation == null)
            {
                return NotFound();
            }

            return View(userConversation);
        }

        // GET: UserConversations/Create
        public IActionResult Create(int? id)
        {
            ViewBag.ConID = id;
            //todo add filer for persons who already exists
            var userIDs = _context.Conversations.Include(x=>x.UserConversations).First(x => x.ID == id).UserConversations.Select(x=>x.UserID).ToList();

            ViewData["UserID"] = new SelectList(_context.Users.Where(x => !userIDs.Contains(x.ID)), "ID", "FullName");
            return View();
        }

        // POST: UserConversations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,ConversationID")] UserConversation userConversation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userConversation);
                await _context.SaveChangesAsync();
                return Redirect("~/Conversations/Details/" + userConversation.ConversationID);
            }
            ViewData["ConversationID"] = new SelectList(_context.Conversations, "ID", "ID", userConversation.ConversationID);
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", userConversation.UserID);
            return View(userConversation);
        }

        // GET: UserConversations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userConversation = await _context.UserConversations.FindAsync(id);
            if (userConversation == null)
            {
                return NotFound();
            }
            ViewData["ConversationID"] = new SelectList(_context.Conversations, "ID", "ID", userConversation.ConversationID);
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", userConversation.UserID);
            return View(userConversation);
        }

        // POST: UserConversations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,ConversationID")] UserConversation userConversation)
        {
            if (id != userConversation.UserID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userConversation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserConversationExists(userConversation.UserID))
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
            ViewData["ConversationID"] = new SelectList(_context.Conversations, "ID", "ID", userConversation.ConversationID);
            ViewData["UserID"] = new SelectList(_context.Users, "ID", "ID", userConversation.UserID);
            return View(userConversation);
        }

        // GET: UserConversations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userConversation = await _context.UserConversations
                .Include(u => u.Conversation)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (userConversation == null)
            {
                return NotFound();
            }

            return View(userConversation);
        }

        // POST: UserConversations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userConversation = await _context.UserConversations.FindAsync(id);
            _context.UserConversations.Remove(userConversation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserConversationExists(int id)
        {
            return _context.UserConversations.Any(e => e.UserID == id);
        }
    }
}

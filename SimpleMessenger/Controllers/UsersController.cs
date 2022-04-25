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
    public class UsersController : Controller
    {
        private readonly MessengerContext _context;

        public UsersController(MessengerContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var currentUser = _context.Users.Include(x => x.UserConversations).FirstOrDefault(x => x.Email == HttpContext.User.Identity.Name);
            return View(await _context.Users.Where(x=>x.ID != currentUser.ID).ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public async Task<IActionResult> Write(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = _context.Users.Include(x => x.UserConversations).FirstOrDefault(x => x.Email == HttpContext.User.Identity.Name);

            var currentUserConversationsIDS = _context.UserConversations.Where(x => x.UserID == currentUser.ID).Select(x => x.ConversationID).Distinct();
            var seconduUserConversationsIDS = _context.UserConversations.Where(x => x.UserID == id).Select(x => x.ConversationID).Distinct();

            List<int> ids = new List<int>();
            foreach (var c in currentUserConversationsIDS)
                if (seconduUserConversationsIDS.Contains(c))
                    ids.Add(c);
                
            foreach (var c in ids)
            {
                var conv = _context.Conversations.First(x => x.ID == c);
                if(conv.ConversationTypeID == 2)
                {
                    return Redirect("~/Conversations/Details/" + conv.ID);
                }
            }
            /*var currentUserConversations = _context.Conversations.Where(x => x.ConversationTypeID == 2 && currentUserConversationsIDS.Contains(x.ID)).Select(x => x.ID);
            var secondUserConversations = _context.Conversations.Where(x => x.ConversationTypeID == 2 && currentUserConversationsIDS.Contains(x.ID)).Select(x => x.ID);*/

            /*foreach(var c in currentUserConversations)
            {
                foreach (var c2 in secondUserConversations)
                {
                    if (c == c2)
                        
                }
            }*/

            Conversation conversation = new Conversation { ConversationTypeID = 2, Name = "" };
            _context.Conversations.Add(conversation);
            _context.SaveChanges();
            int conversationId = conversation.ID;
            _context.UserConversations.Add(new UserConversation { ConversationID = conversationId, UserID = currentUser.ID });
            _context.UserConversations.Add(new UserConversation { ConversationID = conversationId, UserID = (int)id });
            _context.SaveChanges();
            return Redirect("~/Conversations/Details/" + conversationId);

            /*var conversations = _context.UserConversations.Join(_context.UserConversations,
                current => current.ConversationID,
                second => second.ConversationID,
                (current, second) => new
                {
                    currentUserID = current.UserID,
                    secondUserID = second.UserID,
                    conversationID = current.ConversationID
                }).Where(x=>(x.currentUserID == currentUser.ID && x.secondUserID == id) || (x.currentUserID == id && x.secondUserID == currentUser.ID));

            if (conversations.Any())
            {
                foreach(var temp in conversations)
                {
                    Conversation conversation = _context.Conversations.First(x => x.ConversationTypeID == 2 && temp.conversationID == x.ID);
                    if(conversation != null)
                    {
                        return Redirect("~/Conversations/Details/" + conversation.ID);
                    }
                }
            }
            else
            {
                Conversation conversation = new Conversation { ConversationTypeID = 2, Name = "" };
                _context.Conversations.Add(conversation);
                _context.SaveChanges();
                int conversationId = conversation.ID;
                _context.UserConversations.Add(new UserConversation { ConversationID = conversationId, UserID = currentUser.ID });
                _context.UserConversations.Add(new UserConversation { ConversationID = conversationId, UserID = (int)id });
                _context.SaveChanges();
                return Redirect("~/Conversations/Details/" + conversationId);
            }*/
            return View("Index");
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Email,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,Email,Password")] User user)
        {
            if (id != user.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.ID == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using LiveChat.Models;

namespace LiveChat.Controllers
{
    public class DefaultController : Controller
    {
        [HttpGet,AllowAnonymous]
        public ActionResult Login()
        {
            if (HttpContext.User.Identity.Name != null)
            {
                FormsAuthentication.SignOut();
            }
            return View();
        }

        [HttpPost, AllowAnonymous]
        public ActionResult Login(Person peron)
        {
            DatabaseContext db = new DatabaseContext();
            Person newPerson = db.personTable.FirstOrDefault(x => x.userName == peron.userName);

            if (newPerson == null)
            {
                newPerson = peron;
                newPerson.lastChatDate = DateTime.Now;
                db.personTable.Add(newPerson);
                db.SaveChanges();
            }
            
            FormsAuthentication.SetAuthCookie(peron.userName, false);

            return RedirectToAction("UserList", "Default");
        }

        [HttpGet, Authorize]
        public ActionResult UserList()
        {
            DatabaseContext db = new DatabaseContext();

            List<Person> persons = new List<Person>();

            foreach (Person person in db.personTable.ToList())
            {
                if (person.userName != HttpContext.User.Identity.Name)
                {
                    persons.Add(person);
                }
            }
            
            return View(persons);
        }

        [HttpGet, Authorize]
        public ActionResult Message(int otherId)
        {
            DatabaseContext db = new DatabaseContext();
            Person person1 = db.personTable.FirstOrDefault(x => x.userName == HttpContext.User.Identity.Name);
            Person person2 = db.personTable.FirstOrDefault(x => x.Id == otherId);

            ChatViewModel viewModel = new ChatViewModel();
            viewModel.otherName = person2.userName;
            viewModel.otherId = otherId;

            return View(viewModel);
        }

        [HttpPost, Authorize]
        public ActionResult Message(int otherId, string newMessage)
        {
            DatabaseContext db = new DatabaseContext();
            Person person1 = db.personTable.FirstOrDefault(x => x.userName == HttpContext.User.Identity.Name);
            Person person2 = db.personTable.FirstOrDefault(x => x.Id == otherId);

            ChatViewModel viewModel = new ChatViewModel();
            viewModel.otherName = person2.userName;
            viewModel.otherId = otherId;

            DateTime nowDate = DateTime.Now;

            if (newMessage.Count() > 200)
            {
                ViewBag.error = "Bir mesajda 200den fazla karakter olamaz";
                return View(viewModel);
            }
            else if ((nowDate - person1.lastChatDate).TotalSeconds < 5)
            {
                ViewBag.error = "Son mesajdan sonra 5 saniyeden daha az zaman geçti, lütfen bekleyin.";
                return View(viewModel);
            }

            Chat newChat = new Chat {
                message = newMessage,
                toUserId = person2.Id,
                date = nowDate,
            };

            person1.lastChatDate = nowDate;

            person1.chatTable.Add(newChat);

            db.SaveChanges();

            return View(viewModel);
        }

        [HttpGet, Authorize]
        public ActionResult GetMessages(int otherId, int page)
        {
            DatabaseContext db = new DatabaseContext();
            Person person1 = db.personTable.FirstOrDefault(x => x.userName == HttpContext.User.Identity.Name);
            Person person2 = db.personTable.FirstOrDefault(x => x.Id == otherId);

            int pageSize = 50;

            List<Chat> messagesFromPerson1 = person1.chatTable
            .Where(x => x.toUserId == person2.Id).ToList();

            List<Chat> messagesFromPerson2 = person2.chatTable
                .Where(x => x.toUserId == person1.Id).ToList();

            List<Chat> messages = messagesFromPerson1.Concat(messagesFromPerson2)
                .OrderByDescending(x => x.date).Skip((page-1)* pageSize).Take(pageSize).ToList();

            return Json(messages, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Authorize]
        public ActionResult GetLastMessage(int otherId)
        {
            DatabaseContext db = new DatabaseContext();
            Person person1 = db.personTable.FirstOrDefault(x => x.userName == HttpContext.User.Identity.Name);
            Person person2 = db.personTable.FirstOrDefault(x => x.Id == otherId);

            Chat lastMessage = person2.chatTable
                .Where(x => x.toUserId == person1.Id)
                .OrderByDescending(x => x.date)
                .FirstOrDefault();

            return Json(lastMessage, JsonRequestBehavior.AllowGet);
        }
    }
}
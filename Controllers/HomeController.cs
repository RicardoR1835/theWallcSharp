using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheWallCSharp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TheWallCSharp.Controllers
{
    public class HomeController : Controller
    {
        private HomeContext dbContext;
     
        // here we can "inject" our context service into the constructorcopy
        public HomeController(HomeContext context)
        {
            dbContext = context;
        }
        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("create")]
        public IActionResult Create(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(user => user.Email == newUser.Email))
                {
                    ModelState.AddModelError("NewUser.Email", "Email already in use!");
                    return View("Index");
                }
                else
                {
                    Console.WriteLine(newUser.UserId);
                    HttpContext.Session.SetInt32("Id", newUser.UserId);
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password); 
                    dbContext.Add(newUser);
                    dbContext.SaveChanges();
                    Console.WriteLine(newUser.UserId);
                    return Redirect($"dash/{HttpContext.Session.GetInt32("Id")}");
                }
            }
            return View("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LogUser LoggedUser)
        {

            if(ModelState.IsValid)
            {
                var confirmUser = dbContext.Users.FirstOrDefault(user => user.Email == LoggedUser.Email);
                if(confirmUser == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                
                var hasher = new PasswordHasher<LogUser>();
                
                var result = hasher.VerifyHashedPassword(LoggedUser, confirmUser.Password, LoggedUser.Password);
                
                if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                else
                {
                    HttpContext.Session.SetInt32("Id", confirmUser.UserId);
                    return Redirect($"dash/{confirmUser.UserId}");
                }
            }
            return View("Index");

        }

        [HttpGet("dash/{id}")]
        public IActionResult Dash(int id)
        {
            if (HttpContext.Session.GetInt32("Id") == null)
            {
                ModelState.AddModelError("Email", "Please log in or register!");
                return View("Index");
            }
            if (HttpContext.Session.GetInt32("Id") != id)
            {
                return Redirect($"/dash/{HttpContext.Session.GetInt32("Id")}");
            }
            User loggedUser = dbContext.Users
            .FirstOrDefault(user => user.UserId == id);

            var AllMessagesAndComments = dbContext.Messages
            .Include( mes => mes.Creator)
            .Include(mess => mess.Comments)
            .ThenInclude(com => com.CommentCreator)
            .Include(mes => mes.Likes)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

            ViewBag.Id = id;
            ViewBag.LoggedIn = loggedUser;
            ViewBag.Messages = AllMessagesAndComments;

            return View("TheWall");
        }

        [HttpPost("create/message")]
        public IActionResult CreateMessage(Message newMessage)
        {
            if(ModelState.IsValid)
            {
                User Creator = dbContext.Users.FirstOrDefault(user => user.UserId == newMessage.UserId);
                newMessage.UserId = Creator.UserId;
                newMessage.Creator = Creator;
                dbContext.Add(newMessage);
                dbContext.SaveChanges();
                return Redirect($"/dash/{newMessage.UserId}");
            }
            User loggedUser = dbContext.Users
            .FirstOrDefault(user => user.UserId == newMessage.UserId);

            var AllMessagesAndComments = dbContext.Messages
            .Include(mess => mess.Likes)
            .Include(mes => mes.Comments)
            .ThenInclude(likes => likes.CommentCreator)
            .ToList();

            ViewBag.Id = newMessage.UserId;
            ViewBag.LoggedIn = loggedUser;
            ViewBag.Messages = AllMessagesAndComments;
            return View($"dash/{HttpContext.Session.GetInt32("Id")}");
        }

        [HttpPost("create_comment/{id}")]
        public IActionResult CreateComment(int id, Comment commentContent)
        {
            if(ModelState.IsValid)
            {
                User Creator = dbContext.Users.FirstOrDefault(user => user.UserId == commentContent.UserId);
                Message CommentedMessage = dbContext.Messages.FirstOrDefault(mes => mes.MessageId == id);
                Comment newComment = new Comment();
                newComment.MessageId = id;
                newComment.UserId = Creator.UserId;
                newComment.CommentCreator = Creator;
                newComment.Content = commentContent.Content;
                dbContext.Add(newComment);
                dbContext.SaveChanges();
                return Redirect($"/dash/{newComment.UserId}");
            }
            User loggedUser = dbContext.Users
            .FirstOrDefault(user => user.UserId == commentContent.UserId);

            var AllMessagesAndComments = dbContext.Messages
            .Include(mess => mess.Likes)
            .Include(mes => mes.Comments)
            .ThenInclude(likes => likes.CommentCreator)
            .ToList();

            ViewBag.Id = commentContent.UserId;
            ViewBag.LoggedIn = loggedUser;
            ViewBag.Messages = AllMessagesAndComments;
            return View($"dash/{HttpContext.Session.GetInt32("Id")}");
        }

        [HttpGet("destroym/{id}")]
        public IActionResult DestroyMessage(int id)
        {
            Message thisMessage = dbContext.Messages
            .FirstOrDefault(mes => mes.MessageId == id);
            dbContext.Remove(thisMessage);
            dbContext.SaveChanges();
            return Redirect($"/dash/{HttpContext.Session.GetInt32("Id")}");
        }

        [HttpGet("destroyc/{id}")]
        public IActionResult DestroyComment(int id)
        {
            Comment thisComment = dbContext.Comments
            .FirstOrDefault(com => com.CommentId == id);
            dbContext.Remove(thisComment);
            dbContext.SaveChanges();
            return Redirect($"/dash/{HttpContext.Session.GetInt32("Id")}");
        }

        [HttpGet("like/{id}")]
        public IActionResult Like(int id)
        {
            Message PostToLike = dbContext.Messages
            .FirstOrDefault(thismes => thismes.MessageId == id);

            User UserLiking = dbContext.Users
            .FirstOrDefault(user => user.UserId == HttpContext.Session.GetInt32("Id"));

            Like NewLike = new Like();
            NewLike.UserId = UserLiking.UserId;
            NewLike.MessageId = PostToLike.MessageId;
            NewLike.UserLiking = UserLiking;
            NewLike.MessageLiked = PostToLike;
            dbContext.Add(NewLike);
            dbContext.SaveChanges();
            return Redirect($"/dash/{HttpContext.Session.GetInt32("Id")}");
        }

        [HttpGet("unlike/{id}")]
        public IActionResult Unlike(int id)
        {
            Message PostToLike = dbContext.Messages
            .FirstOrDefault(thismes => thismes.MessageId == id);

            User UserUnliking = dbContext.Users
            .FirstOrDefault(user => user.UserId == HttpContext.Session.GetInt32("Id"));

            List<Like> Likes = dbContext.Likes
            .Where(like => like.MessageId == PostToLike.MessageId)
            .ToList();
            Like Unlike = Likes.FirstOrDefault(user => user.UserId == UserUnliking.UserId);
            dbContext.Remove(Unlike);
            dbContext.SaveChanges();
            return Redirect($"/dash/{HttpContext.Session.GetInt32("Id")}");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}

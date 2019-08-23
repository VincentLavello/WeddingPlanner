using System.Net;
using System.Security.Cryptography;
using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
// using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private WeddingPlannerContext dbContext;
        public HomeController(WeddingPlannerContext context) => dbContext = context;

        [HttpGet("")]
        public IActionResult Index() {
            if (!this.IsLoggedIn) {
                return RedirectToAction("Register");
            }
            else {
                return RedirectToAction("Dashboard");
            }
        }
        [HttpPost("NewWedding")]
        public IActionResult NewWedding(Wedding NewWed) {
            int WeddingId = NewWed.WeddingId;
            if (this.IsLoggedIn && ModelState.IsValid)  {
                NewWed.Planner_Id=this.LoggedInUserID;
                dbContext.Weddings.Add(NewWed);
                dbContext.SaveChanges();
                // Debug.Assert(WeddingId!=0);
            }
            else {
                View("Register");
            }
        // return RedirectToAction("ShowWedding",  new { WeddingId =(int) NewWed.WeddingId});
            // return  View("ShowWedding", (int) NewWed.WeddingId);
            return View("ShowCreatedWedding",(int) NewWed.WeddingId );
        }        
        [HttpGet("Register")]
        public IActionResult Register() => View();
        [HttpGet("ViewNewWedding")]
        public IActionResult ViewNewWedding() => View("NewWedding");
        // [HttpGet("ShowWedding/{WeddingId}")]
        [HttpGet("ShowCreatedWedding")]
       public ViewResult ShowCreatedWedding(int WeddingId) {

            Wedding wed = dbContext.Weddings.Include(Wed=>Wed.Guests)
            .ThenInclude(g=>g.Guest).FirstOrDefault(w=>w.WeddingId==WeddingId);
          return  View("ShowWedding", wed);

       }
       
       
        [HttpGet("ShowWedding/{WeddingId}")]
        public IActionResult ShowWedding(int WeddingId )
        {
            if(WeddingId>0) {
                Wedding wed = dbContext.Weddings.Include(Wed=>Wed.Guests)
                .ThenInclude(g=>g.Guest).FirstOrDefault(w=>w.WeddingId==WeddingId);
                    
                System.Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                System.Console.WriteLine(wed.Guests.Count);
                List<RSVP> guests = wed.Guests.ToList();
                   System.Console.WriteLine(guests.Count );
                // foreach(RSVP g in guests) {
                //    System.Console.WriteLine(g.Guest.FullName);
                // }

             return View("ShowWedding", wed);

            }
            else {
                return View("Dashboard");
            }

    
        }
        
        [HttpGet("GetAllUsers")]
        public string GetAllUsers() 
        {
           string[] AllUsers = dbContext.Guests.Select(u=>u.FullName).ToArray();
            var json = JsonConvert.SerializeObject(AllUsers);
            // System.Console.WriteLine(json);
           return  json;  
           
            // List<User> candidates = dbContext.Guests.ToList();
            // System.Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            // foreach(var c in candidates) 
            // {
            //     System.Console.WriteLine(c.FirstName);
            // }
            // ViewBag.UserList = candidates;
        // }
            //  return View();
        }        
        [HttpGet("Dashboard")]
        public IActionResult Dashboard() 
        { if (IsLoggedIn) 
            {
                List<Wedding> All = dbContext.Weddings.Include(Wed=>Wed.Guests).ToList();
                ViewBag.UserId=LoggedInUserID;
                foreach (var w in All) {
                Console.WriteLine(w.Guests.Any(u=>u.UserId==ViewBag.UserId));

                }

                return View("Dashboard", All);         
            }
            else 
            {
                return RedirectToAction("Register");
            }

        }
        [HttpGet("RSVP/{WeddingId}")]
        public IActionResult RSVP(int WeddingId) {

                RSVP rsvp=new RSVP(WeddingId, LoggedInUserID);
                dbContext.RSVPs.Add(rsvp);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
         }
        [HttpGet("UNRSVP/{WeddingId}")]
        public IActionResult UNRSVP(int WeddingId) {
            RSVP rsvp = dbContext.RSVPs.FirstOrDefault(u=>u.WeddingId==WeddingId);
                dbContext.RSVPs.Remove(rsvp);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");

         }
        [HttpGet("CancelWedding/{WeddingId}")]
        public IActionResult CancelWedding(int WeddingId) {
                Wedding dead = dbContext.Weddings.FirstOrDefault(w=>w.WeddingId==WeddingId);
                if (dead!=null) 
                {
                    dbContext.Weddings.Remove(dead);
                    dbContext.SaveChanges();
                }
                return RedirectToAction("Dashboard");

         }

///@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
/// 
        [HttpPost]
        [Route("CreateUser")]
        public IActionResult CreateUser(User newUser)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", newUser);
            }
            else 
            {
                if (dbContext.Guests.Any(u => u.Email == newUser.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Register", newUser);
                }
                else 
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    newUser.CreatedAt = DateTime.Now;
                    dbContext.Add(newUser);
                    // OR dbContext.Guests.Add(newUser);
                    dbContext.SaveChanges();
                    SetLoggedInStatus(newUser.UserId);
                    // Console.WriteLine($"New User ID: {newUser.UserId.ToString()}");
                return RedirectToAction("Dashboard");

                }
            }
        }
        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // 
        // L O G   I N
        [HttpGet("CheckLogin")]
        public string CheckLogin() => IsLoggedIn ? "Yes I am logged in" : "No Not logged in.";        
        [HttpPost("Login")]
        public IActionResult Login(LoginReg user){
            PasswordVerificationResult hasherResult;
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Guests.FirstOrDefault(u => u.Email == user.LoginDetail.Email);
                
                // If no user exists with provided email
                if(userInDb == null) {
                    System.Console.WriteLine("Not in database");
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    IncrementLogInattempts();
                    Console.WriteLine($"attempts: {this.LoginAttempts.ToString()}");
                    ViewBag.Attempts  = this.LoginAttempts.ToString();
                }
                else
                {
                    // ModelState.AddModelError("Email", "Invalid Email/Password");
                    var hasher = new PasswordHasher<Login>();
                    // verify provided password against hash stored in db
                    hasherResult = hasher.VerifyHashedPassword(user.LoginDetail, userInDb.Password, user.LoginDetail.Password);
                    if(hasherResult != 0)
                    {
                        SetLoggedInStatus(userInDb.UserId);
                        // ViewBag.Transactions = GetUserTransactions(); // for use with partial

                        ClearLoginAttempts();
                        return RedirectToAction("Dashboard" ) ;
                    }
                }
            }
            return View("Register", user);
        }
        public  void SetLoggedInStatus(int SetUserID) 
            {
                // Debug.Assert (SetUserID!=0);
                if (SetUserID>0) {
                HttpContext.Session.SetInt32("UserID", (int)SetUserID); 
                }
            }
        private void IncrementLogInattempts() {
            int _LoginAttempts;
            _LoginAttempts=this.LoginAttempts;
            _LoginAttempts++;
            HttpContext.Session.SetInt32("LoginAttempts", (int)_LoginAttempts); 

        }
        private void ClearLoginAttempts() 
        {
            HttpContext.Session.Remove("LoginAttempts");
        }
        public int LoginAttempts {
            get {
                 int? _LoginAttempts = HttpContext.Session.GetInt32("LoginAttempts");

                if (_LoginAttempts != null && _LoginAttempts >0) 
                {
                    return (int) _LoginAttempts;
                }
             return 0;
            }
        }
        public bool IsLoggedIn 
        {  get {
                int? _UserID = HttpContext.Session.GetInt32("UserID");
                // Debug.Assert(_UserID!= null);
                if (_UserID == null) {return  false;}
                else  { return true;}
                }
        }
        //
        // L O G O U T
        [HttpGet("logout")]
        public string logout (){

            HttpContext.Session.Clear();
            return (IsLoggedIn ? "Still logged in..." : "Logged out");
        }
        /// G E T  L O G G E D   I N  U S E R
        public int LoggedInUserID
               {  get {
                int? _UserID = HttpContext.Session.GetInt32("UserID");
                // Debug.Assert(_UserID!= null);
                // Debug.Assert(_UserID!= 0);

                if (_UserID == null) {return  0;}
                else  { return (int) _UserID;}
                }
        }
        public IActionResult Privacy() => View();
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
// error///  crap 
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace mvcproj.Controllers
{
    public class HelloWorldController : Controller
    {
        private readonly Models.MVCTestContext TestContext;

        public HelloWorldController(Models.MVCTestContext _context)
        {
            TestContext = _context;
        }

        public IActionResult Index(string _failAction = "")
        {
            if (_failAction == "SessionExpired")
            {
                ViewData["SessionExpired"] = "true";
            }
            else if (_failAction.Length > 0)
            {
                ViewData["LogFail"] = _failAction;
            }

            var _userId = HttpContext.Session.GetSession<string>("UserId");
            if (_userId != null)
            {
                return RedirectToAction("Home", new { id = _userId });
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Validate(string _name, string _pass)
        {
            var _uId = await (from n in TestContext.User
                              where n.name == _name && n.pass == _pass.Encrypt()
                              select n.id).FirstOrDefaultAsync();
            if (_uId > 0)
            {
                HttpContext.Session.SetSession<string>("User", _name);
                HttpContext.Session.SetSession<int>("UserId", _uId);

                return RedirectToAction("Home", new { id = _uId });
            }
            else
            {
                return RedirectToAction("Index", new { _failAction = _name });
            }
        }

        public async Task<IActionResult> Home(int? id)
        {
            if (id != null)
            {
                var _user = HttpContext.Session.GetSession<string>("User");
                if (_user != null)
                {
                    ViewData["User"] = _user;

                    return View(await TestContext.User.ToListAsync());
                }
                else
                {
                    return RedirectToAction("Index", new { _failAction = "SessionExpired" });
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null && HttpContext.Session.GetSession<int>("UserId") != 0)
            {
                try
                {
                    var _user = await (from user in TestContext.User
                                       where user.id == id
                                       select user).FirstAsync();

                    return View(_user);
                }
                catch
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int _id, string _pass)
        {
            if (_pass != null)
            {
                var _user = await (from user in TestContext.User
                                   where user.id == _id
                                   select user).FirstAsync();

                _user.pass = _pass.Encrypt();

                await TestContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null && HttpContext.Session.GetSession<int>("UserId") != 0)
            {
                try
                {
                    return View(await (from user in TestContext.User
                                       where user.id == id
                                       select user).FirstAsync());
                }
                catch
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(int _id)
        {
            if (_id != 0 && HttpContext.Session.GetSession<int>("UserId") != 0)
            {
                TestContext.User.Remove(await (from user in TestContext.User
                                               where user.id == _id
                                               select user).FirstAsync());

                await TestContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult Add()
        {
            if (HttpContext.Session.GetSession<int>("UserId") != 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(string _name, string _pass)
        {
            await TestContext.User.AddAsync(new Models.Users()
            {
                name = _name,
                pass = _pass.Encrypt()
            });

            await TestContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index");
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}
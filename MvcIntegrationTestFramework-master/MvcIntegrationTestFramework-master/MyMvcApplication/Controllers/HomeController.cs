﻿using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace MyMvcApplication.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }
        
        [HttpPut]
        public ActionResult Echo()
        {
            Request.InputStream.Position = 0;
            var raw = new StreamReader(Request.InputStream).ReadToEnd();

            return Content(Request.ContentType +" "+ raw);
        }

        public ActionResult DoStuffWithSessionAndCookies()
        {
            var inputValue = "";
            var requestCookie = Request.Cookies["inputCookie"];
            if (requestCookie != null) inputValue = requestCookie.Value;

            Session["myIncrementingSessionItem"] = (int?) (Session["myIncrementingSessionItem"] ?? 0) + 1;
            Response.Cookies.Add(new HttpCookie("mycookie", inputValue+"_Changed"));

            return Content("OK");
        }

        public ActionResult FaultyRoute()
        {
            throw new NullReferenceException("This is a sample exception");
        }

        public ActionResult WhoAmI()
        {
            return Content(System.Web.HttpContext.Current.User.Identity.GetType().Name);
        }

        [Authorize]
        public ActionResult SecretAction()
        {
            return Content("Hello, you're logged in as " + User.Identity.Name);
        }
    }
}
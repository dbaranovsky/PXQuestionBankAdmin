using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SamlTest.Controllers
{   
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var certs = new List<string>();

            X509Store store = new X509Store(StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            foreach (var cert in store.Certificates)
            {
                certs.Add(cert.Subject);
            }
            store.Close();

            ViewBag.Message = "Welcome to ASP.NET MVC!";
            ViewBag.Certs = certs;

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}

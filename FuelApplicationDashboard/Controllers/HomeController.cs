using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using  FuelApplicationDashboard.Controllers.Management;
using FuelApplicationDashboard.Models;
namespace FuelApplicationDashboard.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(loginModel model)
        {
            

                if (ModelState.IsValid)
                {

                    String r = new logInController().checkPassword(model.Shortcode, model.Password);
                    if (r == "T")
                    {

                        Session["s"] = model.Shortcode;
                        return RedirectToActionPermanent("Home");
                    }
                    else if (r == "B")
                    {

                        return RedirectToAction("Blocked", "Error");
                    }
                    else if (r == "F")
                    {

                        return RedirectToAction("Index", new { V = "I" });
                    }
                    else
                    {
                        Session["s"] = model.Shortcode;
                        return RedirectToAction("Error500", "Error");
                    }
                }
                return View();
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your contact page.";

            if (Session["s"] != null)
            {
                ViewBag.Message = "Commercial Bank of Ethiopia About Page";
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }

       }
        public ActionResult AboutOutside()
        {
           ViewBag.Message = "Your contact page.";
           return View();
        }
        public ActionResult CommingSoon()
        {

        return View();

        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            if (Session["s"] != null)
            {


                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult TransactionList()
        {
            if (Session["s"] != null)
            {
                List<TXN> query = new TransactionListCRUD().SelectAllTransactions(Session["s"].ToString());
                ViewData["MyData"] = query;
                return View(query);

            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }
        public ActionResult AttendantDetail(String f)
        {
            if (Session["s"] != null)
            {

                if (!String.IsNullOrEmpty(f))
                {
                    DateTime now = DateTime.Now;
                    AttendantsList query = new AttendantsCRUD().SelectAttendantsById(f);
                    ViewData["MyData"] = query;
                    ViewBag.daytoday = new TransactionListCRUD().SelectTransactionByDate(now.Date,query.AttendantName);
                    ViewBag.dayBackOne = new TransactionListCRUD().SelectTransactionByDate(now.AddDays(-1).Date, query.AttendantName);
                    ViewBag.dayBackTwo = new TransactionListCRUD().SelectTransactionByDate(now.AddDays(-2).Date,query.AttendantName);
                    ViewBag.dayBackThree = new TransactionListCRUD().SelectTransactionByDate(now.AddDays(-3).Date, query.AttendantName);
                    ViewBag.dayBackFour = new TransactionListCRUD().SelectTransactionByDate(now.AddDays(-4).Date, query.AttendantName);
                    ViewBag.dayBackFive = new TransactionListCRUD().SelectTransactionByDate(now.AddDays(-5).Date, query.AttendantName);
                    ViewBag.dayBackSix = new TransactionListCRUD().SelectTransactionByDate(now.AddDays(-5).Date, query.AttendantName);
                    return View(query);
                }
                else
                {
                    return RedirectToAction("Error500", "Error");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
           
        }
        public ActionResult UpdateStatus(string i, string s)
        {
            if (Session["s"] != null)
            {
                try
                {
                    Boolean result = new AttendantsCRUD().UpdateAttendantStatus(i, s);
                    if (result)
                    {
                        return RedirectToAction("Attendants");
                    }
                    else
                    {
                        return RedirectToAction("Error400", "Error");
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error500", "Error");
                }

            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }
        public ActionResult Attendants()
        {
            if (Session["s"] != null)
            {
                List<AttendantsList> query = new AttendantsCRUD().SelectAttendants(Session["s"].ToString());
                ViewData["MyData"] = query;
                return View(query);

            }
            else
            {
                return RedirectToAction("Index");
            }

           
        }
        public ActionResult AccountSetting()
        {
            if (Session["s"] != null)
            {

                return View();

            }
            else
            {
                return RedirectToAction("Index");
            }
          
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AccountSetting(changepinModel model)
        {


            if (Session["s"] != null)
            {

                if (ModelState.IsValid)
                {

                    string check = new logInController().checkPassword(Session["s"].ToString(), model.currentpin);
                    if (check == "T")
                    {
                        bool r = new logInController().changingPIN(Session["s"].ToString(), model.newpin);
                        Session["changePasswordStatus"] = new security().encrypt("cbekeys");
                        return RedirectToAction("AccountSetting"); 
                    }
                    else {
                        Session["changePasswordStatus"] = new security().encrypt("cbekeyb");
                        return RedirectToAction("AccountSetting");
                    }
                   
                      
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Home()

        {
            

            if (Session["s"] != null)
            {

                MerchantInfo query = new MerchantInfoController().selectMerchantInformation(Session["s"].ToString());
                ViewData["MyData"] = query;
                return View(query);

            }
            else
            {
                return RedirectToAction("Index");
            }
           
        }
        public ActionResult SignOut()
        {

            if (Session["s"] != null)
            {

                Session["s"] = null;
                return RedirectToAction("Index");

            }
            else
            {
                return RedirectToAction("Index");
            }

        }
        public ActionResult Test()
        {
            
                return View();

           
        }
      
    }
}
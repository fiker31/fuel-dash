using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FuelApplicationDashboard.Models;
namespace FuelApplicationDashboard.Controllers.Management
{
    public class logInController : Controller
    {
        // GET: logIn

        FuelDBEntities CBE = new FuelDBEntities();
        public ActionResult Index()
        {
            return View();
        }
       
        public String checkPassword(String shortcode, String password)
        {
            var rop = (from l in CBE.MerchantInfoes where l.Shortcode == shortcode && l.Password == password select l).Count();
            if (rop > 0) {
                var rop2 = (from l in CBE.MerchantInfoes where l.Shortcode == shortcode && l.Password == password &&  l.Status=="Active" select l).Count();
                if(rop2 > 0)
                {
                    return "T";
                }
                return "B";
            } else
            {
                return "F";
            }

        }
        public Boolean updateLoginStatus(string Id, String status)
        {
            var attendantId = Guid.Parse(Id);
            using (var context = new FuelDBEntities())
            {
                var user = (from st in context.AttendantsLists
                            where st.Id == attendantId
                            select st).FirstOrDefault<AttendantsList>();
                user.Status = status;
                int change = context.SaveChanges();
                if (change > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public Boolean changingPIN(string shortcode, String newpin)
        {
            
            using (var context = new FuelDBEntities())
            {
                var user = (from st in context.MerchantInfoes
                            where st.Shortcode == shortcode
                            select st).FirstOrDefault<MerchantInfo>();
                user.Password = newpin;
                int change = context.SaveChanges();
                if (change > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
    }
}
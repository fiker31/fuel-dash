using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FuelApplicationDashboard.Models;
namespace FuelApplicationDashboard.Controllers.Management
{
    public class MerchantInfoController : Controller
    {
        // GET: MerchantInfo
        public ActionResult Index()
        {
            return View();
        }

        public MerchantInfo selectMerchantInformation(string shortcode)
        {
            using (var context = new FuelDBEntities())
            {
                MerchantInfo query = (from st in context.MerchantInfoes
                                   where st.Shortcode == shortcode select st).FirstOrDefault<MerchantInfo>();


                return query;
            }

        }
    }
}
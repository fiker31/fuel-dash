using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FuelApplicationDashboard.Models;
namespace FuelApplicationDashboard.Controllers.Management
{
    public class AttendantsCRUD : Controller
    {
        // GET: AttendantsCRUD



        public ActionResult Index()
        {
            return View();
        }
      
        public List<AttendantsList> SelectAttendants(string shortcode)
        {
            using (var context = new FuelDBEntities())
            {
                List<AttendantsList> query = (from st in context.AttendantsLists where st.Shortcode == shortcode
                                 orderby st.Id descending  select st).ToList<AttendantsList>();

                return query;
            }

        }
        public Boolean UpdateAttendantStatus(string Id,String status)
        {
            var attendantId = Guid.Parse(Id);
            using (var context = new FuelDBEntities())
            {
                var user = (from st in context.AttendantsLists
                            where st.Id == attendantId
                            select st).FirstOrDefault<AttendantsList>();
                user.Status = status;
                int change = context.SaveChanges();
                if(change > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

     public AttendantsList SelectAttendantsById(String Id)
        {
            var attendantId = Guid.Parse(Id);
            using (var context = new FuelDBEntities())
            {
               AttendantsList query = (from st in context.AttendantsLists where st.Id == attendantId
                                              select st).FirstOrDefault<AttendantsList>();

                return query;
            }

        }
    }
}
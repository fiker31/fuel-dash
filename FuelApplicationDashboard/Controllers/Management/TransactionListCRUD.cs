using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FuelApplicationDashboard.Models;
namespace FuelApplicationDashboard.Controllers.Management
{
    public class TransactionListCRUD : Controller
    {
        // GET: TransactionListCRUD
        public ActionResult Index()
        {
            return View();
        }
        public List<TXN> SelectAllTransactions(string shortcode)
        {
            using (var context = new FuelDBEntities())
            {
                List<TXN> query = (from st in context.TXNs
                                   where st.TillCode == shortcode
                                   orderby st.TransactionEndFuel descending select st).ToList<TXN>();

                return query;
            }

        }
        public List<TXN> SelectTransactionByDate(DateTime comparedate,string phone)
        {
            using (var context = new FuelDBEntities())
            {
                List<TXN> query = (from st in context.TXNs
                                   where DbFunctions.TruncateTime(st.TransactionEndFuel) == comparedate && st.OperatorName==phone
                                   orderby st.TransactionEndFuel descending select st).ToList<TXN>();
                

                return query;
            }

        }
    }
}
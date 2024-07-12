using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FuelApplicationDashboard.Models;
namespace FuelApplicationDashboard.Controllers.Management
{
    public class StatisticController : Controller
    {
        // GET: Statistic
        
        public ActionResult Index()
        {
            return View();
        }
        public double totalAttendants(string shortcode)
        {
            using (var context = new FuelDBEntities())
            {
                double query = (from st in context.AttendantsLists
                                              where st.Shortcode == shortcode
                                              orderby st.Id descending
                                              select st).ToList<AttendantsList>().Count();

                return query;
            }

        }

        public decimal weeklyDiesel(string shortcode)
        {
            DateTime today = DateTime.Now;
            DateTime week = DateTime.Now.AddDays(-7);

            using (var context = new FuelDBEntities())
            {
                var query = (from st in context.TXNs
                                where (st.TillCode == shortcode) && (DbFunctions.TruncateTime(st.TransactionEndFuel) < today && DbFunctions.TruncateTime(st.TransactionEndFuel) > week) && (st.Type=="Diesel")
                                orderby st.Id descending
                                select st).ToList<TXN>();
                decimal Tamount  = query.AsEnumerable().Sum(o => o.Amount);

                return Tamount;
            }

        }
        public decimal weeklyDiselLi(string shortcode)
        {
            DateTime today = DateTime.Now.Date;
            DateTime week = DateTime.Now.AddDays(-7).Date;

            using (var context = new FuelDBEntities())
            {
                var query = (from st in context.TXNs
                             where (st.TillCode == shortcode) && (DbFunctions.TruncateTime(st.TransactionEndFuel) < today && DbFunctions.TruncateTime(st.TransactionEndFuel) > week) && (st.Type == "Diesel")
                             orderby st.Id descending
                             select st).ToList<TXN>();

                var literPrice = (from st in context.FuelPrices
                                  where (st.StartDate <= week && st.EndDate >= today)
                                  select st).ToList<FuelPrice>();
                decimal Tamount = query.AsEnumerable().Sum(o => o.Amount);
                if (literPrice.Count > 1)
                {
                    var P1 = (from st in context.TXNs
                              where (st.TillCode == shortcode) && (DbFunctions.TruncateTime(st.TransactionEndFuel) > literPrice[0].StartDate && DbFunctions.TruncateTime(st.TransactionEndFuel) < literPrice[0].EndDate) && (st.Type == "Diesel")
                              orderby st.Id descending
                              select st).ToList<TXN>();
                    var P2 = (from st in context.TXNs
                              where (st.TillCode == shortcode) && (DbFunctions.TruncateTime(st.TransactionEndFuel) > literPrice[1].StartDate && DbFunctions.TruncateTime(st.TransactionEndFuel) < literPrice[1].EndDate) && (st.Type == "Diesel")
                              orderby st.Id descending
                              select st).ToList<TXN>();
                    decimal P1T = P1.AsEnumerable().Sum(o => o.Amount);
                    decimal P2T = P2.AsEnumerable().Sum(o => o.Amount);

                    return (P1T / literPrice[0].DIESEL) + (P2T / literPrice[1].DIESEL);
                }
                if (literPrice.Count == 1)
                {

                    decimal v = Tamount / literPrice[0].DIESEL;
                    return v;
                }


                return Tamount;
            }

        }
        public decimal dailyDiselLi(string shortcode)
        {
            DateTime today = DateTime.Now.Date;

            using (var context = new FuelDBEntities())
            {
                var query = (from st in context.TXNs
                             where (st.TillCode == shortcode) && DbFunctions.TruncateTime(st.TransactionEndFuel) == today.Date && st.Type == "Diesel"
                             orderby st.Id descending
                             select st).ToList<TXN>();

                var literPrice = (from st in context.FuelPrices
                                  where (DbFunctions.TruncateTime(st.StartDate) <= today && DbFunctions.TruncateTime(st.EndDate) >= today)
                                  orderby st.Id descending
                                  select st).FirstOrDefault<FuelPrice>();
                decimal Tamount = query.AsEnumerable().Sum(o => o.Amount);
                decimal r = Tamount / literPrice.DIESEL;
                return r;
                
            }

        }
        public decimal dailyBenzelLi(string shortcode)
        {
            DateTime today = DateTime.Now.Date;

            using (var context = new FuelDBEntities())
            {
                var query = (from st in context.TXNs
                             where (st.TillCode == shortcode) && DbFunctions.TruncateTime(st.TransactionEndFuel) == today && st.Type == "Benzel"
                             orderby st.Id descending
                             select st).ToList<TXN>();

                var literPrice = (from st in context.FuelPrices
                                  where (DbFunctions.TruncateTime(st.StartDate) <= today && DbFunctions.TruncateTime(st.EndDate) >= today)
                                  orderby st.Id descending
                                  select st).FirstOrDefault<FuelPrice>();
                decimal Tamount = query.AsEnumerable().Sum(o => o.Amount);

                return Tamount / literPrice.BENZEL;

            }

        }
        public decimal weeklyBenzelLi(string shortcode)
        {
            DateTime today = DateTime.Now;
            DateTime week = DateTime.Now.AddDays(-7);

            using (var context = new FuelDBEntities())
            {
                var query = (from st in context.TXNs
                             where (st.TillCode == shortcode) && (DbFunctions.TruncateTime(st.TransactionEndFuel) < today && DbFunctions.TruncateTime(st.TransactionEndFuel) > week) && (st.Type == "Benzel")
                             orderby st.Id descending
                             select st).ToList<TXN>();

                var literPrice = (from st in context.FuelPrices
                             where (st.StartDate <= week && st.EndDate >= today)
                             orderby st.Id descending
                             select st).ToList<FuelPrice>();
                decimal Tamount = query.AsEnumerable().Sum(o => o.Amount);
                if (literPrice.Count > 1)
                {
                    var P1 = (from st in context.TXNs
                                 where (st.TillCode == shortcode) && (DbFunctions.TruncateTime(st.TransactionEndFuel) > literPrice[0].StartDate && DbFunctions.TruncateTime(st.TransactionEndFuel) < literPrice[0].EndDate) && (st.Type == "Benzel")
                                 orderby st.Id descending
                                 select st).ToList<TXN>();
                    var P2 = (from st in context.TXNs
                              where (st.TillCode == shortcode) && (DbFunctions.TruncateTime(st.TransactionEndFuel) > literPrice[1].StartDate && DbFunctions.TruncateTime(st.TransactionEndFuel) < literPrice[1].EndDate) && (st.Type == "Benzel")
                              orderby st.Id descending
                              select st).ToList<TXN>();
                    decimal P1T = P1.AsEnumerable().Sum(o => o.Amount);
                    decimal P2T = P2.AsEnumerable().Sum(o => o.Amount);

                    return (P1T/literPrice[0].BENZEL) + (P2T / literPrice[1].BENZEL);
                }
                if(literPrice.Count ==1 ) {

                    return Tamount / literPrice[0].BENZEL;
                }
                

                return Tamount;
            }

        }
        public decimal weeklyBenzel(string shortcode)
        {
            DateTime today = DateTime.Now;
            DateTime week = DateTime.Now.AddDays(-7);

            using (var context = new FuelDBEntities())
            {
                var query = (from st in context.TXNs
                             where (st.TillCode == shortcode) && (DbFunctions.TruncateTime(st.TransactionEndFuel) < today && DbFunctions.TruncateTime(st.TransactionEndFuel) > week) && (st.Type == "Benzel")
                             orderby st.Id descending
                             select st).ToList<TXN>();
                decimal Tamount = query.AsEnumerable().Sum(o => o.Amount);

                return Tamount;
            }

        }

        public decimal dailyBenzel(string shortcode)
        {
            DateTime today = DateTime.Now.Date;

            using (var context = new FuelDBEntities())
            {
                var query = (from st in context.TXNs
                             where (st.TillCode == shortcode) && DbFunctions.TruncateTime(st.TransactionEndFuel) == today.Date && st.Type == "Benzel"
                             orderby st.Id descending
                             select st).ToList<TXN>();

                
                decimal Tamount = query.AsEnumerable().Sum(o => o.Amount);

                return Tamount;

            }

        }
        public decimal dailyDiesel(string shortcode)
        {
            DateTime today = DateTime.Now.Date;

            using (var context = new FuelDBEntities())
            {
                var query = (from st in context.TXNs
                             where (st.TillCode == shortcode) && DbFunctions.TruncateTime(st.TransactionEndFuel) == today.Date && st.Type == "Diesel"
                             orderby st.Id descending
                             select st).ToList<TXN>();


                decimal Tamount = query.AsEnumerable().Sum(o => o.Amount);

                return Tamount;

            }

        }

        public int pendingAttendants(string shortcode)
        {
            DateTime today = DateTime.Now.Date;

            using (var context = new FuelDBEntities())
            {
                int query = (from st in context.AttendantsLists
                             where (st.Shortcode == shortcode) && st.Status == "Pending"
                             select st).ToList<AttendantsList>().Count();

                return query;

            }

        }

        public List<string> weeklySalesList(string shortcode)
        {
           
            DateTime today = DateTime.Now.Date;
            DateTime todayMinusOne = DateTime.Now.Date.AddDays(-1);
            DateTime todayMinusTwo = DateTime.Now.Date.AddDays(-2);
            DateTime todayMinusThree = DateTime.Now.Date.AddDays(-3);
            DateTime todayMinusFour = DateTime.Now.Date.AddDays(-4);
            DateTime todayMinusFive = DateTime.Now.Date.AddDays(-5);
            DateTime todayMinusSix = DateTime.Now.Date.AddDays(-6);

            using (var context = new FuelDBEntities())
            {
                var todayTransaction = (from st in context.TXNs
                                        where (st.TillCode == shortcode) && DbFunctions.TruncateTime(st.TransactionEndFuel) == today.Date
                                        orderby st.Id descending
                                        select st).ToList<TXN>();

                var todayMinusOneTransactions = (from st in context.TXNs
                                                 where (st.TillCode == shortcode) && DbFunctions.TruncateTime(st.TransactionEndFuel) == todayMinusOne.Date
                                                 orderby st.Id descending
                                                 select st).ToList<TXN>();

                var todayMinusTwoTransactions = (from st in context.TXNs
                                                 where (st.TillCode == shortcode) && DbFunctions.TruncateTime(st.TransactionEndFuel) == todayMinusTwo.Date
                                                 orderby st.Id descending
                                                 select st).ToList<TXN>();

                var todayMinusThreeTransactions = (from st in context.TXNs
                                                   where (st.TillCode == shortcode) && DbFunctions.TruncateTime(st.TransactionEndFuel) == todayMinusThree.Date
                                                   orderby st.Id descending
                                                   select st).ToList<TXN>();

                var todayMinusFourTransactions = (from st in context.TXNs
                                                  where (st.TillCode == shortcode) && DbFunctions.TruncateTime(st.TransactionEndFuel) == todayMinusFour.Date
                                                  orderby st.Id descending
                                                  select st).ToList<TXN>();
                var todayMinusFiveTransactions = (from st in context.TXNs
                                                  where (st.TillCode == shortcode) && DbFunctions.TruncateTime(st.TransactionEndFuel) == todayMinusFive.Date
                                                  orderby st.Id descending
                                                  select st).ToList<TXN>();

                var todayMinusSixTransactions = (from st in context.TXNs
                                                 where (st.TillCode == shortcode) && DbFunctions.TruncateTime(st.TransactionEndFuel) == todayMinusSix.Date
                                                 orderby st.Id descending
                                                 select st).ToList<TXN>();
                List<String> returnList = new List<string>();
                returnList.Add(todayTransaction.AsEnumerable().Sum(o => o.Amount).ToString());
                returnList.Add(todayMinusOneTransactions.AsEnumerable().Sum(o => o.Amount).ToString());
                returnList.Add(todayMinusTwoTransactions.AsEnumerable().Sum(o => o.Amount).ToString());
                returnList.Add(todayMinusThreeTransactions.AsEnumerable().Sum(o => o.Amount).ToString());
                returnList.Add(todayMinusFourTransactions.AsEnumerable().Sum(o => o.Amount).ToString());
                returnList.Add(todayMinusFiveTransactions.AsEnumerable().Sum(o => o.Amount).ToString());
                returnList.Add(todayMinusSixTransactions.AsEnumerable().Sum(o => o.Amount).ToString());


                return returnList;


            }

        }
        public decimal WeeklyAttendant(string phone)
        {
            DateTime today = DateTime.Now;
            DateTime week = DateTime.Now.AddDays(-7);

            using (var context = new FuelDBEntities())
            {
                var query = (from st in context.TXNs
                             where (st.OperatorName == phone) && (DbFunctions.TruncateTime(st.TransactionEndFuel) < today && DbFunctions.TruncateTime(st.TransactionEndFuel) > week)
                             orderby st.Id descending
                             select st).ToList<TXN>();
                decimal Tamount = query.AsEnumerable().Sum(o => o.Amount);

                return Tamount;
            }

        }
        public decimal WeeklyAttendantLi(string phone)
        {
            DateTime today = DateTime.Now;
            DateTime week = DateTime.Now.AddDays(-7);

            using (var context = new FuelDBEntities())
            {
                var TransactionBenzel = (from st in context.TXNs
                             where (st.OperatorName == phone) && (DbFunctions.TruncateTime(st.TransactionEndFuel) < today && DbFunctions.TruncateTime(st.TransactionEndFuel) > week) && st.Type == "Benzel"
                             orderby st.Id descending
                             select st).ToList<TXN>();

                var TransactionDiesel = (from st in context.TXNs
                             where (st.OperatorName == phone) && (DbFunctions.TruncateTime(st.TransactionEndFuel) < today && DbFunctions.TruncateTime(st.TransactionEndFuel) > week) && st.Type== "Diesel"
                             orderby st.Id descending
                             select st).ToList<TXN>();
                decimal TamountBenzel = TransactionBenzel.AsEnumerable().Sum(o => o.Amount);
                decimal TamountDiesel = TransactionDiesel.AsEnumerable().Sum(o => o.Amount);

                var literPrice = (from st in context.FuelPrices
                                  where (st.StartDate <= week && st.EndDate >= today)
                                  orderby st.Id descending
                                  select st).ToList<FuelPrice>();
             
                
                if (literPrice.Count == 1)
                {

                    return (TamountBenzel / literPrice[0].BENZEL) + (TamountDiesel / literPrice[0].DIESEL);
                }


                return 0;
            }

        }
    }
}
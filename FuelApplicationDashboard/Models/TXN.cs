//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FuelApplicationDashboard.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TXN
    {
        public System.Guid Id { get; set; }
        public string TillCode { get; set; }
        public string Phone { get; set; }
        public decimal Amount { get; set; }
        public string PlateNo { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public string OperatorName { get; set; }
        public string Type { get; set; }
        public System.DateTime TransactionEndFuel { get; set; }
        public Nullable<System.DateTime> InsertDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string InsertUser { get; set; }
        public string UpdateUser { get; set; }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UniqueStudio.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public int PackageId { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        public string CustomerName { get; set; }
        public string PackageName { get; set; }

        // Dropdown lists
        public List<SelectListItem> CustomerList { get; set; }
        public List<SelectListItem> PackageList { get; set; }
    }
}
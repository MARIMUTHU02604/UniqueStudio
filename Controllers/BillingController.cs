using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using UniqueStudio.Models;

namespace UniqueStudio.Controllers
{
    public class BillingController : Controller
    {
        private readonly IConfiguration _config;

        public BillingController(IConfiguration config)
        {
            _config = config;
        }

        private string GetConnectionString()
        {
            return _config.GetConnectionString("DbConnection");
        }

        public IActionResult Index()
        {
            var billingsList = new List<Billing>();
            string conString = GetConnectionString();

            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "SELECT * FROM billings";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            billingsList.Add(new Billing
                            {
                                BillingId = Convert.ToInt32(sdr["billingid"]),
                                BookingId = Convert.ToInt32(sdr["bookingid"]),
                                TotalAmount = sdr["totalamount"] != DBNull.Value ? Convert.ToDecimal(sdr["totalamount"]) : 0,
                                PaidAmount = sdr["paidamount"] != DBNull.Value ? Convert.ToDecimal(sdr["paidamount"]) : 0,
                                PaymentStatus = sdr["paymentstatus"] as string
                            });
                        }
                    }
                }
            }
            return View(billingsList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.BookingId = new SelectList(GetBookingsList(), "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Billing billingModel)
        {
            if (ModelState.IsValid)
            {
                string conString = GetConnectionString();
                using (SqlConnection con = new SqlConnection(conString))
                {
                    string query = "INSERT INTO billings (bookingid, totalamount, paidamount, paymentstatus) VALUES (@BookingId, @TotalAmount, @PaidAmount, @PaymentStatus)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@BookingId", billingModel.BookingId);
                        cmd.Parameters.AddWithValue("@TotalAmount", billingModel.TotalAmount);
                        cmd.Parameters.AddWithValue("@PaidAmount", billingModel.PaidAmount);
                        cmd.Parameters.AddWithValue("@PaymentStatus", (object)billingModel.PaymentStatus ?? DBNull.Value);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.BookingId = new SelectList(GetBookingsList(), "Value", "Text", billingModel.BookingId);
            return View(billingModel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Billing billingModel = null;
            string conString = GetConnectionString();

            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "SELECT * FROM billings WHERE billingid = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            billingModel = new Billing
                            {
                                BillingId = Convert.ToInt32(sdr["billingid"]),
                                BookingId = Convert.ToInt32(sdr["bookingid"]),
                                TotalAmount = sdr["totalamount"] != DBNull.Value ? Convert.ToDecimal(sdr["totalamount"]) : 0,
                                PaidAmount = sdr["paidamount"] != DBNull.Value ? Convert.ToDecimal(sdr["paidamount"]) : 0,
                                PaymentStatus = sdr["paymentstatus"] as string
                            };
                        }
                    }
                }
            }

            if (billingModel == null)
            {
                return NotFound();
            }

            ViewBag.BookingId = new SelectList(GetBookingsList(), "Value", "Text", billingModel.BookingId);
            return View(billingModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Billing billingModel)
        {
            if (ModelState.IsValid)
            {
                string conString = GetConnectionString();
                using (SqlConnection con = new SqlConnection(conString))
                {
                    string query = "UPDATE billings SET bookingid = @BookingId, totalamount = @TotalAmount, paidamount = @PaidAmount, paymentstatus = @PaymentStatus WHERE billingid = @Id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@BookingId", billingModel.BookingId);
                        cmd.Parameters.AddWithValue("@TotalAmount", billingModel.TotalAmount);
                        cmd.Parameters.AddWithValue("@PaidAmount", billingModel.PaidAmount);
                        cmd.Parameters.AddWithValue("@PaymentStatus", (object)billingModel.PaymentStatus ?? DBNull.Value);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.BookingId = new SelectList(GetBookingsList(), "Value", "Text", billingModel.BookingId);
            return View(billingModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            string conString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "DELETE FROM billings WHERE billingid = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private List<SelectListItem> GetBookingsList()
        {
            var list = new List<SelectListItem>();
            string conString = GetConnectionString();

            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "SELECT b.bookingid, c.customername FROM bookings b INNER JOIN customers c ON b.customerid = c.customerid";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            list.Add(new SelectListItem
                            {
                                Value = sdr["bookingid"].ToString(),
                                Text = "Booking ID: " + sdr["bookingid"].ToString() + " - " + sdr["customername"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}
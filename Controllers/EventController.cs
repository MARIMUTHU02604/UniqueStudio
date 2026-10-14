using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using UniqueStudio.Models;

namespace UniqueStudio.Controllers
{
    public class EventsController : Controller
    {
        private readonly IConfiguration _config;

        public EventsController(IConfiguration config)
        {
            _config = config;
        }

        private string GetConnectionString()
        {
            return _config.GetConnectionString("DbConnection");
        }

        public IActionResult Index()
        {
            var eventsList = new List<Event>();
            string conString = GetConnectionString();

            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "SELECT * FROM events";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            eventsList.Add(new Event
                            {
                                EventId = Convert.ToInt32(sdr["eventid"]),
                                BookingId = Convert.ToInt32(sdr["bookingid"]),
                                EventDate = Convert.ToDateTime(sdr["eventdate"]),
                                Location = sdr["location"] as string,
                                Photographer = sdr["photographer"] as string
                            });
                        }
                    }
                }
            }
            return View(eventsList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.BookingId = new SelectList(GetBookingsList(), "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Event eventModel)
        {
            if (ModelState.IsValid)
            {
                string conString = GetConnectionString();
                using (SqlConnection con = new SqlConnection(conString))
                {
                    string query = "INSERT INTO events (bookingid, eventdate, location, photographer) VALUES (@BookingId, @EventDate, @Location, @Photographer)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@BookingId", eventModel.BookingId);
                        cmd.Parameters.AddWithValue("@EventDate", eventModel.EventDate);
                        cmd.Parameters.AddWithValue("@Location", (object)eventModel.Location ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Photographer", (object)eventModel.Photographer ?? DBNull.Value);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.BookingId = new SelectList(GetBookingsList(), "Value", "Text", eventModel.BookingId);
            return View(eventModel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Event eventModel = null;
            string conString = GetConnectionString();

            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "SELECT * FROM events WHERE eventid = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            eventModel = new Event
                            {
                                EventId = Convert.ToInt32(sdr["eventid"]),
                                BookingId = Convert.ToInt32(sdr["bookingid"]),
                                EventDate = Convert.ToDateTime(sdr["eventdate"]),
                                Location = sdr["location"] as string,
                                Photographer = sdr["photographer"] as string
                            };
                        }
                    }
                }
            }

            if (eventModel == null)
            {
                return NotFound();
            }

            ViewBag.BookingId = new SelectList(GetBookingsList(), "Value", "Text", eventModel.BookingId);
            return View(eventModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Event eventModel)
        {
            if (ModelState.IsValid)
            {
                string conString = GetConnectionString();
                using (SqlConnection con = new SqlConnection(conString))
                {
                    string query = "UPDATE events SET bookingid = @BookingId, eventdate = @EventDate, location = @Location, photographer = @Photographer WHERE eventid = @Id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@BookingId", eventModel.BookingId);
                        cmd.Parameters.AddWithValue("@EventDate", eventModel.EventDate);
                        cmd.Parameters.AddWithValue("@Location", (object)eventModel.Location ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Photographer", (object)eventModel.Photographer ?? DBNull.Value);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.BookingId = new SelectList(GetBookingsList(), "Value", "Text", eventModel.BookingId);
            return View(eventModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            string conString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "DELETE FROM events WHERE eventid = @Id";
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
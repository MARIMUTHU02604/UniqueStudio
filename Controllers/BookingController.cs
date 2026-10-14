using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;
using UniqueStudio.Models;

namespace UniqueStudio.Controllers
{
    public class BookingController : Controller
    {
        private readonly string _connString;

        public BookingController(IConfiguration configuration)
        {
            _connString = configuration.GetConnectionString("DBConnection");
        }

        public IActionResult Index()
        {
            List<Booking> bookings = new List<Booking>();

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = @"SELECT b.bookingid, b.customerid, b.packageid, b.bookingdate, b.totalamount, b.status, 
                                        c.customername, p.packagename 
                                 FROM bookings b
                                 LEFT JOIN customers c ON b.customerid = c.customerid
                                 LEFT JOIN packages p ON b.packageid = p.packageid";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bookings.Add(new Booking
                            {
                                BookingId = Convert.ToInt32(reader["bookingid"]),
                                CustomerId = reader["customerid"] != DBNull.Value ? Convert.ToInt32(reader["customerid"]) : 0,
                                PackageId = reader["packageid"] != DBNull.Value ? Convert.ToInt32(reader["packageid"]) : 0,
                                BookingDate = reader["bookingdate"] != DBNull.Value ? Convert.ToDateTime(reader["bookingdate"]) : DateTime.Now,
                                TotalAmount = reader["totalamount"] != DBNull.Value ? Convert.ToDecimal(reader["totalamount"]) : 0,
                                Status = reader["status"]?.ToString(),
                                CustomerName = reader["customername"]?.ToString(),
                                PackageName = reader["packagename"]?.ToString()
                            });
                        }
                    }
                }
            }

            return View(bookings);
        }

        public IActionResult Create()
        {
            Booking model = new Booking
            {
                BookingDate = DateTime.Today,
                CustomerList = GetCustomers(),
                PackageList = GetPackages()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Booking model)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = "INSERT INTO bookings (customerid, packageid, bookingdate, totalamount, status) VALUES (@CustomerId, @PackageId, @BookingDate, @TotalAmount, @Status)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", model.CustomerId);
                    cmd.Parameters.AddWithValue("@PackageId", model.PackageId);
                    cmd.Parameters.AddWithValue("@BookingDate", model.BookingDate);
                    cmd.Parameters.AddWithValue("@TotalAmount", model.TotalAmount);
                    cmd.Parameters.AddWithValue("@Status", string.IsNullOrEmpty(model.Status) ? "Pending" : model.Status);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Booking model = null;

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = "SELECT * FROM bookings WHERE bookingid = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new Booking
                            {
                                BookingId = Convert.ToInt32(reader["bookingid"]),
                                CustomerId = reader["customerid"] != DBNull.Value ? Convert.ToInt32(reader["customerid"]) : 0,
                                PackageId = reader["packageid"] != DBNull.Value ? Convert.ToInt32(reader["packageid"]) : 0,
                                BookingDate = reader["bookingdate"] != DBNull.Value ? Convert.ToDateTime(reader["bookingdate"]) : DateTime.Now,
                                TotalAmount = reader["totalamount"] != DBNull.Value ? Convert.ToDecimal(reader["totalamount"]) : 0,
                                Status = reader["status"]?.ToString()
                            };
                        }
                    }
                }
            }

            if (model == null)
            {
                return NotFound();
            }

            model.CustomerList = GetCustomers();
            model.PackageList = GetPackages();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Booking model)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = "UPDATE bookings SET customerid = @CustomerId, packageid = @PackageId, bookingdate = @BookingDate, totalamount = @TotalAmount, status = @Status WHERE bookingid = @BookingId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BookingId", model.BookingId);
                    cmd.Parameters.AddWithValue("@CustomerId", model.CustomerId);
                    cmd.Parameters.AddWithValue("@PackageId", model.PackageId);
                    cmd.Parameters.AddWithValue("@BookingDate", model.BookingDate);
                    cmd.Parameters.AddWithValue("@TotalAmount", model.TotalAmount);
                    cmd.Parameters.AddWithValue("@Status", string.IsNullOrEmpty(model.Status) ? "Pending" : model.Status);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Booking model = null;

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = @"SELECT b.bookingid, b.customerid, b.packageid, b.bookingdate, b.totalamount, b.status, 
                                        c.customername, p.packagename 
                                 FROM bookings b
                                 LEFT JOIN customers c ON b.customerid = c.customerid
                                 LEFT JOIN packages p ON b.packageid = p.packageid
                                 WHERE b.bookingid = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new Booking
                            {
                                BookingId = Convert.ToInt32(reader["bookingid"]),
                                CustomerId = reader["customerid"] != DBNull.Value ? Convert.ToInt32(reader["customerid"]) : 0,
                                PackageId = reader["packageid"] != DBNull.Value ? Convert.ToInt32(reader["packageid"]) : 0,
                                BookingDate = reader["bookingdate"] != DBNull.Value ? Convert.ToDateTime(reader["bookingdate"]) : DateTime.Now,
                                TotalAmount = reader["totalamount"] != DBNull.Value ? Convert.ToDecimal(reader["totalamount"]) : 0,
                                Status = reader["status"]?.ToString(),
                                CustomerName = reader["customername"]?.ToString(),
                                PackageName = reader["packagename"]?.ToString()
                            };
                        }
                    }
                }
            }

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int bookingid)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = "DELETE FROM bookings WHERE bookingid = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", bookingid);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        private List<SelectListItem> GetCustomers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = "SELECT customerid, customername FROM customers";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new SelectListItem
                            {
                                Value = reader["customerid"].ToString(),
                                Text = reader["customername"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        private List<SelectListItem> GetPackages()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = "SELECT packageid, packagename FROM packages";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new SelectListItem
                            {
                                Value = reader["packageid"].ToString(),
                                Text = reader["packagename"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}
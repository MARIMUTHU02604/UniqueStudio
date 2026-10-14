using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using UniqueStudio.Models;
using UniqueStudioMvc.Models;

namespace UniqueStudio.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IConfiguration _config;

        public CustomerController(IConfiguration config)
        {
            _config = config;
        }

        private string GetConnectionString()
        {
            return _config.GetConnectionString("DbConnection");
        }

        public IActionResult Index()
        {
            var customerList = new List<Customer>();
            string conString = GetConnectionString();

            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "SELECT * FROM customers";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            customerList.Add(new Customer
                            {
                                CustomerId = Convert.ToInt32(sdr["customerid"]),
                                CustomerName = sdr["customername"] as string,
                                Email = sdr["email"] as string,
                                Phone = sdr["phone"] as string
                            });
                        }
                    }
                }
            }
            return View(customerList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Customer customerModel)
        {
            if (ModelState.IsValid)
            {
                string conString = GetConnectionString();
                using (SqlConnection con = new SqlConnection(conString))
                {
                    string query = "INSERT INTO customers (customername, email, phone) VALUES (@CustomerName, @Email, @Phone)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CustomerName", (object)customerModel.CustomerName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", (object)customerModel.Email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Phone", (object)customerModel.Phone ?? DBNull.Value);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customerModel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Customer customerModel = null;
            string conString = GetConnectionString();

            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "SELECT * FROM customers WHERE customerid = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            customerModel = new Customer
                            {
                                CustomerId = Convert.ToInt32(sdr["customerid"]),
                                CustomerName = sdr["customername"] as string,
                                Email = sdr["email"] as string,
                                Phone = sdr["phone"] as string
                            };
                        }
                    }
                }
            }

            if (customerModel == null)
            {
                return NotFound();
            }

            return View(customerModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Customer customerModel)
        {
            if (ModelState.IsValid)
            {
                string conString = GetConnectionString();
                using (SqlConnection con = new SqlConnection(conString))
                {
                    string query = "UPDATE customers SET customername = @CustomerName, email = @Email, phone = @Phone WHERE customerid = @Id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@CustomerName", (object)customerModel.CustomerName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", (object)customerModel.Email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Phone", (object)customerModel.Phone ?? DBNull.Value);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customerModel);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Customer customerModel = null;
            string conString = GetConnectionString();

            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "SELECT * FROM customers WHERE customerid = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            customerModel = new Customer
                            {
                                CustomerId = Convert.ToInt32(sdr["customerid"]),
                                CustomerName = sdr["customername"] as string,
                                Email = sdr["email"] as string,
                                Phone = sdr["phone"] as string
                            };
                        }
                    }
                }
            }

            if (customerModel == null)
            {
                return NotFound();
            }

            return View(customerModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            string conString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "DELETE FROM customers WHERE customerid = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
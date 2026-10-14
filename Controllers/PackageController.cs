using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using UniqueStudio.Models;
using UniqueStudioMvc.Models;

namespace UniqueStudio.Controllers
{
    public class PackageController : Controller
    {
        private readonly IConfiguration _config;

        public PackageController(IConfiguration config)
        {
            _config = config;
        }

        private string GetConnectionString()
        {
            return _config.GetConnectionString("DbConnection");
        }

        public IActionResult Index()
        {
            var packageList = new List<Package>();
            string conString = GetConnectionString();

            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "SELECT * FROM packages";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            packageList.Add(new Package
                            {
                                PackageId = Convert.ToInt32(sdr["packageid"]),
                                PackageName = sdr["packagename"] as string,
                                Price = sdr["price"] != DBNull.Value ? Convert.ToDecimal(sdr["price"]) : 0,
                                Description = sdr["description"] as string
                            });
                        }
                    }
                }
            }
            return View(packageList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Package packageModel)
        {
            if (ModelState.IsValid)
            {
                string conString = GetConnectionString();
                using (SqlConnection con = new SqlConnection(conString))
                {
                    string query = "INSERT INTO packages (packagename, price, description) VALUES (@PackageName, @Price, @Description)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PackageName", (object)packageModel.PackageName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Price", packageModel.Price);
                        cmd.Parameters.AddWithValue("@Description", (object)packageModel.Description ?? DBNull.Value);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(packageModel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Package packageModel = null;
            string conString = GetConnectionString();

            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "SELECT * FROM packages WHERE packageid = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            packageModel = new Package
                            {
                                PackageId = Convert.ToInt32(sdr["packageid"]),
                                PackageName = sdr["packagename"] as string,
                                Price = sdr["price"] != DBNull.Value ? Convert.ToDecimal(sdr["price"]) : 0,
                                Description = sdr["description"] as string
                            };
                        }
                    }
                }
            }

            if (packageModel == null)
            {
                return NotFound();
            }

            return View(packageModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Package packageModel)
        {
            if (ModelState.IsValid)
            {
                string conString = GetConnectionString();
                using (SqlConnection con = new SqlConnection(conString))
                {
                    string query = "UPDATE packages SET packagename = @PackageName, price = @Price, description = @Description WHERE packageid = @Id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@PackageName", (object)packageModel.PackageName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Price", packageModel.Price);
                        cmd.Parameters.AddWithValue("@Description", (object)packageModel.Description ?? DBNull.Value);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(packageModel);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Package packageModel = null;
            string conString = GetConnectionString();

            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "SELECT * FROM packages WHERE packageid = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            packageModel = new Package
                            {
                                PackageId = Convert.ToInt32(sdr["packageid"]),
                                PackageName = sdr["packagename"] as string,
                                Price = sdr["price"] != DBNull.Value ? Convert.ToDecimal(sdr["price"]) : 0,
                                Description = sdr["description"] as string
                            };
                        }
                    }
                }
            }

            if (packageModel == null)
            {
                return NotFound();
            }

            return View(packageModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            string conString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "DELETE FROM packages WHERE packageid = @Id";
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
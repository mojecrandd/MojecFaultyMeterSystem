using MojecFaultyMeter.Config;
using MojecFaultyMeter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MojecFaultyMeter.Controllers
{
    public class AdminController : Controller
    {

        List<Discos> _discos = new List<Discos>();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Dashboard()
        {
            return View();
        }


        public ActionResult Dicos()
        {
            return View();
        }

        public ActionResult CreateDicos()
        {

            return View();
        }

     


        public ActionResult CreateMojecstore()
        {
            return View();
        }



        public ActionResult Discos()
        {
            _discos = new List<Discos>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("getallDiscos", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Discos disco = new Discos();
                    disco.DiscoID = Convert.ToInt32(rdr["DiscoID"].ToString());
                    disco.DiscoName = rdr["DiscoName"].ToString();
                    disco.DiscoAB = rdr["Discosn"].ToString();
                    _discos.Add(disco);
                }
                rdr.Close();
            }
            return View(_discos);
      
        }


        public ActionResult CreateDisco()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateDisco(Discos discos)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("AddDiscos", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DiscoName", discos.DiscoName);
                    cmd.Parameters.AddWithValue("@DiscoAB", discos.DiscoAB);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Disco has been saved successfully";
            return RedirectToAction("Discos");
            
        }

        public ActionResult RegisterUser()
        {
            return View();
        }

        public ActionResult CreateDiscoUsers(DiscoUser user)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("InsertDiscoUsers", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DiscoID", user.DiscoID);
                    cmd.Parameters.AddWithValue("@FullName", user.Fullname);
                    cmd.Parameters.AddWithValue("@UserName",  user.Username);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "User has been saved successfully";
            return RedirectToAction("RegisterUser");
        }

        public ActionResult CreateFactoryUsers(FactoryUsers user)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("InsertFactoryUsers", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FullName", user.Fullname);
                    cmd.Parameters.AddWithValue("@UserName", user.Username);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "User has been saved successfully";
            return RedirectToAction("RegisterUser");
        }

        public ActionResult CreateMojecStoreUser(MojecStoreUser user)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("InsertMojecStoreFactoryUsers", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FullName", user.Fullname);
                    cmd.Parameters.AddWithValue("@UserName", user.Username);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "User has been saved successfully";
            return RedirectToAction("RegisterUser");
        }




        public ActionResult DiscoUsers()
        {
            return View();
        }

        public ActionResult MojecStoreUsers()
        {
            return View();
        }

        public ActionResult FactoryUsers()
        {
            return View();
        }


        public ActionResult UpdateDiscos()
        {
            return View();
        }

       

        public ActionResult FaultyMeters()
        {
            return View();
        }

       
    }
}
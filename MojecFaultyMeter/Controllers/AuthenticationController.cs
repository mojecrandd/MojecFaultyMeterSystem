using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MojecFaultyMeter.Controllers
{
    public class AuthenticationController : Controller
    {
        // GET: Authentication

        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        //SqlDataReader dr;
        // GET: Authentication     
        void connectionString()
        {
            con.ConnectionString = Config.StoreConnection.GetConnection();
        }
        public ActionResult UsersLogin()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string Username,string Password)
        {

            string username = "";
            bool found = false;
            SqlDataReader dr;
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "Select * from AdminUser where Username = '" + Username + "'and Password = '" + Password + "'";
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                found = true;
                username = dr["Username"].ToString();
                FormsAuthentication.SetAuthCookie(Username, true);
                Session["Username"] = Username.ToString();
            }
            else
            {
                found = false;
            }
            dr.Close();
            con.Close();
            if (found == true)
            {
                
                    FormsAuthentication.SetAuthCookie(Username, true);
                    Session["Username"] = Username.ToString();
                    return RedirectToAction("Dashboard", "Admin");              
               
            }
            else
            {
                ViewBag.Error = "Username and Password are wrong!";
            }
            con.Close();
            return View();
        }
        public ActionResult MojecstoreLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult MojecstoreLogin(string Username, string Password)
        {
            string username = "";
            bool found = false;
            SqlDataReader dr;
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "GetMojecStoreUserByUsername";
            com.CommandType = System.Data.CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Username", Username);
            com.Parameters.AddWithValue("@Password", Password);
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                found = true;
                username = dr["M_Fullname"].ToString();
                FormsAuthentication.SetAuthCookie(Username, true);
                Session["M_Fullname"] = Username.ToString();
            }
            else
            {
                found = false;
            }
            dr.Close();
            con.Close();
            if (found == true)
            {

                FormsAuthentication.SetAuthCookie(Username, true);
                Session["Username"] = Username.ToString();
                return RedirectToAction("Dashboard", "Store");

            }
            else
            {
                ViewBag.Error = "Username and Password are wrong!";
            }
            con.Close();
            return View();
        }
        public ActionResult FactoryLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FactoryLogin(string Username, string Password)
        {
            string username = "";
            bool found = false;
            SqlDataReader dr;
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "GetFactoryUserByUsername";
            com.CommandType = System.Data.CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Username", Username);
            com.Parameters.AddWithValue("@Password", Password);
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                found = true;
                username = dr["F_Fullname"].ToString();
                FormsAuthentication.SetAuthCookie(Username, true);
                Session["F_Fullname"] = Username.ToString();
            }
            else
            {
                found = false;
            }
            dr.Close();
            con.Close();
            if (found == true)
            {

                FormsAuthentication.SetAuthCookie(Username, true);
                Session["Username"] = Username.ToString();
                return RedirectToAction("Dashboard", "Factory");

            }
            else
            {
                ViewBag.Error = "Username and Password are wrong!";
            }
            con.Close();
            return View();
        }
        public ActionResult DiscoLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DiscoLogin(string Username, string Password,string DiscoID,int? DiscoUserID)
        {
            string username = "";
            bool found = false;
            SqlDataReader dr;
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "GetDiscoUserByUsername";
            com.CommandType = System.Data.CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Username", Username);
            com.Parameters.AddWithValue("@Password", Password);
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                found = true;
                username = dr["Username"].ToString();
                DiscoID = dr["DiscoID"].ToString();
                DiscoUserID = Convert.ToInt32(dr["DiscoUserID"].ToString());
                FormsAuthentication.SetAuthCookie(Username, true);
                FormsAuthentication.SetAuthCookie(DiscoID, true);
                FormsAuthentication.SetAuthCookie(Convert.ToInt32(DiscoUserID).ToString(), true);
                Session["Username"] = Username.ToString();
                Session["DiscoID"] = DiscoID.ToString();
                Session["DiscoUserID"] = DiscoUserID.ToString();

            }
            else
            {
                found = false;
            }
            dr.Close();
            con.Close();
            if (found == true)
            {
                FormsAuthentication.SetAuthCookie(DiscoID, true);
                FormsAuthentication.SetAuthCookie(Username, true);
                FormsAuthentication.SetAuthCookie(Convert.ToInt32(DiscoUserID).ToString(), true);
                Session["Username"] = Username.ToString();
                Session["DiscoID"] = DiscoID.ToString();
                Session["DiscoUserID"] = DiscoUserID.ToString();
                return RedirectToAction("Dashboard", "Disco");
            }
            else
            {
                ViewBag.Error = "Username and Password are wrong!";
            }
            con.Close();
            return View();
        }

        public ActionResult ProcurementLogin()
        {
            return View();
        }
        public ActionResult ProcurementLogin(string Username, string Password)
        {
            string username = "";
            bool found = false;
            SqlDataReader dr;
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "GetProcurementUserByUsername";
            com.CommandType = System.Data.CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Username", Username);
            com.Parameters.AddWithValue("@Password", Password);
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                found = true;
                username = dr["Fullname"].ToString();
                FormsAuthentication.SetAuthCookie(Username, true);
                Session["Fullname"] = Username.ToString();
            }
            else
            {
                found = false;
            }
            dr.Close();
            con.Close();
            if (found == true)
            {

                FormsAuthentication.SetAuthCookie(Username, true);
                Session["Username"] = Username.ToString();
                return RedirectToAction("Dashboard", "Procurement");

            }
            else
            {
                ViewBag.Error = "Username and Password are wrong!";
            }
            con.Close();
            return View();
        }     
    }
}
using MojecFaultyMeter.Config;
using MojecFaultyMeter.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        List<DiscoUser> _discoUsers = new List<DiscoUser>();
        List<MojecStoreUser> _mojecStores = new List<MojecStoreUser>();
        List<FactoryUsers> _factoryusers = new List<FactoryUsers>();
        List<FaultyMeters> _faulty = new List<FaultyMeters>();
        List<DispatchOrders> _dispatchOrders = new List<DispatchOrders>();
        List<ProcurementUsers>  _procurementUsers = new List<ProcurementUsers>();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MojecFaultyMeter"].ConnectionString);
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }

            con.Open();
            SqlCommand cmd = new SqlCommand("select Count(*) from FaultyMeters", con);
            int r = Convert.ToInt32(cmd.ExecuteScalar());
            ViewBag.Total = r;

            SqlCommand cmd1 = new SqlCommand("select Count(*) from FaultyMeters where Status = 'Pending' ", con);
            int r1 = Convert.ToInt32(cmd1.ExecuteScalar());
            ViewBag.Pending = r1;

            SqlCommand cmd2 = new SqlCommand("select Count(*) from FaultyMeters where Status = 'Recieved'", con);
            int r2 = Convert.ToInt32(cmd2.ExecuteScalar());
            ViewBag.Completed = r2;


            SqlCommand cmd3 = new SqlCommand("select Count(*) from FaultyMeters where Status = 'Solved'", con);
            int r3 = Convert.ToInt32(cmd3.ExecuteScalar());
            ViewBag.Solved = r3;

            con.Close();

            return View();


        }
        
        public ActionResult CreateDicos()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            return View();
        }
        public ActionResult CreateMojecstore()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            return View();
        }
        public ActionResult Discos()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
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
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
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
        public ActionResult CreateProcurementUser(ProcurementUsers user)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("InsertProcurementUser", con))
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
        public ActionResult RegisterUser()
        {
            ViewBag.Disco = PopulateDisco();
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
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _discoUsers = new List<DiscoUser>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("getallDiscoUsers", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DiscoUser user = new DiscoUser();
                    user.UserID = Convert.ToInt32(rdr["DiscoUserID"].ToString());
                    user.Fullname = rdr["D_FullName"].ToString();
                    user.Email = rdr["Email"].ToString();
                    user.Username = rdr["UserName"].ToString();
                    user.DiscoAb = rdr["Discosn"].ToString();
                    _discoUsers.Add(user);
                }
                rdr.Close();
            }
            return View(_discoUsers);
        }
        public ActionResult MojecStoreUsers()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _mojecStores = new List<MojecStoreUser>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("getallMojecStoreUsers", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MojecStoreUser user = new MojecStoreUser();
                    user.UserID = Convert.ToInt32(rdr["MojecStoreUserID"].ToString());
                    user.Fullname = rdr["M_FullName"].ToString();
                    user.Email = rdr["Email"].ToString();
                    user.Username = rdr["UserName"].ToString();
                    _mojecStores.Add(user);
                }
                rdr.Close();
            }
            return View(_mojecStores);
        }
        public ActionResult FactoryUsers()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _factoryusers = new List<FactoryUsers>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("getallFactoryUsers", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    FactoryUsers user = new FactoryUsers();
                    user.UserID = Convert.ToInt32(rdr["FactoryUserID"].ToString());
                    user.Fullname = rdr["F_FullName"].ToString();
                    user.Email = rdr["Email"].ToString();
                    user.Username = rdr["UserName"].ToString();
                    _factoryusers.Add(user);
                }
                rdr.Close();
            }
            return View(_factoryusers);
        }
        public ActionResult ProcurementUsers()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _procurementUsers = new List<ProcurementUsers>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetallProcurementUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ProcurementUsers user = new ProcurementUsers();
                    user.UserID = Convert.ToInt32(rdr["ProcurementUserID"].ToString());
                    user.Fullname = rdr["FullName"].ToString();
                    user.Email = rdr["Email"].ToString();
                    user.Username = rdr["UserName"].ToString();
                    
                    _procurementUsers.Add(user);
                }
                rdr.Close();
            }
            return View(_procurementUsers);
        }
        public ActionResult UpdateDiscos()
        {
            return View();
        }
        public ActionResult PendingMeters()
        {

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();
            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetPendingMeterforAdmin", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    FaultyMeters fault = new FaultyMeters();
                    fault.MeterID = Convert.ToInt32(rdr["MeterID"].ToString());
                    fault.CustomerName = rdr["CustomerName"].ToString();
                    fault.MeterNo = rdr["MeterNo"].ToString();
                    fault.Replacementstat = rdr["Replacementstat"].ToString();
                    fault.Status = rdr["Status"].ToString();
                    fault.ReturnDate = rdr["ReturnDate"].ToString();
                    fault.AcceptedBy = rdr["M_Fullname"].ToString();
                    fault.TreatedBy = rdr["F_Fullname"].ToString();
                    fault.DiscoUser = rdr["D_Fullname"].ToString();
                    fault.AccountNo = rdr["AccountNo"].ToString();
                    fault.MeterType = rdr["MeterType"].ToString();
                    fault.WorkOrderID = rdr["WorkOrderID"].ToString();

                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);

        }
        public ActionResult Completecases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();

            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetCompletedMetersForAdmin", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    FaultyMeters fault = new FaultyMeters();
                    fault.MeterID = Convert.ToInt32(rdr["MeterID"].ToString());
                    fault.CustomerName = rdr["CustomerName"].ToString();
                    fault.MeterNo = rdr["MeterNo"].ToString();
                    fault.Replacementstat = rdr["Replacementstat"].ToString();
                    fault.Status = rdr["Status"].ToString();
                    fault.ReturnDate = rdr["ReturnDate"].ToString();
                    fault.AcceptedBy = rdr["M_Fullname"].ToString();
                    fault.TreatedBy = rdr["F_Fullname"].ToString();
                    fault.DiscoUser = rdr["D_Fullname"].ToString();
                    fault.AccountNo = rdr["AccountNo"].ToString();
                    fault.MeterType = rdr["MeterType"].ToString();
                    fault.WorkOrderID = rdr["WorkOrderID"].ToString();

                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }
        public ActionResult Dispatchedcases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();

            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetDispatchedMetersForAdmin", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    FaultyMeters fault = new FaultyMeters();
                    fault.MeterID = Convert.ToInt32(rdr["MeterID"].ToString());
                    fault.CustomerName = rdr["CustomerName"].ToString();
                    fault.MeterNo = rdr["MeterNo"].ToString();
                    fault.Replacementstat = rdr["Replacementstat"].ToString();
                    fault.Status = rdr["Status"].ToString();
                    fault.ReturnDate = rdr["ReturnDate"].ToString();
                    fault.AcceptedBy = rdr["M_Fullname"].ToString();
                    fault.TreatedBy = rdr["F_Fullname"].ToString();
                    fault.DiscoUser = rdr["D_Fullname"].ToString();
                    fault.AccountNo = rdr["AccountNo"].ToString();
                    fault.MeterType = rdr["MeterType"].ToString();
                    fault.WorkOrderID = rdr["WorkOrderID"].ToString();

                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }
        public ActionResult Solvedcases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();
            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetSolvedMeterforAdmin", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    FaultyMeters fault = new FaultyMeters();
                    fault.MeterID = Convert.ToInt32(rdr["MeterID"].ToString());
                    fault.CustomerName = rdr["CustomerName"].ToString();
                    fault.MeterNo = rdr["MeterNo"].ToString();
                    fault.Replacementstat = rdr["Replacementstat"].ToString();
                    fault.Status = rdr["Status"].ToString();
                    fault.ReturnDate = rdr["ReturnDate"].ToString();
                    fault.AcceptedBy = rdr["M_Fullname"].ToString();
                    fault.TreatedBy = rdr["F_Fullname"].ToString();
                    fault.DiscoUser = rdr["D_Fullname"].ToString();
                    fault.AccountNo = rdr["AccountNo"].ToString();
                    fault.MeterType = rdr["MeterType"].ToString();
                    fault.WorkOrderID = rdr["WorkOrderID"].ToString();
                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }
        public ActionResult Replacescases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();
            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetReplacementmetersforAdmin", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    FaultyMeters fault = new FaultyMeters();
                    fault.MeterID = Convert.ToInt32(rdr["MeterID"].ToString());
                    fault.CustomerName = rdr["CustomerName"].ToString();
                    fault.MeterNo = rdr["MeterNo"].ToString();
                    fault.Replacementstat = rdr["Replacementstat"].ToString();
                    fault.Status = rdr["Status"].ToString();
                    fault.ReturnDate = rdr["ReturnDate"].ToString();
                    fault.AcceptedBy = rdr["M_Fullname"].ToString();
                    fault.TreatedBy = rdr["F_Fullname"].ToString();
                    fault.DiscoUser = rdr["D_Fullname"].ToString();
                    fault.AccountNo = rdr["AccountNo"].ToString();
                    fault.MeterType = rdr["MeterType"].ToString();
                    fault.WorkOrderID = rdr["WorkOrderID"].ToString();
                    fault.MeterReplacementNo = rdr["MeterReplacementNo"].ToString();

                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }
        public ActionResult Reparingcases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();
            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetRepairingMeterforAdmin", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    FaultyMeters fault = new FaultyMeters();
                    fault.MeterID = Convert.ToInt32(rdr["MeterID"].ToString());
                    fault.CustomerName = rdr["CustomerName"].ToString();
                    fault.MeterNo = rdr["MeterNo"].ToString();
                    fault.Replacementstat = rdr["Replacementstat"].ToString();
                    fault.Status = rdr["Status"].ToString();
                    fault.ReturnDate = rdr["ReturnDate"].ToString();
                    fault.AcceptedBy = rdr["M_Fullname"].ToString();
                    fault.TreatedBy = rdr["F_Fullname"].ToString();
                    fault.DiscoUser = rdr["D_Fullname"].ToString();
                    fault.AccountNo = rdr["AccountNo"].ToString();
                    fault.MeterType = rdr["MeterType"].ToString();
                    fault.WorkOrderID = rdr["WorkOrderID"].ToString();
                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }
        public ActionResult ReturnWorkorder()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetWorkordersForAdmin", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    FaultyMeters fault = new FaultyMeters();
                    fault.WorkOrderID = rdr["WorkOrderID"].ToString();
                    fault.Disco = rdr["Discosn"].ToString();
                    fault.DiscoUser = rdr["D_FullName"].ToString();
                    fault.ReturnDate = rdr["ReturnDate"].ToString();

                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }
        public ActionResult DispatchedWorkorder()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _dispatchOrders = new List<DispatchOrders>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetDispatchordersForAdmin", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DispatchOrders dispatch = new DispatchOrders();
                    dispatch.DispatchWorkOrder = rdr["Dispatchworkorder"].ToString();
                    dispatch.DispatchDate = Convert.ToDateTime(rdr["DispatchDate"].ToString());
                    dispatch.Disco = rdr["Discosn"].ToString();

                    _dispatchOrders.Add(dispatch);
                }
                rdr.Close();
            }
            return View(_dispatchOrders);
        }
        public ActionResult Acceptedcases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();
            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetAcceptedMeterforAdmin", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    FaultyMeters fault = new FaultyMeters();
                    fault.MeterID = Convert.ToInt32(rdr["MeterID"].ToString());
                    fault.CustomerName = rdr["CustomerName"].ToString();
                    fault.MeterNo = rdr["MeterNo"].ToString();
                    fault.Replacementstat = rdr["Replacementstat"].ToString();
                    fault.Status = rdr["Status"].ToString();
                    fault.ReturnDate = rdr["ReturnDate"].ToString();
                    fault.AcceptedBy = rdr["M_Fullname"].ToString();
                    fault.TreatedBy = rdr["F_Fullname"].ToString();
                    fault.DiscoUser = rdr["D_Fullname"].ToString();
                    fault.AccountNo = rdr["AccountNo"].ToString();
                    fault.MeterType = rdr["MeterType"].ToString();
                    fault.WorkOrderID = rdr["WorkOrderID"].ToString();
                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }
        public ActionResult Rejectcases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();
            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetRejectedMeterforAdmin", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    FaultyMeters fault = new FaultyMeters();
                    fault.MeterID = Convert.ToInt32(rdr["MeterID"].ToString());
                    fault.CustomerName = rdr["CustomerName"].ToString();
                    fault.MeterNo = rdr["MeterNo"].ToString();
                    fault.Replacementstat = rdr["Replacementstat"].ToString();
                    fault.Status = rdr["Status"].ToString();
                    fault.ReturnDate = rdr["ReturnDate"].ToString();
                    fault.AcceptedBy = rdr["M_Fullname"].ToString();
                    fault.TreatedBy = rdr["F_Fullname"].ToString();
                    fault.DiscoUser = rdr["D_Fullname"].ToString();
                    fault.AccountNo = rdr["AccountNo"].ToString();
                    fault.MeterType = rdr["MeterType"].ToString();
                    fault.WorkOrderID = rdr["WorkOrderID"].ToString();
                    fault.Rejectcomment = rdr["RejectionComment"].ToString();
                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }
        public ActionResult AwaitingDispatch()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();
            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetawaitingDispatchcases", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    FaultyMeters fault = new FaultyMeters();
                    fault.MeterID = Convert.ToInt32(rdr["MeterID"].ToString());
                    fault.CustomerName = rdr["CustomerName"].ToString();
                    fault.MeterNo = rdr["MeterNo"].ToString();
                    fault.Replacementstat = rdr["Replacementstat"].ToString();
                    fault.Status = rdr["Status"].ToString();
                    fault.ReturnDate = rdr["ReturnDate"].ToString();
                    fault.AcceptedBy = rdr["M_Fullname"].ToString();
                    fault.TreatedBy = rdr["F_Fullname"].ToString();
                    fault.DiscoUser = rdr["D_Fullname"].ToString();
                    fault.AccountNo = rdr["AccountNo"].ToString();
                    fault.MeterType = rdr["MeterType"].ToString();
                    fault.WorkOrderID = rdr["WorkOrderID"].ToString();
                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }
        public ActionResult AwaitingReplacement()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();
            _faulty = new List<FaultyMeters>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetawaitingReplacementcases", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    FaultyMeters fault = new FaultyMeters();
                    fault.MeterID = Convert.ToInt32(rdr["MeterID"].ToString());
                    fault.CustomerName = rdr["CustomerName"].ToString();
                    fault.MeterNo = rdr["MeterNo"].ToString();
                    fault.Replacementstat = rdr["Replacementstat"].ToString();
                    fault.Status = rdr["Status"].ToString();
                    fault.ReturnDate = rdr["ReturnDate"].ToString();
                    fault.AcceptedBy = rdr["M_Fullname"].ToString();
                    fault.TreatedBy = rdr["F_Fullname"].ToString();
                    fault.DiscoUser = rdr["D_Fullname"].ToString();
                    fault.AccountNo = rdr["AccountNo"].ToString();
                    fault.MeterType = rdr["MeterType"].ToString();
                    fault.WorkOrderID = rdr["WorkOrderID"].ToString();
                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }
        public ActionResult ActivateDiscouser(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("ActivateDiscoUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoUserID", Id);
            }
            TempData["save"] = "Disco user has been activated successfully";
            return RedirectToAction("DiscoUsers");
        }
        public ActionResult DeactivateDiscouser(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("DeactivateDiscoUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoUserID", Id);
            }
            TempData["save"] = "Disco user has been Deactivated successfully";
            return RedirectToAction("DiscoUsers");
        }
        public ActionResult ActivateFactoryuser(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("ActivateFactoryUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FactoryUserID", Id);
            }
            TempData["save"] = "Factory user has been activated successfully";
            return RedirectToAction("FactoryUsers");
        }
        public ActionResult DeactivateFactoryuser(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("DeactivateFactoryUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FactoryUserID", Id);
            }
            TempData["save"] = "Factory user has been Deactivated successfully";
            return RedirectToAction("FactoryUsers");
        }
        public ActionResult ActivateStoreuser(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("ActivateMojecStoreUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MojecStoreUserID ", Id);
            }
            TempData["save"] = "Mojec store user has been activated successfully";
            return RedirectToAction("MojecStoreUsers");
        }
        public ActionResult DeactivateStoreuser(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("DeactivateMojecStoreUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MojecStoreUserID ", Id);
            }
            TempData["save"] = "Mojec store user has been Deactivated successfully";
            return RedirectToAction("MojecStoreUsers");
        }
        public ActionResult ActivateProcurementuser(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("ActivateProcurementUsers", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProcurementUserID", Id);
            }
            TempData["save"] = "Procurement user has been activated successfully";
            return RedirectToAction("ProcurementUsers");
        }
        public ActionResult DeactivateProcurementuser(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("DeactivateProcurementUserss", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProcurementUserID", Id);
            }
            TempData["save"] = "Procurement user has been Deactivated successfully";
            return RedirectToAction("ProcurementUsers");
        }
        private static List<Discos> PopulateDisco()
        {
            List<Discos> discos = new List<Discos>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {

                using (SqlCommand cmd = new SqlCommand("select * from  Disco", con))
                {
                    cmd.Connection = con;

                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            discos.Add(
                                new Discos
                                {
                                    DiscoID = Convert.ToInt32(sdr["DiscoID"]),
                                    DiscoAB = sdr["Discosn"].ToString()
                                }

                                );
                        }
                        con.Close();
                    }


                }

                return discos;

            }
        }
        public ActionResult GetMeterDetails(string Id)
        {
            return View();
        }
        public ActionResult DownloadCompletedcases(string date1,string date2)
        {
            return Redirect("http://mojecdataapi.azurewebsites.net/api/CompletedFile/Download?date1=" + date1 + "&&date2=" + date2);          
        }
        public ActionResult DownloadRejectedcases(string date1,string date2)
        {
            return Redirect("http://mojecdataapi.azurewebsites.net/api/RejectedFile/Download?date1=" + date1 + "&&date2=" + date2);
        }
        public ActionResult DownloadReplacedcases(string date1, string date2)
        {
            return Redirect("http://mojecdataapi.azurewebsites.net/api/ReplacedFile/Download?date1=" + date1 + "&&date2=" + date2);
        }













    }
}
using Mailjet.Client;
using Mailjet.Client.Resources;
using MojecFaultyMeter.Config;
using MojecFaultyMeter.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MojecFaultyMeter.Controllers
{
    public class ProcurementController : Controller
    {
        List<FaultyMeters> _faulty = new List<FaultyMeters>();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MojecFaultyMeter"].ConnectionString);
        // GET: Procurement
        public ActionResult Dashboard()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
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

            return View();
        }
        public ActionResult PendingMeters()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
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

        [HttpPost]
        public ActionResult PendingMeters(string from, string to, string discoId)
        {

            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();
            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand($"Select * from FaultyMeters f full join Disco d on f.DiscoID = d.DiscoID full join DiscoUsers du on f.DiscoUserID= du.DiscoUserID full join MojecStoreUser mu on f.AcceptedBy = mu.MojecStoreUserID full join FactoryUser fu on f.TreatedBy = fu.FactoryUserID where f.Status = 'Pending' and f.Daterecieved between '{from}' and '{to}' and d.DiscoID = {discoId}", con);
                cmd.CommandType = System.Data.CommandType.Text;

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
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
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

        [HttpPost]
        public ActionResult Completecases(string from, string to, string discoId)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();

            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand($"Select * from FaultyMeters f full join Disco d on f.DiscoID = d.DiscoID full join DiscoUsers du on f.DiscoUserID= du.DiscoUserID full join MojecStoreUser mu on f.AcceptedBy = mu.MojecStoreUserID full join FactoryUser fu on f.TreatedBy = fu.FactoryUserID where f.Status = 'Recieved' and f.Daterecieved between '{from}' and '{to}' and d.DiscoID = {discoId}", con);
                cmd.CommandType = System.Data.CommandType.Text;

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
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
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
        [HttpPost]
        public ActionResult Solvedcases(string from, string to, string discoId)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();
            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand($"Select * from FaultyMeters f full join Disco d on f.DiscoID = d.DiscoID full join DiscoUsers du on f.DiscoUserID= du.DiscoUserID full join MojecStoreUser mu on f.AcceptedBy = mu.MojecStoreUserID full join FactoryUser fu on f.TreatedBy = fu.FactoryUserID where f.Status = 'Solved' and f.Daterecieved between '{from}' and '{to}' and d.DiscoID = {discoId}", con);
                cmd.CommandType = System.Data.CommandType.Text;

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
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
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
        [HttpPost]
        public ActionResult Replacescases(string from, string to, string discoId)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();
            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand($"Select * from FaultyMeters f full join Disco d on f.DiscoID = d.DiscoID full join DiscoUsers du on f.DiscoUserID= du.DiscoUserID full join MojecStoreUser mu on f.AcceptedBy = mu.MojecStoreUserID full join FactoryUser fu on f.TreatedBy = fu.FactoryUserID where f.Replacementstat = 'Yes' and f.Daterecieved between '{from}' and '{to}' and d.DiscoID = {discoId}", con);
                cmd.CommandType = System.Data.CommandType.Text;


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
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
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

        [HttpPost]
        public ActionResult Reparingcases(string from, string to, string discoId)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();
            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand($"Select * from FaultyMeters f full join Disco d on f.DiscoID = d.DiscoID full join DiscoUsers du on f.DiscoUserID= du.DiscoUserID full join MojecStoreUser mu on f.AcceptedBy = mu.MojecStoreUserID full join FactoryUser fu on f.TreatedBy = fu.FactoryUserID where f.Status = 'Repairing' and f.Daterecieved between '{from}' and '{to}' and d.DiscoID = {discoId}", con);
                cmd.CommandType = System.Data.CommandType.Text;
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
        public ActionResult Acceptedcases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
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

        [HttpPost]
        public ActionResult Acceptedcases(string from, string to, string discoId)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();
            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand($"Select * from FaultyMeters f full join Disco d on f.DiscoID = d.DiscoID full join DiscoUsers du on f.DiscoUserID= du.DiscoUserID full join MojecStoreUser mu on f.AcceptedBy = mu.MojecStoreUserID full join FactoryUser fu on f.TreatedBy = fu.FactoryUserID where f.Status = 'Accepted' and f.Daterecieved between '{from}' and '{to}' and d.DiscoID = {discoId}", con);
                cmd.CommandType = System.Data.CommandType.Text;
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
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
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

        [HttpPost]
        public ActionResult Rejectcases(string from, string to, string discoId)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();
            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand($"Select * from FaultyMeters f full join Disco d on f.DiscoID = d.DiscoID full join DiscoUsers du on f.DiscoUserID= du.DiscoUserID full join MojecStoreUser mu on f.AcceptedBy = mu.MojecStoreUserID full join FactoryUser fu on f.TreatedBy = fu.FactoryUserID where f.Status = 'Rejected' and f.Daterecieved between '{from}' and '{to}' and d.DiscoID = {discoId}", con);
                cmd.CommandType = System.Data.CommandType.Text;

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
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
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

        [HttpPost]
        public ActionResult AwaitingDispatch(string from, string to, string discoId)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();
            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand($"Select * from FaultyMeters f full join Disco d on f.DiscoID = d.DiscoID full join DiscoUsers du on f.DiscoUserID= du.DiscoUserID full join MojecStoreUser mu on f.AcceptedBy = mu.MojecStoreUserID full join FactoryUser fu on f.TreatedBy = fu.FactoryUserID where f.Status = 'Awaiting Dispatch' and f.Daterecieved between '{from}' and '{to}' and d.DiscoID = {discoId}", con);
                cmd.CommandType = System.Data.CommandType.Text;

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
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
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

        [HttpPost]
        public ActionResult AwaitingReplacement(string from, string to, string discoId)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();
            _faulty = new List<FaultyMeters>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand($"Select * from FaultyMeters f full join Disco d on f.DiscoID = d.DiscoID full join DiscoUsers du on f.DiscoUserID= du.DiscoUserID full join MojecStoreUser mu on f.AcceptedBy = mu.MojecStoreUserID full join FactoryUser fu on f.TreatedBy = fu.FactoryUserID where f.Status = 'Awaiting Replacement' and f.ReplacementApproval = 'Yes' and f.Daterecieved between '{from}' and '{to}' and d.DiscoID = {discoId}", con);
                cmd.CommandType = System.Data.CommandType.Text;
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

        [HttpPost]
        public ActionResult Dispatchedcases(string from, string to, string discoId)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Disco = PopulateDisco();

            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand($"Select * from FaultyMeters f full join Disco d on f.DiscoID = d.DiscoID full join DiscoUsers du on f.DiscoUserID= du.DiscoUserID full join MojecStoreUser mu on f.AcceptedBy = mu.MojecStoreUserID full join FactoryUser fu on f.TreatedBy = fu.FactoryUserID where f.Status = 'Dispatched' and f.Daterecieved between '{from}' and '{to}' and d.DiscoID = {discoId}", con);
                cmd.CommandType = System.Data.CommandType.Text;

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

        public async Task<ActionResult> ApproveMeterReplacement(int Id)
        {
            string storeID = (string)Session["UserID"];
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("UpdateForStoreReplacemetApproval", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeterID", Id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            string Email = "";
            using (SqlCommand cmd4 = new SqlCommand("select * from MojecStoreUser", con))
            {
                con.Open();
                SqlDataReader dr = cmd4.ExecuteReader();
                while (dr.Read())
                {
                    Email = dr["Email"].ToString();
                    MailjetClient client = new MailjetClient("a8d83ddfc6afa0d27997cfff564176db", "bd5afa8d85e11465d2f1319c7038286f");
                    MailjetRequest request = new MailjetRequest
                    {
                        Resource = Send.Resource,
                    }
                    .Property(Send.FromEmail, "faultymeters@mojec.com")
                    .Property(Send.FromName, "Mojec")
                    .Property(Send.To, Email)
                    .Property(Send.Subject, "Mojec Faulty Meter")
                    .Property(Send.TextPart, "A new Faulty Meter has Just been approved for replacement. Please Review");
                    MailjetResponse response = await client.PostAsync(request);
                }
            }
            TempData["save"] = "Meter replacement approved successfully";
            return RedirectToAction("AwaitingReplacement");
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
        public ActionResult DownloadCompletedcases(string date1, string date2)
        {
            return Redirect("http://mojecdataapi.azurewebsites.net/api/CompletedFile/Download?date1=" + date1 + "&&date2=" + date2);
        }
        public ActionResult DownloadRejectedcases(string date1, string date2)
        {
            return Redirect("http://mojecdataapi.azurewebsites.net/api/RejectedFile/Download?date1=" + date1 + "&&date2=" + date2);
        }
        public ActionResult DownloadReplacedcases(string date1, string date2)
        {
            return Redirect("http://mojecdataapi.azurewebsites.net/api/ReplacedFile/Download?date1=" + date1 + "&&date2=" + date2);
        }
    }
}
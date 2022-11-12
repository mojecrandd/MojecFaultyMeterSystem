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
using System.Web.Security;

namespace MojecFaultyMeter.Controllers
{
    public class StoreController : Controller
    {
        List<FaultyMeters> _faulty = new List<FaultyMeters>();
        List<DispatchOrders> _dispatchOrders = new List<DispatchOrders>();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MojecFaultyMeter"].ConnectionString);
        // GET: Store
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

            con.Close();


            return View();
        }
        public ActionResult Workorders()
        {
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
                    

                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }
        public ActionResult DispatchWorkOrder()
        {
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
        public ActionResult Replacescases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _faulty = new List<FaultyMeters>();
            ViewBag.Disco = PopulateDisco();

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
                SqlCommand cmd = new SqlCommand("GetAwaitingStoreReplacementcases", con);
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

        public ActionResult Replacemeter(int Id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            FaultyMeters faulty = new FaultyMeters();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetMeterDetails", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@MeterID", Id);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    faulty.MeterID = Convert.ToInt32(rdr["MeterID"].ToString());

                }
            }
            return View(faulty);
        }
        [HttpPost]
        public async Task<ActionResult> Replacemeter(FaultyMeters faulty)
        {
            string USERID = (string)Session["UserID"];
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("UpdateSp_rplceStoreStatus", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeterID", faulty.MeterID);
                cmd.Parameters.AddWithValue("@ReplacedBy", USERID);
                cmd.Parameters.AddWithValue("@MeterReplacementNo", faulty.MeterReplacementNo);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            string Email = "";
            using (SqlCommand cmd4 = new SqlCommand("select * from ProcurementUsers", con))
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
                    .Property(Send.TextPart, $"A new Faulty Meter has been replace [New Meter No {faulty.MeterReplacementNo} . Please Review");
                    MailjetResponse response = await client.PostAsync(request);
                }
            }
            TempData["save"] = "Meter Replaced";
            return RedirectToAction("AwaitingReplacement");
        }
        public ActionResult Dispatchedcases()
        {
             if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
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
        public ActionResult GetworkorderList(string id)
        {

            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }

            _faulty = new List<FaultyMeters>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetWorkorderDetailsForDisco", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WorkorderID", id);
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

                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }
        public async Task<ActionResult> AcceptMetercase(int Id)
        {
            string storeID = (string)Session["UserID"];
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("UpdateSp_AccptdStoreStatus", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeterID", Id);
                cmd.Parameters.AddWithValue("@AcceptedBy", storeID );
                con.Open();
                cmd.ExecuteNonQuery();
            }
            string Email = "";
            using (SqlCommand cmd4 = new SqlCommand("select * from FactoryUser", con))
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
                    .Property(Send.TextPart, "A new Faulty Meter has Just been accepted by Mojec Store. Please Review");
                    MailjetResponse response = await client.PostAsync(request);
                }

            }


            TempData["save"] = "Meter Accepted successfully";
            return RedirectToAction("PendingMeters");


        }
        public ActionResult RejectMetercase(int Id)
        {
            FaultyMeters faulty = new FaultyMeters();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetMeterDetails", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@MeterID", Id);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    faulty.MeterID = Convert.ToInt32(rdr["MeterID"].ToString());

                }
            }

  
            return View(faulty);
           
        }
        [HttpPost]
        public async Task<ActionResult> RejectMetercase(FaultyMeters meters)
        {

            string storeID = (string)Session["UserID"];
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("UpdateSp_RjctddStoreStatus", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeterID", meters.MeterID);
                cmd.Parameters.AddWithValue("@AcceptedBy", storeID);
                cmd.Parameters.AddWithValue("@Rejectedcomment", meters.Rejectcomment);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            string Email = "";
            using (SqlCommand cmd4 = new SqlCommand("select * from FaultyMeters f inner join DiscoUsers d on f.DiscoUserID = d.DiscoUserID where f.MeterID = '" + meters.MeterID+ "' ", con))
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
                    .Property(Send.TextPart, "A new Faulty Meter has Just been Rejected by Mojec Store. Please Review");
                    MailjetResponse response = await client.PostAsync(request);
                }

            }

            TempData["save"] = "Meter Rejected";
            return RedirectToAction("PendingMeters");
        }
        public ActionResult Confirmsolved(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("UpdateSp_AwaitingStoreStatus", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeterID", Id);
                con.Open();
                cmd.ExecuteNonQuery();
            }

           
           

            TempData["save"] = "Meter set for dispatch successfully";
            return RedirectToAction("Solvedcases");
        }
       
        public async Task<ActionResult> Dispatchmeter(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("UpdateSp_Dispatched", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeterID", Id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            string Email = "";
            using (SqlCommand cmd4 = new SqlCommand("select * from FaultyMeters f inner join DiscoUsers d on f.DiscoUserID = d.DiscoUserID where f.MeterID = '" + Id + "' ", con))
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
                    .Property(Send.TextPart, "A new Faulty Meter has Just been Dispatched to Disco. Please Review");
                    MailjetResponse response = await client.PostAsync(request);
                }

            }


            TempData["save"] = "Meter dispatched successfully";
            return RedirectToAction("AwaitingDispatch");
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
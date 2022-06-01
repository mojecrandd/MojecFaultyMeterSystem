using MojecFaultyMeter.Config;
using MojecFaultyMeter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using Mailjet.Client;
using Mailjet.Client.Resources;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.OleDb;
using System.IO;

namespace MojecFaultyMeter.Controllers
{
    public class DiscoController : Controller
    {
        List<FaultyMeters> _faulty = new List<FaultyMeters>();
        List<DispatchOrders> _dispatchOrders = new List<DispatchOrders>();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MojecFaultyMeter"].ConnectionString);
        // GET: Disco
        public ActionResult Dashboard()
        {
            string DiscoID = (string)Session["DiscoID"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }

            con.Open();
            SqlCommand cmd = new SqlCommand("select Count(*) from FaultyMeters where DiscoID = '"+DiscoID+"' ", con);
            int r = Convert.ToInt32(cmd.ExecuteScalar());
            ViewBag.Total = r;

            SqlCommand cmd1 = new SqlCommand("select Count(*) from FaultyMeters where Status = 'Pending' and DiscoID = '" + DiscoID + "' ", con);
            int r1 = Convert.ToInt32(cmd1.ExecuteScalar());
            ViewBag.Pending = r1;

            SqlCommand cmd2 = new SqlCommand("select Count(*) from FaultyMeters where Status = 'Recieved' and DiscoID = '" + DiscoID + "'", con);
            int r2 = Convert.ToInt32(cmd2.ExecuteScalar());
            ViewBag.Completed = r2;


            SqlCommand cmd3 = new SqlCommand("select Count(*) from FaultyMeters where Status = 'Solved' and DiscoID = '" + DiscoID + "'", con);
            int r3 = Convert.ToInt32(cmd3.ExecuteScalar());
            ViewBag.Solved = r3;

            con.Close();

            return View();
        }
        public ActionResult AddFaultyMeters()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            ViewBag.Fault = PopulateFault();
            ViewBag.Model = PopulateModel();
            ViewBag.MeterType = PopulateMeterType();
            return View();
        }
        [HttpPost]
        public ActionResult AddFaultyMeters(FaultyMeters faulty)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            string DiscoID = (string)Session["DiscoID"];
            string DiscoUserID = (string)Session["DiscoUserID"];
            faulty.UserID = DiscoUserID;
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("Addfaultymeterlog", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeterNo", faulty.MeterNo);
                    cmd.Parameters.AddWithValue("@CustomerName", faulty.CustomerName);
                    cmd.Parameters.AddWithValue("@AccountNo", faulty.AccountNo);
                    cmd.Parameters.AddWithValue("@DiscoID", DiscoID);
                    cmd.Parameters.AddWithValue("@Tariff", faulty.Tarriff);
                    cmd.Parameters.AddWithValue("@MeterType", faulty.MeterType);
                    cmd.Parameters.AddWithValue("@Model", faulty.Model);
                    cmd.Parameters.AddWithValue("@Fault", faulty.Fault);
                    cmd.Parameters.AddWithValue("@DiscoUserID", DiscoUserID);
                    cmd.Parameters.AddWithValue("@FaultyComment", faulty.Faultcomment);
                    cmd.Parameters.AddWithValue("@WorkOrderType", faulty.Faultcomment);
                    cmd.Parameters.AddWithValue("@DateRecieved", faulty.Faultcomment);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Meter has been added to list";
            return RedirectToAction("AddFaultyMeters");
        }

        public ActionResult AddFaultyMetersOnsite()
        {
            
            return View();
            
        }
        [HttpGet]
        public ActionResult ReturnWorkOrder()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _faulty = new List<FaultyMeters>();
            string DiscoID = (string)Session["DiscoID"];
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetReturnordersForDiscoUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoID", DiscoID);
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
        public ActionResult DispatchedWorkOrder()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _dispatchOrders = new List<DispatchOrders>();

            string DiscoID = (string)Session["DiscoID"];

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetDispatchordersForDiscoUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoID", DiscoID);
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
        public ActionResult Completedcases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _faulty = new List<FaultyMeters>();

            string DiscoID = (string)Session["DiscoID"];

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetCompletedMeterperDisco", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoID", DiscoID);
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
                    fault.MeterType = rdr["WorkOrderID"].ToString();

                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }
        public ActionResult Solvedcases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _faulty = new List<FaultyMeters>();
            string DiscoID = (string)Session["DiscoID"];
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetSolvedMeterperDisco", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoID", DiscoID);
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
        public ActionResult Pendingcases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _faulty = new List<FaultyMeters>();
            string DiscoID = (string)Session["DiscoID"];
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetPendingMeterperDisco", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoID", DiscoID);
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
        public ActionResult Replacescases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _faulty = new List<FaultyMeters>();
            string DiscoID = (string)Session["DiscoID"];
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetReplacementMetersperDisco", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoID", DiscoID);

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
                    fault.MeterReplacementNo = rdr["MeterReplacementNo"].ToString();
                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }
        public ActionResult Reparingcases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _faulty = new List<FaultyMeters>();
            string DiscoID = (string)Session["DiscoID"];
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetRepairingMeterperDisco", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoID", DiscoID);

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
        public ActionResult Acceptedcases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _faulty = new List<FaultyMeters>();
            string DiscoID = (string)Session["DiscoID"];
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetAcceptedMeterperDisco", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoID", DiscoID);
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
        public ActionResult Rejectedcases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _faulty = new List<FaultyMeters>();
            string DiscoID = (string)Session["DiscoID"];
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetRejectedMeterperDisco", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoID", DiscoID);
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
                    fault.Rejectcomment = rdr["RejectionComment"].ToString();
                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }
        public ActionResult ConfirmList()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _faulty = new List<FaultyMeters>();
            string DiscoID = (string)Session["DiscoID"];
            string DiscoUserID = (string)Session["DiscoUserID"];
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetAllCaseForWorkOrder", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoID", DiscoID);
                cmd.Parameters.AddWithValue("@DiscoUserID", DiscoUserID);

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
                    fault.DiscoUser = rdr["D_Fullname"].ToString();
                    fault.AccountNo = rdr["AccountNo"].ToString();
                    fault.MeterType = rdr["MeterType"].ToString();

                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }
        public async Task<ActionResult> ConfirmWorkOrder()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            string DiscoID = (string)Session["DiscoID"];
            string DiscoUserID = (string)Session["DiscoUserID"];
            FaultyMeters faulty = new FaultyMeters();
            string ID = faulty.WorkOrderID;
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateWorkorder", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@WorkorderID", faulty.WorkOrderID);
                    cmd.Parameters.AddWithValue("@DiscoID", DiscoID);
                    cmd.Parameters.AddWithValue("@DiscoUserID", DiscoUserID);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();

                    
                }



                using (SqlCommand cmd2 = new SqlCommand("UpdateReturnDate", con))
                {
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@WorkorderID", ID);                  
                    cmd2.Parameters.AddWithValue("@ReturnDate", DateTime.UtcNow);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd2.ExecuteNonQuery();
                }

                using (SqlCommand cmd3 = new SqlCommand("Update FaultyMeters set Status = 'Pending' where WorkOrderID = '"+ID+"'",con))
                {
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd3.ExecuteNonQuery();
                }

                using (SqlCommand cmd9 = new SqlCommand("Update FaultyMeters set Status='Accepted' where WorkOrderType='Onsite'", con))
                {
                    cmd9.CommandType = CommandType.Text;
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd9.ExecuteNonQuery();


                }
                string Email = "";
                using (SqlCommand cmd4 = new SqlCommand("select * from MojecStoreUser",con))
                {
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
                        .Property(Send.Subject,"Mojec Faulty Meter")
                        .Property(Send.TextPart, "A new Workorder "+faulty.WorkOrderID+" has just been sent out to Mojec Store. Please review");
                        MailjetResponse response = await client.PostAsync(request);
                    }

                }


                con.Close();
            }
            TempData["save"] = "Workorder has be created successfully";
            return RedirectToAction("AddFaultyMeters");
        }
        public ActionResult AwaitingDispatch()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _faulty = new List<FaultyMeters>();
            string DiscoID = (string)Session["DiscoID"];
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetawaitingDispatchcasesForDisco", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoID", DiscoID);
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
        public ActionResult AwaitingReplacement()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _faulty = new List<FaultyMeters>();
            string DiscoID = (string)Session["DiscoID"];
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetawaitingReplacementcasesDisco", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoID", DiscoID);
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

        public ActionResult Dispatchedcases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }


            string DiscoID = (string)Session["DiscoID"];
            _faulty = new List<FaultyMeters>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetDispatchedMeterperDiscoAdmin", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoID", DiscoID);
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


        [HttpGet]
        public ActionResult FaultyMeterDetails(int Id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            string DiscoUserID = (string)Session["DiscoUserID"];
            string DiscoID = (string)Session["DiscoID"];
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
                    faulty.MeterNo = rdr["MeterNo"].ToString();
                    faulty.CustomerName = rdr["CustomerName"].ToString();
                    faulty.AccountNo = rdr["AccountNo"].ToString();
                    faulty.DiscoID = Convert.ToInt32(DiscoID.ToString());
                    faulty.Tarriff = rdr["Tariff"].ToString();
                    faulty.MeterType = rdr["MeterType"].ToString();
                    faulty.Model = rdr["Model"].ToString();
                    faulty.Fault = rdr["Fault"].ToString();
                    faulty.Faultcomment = rdr["FaultyComment"].ToString();
                    faulty.DiscoUserID = Convert.ToInt32(DiscoUserID.ToString());
                }
                rdr.Close();
            }
            return View(faulty);
        }
        [HttpPost]
        public ActionResult FaultyMeterDetails(FaultyMeters faulty)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            string DiscoID = (string)Session["DiscoID"];
            string DiscoUserID = (string)Session["DiscoUserID"];
            faulty.UserID = DiscoUserID;
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("Updatefaultymeterlog", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeterID", faulty.MeterID);
                    cmd.Parameters.AddWithValue("@MeterNo", faulty.MeterNo);
                    cmd.Parameters.AddWithValue("@CustomerName", faulty.CustomerName);
                    cmd.Parameters.AddWithValue("@AccountNo", faulty.AccountNo);
                    cmd.Parameters.AddWithValue("@DiscoID", DiscoID);
                    cmd.Parameters.AddWithValue("@Tariff", faulty.Tarriff);
                    cmd.Parameters.AddWithValue("@MeterType", faulty.MeterType);
                    cmd.Parameters.AddWithValue("@Model", faulty.Model);
                    cmd.Parameters.AddWithValue("@Fault", faulty.Fault);
                    cmd.Parameters.AddWithValue("@DiscoUserID", DiscoUserID);
                    cmd.Parameters.AddWithValue("@FaultyComment", faulty.Faultcomment);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Meter has been added to list";
            return RedirectToAction("ConfirmList");
        }
        [HttpGet]
        public ActionResult GetOrderDetails(string id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["DiscoID"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _faulty = new List<FaultyMeters>();
            string DiscoID = (string)Session["DiscoID"];
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

        public ActionResult ConfirmRecieved(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("UpdateSp_RecievedStoreStatus", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeterID", Id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Meter Recieved successfully";
            return RedirectToAction("Dispatchedcases");
        }

        OleDbConnection Econ;
        private void ExcelConn(string FilePath)
        {
            string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", FilePath);
            Econ = new OleDbConnection(constr);
        }
        public ActionResult ReturnUploadWorkorder()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ReturnUploadWorkorder(HttpPostedFileBase file)
        {
            
            string DiscoID = (string)Session["DiscoID"];
            string DiscoUserID = (string)Session["DiscoUserID"];
            string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string filepath = "/excelfolder/" + filename;
            file.SaveAs(Path.Combine(Server.MapPath("/excelfolder/"), filename));
            string fullpath = Server.MapPath("/excelfolder/") + filename;
            ExcelConn(fullpath);
            string Query = string.Format("Select * from [{0}]", "Sheet1$");
            OleDbCommand Ecom = new OleDbCommand(Query, Econ);
            Econ.Open();
            OleDbDataReader dr = Ecom.ExecuteReader();
            DataSet ds = new DataSet();
            OleDbDataAdapter oda = new OleDbDataAdapter(Query, Econ);
            Econ.Close();
            oda.Fill(ds);
            DataTable dt = ds.Tables[0];
            SqlBulkCopy objbulk = new SqlBulkCopy(con);
            objbulk.DestinationTableName = "FaultyMeters";
            objbulk.ColumnMappings.Add("MeterNo", "MeterNo");
            objbulk.ColumnMappings.Add("Customer", "CustomerName");
            objbulk.ColumnMappings.Add("AccountNo", "AccountNo");
            objbulk.ColumnMappings.Add("Tariff", "Tariff");
            objbulk.ColumnMappings.Add("MeterType ", "MeterType");
            objbulk.ColumnMappings.Add("Model", "Model");
            objbulk.ColumnMappings.Add("Fault", "Fault");
            objbulk.ColumnMappings.Add("FaultyComment", "FaultyComment");
            objbulk.ColumnMappings.Add("BU", "BU");
            objbulk.ColumnMappings.Add("DateReceived", "Daterecieved");
            objbulk.ColumnMappings.Add("DiscoID", "DiscoID");
            objbulk.ColumnMappings.Add("DiscoUserID", "DiscoUserID");
            objbulk.ColumnMappings.Add("WorkOrderType", "WorkOrderType");
            con.Open();
            objbulk.WriteToServer(dt);
            con.Close();
            TempData["save"] = "Upload successful";
            return View();
        }

        private static List<Fault> PopulateFault()
        {
            List<Fault> fault = new List<Fault>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {

                using (SqlCommand cmd = new SqlCommand("select * from  Fault_Tbl", con))
                {
                    cmd.Connection = con;

                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            fault.Add(
                                new Fault
                                {
                                    FaultID = Convert.ToInt32(sdr["FaultID"]),
                                    Faultname = sdr["Fault"].ToString()
                                }

                                );
                        }
                        con.Close();
                    }


                }

                return fault;

            }
        }

        private static List<MeterModel> PopulateModel()
        {
            List<MeterModel> model = new List<MeterModel>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {

                using (SqlCommand cmd = new SqlCommand("select * from  MeterModel", con))
                {
                    cmd.Connection = con;

                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            model.Add(
                                new MeterModel
                                {
                                    ModelID = Convert.ToInt32(sdr["ModelID"]),
                                    Modelname = sdr["Model"].ToString()
                                }

                                );
                        }
                        con.Close();
                    }


                }

                return model;

            }
        }

        private static List<MeterType> PopulateMeterType()
        {
            List<MeterType> meter = new List<MeterType>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {

                using (SqlCommand cmd = new SqlCommand("select * from  MeterType", con))
                {
                    cmd.Connection = con;

                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            meter.Add(
                                new MeterType
                                {
                                    MetertypeID = Convert.ToInt32(sdr["MeterTypeID"]),
                                    MetertypeName = sdr["MeterType"].ToString()
                                }

                                );
                        }
                        con.Close();
                    }


                }

                return meter;

            }
        }
        public ActionResult DownloadCompletedcases(string date1, string date2)
        {
            string DiscoID = (string)Session["DiscoID"];
            return Redirect("http://mojecdataapi.azurewebsites.net/api/CompletedDiscoFile/Download?date1=" + date1 + "&&date2=" + date2+"&&id="+DiscoID);
        }
        public ActionResult DownloadRejectedcases(string date1, string date2)
        {
            string DiscoID = (string)Session["DiscoID"];
            return Redirect("http://mojecdataapi.azurewebsites.net/api/RejectedDiscoFile/Download?date1=" + date1 + "&&date2=" + date2+"&&id="+DiscoID);
        }
        public ActionResult DownloadReplacedcases(string date1, string date2)
        {
            string DiscoID = (string)Session["DiscoID"];
            return Redirect("http://mojecdataapi.azurewebsites.net/api/ReplacedDiscoFile/Download?date1=" + date1 + "&&date2=" + date2+"&&id="+DiscoID);
        }


        
    }
}     
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
    public class DiscoController : Controller
    {
        List<FaultyMeters> _faulty = new List<FaultyMeters>();
        List<DispatchOrders> _dispatchOrders = new List<DispatchOrders>();

        // GET: Disco

        public ActionResult Dashboard()
        {

            return View();
        }
        public ActionResult AddFaultyMeters()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddFaultyMeters(FaultyMeters faulty)
        {
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
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Meter has been added to list";
            return RedirectToAction("AddFaultyMeters");
        }
        [HttpGet]
        public ActionResult ReturnWorkOrder()
        {
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
                    fault.ReturnDate = Convert.ToDateTime(rdr["ReturnDate"].ToString());

                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }
        public ActionResult DispatchedWorkOrder()
        {
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
                    fault.ReturnDate = Convert.ToDateTime(rdr["ReturnDate"].ToString());
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
        public ActionResult Solvedcases()
        {
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
                    fault.ReturnDate = Convert.ToDateTime(rdr["ReturnDate"].ToString());
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
                    fault.ReturnDate = Convert.ToDateTime(rdr["ReturnDate"].ToString());
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
                    fault.ReturnDate = Convert.ToDateTime(rdr["ReturnDate"].ToString());
                    fault.AcceptedBy = rdr["M_Fullname"].ToString();
                    fault.TreatedBy = rdr["F_Fullname"].ToString();
                    fault.DiscoUser = rdr["D_.Fullname"].ToString();
                    fault.AccountNo = rdr["AccountNo"].ToString();
                    fault.MeterType = rdr["MeterType"].ToString();

                    _faulty.Add(fault);
                }
                rdr.Close();
            }
            return View(_faulty);
        }       
        public ActionResult Reparingcases()
        {
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
                    fault.ReturnDate = Convert.ToDateTime(rdr["ReturnDate"].ToString());
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
                    fault.ReturnDate = Convert.ToDateTime(rdr["ReturnDate"].ToString());
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
                    fault.ReturnDate = Convert.ToDateTime(rdr["ReturnDate"].ToString());
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
        public ActionResult ConfirmList()
        {
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
        public ActionResult ConfirmWorkOrder()
        {
            string DiscoID = (string)Session["DiscoID"];
            string DiscoUserID = (string)Session["DiscoUserID"];
            FaultyMeters faulty = new FaultyMeters();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateWorkorder", con)) 
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@WorkorderID", faulty.WorkOrderID);
                    cmd.Parameters.AddWithValue("@ReturnedDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@DiscoID", DiscoID);
                    cmd.Parameters.AddWithValue("@DiscoUserID", DiscoUserID);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Workorder has be created successfully";
            return RedirectToAction("AddFaultyMeters");
        }

        public ActionResult AwaitingDispatch()
        {
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
                    fault.ReturnDate = Convert.ToDateTime(rdr["ReturnDate"].ToString());
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
                    fault.ReturnDate = Convert.ToDateTime(rdr["ReturnDate"].ToString());
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
        
        


    }
}
using Mailjet.Client;
using Mailjet.Client.Resources;
using MojecFaultyMeter.Config;
using MojecFaultyMeter.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MojecFaultyMeter.Controllers
{
    public class FactoryManagerController : Controller
    {
        List<Discos> _discos = new List<Discos>();
        List<DiscoUser> _discoUsers = new List<DiscoUser>();
        List<MojecStoreUser> _mojecStores = new List<MojecStoreUser>();
        List<FactoryUsers> _factoryusers = new List<FactoryUsers>();
        List<FaultyMeters> _faulty = new List<FaultyMeters>();
        List<DispatchOrders> _dispatchOrders = new List<DispatchOrders>();
        List<ProcurementUsers> _procurementUsers = new List<ProcurementUsers>();
        List<Fault> _faults = new List<Fault>();
        List<MeterModel> _model = new List<MeterModel>();
        List<MeterType> _metertype = new List<MeterType>();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MojecFaultyMeter"].ConnectionString);
        // GET: FactoryManager
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

        public ActionResult GetReplacementApproval()
        {
            //if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            //{
            //    return RedirectToAction("UsersLogin", "Authentication");
            //}
            _faulty = new List<FaultyMeters>();

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetReplacementApprovalmetersforAdmin", con);
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

        public async Task<ActionResult> AcceptMeter(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("UpdateSp_ReplacementApprovalYes", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeterID", Id);
                con.Open();
                cmd.ExecuteNonQuery();

                string Email = "";
                using (SqlCommand cmd4 = new SqlCommand("select * from ProcurementUsers", con))
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
                        .Property(Send.Subject, "Mojec Faulty Meter")
                        .Property(Send.TextPart, "A new Faulty Meter Requires Replacement. Please Review");
                        MailjetResponse response = await client.PostAsync(request);
                    }
                }
            }

            TempData["save"] = "Meter Accepted successfully";
            return RedirectToAction("GetReplacementApproval");
        }

   
    }


}
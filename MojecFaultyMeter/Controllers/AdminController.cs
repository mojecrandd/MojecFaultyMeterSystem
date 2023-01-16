using MojecFaultyMeter.Config;
using MojecFaultyMeter.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Microsoft.Office.Interop.Excel;
using System.Collections;
using OfficeOpenXml;

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
        List<Fault> _faults = new List<Fault>();
        List<MeterModel> _model = new List<MeterModel>();
        List<MeterType> _metertype = new List<MeterType>();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MojecFaultyMeter"].ConnectionString);
        SqlCommand com = new SqlCommand();
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

        public ActionResult CreateProcurementUser()
        {
            return View();
        }

        [HttpPost]
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
        public ActionResult CreateDiscoUser()
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
        public ActionResult CreateFactoryUser()
        {
            return View();
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
        public ActionResult CreateMojecStoreUser()
        {
            return View();
        }

        [HttpPost]
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
                    user.Status = rdr["Active"].ToString();
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
                    user.Status = rdr["Active"].ToString();
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
                    user.Status = rdr["Active"].ToString();
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
                    user.Status = rdr["Active"].ToString();
                    
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

        [HttpPost]
        public ActionResult PendingMeters(string from , string to, string discoId)
        {

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
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

        [HttpPost]
        public ActionResult Completecases(string from, string to, string discoId)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
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

            ExportData(from, to, discoId);
            return View(_faulty);
        }

        
        
        public FileContentResult DownlaodCompletecasesdata(string from, string to, string discoId)
        {
            using (SqlConnection connection = new SqlConnection(StoreConnection.GetConnection()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"Select * from FaultyMeters where f.Status = 'Recieved' and f.Daterecieved  between {from}   and {to}  and f.DiscoID = {discoId}", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (ExcelPackage package = new ExcelPackage())
                        {
                            //Create the Excel Worksheet
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("FMMSData");

                            //Populate the Excel Worksheet with the data from the SQL query
                            worksheet.Cells["A1"].LoadFromDataReader(reader, true);

                            //Set the content disposition as attachment and the file name as the desired name of the excel file
                            var content = package.GetAsByteArray();
                            var fileName = "FMMSData.xlsx";
                            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                        }
                    }
                }
            }

        }

        public void ExportData(string from, string to, string discoId)
        {
            // Connect to your SQL database
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                con.Open();
                // Retrieve data from your table
                using (SqlCommand cmd = new SqlCommand($"Select * from FaultyMeters f full join Disco d on f.DiscoID = d.DiscoID full join DiscoUsers du on f.DiscoUserID= du.DiscoUserID full join MojecStoreUser mu on f.AcceptedBy = mu.MojecStoreUserID full join FactoryUser fu on f.TreatedBy = fu.FactoryUserID where f.Status = 'Recieved' and f.Daterecieved between '{from}' and '{to}' and d.DiscoID = {discoId}", con))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    System.Data.DataTable dt = new System.Data.DataTable();
                    da.Fill(dt);

                    // Create a new Excel object
                    Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

                    // Create a new Workbook
                    Microsoft.Office.Interop.Excel.Workbook workbook = excel.Workbooks.Add(Type.Missing);

                    // Create a new Worksheet
                    Microsoft.Office.Interop.Excel.Worksheet worksheet = null;

                    // See the excel sheet behind the program
                    excel.Visible = false;

                    try
                    {
                        // Get the first sheet.
                        worksheet = workbook.Sheets["Sheet1"];
                        worksheet = workbook.ActiveSheet;

                        // Rename the sheet
                        worksheet.Name = "ExportedData";

                        // Add the data to the sheet
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                worksheet.Cells[i + 1, j + 1] = dt.Rows[i][j];
                            }
                        }

                        // Save the file
                        workbook.SaveAs("ExportedData.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        excel.Quit();
                        workbook = null;
                        excel = null;
                    }
                }
            }
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
        public ActionResult Dispatchedcases(string from , string to , string discoId)
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

        [HttpPost]
        public ActionResult Solvedcases(string from, string to, string discoId)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
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

        [HttpPost]
        public ActionResult Replacescases(string from, string to, string discoId)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
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

        [HttpPost]
        public ActionResult Reparingcases(string from, string to, string discoId)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
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

        [HttpPost]
        public ActionResult Acceptedcases(string from , string to, string discoId)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
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

        [HttpPost]
        public ActionResult Rejectcases(string from, string to, string discoId)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
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

        [HttpPost]
        public ActionResult AwaitingDispatch(string from, string to, string discoId)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
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

        [HttpPost]
        public ActionResult AwaitingReplacement(string from, string to , string discoId)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
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
        public ActionResult ActivateDiscouser(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("ActivateDiscoUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoUserID", Id);
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Disco user has been activated successfully";
            return RedirectToAction("DiscoUsers");
        }
        public ActionResult DeactivateDiscouser(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DeactivateDiscoUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoUserID", Id);
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Disco user has been Deactivated successfully";
            return RedirectToAction("DiscoUsers");
        }
        public ActionResult ActivateFactoryuser(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("ActivateFactoryUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FactoryUserID", Id);
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Factory user has been activated successfully";
            return RedirectToAction("FactoryUsers");
        }
        public ActionResult DeactivateFactoryuser(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DeactivateFactoryUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FactoryUserID", Id);
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Factory user has been Deactivated successfully";
            return RedirectToAction("FactoryUsers");
        }
        public ActionResult ActivateStoreuser(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("ActivateMojecStoreUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MojecStoreUserID ", Id);
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Mojec store user has been activated successfully";
            return RedirectToAction("MojecStoreUsers");
        }
        public ActionResult DeactivateStoreuser(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DeactivateMojecStoreUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MojecStoreUserID ", Id);
                cmd.ExecuteNonQuery();

            }
            TempData["save"] = "Mojec store user has been Deactivated successfully";
            return RedirectToAction("MojecStoreUsers");
        }
        public ActionResult ActivateProcurementuser(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("ActivateProcurementUsers", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProcurementUserID", Id);
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Procurement user has been activated successfully";
            return RedirectToAction("ProcurementUsers");
        }
        public ActionResult DeactivateProcurementuser(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DeactivateProcurementUsers", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProcurementUserID", Id);
                cmd.ExecuteNonQuery();
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
        public ActionResult AddFault()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddFault(Fault fault)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("AddFault", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Fault", fault.Faultname);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Fault has been saved successfully";
            return RedirectToAction("Fault");
        }
        public ActionResult GetFault(int Id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            Fault fault = new Fault();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("getFaultByID", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@FaultID", Id);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    fault.FaultID = Convert.ToInt32(rdr["FaultID"].ToString());
                    fault.Faultname = rdr["Fault"].ToString();
                }
            }
            return View(fault);
        }
        [HttpPost]
        public ActionResult GetFault(Fault fault)
        {

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateFault", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Fault", fault.Faultname);
                    cmd.Parameters.AddWithValue("@FaultId", fault.FaultID);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Fault has been updated successfully";
            return RedirectToAction("Fault");
        }
        public ActionResult DeleteFault(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("DeleteFault", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FaultId", Id);
                if (con.State != System.Data.ConnectionState.Open)

                    con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Fault has been deleted successfully";
            return RedirectToAction("Fault");
        }
        public ActionResult Fault()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _faults = new List<Fault>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetFault", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Fault fault = new Fault();
                    fault.FaultID = Convert.ToInt32(rdr["FaultID"].ToString());
                    fault.Faultname = rdr["Fault"].ToString();
                    _faults.Add(fault);
                }
                rdr.Close();
            }
            return View(_faults);
        }

        public ActionResult AddModel()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddModel(MeterModel model)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("AddModel", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Model", model.Modelname);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Model has been saved successfully";
            return RedirectToAction("Model");
        }

        public ActionResult Model()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _model = new List<MeterModel>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetModel", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MeterModel model = new MeterModel();
                    model.ModelID = Convert.ToInt32(rdr["ModelID"].ToString());
                    model.Modelname = rdr["Model"].ToString();
                    _model.Add(model);
                }
                rdr.Close();
            }
            return View(_model);
        }

        public ActionResult GetModel(int Id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            MeterModel model = new MeterModel();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("getModelByID", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@ModelID", Id);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    model.ModelID = Convert.ToInt32(rdr["ModelID"].ToString());
                    model.Modelname = rdr["Model"].ToString();
                }
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult GetModel(MeterModel model)
        {

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateModel", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Model", model.Modelname);
                    cmd.Parameters.AddWithValue("@ModelId", model.ModelID);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Model has been updated successfully";
            return RedirectToAction("Model");
        }

        public ActionResult DeleteModel(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("DeleteModel", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ModelId", Id);
                if (con.State != System.Data.ConnectionState.Open)

                    con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Model has been deleted successfully";
            return RedirectToAction("Model");
        }


        public ActionResult AddMeterType()
        {
            //if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            //{
            //    return RedirectToAction("UsersLogin", "Authentication");
            //}
            return View();
        }
        [HttpPost]
        public ActionResult AddMeterType(MeterType meter)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("AddMeterType", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeterType", meter.MetertypeName);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Meter has been saved successfully";
            return RedirectToAction("MeterType");
        }

        public ActionResult MeterType()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            _metertype = new List<MeterType>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("getMeterType", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MeterType meter = new MeterType();
                    meter.MetertypeID = Convert.ToInt32(rdr["MeterTypeID"].ToString());
                    meter.MetertypeName = rdr["MeterType"].ToString();
                    _metertype.Add(meter);
                }
                rdr.Close();
            }
            return View(_metertype);
        }

        public ActionResult GetMeterType(int Id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("UsersLogin", "Authentication");
            }
            MeterType meter = new MeterType();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("getMeterTypeById", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@MeterTypeID", Id);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    meter.MetertypeID = Convert.ToInt32(rdr["MeterTypeID"].ToString());
                    meter.MetertypeName = rdr["MeterType"].ToString();
                }
            }
            return View(meter);
        }

        [HttpPost]
        public ActionResult GetMeterType(MeterType meter)
        {

            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateMeterType", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeterTypeID", meter.MetertypeID);
                    cmd.Parameters.AddWithValue("@MeterType", meter.MetertypeName);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Meter Type has been updated successfully";
            return RedirectToAction("MeterType");
        }

        public ActionResult DeleteMeterType(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("DeleteMeterType", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeterTypeID", Id);
                if (con.State != System.Data.ConnectionState.Open)

                    con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Meter Type has been deleted successfully";
            return RedirectToAction("MeterType");
        }


        public ActionResult Template()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Template(HttpPostedFileBase postedFiles)
        {          
                SqlDataReader dr;
                con.Open();
                com.Connection = con;
                com.CommandText = "Select * from FaultyMeterTemplate where Id = 1";
                dr = com.ExecuteReader();
                if (dr.HasRows)
                {
                    string fileName = Path.GetFileName(postedFiles.FileName);
                    string type = postedFiles.ContentType;
                    byte[] bytes = null;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        postedFiles.InputStream.CopyTo(ms);
                        bytes = ms.ToArray();
                    }
                    using (SqlConnection con = new SqlConnection((StoreConnection.GetConnection())))
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandText = "Update FaultyMeterTemplate set Name = @Name, ContentType = @ContentType, Data = @Data where Id = 1";
                            cmd.Parameters.AddWithValue("@Name", fileName);
                            cmd.Parameters.AddWithValue("@ContentType", type);
                            cmd.Parameters.AddWithValue("@Data", bytes);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
                else
                {
                    string fileName = Path.GetFileName(postedFiles.FileName);
                    string type = postedFiles.ContentType;
                    byte[] bytes = null;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        postedFiles.InputStream.CopyTo(ms);
                        bytes = ms.ToArray();
                    }
                    using (SqlConnection con = new SqlConnection((StoreConnection.GetConnection())))
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandText = "INSERT INTO FaultyMeterTemplate(Name, ContentType, Data) VALUES (@Name, @ContentType, @Data)";
                            cmd.Parameters.AddWithValue("@Name", fileName);
                            cmd.Parameters.AddWithValue("@ContentType", type);
                            cmd.Parameters.AddWithValue("@Data", bytes);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }


            TempData["save"] = "Template Uploaded Successfully";
            return RedirectToAction("Template");
            
        }
















    }
}
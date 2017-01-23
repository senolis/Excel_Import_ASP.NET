using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Excel_App.Models;
using LinqToExcel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Data.SqlClient;


namespace Excel_App.Controllers
{
    public class ProbeController : Controller
    {
       
        private ExcelEntities db = new ExcelEntities();
        // GET: Probe
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>  
        /// This function is used to download excel format.  
        /// </summary>  
        /// <param name="Path"></param>  
        /// <returns>file</returns>  
        //public FileResult DownloadExcel()
        //{
        //    string path = "/Doc/ListSc.xls";
        //    return File(path, "application/vnd.ms-excel", "ListSc.xls");
        //}

        [HttpPost]
        public JsonResult UploadExcel(Probe probes, HttpPostedFileBase FileUpload)
        {

            var data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    var filename = FileUpload.FileName;
                    var targetpath = Server.MapPath("~/Doc/");
                    FileUpload.SaveAs(targetpath + filename);
                    var pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [report$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    var dtable = ds.Tables["ExcelTable"];

                    const string sheetName = "report";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var dataInProbe = from a in excelFile.Worksheet<Probe>(sheetName) select a;

                    foreach (var a in dataInProbe)
                    {
                        try
                        {
                            if (a.ProbeName != "")
                            {
                                var pr = new Probe();
                                pr.ProbeName = a.ProbeName;
                                db.Probes.Add(pr);

                                db.SaveChanges();
                            }
                            else
                            {
                                data.Add("<ul>");
                                if (string.IsNullOrEmpty(a.ProbeName)) data.Add("<li> Probe name is required</li>");
                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
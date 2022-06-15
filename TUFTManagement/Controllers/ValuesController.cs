using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using TUFTManagement.Core;
using TUFTManagement.DTO;
using TUFTManagement.Models;
using TUFTManagement.Services;

namespace TUFTManagement.Controllers
{
    [RoutePrefix("api")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class ValuesController : ApiController
    {
        private SQLManager _sql = SQLManager.Instance;
        private double timestampNow = Utility.DateTimeToUnixTimestamp(DateTime.Now);

        #region Page Login
        [Route("1.0/login")]
        [HttpPost]
        public IHttpActionResult Login([FromBody] LoginRequestDTO loginRs)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            try
            {
                if (string.IsNullOrEmpty(request.Headers["Fromproject"]))
                {
                    throw new Exception("invalid : Fromproject ");
                }
                string json = JsonConvert.SerializeObject(loginRs);
                int logID = _sql.InsertLogReceiveData("Login", json, timestampNow.ToString(), authHeader,
                    0, fromProject.ToLower());

                
                if (string.IsNullOrEmpty(loginRs.username))
                {
                    throw new Exception("invalid : user_name ");
                }
                if (loginRs.password.Equals(null) || loginRs.password.Equals(0))
                {
                    throw new Exception("invalid : password ");
                }
                
                string username = loginRs.username;
                string password = loginRs.password;

                LoginService srv = new LoginService();

                var obj = srv.Login(authHeader, lang, username, password, fromProject.ToLower(), logID);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/logout")]
        [HttpPost]
        public IHttpActionResult Logout()
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);


            try
            {
                string json = JsonConvert.SerializeObject(request.Headers.ToString());
                int logID = _sql.InsertLogReceiveData("Logout", json, timestampNow.ToString(), authHeader,
                    0, fromProject.ToLower());
                LoginService srv = new LoginService();

                var obj = srv.Logout(authHeader, lang, data.userID, fromProject.ToLower(), 1);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/verifytoken")]
        [HttpPost]
        public IHttpActionResult VerifyToken()
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                LoginService srv = new LoginService();

                var obj = srv.VerifyToken(authHeader, lang, fromProject.ToLower(), 1);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }
        #endregion

        #region Add Employees
        [Route("1.0/save/empProfile")]
        [HttpPost]
        public IHttpActionResult SaveEmpProfile(SaveEmpProfileDTO saveEmpProfileDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(saveEmpProfileDTO);
                int logID = _sql.InsertLogReceiveData("SaveEmpProfile", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";
                
                if (string.IsNullOrEmpty(saveEmpProfileDTO.empCode))
                {
                    checkMissingOptional += "empCode ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.userName))
                {
                    checkMissingOptional += "userName ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.password))
                {
                    checkMissingOptional += "password ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.shareCode))
                {
                    checkMissingOptional += "shareCode ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.identityCard))
                {
                    checkMissingOptional += "identityCard ";
                }
                if (saveEmpProfileDTO.identityCard.Count() != 13)
                {
                    checkMissingOptional += "identityCard is incomplete ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.identityCardExpiry))
                {
                    checkMissingOptional += "identityCardExpiry ";
                }
                if (saveEmpProfileDTO.titleID.Equals(0))
                {
                    checkMissingOptional += "titleID ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.firstNameTH))
                {
                    checkMissingOptional += "firstNameTH ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.lastNameTH))
                {
                    checkMissingOptional += "lastNameTH ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.phoneNumber))
                {
                    checkMissingOptional += "phoneNumber ";
                }
                if (saveEmpProfileDTO.phoneNumber.Count() < 9 || saveEmpProfileDTO.phoneNumber.Count() > 10)
                {
                    checkMissingOptional += "phoneNumber is incomplete ";
                }
                if (saveEmpProfileDTO.positionID.Equals(0))
                {
                    checkMissingOptional += "positionID ";
                }
                if (saveEmpProfileDTO.perNum.Equals(0))
                {
                    checkMissingOptional += "perNum ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.dateOfBirth))
                {
                    checkMissingOptional += "dateOfBirth ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.joinDate))
                {
                    checkMissingOptional += "joinDate ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.proPassDate))
                {
                    checkMissingOptional += "proPassDate ";
                }
                if (saveEmpProfileDTO.monthlySalary.Equals(0))
                {
                    checkMissingOptional += "monthlySalary ";
                }
                if (saveEmpProfileDTO.dailySalary.Equals(0))
                {
                    checkMissingOptional += "dailySalary ";
                }
                if (saveEmpProfileDTO.employmentTypeID.Equals(0))
                {
                    checkMissingOptional += "employmentTypeID ";
                }
                if (saveEmpProfileDTO.maritalID.Equals(0))
                {
                    checkMissingOptional += "maritalID ";
                }
                if (saveEmpProfileDTO.pRelationID.Equals(0))
                {
                    checkMissingOptional += "pRelationID ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.pFirstname))
                {
                    checkMissingOptional += "pFirstname ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.pLastname))
                {
                    checkMissingOptional += "pLastname ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.pDateOfBirth))
                {
                    checkMissingOptional += "pDateOfBirth ";
                }
                if (saveEmpProfileDTO.pOccupationID.Equals(0))
                {
                    checkMissingOptional += "pOccupationID ";
                }
                if (saveEmpProfileDTO.bodySetID.Equals(0))
                {
                    checkMissingOptional += "bodySetID ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.shirtSize))
                {
                    checkMissingOptional += "shirtSize ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.cAddress))
                {
                    checkMissingOptional += "cAddress ";
                }
                if (saveEmpProfileDTO.cSubDistrictID.Equals(0))
                {
                    checkMissingOptional += "cSubDistrictID ";
                }
                if (saveEmpProfileDTO.cDistrictID.Equals(0))
                {
                    checkMissingOptional += "cDistrictID ";
                }
                if (saveEmpProfileDTO.cProvinceID.Equals(0))
                {
                    checkMissingOptional += "cProvinceID ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.cZipcode))
                {
                    checkMissingOptional += "cZipcode ";
                }
                if (saveEmpProfileDTO.isSamePermanentAddress.Equals(0))
                {
                    if (string.IsNullOrEmpty(saveEmpProfileDTO.pAddress))
                    {
                        checkMissingOptional += "pAddress ";
                    }
                    if (saveEmpProfileDTO.pSubDistrictID.Equals(0))
                    {
                        checkMissingOptional += "pSubDistrictID ";
                    }
                    if (saveEmpProfileDTO.pDistrictID.Equals(0))
                    {
                        checkMissingOptional += "pDistrictID ";
                    }
                    if (saveEmpProfileDTO.pProvinceID.Equals(0))
                    {
                        checkMissingOptional += "pProvinceID ";
                    }
                    if (string.IsNullOrEmpty(saveEmpProfileDTO.pZipcode))
                    {
                        checkMissingOptional += "pZipcode ";
                    }
                }
                
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                
                InsertService srv1 = new InsertService();
                UpdateService srv = new UpdateService();
                var obj = new object();

                if (saveEmpProfileDTO.empProfileID.Equals(0))
                {
                    obj = srv1.InsertEmpProfileService(authHeader, lang, fromProject.ToLower(), logID, saveEmpProfileDTO, data.roleIDList, data.userID);
                }
                else
                {
                    obj = srv.UpdateEmpProfileService(authHeader, lang, fromProject.ToLower(), logID, saveEmpProfileDTO, data.roleIDList, data.userID);
                }

                return Ok(obj);

            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/delete/empProfile")]
        [HttpPost]
        public IHttpActionResult DeleteEmpProfile(SaveEmpProfileDTO saveEmpProfileDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(saveEmpProfileDTO);
                int logID = _sql.InsertLogReceiveData("DeleteEmpProfile", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (saveEmpProfileDTO.empProfileID.Equals(0))
                {
                    checkMissingOptional += "empProfileID ";
                }
                
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                
                DeleteService srv = new DeleteService();
                var obj = srv.DeleteEmpProfileService(authHeader, lang, fromProject.ToLower(), logID, saveEmpProfileDTO, data.roleIDList, data.userID);
                
                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/get/empProfile")]
        [HttpPost]
        public IHttpActionResult GetEmpProfile(GetEmpProfileDTO getEmpProfileDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);


            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {

                GetService srv = new GetService();

                var obj = srv.GetEmpProfileService(authHeader, getEmpProfileDTO.lang, fromProject.ToLower(), 1, data.userID);
                
                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/save/empRate")]
        [HttpPost]
        public IHttpActionResult SaveEmpRate(SaveEmpRateRequestDTO saveEmpRateDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(saveEmpRateDTO);
                int logID = _sql.InsertLogReceiveData("SaveEmpRate", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (saveEmpRateDTO.empID.Equals(0)|| saveEmpRateDTO.empID.Equals(null))
                {
                    checkMissingOptional += "empID ";
                }
                if (string.IsNullOrEmpty(saveEmpRateDTO.productCode))
                {
                    checkMissingOptional += "productCode ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                InsertService srv = new InsertService();
                UpdateService srv2 = new UpdateService();
                var obj = new object();

                if (saveEmpRateDTO.empRateID.Equals(0) || saveEmpRateDTO.empRateID.Equals(null))
                {
                    obj = srv.InsertEmpRateService(authHeader, lang, fromProject.ToLower(), logID, saveEmpRateDTO, data.roleIDList, data.userID);
                }
                else
                {
                    obj = srv2.UpdateEmpRateService(authHeader, lang, fromProject.ToLower(), logID, saveEmpRateDTO, data.roleIDList, data.userID);
                }
                

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/get/empRate")]
        [HttpPost]
        public IHttpActionResult GetEmpRate(EmpRateRequestDTO empRateRequestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {

                GetService srv = new GetService();

                if (empRateRequestDTO.empID.Equals(0)|| empRateRequestDTO.empID.Equals(null) )
                {
                    throw new Exception("Missing Parameter : empID");
                }

                var obj = srv.GetEmpRateService(authHeader, lang, fromProject.ToLower(), 1, empRateRequestDTO.empID);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/delete/empRate")]
        [HttpPost]
        public IHttpActionResult DeleteEmpRate(EmpRateRequestDTO empRateRequestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(empRateRequestDTO);
                int logID = _sql.InsertLogReceiveData("DeleteEmpRate", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (empRateRequestDTO.empID.Equals(0)|| empRateRequestDTO.empID.Equals(null))
                {
                    checkMissingOptional += "empID ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                DeleteService srv = new DeleteService();
                var obj = srv.DeleteEmpRateService(authHeader, lang, fromProject.ToLower(), logID, empRateRequestDTO, data.roleIDList, data.userID);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/save/empWorkShift")]
        [HttpPost]
        public IHttpActionResult SaveEmpWorkShift(SaveEmpWorkShiftRequestDTO saveEmpWorkShiftRequestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(saveEmpWorkShiftRequestDTO);
                int logID = _sql.InsertLogReceiveData("SaveEmpWorkShift", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (string.IsNullOrEmpty(saveEmpWorkShiftRequestDTO.wsCode))
                {
                    checkMissingOptional += "wsCode ";
                }
                if (string.IsNullOrEmpty(saveEmpWorkShiftRequestDTO.timeStart))
                {
                    checkMissingOptional += "timeStart ";
                }
                if (string.IsNullOrEmpty(saveEmpWorkShiftRequestDTO.timeEnd))
                {
                    checkMissingOptional += "timeEnd ";
                }
                if (saveEmpWorkShiftRequestDTO.workTypeID.Equals(0)|| saveEmpWorkShiftRequestDTO.workTypeID.Equals(null))
                {
                    checkMissingOptional += "workTypeID ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                InsertService srv = new InsertService();
                UpdateService srv2 = new UpdateService();
                var obj = new object();

                if (saveEmpWorkShiftRequestDTO.empWorkShiftID.Equals(0) || saveEmpWorkShiftRequestDTO.empWorkShiftID.Equals(null))
                {
                    obj = srv.InsertEmpWorkShiftService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkShiftRequestDTO, data.roleIDList, data.userID);
                }
                else
                {
                    obj = srv2.UpdateEmpWorkShiftService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkShiftRequestDTO, data.roleIDList, data.userID);
                }


                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/get/empWorkShift")]
        [HttpPost]
        public IHttpActionResult GetEmpWorkShift(SaveEmpWorkShiftRequestDTO requestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(requestDTO);
                int logID = _sql.InsertLogReceiveData("GetEmpWorkShift", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                GetService srv = new GetService();

                if (requestDTO.empWorkShiftID.Equals(0) || requestDTO.empWorkShiftID.Equals(null))
                {
                    throw new Exception("Missing Parameter : empWorkShiftID");
                }

                var obj = srv.GetEmpWorkShiftService(authHeader, lang, fromProject.ToLower(), 1, requestDTO.empWorkShiftID);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/delete/empWorkShift")]
        [HttpPost]
        public IHttpActionResult DeleteEmpWorkShift(SaveEmpWorkShiftRequestDTO requestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(requestDTO);
                int logID = _sql.InsertLogReceiveData("DeleteEmpWorkShift", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (requestDTO.empWorkShiftID.Equals(0) || requestDTO.empWorkShiftID.Equals(null))
                {
                    checkMissingOptional += "empWorkShiftID ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                DeleteService srv = new DeleteService();
                var obj = srv.DeleteEmpWorkShiftService(authHeader, lang, fromProject.ToLower(), logID, requestDTO, data.roleIDList, data.userID);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/upload/excel")]
        [HttpPost]
        public async Task<HttpResponseMessage> UploadFileExcel()
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] ?? "");
            string lang = (request.Headers["lang"] ?? WebConfigurationManager.AppSettings["default_language"]);
            string platform = request.Headers["platform"];
            string version = request.Headers["version"];

            UploadModel value = new UploadModel();
            value.data = new _ServiceUploadData();


            try
            {
                #region Variable Declaration
                HttpResponseMessage ResponseMessage = null;
                var httpRequest = HttpContext.Current.Request;
                DataSet dsexcelRecords = new DataSet();
                IExcelDataReader reader = null;
                HttpPostedFile Inputfile = null;
                Stream FileStream = null;
                SQLManager _sqlmanage = SQLManager.Instance;
                #endregion

                #region Save Student Detail From Excel
                using (Inventory_ComplexEntities1 objEntity = new Inventory_ComplexEntities1())
                {
                    if (httpRequest.Files.Count > 0)
                    {
                        Inputfile = httpRequest.Files[0];
                        FileStream = Inputfile.InputStream;

                        if (Inputfile != null && FileStream != null)
                        {
                            if (string.IsNullOrEmpty(Inputfile.FileName))
                            {
                                throw new Exception("ไม่พบไฟล์");
                            }
                            if (Inputfile.FileName.EndsWith(".xls"))
                            {
                                reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
                            }
                            else if (Inputfile.FileName.EndsWith(".xlsx"))
                            {
                                reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);
                            }
                            else
                            {
                                value.success = false;
                                value.msg = new MsgModel() { code = 0, text = "The file format is not supported.", topic = "No Success" };
                            }

                            dsexcelRecords = reader.AsDataSet();
                            reader.Close();

                            DataTable dtEmpCode = _sqlmanage.GetAllEmpCode();
                            DataTable dtWorkShift = _sqlmanage.GetAllWorkShift();

                            if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                            {
                                DataTable dtExcel = dsexcelRecords.Tables[0];
                                for (int i = 3; i < dtExcel.Rows.Count; i++)
                                {
                                    int year = 0, month = 0; 
                                    int.TryParse(dtExcel.Rows[0][1].ToString(), out year);
                                    int.TryParse(dtExcel.Rows[1][1].ToString(), out month);
                                    if (year == 0)
                                    {
                                        throw new Exception("กรุณาระบุปี");
                                    }
                                    if (month == 0)
                                    {
                                        throw new Exception("กรุณาระบุเดือน");
                                    }
                                    int countDate = DateTime.DaysInMonth(year, month);
                                    string emp_code = Convert.ToString(dtExcel.Rows[i][0]);
                                    int user_id = 0, workshift = 0;
                                    DataRow[] dremp = dtEmpCode.Select("emp_code='" + emp_code + "'");
                                    if (dremp.Length > 0)
                                    {
                                        int.TryParse(dremp[0]["user_id"].ToString(), out user_id);
                                    }

                                    for (int j = 1; j <= countDate; j++)
                                    {
                                        string work_shift = Convert.ToString(dtExcel.Rows[i][j].ToString());
                                        DataRow[] drwork = dtWorkShift.Select("ws_code='" + work_shift + "'");
                                        if (drwork.Length > 0)
                                        {
                                            int.TryParse(drwork[0]["id"].ToString(), out workshift);
                                        }
                                        emp_work_time objWork = new emp_work_time();
                                        objWork.user_id = user_id;
                                        objWork.work_shift_id = workshift;
                                        objWork.work_date = Convert.ToDateTime(year.ToString() + '-' + month.ToString().PadLeft(2, '0') + '-' + j.ToString().PadLeft(2, '0'));
                                        objWork.is_fix = true;
                                        objEntity.emp_work_time.Add(objWork);
                                    }
                                }

                                int output = objEntity.SaveChanges();
                                if (output > 0)
                                {
                                    value.success = true;
                                    value.msg = new MsgModel() { code = 0, text = "The Excel file has been successfully uploaded.", topic = "Success" };
                                }
                                else
                                {
                                    value.success = false;
                                    value.msg = new MsgModel() { code = 0, text = "Something Went Wrong!, The Excel file uploaded has fiald.", topic = "No Success" };
                                }
                            }
                            else
                            {
                                value.success = false;
                                value.msg = new MsgModel() { code = 0, text = "Selected file is empty.", topic = "No Success" };
                            }

                        }
                        else
                        {
                            value.success = false;
                            value.msg = new MsgModel() { code = 0, text = "Invalid File.", topic = "No Success" };
                        }

                    }
                    else
                    {
                        ResponseMessage = Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, value, Configuration.Formatters.JsonFormatter);
                #endregion
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/save/empWorkTime")]
        [HttpPost]
        public IHttpActionResult SaveEmpWorkTime(SaveEmpWorkTimeRequestDTO saveEmpWorkTimeRequestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                var obj = new object();
                string json = JsonConvert.SerializeObject(saveEmpWorkTimeRequestDTO);
                int logID = _sql.InsertLogReceiveData("SaveEmpWorkTime", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(0) || saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(null))
                {
                    checkMissingOptional += "empWorkTimeID ";
                }
                if (saveEmpWorkTimeRequestDTO.empWorkShiftID.Equals(0) || saveEmpWorkTimeRequestDTO.empWorkShiftID.Equals(null))
                {
                    checkMissingOptional += "empWorkShiftID ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {
                    int empWorkTimeID = saveEmpWorkTimeRequestDTO.empWorkTimeID;
                    int param = saveEmpWorkTimeRequestDTO.empWorkShiftID;
                    SaveEmpWorkTimeRequestDTO prepairRequest = new SaveEmpWorkTimeRequestDTO();
                    prepairRequest.empWorkTimeID = empWorkTimeID;
                    prepairRequest.empWorkShiftID = param;

                    UpdateService srv = new UpdateService();
                    
                    if (saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(0) || saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(null))
                    {
                        obj = srv.UpdateEmpWorkTimeService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkTimeRequestDTO, data.roleIDList, data.userID);
                    }
                }
                
                    return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/save/empWorkTime/workIn")]
        [HttpPost]
        public IHttpActionResult SaveEmpWorkTimeWorkIn(SaveEmpWorkTimeRequestDTO saveEmpWorkTimeRequestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                var obj = new object();
                string json = JsonConvert.SerializeObject(saveEmpWorkTimeRequestDTO);
                int logID = _sql.InsertLogReceiveData("SaveEmpWorkTimeWorkIn", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(0) || saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(null))
                {
                    checkMissingOptional += "empWorkTimeID ";
                }
                if (string.IsNullOrEmpty(saveEmpWorkTimeRequestDTO.workIn))
                {
                    checkMissingOptional += "workIn ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {
                    int empWorkTimeID = saveEmpWorkTimeRequestDTO.empWorkTimeID;
                    string param = saveEmpWorkTimeRequestDTO.workIn;
                    SaveEmpWorkTimeRequestDTO prepairRequest = new SaveEmpWorkTimeRequestDTO();
                    prepairRequest.empWorkTimeID = empWorkTimeID;
                    prepairRequest.workIn = param;

                    UpdateService srv = new UpdateService();
                    
                    if (saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(0) || saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(null))
                    {
                        obj = srv.UpdateEmpWorkTimeService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkTimeRequestDTO, data.roleIDList, data.userID);
                    }
                }
                
                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/save/empWorkTime/workOut")]
        [HttpPost]
        public IHttpActionResult SaveEmpWorkTimeWorkOut(SaveEmpWorkTimeRequestDTO saveEmpWorkTimeRequestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                var obj = new object();
                string json = JsonConvert.SerializeObject(saveEmpWorkTimeRequestDTO);
                int logID = _sql.InsertLogReceiveData("SaveEmpWorkTimeWorkOut", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(0) || saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(null))
                {
                    checkMissingOptional += "empWorkTimeID ";
                }
                if (string.IsNullOrEmpty(saveEmpWorkTimeRequestDTO.workOut))
                {
                    checkMissingOptional += "workOut ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {
                    int empWorkTimeID = saveEmpWorkTimeRequestDTO.empWorkTimeID;
                    string param = saveEmpWorkTimeRequestDTO.workOut;
                    SaveEmpWorkTimeRequestDTO prepairRequest = new SaveEmpWorkTimeRequestDTO();
                    prepairRequest.empWorkTimeID = empWorkTimeID;
                    prepairRequest.workOut = param;

                    UpdateService srv = new UpdateService();

                    if (saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(0) || saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(null))
                    {
                        obj = srv.UpdateEmpWorkTimeService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkTimeRequestDTO, data.roleIDList, data.userID);
                    }
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/save/empWorkTime/floorIn")]
        [HttpPost]
        public IHttpActionResult SaveEmpWorkTimeFloorIn(SaveEmpWorkTimeRequestDTO saveEmpWorkTimeRequestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                var obj = new object();
                string json = JsonConvert.SerializeObject(saveEmpWorkTimeRequestDTO);
                int logID = _sql.InsertLogReceiveData("SaveEmpWorkTimeFloorIn", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(0) || saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(null))
                {
                    checkMissingOptional += "empWorkTimeID ";
                }
                if (string.IsNullOrEmpty(saveEmpWorkTimeRequestDTO.floorIn))
                {
                    checkMissingOptional += "floorIn ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {
                    int empWorkTimeID = saveEmpWorkTimeRequestDTO.empWorkTimeID;
                    string param = saveEmpWorkTimeRequestDTO.floorIn;
                    SaveEmpWorkTimeRequestDTO prepairRequest = new SaveEmpWorkTimeRequestDTO();
                    prepairRequest.empWorkTimeID = empWorkTimeID;
                    prepairRequest.floorIn = param;

                    UpdateService srv = new UpdateService();

                    if (saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(0) || saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(null))
                    {
                        obj = srv.UpdateEmpWorkTimeService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkTimeRequestDTO, data.roleIDList, data.userID);
                    }
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/save/empWorkTime/floorOut")]
        [HttpPost]
        public IHttpActionResult SaveEmpWorkTimeFloorOut(SaveEmpWorkTimeRequestDTO saveEmpWorkTimeRequestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                var obj = new object();
                string json = JsonConvert.SerializeObject(saveEmpWorkTimeRequestDTO);
                int logID = _sql.InsertLogReceiveData("SaveEmpWorkTimeFloorOut", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(0) || saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(null))
                {
                    checkMissingOptional += "empWorkTimeID ";
                }
                if (string.IsNullOrEmpty(saveEmpWorkTimeRequestDTO.floorOut))
                {
                    checkMissingOptional += "floorOut ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {
                    int empWorkTimeID = saveEmpWorkTimeRequestDTO.empWorkTimeID;
                    string param = saveEmpWorkTimeRequestDTO.floorOut;
                    SaveEmpWorkTimeRequestDTO prepairRequest = new SaveEmpWorkTimeRequestDTO();
                    prepairRequest.empWorkTimeID = empWorkTimeID;
                    prepairRequest.floorOut = param;

                    UpdateService srv = new UpdateService();

                    if (saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(0) || saveEmpWorkTimeRequestDTO.empWorkTimeID.Equals(null))
                    {
                        obj = srv.UpdateEmpWorkTimeService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkTimeRequestDTO, data.roleIDList, data.userID);
                    }
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/get/empWorkTime")]
        [HttpPost]
        public IHttpActionResult GetEmpWorkTime(SaveEmpWorkTimeRequestDTO empWorkTimeRequest)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {

                GetService srv = new GetService();

                if (empWorkTimeRequest.empWorkTimeID.Equals(0) || empWorkTimeRequest.empWorkTimeID.Equals(null))
                {
                    throw new Exception("Missing Parameter : empWorkTimeID");
                }

                var obj = srv.GetEmpWorkTimeService(authHeader, lang, fromProject.ToLower(), 1, empWorkTimeRequest.empWorkTimeID);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/approve/transChange")]
        [HttpPost]
        public IHttpActionResult ApproveWorkTimeTransChange(SaveWorkTimeTransChangeRequestDTO transChangeRequestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                var obj = new object();
                string json = JsonConvert.SerializeObject(transChangeRequestDTO);
                int logID = _sql.InsertLogReceiveData("ApproveWorkTimeTransChange", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (transChangeRequestDTO.transChangeID.Equals(0) || transChangeRequestDTO.transChangeID.Equals(null))
                {
                    checkMissingOptional += "transChangeID ";
                }
                if (transChangeRequestDTO.statusApprove.Equals(0) || transChangeRequestDTO.statusApprove.Equals(null))
                {
                    checkMissingOptional += "statusApprove ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {
                    UpdateService srv = new UpdateService();

                    if (transChangeRequestDTO.transChangeID.Equals(0) || transChangeRequestDTO.transChangeID.Equals(null))
                    {
                        obj = srv.ApproveWorkTimeTransChangeService(authHeader, lang, fromProject.ToLower(), logID, transChangeRequestDTO, data.roleIDList, data.userID);
                    }
                }

                return Ok(obj);

            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        #endregion

        #region Master
        [Route("1.0/save/master/position")]
        [HttpPost]
        public IHttpActionResult SaveMasterPosition(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("SaveMasterPosition", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (string.IsNullOrEmpty(masterDataDTO.mode))
                {
                    throw new Exception("Missing Parameter : mode ");
                }

                if (masterDataDTO.mode.ToLower().Equals("insert"))
                {
                    if (masterDataDTO.masterID != 0)
                    {
                        checkMissingOptional += "masterID Must 0 ";
                    }
                    if (string.IsNullOrEmpty(masterDataDTO.nameEN))
                    {
                        checkMissingOptional += "nameEN ";
                    }
                    if (string.IsNullOrEmpty(masterDataDTO.nameTH))
                    {
                        checkMissingOptional += "nameTH ";
                    }
                }
                else if (masterDataDTO.mode.ToLower().Equals("update"))
                {
                    if (masterDataDTO.masterID == 0)
                    {
                        checkMissingOptional += "masterID ";
                    }
                    if (string.IsNullOrEmpty(masterDataDTO.nameEN))
                    {
                        checkMissingOptional += "nameEN ";
                    }
                    if (string.IsNullOrEmpty(masterDataDTO.nameTH))
                    {
                        checkMissingOptional += "nameTH ";
                    }
                }
                else if (masterDataDTO.mode.ToLower().Equals("delete"))
                {
                    if (masterDataDTO.masterID == 0)
                    {
                        checkMissingOptional += "masterID ";
                    }
                }
                else
                {
                    throw new Exception("Choose Mode Insert or Update or Delete");
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                obj = srv.SaveMasterService(authHeader, lang, fromProject.ToLower(), logID, masterDataDTO, "master_position", data.userID);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/get/master/position")]
        [HttpPost]
        public IHttpActionResult GetMasterPosition(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("GetMasterPosition", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.GetMasterService(authHeader, lang, fromProject.ToLower(), logID, masterDataDTO.masterID, "master_position");
                }
                else
                {
                    throw new Exception("Missing Parameter : ID ");
                }


                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/search/master/position")]
        [HttpPost]
        public IHttpActionResult SearchMasterDataPosition(SearchMasterDataDTO searchMasterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject("");
                int logID = _sql.InsertLogReceiveData("SearchMasterDataPosition", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (searchMasterDataDTO.pageInt.Equals(null) || searchMasterDataDTO.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (searchMasterDataDTO.perPage.Equals(null) || searchMasterDataDTO.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }
                if (searchMasterDataDTO.sortField > 4)
                {
                    throw new Exception("invalid : sortField " + searchMasterDataDTO.sortField);
                }
                if (!(searchMasterDataDTO.sortType == "a" || searchMasterDataDTO.sortType == "d" || searchMasterDataDTO.sortType == "A" || searchMasterDataDTO.sortType == "D" || searchMasterDataDTO.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchMasterService(authHeader, lang, fromProject.ToLower(), logID, searchMasterDataDTO, "master_position", data.roleIDList);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/save/master/bodySet")]
        [HttpPost]
        public IHttpActionResult SaveBodySet(SaveBodySetRequestDTO saveBodySetDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(saveBodySetDTO);
                int logID = _sql.InsertLogReceiveData("SaveBodySet", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (string.IsNullOrEmpty(saveBodySetDTO.height.ToString()))
                {
                    checkMissingOptional += checkMissingOptional + "height ";
                }
                if (string.IsNullOrEmpty(saveBodySetDTO.weight.ToString()))
                {
                    checkMissingOptional += checkMissingOptional + "weight ";
                }
                if (string.IsNullOrEmpty(saveBodySetDTO.chest.ToString()))
                {
                    checkMissingOptional += checkMissingOptional + "chest ";
                }
                if (string.IsNullOrEmpty(saveBodySetDTO.waist.ToString()))
                {
                    checkMissingOptional += checkMissingOptional + "waist ";
                }
                if (string.IsNullOrEmpty(saveBodySetDTO.hip.ToString()))
                {
                    checkMissingOptional += checkMissingOptional + "hip ";
                }
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                MasterDataService srv = new MasterDataService();
                UpdateService srv2 = new UpdateService();
                var obj = new object();

                if (saveBodySetDTO.id.Equals(0) || saveBodySetDTO.id.Equals(null))
                {
                    obj = srv.InsertBodySetService(authHeader, lang, fromProject.ToLower(), logID, saveBodySetDTO, data.roleIDList, data.userID);
                }
                else
                {
                    obj = srv.UpdateBodySetService(authHeader, lang, fromProject.ToLower(), logID, saveBodySetDTO, data.roleIDList, data.userID);
                }


                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/get/master/bodySet")]
        [HttpPost]
        public IHttpActionResult GetBodySet(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("GetBodySet", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.GetMasterBodySetervice(authHeader, lang, fromProject.ToLower(), logID, masterDataDTO.masterID);
                }
                else
                {
                    throw new Exception("Missing Parameter : ID ");
                }


                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/delete/master/bodySet")]
        [HttpPost]
        public IHttpActionResult DeleteBodySet(SaveBodySetRequestDTO saveBodySetDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(saveBodySetDTO);
                int logID = _sql.InsertLogReceiveData("DeleteBodySet", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                //string checkMissingOptional = "";

                //if (string.IsNullOrEmpty(saveBodySetDTO.height.ToString()))
                //{
                //    checkMissingOptional += checkMissingOptional + "height ";
                //}
                //if (string.IsNullOrEmpty(saveBodySetDTO.weight.ToString()))
                //{
                //    checkMissingOptional += checkMissingOptional + "weight ";
                //}
                //if (string.IsNullOrEmpty(saveBodySetDTO.chest.ToString()))
                //{
                //    checkMissingOptional += checkMissingOptional + "chest ";
                //}
                //if (string.IsNullOrEmpty(saveBodySetDTO.waist.ToString()))
                //{
                //    checkMissingOptional += checkMissingOptional + "waist ";
                //}
                //if (string.IsNullOrEmpty(saveBodySetDTO.hip.ToString()))
                //{
                //    checkMissingOptional += checkMissingOptional + "hip ";
                //}
                //if (checkMissingOptional != "")
                //{
                //    throw new Exception("Missing Parameter : " + checkMissingOptional);
                //}

                MasterDataService srv = new MasterDataService();
                UpdateService srv2 = new UpdateService();
                var obj = new object();

                if (saveBodySetDTO.id.Equals(0) || saveBodySetDTO.id.Equals(null))
                {
                    obj = srv.InsertBodySetService(authHeader, lang, fromProject.ToLower(), logID, saveBodySetDTO, data.roleIDList, data.userID);
                }
                


                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }


        #endregion

        #region Feedback
        [Route("1.0/save/feedback")]
        [HttpPost]
        public IHttpActionResult SaveFeedback(FeedbackDTO feedbackDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(feedbackDTO);
                int logID = _sql.InsertLogReceiveData("SaveFeedback", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (string.IsNullOrEmpty(feedbackDTO.EmpID.ToString()))
                {
                    checkMissingOptional += checkMissingOptional + "EmpID ";
                }
                if (string.IsNullOrEmpty(feedbackDTO.Rate.ToString()))
                {
                    checkMissingOptional += checkMissingOptional + "Rate ";
                }
                if (string.IsNullOrEmpty(feedbackDTO.Comment.ToString()))
                {
                    checkMissingOptional += checkMissingOptional + "Comment ";
                }
                if (string.IsNullOrEmpty(feedbackDTO.TranID.ToString()))
                {
                    checkMissingOptional += checkMissingOptional + "TranID ";
                }
                
                InsertService srv = new InsertService();
                var obj = new object();
                obj = srv.InsertFeedbackService(authHeader, lang, fromProject.ToLower(), logID, feedbackDTO, 0, data.userID);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/get/feedback")]
        [HttpPost]
        public IHttpActionResult GetFeedback(FeedbackDTO feedbackDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = request.Headers["Fromproject"];
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(feedbackDTO);
                int logID = _sql.InsertLogReceiveData("GetFeedback", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                GetService srv = new GetService();

                var obj = new object();

                if (feedbackDTO.EmpID != 0)
                {
                    obj = srv.GetFeedbackService(authHeader, lang, fromProject.ToLower(), logID, feedbackDTO.EmpID);
                }
                else
                {
                    throw new Exception("Missing Parameter : EmpID ");
                }


                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }
        #endregion
    }
}

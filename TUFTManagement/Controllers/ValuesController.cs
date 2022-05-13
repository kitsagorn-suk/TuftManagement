using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            string platform = "web";
            string businesscode = request.Headers["businesscode"]; 

            try
            {
                string json = JsonConvert.SerializeObject(loginRs);
                int logID = _sql.InsertLogReceiveData("Login", json, timestampNow.ToString(), authHeader,
                    0, platform.ToLower());
                
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

                var obj = srv.Login(authHeader, lang, username, password, platform.ToLower(), logID, businesscode);
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
            string platform = "web";

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                LoginService srv = new LoginService();

                var obj = srv.Logout(authHeader, lang, data.user_id, platform.ToLower(), 1);
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
            string platform = "web";

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                LoginService srv = new LoginService();

                var obj = srv.VerifyToken(authHeader, lang, platform.ToLower(), 1);
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
            string platform = "web";

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(saveEmpProfileDTO);
                int logID = _sql.InsertLogReceiveData("SaveEmpProfile", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                string checkMissingOptional = "";
                
                if (string.IsNullOrEmpty(saveEmpProfileDTO.empCode))
                {
                    checkMissingOptional += checkMissingOptional + "empCode ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.userName))
                {
                    checkMissingOptional += checkMissingOptional + "userName ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.password))
                {
                    checkMissingOptional += checkMissingOptional + "password ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.businessCode))
                {
                    checkMissingOptional += checkMissingOptional + "businessCode ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.identityCard))
                {
                    checkMissingOptional += checkMissingOptional + "identityCard ";
                }
                if (saveEmpProfileDTO.identityCard.Count() != 13)
                {
                    checkMissingOptional += checkMissingOptional + "identityCard is incomplete ";
                }
                if (saveEmpProfileDTO.titleID.Equals(0))
                {
                    checkMissingOptional += checkMissingOptional + "titleID ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.firstNameTH))
                {
                    checkMissingOptional += checkMissingOptional + "firstNameTH ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.lastNameTH))
                {
                    checkMissingOptional += checkMissingOptional + "lastNameTH ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.phoneNumber))
                {
                    checkMissingOptional += checkMissingOptional + "phoneNumber ";
                }
                if (saveEmpProfileDTO.phoneNumber.Count() < 9 || saveEmpProfileDTO.phoneNumber.Count() > 10)
                {
                    checkMissingOptional += checkMissingOptional + "phoneNumber is incomplete ";
                }
                if (saveEmpProfileDTO.positionID.Equals(0))
                {
                    checkMissingOptional += checkMissingOptional + "positionID ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.personalCode))
                {
                    checkMissingOptional += checkMissingOptional + "personalCode ";
                }
                if (saveEmpProfileDTO.personalNO.Equals(0))
                {
                    checkMissingOptional += checkMissingOptional + "personalNO ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.joinDate))
                {
                    checkMissingOptional += checkMissingOptional + "joinDate ";
                }
                if (saveEmpProfileDTO.employmentTypeID.Equals(0))
                {
                    checkMissingOptional += checkMissingOptional + "employmentTypeID ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.cAddress))
                {
                    checkMissingOptional += checkMissingOptional + "cAddress ";
                }
                if (saveEmpProfileDTO.cSubDistrictID.Equals(0))
                {
                    checkMissingOptional += checkMissingOptional + "cSubDistrictID ";
                }
                if (saveEmpProfileDTO.cDistrictID.Equals(0))
                {
                    checkMissingOptional += checkMissingOptional + "cDistrictID ";
                }
                if (saveEmpProfileDTO.cProvinceID.Equals(0))
                {
                    checkMissingOptional += checkMissingOptional + "cProvinceID ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.cZipcode))
                {
                    checkMissingOptional += checkMissingOptional + "cZipcode ";
                }
                if (saveEmpProfileDTO.isSamePermanentAddress.Equals(0))
                {
                    if (string.IsNullOrEmpty(saveEmpProfileDTO.pAddress))
                    {
                        checkMissingOptional += checkMissingOptional + "pAddress ";
                    }
                    if (saveEmpProfileDTO.pSubDistrictID.Equals(0))
                    {
                        checkMissingOptional += checkMissingOptional + "pSubDistrictID ";
                    }
                    if (saveEmpProfileDTO.pDistrictID.Equals(0))
                    {
                        checkMissingOptional += checkMissingOptional + "pDistrictID ";
                    }
                    if (saveEmpProfileDTO.pProvinceID.Equals(0))
                    {
                        checkMissingOptional += checkMissingOptional + "pProvinceID ";
                    }
                    if (string.IsNullOrEmpty(saveEmpProfileDTO.pZipcode))
                    {
                        checkMissingOptional += checkMissingOptional + "pZipcode ";
                    }
                }
                
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                
                InsertService srv1 = new InsertService();
                var obj = srv1.InsertEmpProfileService(authHeader, lang, platform.ToLower(), logID, saveEmpProfileDTO, data.role_id, data.user_id);
                
                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/edit/empProfile")]
        [HttpPost]
        public IHttpActionResult EditEmpProfile(SaveEmpProfileDTO saveEmpProfileDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = "web";

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(saveEmpProfileDTO);
                int logID = _sql.InsertLogReceiveData("EditEmpProfile", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                string checkMissingOptional = "";

                if (string.IsNullOrEmpty(saveEmpProfileDTO.businessCode))
                {
                    checkMissingOptional += checkMissingOptional + "businessCode ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.identityCard))
                {
                    checkMissingOptional += checkMissingOptional + "identityCard ";
                }
                if (saveEmpProfileDTO.identityCard.Count() != 13)
                {
                    checkMissingOptional += checkMissingOptional + "identityCard is incomplete ";
                }
                if (saveEmpProfileDTO.titleID.Equals(0))
                {
                    checkMissingOptional += checkMissingOptional + "titleID ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.firstNameTH))
                {
                    checkMissingOptional += checkMissingOptional + "firstNameTH ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.lastNameTH))
                {
                    checkMissingOptional += checkMissingOptional + "lastNameTH ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.phoneNumber))
                {
                    checkMissingOptional += checkMissingOptional + "phoneNumber ";
                }
                if (saveEmpProfileDTO.phoneNumber.Count() < 9 || saveEmpProfileDTO.phoneNumber.Count() > 10)
                {
                    checkMissingOptional += checkMissingOptional + "phoneNumber is incomplete ";
                }
                if (saveEmpProfileDTO.positionID.Equals(0))
                {
                    checkMissingOptional += checkMissingOptional + "positionID ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.personalCode))
                {
                    checkMissingOptional += checkMissingOptional + "personalCode ";
                }
                if (saveEmpProfileDTO.personalNO.Equals(0))
                {
                    checkMissingOptional += checkMissingOptional + "personalNO ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.joinDate))
                {
                    checkMissingOptional += checkMissingOptional + "joinDate ";
                }
                if (saveEmpProfileDTO.employmentTypeID.Equals(0))
                {
                    checkMissingOptional += checkMissingOptional + "employmentTypeID ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.cAddress))
                {
                    checkMissingOptional += checkMissingOptional + "cAddress ";
                }
                if (saveEmpProfileDTO.cSubDistrictID.Equals(0))
                {
                    checkMissingOptional += checkMissingOptional + "cSubDistrictID ";
                }
                if (saveEmpProfileDTO.cDistrictID.Equals(0))
                {
                    checkMissingOptional += checkMissingOptional + "cDistrictID ";
                }
                if (saveEmpProfileDTO.cProvinceID.Equals(0))
                {
                    checkMissingOptional += checkMissingOptional + "cProvinceID ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.cZipcode))
                {
                    checkMissingOptional += checkMissingOptional + "cZipcode ";
                }
                if (saveEmpProfileDTO.isSamePermanentAddress.Equals(0))
                {
                    if (string.IsNullOrEmpty(saveEmpProfileDTO.pAddress))
                    {
                        checkMissingOptional += checkMissingOptional + "pAddress ";
                    }
                    if (saveEmpProfileDTO.pSubDistrictID.Equals(0))
                    {
                        checkMissingOptional += checkMissingOptional + "pSubDistrictID ";
                    }
                    if (saveEmpProfileDTO.pDistrictID.Equals(0))
                    {
                        checkMissingOptional += checkMissingOptional + "pDistrictID ";
                    }
                    if (saveEmpProfileDTO.pProvinceID.Equals(0))
                    {
                        checkMissingOptional += checkMissingOptional + "pProvinceID ";
                    }
                    if (string.IsNullOrEmpty(saveEmpProfileDTO.pZipcode))
                    {
                        checkMissingOptional += checkMissingOptional + "pZipcode ";
                    }
                }
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                
                UpdateService srv = new UpdateService();
                var obj = srv.UpdateEmpProfileService(authHeader, lang, platform.ToLower(), logID, saveEmpProfileDTO, data.role_id, data.user_id);
                
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
            string platform = "web";

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(saveEmpProfileDTO);
                int logID = _sql.InsertLogReceiveData("DeleteEmpProfile", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                string checkMissingOptional = "";

                if (saveEmpProfileDTO.empProfileID.Equals(0))
                {
                    checkMissingOptional += checkMissingOptional + "empProfileID ";
                }
                
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                
                DeleteService srv = new DeleteService();
                var obj = srv.DeleteEmpProfileService(authHeader, lang, platform.ToLower(), logID, saveEmpProfileDTO, data.role_id, data.user_id);
                
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
            string platform = "web";
            

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                
                GetService srv = new GetService();

                if (string.IsNullOrEmpty(getEmpProfileDTO.lang))
                {
                    getEmpProfileDTO.lang = "th";
                }
                var obj = srv.GetEmpProfileService(authHeader, getEmpProfileDTO.lang, platform.ToLower(), 1, data.user_id);
                
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
            string platform = "web";

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(saveEmpRateDTO);
                int logID = _sql.InsertLogReceiveData("SaveEmpRate", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                string checkMissingOptional = "";

                if (saveEmpRateDTO.empID.Equals(0)|| saveEmpRateDTO.empID.Equals(null))
                {
                    checkMissingOptional += checkMissingOptional + "empID ";
                }
                if (string.IsNullOrEmpty(saveEmpRateDTO.productCode))
                {
                    checkMissingOptional += checkMissingOptional + "productCode ";
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
                    obj = srv.InsertEmpRateService(authHeader, lang, platform.ToLower(), logID, saveEmpRateDTO, data.role_id, data.user_id);
                }
                else
                {
                    obj = srv2.UpdateEmpRateService(authHeader, lang, platform.ToLower(), logID, saveEmpRateDTO, data.role_id, data.user_id);
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
        public IHttpActionResult GetEmpRate(GetEmpRateRequestDTO getEmpRateRequestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = "web";
            

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {

                GetService srv = new GetService();

                if (getEmpRateRequestDTO.userID.Equals(0)|| getEmpRateRequestDTO.userID.Equals(null) )
                {
                    throw new Exception("Missing Parameter : userID");
                }

                var obj = srv.GetEmpRateService(authHeader, lang, platform.ToLower(), 1, getEmpRateRequestDTO.userID);

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
            string platform = "web";

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("SaveMasterPosition", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

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
                obj = srv.SaveMasterService(authHeader, lang, platform.ToLower(), logID, masterDataDTO, "master_position", data.user_id);

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
            string platform = "web";

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("GetMasterPosition", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.GetMasterService(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, "master_position");
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
            string platform = "web";

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject("");
                int logID = _sql.InsertLogReceiveData("SearchMasterDataPosition", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

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

                obj = srv.SearchMasterService(authHeader, lang, platform.ToLower(), logID, searchMasterDataDTO, "master_position", data.role_id);

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
            string platform = request.Headers["platform"];
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(saveBodySetDTO);
                int logID = _sql.InsertLogReceiveData("SaveEmpRate", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

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
                    obj = srv.InsertBodySetService(authHeader, lang, platform.ToLower(), logID, saveBodySetDTO, data.role_id, data.user_id);
                }
                else
                {
                    obj = srv.UpdateBodySetService(authHeader, lang, platform.ToLower(), logID, saveBodySetDTO, data.role_id, data.user_id);
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
            string platform = request.Headers["platform"];
            string version = request.Headers["version"];
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("GetMasterBodySet", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.GetMasterService(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, "system_body_set");
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
            string platform = request.Headers["platform"];
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(saveBodySetDTO);
                int logID = _sql.InsertLogReceiveData("SaveEmpRate", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

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
                    obj = srv.InsertBodySetService(authHeader, lang, platform.ToLower(), logID, saveBodySetDTO, data.role_id, data.user_id);
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
            string platform = "web";

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(feedbackDTO);
                int logID = _sql.InsertLogReceiveData("SaveFeedback", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

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
                obj = srv.InsertFeedbackService(authHeader, lang, platform.ToLower(), logID, feedbackDTO, 0, data.user_id);

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
            string platform = request.Headers["platform"];
            string version = request.Headers["version"];
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(feedbackDTO);
                int logID = _sql.InsertLogReceiveData("GetFeedback", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                GetService srv = new GetService();

                var obj = new object();

                if (feedbackDTO.EmpID != 0)
                {
                    obj = srv.GetFeedbackService(authHeader, lang, platform.ToLower(), logID, feedbackDTO.EmpID);
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

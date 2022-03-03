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
    [RoutePrefix("api/2.0")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {
        private SQLManager _sql = SQLManager.Instance;
        private double timestampNow = Utility.DateTimeToUnixTimestamp(DateTime.Now);

        #region Page Login
        [Route("login")]
        [HttpPost]
        public IHttpActionResult Login([FromBody] LoginRequestDTO loginRs)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"];
            string version = request.Headers["version"];

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

                var obj = srv.Login(authHeader, lang, username, password, platform.ToLower(), logID);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }
        #endregion

        #region Add Employees
        [Route("save/empProfile")]
        [HttpPost]
        public IHttpActionResult InsertEmpProfile(SaveEmpProfileDTO saveEmpProfileDTO)
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
                string json = JsonConvert.SerializeObject(saveEmpProfileDTO);
                int logID = _sql.InsertLogReceiveData("SaveEmpProfile", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                string checkMissingOptional = "";

                if (saveEmpProfileDTO.empProfileID.Equals(0))
                {
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
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                var obj = new object();
                if (saveEmpProfileDTO.empProfileID != 0)
                {
                    UpdateService srv = new UpdateService();
                    obj = srv.UpdateEmpProfileService(authHeader, lang, platform.ToLower(), logID, saveEmpProfileDTO, data.role_id, data.user_id);
                }
                else
                {
                    InsertService srv1 = new InsertService();
                    obj = srv1.InsertEmpProfileService(authHeader, lang, platform.ToLower(), logID, saveEmpProfileDTO, data.role_id, data.user_id);
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
        [Route("search/master/position")]
        [HttpPost]
        public IHttpActionResult SearchMasterDataPosition(PageRequestDTO pageRequest)
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
                string json = JsonConvert.SerializeObject("");
                int logID = _sql.InsertLogReceiveData("SearchMasterDataPosition", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (pageRequest.pageInt.Equals(null) || pageRequest.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (pageRequest.perPage.Equals(null) || pageRequest.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }

                if (pageRequest.sortField > 4)
                {
                    throw new Exception("invalid : sortField " + pageRequest.sortField);
                }
                if (!(pageRequest.sortType == "a" || pageRequest.sortType == "d" || pageRequest.sortType == "A" || pageRequest.sortType == "D" || pageRequest.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchMasterDataPositionService(authHeader, lang, platform.ToLower(), logID, pageRequest.perPage, pageRequest.pageInt, pageRequest.paramSearch
                        , pageRequest.sortField, pageRequest.sortType, data.role_id);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("get/master/position")]
        [HttpPost]
        public IHttpActionResult GetMasterPosition(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

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
                    obj = srv.GetMasterPositionService(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, data.role_id);
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

        [Route("save/master/position")]
        [HttpPost]
        public IHttpActionResult SaveMasterPosition(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("SaveMasterPosition", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());
                
                string checkMissingOptional = "";
                
                if (string.IsNullOrEmpty(masterDataDTO.nameEN))
                {
                    checkMissingOptional += checkMissingOptional + "nameEN ";
                }
                if (string.IsNullOrEmpty(masterDataDTO.nameTH))
                {
                    checkMissingOptional += checkMissingOptional + "nameTH ";
                }
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.UpdateMasterPositionService(authHeader, lang, platform.ToLower(), logID, masterDataDTO, data.role_id, data.user_id);
                }
                else
                {
                    obj = srv.InsertMasterPositionService(authHeader, lang, platform.ToLower(), logID, masterDataDTO, data.role_id, data.user_id);
                }
                
                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("delete/master/position")]
        [HttpPost]
        public IHttpActionResult DeleteMasterPosition(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("DeleteMasterPosition", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.DeleteMasterPositionSevice(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, data.role_id, data.user_id);
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

        [Route("search/master/productarea")]
        [HttpPost]
        public IHttpActionResult SearchMasterDataProductArea(PageRequestDTO pageRequest)
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
                string json = JsonConvert.SerializeObject("");
                int logID = _sql.InsertLogReceiveData("SearchMasterDataProductArea", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (pageRequest.pageInt.Equals(null) || pageRequest.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (pageRequest.perPage.Equals(null) || pageRequest.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }

                if (pageRequest.sortField > 4)
                {
                    throw new Exception("invalid : sortField " + pageRequest.sortField);
                }
                if (!(pageRequest.sortType == "a" || pageRequest.sortType == "d" || pageRequest.sortType == "A" || pageRequest.sortType == "D" || pageRequest.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchMasterDataProductAreaService(authHeader, lang, platform.ToLower(), logID, pageRequest.perPage, pageRequest.pageInt, pageRequest.paramSearch
                        , pageRequest.sortField, pageRequest.sortType, data.role_id);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("get/master/productarea")]
        [HttpPost]
        public IHttpActionResult GetMasterProductArea(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("GetMasterProductArea", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.GetMasterProductAreaService(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, data.role_id);
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

        [Route("save/master/productarea")]
        [HttpPost]
        public IHttpActionResult SaveMasterProductArea(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("SaveMasterProductArea", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                string checkMissingOptional = "";

                if (string.IsNullOrEmpty(masterDataDTO.nameEN))
                {
                    checkMissingOptional += checkMissingOptional + "nameEN ";
                }
                if (string.IsNullOrEmpty(masterDataDTO.nameTH))
                {
                    checkMissingOptional += checkMissingOptional + "nameTH ";
                }
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.UpdateMasterProductAreaService(authHeader, lang, platform.ToLower(), logID, masterDataDTO, data.role_id, data.user_id);
                }
                else
                {
                    obj = srv.InsertMasterProductAreaService(authHeader, lang, platform.ToLower(), logID, masterDataDTO, data.role_id, data.user_id);
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("delete/master/productarea")]
        [HttpPost]
        public IHttpActionResult DeleteMasterProductArea(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("DeleteMasterProductArea", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.DeleteMasterProductAreaSevice(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, data.role_id, data.user_id);
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

        [Route("search/master/productcategory")]
        [HttpPost]
        public IHttpActionResult SearchMasterDataProductCategory(PageRequestDTO pageRequest)
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
                string json = JsonConvert.SerializeObject("");
                int logID = _sql.InsertLogReceiveData("SearchMasterDataProductCategory", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (pageRequest.pageInt.Equals(null) || pageRequest.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (pageRequest.perPage.Equals(null) || pageRequest.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }

                if (pageRequest.sortField > 4)
                {
                    throw new Exception("invalid : sortField " + pageRequest.sortField);
                }
                if (!(pageRequest.sortType == "a" || pageRequest.sortType == "d" || pageRequest.sortType == "A" || pageRequest.sortType == "D" || pageRequest.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchMasterDataProductCategoryService(authHeader, lang, platform.ToLower(), logID, pageRequest.perPage, pageRequest.pageInt, pageRequest.paramSearch
                        , pageRequest.sortField, pageRequest.sortType, data.role_id);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("get/master/productcategory")]
        [HttpPost]
        public IHttpActionResult GetMasterProductCategory(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("GetMasterProductCategory", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.GetMasterProductCategoryService(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, data.role_id);
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

        [Route("save/master/productcategory")]
        [HttpPost]
        public IHttpActionResult SaveMasterProductCategory(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("SaveMasterProductCategory", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                string checkMissingOptional = "";

                if (string.IsNullOrEmpty(masterDataDTO.nameEN))
                {
                    checkMissingOptional += checkMissingOptional + "nameEN ";
                }
                if (string.IsNullOrEmpty(masterDataDTO.nameTH))
                {
                    checkMissingOptional += checkMissingOptional + "nameTH ";
                }
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.UpdateMasterProductCategoryService(authHeader, lang, platform.ToLower(), logID, masterDataDTO, data.role_id, data.user_id);
                }
                else
                {
                    obj = srv.InsertMasterProductCategoryService(authHeader, lang, platform.ToLower(), logID, masterDataDTO, data.role_id, data.user_id);
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("delete/master/productcategory")]
        [HttpPost]
        public IHttpActionResult DeleteMasterProductCategory(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("DeleteMasterProductCategory", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.DeleteMasterProductCategorySevice(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, data.role_id, data.user_id);
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

        [Route("search/master/producttype")]
        [HttpPost]
        public IHttpActionResult SearchMasterDataProductType(PageRequestDTO pageRequest)
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
                string json = JsonConvert.SerializeObject("");
                int logID = _sql.InsertLogReceiveData("SearchMasterDataProductType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (pageRequest.pageInt.Equals(null) || pageRequest.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (pageRequest.perPage.Equals(null) || pageRequest.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }

                if (pageRequest.sortField > 4)
                {
                    throw new Exception("invalid : sortField " + pageRequest.sortField);
                }
                if (!(pageRequest.sortType == "a" || pageRequest.sortType == "d" || pageRequest.sortType == "A" || pageRequest.sortType == "D" || pageRequest.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchMasterDataProductTypeService(authHeader, lang, platform.ToLower(), logID, pageRequest.perPage, pageRequest.pageInt, pageRequest.paramSearch
                        , pageRequest.sortField, pageRequest.sortType, data.role_id);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("get/master/producttype")]
        [HttpPost]
        public IHttpActionResult GetMasterProductType(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("GetMasterProductType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.GetMasterProductTypeService(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, data.role_id);
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

        [Route("save/master/producttype")]
        [HttpPost]
        public IHttpActionResult SaveMasterProductType(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("SaveMasterProductType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                string checkMissingOptional = "";

                if (string.IsNullOrEmpty(masterDataDTO.nameEN))
                {
                    checkMissingOptional += checkMissingOptional + "nameEN ";
                }
                if (string.IsNullOrEmpty(masterDataDTO.nameTH))
                {
                    checkMissingOptional += checkMissingOptional + "nameTH ";
                }
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.UpdateMasterProductTypeService(authHeader, lang, platform.ToLower(), logID, masterDataDTO, data.role_id, data.user_id);
                }
                else
                {
                    obj = srv.InsertMasterProductTypeService(authHeader, lang, platform.ToLower(), logID, masterDataDTO, data.role_id, data.user_id);
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("delete/master/producttype")]
        [HttpPost]
        public IHttpActionResult DeleteMasterProductType(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("DeleteMasterProductType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.DeleteMasterProductTypeSevice(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, data.role_id, data.user_id);
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

        [Route("search/master/quemembertype")]
        [HttpPost]
        public IHttpActionResult SearchMasterDataQueMemberType(PageRequestDTO pageRequest)
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
                string json = JsonConvert.SerializeObject("");
                int logID = _sql.InsertLogReceiveData("SearchMasterDataQueMemberType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (pageRequest.pageInt.Equals(null) || pageRequest.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (pageRequest.perPage.Equals(null) || pageRequest.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }

                if (pageRequest.sortField > 4)
                {
                    throw new Exception("invalid : sortField " + pageRequest.sortField);
                }
                if (!(pageRequest.sortType == "a" || pageRequest.sortType == "d" || pageRequest.sortType == "A" || pageRequest.sortType == "D" || pageRequest.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchMasterDataQueMemberTypeService(authHeader, lang, platform.ToLower(), logID, pageRequest.perPage, pageRequest.pageInt, pageRequest.paramSearch
                        , pageRequest.sortField, pageRequest.sortType, data.role_id);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("get/master/quemembertype")]
        [HttpPost]
        public IHttpActionResult GetMasterQueMemberType(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("GetMasterQueMemberType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.GetMasterQueMemberTypeService(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, data.role_id);
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

        [Route("save/master/quemembertype")]
        [HttpPost]
        public IHttpActionResult SaveMasterQueMemberType(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("SaveMasterQueMemberType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                string checkMissingOptional = "";

                if (string.IsNullOrEmpty(masterDataDTO.nameEN))
                {
                    checkMissingOptional += checkMissingOptional + "nameEN ";
                }
                if (string.IsNullOrEmpty(masterDataDTO.nameTH))
                {
                    checkMissingOptional += checkMissingOptional + "nameTH ";
                }
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.UpdateMasterQueMemberTypeService(authHeader, lang, platform.ToLower(), logID, masterDataDTO, data.role_id, data.user_id);
                }
                else
                {
                    obj = srv.InsertMasterQueMemberTypeService(authHeader, lang, platform.ToLower(), logID, masterDataDTO, data.role_id, data.user_id);
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("delete/master/quemembertype")]
        [HttpPost]
        public IHttpActionResult DeleteMasterQueMemberType(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("DeleteMasterQueMemberType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.DeleteMasterQueMemberTypeSevice(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, data.role_id, data.user_id);
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

        [Route("search/master/questafftype")]
        [HttpPost]
        public IHttpActionResult SearchMasterDataQueStaffType(PageRequestDTO pageRequest)
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
                string json = JsonConvert.SerializeObject("");
                int logID = _sql.InsertLogReceiveData("SearchMasterDataQueStaffType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (pageRequest.pageInt.Equals(null) || pageRequest.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (pageRequest.perPage.Equals(null) || pageRequest.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }

                if (pageRequest.sortField > 4)
                {
                    throw new Exception("invalid : sortField " + pageRequest.sortField);
                }
                if (!(pageRequest.sortType == "a" || pageRequest.sortType == "d" || pageRequest.sortType == "A" || pageRequest.sortType == "D" || pageRequest.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchMasterDataQueStaffTypeService(authHeader, lang, platform.ToLower(), logID, pageRequest.perPage, pageRequest.pageInt, pageRequest.paramSearch
                        , pageRequest.sortField, pageRequest.sortType, data.role_id);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("get/master/questafftype")]
        [HttpPost]
        public IHttpActionResult GetMasterQueStaffType(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("GetMasterQueStaffType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.GetMasterQueStaffTypeService(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, data.role_id);
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

        [Route("save/master/questafftype")]
        [HttpPost]
        public IHttpActionResult SaveMasterQueStaffType(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("SaveMasterQueStaffType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                string checkMissingOptional = "";

                if (string.IsNullOrEmpty(masterDataDTO.nameEN))
                {
                    checkMissingOptional += checkMissingOptional + "nameEN ";
                }
                if (string.IsNullOrEmpty(masterDataDTO.nameTH))
                {
                    checkMissingOptional += checkMissingOptional + "nameTH ";
                }
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.UpdateMasterQueStaffTypeService(authHeader, lang, platform.ToLower(), logID, masterDataDTO, data.role_id, data.user_id);
                }
                else
                {
                    obj = srv.InsertMasterQueStaffTypeService(authHeader, lang, platform.ToLower(), logID, masterDataDTO, data.role_id, data.user_id);
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("delete/master/questafftype")]
        [HttpPost]
        public IHttpActionResult DeleteMasterQueStaffType(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("DeleteMasterQueStaffType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.DeleteMasterQueStaffTypeSevice(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, data.role_id, data.user_id);
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

        [Route("search/master/roomtype")]
        [HttpPost]
        public IHttpActionResult SearchMasterDataRoomType(PageRequestDTO pageRequest)
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
                string json = JsonConvert.SerializeObject("");
                int logID = _sql.InsertLogReceiveData("SearchMasterDataRoomType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (pageRequest.pageInt.Equals(null) || pageRequest.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (pageRequest.perPage.Equals(null) || pageRequest.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }

                if (pageRequest.sortField > 4)
                {
                    throw new Exception("invalid : sortField " + pageRequest.sortField);
                }
                if (!(pageRequest.sortType == "a" || pageRequest.sortType == "d" || pageRequest.sortType == "A" || pageRequest.sortType == "D" || pageRequest.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchMasterDataRoomTypeService(authHeader, lang, platform.ToLower(), logID, pageRequest.perPage, pageRequest.pageInt, pageRequest.paramSearch
                        , pageRequest.sortField, pageRequest.sortType, data.role_id);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("get/master/roomtype")]
        [HttpPost]
        public IHttpActionResult GetMasterRoomType(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("GetMasterRoomType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.GetMasterRoomTypeService(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, data.role_id);
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

        [Route("save/master/roomtype")]
        [HttpPost]
        public IHttpActionResult SaveMasterRoomType(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("SaveMasterRoomType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                string checkMissingOptional = "";

                if (string.IsNullOrEmpty(masterDataDTO.nameEN))
                {
                    checkMissingOptional += checkMissingOptional + "nameEN ";
                }
                if (string.IsNullOrEmpty(masterDataDTO.nameTH))
                {
                    checkMissingOptional += checkMissingOptional + "nameTH ";
                }
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.UpdateMasterRoomTypeService(authHeader, lang, platform.ToLower(), logID, masterDataDTO, data.role_id, data.user_id);
                }
                else
                {
                    obj = srv.InsertMasterRoomTypeService(authHeader, lang, platform.ToLower(), logID, masterDataDTO, data.role_id, data.user_id);
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("delete/master/roomtype")]
        [HttpPost]
        public IHttpActionResult DeleteMasterRoomType(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("DeleteMasterRoomType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.DeleteMasterRoomTypeSevice(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, data.role_id, data.user_id);
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

        [Route("search/master/stocktype")]
        [HttpPost]
        public IHttpActionResult SearchMasterDataStockType(PageRequestDTO pageRequest)
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
                string json = JsonConvert.SerializeObject("");
                int logID = _sql.InsertLogReceiveData("SearchMasterDataStockType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (pageRequest.pageInt.Equals(null) || pageRequest.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (pageRequest.perPage.Equals(null) || pageRequest.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }

                if (pageRequest.sortField > 4)
                {
                    throw new Exception("invalid : sortField " + pageRequest.sortField);
                }
                if (!(pageRequest.sortType == "a" || pageRequest.sortType == "d" || pageRequest.sortType == "A" || pageRequest.sortType == "D" || pageRequest.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchMasterDataStockTypeService(authHeader, lang, platform.ToLower(), logID, pageRequest.perPage, pageRequest.pageInt, pageRequest.paramSearch
                        , pageRequest.sortField, pageRequest.sortType, data.role_id);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("get/master/stocktype")]
        [HttpPost]
        public IHttpActionResult GetMasterStockType(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("GetMasterStockType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.GetMasterStockTypeService(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, data.role_id);
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

        [Route("save/master/stocktype")]
        [HttpPost]
        public IHttpActionResult SaveMasterStockType(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("SaveMasterStockType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                string checkMissingOptional = "";

                if (string.IsNullOrEmpty(masterDataDTO.nameEN))
                {
                    checkMissingOptional += checkMissingOptional + "nameEN ";
                }
                if (string.IsNullOrEmpty(masterDataDTO.nameTH))
                {
                    checkMissingOptional += checkMissingOptional + "nameTH ";
                }
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.UpdateMasterStockTypeService(authHeader, lang, platform.ToLower(), logID, masterDataDTO, data.role_id, data.user_id);
                }
                else
                {
                    obj = srv.InsertMasterStockTypeService(authHeader, lang, platform.ToLower(), logID, masterDataDTO, data.role_id, data.user_id);
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("delete/master/stocktype")]
        [HttpPost]
        public IHttpActionResult DeleteMasterStockType(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string platform = request.Headers["platform"]; //
            string version = request.Headers["version"];

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, true);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveData("DeleteMasterStockType", json, timestampNow.ToString(), authHeader,
                    data.user_id, platform.ToLower());

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.DeleteMasterStockTypeSevice(authHeader, lang, platform.ToLower(), logID, masterDataDTO.masterID, data.role_id, data.user_id);
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
        #endregion
    }
}

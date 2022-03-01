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
                if (saveEmpProfileDTO.phoneNumber.Count() != 9 || saveEmpProfileDTO.phoneNumber.Count() != 10)
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

                string json = JsonConvert.SerializeObject(saveEmpProfileDTO);
                int logID = _sql.InsertLogReceiveData("InsertEmpProfile", json, timestampNow.ToString(), authHeader,
                    0, platform.ToLower());

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
    }
}

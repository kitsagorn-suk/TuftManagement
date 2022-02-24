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
        [Route("insert/empProfile")]
        [HttpPost]
        public IHttpActionResult InsertEmpProfile(InsertEmpProfileDTO insertEmpProfileDTO)
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
                if (string.IsNullOrEmpty(insertEmpProfileDTO.empCode))
                {
                    checkMissingOptional = checkMissingOptional + "empCode ";
                }
                if (string.IsNullOrEmpty(insertEmpProfileDTO.userName))
                {
                    checkMissingOptional = checkMissingOptional + "userName ";
                }
                if (string.IsNullOrEmpty(insertEmpProfileDTO.password))
                {
                    checkMissingOptional = checkMissingOptional + "password ";
                }
                if (string.IsNullOrEmpty(insertEmpProfileDTO.identityCard))
                {
                    checkMissingOptional = checkMissingOptional + "identityCard ";
                }
                if (insertEmpProfileDTO.titleID.Equals(null) || insertEmpProfileDTO.titleID.Equals(0))
                {
                    checkMissingOptional = checkMissingOptional + "titleID ";
                }
                if (string.IsNullOrEmpty(insertEmpProfileDTO.firstNameTH))
                {
                    checkMissingOptional = checkMissingOptional + "firstNameTH ";
                }
                if (string.IsNullOrEmpty(insertEmpProfileDTO.lastNameTH))
                {
                    checkMissingOptional = checkMissingOptional + "lastNameTH ";
                }
                if (string.IsNullOrEmpty(insertEmpProfileDTO.contact))
                {
                    checkMissingOptional = checkMissingOptional + "contact ";
                }
                if (insertEmpProfileDTO.positionID.Equals(null) || insertEmpProfileDTO.positionID.Equals(0))
                {
                    checkMissingOptional = checkMissingOptional + "positionID ";
                }
                if (string.IsNullOrEmpty(insertEmpProfileDTO.personalCode))
                {
                    checkMissingOptional = checkMissingOptional + "personalCode ";
                }
                if (insertEmpProfileDTO.personalNO.Equals(null) || insertEmpProfileDTO.personalNO.Equals(0))
                {
                    checkMissingOptional = checkMissingOptional + "personalNO ";
                }
                if (string.IsNullOrEmpty(insertEmpProfileDTO.joinDate))
                {
                    checkMissingOptional = checkMissingOptional + "joinDate ";
                }
                if (string.IsNullOrEmpty(insertEmpProfileDTO.condition))
                {
                    checkMissingOptional = checkMissingOptional + "condition ";
                }
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                string json = JsonConvert.SerializeObject(insertEmpProfileDTO);
                int logID = _sql.InsertLogReceiveData("InsertEmpProfile", json, timestampNow.ToString(), authHeader,
                    0, platform.ToLower());

                InsertService srv = new InsertService();

                var obj = srv.InsertEmpProfileService(authHeader, lang, platform.ToLower(), logID, insertEmpProfileDTO, data.role_id, data.user_id);
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

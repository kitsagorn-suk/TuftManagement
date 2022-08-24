using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
//
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
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
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

        [Route("1.1/decode")]
        [HttpPost]
        public IHttpActionResult DecodeString()
        {
            var request = HttpContext.Current.Request;
            string strText = (request.Headers["strText"] == null ? "" : request.Headers["strText"]);
            string type = (request.Headers["type"] == null ? "" : request.Headers["type"]);

            try
            {
                var obj = new object();

                if (type == "encode")
                {
                    obj = new
                    {
                        value = Utility.Base64UrlEncode(strText)
                    };
                }
                if (type == "decode")
                {
                    obj = new
                    {
                        value = Utility.Base64ForUrlDecode(strText)
                    };
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        #endregion

        #region Main API

        [Route("1.0/get/dropdownByModuleName")]
        [HttpPost]
        public IHttpActionResult GetDropdownByModuleName(GetDropdownRequestDTO getDropdownRequestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(getDropdownRequestDTO);
                int logID = _sql.InsertLogReceiveData("GetDropdownByModuleName", json, timestampNow.ToString(), headersDTO.authHeader,
                    data.userID, fromProject.ToLower());

                GetService srv = new GetService();
                //ValidateService validateService = new ValidateService();
                ValidationModel chkRequestBody = new ValidationModel(); //validateService.RequireOptionalAllDropdown(lang, fromProject.ToLower(), logID, getDropdownRequestDTO);

                var obj = new object();
                obj = chkRequestBody;

                if (getDropdownRequestDTO.moduleName.ToLower() == "subDistrict".ToLower())
                {
                    obj = srv.GetSubDistrictDropdownService(authHeader, lang, fromProject.ToLower(), logID, getDropdownRequestDTO);
                }
                if (getDropdownRequestDTO.moduleName.ToLower() == "titlename".ToLower())
                {
                    obj = srv.GetTitleNameDropdownService(authHeader, lang, fromProject.ToLower(), logID, getDropdownRequestDTO);
                }
                if (getDropdownRequestDTO.moduleName.ToLower() == "positionFilter".ToLower())
                {
                    if(getDropdownRequestDTO.departmentID != 0)
                    {
                        obj = srv.GetPositionByDepartmentDropdownService(authHeader, lang, fromProject.ToLower(), logID, getDropdownRequestDTO);
                    }
                    else
                    {
                        throw new Exception("Missing Parameter : departmentID");
                    }
                }
                else
                {
                    obj = srv.GetAllDropdownService(authHeader, lang, fromProject.ToLower(), logID, getDropdownRequestDTO);
                }
                
                
                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/user/upload/file")]
        [HttpPost]
        public async Task<HttpResponseMessage> UploadFile()
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            

            //var obj = new Object();
            UploadModel value = new UploadModel();
            value.data = new _ServiceUploadData();
            value.data.fileDetails = new List<_fileDetails>();

            int ActionUserID = 0;
            int userID = 0;
            string diskFolderPath = string.Empty;
            string subFolder = string.Empty;
            string keyName = string.Empty;
            string fileName = string.Empty;
            string newFileName = string.Empty;
            string fileURL = string.Empty;
            var fileSize = long.MinValue;
            string fileCode = string.Empty;

            #region gen file code
            Random rnd = new Random();
            int passwordRandom = rnd.Next(100000, 999999);
            fileCode = Utility.MD5Hash(passwordRandom.ToString() + "_" + ActionUserID.ToString()).ToUpper();
            #endregion

            var path = WebConfigurationManager.AppSettings["body_path"];
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
            }

            MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider(path);

            await Request.Content.ReadAsMultipartAsync(streamProvider);

            //get userID for action
            foreach (var findUserID in streamProvider.FormData.AllKeys)
            {
                foreach (var val in streamProvider.FormData.GetValues(findUserID))
                {
                    if (findUserID == "userID")
                    {
                        ActionUserID = int.Parse(val);
                    }
                    if (findUserID == "fileCode")
                    {
                        fileCode = val.ToString();
                    }
                }
            }

            foreach (MultipartFileData fileData in streamProvider.FileData)
            {
                fileSize = new FileInfo(fileData.LocalFileName).Length;
                if (fileSize > 3100000)
                {
                    throw new Exception("error file size limit 3.00 MB");
                }

                keyName = fileData.Headers.ContentDisposition.Name.Replace("\"", "");
                fileName = fileData.Headers.ContentDisposition.FileName.Replace("\"", "");
                newFileName = Guid.NewGuid() + Path.GetExtension(fileName);


                if (keyName == "upload_image_profile" || keyName == "upload_image_gallery" || keyName == "upload_image_identity")
                {
                    AuthenticationController _auth = AuthenticationController.Instance;
                    AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);
                    userID = data.userID;
                }
                if (keyName == "upload_image_profile")
                {
                    subFolder = "\\ProFilePath";
                    diskFolderPath = string.Format(WebConfigurationManager.AppSettings["file_user_path"], subFolder);
                    //newFileName = _sql.GenImageProfile(userID);
                    fileURL = string.Format(WebConfigurationManager.AppSettings["file_user_url"], "ProFilePath", newFileName);
                }
                if (keyName == "upload_image_gallery")
                {
                    subFolder = "\\GalleryPath";
                    diskFolderPath = string.Format(WebConfigurationManager.AppSettings["file_user_path"], subFolder);
                    //newFileName = _sql.GenImageProfile(userID);
                    fileURL = string.Format(WebConfigurationManager.AppSettings["file_user_url"], "GalleryPath", newFileName);
                }
                if (keyName == "upload_image_identity")
                {
                    subFolder = "\\IdentityPath";
                    diskFolderPath = string.Format(WebConfigurationManager.AppSettings["file_user_path"], subFolder);
                    //newFileName = _sql.GenImageProfile(userID);
                    fileURL = string.Format(WebConfigurationManager.AppSettings["file_user_url"], "IdentityPath", newFileName);
                }
                if (keyName == "upload_image_employment_document")
                {
                    subFolder = "\\EmploymentDocument";
                    diskFolderPath = string.Format(WebConfigurationManager.AppSettings["file_user_path"], subFolder);
                    //newFileName = _sql.GenImageProfile(userID);
                    fileURL = string.Format(WebConfigurationManager.AppSettings["file_user_url"], "EmploymentDocument", newFileName);
                }

                var fullPath = Path.Combine(diskFolderPath, newFileName);
                var fileInfo = new FileInfo(fullPath);
                while (fileInfo.Exists)
                {
                    if (keyName == "upload_user_profile" || keyName == "upload_image_gallery" || keyName == "upload_image_identity" || keyName == "upload_image_employment_document")
                    {
                        File.Delete(fullPath);
                    }
                    else
                    {
                        newFileName = fileInfo.Name.Replace(fileInfo.Extension, "");
                        newFileName = newFileName + Guid.NewGuid().ToString() + fileInfo.Extension;
                    }

                    fullPath = Path.Combine(diskFolderPath, newFileName);
                    fileInfo = new FileInfo(fullPath);
                    break;
                }

                if (!Directory.Exists(fileInfo.Directory.FullName))
                {
                    Directory.CreateDirectory(fileInfo.Directory.FullName);
                }
                File.Move(fileData.LocalFileName, fullPath);

                if (!File.Exists(fullPath))
                {
                    value.success = false;
                    value.msg = new MsgModel() { code = 0, text = "อัพโหลดไม่สำเร็จ", topic = "ไม่สำเร็จ" };
                }
                else
                {
                    if (keyName == "upload_image_profile")
                    {
                        ValidationModel validation = new ValidationModel();
                        validation = ValidationManager.CheckValidationWithShareCode(shareCode, 0, lang, "");
                        if (validation.Success == true)
                        {

                            _ReturnIdModel result = _sql.InsertUploadFileDetails(shareCode, keyName, fileCode, "", fileName, fileURL, userID);
                            value.success = validation.Success;

                            _fileDetails file = new _fileDetails();
                            file.id = result.id;
                            file.fileUrl = fileURL;
                            file.fileName = fileName;
                            file.fileSize = fileSize.ToString();

                            value.data.fileCode = fileCode;
                            value.data.fileDetails.Add(file);

                            value.msg = new MsgModel() { code = 0, text = "อัพโหลดสำเร็จ", topic = "สำเร็จ" };
                        }
                        else
                        {
                            value.success = validation.Success;
                            value.msg = new MsgModel() { code = 0, text = validation.InvalidMessage, topic = validation.InvalidText };
                        }

                    }

                    if (keyName == "upload_image_gallery")
                    {
                        ValidationModel validation = new ValidationModel();
                        validation = ValidationManager.CheckValidationWithShareCode(shareCode, 0, lang, "");
                        if (validation.Success == true)
                        {

                            _ReturnIdModel result = _sql.InsertUploadFileDetails(shareCode, keyName, fileCode, "", fileName, fileURL, userID);
                            value.success = validation.Success;

                            _fileDetails file = new _fileDetails();

                            file.id = result.id;
                            file.fileUrl = fileURL;
                            file.fileName = fileName;
                            file.fileSize = fileSize.ToString();

                            value.data.fileCode = fileCode;
                            value.data.fileDetails.Add(file);

                            value.msg = new MsgModel() { code = 0, text = "อัพโหลดสำเร็จ", topic = "สำเร็จ" };
                        }
                        else
                        {
                            value.success = validation.Success;
                            value.msg = new MsgModel() { code = 0, text = validation.InvalidMessage, topic = validation.InvalidText };
                        }

                    }
                    if (keyName == "upload_image_identity")
                    {
                        ValidationModel validation = new ValidationModel();
                        validation = ValidationManager.CheckValidationWithShareCode(shareCode, 0, lang, "");
                        if (validation.Success == true)
                        {

                            _ReturnIdModel result = _sql.InsertUploadFileDetails(shareCode, keyName, fileCode, "", fileName, fileURL, userID);
                            value.success = validation.Success;

                            _fileDetails file = new _fileDetails();
                            file.id = result.id;
                            file.fileUrl = fileURL;
                            file.fileName = fileName;
                            file.fileSize = fileSize.ToString();

                            value.data.fileCode = fileCode;
                            value.data.fileDetails.Add(file);

                            value.msg = new MsgModel() { code = 0, text = "อัพโหลดสำเร็จ", topic = "สำเร็จ" };
                        }
                        else
                        {
                            value.success = validation.Success;
                            value.msg = new MsgModel() { code = 0, text = validation.InvalidMessage, topic = validation.InvalidText };
                        }

                    }

                    if (keyName == "upload_image_employment_document")
                    {
                        ValidationModel validation = new ValidationModel();
                        validation = ValidationManager.CheckValidationWithShareCode(shareCode, 0, lang, "");
                        if (validation.Success == true)
                        {

                            _ReturnIdModel result = _sql.InsertUploadFileDetails(shareCode, keyName, fileCode, "", fileName, fileURL, userID);
                            value.success = validation.Success;

                            _fileDetails file = new _fileDetails();
                            file.id = result.id;
                            file.fileUrl = fileURL;
                            file.fileName = fileName;
                            file.fileSize = fileSize.ToString();

                            value.data.fileCode = fileCode;
                            value.data.fileDetails.Add(file);

                            value.msg = new MsgModel() { code = 0, text = "อัพโหลดสำเร็จ", topic = "สำเร็จ" };
                        }
                        else
                        {
                            value.success = validation.Success;
                            value.msg = new MsgModel() { code = 0, text = validation.InvalidMessage, topic = validation.InvalidText };
                        }

                    }
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, value, Configuration.Formatters.JsonFormatter);
        }

        [Route("1.0/user/delete/file")]
        [HttpPost]
        public IHttpActionResult DeleteFile(RequestDTO requestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(requestDTO);
                int logID = _sql.InsertLogReceiveData("DeleteFile", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (requestDTO.id.Equals(0))
                {
                    checkMissingOptional += "id ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                DeleteService srv = new DeleteService();
                var obj = srv.DeleteEmpFileService(shareCode, authHeader, lang, fromProject.ToLower(), logID, requestDTO, data.roleIDList, data.userID);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/user/get/file")]
        [HttpPost]
        public IHttpActionResult GetFileByCode(RequestDTO requestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(requestDTO);
                int logID = _sql.InsertLogReceiveData("GetFileByCode", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (requestDTO.fileCode.Equals(0))
                {
                    checkMissingOptional += "fileCode ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                GetService srv = new GetService();
                var obj = srv.GetFileByCodeService(shareCode, authHeader, lang, fromProject.ToLower(), logID, data.userID, requestDTO.fileCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        #endregion

        #region Employees

        [Route("1.0/save/empProfile")]
        [HttpPost]
        public IHttpActionResult SaveEmpProfile(SaveEmpProfileDTO saveEmpProfileDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);
            string agentID = (request.Headers["AgentID"] == null ? "" : request.Headers["AgentID"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(saveEmpProfileDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SaveEmpProfile", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                ValidateService validateService = new ValidateService();

                

                if (!string.IsNullOrEmpty(agentID))
                {
                    saveEmpProfileDTO.agentID = int.Parse(agentID.ToString());
                }
                saveEmpProfileDTO.shareCode = shareCode;
                saveEmpProfileDTO.shareID = _sql.getShareIdByShareCode(shareCode);






                 ValidationModel chkRequestBody = validateService.RequireOptionalSaveEmpProfile(shareCode, lang, fromProject.ToLower(), logID, saveEmpProfileDTO);

                // prepair username 
                saveEmpProfileDTO.userName = shareCode.ToUpper() + "-" + saveEmpProfileDTO.userName;

                // prepair isSamePermanentAddress
                if (saveEmpProfileDTO.isSamePermanentAddress == 1)
                {
                    saveEmpProfileDTO.cCountryID = saveEmpProfileDTO.pCountryID;
                    saveEmpProfileDTO.cAddress = saveEmpProfileDTO.pAddress;
                    saveEmpProfileDTO.cProvinceID = saveEmpProfileDTO.pProvinceID;
                    saveEmpProfileDTO.cDistrictID = saveEmpProfileDTO.pDistrictID;
                    saveEmpProfileDTO.cSubDistrictID = saveEmpProfileDTO.pSubDistrictID;
                    saveEmpProfileDTO.cZipcode = saveEmpProfileDTO.pZipcode;
                    saveEmpProfileDTO.cPhoneContact = saveEmpProfileDTO.pPhoneContact;
                }

                var obj = new object();
                if (chkRequestBody.Success == true)
                {
                    if (saveEmpProfileDTO.empProfileID.Equals(0) && saveEmpProfileDTO.mode.ToLower() == "insert")
                    {
                        InsertService srv = new InsertService();
                        obj = srv.InsertEmpProfileService(shareCode, authHeader, lang, fromProject.ToLower(), logID, saveEmpProfileDTO, data.roleIDList, data.userID);
                    }
                    else if (saveEmpProfileDTO.empProfileID > 0 && saveEmpProfileDTO.mode.ToLower() == "update")
                    {
                        UpdateService srv = new UpdateService();
                        obj = srv.UpdateEmpProfileService(shareCode, authHeader, lang, fromProject.ToLower(), logID, saveEmpProfileDTO, data.roleIDList, data.userID);
                    }
                }
                
                return Ok(obj);

            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/get/empProfileDetails")]
        [HttpPost]
        public IHttpActionResult GetEmpProfile(RequestDTO requestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(data.userID);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "GetEmpProfile", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                GetService srv = new GetService();

                var obj = srv.GetEmpProfileService(shareCode, authHeader, lang, fromProject.ToLower(), logID, data.userID, requestDTO);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/search/allemployee")]
        [HttpPost]
        public IHttpActionResult GetSearchAllEmployee(PageRequestDTO pageRequestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject("");
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SearchAllEmployee", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                
                string strDepartmentSearch = JsonConvert.SerializeObject(pageRequestDTO.departmentSearch);
                strDepartmentSearch = string.Join(",", pageRequestDTO.departmentSearch);
                pageRequestDTO.prepairDepartmentSearch = strDepartmentSearch;

                string strPositionSearch = JsonConvert.SerializeObject(pageRequestDTO.positionSearch);
                strPositionSearch = string.Join(",", pageRequestDTO.positionSearch);
                pageRequestDTO.prepairPositionSearch = strPositionSearch;

                string strEmpTypeSearch = JsonConvert.SerializeObject(pageRequestDTO.empTypeSearch);
                strEmpTypeSearch = string.Join(",", pageRequestDTO.empTypeSearch);
                pageRequestDTO.prepairEmpTypeSearch = strEmpTypeSearch;

                string strEmpStatusSearch = JsonConvert.SerializeObject(pageRequestDTO.empStatusSearch);
                strEmpStatusSearch = string.Join(",", pageRequestDTO.empStatusSearch);
                pageRequestDTO.prepairEmpStatusSearch = strEmpStatusSearch;


                if (pageRequestDTO.pageInt.Equals(null) || pageRequestDTO.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (pageRequestDTO.perPage.Equals(null) || pageRequestDTO.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }

                if (pageRequestDTO.sortField > 3)
                {
                    throw new Exception("invalid : sortField " + pageRequestDTO.sortField);
                }
                if (!(pageRequestDTO.sortType == "a" || pageRequestDTO.sortType == "d" || pageRequestDTO.sortType == "A" || pageRequestDTO.sortType == "D" || pageRequestDTO.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchAllEmployee(authHeader, lang, fromProject.ToLower(), logID, pageRequestDTO, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.1/get/empProfile")]
        [HttpPost]
        public IHttpActionResult GetEmpProfileV1_1()
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(data.userID);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "GetEmpProfile", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                GetService srv = new GetService();

                var obj = srv.GetEmpProfileService(shareCode, authHeader, lang, fromProject.ToLower(), logID, data.userID);
                
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
                if (saveEmpRateDTO.serviceNo.Equals(0) || saveEmpRateDTO.serviceNo.Equals(null))
                {
                    checkMissingOptional += "serviceNo ";
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
                    obj = srv2.UpdateEmpRateService(shareCode, authHeader, lang, fromProject.ToLower(), logID, saveEmpRateDTO, data.roleIDList, data.userID);
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

        [Route("1.0/update/EmpStatus")]
        [HttpPost]
        public IHttpActionResult UpdateEmployeeStatus(SaveEmpStatusDTO saveEmpStatusDTO)
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
                string json = JsonConvert.SerializeObject(saveEmpStatusDTO);
                int logID = _sql.InsertLogReceiveData("UpdateEmployeeStatus", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (saveEmpStatusDTO.userID.Equals(0) || saveEmpStatusDTO.userID.Equals(null))
                {
                    checkMissingOptional += "userID ";
                }
                if (saveEmpStatusDTO.employmentStatusID.Equals(0) || saveEmpStatusDTO.employmentStatusID.Equals(null))
                {
                    checkMissingOptional += "employmentStatusID ";
                }
                if (string.IsNullOrEmpty(saveEmpStatusDTO.imageEmploymentCode))
                {
                    checkMissingOptional += "imageEmploymentCode ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                UpdateService srv = new UpdateService();
                var obj = srv.UpdateEmpStatusService(shareCode, authHeader, lang, fromProject.ToLower(), logID, saveEmpStatusDTO, data.roleIDList, data.userID);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }


        #endregion

        #region Time Attendance

        [Route("1.0/save/empWorkShift")]
        [HttpPost]
        public IHttpActionResult SaveEmpWorkShift(SaveEmpWorkShiftRequestDTO saveEmpWorkShiftRequestDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(saveEmpWorkShiftRequestDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SaveEmpWorkShift", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                ValidateService validateService = new ValidateService();
                ValidationModel chkRequestBody = validateService.RequireOptionalSaveEmpWorkShift(shareCode, lang, fromProject.ToLower(), logID, saveEmpWorkShiftRequestDTO);
                
                
                InsertService srvInsert = new InsertService();
                UpdateService srvUpdate = new UpdateService();
                DeleteService srvDelete = new DeleteService();
                var obj = new object();

                if(chkRequestBody.Success == true)
                {
                    if (saveEmpWorkShiftRequestDTO.empWorkShiftID.Equals(0) && saveEmpWorkShiftRequestDTO.mode.ToLower() == "insert")
                    {
                        obj = srvInsert.InsertEmpWorkShiftService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkShiftRequestDTO, data.roleIDList, data.userID);
                    }
                    else if (saveEmpWorkShiftRequestDTO.empWorkShiftID > 0 && saveEmpWorkShiftRequestDTO.mode.ToLower() == "update")
                    {
                        obj = srvUpdate.UpdateEmpWorkShiftService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkShiftRequestDTO, data.roleIDList, data.userID);
                    }
                    else if (saveEmpWorkShiftRequestDTO.empWorkShiftID > 0 && saveEmpWorkShiftRequestDTO.mode.ToLower() == "delete")
                    {
                        obj = srvDelete.DeleteEmpWorkShiftService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkShiftRequestDTO, data.roleIDList, data.userID);
                    }
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

        [Route("1.0/upload/workshift")]
        [HttpPost]
        public async Task<HttpResponseMessage> UploadWorkShift()
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            GetEmpWorkTimeUploadModel value = new GetEmpWorkTimeUploadModel();
            value.data = new EmpWorkTimeUpload();


            try
            {
                #region Variable Declaration
                HttpResponseMessage ResponseMessage = null;
                var httpRequest = HttpContext.Current.Request;
                DataSet dsexcelRecords = new DataSet();
                IExcelDataReader reader = null;
                HttpPostedFile Inputfile = null;
                Stream FileStream = null;
                SQLManager _sql = SQLManager.Instance;

                value.data.employeeUpload = new List<EmployeeUpload>();

                int userID = 0, workShiftID = 0, year = 0, month = 0, day = 0;
                #endregion

                #region Save Detail From Excel
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

                            DataTable dtEmpCode = _sql.GetAllEmpCode(lang);
                            DataTable dtWorkShift = _sql.GetAllWorkShift();

                            if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                            {
                                DataTable dtExcel = dsexcelRecords.Tables[0];
                                for (int i = 3; i < dtExcel.Rows.Count; i++)
                                {
                                    EmployeeUpload employeeUpload = new EmployeeUpload();
                                    employeeUpload.workShiftUpload = new List<WorkShiftUpload>();

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
                                    string isFix = Convert.ToString(dtExcel.Rows[i][1]);

                                    string empCode = Convert.ToString(dtExcel.Rows[i][0]);
                                    DataRow[] dremp = dtEmpCode.Select("emp_code='" + empCode + "'");
                                    if (dremp.Length > 0)
                                    {
                                        int.TryParse(dremp[0]["user_id"].ToString(), out userID);

                                        employeeUpload.userID = int.Parse(dremp[0]["user_id"].ToString());
                                        employeeUpload.empCode = dremp[0]["emp_code"].ToString();
                                        employeeUpload.empFullName = dremp[0]["fullname"].ToString();
                                        employeeUpload.departmentPositionName = dremp[0]["dept_position_name"].ToString();
                                        employeeUpload.isFix = int.Parse(isFix);
                                    }
                                    
                                    for (int j = 2; j < 31; j++)
                                    {
                                        WorkShiftUpload workShiftUpload = new WorkShiftUpload();
                                        int.TryParse(dtExcel.Rows[i][j - 1].ToString(), out day);

                                        string wsCode = Convert.ToString(dtExcel.Rows[i][j]);                                        
                                        string workDate = year.ToString() + "-" + month.ToString() + "-" + day.ToString();

                                        DataRow[] drwork = dtWorkShift.Select("ws_code='" + wsCode + "'");
                                        if (drwork.Length > 0)
                                        {
                                            int.TryParse(drwork[0]["id"].ToString(), out workShiftID);

                                            workShiftUpload.empWorkShiftID = int.Parse(drwork[0]["id"].ToString());
                                            workShiftUpload.wsCode = drwork[0]["ws_code"].ToString();
                                            workShiftUpload.timeStart = drwork[0]["time_start"].ToString();
                                            workShiftUpload.timeEnd = drwork[0]["time_end"].ToString();
                                            workShiftUpload.workDate = workDate;
                                        }
                                        employeeUpload.workShiftUpload.Add(workShiftUpload);
                                    }
                                    value.data.employeeUpload.Add(employeeUpload);
                                    //for (int j = 1; j <= countDate; j++)
                                    //{
                                    //    string work_shift = Convert.ToString(dtExcel.Rows[i][j].ToString());
                                    //    DataRow[] drwork = dtWorkShift.Select("ws_code='" + work_shift + "'");
                                    //    if (drwork.Length > 0)
                                    //    {
                                    //        int.TryParse(drwork[0]["id"].ToString(), out workShiftID);
                                    //    }
                                    //    emp_work_time objWork = new emp_work_time();
                                    //    objWork.user_id = userID;
                                    //    objWork.work_shift_id = workShiftID;
                                    //    objWork.work_date = Convert.ToDateTime(year.ToString() + '-' + month.ToString().PadLeft(2, '0') + '-' + j.ToString().PadLeft(2, '0'));
                                    //    objWork.is_fix = true;
                                    //    objEntity.emp_work_time.Add(objWork);
                                    //}
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

        [Route("1.0/save/empWorkShift")]
        [HttpPost]
        public IHttpActionResult SaveEmpWorkShift(SaveEmpWorkTimeRequestDTO saveEmpWorkTimeRequestDTO)
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
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SaveMasterPosition", json, timestampNow.ToString(), headersDTO,
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
                    if (masterDataDTO.deptID == 0)
                    {
                        checkMissingOptional += "deptID ";
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
                    if (masterDataDTO.deptID == 0)
                    {
                        checkMissingOptional += "deptID ";
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
                obj = srv.SaveMasterService(authHeader, lang, fromProject.ToLower(), logID, masterDataDTO, "system_position", data.userID, shareCode);

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
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "GetMasterPosition", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.GetPositionService(authHeader, lang, fromProject.ToLower(), logID, masterDataDTO.masterID, shareCode);
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
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(searchMasterDataDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SearchMasterDataPosition", json, timestampNow.ToString(), headersDTO,
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

                obj = srv.SearchMasterService(authHeader, lang, fromProject.ToLower(), logID, searchMasterDataDTO, "system_position", data.roleIDList, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/search/master/bodySet")]
        [HttpPost]
        public IHttpActionResult SearchMasterDataBodySet(SearchMasterDataDTO searchMasterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(searchMasterDataDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SearchMasterDataBodySet", json, timestampNow.ToString(), headersDTO,
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

                obj = srv.SearchMasterBodySetService(authHeader, lang, fromProject.ToLower(), logID, searchMasterDataDTO, "system_body_set", data.roleIDList, shareCode);

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
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(saveBodySetDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SaveBodySet", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());
                
                ValidateService validateService = new ValidateService();
                ValidationModel chkRequestBody = new ValidationModel();

                chkRequestBody = validateService.RequireOptionalBodySet(shareCode, lang, fromProject.ToLower(), logID, saveBodySetDTO);

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                obj = chkRequestBody;
                
                if (chkRequestBody.Success == true)
                {
                    if (saveBodySetDTO.mode.ToLower().Equals("insert"))
                    {
                        obj = srv.InsertBodySetService(authHeader, lang, fromProject.ToLower(), logID, saveBodySetDTO, data.roleIDList, data.userID, shareCode);
                    }
                    else if (saveBodySetDTO.mode.ToLower().Equals("update"))
                    {
                        obj = srv.UpdateBodySetService(authHeader, lang, fromProject.ToLower(), logID, saveBodySetDTO, data.roleIDList, data.userID, shareCode);
                    }
                    else if (saveBodySetDTO.mode.ToLower().Equals("delete"))
                    {
                        obj = srv.DeleteBodySetService(authHeader, lang, fromProject.ToLower(), logID, saveBodySetDTO, data.roleIDList, data.userID, shareCode);
                    }
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
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "GetBodySet", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.GetMasterBodySetervice(authHeader, lang, fromProject.ToLower(), logID, masterDataDTO.masterID, shareCode);
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

        [Route("1.0/save/master/department")]
        [HttpPost]
        public IHttpActionResult SaveMasterDepartment(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SaveMasterDepartment", json, timestampNow.ToString(), headersDTO,
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
                //else if (masterDataDTO.mode.ToLower().Equals("delete"))
                //{
                //    if (masterDataDTO.masterID == 0)
                //    {
                //        checkMissingOptional += "masterID ";
                //    }
                //}
                else
                {
                    throw new Exception("Choose Mode Insert or Update");
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                obj = srv.SaveMasterService(authHeader, lang, fromProject.ToLower(), logID, masterDataDTO, "system_department", data.userID, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/get/master/department")]
        [HttpPost]
        public IHttpActionResult GetDepartment(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "GetDepartment", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.GetMasterDepartmentService(authHeader, lang, fromProject.ToLower(), logID, masterDataDTO.masterID, shareCode);
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

        [Route("1.0/search/master/department")]
        [HttpPost]
        public IHttpActionResult SearchMasterDataDepartment(SearchMasterDataDTO searchMasterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(searchMasterDataDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SearchMasterDataDepartment", json, timestampNow.ToString(), headersDTO,
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
                if (searchMasterDataDTO.sortField > 2)
                {
                    throw new Exception("invalid : sortField " + searchMasterDataDTO.sortField);
                }
                if (!(searchMasterDataDTO.sortType == "a" || searchMasterDataDTO.sortType == "d" || searchMasterDataDTO.sortType == "A" || searchMasterDataDTO.sortType == "D" || searchMasterDataDTO.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchMasterDepartmentService(authHeader, lang, fromProject.ToLower(), logID, searchMasterDataDTO, "system_department", data.roleIDList, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/save/master/key")]
        [HttpPost]
        public IHttpActionResult SaveMasterKey(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SaveMasterKey", json, timestampNow.ToString(), headersDTO,
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
                    if (string.IsNullOrEmpty(masterDataDTO.keyName))
                    {
                        checkMissingOptional += "keyName ";
                    }
                    
                }
                else if (masterDataDTO.mode.ToLower().Equals("update"))
                {
                    if (masterDataDTO.masterID == 0)
                    {
                        checkMissingOptional += "masterID ";
                    }
                    if (string.IsNullOrEmpty(masterDataDTO.keyName))
                    {
                        checkMissingOptional += "keyName ";
                    }
                    
                }
                //else if (masterDataDTO.mode.ToLower().Equals("delete"))
                //{
                //    if (masterDataDTO.masterID == 0)
                //    {
                //        checkMissingOptional += "masterID ";
                //    }
                //}
                else
                {
                    throw new Exception("Choose Mode Insert or Update");
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                obj = srv.SaveMasterKeyService(authHeader, lang, fromProject.ToLower(), logID, masterDataDTO, "system_master_key", data.userID, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/get/master/key")]
        [HttpPost]
        public IHttpActionResult GetKey(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "GetKey", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (masterDataDTO.masterID != 0)
                {
                    obj = srv.GetMasterKeyService(authHeader, lang, fromProject.ToLower(), logID, masterDataDTO.masterID, shareCode);
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

        [Route("1.0/search/master/key")]
        [HttpPost]
        public IHttpActionResult SearchMasterkey(SearchMasterDataDTO searchMasterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(searchMasterDataDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SearchMasterKey", json, timestampNow.ToString(), headersDTO,
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
                if (searchMasterDataDTO.sortField > 2)
                {
                    throw new Exception("invalid : sortField " + searchMasterDataDTO.sortField);
                }
                if (!(searchMasterDataDTO.sortType == "a" || searchMasterDataDTO.sortType == "d" || searchMasterDataDTO.sortType == "A" || searchMasterDataDTO.sortType == "D" || searchMasterDataDTO.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchMasterKeyService(authHeader, lang, fromProject.ToLower(), logID, searchMasterDataDTO, "system_master_key", data.roleIDList, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/save/master/systemmaster")]
        [HttpPost]
        public IHttpActionResult SaveSystemMaster(SystemMasterDTO systemMasterDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            try
            {
                string json = JsonConvert.SerializeObject(systemMasterDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SaveSystemMaster", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (string.IsNullOrEmpty(systemMasterDTO.mode))
                {
                    throw new Exception("Missing Parameter : mode ");
                }

                if (systemMasterDTO.mode.ToLower().Equals("insert"))
                {
                    if (systemMasterDTO.masterID != 0)
                    {
                        checkMissingOptional += "masterID Must 0 ";
                    }
                    if (systemMasterDTO.keyID == 0)
                    {
                        checkMissingOptional += "keyID ";
                    }
                    if (systemMasterDTO.value == 0)
                    {
                        checkMissingOptional += "value ";
                    }
                    if (string.IsNullOrEmpty(systemMasterDTO.nameEN))
                    {
                        checkMissingOptional += "nameEN ";
                    }
                    if (string.IsNullOrEmpty(systemMasterDTO.nameTH))
                    {
                        checkMissingOptional += "nameTH ";
                    }
                    if (systemMasterDTO.order == 0)
                    {
                        checkMissingOptional += "order ";
                    }

                }
                else if (systemMasterDTO.mode.ToLower().Equals("update"))
                {
                    if (systemMasterDTO.masterID == 0)
                    {
                        checkMissingOptional += "masterID ";
                    }
                    if (systemMasterDTO.keyID == 0)
                    {
                        checkMissingOptional += "keyID ";
                    }
                    if (systemMasterDTO.value == 0)
                    {
                        checkMissingOptional += "value ";
                    }
                    if (string.IsNullOrEmpty(systemMasterDTO.nameEN))
                    {
                        checkMissingOptional += "nameEN ";
                    }
                    if (string.IsNullOrEmpty(systemMasterDTO.nameTH))
                    {
                        checkMissingOptional += "nameTH ";
                    }
                    if (systemMasterDTO.order == 0)
                    {
                        checkMissingOptional += "order ";
                    }

                }
                //else if (masterDataDTO.mode.ToLower().Equals("delete"))
                //{
                //    if (masterDataDTO.masterID == 0)
                //    {
                //        checkMissingOptional += "masterID ";
                //    }
                //}
                else
                {
                    throw new Exception("Choose Mode Insert or Update");
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                obj = srv.SaveSystemMasterService(authHeader, lang, fromProject.ToLower(), logID, systemMasterDTO, "system_master", data.userID, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/get/master/systemmaster")]
        [HttpPost]
        public IHttpActionResult GetSystemMaster(SystemMasterDTO systemMasterDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(systemMasterDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "GetSystemMaster", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (systemMasterDTO.masterID != 0)
                {
                    obj = srv.GetSystemMasterService(authHeader, lang, fromProject.ToLower(), logID, systemMasterDTO.masterID, shareCode);
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
        
        [Route("1.0/search/master/systemmaster")]
        [HttpPost]
        public IHttpActionResult SearchSystemMaster(SearchSystemMasterDTO searchSystemMasterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(searchSystemMasterDataDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SearchSystemMaster", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                MasterDataService srv = new MasterDataService();

                var obj = new object();

                if (searchSystemMasterDataDTO.pageInt.Equals(null) || searchSystemMasterDataDTO.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (searchSystemMasterDataDTO.perPage.Equals(null) || searchSystemMasterDataDTO.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }
                if (searchSystemMasterDataDTO.sortField > 2)
                {
                    throw new Exception("invalid : sortField " + searchSystemMasterDataDTO.sortField);
                }
                if (!(searchSystemMasterDataDTO.sortType == "a" || searchSystemMasterDataDTO.sortType == "d" || searchSystemMasterDataDTO.sortType == "A" || searchSystemMasterDataDTO.sortType == "D" || searchSystemMasterDataDTO.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchSystemMasterService(authHeader, lang, fromProject.ToLower(), logID, searchSystemMasterDataDTO, "system_master", data.roleIDList, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }




        #region Update Active Master
        [Route("1.0/update/master/active")]
        [HttpPost]
        public IHttpActionResult UpdateActiveMaster(MasterDataDTO masterDataDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            try
            {
                string tableName = "";

                
                if (masterDataDTO.mode == "department")
                {
                    tableName = "system_department";
                }
                else if (masterDataDTO.mode == "position")
                {
                    tableName = "system_position";
                }
                else if (masterDataDTO.mode == "masterkey")
                {
                    tableName = "system_master_key";
                }
                else if (masterDataDTO.mode == "systemmaster")
                {
                    tableName = "system_master";
                }

                string json = JsonConvert.SerializeObject(masterDataDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "UpdateActiveMaster", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (masterDataDTO.masterID == 0)
                {
                     checkMissingOptional += "masterID";
                }
               
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                MasterDataService srv = new MasterDataService();
                var obj = new object();
                
                if((tableName == "system_department" || tableName == "system_master_key") && masterDataDTO.IsActive == "0")
                {
                    var checkUse = _sql.CheckIsActiveService(tableName, masterDataDTO.masterID);
                    if (checkUse == 0)
                    {
                        obj = srv.UpdateActiveMasterService(authHeader, lang, fromProject.ToLower(), logID, masterDataDTO, tableName, data.userID, shareCode);
                    }
                    else
                    {
                        throw new Exception("Master is still active");
                    }
                }
                else
                {
                    obj = srv.UpdateActiveMasterService(authHeader, lang, fromProject.ToLower(), logID, masterDataDTO, tableName, data.userID, shareCode);

                }



                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }
        #endregion


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

        #region leave
        [Route("1.0/search/allleave")]
        [HttpPost]
        public IHttpActionResult GetSearchAllLeave(SearchLeaveDTO searchLeaveDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject("");
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SearchAllLeave", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                MasterDataService srv = new MasterDataService();
                var obj = new object();

                if (searchLeaveDTO.pageInt.Equals(null) || searchLeaveDTO.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (searchLeaveDTO.perPage.Equals(null) || searchLeaveDTO.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }

                if (searchLeaveDTO.sortField > 3)
                {
                    throw new Exception("invalid : sortField " + searchLeaveDTO.sortField);
                }
                if (!(searchLeaveDTO.sortType == "a" || searchLeaveDTO.sortType == "d" || searchLeaveDTO.sortType == "A" || searchLeaveDTO.sortType == "D" || searchLeaveDTO.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchAllLeave(authHeader, lang, fromProject.ToLower(), logID, searchLeaveDTO, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/get/leavedetail")]
        [HttpPost]
        public IHttpActionResult GetLeaveDetail(GetLeaveDetailRequestDTO leaveDetailDTO)
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

                if (leaveDetailDTO.leaveID.Equals(0) || leaveDetailDTO.leaveID.Equals(null))
                {
                    throw new Exception("Missing Parameter : leaveID");
                }

                var obj = srv.GetLeaveDetailService(authHeader, lang, fromProject.ToLower(), 1, leaveDetailDTO, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/save/LeaveDetail")]
        [HttpPost]
        public IHttpActionResult SaveLeaveDetail(SaveLeaveDetailDTO saveLeaveDetailDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

            try
            {
                string json = JsonConvert.SerializeObject(saveLeaveDetailDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SaveLeaveDetail", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                ValidateService validateService = new ValidateService();
                ValidationModel chkRequestBody = validateService.RequireOptionalSaveLeaveDetail(shareCode, lang, fromProject.ToLower(), logID, saveLeaveDetailDTO);


                InsertService srvInsert = new InsertService();
                UpdateService srvUpdate = new UpdateService();
                //DeleteService srvDelete = new DeleteService();
                var obj = new object();

                if (chkRequestBody.Success == true)
                {
                    if (saveLeaveDetailDTO.leaveId.Equals(0) && saveLeaveDetailDTO.mode.ToLower() == "insert")
                    {
                        obj = srvInsert.InsertLeaveDetailService(authHeader, lang, fromProject.ToLower(), logID, saveLeaveDetailDTO, data.roleIDList, data.userID, shareCode);
                    }
                    else if (saveLeaveDetailDTO.leaveId > 0 && saveLeaveDetailDTO.mode.ToLower() == "update")
                    {
                        obj = srvUpdate.UpdateLeaveDetailService(authHeader, lang, fromProject.ToLower(), logID, saveLeaveDetailDTO, data.roleIDList, data.userID, shareCode);
                    }
                    //else if (saveLeaveDetailDTO.leaveId > 0 && saveLeaveDetailDTO.mode.ToLower() == "delete")
                    //{
                        //obj = srvDelete.DeleteEmpWorkShiftService(authHeader, lang, fromProject.ToLower(), logID, saveLeaveDetailDTO, data.roleIDList, data.userID);
                    //}
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/cancel/leaveForm")]
        [HttpPost]
        public IHttpActionResult CancelLeaveForm(ActionLeaveFormDTO actionLeaveFormDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);
            try
            {
                string json = JsonConvert.SerializeObject(actionLeaveFormDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "CancelLeaveForm", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                #region msgCheck
                string msgCheck = "";
                if (actionLeaveFormDTO.leaveID.Equals(0) || actionLeaveFormDTO.leaveID.Equals(null))
                {
                    msgCheck += "leaveID ";
                }
                if (string.IsNullOrEmpty(actionLeaveFormDTO.cancelReason))
                {
                    msgCheck += "cancelReason ";
                }

                if (!string.IsNullOrEmpty(msgCheck))
                {
                    throw new Exception(msgCheck);
                }
                #endregion

                UpdateService srv = new UpdateService();
                var obj = srv.CancelLeaveFormService(authHeader, lang, fromProject.ToLower(), logID, actionLeaveFormDTO, data.userID, shareCode);
                return Ok(obj);
            }
            catch(Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/reject/leaveForm")]
        [HttpPost]
        public IHttpActionResult RejectLeaveForm(ActionLeaveFormDTO actionLeaveFormDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);
            try
            {
                string json = JsonConvert.SerializeObject(actionLeaveFormDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "RejectLeaveForm", json, timestampNow.ToString(), headersDTO,
                   data.userID, fromProject.ToLower());

                #region msgCheck
                string msgCheck = "";
                if (actionLeaveFormDTO.leaveID.Equals(0) || actionLeaveFormDTO.leaveID.Equals(null))
                {
                    msgCheck += "leaveID ";
                }
                if (string.IsNullOrEmpty(actionLeaveFormDTO.rejectReason))
                {
                    msgCheck += "rejectReason ";
                }

                if (!string.IsNullOrEmpty(msgCheck))
                {
                    throw new Exception(msgCheck);
                }
                #endregion

                UpdateService srv = new UpdateService();
                var obj = srv.RejectLeaveFormService(authHeader, lang, fromProject.ToLower(), logID, actionLeaveFormDTO, data.userID, shareCode);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/approve/leaveForm")]
        [HttpPost]
        public IHttpActionResult ApproveLeaveForm(ActionLeaveFormDTO actionLeaveFormDTO)
        {
            var request = HttpContext.Current.Request;
            string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
            string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
            string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
            string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

            HeadersDTO headersDTO = new HeadersDTO();
            headersDTO.authHeader = authHeader;
            headersDTO.lang = lang;
            headersDTO.fromProject = fromProject;
            headersDTO.shareCode = shareCode;

            AuthenticationController _auth = AuthenticationController.Instance;
            AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);
            try
            {
                string json = JsonConvert.SerializeObject(actionLeaveFormDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "ApproveLeaveForm", json, timestampNow.ToString(), headersDTO,
                   data.userID, fromProject.ToLower());

                if (actionLeaveFormDTO.leaveID.Equals(null) || actionLeaveFormDTO.leaveID.Equals(0))
                {
                    throw new Exception("leaveID ");
                }

                UpdateService srv = new UpdateService();
                var obj = srv.ApproveLeaveFormService(authHeader, lang, fromProject.ToLower(), logID, actionLeaveFormDTO, data.userID, shareCode);
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

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
using static TUFTManagement.DTO.SaveEmpWorkTimeRequestDTO_V1_1;

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

        [Route("1.0/get/accessRole")]
        [HttpGet]
        public IHttpActionResult GetAccessRole()
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
                int logID = _sql.InsertLogReceiveData("GetAccessRole", json, timestampNow.ToString(), authHeader,
                    0, fromProject.ToLower());
                LoginService srv = new LoginService();

                var obj = srv.GetAccessRole(authHeader, lang, fromProject.ToLower(), shareCode, data.userID, logID);
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
                else if (getDropdownRequestDTO.moduleName.ToLower() == "titlename".ToLower())
                {
                    obj = srv.GetTitleNameDropdownService(authHeader, lang, fromProject.ToLower(), logID, getDropdownRequestDTO);
                }
                else if (getDropdownRequestDTO.moduleName.ToLower() == "positionFilter".ToLower())
                {
                    if (getDropdownRequestDTO.departmentID != 0)
                    {
                        obj = srv.GetPositionByDepartmentDropdownService(authHeader, lang, fromProject.ToLower(), logID, getDropdownRequestDTO);
                    }
                    else
                    {
                        throw new Exception("Missing Parameter : departmentID");
                    }
                }
                else if (getDropdownRequestDTO.moduleName.ToLower() == "empTradeWorkShift".ToLower())
                {
                    if (string.IsNullOrEmpty(getDropdownRequestDTO.workDate))
                    {
                        throw new Exception("Missing Parameter : workDate");
                    }
                    else
                    {
                        obj = srv.GetEmpTradeWorkShiftDropdownService(shareCode, authHeader, lang, fromProject.ToLower(), logID, getDropdownRequestDTO);
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
                var obj = srv.DeleteEmpFileService(shareCode, authHeader, lang, fromProject.ToLower(), logID, requestDTO, data.userID);

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

        #region systemrole
        [Route("1.0/save/systemroleAssign")]
        [HttpPost]
        public IHttpActionResult SaveSystemRoleAssign(SaveSystemRoleAssignDTO saveSystemRoleAssignDTO)
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
                string json = JsonConvert.SerializeObject(saveSystemRoleAssignDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SaveSystemRoleAssign", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                ValidateService validateService = new ValidateService();
                ValidationModel chkRequestBody = validateService.RequireOptionalSaveSystemRoleAssign(shareCode, lang, fromProject.ToLower(), logID, saveSystemRoleAssignDTO);

                int _chkPosition = _sql.CheckPositionID(saveSystemRoleAssignDTO.positionID);

                if (_chkPosition != 0)
                {
                    InsertService srvInsert = new InsertService();
                    UpdateService srvUpdate = new UpdateService();
                    var obj = new object();

                    if (chkRequestBody.Success == true)
                    {
                        if (saveSystemRoleAssignDTO.id == 0)
                        {
                            obj = srvInsert.InsertSystemRoleAssignService(authHeader, lang, fromProject.ToLower(), logID, saveSystemRoleAssignDTO, data.userID, shareCode);
                        }
                        else
                        {
                            obj = srvUpdate.UpdateSystemRoleAssignService(authHeader, lang, fromProject.ToLower(), logID, saveSystemRoleAssignDTO, data.userID, shareCode);
                        }

                    }

                    return Ok(obj);
                }
                else
                {
                    throw new Exception("Don't have this position ID");
                }


            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/cancel/systemroleAssign")]
        [HttpPost]
        public IHttpActionResult CancelSystemRoleAssign(SaveSystemRoleAssignDTO saveSystemRoleAssignDTO)
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
                string json = JsonConvert.SerializeObject(saveSystemRoleAssignDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "CancelSystemRoleAssign", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                ValidateService validateService = new ValidateService();
                ValidationModel chkRequestBody = validateService.RequireOptionalSaveSystemRoleAssign(shareCode, lang, fromProject.ToLower(), logID, saveSystemRoleAssignDTO);

                string checkMissingOptional = "";

                if (saveSystemRoleAssignDTO.id == null || saveSystemRoleAssignDTO.id == 0)
                {
                    throw new Exception("Missing Parameter : id ");
                }
                if (saveSystemRoleAssignDTO.isActive == null )
                {
                    throw new Exception("Missing Parameter : isActive ");
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                var obj = new object();
                UpdateService update = new UpdateService();
                obj = update.UpdateActiveSystemRoleAssignService(authHeader, lang, fromProject, logID, saveSystemRoleAssignDTO, data.userID, shareCode);


                return Ok(obj);
               


            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }


        [Route("1.0/search/systemroleAssign")]
        [HttpPost]
        public IHttpActionResult SearchSystemRoleAssign(SearchSystemRoleAssignDTO searchSystemRoleAssignDTO)
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
                string json = JsonConvert.SerializeObject(searchSystemRoleAssignDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SearchSystemRoleAssign", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                GetService srv = new GetService();

                var obj = new object();

                if (searchSystemRoleAssignDTO.pageInt.Equals(null) || searchSystemRoleAssignDTO.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (searchSystemRoleAssignDTO.perPage.Equals(null) || searchSystemRoleAssignDTO.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }
                if (searchSystemRoleAssignDTO.sortField > 4)
                {
                    throw new Exception("invalid : sortField " + searchSystemRoleAssignDTO.sortField);
                }
                if (!(searchSystemRoleAssignDTO.sortType == "a" || searchSystemRoleAssignDTO.sortType == "d" || searchSystemRoleAssignDTO.sortType == "A" || searchSystemRoleAssignDTO.sortType == "D" || searchSystemRoleAssignDTO.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchAllSystemRoleAssign(authHeader, lang, fromProject.ToLower(), logID, searchSystemRoleAssignDTO, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/get/systemroleAssign")]
        [HttpPost]
        public IHttpActionResult GetDetailSystemRoleAssign(SaveSystemRoleAssignDTO saveSystemRoleAssignDTO)
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
                string json = JsonConvert.SerializeObject(saveSystemRoleAssignDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "GetDetailSystemRoleAssign", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                GetService srv = new GetService();

                var obj = new object();

                if (saveSystemRoleAssignDTO.id != 0)
                {
                    obj = srv.GetDetailSystemRoleAssignService(authHeader, lang, fromProject.ToLower(), logID, saveSystemRoleAssignDTO.id, shareCode);
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

        [Route("1.0/save/systemroleTemp")]
        [HttpPost]
        public IHttpActionResult SaveSystemRoleTemp(SaveSystemRoleTempDTO saveSystemRoleTempDTO)
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
                string json = JsonConvert.SerializeObject(saveSystemRoleTempDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SaveSystemRoleTemp", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                ValidateService validateService = new ValidateService();
                ValidationModel chkRequestBody = validateService.RequireOptionalSaveSystemRoleTemp(shareCode, lang, fromProject.ToLower(), logID, saveSystemRoleTempDTO);

                //int _chkPosition = _sql.CheckPositionID(saveSystemRoleTempDTO.positionID);

                int chkDupObjID = _sql.CheckDuplicateObjID(saveSystemRoleTempDTO.objectID, fromProject.ToLower(), shareCode);
                int chkParent = _sql.CheckDuplicateObjID(saveSystemRoleTempDTO.parentID, fromProject.ToLower(), shareCode);


                InsertService srvInsert = new InsertService();
                UpdateService srvUpdate = new UpdateService();
                var obj = new object();

                if (chkRequestBody.Success == true)
                {
                    if(chkParent > 0 || saveSystemRoleTempDTO.parentID == 0)
                    {
                        if (chkDupObjID == 0)
                        {
                            obj = srvInsert.InsertSystemRoleTempService(authHeader, lang, fromProject.ToLower(), logID, saveSystemRoleTempDTO, data.userID, shareCode);
                        }
                        else
                        {
                            obj = srvUpdate.UpdateSystemRoleTempService(authHeader, lang, fromProject.ToLower(), logID, saveSystemRoleTempDTO, data.userID, shareCode);
                        }
                    }
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/cancel/systemroleTemp")]
        [HttpPost]
        public IHttpActionResult CancelSystemRoleTemp(SaveSystemRoleTempDTO saveSystemRoleTempDTO)
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
                string json = JsonConvert.SerializeObject(saveSystemRoleTempDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "CancelSystemRoleTemp", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                ValidateService validateService = new ValidateService();
                ValidationModel chkRequestBody = validateService.RequireOptionalSaveSystemRoleTemp(shareCode, lang, fromProject.ToLower(), logID, saveSystemRoleTempDTO);

                string checkMissingOptional = "";

                if (saveSystemRoleTempDTO.objectID == null || saveSystemRoleTempDTO.objectID == 0)
                {
                    throw new Exception("Missing Parameter : id ");
                }
                if (saveSystemRoleTempDTO.parentID == null)
                {
                    throw new Exception("Missing Parameter : parentID ");
                }
                if (saveSystemRoleTempDTO.isActive == null)
                {
                    throw new Exception("Missing Parameter : isActive ");
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                var obj = new object();
                UpdateService update = new UpdateService();
                obj = update.UpdateActiveSystemRoleTempService(authHeader, lang, fromProject, logID, saveSystemRoleTempDTO, data.userID, shareCode);


                return Ok(obj);



            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }


        [Route("1.0/search/systemroleTemp")]
        [HttpPost]
        public IHttpActionResult SearchSystemRoleTemp(SearchSystemRoleTempDTO searchSystemRoleTempDTO)
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
                string json = JsonConvert.SerializeObject(searchSystemRoleTempDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SearchSystemRoleTempDTO", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                GetService srv = new GetService();

                var obj = new object();

                if (searchSystemRoleTempDTO.pageInt.Equals(null) || searchSystemRoleTempDTO.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (searchSystemRoleTempDTO.perPage.Equals(null) || searchSystemRoleTempDTO.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }
                if (searchSystemRoleTempDTO.sortField > 4)
                {
                    throw new Exception("invalid : sortField " + searchSystemRoleTempDTO.sortField);
                }
                if (!(searchSystemRoleTempDTO.sortType == "a" || searchSystemRoleTempDTO.sortType == "d" || searchSystemRoleTempDTO.sortType == "A" || searchSystemRoleTempDTO.sortType == "D" || searchSystemRoleTempDTO.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchAllSystemRoleTemp(authHeader, lang, fromProject.ToLower(), logID, searchSystemRoleTempDTO, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/get/systemroleTemp")]
        [HttpPost]
        public IHttpActionResult GetDetailSystemRoleTemp(SaveSystemRoleTempDTO saveSystemRoleTempDTO)
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
                string json = JsonConvert.SerializeObject(saveSystemRoleTempDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "GetDetailSystemRoleTemp", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                GetService srv = new GetService();

                var obj = new object();

                if (saveSystemRoleTempDTO.objectID != 0)
                {
                    obj = srv.GetDetailSystemRoleTempService(authHeader, lang, fromProject.ToLower(), logID, saveSystemRoleTempDTO, shareCode);
                }
                else
                {
                    throw new Exception("Missing Parameter : ObjectID ");
                }


                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }


        #endregion

        //[Route("1.0/save/systemrole")]
        //[HttpPost]
        //public IHttpActionResult SaveSystemRole(SaveSystemRoleAssignDTO saveSystemRoleAssignDTO)
        //{
        //    var request = HttpContext.Current.Request;
        //    string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
        //    string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
        //    string fromProject = (request.Headers["Fromproject"] == null ? "" : request.Headers["Fromproject"]);
        //    string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

        //    HeadersDTO headersDTO = new HeadersDTO();
        //    headersDTO.authHeader = authHeader;
        //    headersDTO.lang = lang;
        //    headersDTO.fromProject = fromProject;
        //    headersDTO.shareCode = shareCode;

        //    AuthenticationController _auth = AuthenticationController.Instance;
        //    AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

        //    try
        //    {
        //        string json = JsonConvert.SerializeObject(saveSystemRoleAssignDTO);
        //        int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SaveSystemRoleAssign", json, timestampNow.ToString(), headersDTO,
        //            data.userID, fromProject.ToLower());

        //        ValidateService validateService = new ValidateService();
        //        ValidationModel chkRequestBody = validateService.RequireOptionalSaveSystemRole(shareCode, lang, fromProject.ToLower(), logID, saveSystemRoleAssignDTO);

        //        int _chkPosition = _sql.CheckPositionID(saveSystemRoleAssignDTO.positionID);

        //        if(_chkPosition != 0)
        //        {
        //            InsertService srvInsert = new InsertService();
        //            UpdateService srvUpdate = new UpdateService();
        //            var obj = new object();

        //            if (chkRequestBody.Success == true)
        //            {
        //                foreach (SaveSystemRoleTemp item in saveSystemRoleAssignDTO.listTemp)
        //                {
        //                    int _chkDup = _sql.CheckDuplicateObjID(item.objID, fromProject.ToLower(), shareCode);
        //                    int _chkParent = _sql.CheckDuplicateObjID(item.parentID, fromProject.ToLower(), shareCode);

        //                    int _chkDupPosition = _sql.CheckPositionIDAssignment(item.objID, saveSystemRoleAssignDTO.positionID, shareCode);

        //                    if ((_chkDup == 0 && _chkParent > 0) || (_chkDup == 0 && item.parentID == "0"))
        //                    {
        //                        obj = srvInsert.InsertSystemRoleTempService(authHeader, lang, fromProject.ToLower(), logID, item,  data.userID, shareCode);

        //                    }
        //                    else if ((_chkDup > 0 && _chkParent > 0) || (_chkDup > 0 && item.parentID == "0"))
        //                    {
        //                        obj = srvUpdate.UpdateSystemRoleTempService(authHeader, lang, fromProject.ToLower(), logID, item, data.userID, shareCode);

        //                    }

        //                    if (_chkDupPosition == 0 )
        //                    {
        //                        obj = srvInsert.InsertSystemRoleAssignService(authHeader, lang, fromProject.ToLower(), logID, saveSystemRoleAssignDTO, item,  data.userID, shareCode);

        //                    }
        //                    else
        //                    {
        //                        obj = srvUpdate.UpdateSystemRoleAssignService(authHeader, lang, fromProject.ToLower(), logID, saveSystemRoleAssignDTO, item,  data.userID, shareCode);

        //                    }
        //                }

        //            }

        //            return Ok(obj);
        //        }
        //        else
        //        {
        //            throw new Exception("Don't have this position ID");
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
        //    }
        //}


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

                // prepair imageGalleryCode
                Random rnd = new Random();
                int passwordRandom = rnd.Next(100000, 999999);
                string fileCode = Utility.MD5Hash(passwordRandom.ToString() + "_" + saveEmpProfileDTO.firstNameEN).ToUpper();
                saveEmpProfileDTO.imageGalleryCode = fileCode;

                foreach (int item in saveEmpProfileDTO.imageGallery)
                {
                    _sql.UpdateFileDetails(shareCode, item, fileCode, data.userID);
                }
                

                var obj = new object();
                if (chkRequestBody.Success == true)
                {
                    if (saveEmpProfileDTO.empProfileID.Equals(0) && saveEmpProfileDTO.mode.ToLower() == "insert")
                    {
                        InsertService srv = new InsertService();
                        obj = srv.InsertEmpProfileService(shareCode, authHeader, lang, fromProject.ToLower(), logID, saveEmpProfileDTO,  data.userID);
                    }
                    else if (saveEmpProfileDTO.empProfileID > 0 && saveEmpProfileDTO.mode.ToLower() == "update")
                    {
                        UpdateService srv = new UpdateService();
                        obj = srv.UpdateEmpProfileService(shareCode, authHeader, lang, fromProject.ToLower(), logID, saveEmpProfileDTO,  data.userID);
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
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "GetEmpProfileV1_1", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                GetService srv = new GetService();

                var obj = srv.GetEmpProfileV1_1Service(shareCode, authHeader, lang, fromProject.ToLower(), logID, data.userID);
                
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
                var obj = srv.DeleteEmpProfileService(authHeader, lang, fromProject.ToLower(), logID, saveEmpProfileDTO,  data.userID);

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
                    obj = srv.InsertEmpRateService(authHeader, lang, fromProject.ToLower(), logID, saveEmpRateDTO,  data.userID);
                }
                else
                {
                    obj = srv2.UpdateEmpRateService(shareCode, authHeader, lang, fromProject.ToLower(), logID, saveEmpRateDTO,  data.userID);
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
                var obj = srv.DeleteEmpRateService(authHeader, lang, fromProject.ToLower(), logID, empRateRequestDTO,  data.userID);

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
                //if (string.IsNullOrEmpty(saveEmpStatusDTO.imageEmploymentCode))
                //{
                //    checkMissingOptional += "imageEmploymentCode ";
                //}

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }

                UpdateService srv = new UpdateService();
                var obj = srv.UpdateEmpStatusService(shareCode, authHeader, lang, fromProject.ToLower(), logID, saveEmpStatusDTO,  data.userID);

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
                        obj = srvInsert.InsertEmpWorkShiftService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkShiftRequestDTO,  data.userID);
                    }
                    else if (saveEmpWorkShiftRequestDTO.empWorkShiftID > 0 && saveEmpWorkShiftRequestDTO.mode.ToLower() == "update")
                    {
                        obj = srvUpdate.UpdateEmpWorkShiftService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkShiftRequestDTO,  data.userID);
                    }
                    else if (saveEmpWorkShiftRequestDTO.empWorkShiftID > 0 && saveEmpWorkShiftRequestDTO.mode.ToLower() == "delete")
                    {
                        obj = srvDelete.DeleteEmpWorkShiftService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkShiftRequestDTO,  data.userID);
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
                var obj = srv.DeleteEmpWorkShiftService(authHeader, lang, fromProject.ToLower(), logID, requestDTO,  data.userID);

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

                            DataTable dtEmpCode = _sql.GetAllEmpCode(shareCode, lang);
                            DataTable dtWorkShift = _sql.GetAllWorkShift(shareCode);

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

                                    for (int j = 2; j < countDate + 2; j++)
                                    {
                                        WorkShiftUpload workShiftUpload = new WorkShiftUpload();
                                        int.TryParse(dtExcel.Rows[2][j].ToString(), out day);

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
                                            workShiftUpload.workDate = Utility.convertToDateServiceFormatString(workDate);
                                        }
                                        employeeUpload.workShiftUpload.Add(workShiftUpload);
                                    }
                                    value.data.employeeUpload.Add(employeeUpload);
                                    
                                }

                                value.success = true;
                                value.msg = new MsgModel() { code = 0, text = "The Excel file has been successfully uploaded.", topic = "Success" };
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

        [Route("1.0/get/empWorkShiftHeader")]
        [HttpPost]
        public IHttpActionResult GetEmpWorkShiftHeader(SaveEmpWorkShiftRequestDTO requestDTO)
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
                int logID = _sql.InsertLogReceiveData("GetEmpWorkShiftTimeHeader", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                GetService srv = new GetService();

                if (requestDTO.empWorkTimeID.Equals(0) || requestDTO.empWorkTimeID.Equals(null))
                {
                    throw new Exception("Missing Parameter : empWorkTimeID");
                }

                var obj = srv.GetEmpWorkShiftHeaderService(authHeader, lang, fromProject.ToLower(), 1, requestDTO.empWorkTimeID, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }


        #region 1.1 Time Attendance

        [Route("1.1/save/empWorkShift")]
        [HttpPost]
        public IHttpActionResult SaveEmpWorkShift_V1_1(SaveEmpWorkTimeRequestDTO_V1_1 saveEmpWorkTimeRequestDTO_V1_1)
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
                string json = JsonConvert.SerializeObject(saveEmpWorkTimeRequestDTO_V1_1);
                int logID = _sql.InsertLogReceiveData("SaveEmpWorkShift_V1_1", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (saveEmpWorkTimeRequestDTO_V1_1.empWorkTimeRequestDTO.Count > 0)
                {
                    foreach (EmpWorkTimeRequestDTO item in saveEmpWorkTimeRequestDTO_V1_1.empWorkTimeRequestDTO)
                    {
                        int countRecord = 1;
                        if (item.userID.Equals(0) || item.userID.Equals(null))
                        {
                            checkMissingOptional += "userID [" + countRecord + "] ";
                        }
                        if (string.IsNullOrEmpty(item.workDate))
                        {
                            checkMissingOptional += "workDate [" + countRecord + "] ";
                        }
                        if (item.workShiftID.Equals(0) || item.workShiftID.Equals(null))
                        {
                            checkMissingOptional += "workshiftID [" + countRecord + "] ";
                        }
                        if (string.IsNullOrEmpty(item.workDate))
                        {
                            checkMissingOptional += "workDate [" + countRecord + "] ";
                        }
                        countRecord++;
                    }
                }
                else
                {
                    checkMissingOptional += "empWorkTimeRequestDTO[]";
                }
                

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {

                    InsertService srv = new InsertService();

                    if (checkMissingOptional == "" && saveEmpWorkTimeRequestDTO_V1_1.empWorkTimeRequestDTO.Count > 0)
                    {
                        obj = srv.InsertEmpWorkTimeV1_1Service(shareCode, authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkTimeRequestDTO_V1_1,  data.userID);
                    }
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.1/update/empWorkShift")]
        [HttpPost]
        public IHttpActionResult UpdateEmpWorkShift_V1_1(SaveEmpWorkTimeRequestDTO saveEmpWorkTimeRequestDTO)
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
                int logID = _sql.InsertLogReceiveData("UpdateEmpWorkShift_V1_1", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                if (saveEmpWorkTimeRequestDTO.empWorkTimeID > 0)
                {
                    
                        if (saveEmpWorkTimeRequestDTO.empID.Equals(0) || saveEmpWorkTimeRequestDTO.empID.Equals(null))
                        {
                            checkMissingOptional += "empID ";
                        }
                        if (string.IsNullOrEmpty(saveEmpWorkTimeRequestDTO.workDate))
                        {
                            checkMissingOptional += "workDate ";
                        }
                        if (saveEmpWorkTimeRequestDTO.empWorkShiftID.Equals(0) || saveEmpWorkTimeRequestDTO.empWorkShiftID.Equals(null))
                        {
                            checkMissingOptional += "empWorkShiftID ";
                        }
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {

                    UpdateService srv = new UpdateService();

                    if (checkMissingOptional == "" && saveEmpWorkTimeRequestDTO.empWorkTimeID > 0)
                    {
                        obj = srv.UpdateEmpWorkTimeNewVerService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkTimeRequestDTO, data.userID, shareCode);
                    }
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/get/emphistoryworkshift")]
        [HttpPost]
        public IHttpActionResult GetHistoryWorkShiftTime(GetHistoryWorkShiftTimeDTO getHistoryWorkShiftTimeDTO)
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
                string json = JsonConvert.SerializeObject(getHistoryWorkShiftTimeDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "GetHistoryWorkShiftTime", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                GetService srv = new GetService();

                var obj = new object();

                if (getHistoryWorkShiftTimeDTO.pageInt.Equals(null) || getHistoryWorkShiftTimeDTO.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (getHistoryWorkShiftTimeDTO.perPage.Equals(null) || getHistoryWorkShiftTimeDTO.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }
                if (getHistoryWorkShiftTimeDTO.sortField > 2)
                {
                    throw new Exception("invalid : sortField " + getHistoryWorkShiftTimeDTO.sortField);
                }
                if (!(getHistoryWorkShiftTimeDTO.sortType == "a" || getHistoryWorkShiftTimeDTO.sortType == "d" || getHistoryWorkShiftTimeDTO.sortType == "A" || getHistoryWorkShiftTimeDTO.sortType == "D" || getHistoryWorkShiftTimeDTO.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.GetEmpWorkShiftTimeService(authHeader, lang, fromProject.ToLower(), logID, getHistoryWorkShiftTimeDTO,  shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }



        #endregion


        #region wait approve zone

        [Route("1.0/save/changeWorkShiftTime")]
        [HttpPost]
        public IHttpActionResult SaveChangeWorkShiftTime(SaveChangeWorkShiftTimeRequestDTO saveChangeWorkShiftTimeRequestDTO)
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
                string json = JsonConvert.SerializeObject(saveChangeWorkShiftTimeRequestDTO);
                int logID = _sql.InsertLogReceiveData("SaveChangeWorkShiftTime", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";


                if (saveChangeWorkShiftTimeRequestDTO.empWorkTimeID.Equals(0) || saveChangeWorkShiftTimeRequestDTO.empWorkTimeID.Equals(null))
                {
                    checkMissingOptional += "empWorkTimeID ";
                }
                if (saveChangeWorkShiftTimeRequestDTO.userID.Equals(0) || saveChangeWorkShiftTimeRequestDTO.userID.Equals(null))
                {
                    checkMissingOptional += "userID ";
                }
                if (saveChangeWorkShiftTimeRequestDTO.workShiftID.Equals(0) || saveChangeWorkShiftTimeRequestDTO.workShiftID.Equals(null))
                {
                    checkMissingOptional += "workShiftID ";
                }

                if (saveChangeWorkShiftTimeRequestDTO.newEmpWorkTimeID.Equals(0) || saveChangeWorkShiftTimeRequestDTO.newEmpWorkTimeID.Equals(null))
                {
                    checkMissingOptional += "newEmpWorkTimeID ";
                }
                if (saveChangeWorkShiftTimeRequestDTO.newUserID.Equals(0) || saveChangeWorkShiftTimeRequestDTO.newUserID.Equals(null))
                {
                    checkMissingOptional += "newUserID ";
                }
                if (saveChangeWorkShiftTimeRequestDTO.newWorkShiftID.Equals(0) || saveChangeWorkShiftTimeRequestDTO.newWorkShiftID.Equals(null))
                {
                    checkMissingOptional += "newWorkShiftID ";
                }

                if (saveChangeWorkShiftTimeRequestDTO.remark.Equals(0) || saveChangeWorkShiftTimeRequestDTO.remark.Equals(null))
                {
                    checkMissingOptional += "remark ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {
                    InsertService srv = new InsertService();
                    obj = srv.InsertEmpWorkShiftTimeTransChangeService(shareCode, authHeader, lang, fromProject.ToLower(), logID, saveChangeWorkShiftTimeRequestDTO,  data.userID);
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/search/worktime/pending")]
        [HttpPost]
        public IHttpActionResult SearchWorkTimePending(SearchWorkTimePendingDTO searchWorkTimePendingDTO)
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
                string json = JsonConvert.SerializeObject(searchWorkTimePendingDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SearchWorkTimePending", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                GetService srv = new GetService();
                var obj = new object();

                
                if (searchWorkTimePendingDTO.pageInt.Equals(null) || searchWorkTimePendingDTO.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (searchWorkTimePendingDTO.perPage.Equals(null) || searchWorkTimePendingDTO.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }
                if (searchWorkTimePendingDTO.sortField > 2)
                {
                    throw new Exception("invalid : sortField " + searchWorkTimePendingDTO.sortField);
                }
                if (!(searchWorkTimePendingDTO.sortType == "a" || searchWorkTimePendingDTO.sortType == "d" || searchWorkTimePendingDTO.sortType == "A" || searchWorkTimePendingDTO.sortType == "D" || searchWorkTimePendingDTO.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchWorkTimePendingService(authHeader, lang, fromProject.ToLower(), logID, searchWorkTimePendingDTO, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/Approve/changeWorkShiftTime")]
        [HttpPost]
        public IHttpActionResult ApproveChangeWorkShiftTime(ApproveChangeWorkShiftTimeRequestDTO approveChangeWorkShiftTimeRequestDTO)
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
                string json = JsonConvert.SerializeObject(approveChangeWorkShiftTimeRequestDTO);
                int logID = _sql.InsertLogReceiveData("ApproveChangeWorkShiftTime", json, timestampNow.ToString(), authHeader,
                    data.userID, fromProject.ToLower());

                string checkMissingOptional = "";

                string strApproveListEmpWorkTimeID = JsonConvert.SerializeObject(approveChangeWorkShiftTimeRequestDTO.approveListEmpWorkTimeID);
                strApproveListEmpWorkTimeID = string.Join(",", approveChangeWorkShiftTimeRequestDTO.approveListEmpWorkTimeID);
                approveChangeWorkShiftTimeRequestDTO.prepairApproveListEmpWorkTimeID = strApproveListEmpWorkTimeID;

                string strRejectListEmpWorkTimeID = JsonConvert.SerializeObject(approveChangeWorkShiftTimeRequestDTO.rejectListEmpWorkTimeID);
                strRejectListEmpWorkTimeID = string.Join(",", approveChangeWorkShiftTimeRequestDTO.rejectListEmpWorkTimeID);
                approveChangeWorkShiftTimeRequestDTO.prepairRejectListEmpWorkTimeID = strRejectListEmpWorkTimeID;

                if (string.IsNullOrEmpty(approveChangeWorkShiftTimeRequestDTO.prepairApproveListEmpWorkTimeID))
                {
                    checkMissingOptional += "approveListEmpWorkTimeID[] ";
                }

                if (string.IsNullOrEmpty(approveChangeWorkShiftTimeRequestDTO.prepairRejectListEmpWorkTimeID))
                {
                    checkMissingOptional += "rejectListEmpWorkTimeID[] ";
                }
                
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {
                    InsertService srv = new InsertService();
                    obj = srv.ApproveEmpWorkShiftTimeTransChangeService(shareCode, authHeader, lang, fromProject.ToLower(), logID, approveChangeWorkShiftTimeRequestDTO,  data.userID);
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        #endregion



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
                        obj = srv.UpdateEmpWorkTimeService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkTimeRequestDTO,  data.userID);
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
                        obj = srv.UpdateEmpWorkTimeService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkTimeRequestDTO,  data.userID);
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
                        obj = srv.UpdateEmpWorkTimeService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkTimeRequestDTO,  data.userID);
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
                        obj = srv.UpdateEmpWorkTimeService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkTimeRequestDTO,  data.userID);
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
                        obj = srv.UpdateEmpWorkTimeService(authHeader, lang, fromProject.ToLower(), logID, saveEmpWorkTimeRequestDTO,  data.userID);
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
                        obj = srv.ApproveWorkTimeTransChangeService(authHeader, lang, fromProject.ToLower(), logID, transChangeRequestDTO,  data.userID);
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

                obj = srv.SearchMasterService(authHeader, lang, fromProject.ToLower(), logID, searchMasterDataDTO, "system_position",  shareCode);

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

                obj = srv.SearchMasterBodySetService(authHeader, lang, fromProject.ToLower(), logID, searchMasterDataDTO, "system_body_set", shareCode);

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
                        obj = srv.InsertBodySetService(authHeader, lang, fromProject.ToLower(), logID, saveBodySetDTO,  data.userID, shareCode);
                    }
                    else if (saveBodySetDTO.mode.ToLower().Equals("update"))
                    {
                        obj = srv.UpdateBodySetService(authHeader, lang, fromProject.ToLower(), logID, saveBodySetDTO,  data.userID, shareCode);
                    }
                    else if (saveBodySetDTO.mode.ToLower().Equals("delete"))
                    {
                        obj = srv.DeleteBodySetService(authHeader, lang, fromProject.ToLower(), logID, saveBodySetDTO,  data.userID, shareCode);
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
                else if (masterDataDTO.mode.ToLower().Equals("delete"))
                {
                    if (masterDataDTO.masterID == 0)
                    {
                        checkMissingOptional += "masterID ";
                    }
                }
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

                obj = srv.SearchMasterDepartmentService(authHeader, lang, fromProject.ToLower(), logID, searchMasterDataDTO, "system_department",  shareCode);

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

                obj = srv.SearchMasterKeyService(authHeader, lang, fromProject.ToLower(), logID, searchMasterDataDTO, "system_master_key",  shareCode);

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
                    //if (string.IsNullOrEmpty(systemMasterDTO.nameEN))
                    //{
                    //    checkMissingOptional += "nameEN ";
                    //}
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
                    //if (string.IsNullOrEmpty(systemMasterDTO.nameEN))
                    //{
                    //    checkMissingOptional += "nameEN ";
                    //}
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

                if (systemMasterDTO.keyID != 0)
                {
                    obj = srv.GetSystemMasterService(authHeader, lang, fromProject.ToLower(), logID, systemMasterDTO.keyID, shareCode);
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

                obj = srv.SearchSystemMasterService(authHeader, lang, fromProject.ToLower(), logID, searchSystemMasterDataDTO, "system_master",  shareCode);

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

                string strDepartmentSearch = JsonConvert.SerializeObject(searchLeaveDTO.departmentSearch);
                strDepartmentSearch = string.Join(",", searchLeaveDTO.departmentSearch);
                searchLeaveDTO.prepairDepartmentSearch = strDepartmentSearch;

                string strPositionSearch = JsonConvert.SerializeObject(searchLeaveDTO.positionSearch);
                strPositionSearch = string.Join(",", searchLeaveDTO.positionSearch);
                searchLeaveDTO.prepairPositionSearch = strPositionSearch;

                string strEmpTypeSearch = JsonConvert.SerializeObject(searchLeaveDTO.empTypeSearch);
                strEmpTypeSearch = string.Join(",", searchLeaveDTO.empTypeSearch);
                searchLeaveDTO.prepairEmpTypeSearch = strEmpTypeSearch;

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

                int allLeaveDays = _sql.GetTotalDayPerYearByLeaveType(saveLeaveDetailDTO.leavetypeId);
                int useDay = _sql.GetTotalUseDayPerYear(saveLeaveDetailDTO.leaveId, shareCode);

                int remainDay = allLeaveDays - useDay;

                InsertService srvInsert = new InsertService();
                UpdateService srvUpdate = new UpdateService();
                //DeleteService srvDelete = new DeleteService();
                var obj = new object();

                if(remainDay - saveLeaveDetailDTO.numdays >= 0)
                {
                    if (chkRequestBody.Success == true)
                    {
                        if (saveLeaveDetailDTO.leaveId.Equals(0) && saveLeaveDetailDTO.mode.ToLower() == "insert")
                        {
                            obj = srvInsert.InsertLeaveDetailService(authHeader, lang, fromProject.ToLower(), logID, saveLeaveDetailDTO, remainDay, data.userID, shareCode);
                        }
                        else if (saveLeaveDetailDTO.leaveId > 0 && saveLeaveDetailDTO.mode.ToLower() == "update")
                        {
                            obj = srvUpdate.UpdateLeaveDetailService(authHeader, lang, fromProject.ToLower(), logID, saveLeaveDetailDTO, data.userID, shareCode);
                        }
                        //else if (saveLeaveDetailDTO.leaveId > 0 && saveLeaveDetailDTO.mode.ToLower() == "delete")
                        //{
                        //obj = srvDelete.DeleteEmpWorkShiftService(authHeader, lang, fromProject.ToLower(), logID, saveLeaveDetailDTO,  data.userID);
                        //}
                    }
                }
                else
                {
                    throw new Exception("You choose an overdue leave date.");
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
        public IHttpActionResult RejectLeaveForm(ApproveLeaveRequestDTO approveLeaveRequestDTO)
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
                var obj = new object();

                string json = JsonConvert.SerializeObject(approveLeaveRequestDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "RejectLeaveForm", json, timestampNow.ToString(), headersDTO,
                   data.userID, fromProject.ToLower());

                #region msgCheck
                 string checkMissingOptional = "";

                string strRejectListLeaveID = JsonConvert.SerializeObject(approveLeaveRequestDTO.rejectListLeaveID);
                strRejectListLeaveID = string.Join(",", approveLeaveRequestDTO.rejectListLeaveID);
                approveLeaveRequestDTO.prepairRejectListLeaveID = strRejectListLeaveID;

                if (string.IsNullOrEmpty(approveLeaveRequestDTO.prepairRejectListLeaveID))
                {
                    checkMissingOptional += "rejectListLeaveID[] ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {
                    UpdateService srv = new UpdateService();
                    obj = srv.RejectLeaveFormService(authHeader, lang, fromProject.ToLower(), logID, approveLeaveRequestDTO, data.userID, shareCode);
                }
                #endregion

                //int allLeaveDays = _sql.GetTotalDayPerYear(actionLeaveFormDTO.l);
                //int useDay = _sql.GetTotalUseDayPerYear(actionLeaveFormDTO.leaveID, shareCode);

                //int remainDay = allLeaveDays - useDay;


               // UpdateService srv = new UpdateService();
                //var obj = srv.RejectLeaveFormService(authHeader, lang, fromProject.ToLower(), logID, actionLeaveFormDTO, data.userID, shareCode);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/approve/leaveForm")]
        [HttpPost]
        public IHttpActionResult ApproveLeaveForm(ApproveLeaveRequestDTO approveLeaveRequestDTO)
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
                var obj = new object();
                string json = JsonConvert.SerializeObject(approveLeaveRequestDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "ApproveLeaveForm", json, timestampNow.ToString(), headersDTO,
                   data.userID, fromProject.ToLower());

                #region msgCheck
                string checkMissingOptional = "";

                string strApprovrListLeaveID = JsonConvert.SerializeObject(approveLeaveRequestDTO.approveListLeaveID);
                strApprovrListLeaveID = string.Join(",", approveLeaveRequestDTO.approveListLeaveID);
                approveLeaveRequestDTO.prepairApproveListLeaveID = strApprovrListLeaveID;

                if (string.IsNullOrEmpty(approveLeaveRequestDTO.prepairApproveListLeaveID))
                {
                    checkMissingOptional += "approveListLeaveID[] ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {
                    UpdateService srv = new UpdateService();
                    obj = srv.ApproveLeaveFormService(authHeader, lang, fromProject.ToLower(), logID, approveLeaveRequestDTO, data.userID, shareCode);
                }
                #endregion


                //int allLeaveDays = _sql.GetTotalDayPerYear(actionLeaveFormDTO.leaveID);
                //int useDay = _sql.GetTotalUseDayPerYear(actionLeaveFormDTO.leaveID, shareCode);

                //int remainDay = allLeaveDays - useDay;



                //UpdateService srv = new UpdateService();
                //var obj = srv.ApproveLeaveFormService(authHeader, lang, fromProject.ToLower(), logID, actionLeaveFormDTO, remainDay, data.userID, shareCode);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        //[Route("1.0/Approve/leaveForm")]
        //[HttpPost]
        //public IHttpActionResult ApproveLeave(ApproveLeaveRequestDTO approveLeaveRequestDTO)
        //{
        //    var request = HttpContext.Current.Request;
        //    string authHeader = (request.Headers["Authorization"] == null ? "" : request.Headers["Authorization"]);
        //    string lang = (request.Headers["lang"] == null ? WebConfigurationManager.AppSettings["default_language"] : request.Headers["lang"]);
        //    string fromProject = request.Headers["Fromproject"];
        //    string shareCode = (request.Headers["Sharecode"] == null ? "" : request.Headers["Sharecode"]);

        //    AuthenticationController _auth = AuthenticationController.Instance;
        //    AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);

        //    try
        //    {
        //        var obj = new object();
        //        string json = JsonConvert.SerializeObject(approveLeaveRequestDTO);
        //        int logID = _sql.InsertLogReceiveData("ApproveLeave", json, timestampNow.ToString(), authHeader,
        //            data.userID, fromProject.ToLower());

        //        string checkMissingOptional = "";

        //        string strApproveListLeaveID = JsonConvert.SerializeObject(approveLeaveRequestDTO.approveListLeaveID);
        //        strApproveListLeaveID = string.Join(",", approveLeaveRequestDTO.approveListLeaveID);
        //        approveLeaveRequestDTO.prepairApproveListLeaveID = strApproveListLeaveID;

        //        string strRejectListLeaveID = JsonConvert.SerializeObject(approveLeaveRequestDTO.rejectListLeaveID);
        //        strRejectListLeaveID = string.Join(",", approveLeaveRequestDTO.rejectListLeaveID);
        //        approveLeaveRequestDTO.prepairRejectListLeaveID = strRejectListLeaveID;

        //        if (string.IsNullOrEmpty(approveLeaveRequestDTO.prepairApproveListLeaveID))
        //        {
        //            checkMissingOptional += "approveListLeaveID[] ";
        //        }

        //        if (string.IsNullOrEmpty(approveLeaveRequestDTO.prepairRejectListLeaveID))
        //        {
        //            checkMissingOptional += "rejectListLeaveID[] ";
        //        }

        //        if (checkMissingOptional != "")
        //        {
        //            throw new Exception("Missing Parameter : " + checkMissingOptional);
        //        }
        //        else
        //        {
        //            InsertService srv = new InsertService();
        //            obj = srv.ApproveEmpWorkShiftTimeTransChangeService(shareCode, authHeader, lang, fromProject.ToLower(), logID, approveChangeWorkShiftTimeRequestDTO, data.userID);
        //        }

        //        return Ok(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
        //    }
        //}


        #endregion

        #region pleng work time

        [Route("1.0/search/worktime")]
        [HttpPost]
        public IHttpActionResult SearchWorkTime(SearchWorkTimeDTO searchWorkTimeDTO)
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
                string json = JsonConvert.SerializeObject(searchWorkTimeDTO);
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SearchWorkTime", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                GetService srv = new GetService();
                var obj = new object();

                string strDepartmentSearch = JsonConvert.SerializeObject(searchWorkTimeDTO.departmentSearch);
                strDepartmentSearch = string.Join(",", searchWorkTimeDTO.departmentSearch);
                searchWorkTimeDTO.prepairDepartmentSearch = strDepartmentSearch;

                string strPositionSearch = JsonConvert.SerializeObject(searchWorkTimeDTO.positionSearch);
                strPositionSearch = string.Join(",", searchWorkTimeDTO.positionSearch);
                searchWorkTimeDTO.prepairPositionSearch = strPositionSearch;

                if (searchWorkTimeDTO.pageInt.Equals(null) || searchWorkTimeDTO.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (searchWorkTimeDTO.perPage.Equals(null) || searchWorkTimeDTO.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }
                if (searchWorkTimeDTO.sortField > 2)
                {
                    throw new Exception("invalid : sortField " + searchWorkTimeDTO.sortField);
                }
                if (!(searchWorkTimeDTO.sortType == "a" || searchWorkTimeDTO.sortType == "d" || searchWorkTimeDTO.sortType == "A" || searchWorkTimeDTO.sortType == "D" || searchWorkTimeDTO.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

               obj = srv.SearchWorkTimeService(authHeader, lang, fromProject.ToLower(), logID, searchWorkTimeDTO, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }


        #endregion

        [Route("1.0/search/allreportsalary")]
        [HttpPost]
        public IHttpActionResult GetSearchAllReportSalary(SearchReportDTO searchReportSalaryDTO)
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
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SearchAllReportSalary", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                GetService srv = new GetService();
                var obj = new object();

                string strDepartmentSearch = JsonConvert.SerializeObject(searchReportSalaryDTO.departmentSearch);
                strDepartmentSearch = string.Join(",", searchReportSalaryDTO.departmentSearch);
                searchReportSalaryDTO.prepairDepartmentSearch = strDepartmentSearch;

                string strPositionSearch = JsonConvert.SerializeObject(searchReportSalaryDTO.positionSearch);
                strPositionSearch = string.Join(",", searchReportSalaryDTO.positionSearch);
                searchReportSalaryDTO.prepairPositionSearch = strPositionSearch;

                string strEmpTypeSearch = JsonConvert.SerializeObject(searchReportSalaryDTO.empTypeSearch);
                strEmpTypeSearch = string.Join(",", searchReportSalaryDTO.empTypeSearch);
                searchReportSalaryDTO.prepairEmpTypeSearch = strEmpTypeSearch;

                string strEmpStatusSearch = JsonConvert.SerializeObject(searchReportSalaryDTO.empStatusSearch);
                strEmpStatusSearch = string.Join(",", searchReportSalaryDTO.empStatusSearch);
                searchReportSalaryDTO.prepairEmpStatusSearch = strEmpStatusSearch;

                if (searchReportSalaryDTO.pageInt.Equals(null) || searchReportSalaryDTO.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (searchReportSalaryDTO.perPage.Equals(null) || searchReportSalaryDTO.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }

                if (searchReportSalaryDTO.sortField > 3)
                {
                    throw new Exception("invalid : sortField " + searchReportSalaryDTO.sortField);
                }
                if (!(searchReportSalaryDTO.sortType == "a" || searchReportSalaryDTO.sortType == "d" || searchReportSalaryDTO.sortType == "A" || searchReportSalaryDTO.sortType == "D" || searchReportSalaryDTO.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchAllSalaryReportService(authHeader, lang, fromProject.ToLower(), logID, searchReportSalaryDTO, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [Route("1.0/search/allreportemployee")]
        [HttpPost]
        public IHttpActionResult GetSearchAllReportEmployee(SearchReportDTO searchReportEmployeeDTO)
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
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SearchAllReportEmployee", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                GetService srv = new GetService();
                var obj = new object();

                string strDepartmentSearch = JsonConvert.SerializeObject(searchReportEmployeeDTO.departmentSearch);
                strDepartmentSearch = string.Join(",", searchReportEmployeeDTO.departmentSearch);
                searchReportEmployeeDTO.prepairDepartmentSearch = strDepartmentSearch;

                string strPositionSearch = JsonConvert.SerializeObject(searchReportEmployeeDTO.positionSearch);
                strPositionSearch = string.Join(",", searchReportEmployeeDTO.positionSearch);
                searchReportEmployeeDTO.prepairPositionSearch = strPositionSearch;

                string strEmpTypeSearch = JsonConvert.SerializeObject(searchReportEmployeeDTO.empTypeSearch);
                strEmpTypeSearch = string.Join(",", searchReportEmployeeDTO.empTypeSearch);
                searchReportEmployeeDTO.prepairEmpTypeSearch = strEmpTypeSearch;

                string strEmpStatusSearch = JsonConvert.SerializeObject(searchReportEmployeeDTO.empStatusSearch);
                strEmpStatusSearch = string.Join(",", searchReportEmployeeDTO.empStatusSearch);
                searchReportEmployeeDTO.prepairEmpStatusSearch = strEmpStatusSearch;

                if (searchReportEmployeeDTO.pageInt.Equals(null) || searchReportEmployeeDTO.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (searchReportEmployeeDTO.perPage.Equals(null) || searchReportEmployeeDTO.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }

                if (searchReportEmployeeDTO.sortField > 3)
                {
                    throw new Exception("invalid : sortField " + searchReportEmployeeDTO.sortField);
                }
                if (!(searchReportEmployeeDTO.sortType == "a" || searchReportEmployeeDTO.sortType == "d" || searchReportEmployeeDTO.sortType == "A" || searchReportEmployeeDTO.sortType == "D" || searchReportEmployeeDTO.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchAllEmployeeReportService(authHeader, lang, fromProject.ToLower(), logID, searchReportEmployeeDTO, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }


        [Route("1.0/search/allreportworktime")]
        [HttpPost]
        public IHttpActionResult GetSearchAllReportWorkTime(SearchReportDTO searchReportWorkTimeDTO)
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
                int logID = _sql.InsertLogReceiveDataWithShareCode(shareCode, "SearchAllReportWorkTime", json, timestampNow.ToString(), headersDTO,
                    data.userID, fromProject.ToLower());

                GetService srv = new GetService();
                var obj = new object();

                string strDepartmentSearch = JsonConvert.SerializeObject(searchReportWorkTimeDTO.departmentSearch);
                strDepartmentSearch = string.Join(",", searchReportWorkTimeDTO.departmentSearch);
                searchReportWorkTimeDTO.prepairDepartmentSearch = strDepartmentSearch;

                string strPositionSearch = JsonConvert.SerializeObject(searchReportWorkTimeDTO.positionSearch);
                strPositionSearch = string.Join(",", searchReportWorkTimeDTO.positionSearch);
                searchReportWorkTimeDTO.prepairPositionSearch = strPositionSearch;

                string strEmpTypeSearch = JsonConvert.SerializeObject(searchReportWorkTimeDTO.empTypeSearch);
                strEmpTypeSearch = string.Join(",", searchReportWorkTimeDTO.empTypeSearch);
                searchReportWorkTimeDTO.prepairEmpTypeSearch = strEmpTypeSearch;

                if (searchReportWorkTimeDTO.pageInt.Equals(null) || searchReportWorkTimeDTO.pageInt.Equals(0))
                {
                    throw new Exception("invalid : pageInt ");
                }
                if (searchReportWorkTimeDTO.perPage.Equals(null) || searchReportWorkTimeDTO.perPage.Equals(0))
                {
                    throw new Exception("invalid : perPage ");
                }

                if (searchReportWorkTimeDTO.sortField > 3)
                {
                    throw new Exception("invalid : sortField " + searchReportWorkTimeDTO.sortField);
                }
                if (!(searchReportWorkTimeDTO.sortType == "a" || searchReportWorkTimeDTO.sortType == "d" || searchReportWorkTimeDTO.sortType == "A" || searchReportWorkTimeDTO.sortType == "D" || searchReportWorkTimeDTO.sortType == ""))
                {
                    throw new Exception("invalid sortType");
                }

                obj = srv.SearchAllWorkTimeReportService(authHeader, lang, fromProject.ToLower(), logID, searchReportWorkTimeDTO, shareCode);

                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }
        }


        #region Report


        #endregion

    }
}

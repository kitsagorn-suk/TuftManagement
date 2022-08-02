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
                else
                {
                    obj = srv.GetAllDropdownService(authHeader, lang, fromProject.ToLower(), logID, getDropdownRequestDTO);
                }

                //if (chkRequestBody.Success == true)
                //{
                //    if (getDropdownRequestDTO.moduleName.ToLower() == "subDistrict".ToLower())
                //    {
                //        obj = srv.GetSubDistrictDropdownService(authHeader, lang, fromProject.ToLower(), logID, getDropdownRequestDTO);
                //    }
                //    if (getDropdownRequestDTO.moduleName.ToLower() == "titlename".ToLower())
                //    {
                //        obj = srv.GetTitleNameDropdownService(authHeader, lang, fromProject.ToLower(), logID, getDropdownRequestDTO);
                //    }
                //    else
                //    {
                //        obj = srv.GetAllDropdownService(authHeader, lang, fromProject.ToLower(), logID, getDropdownRequestDTO);
                //    }
                //}
                
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


                if (keyName == "upload_image_profile" || keyName == "upload_image_gallery")
                {
                    AuthenticationController _auth = AuthenticationController.Instance;
                    AuthorizationModel data = _auth.ValidateHeader(authHeader, lang, fromProject, shareCode);
                    userID = data.userID;
                }
                if (keyName == "upload_image_profile")
                {
                    subFolder = ActionUserID + "\\ProFilePath";
                    diskFolderPath = string.Format(WebConfigurationManager.AppSettings["file_user_path"], subFolder);
                    //newFileName = _sql.GenImageProfile(userID);
                    fileURL = string.Format(WebConfigurationManager.AppSettings["file_user_url"], ActionUserID + "/ProFilePath", newFileName);
                }
                if (keyName == "upload_image_gallery")
                {
                    subFolder = ActionUserID + "\\GalleryPath";
                    diskFolderPath = string.Format(WebConfigurationManager.AppSettings["file_user_path"], subFolder);
                    //newFileName = _sql.GenImageProfile(userID);
                    fileURL = string.Format(WebConfigurationManager.AppSettings["file_user_url"], ActionUserID + "/GalleryPath", newFileName);
                }

                var fullPath = Path.Combine(diskFolderPath, newFileName);
                var fileInfo = new FileInfo(fullPath);
                while (fileInfo.Exists)
                {
                    if (keyName == "upload_user_profile" || keyName == "upload_image_gallery")
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

                            _ReturnIdModel result = _sql.InsertUploadFileDetails(shareCode, keyName, fileCode, "", fileName, fileURL, ActionUserID);
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

                            _sql.InsertUploadFileDetails(shareCode, keyName, fileCode, "", fileName, fileURL, ActionUserID);
                            value.success = validation.Success;

                            _fileDetails file = new _fileDetails();
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
                saveEmpProfileDTO.shareCode = shareCode;
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
                obj = srv.SaveMasterService(authHeader, lang, fromProject.ToLower(), logID, masterDataDTO, "master_position", data.userID, shareCode);

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
                    obj = srv.GetMasterService(authHeader, lang, fromProject.ToLower(), logID, masterDataDTO.masterID, "master_position", shareCode);
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

                obj = srv.SearchMasterService(authHeader, lang, fromProject.ToLower(), logID, searchMasterDataDTO, "master_position", data.roleIDList, shareCode);

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

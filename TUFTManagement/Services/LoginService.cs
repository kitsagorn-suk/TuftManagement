using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using TUFTManagement.Core;
using TUFTManagement.DTO;
using TUFTManagement.Models;

namespace TUFTManagement.Services
{
    public class LoginService
    {
        private SQLManager _sql = SQLManager.Instance;

        public LoginModel Login(string authorization, string lang, string username, string password, string platform, int logID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            LoginModel value = new LoginModel();
            try
            {
                string businesscode = "";
                //user หา business_code มาว่าอยู่ร้านไหนบ้าง
                value.data = new LoginData();
                ValidationModel validation = ValidationManager.CheckValidationLogin(username, password.Trim(), lang, value.data.id, businesscode);

                if (validation.Success == true)
                {
                    string auth = GenAuthorization.GetAuthorization(username, password, "InventoryComplex", platform.ToLower());
                    value.data = _sql.Login(username, password, auth, lang);
                    value.data.token = auth;
                    value.data.platform = platform;
                    if (value.data.roleID != 0)
                    {
                        value.data.access_list = new List<AccessRole>();
                        value.data.access_list = _sql.GetAllAccessRole(value.data.roleID);

                        value.data.menuList = new List<MenuList>();
                        value.data.menuList = _sql.GetAllMenuMain(value.data.roleID, lang);
                    }
                }
                else
                {
                    value.data = null;
                }

                value.success = validation.Success;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "Login:" + username);
                if (logID > 0)
                {
                    _sql.UpdateLogReceiveDataError(logID, ex.ToString());
                }
                throw ex;
            }
            finally
            {
                _sql.UpdateStatusLog(logID, 1);
            }
            return value;
        }

        public ReturnIdModel Logout(string authorization, string lang, int userID, string platform, int logID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    value.data = _sql.Logout(userID);
                }
                else
                {
                    value.data = null;
                }

                value.success = validation.Success;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "Logout:" + userID);
                if (logID > 0)
                {
                    _sql.UpdateLogReceiveDataError(logID, ex.ToString());
                }
                throw ex;
            }
            finally
            {
                _sql.UpdateStatusLog(logID, 1);
            }
            return value;
        }

        public ReturnVerifyModel VerifyToken(string authorization, string lang, string platform, int logID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnVerifyModel value = new ReturnVerifyModel();
            try
            {
                value.data = new _ReturnVerifyModel();
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    value.data = _sql.VerifyToken(authorization);
                }
                else
                {
                    value.data = null;
                }

                value.success = validation.Success;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "VerifyToken:" + authorization);
                if (logID > 0)
                {
                    _sql.UpdateLogReceiveDataError(logID, ex.ToString());
                }
                throw ex;
            }
            finally
            {
                _sql.UpdateStatusLog(logID, 1);
            }
            return value;
        }
    }
}
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
                CheckUserByTokenModel dataFormToken = new CheckUserByTokenModel();
                dataFormToken = _sql.CheckUserID(username, password);

                value.data = new LoginData();
                ValidationModel validation = ValidationManager.CheckValidationLogin(username, password.Trim(), lang);

                if (validation.Success == true)
                {
                    string auth = GenAuthorization.GetAuthorization(username, password, "InventoryComplex", platform.ToLower(), dataFormToken);
                    value.data = _sql.Login(username, password, auth, lang);
                    value.data.token = auth;

                    value.data.role = new List<RoleIDList>();
                    value.data.role = _sql.GetUserRole(value.data.id, lang);

                    value.data.shareHolder = new List<ShareHolderList>();
                    value.data.shareHolder = _sql.GetUserShareHolder(value.data.id, lang);
                    
                    value.data.accessList = new List<AccessRole>();
                    //value.data.accessList = _sql.GetAllAccessRole(int.Parse(roleID));
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
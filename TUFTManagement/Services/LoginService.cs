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
                value.data = new LoginData();
                ValidationModel validation = ValidationManager.CheckValidationLogin(username, password.Trim(), lang, value.data.id);

                if (validation.Success == true)
                {
                    string auth = GenAuthorization.GetAuthorization(username, password, "TuftManagement", platform.ToLower());
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
    }
}
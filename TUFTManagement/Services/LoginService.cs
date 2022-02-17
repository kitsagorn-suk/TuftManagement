using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using TUFTManagement.Core;
using TUFTManagement.Models;

namespace TUFTManagement.Services
{
    public class LoginService
    {
        private SQLManager _sql = SQLManager.Instance;

        public LoginModel Login(int companyID, string authorization, string lang, string username, string password, string platform, int logID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            LoginModel value = new LoginModel();
            try
            {
                value.data = new LoginData();
                lang = (lang == null) ? WebConfigurationManager.AppSettings["default_language"] : lang.ToUpper();
                ValidationModel validation = ValidationManager.CheckValidationLogin(username, password.Trim(), lang, companyID, value.data.id);

                if (validation.Success == true)
                {
                    string auth = GenAuthorization.GetAuthorization(username, password, "ResourceManagement", platform.ToLower());
                    value.data = _sql.Login(companyID, username, password, auth, "", "", "", lang);
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
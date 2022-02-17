using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TUFTManagement.DTO;
using TUFTManagement.Models;

namespace TUFTManagement.Core
{
    public class ValidationManager
    {
        private static SQLManager _sql = SQLManager.Instance;

        public static ValidationModel CheckValidationLogin(string username, string password, string lang, int company_id, int dataID)
        {
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state;



                #region E301002
                state = ValidationModel.InvalidState.E301002; //คุณยังไม่ได้เปลี่ยนรหัสผ่านครั้งแรก
                bool is_first = _sql.CheckFirstSettingLogin(username, password);
                if (is_first == true)
                {
                    getMessage = ValidationModel.GetInvalidMessage(state, lang);
                    return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                }
                #endregion


                #region E301008
                state = ValidationModel.InvalidState.E301008; //ไม่พบชื่อผู้ใช้ในบริษัทนี้
                int check_company = _sql.CheckCompanyLogin(username, company_id);
                if (check_company == 0)
                {
                    getMessage = ValidationModel.GetInvalidMessage(state, lang);
                    return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                }
                #endregion

                #region E301001
                state = ValidationModel.InvalidState.E301001; //ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง
                int user_id = _sql.CheckUserPassword(username, password);
                if (user_id == 0)
                {
                    //update count fail
                    int checkCount = _sql.UpdateCountFailPassword(username);
                    bool is_lock = _sql.CheckLockPassword(username);

                    if (checkCount >= 3)
                    {
                        is_lock = _sql.UpdateIsLockPassword(username);
                    }
                    if (is_lock == true)
                    {
                        state = ValidationModel.InvalidState.E301003; //บัญชีของคุณถูกระงับ โปรดติดต่อฝ่ายบุคคล
                        getMessage = ValidationModel.GetInvalidMessage(state, lang);
                        return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                    }
                    else
                    {
                        getMessage = ValidationModel.GetInvalidMessage(state, lang);
                        return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };

                    }
                }
                #endregion

                getMessage = ValidationModel.GetInvalidMessage(ValidationModel.InvalidState.S201001, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(ValidationModel.InvalidState.S201001);
                value.InvalidMessage = getMessage.message;
                value.InvalidText = getMessage.topic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }
    }
}
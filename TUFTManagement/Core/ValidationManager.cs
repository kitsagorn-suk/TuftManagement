using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TUFTManagement.DTO;
using TUFTManagement.Models;

namespace TUFTManagement.Core
{
    public class ValidationManager
    {
        private static SQLManager _sql = SQLManager.Instance;

        public static ValidationModel CheckValidation(int chkID, string lang, string platform)
        {
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state = ValidationModel.InvalidState.S201001;

                getMessage = ValidationModel.GetInvalidMessage(state, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(state);
                value.InvalidMessage = getMessage.message;
                value.InvalidText = getMessage.topic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }
        public static ValidationModel CheckValidationWithShareCode(string shareCode, int chkID, string lang, string platform)
        {
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state = ValidationModel.InvalidState.S201001;

                getMessage = ValidationModel.GetInvalidMessage(state, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(state);
                value.InvalidMessage = getMessage.message;
                value.InvalidText = getMessage.topic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }

        public static ValidationModel CheckValidationLogin(string username, string password, string lang)
        {
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state = ValidationModel.InvalidState.S201001;
                
                #region E301001
                
                int user_id = _sql.CheckUserPassword(username, password);
                if (user_id == 0)
                {
                    state = ValidationModel.InvalidState.E301001; //ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง
                    getMessage = ValidationModel.GetInvalidMessage(state, lang);
                    return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                }
                #endregion

                getMessage = ValidationModel.GetInvalidMessage(state, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(state);
                value.InvalidMessage = getMessage.message;
                value.InvalidText = getMessage.topic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }
        public static ValidationModel CheckValidationWorktime(int chkID, string lang, string platform, int workTimeID, string shareCode)
        {
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state = ValidationModel.InvalidState.S201001;

                #region E301007
                
                GetEmpWorkTime dataCheck = _sql.GetEmpWorkTimeNewVer(workTimeID,shareCode);
                if (dataCheck.isFix == 1)
                {
                    state = ValidationModel.InvalidState.E301007; //check is fix
                    getMessage = ValidationModel.GetInvalidMessage(state, lang);
                    return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                }
                #endregion
               

                #region E302001
                
                if (chkID == 0)
                {
                    state = ValidationModel.InvalidState.E302001; //Data not found
                    getMessage = ValidationModel.GetInvalidMessage(state, lang);
                    return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                }
                #endregion

                getMessage = ValidationModel.GetInvalidMessage(state, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(state);
                value.InvalidMessage = getMessage.message;
                value.InvalidText = getMessage.topic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }
        public static ValidationModel CheckValidationTransChange(string lang, string platform, int transChangeID)
        {
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state = ValidationModel.InvalidState.S201001;

                #region E302001                
                int  dataCheck = _sql.CheckTransChange(transChangeID);
                if (dataCheck < 0)
                {
                    state = ValidationModel.InvalidState.E302001; //Data not found
                    getMessage = ValidationModel.GetInvalidMessage(state, lang);
                    return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                }
                #endregion

                #region E300001
                
                if (platform != "web" || platform == null || platform == "")
                {
                    state = ValidationModel.InvalidState.E300001; //Error Platform
                    getMessage = ValidationModel.GetInvalidMessage(state, lang);
                    return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                }
                #endregion

                getMessage = ValidationModel.GetInvalidMessage(state, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(state);
                value.InvalidMessage = getMessage.message;
                value.InvalidText = getMessage.topic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }

        public static ValidationModel CheckRoleValidation(string lang, List<string> listobjectID, int roleID)
        {
            //เช็ค IDเรื่องนั้นๆว่ามีไหม และเช็คว่าถ้าคนแก้ชื่อไม่ตรง จะไปเช็คสิทธิว่ามีสิทธิแก้ไหม
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state = ValidationModel.InvalidState.S201001;

                string result = "";
                bool haveAuthorization = false;
                foreach (var pObjectID in listobjectID)
                {
                    DataTable dtIsActive = _sql.CheckValidationRoleID(pObjectID, roleID);
                    if (dtIsActive.Rows.Count > 0)
                    {
                        if (dtIsActive.Rows[0]["is_active"].ToString().Equals("1"))
                        {
                            result += "1";
                        }
                        else
                        {
                            result += "0";
                        }
                    }
                    else
                    {
                        result += "0";
                    }
                }

                if (!string.IsNullOrEmpty(result))
                {
                    if (!result.Contains("0"))
                    {
                        haveAuthorization = true;
                    }
                }

                if (!haveAuthorization)
                {
                    state = ValidationModel.InvalidState.E301007; //คุณไม่มีสิทธิ์แก้ไข
                    getMessage = ValidationModel.GetInvalidMessage(state, lang);
                    return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                }

                getMessage = ValidationModel.GetInvalidMessage(state, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(state);
                value.InvalidMessage = getMessage.message;
                value.InvalidText = getMessage.topic;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }

        public static ValidationModel CheckValidationUpdate(int ID, string Type, string lang, List<string> listobjectID, int roleID)
        {
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state = ValidationModel.InvalidState.S201001;

                DataTable dt = _sql.CheckValidationUpdateByID(ID, Type);

                string result = "";
                bool haveAuthorization = false;
                foreach (var pObjectID in listobjectID)
                {
                    DataTable dtIsActive = _sql.CheckValidationRoleID(pObjectID, roleID);
                    if (dtIsActive.Rows.Count > 0)
                    {
                        if (dtIsActive.Rows[0]["is_active"].ToString().Equals("1"))
                        {
                            result += "1";
                        }
                        else
                        {
                            result += "0";
                        }
                    }
                    else
                    {
                        result += "0";
                    }
                }

                if (!string.IsNullOrEmpty(result))
                {
                    if (!result.Contains("0"))
                    {
                        haveAuthorization = true;
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    if (!haveAuthorization)
                    {
                        state = ValidationModel.InvalidState.E301007; //คุณไม่มีสิทธิ์แก้ไข
                        getMessage = ValidationModel.GetInvalidMessage(state, lang);
                        return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                    }
                }
                else
                {
                    state = ValidationModel.InvalidState.E302001; //Data not found
                    getMessage = ValidationModel.GetInvalidMessage(state, lang);
                    return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                }

                getMessage = ValidationModel.GetInvalidMessage(state, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(state);
                value.InvalidMessage = getMessage.message;
                value.InvalidText = getMessage.topic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }

        public static ValidationModel CheckValidationDupicateInsertEmp(string shareCode, string lang, SaveEmpProfileDTO saveEmpProfileDTO)
        {
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state = ValidationModel.InvalidState.S201001;

                int userIdTarget = _sql.getUserIdByEmpProfileID(shareCode, saveEmpProfileDTO.empProfileID);

                DataTable dt = _sql.CheckDupicateInsertEmp(shareCode, saveEmpProfileDTO, userIdTarget);
                DataTable dt2 = _sql.CheckDupicateUsername(saveEmpProfileDTO, userIdTarget);

                if (dt2.Rows.Count > 0)
                {
                    if (dt2.Rows[0]["UserName"].ToString() != "0")
                    {
                        state = ValidationModel.InvalidState.E301004;
                        getMessage = ValidationModel.GetInvalidMessage(state, lang);
                        return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                    }
                }
                
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["Name"].ToString() != "0")
                    {
                        state = ValidationModel.InvalidState.E301002;
                        getMessage = ValidationModel.GetInvalidMessageWithShareCode(shareCode, state, lang);
                        return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                    }
                    if (dt.Rows[0]["EmpCode"].ToString() != "0")
                    {
                        state = ValidationModel.InvalidState.E301003;
                        getMessage = ValidationModel.GetInvalidMessageWithShareCode(shareCode, state, lang);
                        return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                    }
                    if (dt.Rows[0]["IdentityCard"].ToString() != "0")
                    {
                        state = ValidationModel.InvalidState.E301005;
                        getMessage = ValidationModel.GetInvalidMessageWithShareCode(shareCode, state, lang);
                        return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                    }
                }

                getMessage = ValidationModel.GetInvalidMessage(state, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(state);
                value.InvalidMessage = getMessage.message;
                value.InvalidText = getMessage.topic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }

        public static ValidationModel CheckValidationDupicateInsertEmpRate(string lang, SaveEmpRateRequestDTO saveEmpRateDTO)
        {
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state = ValidationModel.InvalidState.S201001;

                getMessage = ValidationModel.GetInvalidMessage(state, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(state);
                value.InvalidMessage = getMessage.message;
                value.InvalidText = getMessage.topic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }

        public static ValidationModel CheckValidationDupicateInsertBodySet(string shareCode, string lang, SaveBodySetRequestDTO saveBodySetDTO)
        {
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state = ValidationModel.InvalidState.S201001;

                getMessage = ValidationModel.GetInvalidMessage(state, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(state);
                value.InvalidMessage = getMessage.message;
                value.InvalidText = getMessage.topic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }

        public static ValidationModel CheckValidationDupicateInsertFeedback(string lang, FeedbackDTO feedbackDTO)
        {
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state = ValidationModel.InvalidState.S201001;

                getMessage = ValidationModel.GetInvalidMessage(state, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(state);
                value.InvalidMessage = getMessage.message;
                value.InvalidText = getMessage.topic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }

        public static ValidationModel CheckValidationDupicateInsertEmpWorkShift(string lang, SaveEmpWorkShiftRequestDTO saveEmpWorkShiftRequestDTO)
        {
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state = ValidationModel.InvalidState.S201001;

                getMessage = ValidationModel.GetInvalidMessage(state, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(state);
                value.InvalidMessage = getMessage.message;
                value.InvalidText = getMessage.topic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }

        public static ValidationModel CheckValidationDupicateInsertLeaveDetail(string lang, SaveLeaveDetailDTO saveLeaveDetailDTO)
        {
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state = ValidationModel.InvalidState.S201001;

                getMessage = ValidationModel.GetInvalidMessage(state, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(state);
                value.InvalidMessage = getMessage.message;
                value.InvalidText = getMessage.topic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }


        public static ValidationModel CheckValidationDupicateMasterData(string shareCode, string lang, string TableName, MasterDataDTO masterDataDTO)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state = ValidationModel.InvalidState.S201001;

                DataTable dt = _sql.CheckDuplicateMaster(shareCode, TableName, masterDataDTO);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["status_name_en"].ToString() != "0")
                    {
                        state = ValidationModel.InvalidState.E301008;
                        getMessage = ValidationModel.GetInvalidMessageWithShareCode(shareCode, state, lang);
                        return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                    }
                    if (dt.Rows[0]["status_name_th"].ToString() != "0")
                    {
                        state = ValidationModel.InvalidState.E301009;
                        getMessage = ValidationModel.GetInvalidMessageWithShareCode(shareCode, state, lang);
                        return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                    }
                }

                getMessage = ValidationModel.GetInvalidMessageWithShareCode(shareCode, 0, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(0);
                value.InvalidMessage = getMessage.message;
                value.InvalidText = getMessage.topic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }

        public static ValidationModel CheckValidationDupicateMasterKey(string shareCode, string lang, string TableName, MasterDataDTO masterDataDTO)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state = ValidationModel.InvalidState.S201001;

                DataTable dt = _sql.CheckDuplicateMasterKey(shareCode, TableName, masterDataDTO);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["counttotal"].ToString() != "0")
                    {
                        state = ValidationModel.InvalidState.E301010;
                        getMessage = ValidationModel.GetInvalidMessageWithShareCode(shareCode, state, lang);
                        return new ValidationModel { Success = false, InvalidCode = ValidationModel.GetInvalidCode(state), InvalidMessage = getMessage.message, InvalidText = getMessage.topic };
                    }
                }

                getMessage = ValidationModel.GetInvalidMessageWithShareCode(shareCode, 0, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(0);
                value.InvalidMessage = getMessage.message;
                value.InvalidText = getMessage.topic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }

        public static ValidationModel CheckValidationDupicateInsertSystemRole(string lang, SaveSystemRoleDTO saveSystemRoleDTO)
        {
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state = ValidationModel.InvalidState.S201001;

                getMessage = ValidationModel.GetInvalidMessage(state, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(state);
                value.InvalidMessage = getMessage.message;
                value.InvalidText = getMessage.topic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }

        public static ValidationModel CheckValidationObject(string lang, SaveSystemRoleDTO saveSystemRoleDTO)
        {
            ValidationModel value = new ValidationModel();
            try
            {
                GetMessageTopicDTO getMessage = new GetMessageTopicDTO();
                ValidationModel.InvalidState state = ValidationModel.InvalidState.S201001;

                getMessage = ValidationModel.GetInvalidMessage(state, lang);
                value.Success = true;
                value.InvalidCode = ValidationModel.GetInvalidCode(state);
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
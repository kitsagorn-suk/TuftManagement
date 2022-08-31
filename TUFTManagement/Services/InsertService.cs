using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TUFTManagement.Core;
using TUFTManagement.DTO;
using TUFTManagement.Models;
using static TUFTManagement.DTO.SaveEmpWorkTimeRequestDTO_V1_1;

namespace TUFTManagement.Services
{
    public class InsertService
    {
        private SQLManager _sql = SQLManager.Instance;

        #region Insert Employees

        public InsertLoginModel InsertEmpProfileService(string shareCode, string authorization, string lang, string platform, int logID,
            SaveEmpProfileDTO saveEmpProfileDTO, string roleIDList, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            InsertLoginModel value = new InsertLoginModel();
            try
            {
                value.data = new InsertLogin();
                ValidationModel validation = ValidationManager.CheckValidationDupicateInsertEmp(shareCode, lang, saveEmpProfileDTO); 
                if (validation.Success == true)
                {
                    //รอเรื่องสิทธิ์
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100301001");
                    //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);

                    saveEmpProfileDTO.userID = _sql.insertUserLogin(saveEmpProfileDTO, userID);
                    
                    if (saveEmpProfileDTO.userID != 0)
                    {

                        value.data = _sql.InsertEmpProfile(shareCode, saveEmpProfileDTO, userID);
                                    _sql.InsertEmpAddress(shareCode, saveEmpProfileDTO, userID);
                                    _sql.InsertEmpBankAccount(shareCode, saveEmpProfileDTO, userID);

                        SaveEmergencyContact emergencyContact = new SaveEmergencyContact();

                        if (saveEmpProfileDTO.emergencyContact.Count > 0)
                        {
                            foreach(SaveEmergencyContact item in saveEmpProfileDTO.emergencyContact)
                            {
                                _sql.InsertEmpEmergencyContact(shareCode, item, saveEmpProfileDTO.userID, userID);
                            }
                        }

                        if (saveEmpProfileDTO.positionID == 14) // ถ้าเป็นพริตตี้ เพิ่ม ข้อ 6/7/8
                        {
                            SaveEmpRateRequestDTO saveEmpRateRequestDTO = new SaveEmpRateRequestDTO();
                            saveEmpRateRequestDTO.empID = saveEmpProfileDTO.userID;
                            saveEmpRateRequestDTO.serviceNo = saveEmpProfileDTO.serviceNo;
                            saveEmpRateRequestDTO.startDrink = saveEmpProfileDTO.startDrink;
                            saveEmpRateRequestDTO.fullDrink = saveEmpProfileDTO.fullDrink;
                            saveEmpRateRequestDTO.rateStaff = saveEmpProfileDTO.rateStaff;
                            saveEmpRateRequestDTO.rateManager = saveEmpProfileDTO.rateManager;
                            saveEmpRateRequestDTO.rateOwner = saveEmpProfileDTO.rateOwner;
                            saveEmpRateRequestDTO.rateConfirm = saveEmpProfileDTO.rateConfirm;

                            _sql.InsertEmpRate(shareCode, saveEmpRateRequestDTO, userID);
                        }
                        
                    }
                        else
                    {
                        _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                    }
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertEmpProfileService:");
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

        public ReturnIdModel InsertEmpRateService(string authorization, string lang, string platform, int logID,
            SaveEmpRateRequestDTO saveEmpRateDTO, string roleIDList, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidationDupicateInsertEmpRate(lang, saveEmpRateDTO);
                if (validation.Success == true)
                {
                    //value.data = _sql.InsertEmpRate(saveEmpRateDTO, userID);
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertEmpRateService:");
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

        public ReturnIdModel InsertEmpWorkShiftService(string authorization, string lang, string platform, int logID,
            SaveEmpWorkShiftRequestDTO saveEmpWorkShiftRequestDTO, string roleIDList, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidationDupicateInsertEmpWorkShift(lang, saveEmpWorkShiftRequestDTO);
                if (validation.Success == true)
                {
                    value.data = _sql.InsertEmpWorkShift(saveEmpWorkShiftRequestDTO, userID);
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertEmpWorkShiftService:");
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

        public ReturnIdModel InsertLeaveDetailService(string authorization, string lang, string platform, int logID,
    SaveLeaveDetailDTO saveLeaveDetailDTO, string roleIDList, int userID,string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidationDupicateInsertLeaveDetail(lang, saveLeaveDetailDTO);
                if (validation.Success == true)
                {
                    value.data = _sql.InsertLeaveDetail(saveLeaveDetailDTO, userID, shareCode);
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertLeaveDetailService:");
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


        #endregion

        #region Feedback

        public ReturnIdModel InsertFeedbackService(string authorization, string lang, string platform, int logID,
            FeedbackDTO feedbackDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidationDupicateInsertFeedback(lang, feedbackDTO);
                if (validation.Success == true)
                {
                    value.data = _sql.InsertFeedback(feedbackDTO, userID);
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertFeedbackService:");
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
        #endregion

        #region Insert Time Attendance

        public ReturnIdModel InsertEmpWorkTimeV1_1Service(string shareCode, string authorization, string lang, string platform, int logID,
            SaveEmpWorkTimeRequestDTO_V1_1 saveEmpWorkTimeRequestDTO_V1_1, string roleIDList, int tokenUserID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 0, lang, platform);
                if (validation.Success == true)
                {
                    if (saveEmpWorkTimeRequestDTO_V1_1.empWorkTimeRequestDTO.Count > 0)
                    {
                        foreach (EmpWorkTimeRequestDTO item in saveEmpWorkTimeRequestDTO_V1_1.empWorkTimeRequestDTO)
                        {
                            if (item.empWorkTimeID == 0)
                            {
                                value.data = _sql.insertWorkTime(shareCode, item, tokenUserID);
                            }
                            else
                            {
                                _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                            }
                        }
                    }
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertEmpWorkTimeV1_1Service:");
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

        #endregion

        #region SystemRole
        public ReturnIdModel InsertSystemRoleTempService(string authorization, string lang, string platform, int logID,
    SaveSystemRoleDTO saveSystemRoleDTO, string roleIDList, int userID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidationDupicateInsertSystemRole(lang, saveSystemRoleDTO);
                if (validation.Success == true)
                {
                    value.data = _sql.InsertSystemRole(shareCode, saveSystemRoleDTO, userID, platform);
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertSystemRoleService:");
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

        public ReturnIdModel InsertSystemRoleAssignService(string authorization, string lang, string platform, int logID, SaveSystemRoleDTO saveSystemRoleDTO,
SaveSystemRole saveSystemRole, string roleIDList, int userID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidationObject(lang, saveSystemRoleDTO);
                if (validation.Success == true)
                {
                    value.data = _sql.InsertSystemRoleAssign(shareCode, saveSystemRoleDTO, saveSystemRole, userID);
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertSystemRoleService:");
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


        #endregion
    }
}
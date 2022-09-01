using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TUFTManagement.Core;
using TUFTManagement.DTO;
using TUFTManagement.Models;
using static TUFTManagement.DTO.SaveChangeWorkShiftTimeRequestDTO;
using static TUFTManagement.DTO.SaveEmpWorkTimeRequestDTO_V1_1;

namespace TUFTManagement.Services
{
    public class InsertService
    {
        private SQLManager _sql = SQLManager.Instance;

        #region Insert Employees

        public InsertLoginModel InsertEmpProfileService(string shareCode, string authorization, string lang, string platform, int logID,
            SaveEmpProfileDTO saveEmpProfileDTO, int userID)
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
            SaveEmpRateRequestDTO saveEmpRateDTO, int userID)
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
            SaveEmpWorkShiftRequestDTO saveEmpWorkShiftRequestDTO, int userID)
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
    SaveLeaveDetailDTO saveLeaveDetailDTO, int userID,string shareCode)
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
            SaveEmpWorkTimeRequestDTO_V1_1 saveEmpWorkTimeRequestDTO_V1_1, int tokenUserID)
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

        public ReturnIdModel InsertEmpWorkShiftTimeTransChangeService(string shareCode, string authorization, string lang, string platform, int logID,
            SaveChangeWorkShiftTimeRequestDTO saveChangeWorkShiftTimeRequestDTO, int tokenUserID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            ReturnIdModel value = new ReturnIdModel();
            
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidationWorktime(1, lang, platform, saveChangeWorkShiftTimeRequestDTO.empWorkTimeID, shareCode);
                if (validation.Success == true)
                {
                    int firstTransChange = _sql.InsertFirstEmpWorkTimeTransChangeTrade(shareCode, saveChangeWorkShiftTimeRequestDTO, tokenUserID);
                    int secondTransChange = _sql.InsertSecondEmpWorkTimeTransChangeTrade(shareCode, saveChangeWorkShiftTimeRequestDTO, tokenUserID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertEmpWorkShiftTimeTransChangeService:");
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

        public ReturnIdModel ApproveEmpWorkShiftTimeTransChangeService(string shareCode, string authorization, string lang, string platform, int logID,
            ApproveChangeWorkShiftTimeRequestDTO approveChangeWorkShiftTimeRequestDTO, int tokenUserID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            ReturnIdModel value = new ReturnIdModel();

            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidationApproveTransChange(1, lang, platform, approveChangeWorkShiftTimeRequestDTO, shareCode);

                if (validation.Success == true)
                {
                    //Approve CASE
                    foreach (int ApproveEmpWorkTimeID in approveChangeWorkShiftTimeRequestDTO.approveListEmpWorkTimeID)
                    {
                        GetTransWorkShiftDTO getDetails = _sql.GetTransChangeWorkShift(shareCode, ApproveEmpWorkTimeID);
                        
                        
                        if (getDetails.action == 1)
                        {
                            //Update work shift CASE
                            SaveChangeWorkShiftTimeRequestDTO itemUpdate = new SaveChangeWorkShiftTimeRequestDTO();
                            itemUpdate.empWorkTimeID = getDetails.workTimeID;
                            itemUpdate.newWorkShiftID = getDetails.workShiftIdNew;

                            int changeCase = _sql.UpdateEmpWorkShif(shareCode, itemUpdate, tokenUserID);
                        }
                        else if (getDetails.action == 2)
                        {
                            //Trade work shift CASE
                            SaveChangeWorkShiftTimeRequestDTO targetUpdate = new SaveChangeWorkShiftTimeRequestDTO();
                            targetUpdate.empWorkTimeID = getDetails.workTimeID;
                            targetUpdate.newWorkShiftID = getDetails.workShiftIdNew;

                            SaveChangeWorkShiftTimeRequestDTO effectUpdate = new SaveChangeWorkShiftTimeRequestDTO();
                            effectUpdate.empWorkTimeID = getDetails.tradeWorkTimeID;
                            effectUpdate.newWorkShiftID = getDetails.workTimeID;
                            
                            int tradeTargetCase = _sql.UpdateEmpWorkShif(shareCode, targetUpdate, tokenUserID);
                            int tradeEffectCase = _sql.UpdateEmpWorkShif(shareCode, effectUpdate, tokenUserID);
                            
                        }
                    }
                    string approvedList = _sql.ApproveEmpWorkShift(shareCode, approveChangeWorkShiftTimeRequestDTO.prepairApproveListEmpWorkTimeID, 1, tokenUserID);
                    
                    //Reject CASE
                    string rejectList = _sql.ApproveEmpWorkShift(shareCode, approveChangeWorkShiftTimeRequestDTO.prepairRejectListEmpWorkTimeID, 2, tokenUserID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "ApproveEmpWorkShiftTimeTransChangeService:");
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
        public ReturnIdModel InsertSystemRoleTempService(string authorization, string lang, string platform, int logID, SaveSystemRoleTemp saveSystemRoleTemp, int userID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidationDupicateInsertSystemRole(lang, saveSystemRoleTemp);
                if (validation.Success == true)
                {
                    value.data = _sql.InsertSystemRole(shareCode, saveSystemRoleTemp, userID, platform);
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

        public ReturnIdModel InsertSystemRoleAssignService(string authorization, string lang, string platform, int logID, SaveSystemRoleAssignDTO saveSystemRoleAssignDTO, SaveSystemRoleTemp saveSystemRoleTemp, int userID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidationObject(lang, saveSystemRoleAssignDTO);
                if (validation.Success == true)
                {
                    value.data = _sql.InsertSystemRoleAssign(shareCode, saveSystemRoleAssignDTO, saveSystemRoleTemp, userID);
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
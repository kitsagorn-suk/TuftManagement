using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TUFTManagement.Core;
using TUFTManagement.DTO;
using TUFTManagement.Models;

namespace TUFTManagement.Services
{
    public class InsertService
    {
        private SQLManager _sql = SQLManager.Instance;

        #region Insert Employees

        public InsertLoginModel InsertEmpProfileService(string authorization, string lang, string platform, int logID,
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
                ValidationModel validation = ValidationManager.CheckValidationDupicateInsertEmp(lang, saveEmpProfileDTO); 
                if (validation.Success == true)
                {
                    //รอเรื่องสิทธิ์
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100301001");
                    //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        value.data = _sql.InsertEmpProfile(saveEmpProfileDTO, userID);
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
                        value.data = _sql.InsertEmpRate(saveEmpRateDTO, userID);
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

    }
}
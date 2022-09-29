using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TUFTManagement.Core;
using TUFTManagement.DTO;
using TUFTManagement.Models;

namespace TUFTManagement.Services
{
    public class DeleteService
    {
        private SQLManager _sql = SQLManager.Instance;

        public ReturnIdModel DeleteEmpProfileService(string authorization, string lang, string platform, int logID,
            SaveEmpProfileDTO saveEmpProfileDTO, int userID, string projectName, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();

                //เช็คสิทธิในการเข้าใช้
                //Employee > Add Employee > Delete Employee
                List<string> listobjectID = new List<string>();
                listobjectID.Add("2041000");
                string objectID = string.Join(",", listobjectID.ToArray());
                ValidationModel validation = ValidationManager.CheckValidationWithProjectName(shareCode, lang, objectID, projectName, userID);
                
                if (validation.Success == true)
                {
                    value.data = _sql.DeleteEmpProfile(saveEmpProfileDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteEmergencyContactSevice:");
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

        public ReturnIdModel DeleteEmpFileService(string shareCode, string authorization, string lang, string platform, int logID,
            RequestDTO requestDTO, int userID)
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
                    value.data = _sql.DeleteEmpfile(shareCode, requestDTO.id, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteEmpFileService:");
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

        public ReturnIdModel DeleteEmpRateService(string authorization, string lang, string platform, int logID,
            EmpRateRequestDTO empRateRequestDTO, int userID)
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
                    value.data = _sql.DeleteEmpRate(empRateRequestDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteEmpRateService:");
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

        public ReturnIdModel DeleteEmpWorkShiftService(string authorization, string lang, string platform, int logID,
            SaveEmpWorkShiftRequestDTO requestDTO, int userID)
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
                    value.data = _sql.DeleteEmpWorkShift(requestDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteEmpWorkShiftService:");
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
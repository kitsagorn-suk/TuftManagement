using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TUFTManagement.Core;
using TUFTManagement.DTO;
using TUFTManagement.Models;

namespace TUFTManagement.Services
{
    public class UpdateService
    {
        private SQLManager _sql = SQLManager.Instance;

        public ReturnIdModel UpdateEmpProfileService(string authorization, string lang, string platform, int logID, SaveEmpProfileDTO saveEmpProfileDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();

                //รอเรื่องสิทธิ์
                //ValidationModel validation = new ValidationModel();
                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100201001");
                //listobjectID.Add("100301003");
                //validation = ValidationManager.CheckValidationUpdate(insertEmpProfileDTO.empProfileID, "emp_profile", lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    string TableName = "emp_profile";
                    _sql.InsertSystemLogChange(saveEmpProfileDTO.empProfileID, TableName, "identity_card", saveEmpProfileDTO.identityCard, userID);
                    _sql.InsertSystemLogChange(saveEmpProfileDTO.empProfileID, TableName, "title_id", saveEmpProfileDTO.titleID.ToString(), userID);
                    _sql.InsertSystemLogChange(saveEmpProfileDTO.empProfileID, TableName, "firstname_en", saveEmpProfileDTO.firstNameEN, userID);
                    _sql.InsertSystemLogChange(saveEmpProfileDTO.empProfileID, TableName, "firstname_th", saveEmpProfileDTO.firstNameTH, userID);
                    _sql.InsertSystemLogChange(saveEmpProfileDTO.empProfileID, TableName, "lastname_en", saveEmpProfileDTO.lastNameEN, userID);
                    _sql.InsertSystemLogChange(saveEmpProfileDTO.empProfileID, TableName, "lastname_th", saveEmpProfileDTO.lastNameTH, userID);
                    _sql.InsertSystemLogChange(saveEmpProfileDTO.empProfileID, TableName, "nickname", saveEmpProfileDTO.nickName, userID);
                    _sql.InsertSystemLogChange(saveEmpProfileDTO.empProfileID, TableName, "phone_number", saveEmpProfileDTO.phoneNumber, userID);
                    _sql.InsertSystemLogChange(saveEmpProfileDTO.empProfileID, TableName, "position_id", saveEmpProfileDTO.positionID.ToString(), userID);
                    _sql.InsertSystemLogChange(saveEmpProfileDTO.empProfileID, TableName, "per_num", saveEmpProfileDTO.personalNO.ToString(), userID);
                    _sql.InsertSystemLogChange(saveEmpProfileDTO.empProfileID, TableName, "date_of_birth", saveEmpProfileDTO.dateOfBirth, userID);
                    _sql.InsertSystemLogChange(saveEmpProfileDTO.empProfileID, TableName, "join_date", saveEmpProfileDTO.joinDate, userID);
                    _sql.InsertSystemLogChange(saveEmpProfileDTO.empProfileID, TableName, "pro_pass_date", saveEmpProfileDTO.proPassDate, userID);
                    _sql.InsertSystemLogChange(saveEmpProfileDTO.empProfileID, TableName, "monthly_salary", saveEmpProfileDTO.monthlySalary.ToString(), userID);
                    _sql.InsertSystemLogChange(saveEmpProfileDTO.empProfileID, TableName, "daily_salary", saveEmpProfileDTO.dailySalary.ToString(), userID);
                    _sql.InsertSystemLogChange(saveEmpProfileDTO.empProfileID, TableName, "employment_type_id", saveEmpProfileDTO.employmentTypeID.ToString(), userID);
                    _sql.InsertSystemLogChange(saveEmpProfileDTO.empProfileID, TableName, "role_id", saveEmpProfileDTO.roleID.ToString(), userID);
                    value.data = _sql.UpdateEmpProfile(saveEmpProfileDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateAddressService:");
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

        public ReturnIdModel UpdateEmpRateService(string authorization, string lang, string platform, int logID, SaveEmpRateRequestDTO saveEmpRateDTO, int roleID, int userID)
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
                    string TableName = "emp_rate";
                    _sql.InsertSystemLogChange(saveEmpRateDTO.empRateID, TableName, "product_code", saveEmpRateDTO.productCode, userID);
                    _sql.InsertSystemLogChange(saveEmpRateDTO.empRateID, TableName, "rate_staff", saveEmpRateDTO.rateStaff.ToString(), userID);
                    _sql.InsertSystemLogChange(saveEmpRateDTO.empRateID, TableName, "rate_manager", saveEmpRateDTO.rateManager.ToString(), userID);
                    _sql.InsertSystemLogChange(saveEmpRateDTO.empRateID, TableName, "rate_owner", saveEmpRateDTO.rateOwner.ToString(), userID);
                    _sql.InsertSystemLogChange(saveEmpRateDTO.empRateID, TableName, "rate_confirm", saveEmpRateDTO.rateConfirm.ToString(), userID);

                    value.data = _sql.UpdateEmpRate(saveEmpRateDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateEmpRateService:");
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

        public ReturnIdModel UpdateEmpWorkShiftService(string authorization, string lang, string platform, int logID, SaveEmpWorkShiftRequestDTO saveEmpWorkShiftRequestDTO, int roleID, int userID)
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
                    string TableName = "emp_rate";
                    _sql.InsertSystemLogChange(saveEmpWorkShiftRequestDTO.empWorkShiftID, TableName, "ws_code", saveEmpWorkShiftRequestDTO.wsCode, userID);
                    _sql.InsertSystemLogChange(saveEmpWorkShiftRequestDTO.empWorkShiftID, TableName, "time_start", saveEmpWorkShiftRequestDTO.timeStart, userID);
                    _sql.InsertSystemLogChange(saveEmpWorkShiftRequestDTO.empWorkShiftID, TableName, "time_end", saveEmpWorkShiftRequestDTO.timeEnd, userID);
                    _sql.InsertSystemLogChange(saveEmpWorkShiftRequestDTO.workTypeID, TableName, "work_type_id", saveEmpWorkShiftRequestDTO.workTypeID.ToString(), userID);

                    value.data = _sql.UpdateEmpWorkShift(saveEmpWorkShiftRequestDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateEmpWorkShiftService:");
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
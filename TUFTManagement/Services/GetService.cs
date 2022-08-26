using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TUFTManagement.Core;
using TUFTManagement.DTO;
using TUFTManagement.Models;
using static TUFTManagement.Models.EmployeeDetails;

namespace TUFTManagement.Services
{
    public class GetService
    {
        private SQLManager _sql = SQLManager.Instance;

        public GetAllDropdownModel GetAllDropdownService(string authorization, string lang, string platform, int logID, GetDropdownRequestDTO request)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetAllDropdownModel value = new GetAllDropdownModel();
            try
            {
                value.data = new List<_DropdownAllData>();

                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);
                if (validation.Success == true)
                {
                    if (request.moduleName == "district")
                    {
                        value.data = _sql.GetDropdownDistrict(lang, request.provinceID);
                    }
                    else
                    {
                        value.data = _sql.GetDropdownByModuleName(lang, request.moduleName);
                    }
                    value.success = validation.Success;
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetAllDropdownService:");
                if (logID > 0)
                {
                    _sql.UpdateLogReceiveDataError(logID, ex.ToString());
                }
                throw new Exception(ex.Message);
            }
            finally
            {
                _sql.UpdateStatusLog(logID, 1);
            }
            return value;
        }
        public GetSubDistrictDropdownModel GetSubDistrictDropdownService(string authorization, string lang, string platform, int logID, GetDropdownRequestDTO request)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetSubDistrictDropdownModel value = new GetSubDistrictDropdownModel();
            try
            {
                value.data = new List<DropdownSubDistrict>();

                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);
                if (validation.Success == true)
                {
                    value.data = _sql.GetDropdownSubDistrict(lang, request.districtID);
                    value.success = validation.Success;
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetSubDistrictDropdownService:");
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
        public GetEmpTradeWorkShiftDropdownModel GetEmpTradeWorkShiftDropdownService(string authorization, string lang, string platform, int logID, GetDropdownRequestDTO request)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetEmpTradeWorkShiftDropdownModel value = new GetEmpTradeWorkShiftDropdownModel();
            try
            {
                value.data = new List<EmpTradeWorkShift>();

                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);
                if (validation.Success == true)
                {
                    value.data = _sql.GetDropdownEmpTradeWorkShift(lang, request.workDate);
                    value.success = validation.Success;
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetSubDistrictDropdownService:");
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
        public GetDropdownTitleNameModel GetTitleNameDropdownService(string authorization, string lang, string platform, int logID, GetDropdownRequestDTO request)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetDropdownTitleNameModel value = new GetDropdownTitleNameModel();
            try
            {
                value.data = new List<DropdownTitleName>();

                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);
                if (validation.Success == true)
                {
                    value.data = _sql.GetDropdownTitle(lang);
                    value.success = validation.Success;
                }   
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetSubDistrictDropdownService:");
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
        public GetAllDropdownModel GetPositionByDepartmentDropdownService(string authorization, string lang, string platform, int logID, GetDropdownRequestDTO request)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetAllDropdownModel value = new GetAllDropdownModel();
            try
            {
                value.data = new List<_DropdownAllData>();

                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);
                if (validation.Success == true)
                {
                    value.data = _sql.GetDropdownPositionFilter(lang, request.departmentID);
                    value.success = validation.Success;
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetPositionByDepartmentDropdownService:");
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


        public GetEmpProfileModel GetEmpProfileService(string shareCode, string authorization, string lang, string platform, int logID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetEmpProfileModel value = new GetEmpProfileModel();
            try
            {
                EmpProfile data = new EmpProfile();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetEmpProfile(shareCode, userID, lang);
                    value.data = data;

                    value.data.role = new List<RoleIDList>();
                    value.data.role = _sql.GetUserRole(userID, lang);

                    value.data.shareHolder = new List<ShareHolderList>();
                    value.data.shareHolder = _sql.GetUserShareHolder(userID, lang);

                    value.data.accessList = new List<AccessRole>();

                    value.success = validation.Success;
                }
                else
                {
                    _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, validation.InvalidMessage);
                }

                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterService:");
                if (logID > 0)
                {
                    _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, ex.ToString());
                }
                throw ex;
            }
            finally
            {
                _sql.UpdateStatusLog(logID, 1);
            }
            return value;
        }

        public GetEmployeeDetailsModel GetEmpProfileService(string shareCode, string authorization, string lang, string platform, int logID, int userID, RequestDTO requestDTO)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetEmployeeDetailsModel value = new GetEmployeeDetailsModel();
            try
            {
                EmployeeDetails data = new EmployeeDetails();
                data.emergencyContact = new List<EmergencyContact>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetEmpProfile(shareCode, userID, lang, requestDTO);
                    data.emergencyContact = _sql.GetEmerContact(shareCode, requestDTO.userID);
                    data.imageGallary = _sql.GetImgGallary(shareCode, requestDTO.userID);
                    value.data = data;
                    value.success = validation.Success;
                }
                else
                {
                    _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, validation.InvalidMessage);
                }

                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetEmpProfileService:");
                if (logID > 0)
                {
                    _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, ex.ToString());
                }
                throw ex;
            }
            finally
            {
                _sql.UpdateStatusLog(logID, 1);
            }
            return value;
        }

        public GetfileByCodeModel GetFileByCodeService(string shareCode, string authorization, string lang, string platform, int logID, int userID, string fileCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetfileByCodeModel value = new GetfileByCodeModel();
            try
            {
                List<_GetfileByCode> data = new List<_GetfileByCode>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetEmpFileByCode(shareCode, userID, lang, fileCode);
                    value.data = data;
                    value.success = validation.Success;
                }
                else
                {
                    _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, validation.InvalidMessage);
                }

                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetFileByCodeService:");
                if (logID > 0)
                {
                    _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, ex.ToString());
                }
                throw ex;
            }
            finally
            {
                _sql.UpdateStatusLog(logID, 1);
            }
            return value;
        }

        public GetEmpRateModel GetEmpRateService(string authorization, string lang, string platform, int logID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetEmpRateModel value = new GetEmpRateModel();
            try
            {
                GetEmpRate data = new GetEmpRate();

                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetEmpRate(userID);
                    value.data = data;
                    value.success = validation.Success;
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetEmpRateService:");
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

        public GetLeaveDetailModel GetLeaveDetailService(string authorization, string lang, string platform, int logID, GetLeaveDetailRequestDTO leaveDetailDTO,string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetLeaveDetailModel value = new GetLeaveDetailModel();
            try
            {
                GetLeaveDetail data = new GetLeaveDetail();

                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetLeaveDetail(leaveDetailDTO, shareCode);
                    value.data = data;
                    value.success = validation.Success;
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetLeaveDetailService:");
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


        public GetFeedbackModel GetFeedbackService(string authorization, string lang, string platform, int logID, int userID)
        {
            GetFeedbackModel value = new GetFeedbackModel();
            try
            {
                GetFeedback data = new GetFeedback();
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetFeedback(userID);
                    value.data = data;
                    value.success = validation.Success;
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {

                LogManager.ServiceLog.WriteExceptionLog(ex, "GetFeedbackService:");

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

        public GetEmpWorkShiftModel GetEmpWorkShiftService(string authorization, string lang, string platform, int logID, int empWorkShiftID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetEmpWorkShiftModel value = new GetEmpWorkShiftModel();
            try
            {
                GetEmpWorkShift data = new GetEmpWorkShift();

                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetEmpWorkShift(empWorkShiftID);
                    value.data = data;
                    value.success = validation.Success;
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {

                LogManager.ServiceLog.WriteExceptionLog(ex, "GetEmpWorkShiftService:");

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

        public GetEmpWorkTimeModel GetEmpWorkTimeService(string authorization, string lang, string platform, int logID, int empWorkTimeID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetEmpWorkTimeModel value = new GetEmpWorkTimeModel();
            try
            {
                GetEmpWorkTime data = new GetEmpWorkTime();

                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetEmpWorkTime(empWorkTimeID);
                    value.data = data;
                    value.success = validation.Success;
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetEmpWorkTimeService:");
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

        #region search service
        public SearchWorkTimeModel SearchWorkTimeService(string authorization, string lang, string platform, int logID, SearchWorkTimeDTO searchWorkTimeDTO, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchWorkTimeModel value = new SearchWorkTimeModel();
            try
            {
                Pagination<SearchWorkTime> data = new Pagination<SearchWorkTime>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchWorkTime(shareCode, searchWorkTimeDTO);
                }
                else
                {
                    _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.data = data;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchWorkTime:");
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

        public SearchWorkTimePendingModel SearchWorkTimePendingService(string authorization, string lang, string platform, int logID, SearchWorkTimePendingDTO searchWorkTimePendingDTO, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchWorkTimePendingModel value = new SearchWorkTimePendingModel();
            try
            {
                Pagination<SearchWorkTimePendingPage> data = new Pagination<SearchWorkTimePendingPage>();
                SearchWorktimePendingTotalDTO header = new SearchWorktimePendingTotalDTO();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchWorkTimePending(shareCode, searchWorkTimePendingDTO);

                    List<SearchWorkTimePendingPage> total = data.data;

                    List<int> termsList = new List<int>();
                    foreach (SearchWorkTimePendingPage item in total)
                    {
                        termsList.Add(item.userID);
                    }
                    int[] terms = termsList.ToArray();
                    string theString = string.Join(",", terms);

                    header = _sql.GetTotalSearchWorkTimePendingList(shareCode, theString);
                }
                else
                {
                    _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, validation.InvalidMessage);
                }

                SearchWorkTimePending dataValue = new SearchWorkTimePending();
                dataValue.header = header;
                dataValue.body = data;

                value.success = validation.Success;
                value.data = dataValue;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchWorkTimePendingService:");
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
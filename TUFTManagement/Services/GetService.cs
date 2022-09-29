﻿using Newtonsoft.Json;
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
        public GetEmpTradeWorkShiftDropdownModel GetEmpTradeWorkShiftDropdownService(string shareCode, string authorization, string lang, string platform, int logID, GetDropdownRequestDTO request)
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
                    value.data = _sql.GetDropdownEmpTradeWorkShift(shareCode, lang, request.workDate);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetEmpTradeWorkShiftDropdownService:");
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


        public GetEmpProfileModel GetEmpProfileV1_1Service(string shareCode, string authorization, string lang, string fromProject, 
            int logID, int userID, string projectName)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetEmpProfileModel value = new GetEmpProfileModel();
            try
            {
                EmpProfile data = new EmpProfile();

                //เช็คสิทธิในการเข้าใช้
                //Dashboard > Profile Self
                List<string> listobjectID = new List<string>();
                listobjectID.Add("1004000");
                string objectID = string.Join(",", listobjectID.ToArray());
                ValidationModel validation = ValidationManager.CheckValidationWithProjectName(shareCode, lang, objectID, projectName, userID);

                if (validation.Success == true)
                {
                    data = _sql.GetEmpProfile(shareCode, userID, lang);
                    value.data = data;

                    //value.data.role = new List<RoleIDList>();
                    //value.data.role = _sql.GetUserRole(userID, lang);

                    value.data.shareHolder = new List<ShareHolderList>();
                    value.data.shareHolder = _sql.GetUserShareHolder(userID, lang, fromProject);

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

        public GetEmployeeDetailsModel GetEmpProfileService(string shareCode, string authorization, string lang, string platform, 
            int logID, int userID, RequestDTO requestDTO, string projectName)
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

                //เช็คสิทธิในการเข้าใช้
                //Employee > All Employee > View
                List<string> listobjectID = new List<string>();
                listobjectID.Add("2082000");
                string objectID = string.Join(",", listobjectID.ToArray());
                ValidationModel validation = ValidationManager.CheckValidationWithProjectName(shareCode, lang, objectID, projectName, requestDTO.userID, userID);

                string cutObjectID = "2072100";
                List<NewMenuList> menuList = ValidationManager.ReturnObjectID(shareCode, lang, projectName, cutObjectID, requestDTO.userID, userID);
                

                if (validation.Success == true)
                {
                    data = _sql.GetEmpProfile(shareCode, userID, lang, requestDTO);
                    data.emergencyContact = _sql.GetEmerContact(shareCode, requestDTO.userID);
                    data.imageGallery = _sql.GetImgGallary(shareCode, requestDTO.userID);
                    value.data = data;
                    value.data.menuList = menuList;
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

        public GetAllEmployeePrettyModel GetEmployeePrettyService(string shareCode, string authorization, string lang, string platform,
            int logID, PageRequestDTO pageRequestDTO, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetAllEmployeePrettyModel value = new GetAllEmployeePrettyModel();
            try
            {
                value.data = new Pagination<GetAllEmployee>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    value.data = _sql.GetAllEmployeePretty(shareCode, userID, lang, pageRequestDTO);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetEmployeePrettyService:");
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

        public GetAllEmployeeByPositionModel GetEmployeeByPositionService(string shareCode, string authorization, string lang, string platform,
            int logID, int userID, int position, PageRequestDTO pageRequestDTO)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetAllEmployeeByPositionModel value = new GetAllEmployeeByPositionModel();
            try
            {
                Pagination<GetAllEmployeeNormal> data = new Pagination<GetAllEmployeeNormal>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetAllEmployeeNormal(shareCode, userID, lang, position, pageRequestDTO);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetEmployeeByPositionService:");
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

        public GetLeaveDetailModel GetLeaveDetailService(string authorization, string lang, string platform, int logID, GetLeaveDetailRequestDTO leaveDetailDTO,string shareCode, string projectName, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetLeaveDetailModel value = new GetLeaveDetailModel();
            try
            {
                GetLeaveDetail data = new GetLeaveDetail();

                //เช็คสิทธิในการเข้าใช้
                //Employee > Leave
                List<string> listobjectID = new List<string>();
                listobjectID.Add("2100000");
                string objectID = string.Join(",", listobjectID.ToArray());
                ValidationModel validation = ValidationManager.CheckValidationWithProjectName(shareCode, lang, objectID, projectName, userID);

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

        public GetEmpWorkShiftTimeModel GetEmpWorkShiftTimeService(string authorization, string lang, string platform, int logID, 
            GetHistoryWorkShiftTimeDTO getHistoryWorkShiftTimeDTO, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetEmpWorkShiftTimeModel value = new GetEmpWorkShiftTimeModel();
            value.data = new EmpWorkShiftTimeDetail();
            value.data.dataHeader = new EmpWorkShiftTimeHeader();
            value.data.dataSearch = new Pagination<EmpWorkShiftTimeSearch>();
            try
            {
                Pagination<EmpWorkShiftTimeSearch> data = new Pagination<EmpWorkShiftTimeSearch>();
                EmpWorkShiftTimeHeader dataHeader = new EmpWorkShiftTimeHeader();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetWorkShiftTime(getHistoryWorkShiftTimeDTO,lang,shareCode);
                    dataHeader = _sql.GetWorkShiftTimeHeader(getHistoryWorkShiftTimeDTO.empId, lang, shareCode);
                }
                else
                {
                    _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.data.dataHeader = dataHeader;
                value.data.dataSearch = data;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterKeyService:");
                if (logID > 0)
                {
                    _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, ex.ToString());
                }
                throw ex;
            }
            finally
            {
                _sql.UpdateStatusLogWithShareCode(shareCode, logID, 1);
            }
            return value;
        }

        public GetWorkShiftTimeHeaderModel GetEmpWorkShiftHeaderService(string authorization, string lang, string platform, int logID, int empWorkShiftID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetWorkShiftTimeHeaderModel value = new GetWorkShiftTimeHeaderModel();
            try
            {
                HeaderDetail data = new HeaderDetail();

                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetWorkShiftTimeHeaderByWTId(empWorkShiftID, lang, shareCode);
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


        #region search service
        public SearchAllEmployeeModel SearchAllEmployee(string authorization, string lang, string platform, int logID, PageRequestDTO pageRequestDTO, string shareCode, string projectName, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchAllEmployeeModel value = new SearchAllEmployeeModel();
            try
            {
                Pagination<SearchAllEmployee> data = new Pagination<SearchAllEmployee>();

                //เช็คสิทธิในการเข้าใช้
                //Employee > All Employee > Search
                List<string> listobjectID = new List<string>();
                listobjectID.Add("2081000");
                string objectID = string.Join(",", listobjectID.ToArray());
                ValidationModel validation = ValidationManager.CheckValidationWithProjectName(shareCode, lang, objectID, projectName, userID);

                if (validation.Success == true)
                {
                    data = _sql.SearchAllEmployee(shareCode, pageRequestDTO);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchAllEmployee:");
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

        public SearchWorkTimeModel SearchWorkTimeService(string authorization, string lang, string platform, int logID, SearchWorkTimeDTO searchWorkTimeDTO, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchWorkTimeModel value = new SearchWorkTimeModel();
            value.data = new SearchWorkTimeAll();
            value.data.header = new List<SearchWorkShiftTimeAllTotalDTO>();
            value.data.body = new Pagination<SearchWorkTime>();
            try
            {
                SearchWorkTimeAll data = new SearchWorkTimeAll();
                List<SearchWorkShiftTimeAllTotalDTO> dataheader = new List<SearchWorkShiftTimeAllTotalDTO>();
                Pagination<SearchWorkTime> databody = new Pagination<SearchWorkTime>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    databody = _sql.SearchWorkTime(shareCode, searchWorkTimeDTO);

                    List<SearchWorkTime> total = databody.data;

                    List<int> termsList = new List<int>();
                    foreach (SearchWorkTime item in total)
                    {
                        termsList.Add(item.empProfileID);
                    }
                    int[] terms = termsList.ToArray();
                    string theString = string.Join(",", terms);

                    List<GetWorkShift> allWS = new List<GetWorkShift>();
                    allWS = _sql.GetAllWorkShiftList(shareCode);

                    foreach (GetWorkShift item in allWS)
                    {
                        SearchWorkShiftTimeAllTotalDTO sss = new SearchWorkShiftTimeAllTotalDTO();
                        sss = _sql.GetWorkShiftTotalHeader(shareCode, theString, item.workShiftID, searchWorkTimeDTO.dateSearch);
                       
                        dataheader.Add(sss);
                    }

                    
                    
                    
                }
                else
                {
                    _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.data.header = dataheader;
                value.data.body = databody;
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

        #region systemrole

        public SearchAllSystemRoleAssignModel SearchAllSystemRoleAssign(string authorization, string lang, string platform, int logID, SearchSystemRoleAssignDTO searchSystemRoleAssignDTO, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchAllSystemRoleAssignModel value = new SearchAllSystemRoleAssignModel();
            try
            {
                Pagination<SearchAllSystemRoleAssign> data = new Pagination<SearchAllSystemRoleAssign>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchAllSystemRoleAssign(shareCode, searchSystemRoleAssignDTO);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchAllSystemRoleAssign:");
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

        public GetDetailSystemRoleAssignModel GetDetailSystemRoleAssignService(string authorization, string lang, string platform, int logID, int ID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetDetailSystemRoleAssignModel value = new GetDetailSystemRoleAssignModel();
            try
            {
                Assign data = new Assign();


                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetDetailSystemRoleAssign(ID, shareCode);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetBodySetService:");
                if (logID > 0)
                {
                    _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, ex.ToString());
                }
                throw ex;
            }
            finally
            {
                _sql.UpdateStatusLogWithShareCode(shareCode, logID, 1);
            }
            return value;
        }

        public SearchAllSystemRoleTempModel SearchAllSystemRoleTemp(string authorization, string lang, string platform, int logID, SearchSystemRoleTempDTO searchSystemRoleTempDTO, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchAllSystemRoleTempModel value = new SearchAllSystemRoleTempModel();
            try
            {
                Pagination<SearchAllSystemRoleTemp> data = new Pagination<SearchAllSystemRoleTemp>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchAllSystemRoleTemp(shareCode, searchSystemRoleTempDTO);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchAllSystemRoleAssign:");
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

        public GetDetailSystemRoleTempModel GetDetailSystemRoleTempService(string authorization, string lang, string platform, int logID, SaveSystemRoleTempDTO saveSystemRoleTempDTO, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetDetailSystemRoleTempModel value = new GetDetailSystemRoleTempModel();
            try
            {
                Temp data = new Temp();


                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetDetailSystemRoleTemp(saveSystemRoleTempDTO, platform, shareCode);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetDetailSystemRoleTempService:");
                if (logID > 0)
                {
                    _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, ex.ToString());
                }
                throw ex;
            }
            finally
            {
                _sql.UpdateStatusLogWithShareCode(shareCode, logID, 1);
            }
            return value;
        }

        #endregion

        #region Report

        public SearchAllSalaryReportModel SearchAllSalaryReportService(string authorization, string lang, string platform, int logID, SearchReportDTO searchReportSalaryDTO, string shareCode, string projectName, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchAllSalaryReportModel value = new SearchAllSalaryReportModel();
            try
            {
                Pagination<SearchAllSalaryReport> data = new Pagination<SearchAllSalaryReport>();

                //เช็คสิทธิในการเข้าใช้
                //Report > Salary
                List<string> listobjectID = new List<string>();
                listobjectID.Add("4020000");
                string objectID = string.Join(",", listobjectID.ToArray());
                ValidationModel validation = ValidationManager.CheckValidationWithProjectName(shareCode, lang, objectID, projectName, userID);

                if (validation.Success == true)
                {
                    data = _sql.SearchAllSalaryReport(shareCode, searchReportSalaryDTO);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchAllSalaryReportService:");
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

        public SearchAllEmployeeReportModel SearchAllEmployeeReportService(string authorization, string lang, string platform, int logID, SearchReportDTO searchReportEmployeeDTO, string shareCode, string projectName, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchAllEmployeeReportModel value = new SearchAllEmployeeReportModel();
            value.data = new SearchAllEmployeeReport();
            value.data.header = new EmployeeReportHeader();
            value.data.body = new Pagination<SearchAllEmployeeReportBody>();
            try
            {
                EmployeeReportHeader dataHeader = new EmployeeReportHeader();
                Pagination<SearchAllEmployeeReportBody> dataBody = new Pagination<SearchAllEmployeeReportBody>();

                //เช็คสิทธิในการเข้าใช้
                //Report > Employees
                List<string> listobjectID = new List<string>();
                listobjectID.Add("4010000");
                string objectID = string.Join(",", listobjectID.ToArray());
                ValidationModel validation = ValidationManager.CheckValidationWithProjectName(shareCode, lang, objectID, projectName, userID);

                if (validation.Success == true)
                {
                    dataHeader = _sql.GetEmployeeReportHeader(shareCode);
                    dataBody = _sql.SearchAllEmployeeReport(shareCode, searchReportEmployeeDTO);
                }
                else
                {
                    _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.data.header = dataHeader;
                value.data.body = dataBody;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchAllEmployeeReportService:");
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

        public SearchAllWorkTimeReportModel SearchAllWorkTimeReportService(string authorization, string lang, string platform, int logID, SearchReportDTO searchReportWorkTimeDTO, string shareCode, string projectName, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchAllWorkTimeReportModel value = new SearchAllWorkTimeReportModel();
            try
            {
                Pagination<SearchAllWorkTimeReport> data = new Pagination<SearchAllWorkTimeReport>();

                //เช็คสิทธิในการเข้าใช้
                //Report > Work time
                List<string> listobjectID = new List<string>();
                listobjectID.Add("4040000");
                string objectID = string.Join(",", listobjectID.ToArray());
                ValidationModel validation = ValidationManager.CheckValidationWithProjectName(shareCode, lang, objectID, projectName, userID);

                if (validation.Success == true)
                {
                    data = _sql.SearchAllWorkTimeReport(shareCode, searchReportWorkTimeDTO);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchAllWorkTimeReportService:");
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

        public SearchAllWorkTimeReportModel SearchAllPayRollService(string authorization, string lang, string platform, int logID, SearchPayRollDTO searchPayRollDTO, string shareCode)
        { //เปลี่ยน model
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchAllWorkTimeReportModel value = new SearchAllWorkTimeReportModel();
            try
            {
                Pagination<SearchAllWorkTimeReport> data = new Pagination<SearchAllWorkTimeReport>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    //data = _sql.SearchAllWorkTimeReport(shareCode, searchPayRollDTO);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchAllPayRollService:");
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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TUFTManagement.Core;
using TUFTManagement.DTO;
using TUFTManagement.Models;

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

                ValidationModel validation = ValidationManager.CheckValidation(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetEmpProfile(shareCode, userID, lang);
                    value.data = data;
                    value.success = validation.Success;
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(shareCode, logID, validation.InvalidMessage);
                }

                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterService:");
                if (logID > 0)
                {
                    _sql.UpdateLogReceiveDataError(shareCode, logID, ex.ToString());
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

    }
}
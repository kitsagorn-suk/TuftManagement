﻿using System;
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

                    saveEmpProfileDTO.newUserID = _sql.insertUserLogin(saveEmpProfileDTO, userID);
                    
                    if (saveEmpProfileDTO.newUserID != 0)
                    {
                        if (saveEmpProfileDTO.positionID == 14) // ถ้าเป็นพริตตี้ เพิ่ม ข้อ 6/7/8
                        {
                            SaveBodySetRequestDTO saveBodySetRequestDTO = new SaveBodySetRequestDTO();
                            saveBodySetRequestDTO.height = saveEmpProfileDTO.height;
                            saveBodySetRequestDTO.weight = saveEmpProfileDTO.weight;
                            saveBodySetRequestDTO.chest = saveEmpProfileDTO.chest;
                            saveBodySetRequestDTO.waist = saveEmpProfileDTO.waist;
                            saveBodySetRequestDTO.hip = saveEmpProfileDTO.hip;

                            _ReturnIdModel saveBodySet = _sql.InsertBodySet(shareCode, saveBodySetRequestDTO, userID);
                            saveEmpProfileDTO.bodySetID = saveBodySet.id;
                        }

                        value.data = _sql.InsertEmpProfile(shareCode, saveEmpProfileDTO, userID);
                                    _sql.InsertEmpAddress(shareCode, saveEmpProfileDTO, userID);
                                    _sql.InsertEmpBankAccount(shareCode, saveEmpProfileDTO, userID);

                        SaveEmergencyContact emergencyContact = new SaveEmergencyContact();

                        if (saveEmpProfileDTO.emergencyContact.Count > 0)
                        {
                            foreach(SaveEmergencyContact item in saveEmpProfileDTO.emergencyContact)
                            {
                                _sql.InsertEmpEmergencyContact(shareCode, item, saveEmpProfileDTO.newUserID, userID);
                            }
                        }

                        if (saveEmpProfileDTO.positionID == 14) // ถ้าเป็นพริตตี้ เพิ่ม ข้อ 6/7/8
                        {
                            SaveEmpRateRequestDTO saveEmpRateRequestDTO = new SaveEmpRateRequestDTO();
                            saveEmpRateRequestDTO.empID = saveEmpProfileDTO.newUserID;
                            saveEmpRateRequestDTO.productCode = saveEmpProfileDTO.productCode;
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
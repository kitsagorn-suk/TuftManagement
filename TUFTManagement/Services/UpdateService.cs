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

        public ReturnIdModel UpdateEmpProfileService(string shareCode, string authorization, string lang, string platform, int logID, SaveEmpProfileDTO saveEmpProfileDTO, int userID)
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
                ValidationModel validation = ValidationManager.CheckValidationDupicateInsertEmp(shareCode, lang, saveEmpProfileDTO);

                if (validation.Success == true)
                {
                    saveEmpProfileDTO.userID = _sql.getUserIdByEmpProfileID(shareCode, saveEmpProfileDTO.empProfileID);

                    string TableName = "emp_profile";
                    saveEmpProfileDTO.empProfileID = _sql.GetIdUpdateByUserID(shareCode, TableName, saveEmpProfileDTO.userID.ToString());
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "join_date", saveEmpProfileDTO.joinDate, userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "monthly_salary", saveEmpProfileDTO.monthlySalary.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "daily_salary", saveEmpProfileDTO.dailySalary.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "department_id", saveEmpProfileDTO.departmentID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "position_id", saveEmpProfileDTO.positionID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "employment_type_id", saveEmpProfileDTO.employmentTypeID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "title_id", saveEmpProfileDTO.titleID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "firstname_th", saveEmpProfileDTO.firstNameTH, userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "lastname_th", saveEmpProfileDTO.lastNameTH, userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "nickname_th", saveEmpProfileDTO.nickNameTH.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "firstname_en", saveEmpProfileDTO.firstNameEN.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "lastname_en", saveEmpProfileDTO.lastNameEN, userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "nickname_en", saveEmpProfileDTO.nickNameEN, userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "nationality_id", saveEmpProfileDTO.nationalityID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "citizenship_id", saveEmpProfileDTO.citizenshipID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "religion_id", saveEmpProfileDTO.religionID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "date_of_birth", saveEmpProfileDTO.dateOfBirth.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "identity_card", saveEmpProfileDTO.identityCard.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "identity_card_expiry", saveEmpProfileDTO.identityCardExpiry.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "height", saveEmpProfileDTO.height.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "weight", saveEmpProfileDTO.weight.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "shirt_size_id", saveEmpProfileDTO.shirtSizeID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "blood_type_id", saveEmpProfileDTO.bloodTypeID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "phone_number", saveEmpProfileDTO.phoneNumber.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "image_profile_code", saveEmpProfileDTO.imageProfileCode.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpProfileDTO.empProfileID, TableName, "image_gallery_code", saveEmpProfileDTO.imageGalleryCode.ToString(), userID);
                    value.data = _sql.UpdateEmpProfile(shareCode, saveEmpProfileDTO, userID);

                    string TableName2 = "emp_profile_address";
                    int empAdressID = _sql.GetIdUpdateByUserID(shareCode, TableName2, saveEmpProfileDTO.userID.ToString());
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empAdressID, TableName2, "c_country_id", saveEmpProfileDTO.cCountryID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empAdressID, TableName2, "c_address", saveEmpProfileDTO.cAddress.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empAdressID, TableName2, "c_sub_district_id", saveEmpProfileDTO.cSubDistrictID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empAdressID, TableName2, "c_district_id", saveEmpProfileDTO.cDistrictID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empAdressID, TableName2, "c_province_id", saveEmpProfileDTO.cProvinceID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empAdressID, TableName2, "c_zipcode", saveEmpProfileDTO.cZipcode.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empAdressID, TableName2, "c_phone_contact", saveEmpProfileDTO.cPhoneContact, userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empAdressID, TableName2, "is_same_permanent_address", saveEmpProfileDTO.isSamePermanentAddress.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empAdressID, TableName2, "p_country_id", saveEmpProfileDTO.pCountryID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empAdressID, TableName2, "p_address", saveEmpProfileDTO.pAddress.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empAdressID, TableName2, "p_sub_district_id", saveEmpProfileDTO.pSubDistrictID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empAdressID, TableName2, "p_district_id", saveEmpProfileDTO.pDistrictID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empAdressID, TableName2, "p_province_id", saveEmpProfileDTO.pProvinceID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empAdressID, TableName2, "p_zipcode", saveEmpProfileDTO.pZipcode.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empAdressID, TableName2, "p_phone_contact", saveEmpProfileDTO.pPhoneContact.ToString(), userID);
                    _sql.UpdateEmpAddress(shareCode, saveEmpProfileDTO, userID);

                    string TableName3 = "emp_bank_account";
                    int empBankAccountID = _sql.GetIdUpdateByUserID(shareCode, TableName3, saveEmpProfileDTO.userID.ToString());
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empBankAccountID, TableName3, "bank_id", saveEmpProfileDTO.bankID.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empBankAccountID, TableName3, "account_no", saveEmpProfileDTO.bankAccountNumber.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, empBankAccountID, TableName3, "account_name", saveEmpProfileDTO.bankAccountName.ToString(), userID);
                    _sql.UpdateEmpBankAccount(shareCode, saveEmpProfileDTO, userID);
                    

                    string TableName4 = "emp_emergency_contact";
                    if (saveEmpProfileDTO.emergencyContact.Count > 0)
                    {
                        foreach(SaveEmergencyContact item in saveEmpProfileDTO.emergencyContact)
                        {
                            if (item.emergencyContactID != 0)
                            {
                                //_sql.InsertSystemLogChangeWithShareCode(shareCode, item.emergencyContactID, TableName4, "emer_full_name", item.emerFullName.ToString(), userID);
                                //_sql.InsertSystemLogChangeWithShareCode(shareCode, item.emergencyContactID, TableName4, "emer_relationship_id", item.emerRelationShipID.ToString(), userID);
                                //_sql.InsertSystemLogChangeWithShareCode(shareCode, item.emergencyContactID, TableName4, "emer_contact", item.emerContact.ToString(), userID);
                                _sql.UpdateEmpEmergencyContact(shareCode, item, userID);
                            }
                            else
                            {
                                _sql.InsertEmpEmergencyContact(shareCode, item, saveEmpProfileDTO.userID, userID);
                            }
                        }
                    }

                    if (saveEmpProfileDTO.positionID == 14) //ถ้าเป็นพริตตีถึงอัพเดตได้
                    {
                        string TableName6 = "emp_rate";
                        int empRateID = _sql.GetIdUpdateByUserID(shareCode, TableName6, saveEmpProfileDTO.userID.ToString());
                        //_sql.InsertSystemLogChangeWithShareCode(shareCode, empRateID, TableName6, "product_code", saveEmpProfileDTO.productCode.ToString(), userID);
                        //_sql.InsertSystemLogChangeWithShareCode(shareCode, empRateID, TableName6, "rate_staff", saveEmpProfileDTO.rateStaff.ToString(), userID);
                        //_sql.InsertSystemLogChangeWithShareCode(shareCode, empRateID, TableName6, "rate_manager", saveEmpProfileDTO.rateManager.ToString(), userID);
                        //_sql.InsertSystemLogChangeWithShareCode(shareCode, empRateID, TableName6, "rate_owner", saveEmpProfileDTO.rateOwner.ToString(), userID);
                        //_sql.InsertSystemLogChangeWithShareCode(shareCode, empRateID, TableName6, "rate_confirm", saveEmpProfileDTO.rateConfirm.ToString(), userID);

                        SaveEmpRateRequestDTO saveEmpRateRequestDTO = new SaveEmpRateRequestDTO();
                        saveEmpRateRequestDTO.empRateID = empRateID;
                        saveEmpRateRequestDTO.serviceNo = saveEmpProfileDTO.serviceNo;
                        saveEmpRateRequestDTO.productGrade = saveEmpProfileDTO.productGrade;
                        saveEmpRateRequestDTO.startDrink = saveEmpProfileDTO.startDrink;
                        saveEmpRateRequestDTO.fullDrink = saveEmpProfileDTO.fullDrink;
                        saveEmpRateRequestDTO.rateStaff = float.Parse(saveEmpProfileDTO.rateStaff);
                        saveEmpRateRequestDTO.rateManager = float.Parse(saveEmpProfileDTO.rateManager);
                        saveEmpRateRequestDTO.rateOwner = float.Parse(saveEmpProfileDTO.rateOwner);
                        saveEmpRateRequestDTO.rateConfirm = float.Parse(saveEmpProfileDTO.rateConfirm);
                        _sql.UpdateEmpRate(shareCode, saveEmpRateRequestDTO, userID);
                    }
                    
                }
                else
                {
                    _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateEmpProfileService:");
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

        public ReturnIdModel UpdateEmpRateService(string shareCode, string authorization, string lang, string platform, int logID, SaveEmpRateRequestDTO saveEmpRateDTO, int userID)
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
                    _sql.InsertSystemLogChange(saveEmpRateDTO.empRateID, TableName, "service_no", saveEmpRateDTO.serviceNo.ToString(), userID);
                    _sql.InsertSystemLogChange(saveEmpRateDTO.empRateID, TableName, "rate_staff", saveEmpRateDTO.rateStaff.ToString(), userID);
                    _sql.InsertSystemLogChange(saveEmpRateDTO.empRateID, TableName, "rate_manager", saveEmpRateDTO.rateManager.ToString(), userID);
                    _sql.InsertSystemLogChange(saveEmpRateDTO.empRateID, TableName, "rate_owner", saveEmpRateDTO.rateOwner.ToString(), userID);
                    _sql.InsertSystemLogChange(saveEmpRateDTO.empRateID, TableName, "rate_confirm", saveEmpRateDTO.rateConfirm.ToString(), userID);

                    value.data = _sql.UpdateEmpRate(shareCode, saveEmpRateDTO, userID);
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

        public ReturnIdModel UpdateEmpWorkShiftService(string authorization, string lang, string platform, int logID, SaveEmpWorkShiftRequestDTO saveEmpWorkShiftRequestDTO, int userID)
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
        public ReturnIdModel UpdateEmpWorkTimeService(string authorization, string lang, string platform, int logID, SaveEmpWorkTimeRequestDTO saveEmpWorkTimeRequestDTO, int userID)
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
                    //validation = ValidationManager.CheckValidationWorktime(1, lang, platform, saveEmpWorkTimeRequestDTO.empWorkTimeID);

                    if (validation.Success == true)
                    {
                        if (saveEmpWorkTimeRequestDTO.empWorkShiftID > 0)
                        {
                            string TableName = "emp_work_time";
                            _sql.InsertSystemLogChange(saveEmpWorkTimeRequestDTO.empWorkTimeID, TableName, "work_shift_id", saveEmpWorkTimeRequestDTO.empWorkShiftID.ToString(), userID);
                            value.data = _sql.UpdateEmpWorkTime(saveEmpWorkTimeRequestDTO, userID);
                        }
                        else if (!string.IsNullOrEmpty(saveEmpWorkTimeRequestDTO.workIn))
                        {
                            string TableName = "emp_work_time";
                            _sql.InsertSystemLogChange(saveEmpWorkTimeRequestDTO.empWorkTimeID, TableName, "work_in", saveEmpWorkTimeRequestDTO.workIn.ToString(), userID);
                            value.data = _sql.UpdateEmpWorkTime_WorkIn(saveEmpWorkTimeRequestDTO, userID);
                        }
                        else if (!string.IsNullOrEmpty(saveEmpWorkTimeRequestDTO.workOut))
                        {
                            string TableName = "emp_work_time";
                            _sql.InsertSystemLogChange(saveEmpWorkTimeRequestDTO.empWorkTimeID, TableName, "work_out", saveEmpWorkTimeRequestDTO.workOut.ToString(), userID);
                            value.data = _sql.UpdateEmpWorkTime_WorkOut(saveEmpWorkTimeRequestDTO, userID);
                        }
                        else if (!string.IsNullOrEmpty(saveEmpWorkTimeRequestDTO.floorIn))
                        {
                            string TableName = "emp_work_time";
                            _sql.InsertSystemLogChange(saveEmpWorkTimeRequestDTO.empWorkTimeID, TableName, "floor_in", saveEmpWorkTimeRequestDTO.floorIn.ToString(), userID);
                            value.data = _sql.UpdateEmpWorkTime_FloorIn(saveEmpWorkTimeRequestDTO, userID);
                        }
                        else if (!string.IsNullOrEmpty(saveEmpWorkTimeRequestDTO.floorOut))
                        {
                            string TableName = "emp_work_time";
                            _sql.InsertSystemLogChange(saveEmpWorkTimeRequestDTO.empWorkTimeID, TableName, "floor_out", saveEmpWorkTimeRequestDTO.floorOut.ToString(), userID);
                            value.data = _sql.UpdateEmpWorkTime_FloorOut(saveEmpWorkTimeRequestDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateEmpWorkTimeService:");
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

        public ReturnIdModel ApproveWorkTimeTransChangeService(string authorization, string lang, string platform, int logID, SaveWorkTimeTransChangeRequestDTO transChangeRequest, int userID)
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
                    validation = ValidationManager.CheckValidationTransChange(lang, platform, transChangeRequest.transChangeID);

                    if (validation.Success == true)
                    {
                        string TableName = "tran_change_work_shift";
                        _sql.InsertSystemLogChange(transChangeRequest.transChangeID, TableName, "status_approve", transChangeRequest.statusApprove.ToString(), userID);
                        _sql.InsertSystemLogChange(transChangeRequest.transChangeID, TableName, "remark", transChangeRequest.remark.ToString(), userID);
                        value.data = _sql.ApproveTransChange_WorkShift(transChangeRequest, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "ApproveWorkTimeTransChangeService:");
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

        public ReturnIdModel UpdateEmpStatusService(string shareCode, string authorization, string lang, string platform, int logID, SaveEmpStatusDTO saveEmpStatusDTO, int userID)
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
                    string TableName = "emp_profile";
                    //_sql.InsertSystemLogChange(saveEmpStatusDTO.empID, TableName, "status", saveEmpStatusDTO.status.ToString(), userID);

                    value.data = _sql.UpdateEmpStatus(shareCode, saveEmpStatusDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateEmpStatusService:");
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

        public ReturnIdModel UpdateLeaveDetailService(string authorization, string lang, string platform, int logID, SaveLeaveDetailDTO saveLeaveDetailDTO, int userID,string shareCode)
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
                    string TableName = "emp_leave";
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveLeaveDetailDTO.leaveId, TableName, "leave_type_id", saveLeaveDetailDTO.leavetypeId.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveLeaveDetailDTO.leaveId, TableName, "start_date", saveLeaveDetailDTO.startdate, userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveLeaveDetailDTO.leaveId, TableName, "end_date", saveLeaveDetailDTO.enddate, userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveLeaveDetailDTO.leaveId, TableName, "numbers_of_days", saveLeaveDetailDTO.numdays.ToString(), userID);
                    //_sql.InsertSystemLogChangeWithShareCode(shareCode, saveLeaveDetailDTO.leaveId, TableName, "leave_reason00", saveLeaveDetailDTO.leavereason.ToString(), userID);

                    value.data = _sql.UpdateLeaveDetail(saveLeaveDetailDTO, userID, shareCode);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateLeaveDetailService:");
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

        public ReturnIdModel CancelLeaveFormService(string authorization, string lang, string platform, int logID, ActionLeaveFormDTO actionLeaveFormDTO, int userID,string shareCode)
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
                //MailModel mail = new MailModel();

               
                if (validation.Success == true)
                {
                    value.data = _sql.CancelLeaveForm(actionLeaveFormDTO.leaveID, userID, actionLeaveFormDTO.cancelReason, shareCode);

                    //MailService srv = new MailService();
                    //mail = srv.SendMailLeave(lang, logID, value.data.id, 4);
                    //value.mail = mail;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "CancelLeaveFormService:");
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

        public ReturnIdModel RejectLeaveFormService(string authorization, string lang, string platform, int logID, ActionLeaveFormDTO actionLeaveFormDTO, int userID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                //MailModel mail = new MailModel();
               
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    value.data = _sql.RejectLeaveForm(actionLeaveFormDTO.leaveID, userID, actionLeaveFormDTO.rejectReason, shareCode);

                    //MailService srv = new MailService();
                    //if (_sql.CheckleaveIsEdit(value.data.id))
                    //{
                    //    mail = srv.SendMailLeave(lang, logID, value.data.id, 5);
                    //}
                    //else
                    //{
                    //    mail = srv.SendMailLeave(lang, logID, value.data.id, 2);
                    //}
                    //value.mail = mail;
                //}
                //else
                //{
                //    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "RejectLeaveFormService:");
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

        public ReturnIdModel ApproveLeaveFormService(string authorization, string lang, string platform, int logID, ActionLeaveFormDTO actionLeaveFormDTO, int remainDay, int userID,string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            //MailModel mail = new MailModel();
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    value.data = _sql.ApproveLeaveForm(actionLeaveFormDTO.leaveID, remainDay, userID, shareCode);

                    //MailService srv = new MailService();
                    //if (_sql.CheckleaveIsEdit(value.data.id))
                    //{
                    //    mail = srv.SendMailLeave(lang, logID, value.data.id, 5);
                    //}
                    //else
                    //{
                    //    mail = srv.SendMailLeave(lang, logID, value.data.id, 2);
                    //}
                    //value.mail = mail;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "ApproveLeaveFormService:");
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

        public ReturnIdModel UpdateEmpWorkTimeNewVerService(string authorization, string lang, string platform, int logID, SaveEmpWorkTimeRequestDTO saveEmpWorkTimeRequestDTO, int userID, string shareCode)
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
                    validation = ValidationManager.CheckValidationWorktime(1, lang, platform, saveEmpWorkTimeRequestDTO.empWorkTimeID,shareCode);

                    if (validation.Success == true)
                    {
                        if (saveEmpWorkTimeRequestDTO.empWorkShiftID > 0)
                        {
                            string TableName = "emp_work_time";
                            _sql.InsertSystemLogChangeWithShareCode(shareCode, saveEmpWorkTimeRequestDTO.empWorkTimeID, TableName, "work_shift_id", saveEmpWorkTimeRequestDTO.empWorkShiftID.ToString(), userID);
                            value.data = _sql.InsertEmpWorkTimeTran(saveEmpWorkTimeRequestDTO, userID, shareCode);
                            //value.data = _sql.UpdateEmpWorkTimeNewVer(saveEmpWorkTimeRequestDTO, userID, shareCode);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateEmpWorkTimeService:");
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


        #region SystemRole
        public ReturnIdModel UpdateSystemRoleTempService(string authorization, string lang, string platform, int logID, SaveSystemRoleTempDTO saveSystemRoleTempDTO, int userID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidationDupicateInsertSystemRole(lang, saveSystemRoleTempDTO);
                if (validation.Success == true)
                {
                    value.data = _sql.UpdateSystemRoleTemp(shareCode, saveSystemRoleTempDTO, userID, platform);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateSystemRoleTemplateService:");
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

        public ReturnIdModel UpdateActiveSystemRoleTempService(string authorization, string lang, string platform, int logID, SaveSystemRoleTempDTO saveSystemRoleTempDTO, int userID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidationDupicateInsertSystemRole(lang, saveSystemRoleTempDTO);
                if (validation.Success == true)
                {
                    value.data = _sql.UpdateActiveSystemRoleTemp(shareCode, platform, saveSystemRoleTempDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateSystemRoleAssignmentService:");
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


        public ReturnIdModel UpdateSystemRoleAssignService(string authorization, string lang, string platform, int logID, SaveSystemRoleAssignDTO saveSystemRoleAssignDTO, int userID, string shareCode)
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
                    value.data = _sql.UpdateSystemRoleAssign(shareCode, platform, saveSystemRoleAssignDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateSystemRoleAssignmentService:");
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

        public ReturnIdModel UpdateActiveSystemRoleAssignService(string authorization, string lang, string platform, int logID, SaveSystemRoleAssignDTO saveSystemRoleAssignDTO, int userID, string shareCode)
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
                    value.data = _sql.UpdateActiveSystemRoleAssign(shareCode, platform, saveSystemRoleAssignDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateSystemRoleAssignmentService:");
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
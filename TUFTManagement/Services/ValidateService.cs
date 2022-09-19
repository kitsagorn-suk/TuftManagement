﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TUFTManagement.Core;
using TUFTManagement.DTO;
using TUFTManagement.Models;

namespace TUFTManagement.Services
{
    public class ValidateService
    {
        private SQLManager _sql = SQLManager.Instance;

        public ValidationModel RequireOptionalBodySet(string shareCode, string lang, string platform, int logID, SaveBodySetRequestDTO saveBodySetDTO)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ValidationModel validation = new ValidationModel();

            try
            {
                string checkMissingOptional = "";

                if (string.IsNullOrEmpty(saveBodySetDTO.mode))
                {
                    throw new Exception("Missing Parameter : mode ");
                }
                else if(saveBodySetDTO.mode.ToLower().Equals("delete"))
                {
                    if (saveBodySetDTO.masterID == 0)
                    {
                        checkMissingOptional += "id ";
                    }
                }
                else if(saveBodySetDTO.mode.ToLower().Equals("insert") || saveBodySetDTO.mode.ToLower().Equals("update"))
                {
                    if (saveBodySetDTO.mode.ToLower().Equals("insert") && saveBodySetDTO.masterID != 0)
                    {
                        checkMissingOptional += "id ";
                    }
                    if (saveBodySetDTO.mode.ToLower().Equals("update") && saveBodySetDTO.masterID == 0)
                    {
                        checkMissingOptional += "id ";
                    }
                    if (string.IsNullOrEmpty(saveBodySetDTO.height.ToString()))
                    {
                        checkMissingOptional += checkMissingOptional + "height ";
                    }
                    if (string.IsNullOrEmpty(saveBodySetDTO.weight.ToString()))
                    {
                        checkMissingOptional += checkMissingOptional + "weight ";
                    }
                    if (string.IsNullOrEmpty(saveBodySetDTO.chest.ToString()))
                    {
                        checkMissingOptional += checkMissingOptional + "chest ";
                    }
                    if (string.IsNullOrEmpty(saveBodySetDTO.waist.ToString()))
                    {
                        checkMissingOptional += checkMissingOptional + "waist ";
                    }
                    if (string.IsNullOrEmpty(saveBodySetDTO.hip.ToString()))
                    {
                        checkMissingOptional += checkMissingOptional + "hip ";
                    }
                }
                else
                {
                    throw new Exception("Not found this Mode");
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {
                    validation = ValidationManager.CheckValidationWithShareCode(shareCode, saveBodySetDTO.masterID, lang, platform);
                }
                
            }
            catch (Exception ex)
            {
                //LogManager.ServiceLog.WriteExceptionLog(ex, "RequireOptionalBodySet:");
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
            return validation;
        }
        public ValidationModel RequireOptionalAllDropdown(string lang, string platform, int logID, 
            GetDropdownRequestDTO getDropdownRequestDTO)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ValidationModel validation = new ValidationModel();

            try
            {
                if (string.IsNullOrEmpty(getDropdownRequestDTO.moduleName))
                {
                    throw new Exception("Missing Parameter : moduleName ");
                }
                else if (getDropdownRequestDTO.moduleName.ToLower() == "district".ToLower() &&
                         getDropdownRequestDTO.provinceID.Equals(0) || getDropdownRequestDTO.provinceID.Equals(null))
                {
                    throw new Exception("Missing Parameter : provinceID ");
                }
                else if (getDropdownRequestDTO.moduleName.ToLower() == "subDistrict".ToLower() &&
                         getDropdownRequestDTO.districtID.Equals(0) || getDropdownRequestDTO.districtID.Equals(null))
                {
                    throw new Exception("Missing Parameter : districtID ");
                }
                else
                {
                    validation = ValidationManager.CheckValidation(0, lang, platform);
                }

                return validation;

            }
            catch (Exception ex)
            {
                //LogManager.ServiceLog.WriteExceptionLog(ex, "RequireOptionalAllDropdown:");
                if (logID > 0)
                {
                    _sql.UpdateLogReceiveDataError(logID, ex.ToString());
                }
                throw ex;
            }
        }
        public ValidationModel RequireOptionalSaveEmpProfile(string shareCode, string lang, string platform, int logID, SaveEmpProfileDTO saveEmpProfileDTO)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ValidationModel validation = new ValidationModel();

            try
            {
                string checkMissingOptional = "";

                if (string.IsNullOrEmpty(saveEmpProfileDTO.userName) && saveEmpProfileDTO.empProfileID.Equals(0))
                {
                    checkMissingOptional += "userName ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.shareCode))
                {
                    checkMissingOptional += "shareCode ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.joinDate))
                {
                    checkMissingOptional += "joinDate ";
                }
                if (saveEmpProfileDTO.employmentTypeID.Equals(0))
                {
                    checkMissingOptional += "employmentTypeID ";
                }

                if (!shareCode.ToLower().Equals("lls")) // ถ้าไม่ใช่ ลาลิสา
                {
                    if (saveEmpProfileDTO.monthlySalary.Equals(0) && saveEmpProfileDTO.employmentTypeID != 2) // ถ้าเป็นพริตตี้ รายวัน จะไม่เช็ค
                    {
                        checkMissingOptional += "monthlySalary ";
                    }
                    if (saveEmpProfileDTO.dailySalary.Equals(0) && saveEmpProfileDTO.employmentTypeID != 1) // ถ้าเป็นพริตตี้ รายเดือน จะไม่เช็ค
                    {
                        checkMissingOptional += "dailySalary ";
                    }
                }
                
                if (saveEmpProfileDTO.departmentID.Equals(0))
                {
                    checkMissingOptional += "departmentID ";
                }
                if (saveEmpProfileDTO.positionID.Equals(0))
                {
                    checkMissingOptional += "positionID ";
                }
                
                if (saveEmpProfileDTO.titleID.Equals(0))
                {
                    checkMissingOptional += "titleID ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.firstNameTH))
                {
                    checkMissingOptional += "firstNameTH ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.lastNameTH))
                {
                    checkMissingOptional += "lastNameTH ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.nickNameTH))
                {
                    checkMissingOptional += "nickNameTH ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.firstNameEN))
                {
                    checkMissingOptional += "firstNameEN ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.lastNameEN))
                {
                    checkMissingOptional += "lastNameEN ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.nickNameEN))
                {
                    checkMissingOptional += "nickNameEN ";
                }
                if (saveEmpProfileDTO.nationalityID.Equals(0))
                {
                    checkMissingOptional += "nationalityID ";
                }
                if (saveEmpProfileDTO.citizenshipID.Equals(0))
                {
                    checkMissingOptional += "citizenshipID ";
                }
                if (saveEmpProfileDTO.religionID.Equals(0))
                {
                    checkMissingOptional += "religionID ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.dateOfBirth))
                {
                    checkMissingOptional += "dateOfBirth ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.identityCard) && saveEmpProfileDTO.citizenshipID == 20) //Thai
                {
                    checkMissingOptional += "identityCard ";
                }
                if (saveEmpProfileDTO.identityCard.Count() != 13)
                {
                    checkMissingOptional += "identityCard is incomplete ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.identityCardExpiry))
                {
                    checkMissingOptional += "identityCardExpiry ";
                }
                if (saveEmpProfileDTO.height.Equals(0))
                {
                    checkMissingOptional += "height ";
                }
                if (saveEmpProfileDTO.weight.Equals(0))
                {
                    checkMissingOptional += "weight ";
                }
                if (saveEmpProfileDTO.shirtSizeID.Equals(0))
                {
                    checkMissingOptional += "shirtSize ";
                }
                if (saveEmpProfileDTO.bloodTypeID.Equals(0))
                {
                    checkMissingOptional += "bloodTypeID ";
                }
                if (string.IsNullOrEmpty(saveEmpProfileDTO.phoneNumber))
                {
                    checkMissingOptional += "phoneNumber ";
                }

                if (saveEmpProfileDTO.employmentTypeID != 2) // ถ้าเป็น 2 คือพนักงานพาร์ททาม ไม่ต้องเช็คข้อ 2 3 4 นี้
                {
                    if (saveEmpProfileDTO.pCountryID.Equals(0) && saveEmpProfileDTO.citizenshipID != 20) //ต่างชาติไม่ต้องกรอกก็ได้
                    {
                        checkMissingOptional += "pCountryID ";
                    }
                    if (saveEmpProfileDTO.pCountryID == 20) //Thailand
                    {
                        if (saveEmpProfileDTO.pSubDistrictID.Equals(0))
                        {
                            checkMissingOptional += "pSubDistrictID ";
                        }
                        if (saveEmpProfileDTO.pDistrictID.Equals(0))
                        {
                            checkMissingOptional += "pDistrictID ";
                        }
                        if (saveEmpProfileDTO.pProvinceID.Equals(0))
                        {
                            checkMissingOptional += "pProvinceID ";
                        }
                        if (string.IsNullOrEmpty(saveEmpProfileDTO.pZipcode))
                        {
                            checkMissingOptional += "pZipcode ";
                        }
                    }

                    if (string.IsNullOrEmpty(saveEmpProfileDTO.pPhoneContact) && saveEmpProfileDTO.citizenshipID != 20) //ต่างชาติไม่ต้องกรอกก็ได้
                    {
                        checkMissingOptional += "pPhoneContact ";
                    }
                    if (string.IsNullOrEmpty(saveEmpProfileDTO.pAddress) && saveEmpProfileDTO.citizenshipID != 20) //ต่างชาติไม่ต้องกรอกก็ได้
                    {
                        checkMissingOptional += "pAddress ";
                    }

                    if (saveEmpProfileDTO.isSamePermanentAddress.Equals(0))
                    {
                        if (saveEmpProfileDTO.citizenshipID == 20) //Thai
                        {
                            if (saveEmpProfileDTO.cSubDistrictID.Equals(0))
                            {
                                checkMissingOptional += "cSubDistrictID ";
                            }
                            if (saveEmpProfileDTO.cDistrictID.Equals(0))
                            {
                                checkMissingOptional += "cDistrictID ";
                            }
                            if (saveEmpProfileDTO.cProvinceID.Equals(0))
                            {
                                checkMissingOptional += "cProvinceID ";
                            }
                            if (string.IsNullOrEmpty(saveEmpProfileDTO.cZipcode))
                            {
                                checkMissingOptional += "cZipcode ";
                            }
                        }
                        if (string.IsNullOrEmpty(saveEmpProfileDTO.cPhoneContact))
                        {
                            checkMissingOptional += "cPhoneContact ";
                        }
                        if (string.IsNullOrEmpty(saveEmpProfileDTO.cAddress))
                        {
                            checkMissingOptional += "cAddress ";
                        }
                    }
                    else
                    {
                        // ถ้าติ๊ก ที่อยู่ตรงกับทะเบียนบ้าน
                        saveEmpProfileDTO.cCountryID = saveEmpProfileDTO.pCountryID;
                        saveEmpProfileDTO.cAddress = saveEmpProfileDTO.pAddress;
                        saveEmpProfileDTO.cProvinceID = saveEmpProfileDTO.pProvinceID;
                        saveEmpProfileDTO.cDistrictID = saveEmpProfileDTO.pDistrictID;
                        saveEmpProfileDTO.cSubDistrictID = saveEmpProfileDTO.pSubDistrictID;
                        saveEmpProfileDTO.cZipcode = saveEmpProfileDTO.pZipcode;
                        saveEmpProfileDTO.cPhoneContact = saveEmpProfileDTO.pPhoneContact;
                    }

                    if (string.IsNullOrEmpty(saveEmpProfileDTO.bankAccountName))
                    {
                        checkMissingOptional += "bankAccountName ";
                    }
                    if (string.IsNullOrEmpty(saveEmpProfileDTO.bankAccountNumber))
                    {
                        checkMissingOptional += "bankAccountNumber ";
                    }
                    if (saveEmpProfileDTO.bankID.Equals(0))
                    {
                        checkMissingOptional += "bankID ";
                    }
                }

                if (saveEmpProfileDTO.emergencyContact != null)
                {
                    if (saveEmpProfileDTO.emergencyContact.Count > 0)
                    {
                        if (saveEmpProfileDTO.emergencyContact.Count <= 5)
                        {
                            foreach (SaveEmergencyContact item in saveEmpProfileDTO.emergencyContact)
                            {
                                int isDupName = _sql.CheckDupEmergencyName(shareCode, item.emerFullName, item.emerContactID);
                                if (isDupName > 0)
                                {
                                    throw new Exception("emergency fullname : " + item.emerFullName + " is already");
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(item.emerFullName))
                                    {
                                        checkMissingOptional += "emerFullName ";
                                    }
                                    if (item.emerRelationShipID.Equals(0))
                                    {
                                        checkMissingOptional += "emerRelationShipID ";
                                    }
                                    if (string.IsNullOrEmpty(item.emerContact))
                                    {
                                        checkMissingOptional += "emerContact ";
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("emergency contact limit 5 record");
                        }

                    }
                }
                

                if (saveEmpProfileDTO.positionID == 14) //ถ้าเป็นพริตตี้ ต้องกรอก
                {
                    if (saveEmpProfileDTO.chest.Equals(0))
                    {
                        checkMissingOptional += "chest ";
                    }
                    if (saveEmpProfileDTO.waist.Equals(0))
                    {
                        checkMissingOptional += "waist ";
                    }
                    if (saveEmpProfileDTO.hip.Equals(0))
                    {
                        checkMissingOptional += "hip ";
                    }
                    if (saveEmpProfileDTO.serviceNo.Equals(0))
                    {
                        checkMissingOptional += "serviceNo ";
                    }
                    if (saveEmpProfileDTO.productGrade.Equals(0))
                    {
                        checkMissingOptional += "productGrade ";
                    }
                    if (saveEmpProfileDTO.rateStaff.Equals(0))
                    {
                        checkMissingOptional += "rateStaff ";
                    }
                    if (saveEmpProfileDTO.rateManager.Equals(0))
                    {
                        checkMissingOptional += "rateManager ";
                    }
                    if (saveEmpProfileDTO.rateOwner.Equals(0))
                    {
                        checkMissingOptional += "rateOwner ";
                    }
                    if (saveEmpProfileDTO.rateConfirm.Equals(0))
                    {
                        checkMissingOptional += "rateConfirm ";
                    }
                }
                
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {
                    validation = ValidationManager.CheckValidationWithShareCode(shareCode, 0, lang, platform);
                }

                return validation;

            }
            catch (Exception ex)
            {
                //LogManager.ServiceLog.WriteExceptionLog(ex, "RequireOptionalAllDropdown:");
                if (logID > 0)
                {
                    _sql.UpdateLogReceiveDataError(logID, ex.ToString());
                }
                throw ex;
            }
        }

        public ValidationModel RequireOptionalSaveEmpWorkShift(string shareCode, string lang, string platform, int logID, SaveEmpWorkShiftRequestDTO saveEmpWorkShiftRequestDTO)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ValidationModel validation = new ValidationModel();

            try
            {
                string checkMissingOptional = "";

                if (string.IsNullOrEmpty(saveEmpWorkShiftRequestDTO.wsCode))
                {
                    checkMissingOptional += "wsCode ";
                }
                if (string.IsNullOrEmpty(saveEmpWorkShiftRequestDTO.timeStart))
                {
                    checkMissingOptional += "timeStart ";
                }
                if (string.IsNullOrEmpty(saveEmpWorkShiftRequestDTO.timeEnd))
                {
                    checkMissingOptional += "timeEnd ";
                }
                if (saveEmpWorkShiftRequestDTO.workTypeID.Equals(0) || saveEmpWorkShiftRequestDTO.workTypeID.Equals(null))
                {
                    checkMissingOptional += "workTypeID ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {
                    validation = ValidationManager.CheckValidationWithShareCode(shareCode, 0, lang, platform);
                }

                return validation;

            }
            catch (Exception ex)
            {
                //LogManager.ServiceLog.WriteExceptionLog(ex, "RequireOptionalAllDropdown:");
                if (logID > 0)
                {
                    _sql.UpdateLogReceiveDataError(logID, ex.ToString());
                }
                throw ex;
            }
        }

        public ValidationModel RequireOptionalSaveLeaveDetail(string shareCode, string lang, string platform, int logID, SaveLeaveDetailDTO saveLeaveDetailDTO)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ValidationModel validation = new ValidationModel();

            try
            {
                string checkMissingOptional = "";

                if (string.IsNullOrEmpty(saveLeaveDetailDTO.leaveId.ToString()))
                {
                    checkMissingOptional += "leaveID ";
                }
                if (string.IsNullOrEmpty(saveLeaveDetailDTO.leavetypeId.ToString()))
                {
                    checkMissingOptional += "leaveTypeID ";
                }
                if (string.IsNullOrEmpty(saveLeaveDetailDTO.startdate))
                {
                    checkMissingOptional += "startDate ";
                }
                if (string.IsNullOrEmpty(saveLeaveDetailDTO.enddate))
                {
                    checkMissingOptional += "endDate ";
                }
                if (string.IsNullOrEmpty(saveLeaveDetailDTO.leavereason))
                {
                    checkMissingOptional += "leaveReason ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {
                    validation = ValidationManager.CheckValidationWithShareCode(shareCode, 0, lang, platform);
                }

                return validation;

            }
            catch (Exception ex)
            {
                //LogManager.ServiceLog.WriteExceptionLog(ex, "RequireOptionalAllDropdown:");
                if (logID > 0)
                {
                    _sql.UpdateLogReceiveDataError(logID, ex.ToString());
                }
                throw ex;
            }
        }

        public ValidationModel RequireOptionalSaveSystemRoleAssign(string shareCode, string lang, string platform, int logID, SaveSystemRoleAssignDTO saveSystemRoleAssignDTO)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ValidationModel validation = new ValidationModel();

            try
            {
                string checkMissingOptional = "";
                
                if (saveSystemRoleAssignDTO.positionID  == 0)
                {
                    checkMissingOptional += "positionID ";
                }
                
                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {
                    validation = ValidationManager.CheckValidationWithShareCode(shareCode, 0, lang, platform);
                }

                return validation;

            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "RequireOptionalSaveSystemRoleAssign:");
                if (logID > 0)
                {
                    _sql.UpdateLogReceiveDataError(logID, ex.ToString());
                }
                throw ex;
            }
        }

        public ValidationModel RequireOptionalSaveSystemRoleTemp(string shareCode, string lang, string platform, int logID, SaveSystemRoleTempDTO saveSystemRoleTempDTO)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ValidationModel validation = new ValidationModel();

            try
            {
                string checkMissingOptional = "";

                if (saveSystemRoleTempDTO.objectID == 0)
                {
                    checkMissingOptional += "objID ";
                }

                if (checkMissingOptional != "")
                {
                    throw new Exception("Missing Parameter : " + checkMissingOptional);
                }
                else
                {
                    validation = ValidationManager.CheckValidationWithShareCode(shareCode, 0, lang, platform);
                }

                return validation;

            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "RequireOptionalSaveSystemRoleTemp:");
                if (logID > 0)
                {
                    _sql.UpdateLogReceiveDataError(logID, ex.ToString());
                }
                throw ex;
            }
        }

    }
}
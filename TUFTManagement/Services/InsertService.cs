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

        public InsertLoginModel InsertEmpProfileService(string authorization, string lang, string platform, int logID,
            SaveEmpProfileDTO saveEmpProfileDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            InsertLoginModel value = new InsertLoginModel();
            try
            {
                value.data = new InsertLogin();
                DataTable dt = _sql.CheckDupicateInsertEmp(saveEmpProfileDTO, 0);
                ValidationModel validationDup = ValidationManager.CheckValidationDupicateInsertEmp(lang, saveEmpProfileDTO); 
                //รอเรื่องสิทธิ์
                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100301001");
                //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform); 
                
                if (validation.Success == true)
                {
                    value.data = _sql.InsertEmpProfile(saveEmpProfileDTO, userID);
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
        #endregion
    }
}
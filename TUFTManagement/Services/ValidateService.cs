using System;
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
                    validation = ValidationManager.CheckValidation(shareCode, saveBodySetDTO.masterID, lang, platform);
                }
                
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "RequireOptionalBodySet:");
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
            return validation;
        }

    }
}
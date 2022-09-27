using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TUFTManagement.Core;
using TUFTManagement.DTO;
using TUFTManagement.Models;

namespace TUFTManagement.Services
{
    public class MasterDataService
    {
        private SQLManager _sql = SQLManager.Instance;

        public ReturnIdModel SaveMasterService(string authorization, string lang, string platform, int logID, MasterDataDTO masterDataDTO, 
            string TableName, int userID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(shareCode, lang, TableName, masterDataDTO);
                if (validation.Success == true)
                {
                    if (masterDataDTO.mode.ToLower() == "insert")
                    {
                        //List<string> listobjectID = new List<string>();
                        //listobjectID.Add("100401001");
                        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                        validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);
                        value.data = _sql.InsertMasterData(shareCode, masterDataDTO, TableName, userID);
                    }
                    else if (masterDataDTO.mode.ToLower() == "update")
                    {
                        //ValidationModel validation = new ValidationModel();
                        //List<string> listobjectID = new List<string>();
                        //listobjectID.Add("100401002");
                        //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                        validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);
                        if (validation.Success == true)
                        {
                            _sql.InsertSystemLogChangeWithShareCode(shareCode, masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
                            _sql.InsertSystemLogChangeWithShareCode(shareCode, masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
                            value.data = _sql.UpdateMasterData(shareCode, masterDataDTO, TableName, userID);
                        }
                        else
                        {
                            _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, validation.InvalidMessage);
                        }
                    }
                    else if (masterDataDTO.mode.ToLower() == "delete")
                    {
                        //ValidationModel validation = new ValidationModel();
                        //List<string> listobjectID = new List<string>();
                        //listobjectID.Add("100401002");
                        //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                        validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);
                        if (validation.Success == true)
                        {
                            value.data = _sql.CancelSystemDepartment(masterDataDTO, userID);
                            //value.data = _sql.DeleteMasterData(shareCode, masterDataDTO, TableName, userID);
                        }
                        else
                        {
                            _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, validation.InvalidMessage);
                        }
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "SaveMasterService:");
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

        public GetMasterDataModel GetMasterService(string authorization, string lang, string platform, int logID, int masterID, 
            string TableName, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetMasterDataModel value = new GetMasterDataModel();
            try
            {
                MasterData data = new MasterData();

                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetMasterData(shareCode, masterID, TableName);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterService:");
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

        public SearchMasterDataModel SearchMasterService(string authorization, string lang, string platform, int logID, 
            SearchMasterDataDTO searchMasterDataDTO, string TableName, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchMasterDataModel value = new SearchMasterDataModel();
            try
            {
                Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchMaster(shareCode, searchMasterDataDTO, TableName);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterService:");
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

        public SearchMasterDataBodySetModel SearchMasterBodySetService(string authorization, string lang, string platform, 
            int logID, SearchMasterDataDTO searchMasterDataDTO, string TableName, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchMasterDataBodySetModel value = new SearchMasterDataBodySetModel();
            try
            {
                Pagination<SearchMasterDataBodySet> data = new Pagination<SearchMasterDataBodySet>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchMasterBodySet(shareCode, searchMasterDataDTO);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterBodySetService:");
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

        public ReturnIdModel UpdateActiveMasterService(string authorization, string lang, string platform, int logID, MasterDataDTO masterDataDTO, string TableName, int userID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(shareCode, lang, TableName, masterDataDTO);
                if (validation.Success == true)
                {
                    //ValidationModel validation = new ValidationModel();
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401002");
                    //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        _sql.InsertSystemLogChange(shareCode, masterDataDTO.masterID, TableName, "is_active", masterDataDTO.IsActive, userID);
                       // _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
                        value.data = _sql.UpdateActiveMaster(shareCode, masterDataDTO,TableName, userID); ;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateActiveMasterService:");
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

        public ReturnIdModel UpdateActiveSystemPositionService(string authorization, string lang, string platform, int logID, MasterDataDTO masterDataDTO, int userID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(shareCode, lang, "system_position", masterDataDTO);
                if (validation.Success == true)
                {
                    //ValidationModel validation = new ValidationModel();
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401002");
                    //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        _sql.InsertSystemLogChange(shareCode, masterDataDTO.masterID, "system_position", "is_active", masterDataDTO.IsActive, userID);
                        // _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
                        value.data = _sql.CancelSystemPosition(masterDataDTO, userID); ;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateActiveMasterService:");
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

        public ReturnIdModel UpdateActiveSystemDepartmentService(string authorization, string lang, string platform, int logID, MasterDataDTO masterDataDTO, int userID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(shareCode, lang, "system_departrment", masterDataDTO);
                if (validation.Success == true)
                {
                    //ValidationModel validation = new ValidationModel();
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401002");
                    //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        _sql.InsertSystemLogChange(shareCode, masterDataDTO.masterID, "system_departrment", "is_active", masterDataDTO.IsActive, userID);
                        // _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
                        value.data = _sql.CancelSystemDepartment(masterDataDTO, userID); ;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateActiveMasterService:");
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


        public SearchMasterDataDepartmentModel SearchMasterDepartmentService(string authorization, string lang, string platform, int logID, SearchMasterDataDTO searchMasterDataDTO, string TableName, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchMasterDataDepartmentModel value = new SearchMasterDataDepartmentModel();
            try
            {
                Pagination<SearchMasterDataDepartment> data = new Pagination<SearchMasterDataDepartment>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchMasterDepartment(searchMasterDataDTO);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterDepartmentService:");
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

        public GetMasterPositionModel GetPositionService(string authorization, string lang, string platform, int logID, int masterID,string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetMasterPositionModel value = new GetMasterPositionModel();
            try
            {
                MasterPosition data = new MasterPosition();

                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetMasterPosition(masterID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterPositionService:");
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

        public ReturnIdModel SaveMasterKeyService(string authorization, string lang, string platform, int logID, MasterDataDTO masterDataDTO, string TableName, int userID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterKey(shareCode, lang, TableName, masterDataDTO);
                if (validation.Success == true)
                {
                    if (masterDataDTO.mode.ToLower() == "insert")
                    {
                        //List<string> listobjectID = new List<string>();
                        //listobjectID.Add("100401001");
                        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                        validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);
                        value.data = _sql.InsertMasterKey(shareCode, masterDataDTO, TableName, userID);
                    }
                    else if (masterDataDTO.mode.ToLower() == "update")
                    {
                        //ValidationModel validation = new ValidationModel();
                        //List<string> listobjectID = new List<string>();
                        //listobjectID.Add("100401002");
                        //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                        validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);
                        if (validation.Success == true)
                        {
                            _sql.InsertSystemLogChangeWithShareCode(shareCode, masterDataDTO.masterID, TableName, "key_name", masterDataDTO.keyName, userID);
                            //_sql.InsertSystemLogChangeWithShareCode(shareCode, masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
                            value.data = _sql.UpdateMasterKey(shareCode, masterDataDTO, TableName, userID);
                        }
                        else
                        {
                            _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, validation.InvalidMessage);
                        }
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "SaveMasterKeyService:");
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

        public SearchMasterKeyModel SearchMasterKeyService(string authorization, string lang, string platform, int logID, SearchMasterDataDTO searchMasterDataDTO, string TableName, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchMasterKeyModel value = new SearchMasterKeyModel();
            try
            {
                Pagination<SearchMasterKey> data = new Pagination<SearchMasterKey>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchMasterKey(searchMasterDataDTO);
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

        public GetMasterKeyModel GetMasterKeyService(string authorization, string lang, string platform, int logID, int masterID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetMasterKeyModel value = new GetMasterKeyModel();
            try
            {
                MasterKey data = new MasterKey();

                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetMasterKey(masterID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterService:");
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

        public ReturnIdModel SaveSystemMasterService(string authorization, string lang, string platform, int logID, SystemMasterDTO systemMasterDTO, string TableName, int userID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                MasterDataDTO masterDataDTO = new MasterDataDTO();
                masterDataDTO.masterID = systemMasterDTO.masterID;
                masterDataDTO.nameEN = systemMasterDTO.nameEN;
                masterDataDTO.nameTH = systemMasterDTO.nameTH;


                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(shareCode, lang, TableName, masterDataDTO);
                if (validation.Success == true)
                {
                    if (systemMasterDTO.mode.ToLower() == "insert")
                    {
                        //List<string> listobjectID = new List<string>();
                        //listobjectID.Add("100401001");
                        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                        validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);
                        value.data = _sql.InsertSystemMaster(shareCode, systemMasterDTO, TableName, userID);
                    }
                    else if (systemMasterDTO.mode.ToLower() == "update")
                    {
                        //ValidationModel validation = new ValidationModel();
                        //List<string> listobjectID = new List<string>();
                        //listobjectID.Add("100401002");
                        //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                        validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);
                        if (validation.Success == true)
                        {
                            _sql.InsertSystemLogChangeWithShareCode(shareCode, systemMasterDTO.masterID, TableName, "key_id", systemMasterDTO.keyID.ToString(), userID);
                            _sql.InsertSystemLogChangeWithShareCode(shareCode, systemMasterDTO.masterID, TableName, "value", systemMasterDTO.value.ToString(), userID);
                            //_sql.InsertSystemLogChangeWithShareCode(shareCode, systemMasterDTO.masterID, TableName, "name_en", systemMasterDTO.nameEN, userID);
                            _sql.InsertSystemLogChangeWithShareCode(shareCode, systemMasterDTO.masterID, TableName, "name_th", systemMasterDTO.nameTH, userID);
                            _sql.InsertSystemLogChangeWithShareCode(shareCode, systemMasterDTO.masterID, TableName, "order", systemMasterDTO.order.ToString(), userID);


                            value.data = _sql.UpdateSystemMaster(shareCode, systemMasterDTO, TableName, userID);
                        }
                        else
                        {
                            _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, validation.InvalidMessage);
                        }
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "SaveMasterKeyService:");
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

        public SearchSystemMasterModel SearchSystemMasterService(string authorization, string lang, string platform, int logID, SearchSystemMasterDTO searchSystemMasterDTO, string TableName, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchSystemMasterModel value = new SearchSystemMasterModel();
            try
            {
                Pagination<SearchSystemMaster> data = new Pagination<SearchSystemMaster>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchSystemMaster(searchSystemMasterDTO,lang);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterService:");
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

        public GetSystemMasterModel GetSystemMasterService(string authorization, string lang, string platform, int logID, int keyID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetSystemMasterModel value = new GetSystemMasterModel();
            try
            {
                SystemMaster data = new SystemMaster();

                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetSystemMaster(keyID,lang);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterService:");
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


        //public SearchMasterDataModel SearchMasterDataPositionService(string authorization, string lang, string platform, int logID, int perPage, int pageInt, string paramSearch
        //    , int sortField, string sortType, int roleID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    SearchMasterDataModel value = new SearchMasterDataModel();
        //    try
        //    {
        //        Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401000");
        //        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.SearchMasterDataPosition(paramSearch, perPage, pageInt, sortField, sortType);
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.data = data;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterDataPositionService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public GetMasterDataModel GetMasterPositionService(string authorization, string lang, string platform, int logID, int masterID, int roleID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    GetMasterDataModel value = new GetMasterDataModel();
        //    try
        //    {
        //        MasterData data = new MasterData();

        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401003");
        //        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.GetMasterPosition(masterID);
        //            value.data = data;
        //            value.success = validation.Success;
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterPositionService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel InsertMasterPositionService(string authorization, string lang, string platform, int logID,
        //    MasterDataDTO masterDataDTO, int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "position", masterDataDTO);
        //        if (validation.Success == true)
        //        {
        //            //List<string> listobjectID = new List<string>();
        //            //listobjectID.Add("100401001");
        //            //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //            validation = ValidationManager.CheckValidation(1, lang, platform);
        //            if (validation.Success == true)
        //            {
        //                value.data = _sql.InsertMasterPosition(masterDataDTO, userID);
        //            }
        //            else
        //            {
        //                _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //            }
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "InsertMasterPositionService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel UpdateMasterPositionService(string authorization, string lang, string platform, int logID,
        //    MasterDataDTO masterDataDTO, int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "position", masterDataDTO);
        //        if (validation.Success == true)
        //        {
        //            //ValidationModel validation = new ValidationModel();
        //            //List<string> listobjectID = new List<string>();
        //            //listobjectID.Add("100401002");
        //            //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
        //            validation = ValidationManager.CheckValidation(1, lang, platform);
        //            if (validation.Success == true)
        //            {
        //                string TableName = "master_position";
        //                _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
        //                _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
        //                value.data = _sql.UpdateMasterPosition(masterDataDTO, userID);
        //            }
        //            else
        //            {
        //                _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //            }
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateMasterPositionService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel DeleteMasterPositionSevice(string authorization, string lang, string platform, int logID, int masterID,
        //    int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        //ValidationModel validation = new ValidationModel();
        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401002");
        //        //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.DeleteMasterPosition(masterID, userID);
        //            value.data = data;
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteMasterPositionSevice:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public SearchMasterDataModel SearchMasterDataProductAreaService(string authorization, string lang, string platform, int logID, int perPage, int pageInt, string paramSearch
        //    , int sortField, string sortType, int roleID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    SearchMasterDataModel value = new SearchMasterDataModel();
        //    try
        //    {
        //        Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401000");
        //        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.SearchMasterDataProductArea(paramSearch, perPage, pageInt, sortField, sortType);
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.data = data;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterDataProductAreaService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public GetMasterDataModel GetMasterProductAreaService(string authorization, string lang, string platform, int logID, int masterID, int roleID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    GetMasterDataModel value = new GetMasterDataModel();
        //    try
        //    {
        //        MasterData data = new MasterData();

        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401003");
        //        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.GetMasterProductArea(masterID);
        //            value.data = data;
        //            value.success = validation.Success;
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterProductAreaService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel InsertMasterProductAreaService(string authorization, string lang, string platform, int logID,
        //   MasterDataDTO masterDataDTO, int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "productarea", masterDataDTO);
        //        if (validation.Success == true)
        //        {
        //            //List<string> listobjectID = new List<string>();
        //            //listobjectID.Add("100401001");
        //            //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //            validation = ValidationManager.CheckValidation(1, lang, platform);
        //            if (validation.Success == true)
        //            {
        //                value.data = _sql.InsertMasterProductArea(masterDataDTO, userID);
        //            }
        //            else
        //            {
        //                _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //            }
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "InsertMasterProductAreaService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel UpdateMasterProductAreaService(string authorization, string lang, string platform, int logID,
        //    MasterDataDTO masterDataDTO, int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "productarea", masterDataDTO);
        //        if (validation.Success == true)
        //        {
        //            //ValidationModel validation = new ValidationModel();
        //            //List<string> listobjectID = new List<string>();
        //            //listobjectID.Add("100401002");
        //            //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
        //            validation = ValidationManager.CheckValidation(1, lang, platform);
        //            if (validation.Success == true)
        //            {
        //                string TableName = "master_product_area";
        //                _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
        //                _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
        //                value.data = _sql.UpdateMasterProductArea(masterDataDTO, userID); ;
        //            }
        //            else
        //            {
        //                _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //            }
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateMasterProductAreaService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel DeleteMasterProductAreaSevice(string authorization, string lang, string platform, int logID, int masterID,
        //    int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        //ValidationModel validation = new ValidationModel();
        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401002");
        //        //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.DeleteMasterProductArea(masterID, userID);
        //            value.data = data;
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteMasterProductAreaSevice:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public SearchMasterDataModel SearchMasterDataProductCategoryService(string authorization, string lang, string platform, int logID, int perPage, int pageInt, string paramSearch
        //    , int sortField, string sortType, int roleID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    SearchMasterDataModel value = new SearchMasterDataModel();
        //    try
        //    {
        //        Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401000");
        //        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.SearchMasterDataProductCategory(paramSearch, perPage, pageInt, sortField, sortType);
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.data = data;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterDataProductCategoryService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public GetMasterDataModel GetMasterProductCategoryService(string authorization, string lang, string platform, int logID, int masterID, int roleID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    GetMasterDataModel value = new GetMasterDataModel();
        //    try
        //    {
        //        MasterData data = new MasterData();

        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401003");
        //        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.GetMasterProductCategory(masterID);
        //            value.data = data;
        //            value.success = validation.Success;
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterProductCategoryService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel InsertMasterProductCategoryService(string authorization, string lang, string platform, int logID,
        //  MasterDataDTO masterDataDTO, int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "productcategory", masterDataDTO);
        //        if (validation.Success == true)
        //        {
        //            //List<string> listobjectID = new List<string>();
        //            //listobjectID.Add("100401001");
        //            //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //            validation = ValidationManager.CheckValidation(1, lang, platform);
        //            if (validation.Success == true)
        //            {
        //                value.data = _sql.InsertMasterProductCategory(masterDataDTO, userID);
        //            }
        //            else
        //            {
        //                _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //            }
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "InsertMasterProductCategoryService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel UpdateMasterProductCategoryService(string authorization, string lang, string platform, int logID,
        //    MasterDataDTO masterDataDTO, int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "productcategory", masterDataDTO);
        //        if (validation.Success == true)
        //        {
        //            //ValidationModel validation = new ValidationModel();
        //            //List<string> listobjectID = new List<string>();
        //            //listobjectID.Add("100401002");
        //            //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
        //            validation = ValidationManager.CheckValidation(1, lang, platform);
        //            if (validation.Success == true)
        //            {
        //                string TableName = "master_product_category";
        //                _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
        //                _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
        //                value.data = _sql.UpdateMasterProductCategory(masterDataDTO, userID); ;
        //            }
        //            else
        //            {
        //                _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //            }
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateMasterProductCategoryService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel DeleteMasterProductCategorySevice(string authorization, string lang, string platform, int logID, int masterID,
        //    int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        //ValidationModel validation = new ValidationModel();
        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401002");
        //        //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.DeleteMasterProductCategory(masterID, userID);
        //            value.data = data;
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteMasterProductCategorySevice:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public SearchMasterDataModel SearchMasterDataProductTypeService(string authorization, string lang, string platform, int logID, int perPage, int pageInt, string paramSearch
        //    , int sortField, string sortType, int roleID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    SearchMasterDataModel value = new SearchMasterDataModel();
        //    try
        //    {
        //        Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401000");
        //        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.SearchMasterDataProductType(paramSearch, perPage, pageInt, sortField, sortType);
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.data = data;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterDataProductTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public GetMasterDataModel GetMasterProductTypeService(string authorization, string lang, string platform, int logID, int masterID, int roleID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    GetMasterDataModel value = new GetMasterDataModel();
        //    try
        //    {
        //        MasterData data = new MasterData();

        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401003");
        //        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.GetMasterProductType(masterID);
        //            value.data = data;
        //            value.success = validation.Success;
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterProductTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel InsertMasterProductTypeService(string authorization, string lang, string platform, int logID,
        //  MasterDataDTO masterDataDTO, int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "producttype", masterDataDTO);
        //        if (validation.Success == true)
        //        {
        //            //List<string> listobjectID = new List<string>();
        //            //listobjectID.Add("100401001");
        //            //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //            validation = ValidationManager.CheckValidation(1, lang, platform);
        //            if (validation.Success == true)
        //            {
        //                value.data = _sql.InsertMasterProductType(masterDataDTO, userID);
        //            }
        //            else
        //            {
        //                _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //            }
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "InsertMasterProductTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel UpdateMasterProductTypeService(string authorization, string lang, string platform, int logID,
        //    MasterDataDTO masterDataDTO, int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "producttype", masterDataDTO);
        //        if (validation.Success == true)
        //        {
        //            //ValidationModel validation = new ValidationModel();
        //            //List<string> listobjectID = new List<string>();
        //            //listobjectID.Add("100401002");
        //            //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
        //            validation = ValidationManager.CheckValidation(1, lang, platform);
        //            if (validation.Success == true)
        //            {
        //                string TableName = "master_product_type";
        //                _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
        //                _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
        //                value.data = _sql.UpdateMasterProductType(masterDataDTO, userID); ;
        //            }
        //            else
        //            {
        //                _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //            }
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateMasterProductTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel DeleteMasterProductTypeSevice(string authorization, string lang, string platform, int logID, int masterID,
        //   int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        //ValidationModel validation = new ValidationModel();
        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401002");
        //        //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.DeleteMasterProductType(masterID, userID);
        //            value.data = data;
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteMasterProductTypeSevice:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public SearchMasterDataModel SearchMasterDataQueMemberTypeService(string authorization, string lang, string platform, int logID, int perPage, int pageInt, string paramSearch
        //    , int sortField, string sortType, int roleID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    SearchMasterDataModel value = new SearchMasterDataModel();
        //    try
        //    {
        //        Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401000");
        //        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.SearchMasterDataQueMemberType(paramSearch, perPage, pageInt, sortField, sortType);
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.data = data;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterDataQueMemberTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public GetMasterDataModel GetMasterQueMemberTypeService(string authorization, string lang, string platform, int logID, int masterID, int roleID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    GetMasterDataModel value = new GetMasterDataModel();
        //    try
        //    {
        //        MasterData data = new MasterData();

        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401003");
        //        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.GetMasterQueMemberType(masterID);
        //            value.data = data;
        //            value.success = validation.Success;
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterQueMemberTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel InsertMasterQueMemberTypeService(string authorization, string lang, string platform, int logID,
        //  MasterDataDTO masterDataDTO, int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "quemembertype", masterDataDTO);
        //        if (validation.Success == true)
        //        {
        //            //List<string> listobjectID = new List<string>();
        //            //listobjectID.Add("100401001");
        //            //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //            validation = ValidationManager.CheckValidation(1, lang, platform);
        //            if (validation.Success == true)
        //            {
        //                value.data = _sql.InsertMasterQueMemberType(masterDataDTO, userID);
        //            }
        //            else
        //            {
        //                _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //            }
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "InsertMasterQueMemberTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel UpdateMasterQueMemberTypeService(string authorization, string lang, string platform, int logID,
        //    MasterDataDTO masterDataDTO, int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "quemembertype", masterDataDTO);
        //        if (validation.Success == true)
        //        {
        //            //ValidationModel validation = new ValidationModel();
        //            //List<string> listobjectID = new List<string>();
        //            //listobjectID.Add("100401002");
        //            //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
        //            validation = ValidationManager.CheckValidation(1, lang, platform);
        //            if (validation.Success == true)
        //            {
        //                string TableName = "master_que_member_type";
        //                _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
        //                _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
        //                value.data = _sql.UpdateMasterQueMemberType(masterDataDTO, userID); ;
        //            }
        //            else
        //            {
        //                _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //            }
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateMasterQueMemberTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel DeleteMasterQueMemberTypeSevice(string authorization, string lang, string platform, int logID, int masterID,
        //   int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        //ValidationModel validation = new ValidationModel();
        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401002");
        //        //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.DeleteMasterQueMemberType(masterID, userID);
        //            value.data = data;
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteMasterQueMemberTypeSevice:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public SearchMasterDataModel SearchMasterDataQueStaffTypeService(string authorization, string lang, string platform, int logID, int perPage, int pageInt, string paramSearch
        //   , int sortField, string sortType, int roleID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    SearchMasterDataModel value = new SearchMasterDataModel();
        //    try
        //    {
        //        Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401000");
        //        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.SearchMasterDataQueStaffType(paramSearch, perPage, pageInt, sortField, sortType);
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.data = data;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterDataQueStaffTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public GetMasterDataModel GetMasterQueStaffTypeService(string authorization, string lang, string platform, int logID, int masterID, int roleID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    GetMasterDataModel value = new GetMasterDataModel();
        //    try
        //    {
        //        MasterData data = new MasterData();

        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401003");
        //        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.GetMasterQueStaffType(masterID);
        //            value.data = data;
        //            value.success = validation.Success;
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterQueMemberTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel InsertMasterQueStaffTypeService(string authorization, string lang, string platform, int logID,
        //  MasterDataDTO masterDataDTO, int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "questafftype", masterDataDTO);
        //        if (validation.Success == true)
        //        {
        //            //List<string> listobjectID = new List<string>();
        //            //listobjectID.Add("100401001");
        //            //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //            validation = ValidationManager.CheckValidation(1, lang, platform);
        //            if (validation.Success == true)
        //            {
        //                value.data = _sql.InsertMasterQueStaffType(masterDataDTO, userID);
        //            }
        //            else
        //            {
        //                _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //            }
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "InsertMasterQueStaffTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel UpdateMasterQueStaffTypeService(string authorization, string lang, string platform, int logID,
        //    MasterDataDTO masterDataDTO, int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "questafftype", masterDataDTO);
        //        if (validation.Success == true)
        //        {
        //            //ValidationModel validation = new ValidationModel();
        //            //List<string> listobjectID = new List<string>();
        //            //listobjectID.Add("100401002");
        //            //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
        //            validation = ValidationManager.CheckValidation(1, lang, platform);
        //            if (validation.Success == true)
        //            {
        //                string TableName = "master_que_staff_type";
        //                _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
        //                _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
        //                value.data = _sql.UpdateMasterQueStaffType(masterDataDTO, userID); ;
        //            }
        //            else
        //            {
        //                _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //            }
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateMasterQueStaffTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel DeleteMasterQueStaffTypeSevice(string authorization, string lang, string platform, int logID, int masterID,
        //   int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        //ValidationModel validation = new ValidationModel();
        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401002");
        //        //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.DeleteMasterQueStaffType(masterID, userID);
        //            value.data = data;
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteMasterQueMemberTypeSevice:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public SearchMasterDataModel SearchMasterDataRoomTypeService(string authorization, string lang, string platform, int logID, int perPage, int pageInt, string paramSearch
        //   , int sortField, string sortType, int roleID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    SearchMasterDataModel value = new SearchMasterDataModel();
        //    try
        //    {
        //        Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401000");
        //        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.SearchMasterDataRoomType(paramSearch, perPage, pageInt, sortField, sortType);
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.data = data;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterDataRoomTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public GetMasterDataModel GetMasterRoomTypeService(string authorization, string lang, string platform, int logID, int masterID, int roleID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    GetMasterDataModel value = new GetMasterDataModel();
        //    try
        //    {
        //        MasterData data = new MasterData();

        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401003");
        //        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.GetMasterRoomType(masterID);
        //            value.data = data;
        //            value.success = validation.Success;
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterRoomTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel InsertMasterRoomTypeService(string authorization, string lang, string platform, int logID,
        // MasterDataDTO masterDataDTO, int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "roomtype", masterDataDTO);
        //        if (validation.Success == true)
        //        {
        //            //List<string> listobjectID = new List<string>();
        //            //listobjectID.Add("100401001");
        //            //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //            validation = ValidationManager.CheckValidation(1, lang, platform);
        //            if (validation.Success == true)
        //            {
        //                value.data = _sql.InsertMasterRoomType(masterDataDTO, userID);
        //            }
        //            else
        //            {
        //                _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //            }
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "InsertMasterRoomTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel UpdateMasterRoomTypeService(string authorization, string lang, string platform, int logID,
        //    MasterDataDTO masterDataDTO, int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "roomtype", masterDataDTO);
        //        if (validation.Success == true)
        //        {
        //            //ValidationModel validation = new ValidationModel();
        //            //List<string> listobjectID = new List<string>();
        //            //listobjectID.Add("100401002");
        //            //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
        //            validation = ValidationManager.CheckValidation(1, lang, platform);
        //            if (validation.Success == true)
        //            {
        //                string TableName = "master_room_type";
        //                _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
        //                _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
        //                value.data = _sql.UpdateMasterRoomType(masterDataDTO, userID); ;
        //            }
        //            else
        //            {
        //                _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //            }
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateMasterRoomTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel DeleteMasterRoomTypeSevice(string authorization, string lang, string platform, int logID, int masterID,
        //   int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        //ValidationModel validation = new ValidationModel();
        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401002");
        //        //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.DeleteMasterRoomType(masterID, userID);
        //            value.data = data;
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteMasterRoomTypeSevice:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public SearchMasterDataModel SearchMasterDataStockTypeService(string authorization, string lang, string platform, int logID, int perPage, int pageInt, string paramSearch
        //  , int sortField, string sortType, int roleID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    SearchMasterDataModel value = new SearchMasterDataModel();
        //    try
        //    {
        //        Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401000");
        //        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.SearchMasterDataStockType(paramSearch, perPage, pageInt, sortField, sortType);
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.data = data;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterDataStockTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public GetMasterDataModel GetMasterStockTypeService(string authorization, string lang, string platform, int logID, int masterID, int roleID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    GetMasterDataModel value = new GetMasterDataModel();
        //    try
        //    {
        //        MasterData data = new MasterData();

        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401003");
        //        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.GetMasterStockType(masterID);
        //            value.data = data;
        //            value.success = validation.Success;
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterStockTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel InsertMasterStockTypeService(string authorization, string lang, string platform, int logID,
        // MasterDataDTO masterDataDTO, int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "stocktype", masterDataDTO);
        //        if (validation.Success == true)
        //        {
        //            //List<string> listobjectID = new List<string>();
        //            //listobjectID.Add("100401001");
        //            //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
        //            validation = ValidationManager.CheckValidation(1, lang, platform);
        //            if (validation.Success == true)
        //            {
        //                value.data = _sql.InsertMasterStockType(masterDataDTO, userID);
        //            }
        //            else
        //            {
        //                _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //            }
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "InsertMasterStockTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel UpdateMasterStockTypeService(string authorization, string lang, string platform, int logID,
        //    MasterDataDTO masterDataDTO, int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "stocktype", masterDataDTO);
        //        if (validation.Success == true)
        //        {
        //            //ValidationModel validation = new ValidationModel();
        //            //List<string> listobjectID = new List<string>();
        //            //listobjectID.Add("100401002");
        //            //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
        //            validation = ValidationManager.CheckValidation(1, lang, platform);
        //            if (validation.Success == true)
        //            {
        //                string TableName = "master_stock_type";
        //                _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
        //                _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
        //                value.data = _sql.UpdateMasterStockType(masterDataDTO, userID); ;
        //            }
        //            else
        //            {
        //                _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //            }
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateMasterStockTypeService:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}

        //public ReturnIdModel DeleteMasterStockTypeSevice(string authorization, string lang, string platform, int logID, int masterID,
        //   int roleID, int userID)
        //{
        //    if (_sql == null)
        //    {
        //        _sql = SQLManager.Instance;
        //    }

        //    ReturnIdModel value = new ReturnIdModel();
        //    try
        //    {
        //        _ReturnIdModel data = new _ReturnIdModel();

        //        //ValidationModel validation = new ValidationModel();
        //        //List<string> listobjectID = new List<string>();
        //        //listobjectID.Add("100401002");
        //        //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
        //        ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

        //        if (validation.Success == true)
        //        {
        //            data = _sql.DeleteMasterStockType(masterID, userID);
        //            value.data = data;
        //        }
        //        else
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
        //        }

        //        value.success = validation.Success;
        //        value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteMasterStockTypeSevice:");
        //        if (logID > 0)
        //        {
        //            _sql.UpdateLogReceiveDataError(logID, ex.ToString());
        //        }
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _sql.UpdateStatusLog(logID, 1);
        //    }
        //    return value;
        //}
        public ReturnIdModel DeleteBodySetService(string authorization, string lang, string platform, int logID, 
            SaveBodySetRequestDTO saveBodySetDTO, int userID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    string TableName = "system_body_set";
                    _sql.InsertSystemLogChangeWithShareCode(shareCode, saveBodySetDTO.masterID, TableName, "status", "0", userID);
                    
                    value.data = _sql.DeleteBodySet(shareCode, saveBodySetDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateBodySetService:");
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

        public ReturnIdModel InsertBodySetService(string authorization, string lang, string platform, int logID,
            SaveBodySetRequestDTO saveBodySetDTO, int userID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidationDupicateInsertBodySet(shareCode, lang, saveBodySetDTO);
                if (validation.Success == true)
                {
                    value.data = _sql.InsertBodySet(shareCode, saveBodySetDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertEmpRateService:");
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

        public ReturnIdModel UpdateBodySetService(string authorization, string lang, string platform, int logID, SaveBodySetRequestDTO saveBodySetDTO, int userID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    string TableName = "system_body_set";
                    _sql.InsertSystemLogChangeWithShareCode(shareCode, saveBodySetDTO.masterID, TableName, "height", saveBodySetDTO.height.ToString(), userID);
                    _sql.InsertSystemLogChangeWithShareCode(shareCode, saveBodySetDTO.masterID, TableName, "weight", saveBodySetDTO.weight.ToString(), userID);
                    _sql.InsertSystemLogChangeWithShareCode(shareCode, saveBodySetDTO.masterID, TableName, "chest", saveBodySetDTO.chest.ToString(), userID);
                    _sql.InsertSystemLogChangeWithShareCode(shareCode, saveBodySetDTO.masterID, TableName, "waist", saveBodySetDTO.waist.ToString(), userID);
                    _sql.InsertSystemLogChangeWithShareCode(shareCode, saveBodySetDTO.masterID, TableName, "hip", saveBodySetDTO.hip.ToString(), userID);

                    value.data = _sql.UpdateBodySet(shareCode, saveBodySetDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateBodySetService:");
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

        
        public GetBodySetModel GetMasterBodySetervice(string authorization, string lang, string platform, int logID, int masterID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetBodySetModel value = new GetBodySetModel();
            try
            {
                BodySet data = new BodySet();

                
                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetBodySet(shareCode, masterID);
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

        public GetDepartmentModel GetMasterDepartmentService(string authorization, string lang, string platform, int logID, int masterID, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetDepartmentModel value = new GetDepartmentModel();
            try
            {
                Department data = new Department();


                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetDepartment(masterID);
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



        #region searchService
        public SearchAllEmployeeModel SearchAllEmployee(string authorization, string lang, string platform, int logID, PageRequestDTO pageRequestDTO, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchAllEmployeeModel value = new SearchAllEmployeeModel();
            try
            {
                Pagination<SearchAllEmployee> data = new Pagination<SearchAllEmployee>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

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

        public SearchAllLeaveModel SearchAllLeave(string authorization, string lang, string platform, int logID, SearchLeaveDTO searchLeavetDTO, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchAllLeaveModel value = new SearchAllLeaveModel();
            value.data = new SearchAllLeaveDetail();
            value.data.dataSearch = new Pagination<SearchAllLeave>();
            //value.data.dataShift = new List<AllLeaveShift>();
            value.data.dataLeave = new List<AllLeave>();
            try
            {
                Pagination<SearchAllLeave> dataSearch = new Pagination<SearchAllLeave>();
                List<AllLeaveShift> dataShift = new List<AllLeaveShift>();
                List<AllLeave> dataLeave = new List<AllLeave>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    dataSearch = _sql.SearchAllLeave(shareCode, searchLeavetDTO);

                    List<SearchAllLeave> total = dataSearch.data;

                    List<int> termsList = new List<int>();
                    foreach (SearchAllLeave item in total)
                    {
                        termsList.Add(item.empID);
                    }

                    //int[] terms = termsList.ToArray();
                    //string theString = string.Join(",", terms);

                    //List<GetWorkShift> allWS = new List<GetWorkShift>();
                    //allWS = _sql.GetAllWorkShiftList(shareCode);

                    //foreach (GetWorkShift item in allWS)
                    //{
                    //    AllLeaveShift sss = new AllLeaveShift();
                    //    sss = _sql.GetWorkShiftLeave(shareCode, theString, item.workShiftID, searchLeavetDTO.paramSearch);

                    //    dataShift.Add(sss);
                    //}

                    List<SearchAllLeave> totalLeave = dataSearch.data;

                    List<int> _termsList = new List<int>();
                    foreach (SearchAllLeave item in totalLeave)
                    {
                        _termsList.Add(item.empID);
                    }
                    int[] _terms = _termsList.ToArray();
                    string _theString = string.Join(",", _terms);

                    List<GetLeave> _allWS = new List<GetLeave>();
                    _allWS = _sql.GetAllLeaveList();

                    foreach (GetLeave item in _allWS)
                    {
                        AllLeave listLeave = new AllLeave();
                        listLeave = _sql.GetLeave(shareCode, _theString, item.leaveTypeID, searchLeavetDTO.paramSearch);

                        dataLeave.Add(listLeave);
                    }

                    //dataLeave = _sql.;
                }
                else
                {
                    _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.data.dataSearch = dataSearch;
                //value.data.dataShift = dataShift;
                value.data.dataLeave = dataLeave;
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

        public SearchAllPendingLeaveModel SearchAllPendingLeave(string authorization, string lang, string platform, int logID, SearchPendingLeaveDTO searchPendingLeaveDTO, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchAllPendingLeaveModel value = new SearchAllPendingLeaveModel();
            value.data = new SearchAllPendingLeaveDetail();
            value.data.dataSearch = new Pagination<SearchAllLeave>();
            value.data.dataLeave = new List<AllLeave>();
            try
            {
                Pagination<SearchAllLeave> dataSearch = new Pagination<SearchAllLeave>();
                List<AllLeaveShift> dataShift = new List<AllLeaveShift>();
                List<AllLeave> dataLeave = new List<AllLeave>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    dataSearch = _sql.SearchAllPendingLeaves(lang, shareCode, searchPendingLeaveDTO);

                    List<SearchAllLeave> total = dataSearch.data;

                    List<int> termsList = new List<int>();
                    foreach (SearchAllLeave item in total)
                    {
                        termsList.Add(item.empID);
                    }
                    
                    List<SearchAllLeave> totalLeave = dataSearch.data;

                    List<int> _termsList = new List<int>();
                    foreach (SearchAllLeave item in totalLeave)
                    {
                        _termsList.Add(item.empID);
                    }
                    int[] _terms = _termsList.ToArray();
                    string _theString = string.Join(",", _terms);

                    List<GetLeave> _allWS = new List<GetLeave>();
                    _allWS = _sql.GetAllLeaveList();

                    foreach (GetLeave item in _allWS)
                    {
                        AllLeave listLeave = new AllLeave();
                        listLeave = _sql.GetLeave(shareCode, _theString, item.leaveTypeID, searchPendingLeaveDTO.paramSearch);

                        dataLeave.Add(listLeave);
                    }

                    //dataLeave = _sql.;
                }
                else
                {
                    _sql.UpdateLogReceiveDataErrorWithShareCode(shareCode, logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.data.dataSearch = dataSearch;
                //value.data.dataShift = dataShift;
                value.data.dataLeave = dataLeave;
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


        public SearchAllMasterDepartmentPositionModel SearchAllDepartmentPosition(string authorization, string lang, string platform, int logID, SearchMasterDepartmentPositionDTO searchMasterDepartmentPositionDTO)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchAllMasterDepartmentPositionModel value = new SearchAllMasterDepartmentPositionModel();
            value.data = new Pagination<SearchAllMasterDepartmentPosition>();
            try
            {
                Pagination<SearchAllMasterDepartmentPosition> dataSearch = new Pagination<SearchAllMasterDepartmentPosition>();

                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    dataSearch = _sql.SearchAllDepartmentPosition(searchMasterDepartmentPositionDTO);
                    
                    

                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.data = dataSearch;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchAllDepartmentPosition:");
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

        public ReturnIdModel InsertMasterWorkShiftService(string shareCode, string authorization, string lang, string platform, int logID,
            SaveEmpWorkShiftRequestDTO saveEmpWorkShiftRequestDTO, int userID)
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
                    value.data = _sql.InsertEmpWorkShift(shareCode, saveEmpWorkShiftRequestDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertMasterWorkShiftService:");
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

        public ReturnIdModel UpdateMasterWorkShiftService(string shareCode, string authorization, string lang, string platform, int logID, SaveEmpWorkShiftRequestDTO saveEmpWorkShiftRequestDTO, int userID)
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
                    string TableName = "emp_work_shift";
                    _sql.InsertSystemLogChange(shareCode, saveEmpWorkShiftRequestDTO.workShiftID, TableName, "ws_code", saveEmpWorkShiftRequestDTO.wsCode, userID);
                    _sql.InsertSystemLogChange(shareCode, saveEmpWorkShiftRequestDTO.workShiftID, TableName, "time_start", saveEmpWorkShiftRequestDTO.timeStart, userID);
                    _sql.InsertSystemLogChange(shareCode, saveEmpWorkShiftRequestDTO.workShiftID, TableName, "time_end", saveEmpWorkShiftRequestDTO.timeEnd, userID);
                    _sql.InsertSystemLogChange(shareCode, saveEmpWorkShiftRequestDTO.workTypeID, TableName, "work_type_id", saveEmpWorkShiftRequestDTO.workTypeID.ToString(), userID);
                    _sql.InsertSystemLogChange(shareCode, saveEmpWorkShiftRequestDTO.workShiftID, TableName, "remark", saveEmpWorkShiftRequestDTO.remark, userID);
                    _sql.InsertSystemLogChange(shareCode, saveEmpWorkShiftRequestDTO.workShiftID, TableName, "status", saveEmpWorkShiftRequestDTO.status.ToString(), userID);

                    value.data = _sql.UpdateEmpWorkShift(shareCode, saveEmpWorkShiftRequestDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateMasterWorkShiftService:");
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

        public GetEmpWorkShiftModel GetMasterWorkShiftService(string shareCode, string authorization, string lang, string platform, int logID, int empWorkShiftID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetEmpWorkShiftModel value = new GetEmpWorkShiftModel();
            try
            {
                GetEmpWorkShift data = new GetEmpWorkShift();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetEmpWorkShift(shareCode, empWorkShiftID);
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

        public UpdateSwitchWorkShiftModel UpdateSwitchWorkShiftService(string shareCode, string authorization, string lang, string platform, int logID, int workShiftID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            UpdateSwitchWorkShiftModel value = new UpdateSwitchWorkShiftModel();
            try
            {
                value.data = new SwitchWorkShift();
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    GetEmpWorkShift data = new GetEmpWorkShift();
                    data = _sql.GetEmpWorkShift(shareCode, workShiftID);

                    string TableName = "emp_work_shift";
                    _sql.InsertSystemLogChange(shareCode, workShiftID, TableName, "status", data.status.ToString(), userID);

                    value.data = _sql.UpdateSwitchStatusWorkShift(shareCode, workShiftID, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateSwitchWorkShiftService:");
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

        public SearchAllMasterWorkShiftModel SearchAllWorkShiftService(string authorization, string lang, string platform, int logID, PageRequestDTO pageRequestDTO, string shareCode)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchAllMasterWorkShiftModel value = new SearchAllMasterWorkShiftModel();
            try
            {
                Pagination<SearchAllMasterWorkShift> data = new Pagination<SearchAllMasterWorkShift>();

                ValidationModel validation = ValidationManager.CheckValidationWithShareCode(shareCode, 1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchAllWorkShiftPage(shareCode, pageRequestDTO);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchAllWorkShiftService:");
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
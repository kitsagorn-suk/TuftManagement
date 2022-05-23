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

        public ReturnIdModel SaveMasterService(string authorization, string lang, string platform, int logID,
            MasterDataDTO masterDataDTO, string TableName, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, TableName, masterDataDTO);
                if (validation.Success == true)
                {
                    if (masterDataDTO.mode.ToLower() == "insert")
                    {
                        //List<string> listobjectID = new List<string>();
                        //listobjectID.Add("100401001");
                        //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                        validation = ValidationManager.CheckValidation(1, lang, platform);
                        value.data = _sql.InsertMasterData(masterDataDTO, TableName, userID);
                    }
                    else if (masterDataDTO.mode.ToLower() == "update")
                    {
                        //ValidationModel validation = new ValidationModel();
                        //List<string> listobjectID = new List<string>();
                        //listobjectID.Add("100401002");
                        //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                        validation = ValidationManager.CheckValidation(1, lang, platform);
                        if (validation.Success == true)
                        {
                            _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
                            _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
                            value.data = _sql.UpdateMasterData(masterDataDTO, TableName, userID);
                        }
                        else
                        {
                            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                        }
                    }
                    else if (masterDataDTO.mode.ToLower() == "delete")
                    {
                        //ValidationModel validation = new ValidationModel();
                        //List<string> listobjectID = new List<string>();
                        //listobjectID.Add("100401002");
                        //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                        validation = ValidationManager.CheckValidation(1, lang, platform);
                        if (validation.Success == true)
                        {
                            value.data = _sql.DeleteMasterData(masterDataDTO, TableName, userID);
                        }
                        else
                        {
                            _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                        }
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "SaveMasterService:");
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

        public GetMasterDataModel GetMasterService(string authorization, string lang, string platform, int logID, int masterID, string TableName)
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
                    data = _sql.GetMasterData(masterID, TableName);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterService:");
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

        public SearchMasterDataModel SearchMasterService(string authorization, string lang, string platform, int logID, SearchMasterDataDTO searchMasterDataDTO, string TableName, int roleID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchMasterDataModel value = new SearchMasterDataModel();
            try
            {
                Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchMaster(searchMasterDataDTO, TableName);
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
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




        public SearchMasterDataModel SearchMasterDataPositionService(string authorization, string lang, string platform, int logID, int perPage, int pageInt, string paramSearch
            , int sortField, string sortType, int roleID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchMasterDataModel value = new SearchMasterDataModel();
            try
            {
                Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401000");
                //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchMasterDataPosition(paramSearch, perPage, pageInt, sortField, sortType);
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.data = data;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterDataPositionService:");
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

        public GetMasterDataModel GetMasterPositionService(string authorization, string lang, string platform, int logID, int masterID, int roleID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetMasterDataModel value = new GetMasterDataModel();
            try
            {
                MasterData data = new MasterData();

                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401003");
                //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetMasterPosition(masterID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterPositionService:");
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

        public ReturnIdModel InsertMasterPositionService(string authorization, string lang, string platform, int logID,
            MasterDataDTO masterDataDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            
            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "position", masterDataDTO);
                if (validation.Success == true)
                {
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401001");
                    //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        value.data = _sql.InsertMasterPosition(masterDataDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertMasterPositionService:");
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

        public ReturnIdModel UpdateMasterPositionService(string authorization, string lang, string platform, int logID,
            MasterDataDTO masterDataDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            
            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();
                
                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "position", masterDataDTO);
                if (validation.Success == true)
                {
                    //ValidationModel validation = new ValidationModel();
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401002");
                    //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        string TableName = "master_position";
                        _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
                        _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
                        value.data = _sql.UpdateMasterPosition(masterDataDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateMasterPositionService:");
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

        public ReturnIdModel DeleteMasterPositionSevice(string authorization, string lang, string platform, int logID, int masterID,
            int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                //ValidationModel validation = new ValidationModel();
                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401002");
                //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.DeleteMasterPosition(masterID, userID);
                    value.data = data;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteMasterPositionSevice:");
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

        public SearchMasterDataModel SearchMasterDataProductAreaService(string authorization, string lang, string platform, int logID, int perPage, int pageInt, string paramSearch
            , int sortField, string sortType, int roleID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchMasterDataModel value = new SearchMasterDataModel();
            try
            {
                Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401000");
                //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchMasterDataProductArea(paramSearch, perPage, pageInt, sortField, sortType);
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.data = data;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterDataProductAreaService:");
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

        public GetMasterDataModel GetMasterProductAreaService(string authorization, string lang, string platform, int logID, int masterID, int roleID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetMasterDataModel value = new GetMasterDataModel();
            try
            {
                MasterData data = new MasterData();

                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401003");
                //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetMasterProductArea(masterID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterProductAreaService:");
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

        public ReturnIdModel InsertMasterProductAreaService(string authorization, string lang, string platform, int logID,
           MasterDataDTO masterDataDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "productarea", masterDataDTO);
                if (validation.Success == true)
                {
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401001");
                    //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        value.data = _sql.InsertMasterProductArea(masterDataDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertMasterProductAreaService:");
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

        public ReturnIdModel UpdateMasterProductAreaService(string authorization, string lang, string platform, int logID,
            MasterDataDTO masterDataDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "productarea", masterDataDTO);
                if (validation.Success == true)
                {
                    //ValidationModel validation = new ValidationModel();
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401002");
                    //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        string TableName = "master_product_area";
                        _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
                        _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
                        value.data = _sql.UpdateMasterProductArea(masterDataDTO, userID); ;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateMasterProductAreaService:");
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

        public ReturnIdModel DeleteMasterProductAreaSevice(string authorization, string lang, string platform, int logID, int masterID,
            int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                //ValidationModel validation = new ValidationModel();
                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401002");
                //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.DeleteMasterProductArea(masterID, userID);
                    value.data = data;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteMasterProductAreaSevice:");
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

        public SearchMasterDataModel SearchMasterDataProductCategoryService(string authorization, string lang, string platform, int logID, int perPage, int pageInt, string paramSearch
            , int sortField, string sortType, int roleID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchMasterDataModel value = new SearchMasterDataModel();
            try
            {
                Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401000");
                //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchMasterDataProductCategory(paramSearch, perPage, pageInt, sortField, sortType);
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.data = data;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterDataProductCategoryService:");
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

        public GetMasterDataModel GetMasterProductCategoryService(string authorization, string lang, string platform, int logID, int masterID, int roleID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetMasterDataModel value = new GetMasterDataModel();
            try
            {
                MasterData data = new MasterData();

                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401003");
                //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetMasterProductCategory(masterID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterProductCategoryService:");
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

        public ReturnIdModel InsertMasterProductCategoryService(string authorization, string lang, string platform, int logID,
          MasterDataDTO masterDataDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "productcategory", masterDataDTO);
                if (validation.Success == true)
                {
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401001");
                    //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        value.data = _sql.InsertMasterProductCategory(masterDataDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertMasterProductCategoryService:");
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

        public ReturnIdModel UpdateMasterProductCategoryService(string authorization, string lang, string platform, int logID,
            MasterDataDTO masterDataDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "productcategory", masterDataDTO);
                if (validation.Success == true)
                {
                    //ValidationModel validation = new ValidationModel();
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401002");
                    //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        string TableName = "master_product_category";
                        _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
                        _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
                        value.data = _sql.UpdateMasterProductCategory(masterDataDTO, userID); ;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateMasterProductCategoryService:");
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

        public ReturnIdModel DeleteMasterProductCategorySevice(string authorization, string lang, string platform, int logID, int masterID,
            int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                //ValidationModel validation = new ValidationModel();
                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401002");
                //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.DeleteMasterProductCategory(masterID, userID);
                    value.data = data;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteMasterProductCategorySevice:");
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

        public SearchMasterDataModel SearchMasterDataProductTypeService(string authorization, string lang, string platform, int logID, int perPage, int pageInt, string paramSearch
            , int sortField, string sortType, int roleID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchMasterDataModel value = new SearchMasterDataModel();
            try
            {
                Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401000");
                //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchMasterDataProductType(paramSearch, perPage, pageInt, sortField, sortType);
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.data = data;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterDataProductTypeService:");
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

        public GetMasterDataModel GetMasterProductTypeService(string authorization, string lang, string platform, int logID, int masterID, int roleID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetMasterDataModel value = new GetMasterDataModel();
            try
            {
                MasterData data = new MasterData();

                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401003");
                //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetMasterProductType(masterID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterProductTypeService:");
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

        public ReturnIdModel InsertMasterProductTypeService(string authorization, string lang, string platform, int logID,
          MasterDataDTO masterDataDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "producttype", masterDataDTO);
                if (validation.Success == true)
                {
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401001");
                    //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        value.data = _sql.InsertMasterProductType(masterDataDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertMasterProductTypeService:");
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

        public ReturnIdModel UpdateMasterProductTypeService(string authorization, string lang, string platform, int logID,
            MasterDataDTO masterDataDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "producttype", masterDataDTO);
                if (validation.Success == true)
                {
                    //ValidationModel validation = new ValidationModel();
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401002");
                    //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        string TableName = "master_product_type";
                        _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
                        _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
                        value.data = _sql.UpdateMasterProductType(masterDataDTO, userID); ;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateMasterProductTypeService:");
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

        public ReturnIdModel DeleteMasterProductTypeSevice(string authorization, string lang, string platform, int logID, int masterID,
           int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                //ValidationModel validation = new ValidationModel();
                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401002");
                //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.DeleteMasterProductType(masterID, userID);
                    value.data = data;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteMasterProductTypeSevice:");
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

        public SearchMasterDataModel SearchMasterDataQueMemberTypeService(string authorization, string lang, string platform, int logID, int perPage, int pageInt, string paramSearch
            , int sortField, string sortType, int roleID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchMasterDataModel value = new SearchMasterDataModel();
            try
            {
                Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401000");
                //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchMasterDataQueMemberType(paramSearch, perPage, pageInt, sortField, sortType);
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.data = data;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterDataQueMemberTypeService:");
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

        public GetMasterDataModel GetMasterQueMemberTypeService(string authorization, string lang, string platform, int logID, int masterID, int roleID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetMasterDataModel value = new GetMasterDataModel();
            try
            {
                MasterData data = new MasterData();

                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401003");
                //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetMasterQueMemberType(masterID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterQueMemberTypeService:");
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

        public ReturnIdModel InsertMasterQueMemberTypeService(string authorization, string lang, string platform, int logID,
          MasterDataDTO masterDataDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "quemembertype", masterDataDTO);
                if (validation.Success == true)
                {
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401001");
                    //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        value.data = _sql.InsertMasterQueMemberType(masterDataDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertMasterQueMemberTypeService:");
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

        public ReturnIdModel UpdateMasterQueMemberTypeService(string authorization, string lang, string platform, int logID,
            MasterDataDTO masterDataDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "quemembertype", masterDataDTO);
                if (validation.Success == true)
                {
                    //ValidationModel validation = new ValidationModel();
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401002");
                    //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        string TableName = "master_que_member_type";
                        _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
                        _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
                        value.data = _sql.UpdateMasterQueMemberType(masterDataDTO, userID); ;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateMasterQueMemberTypeService:");
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

        public ReturnIdModel DeleteMasterQueMemberTypeSevice(string authorization, string lang, string platform, int logID, int masterID,
           int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                //ValidationModel validation = new ValidationModel();
                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401002");
                //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.DeleteMasterQueMemberType(masterID, userID);
                    value.data = data;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteMasterQueMemberTypeSevice:");
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

        public SearchMasterDataModel SearchMasterDataQueStaffTypeService(string authorization, string lang, string platform, int logID, int perPage, int pageInt, string paramSearch
           , int sortField, string sortType, int roleID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchMasterDataModel value = new SearchMasterDataModel();
            try
            {
                Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401000");
                //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchMasterDataQueStaffType(paramSearch, perPage, pageInt, sortField, sortType);
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.data = data;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterDataQueStaffTypeService:");
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

        public GetMasterDataModel GetMasterQueStaffTypeService(string authorization, string lang, string platform, int logID, int masterID, int roleID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetMasterDataModel value = new GetMasterDataModel();
            try
            {
                MasterData data = new MasterData();

                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401003");
                //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetMasterQueStaffType(masterID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterQueMemberTypeService:");
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

        public ReturnIdModel InsertMasterQueStaffTypeService(string authorization, string lang, string platform, int logID,
          MasterDataDTO masterDataDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "questafftype", masterDataDTO);
                if (validation.Success == true)
                {
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401001");
                    //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        value.data = _sql.InsertMasterQueStaffType(masterDataDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertMasterQueStaffTypeService:");
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

        public ReturnIdModel UpdateMasterQueStaffTypeService(string authorization, string lang, string platform, int logID,
            MasterDataDTO masterDataDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "questafftype", masterDataDTO);
                if (validation.Success == true)
                {
                    //ValidationModel validation = new ValidationModel();
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401002");
                    //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        string TableName = "master_que_staff_type";
                        _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
                        _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
                        value.data = _sql.UpdateMasterQueStaffType(masterDataDTO, userID); ;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateMasterQueStaffTypeService:");
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

        public ReturnIdModel DeleteMasterQueStaffTypeSevice(string authorization, string lang, string platform, int logID, int masterID,
           int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                //ValidationModel validation = new ValidationModel();
                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401002");
                //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.DeleteMasterQueStaffType(masterID, userID);
                    value.data = data;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteMasterQueMemberTypeSevice:");
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

        public SearchMasterDataModel SearchMasterDataRoomTypeService(string authorization, string lang, string platform, int logID, int perPage, int pageInt, string paramSearch
           , int sortField, string sortType, int roleID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchMasterDataModel value = new SearchMasterDataModel();
            try
            {
                Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401000");
                //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchMasterDataRoomType(paramSearch, perPage, pageInt, sortField, sortType);
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.data = data;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterDataRoomTypeService:");
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

        public GetMasterDataModel GetMasterRoomTypeService(string authorization, string lang, string platform, int logID, int masterID, int roleID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetMasterDataModel value = new GetMasterDataModel();
            try
            {
                MasterData data = new MasterData();

                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401003");
                //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetMasterRoomType(masterID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterRoomTypeService:");
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

        public ReturnIdModel InsertMasterRoomTypeService(string authorization, string lang, string platform, int logID,
         MasterDataDTO masterDataDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "roomtype", masterDataDTO);
                if (validation.Success == true)
                {
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401001");
                    //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        value.data = _sql.InsertMasterRoomType(masterDataDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertMasterRoomTypeService:");
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

        public ReturnIdModel UpdateMasterRoomTypeService(string authorization, string lang, string platform, int logID,
            MasterDataDTO masterDataDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "roomtype", masterDataDTO);
                if (validation.Success == true)
                {
                    //ValidationModel validation = new ValidationModel();
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401002");
                    //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        string TableName = "master_room_type";
                        _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
                        _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
                        value.data = _sql.UpdateMasterRoomType(masterDataDTO, userID); ;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateMasterRoomTypeService:");
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

        public ReturnIdModel DeleteMasterRoomTypeSevice(string authorization, string lang, string platform, int logID, int masterID,
           int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                //ValidationModel validation = new ValidationModel();
                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401002");
                //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.DeleteMasterRoomType(masterID, userID);
                    value.data = data;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteMasterRoomTypeSevice:");
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

        public SearchMasterDataModel SearchMasterDataStockTypeService(string authorization, string lang, string platform, int logID, int perPage, int pageInt, string paramSearch
          , int sortField, string sortType, int roleID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            SearchMasterDataModel value = new SearchMasterDataModel();
            try
            {
                Pagination<SearchMasterData> data = new Pagination<SearchMasterData>();

                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401000");
                //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.SearchMasterDataStockType(paramSearch, perPage, pageInt, sortField, sortType);
                }
                else
                {
                    _sql.UpdateLogReceiveDataError(logID, validation.InvalidMessage);
                }

                value.success = validation.Success;
                value.data = data;
                value.msg = new MsgModel() { code = validation.InvalidCode, text = validation.InvalidMessage, topic = validation.InvalidText };
            }
            catch (Exception ex)
            {
                LogManager.ServiceLog.WriteExceptionLog(ex, "SearchMasterDataStockTypeService:");
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

        public GetMasterDataModel GetMasterStockTypeService(string authorization, string lang, string platform, int logID, int masterID, int roleID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetMasterDataModel value = new GetMasterDataModel();
            try
            {
                MasterData data = new MasterData();

                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401003");
                //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetMasterStockType(masterID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetMasterStockTypeService:");
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

        public ReturnIdModel InsertMasterStockTypeService(string authorization, string lang, string platform, int logID,
         MasterDataDTO masterDataDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "stocktype", masterDataDTO);
                if (validation.Success == true)
                {
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401001");
                    //ValidationModel validation = ValidationManager.CheckRoleValidation(lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        value.data = _sql.InsertMasterStockType(masterDataDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "InsertMasterStockTypeService:");
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

        public ReturnIdModel UpdateMasterStockTypeService(string authorization, string lang, string platform, int logID,
            MasterDataDTO masterDataDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                ValidationModel validation = ValidationManager.CheckValidationDupicateMasterData(lang, "stocktype", masterDataDTO);
                if (validation.Success == true)
                {
                    //ValidationModel validation = new ValidationModel();
                    //List<string> listobjectID = new List<string>();
                    //listobjectID.Add("100401002");
                    //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                    validation = ValidationManager.CheckValidation(1, lang, platform);
                    if (validation.Success == true)
                    {
                        string TableName = "master_stock_type";
                        _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_en", masterDataDTO.nameEN, userID);
                        _sql.InsertSystemLogChange(masterDataDTO.masterID, TableName, "name_th", masterDataDTO.nameTH, userID);
                        value.data = _sql.UpdateMasterStockType(masterDataDTO, userID); ;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateMasterStockTypeService:");
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

        public ReturnIdModel DeleteMasterStockTypeSevice(string authorization, string lang, string platform, int logID, int masterID,
           int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            ReturnIdModel value = new ReturnIdModel();
            try
            {
                _ReturnIdModel data = new _ReturnIdModel();

                //ValidationModel validation = new ValidationModel();
                //List<string> listobjectID = new List<string>();
                //listobjectID.Add("100401002");
                //validation = ValidationManager.CheckValidationUpdate(masterCompanyDTO.companyID, "system_company", userID, lang, listobjectID, roleID);
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.DeleteMasterStockType(masterID, userID);
                    value.data = data;
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "DeleteMasterStockTypeSevice:");
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

        public ReturnIdModel InsertBodySetService(string authorization, string lang, string platform, int logID,
            SaveBodySetRequestDTO saveBodySetDTO, int roleID, int userID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }
            ReturnIdModel value = new ReturnIdModel();
            try
            {
                value.data = new _ReturnIdModel();
                ValidationModel validation = ValidationManager.CheckValidationDupicateInsertBodySet(lang, saveBodySetDTO);
                if (validation.Success == true)
                {
                    value.data = _sql.InsertBodySet(saveBodySetDTO, userID);
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

        public ReturnIdModel UpdateBodySetService(string authorization, string lang, string platform, int logID, SaveBodySetRequestDTO saveBodySetDTO, int roleID, int userID)
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
                    string TableName = "system_body_set";
                    _sql.InsertSystemLogChange(saveBodySetDTO.id, TableName, "height", saveBodySetDTO.height.ToString(), userID);
                    _sql.InsertSystemLogChange(saveBodySetDTO.id, TableName, "weight", saveBodySetDTO.weight.ToString(), userID);
                    _sql.InsertSystemLogChange(saveBodySetDTO.id, TableName, "chest", saveBodySetDTO.chest.ToString(), userID);
                    _sql.InsertSystemLogChange(saveBodySetDTO.id, TableName, "waist", saveBodySetDTO.waist.ToString(), userID);
                    _sql.InsertSystemLogChange(saveBodySetDTO.id, TableName, "hip", saveBodySetDTO.hip.ToString(), userID);

                    value.data = _sql.UpdateBodySet(saveBodySetDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateBodySetService:");
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

        public ReturnIdModel DeleteBodySetService(string authorization, string lang, string platform, int logID, SaveBodySetRequestDTO saveBodySetDTO, int roleID, int userID)
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
                    string TableName = "system_body_set";
                    _sql.InsertSystemLogChange(saveBodySetDTO.id, TableName, "status", "0", userID);
                   

                    value.data = _sql.DeleteBodySet(saveBodySetDTO, userID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "UpdateBodySetService:");
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

        public GetBodySetModel GetMasterBodySetervice(string authorization, string lang, string platform, int logID, int masterID)
        {
            if (_sql == null)
            {
                _sql = SQLManager.Instance;
            }

            GetBodySetModel value = new GetBodySetModel();
            try
            {
                BodySet data = new BodySet();

                
                ValidationModel validation = ValidationManager.CheckValidation(1, lang, platform);

                if (validation.Success == true)
                {
                    data = _sql.GetBodySet(masterID);
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
                LogManager.ServiceLog.WriteExceptionLog(ex, "GetBodySetService:");
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
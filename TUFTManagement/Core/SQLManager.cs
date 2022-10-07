﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using TUFTManagement.DTO;
using TUFTManagement.Models;
using static TUFTManagement.DTO.SaveChangeWorkShiftTimeRequestDTO;
using static TUFTManagement.DTO.SaveEmpWorkTimeRequestDTO_V1_1;
using static TUFTManagement.Models.GetAllEmployee;

namespace TUFTManagement.Core
{
    public class SQLManager
    {
        string userAgent = HttpContext.Current.Request.UserAgent.ToLower();
        string ipAddress = "";

        public static string GetIP()
        {

            var context = System.Web.HttpContext.Current;
            string ip = String.Empty;

            if (context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            else if (!String.IsNullOrWhiteSpace(context.Request.UserHostAddress))
                ip = context.Request.UserHostAddress;

            if (ip == "::1")
                ip = "127.0.0.1";

            ip = ip.Split(':')[0];

            return ip;
        }

        [ThreadStatic]
        private static SQLManager instance = null;

        private SQLManager()
        {

        }

        public static SQLManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SQLManager();
                }
                return instance;
            }
        }

        public int InsertLogReceiveData(string pServiceName, string pReceiveData, string pTimeStampNow, string pAuthorization,
            int pUserID, string pType)
        {
            int id = 0;

            ipAddress = GetIP();

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_log_receive_data @pServiceName, @pReceiveData, " +
                "@pTimeStampNow, @pAuthorization, @pUserID, " +
                "@pType, @pDeviceInfo");

            SqlParameter paramServiceName = new SqlParameter(@"pServiceName", SqlDbType.VarChar, 50);
            paramServiceName.Direction = ParameterDirection.Input;
            paramServiceName.Value = pServiceName;

            SqlParameter paramReceiveData = new SqlParameter(@"pReceiveData", SqlDbType.Text);
            paramReceiveData.Direction = ParameterDirection.Input;
            paramReceiveData.Value = pReceiveData;

            SqlParameter paramTimeStampNow = new SqlParameter(@"pTimeStampNow", SqlDbType.VarChar, 100);
            paramTimeStampNow.Direction = ParameterDirection.Input;
            paramTimeStampNow.Value = pTimeStampNow;

            SqlParameter paramAuthorization = new SqlParameter(@"pAuthorization", SqlDbType.Text);
            paramAuthorization.Direction = ParameterDirection.Input;
            paramAuthorization.Value = pAuthorization == null ? "" : pAuthorization;

            SqlParameter paramUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            paramUserID.Direction = ParameterDirection.Input;
            paramUserID.Value = pUserID;

            SqlParameter paramType = new SqlParameter(@"pType", SqlDbType.VarChar, 100);
            paramType.Direction = ParameterDirection.Input;
            paramType.Value = pType;

            SqlParameter paramDeviceInfo = new SqlParameter(@"pDeviceInfo", SqlDbType.Text);
            paramDeviceInfo.Direction = ParameterDirection.Input;
            paramDeviceInfo.Value = userAgent;

            sql.Parameters.Add(paramServiceName);
            sql.Parameters.Add(paramReceiveData);
            sql.Parameters.Add(paramTimeStampNow);
            sql.Parameters.Add(paramAuthorization);
            sql.Parameters.Add(paramUserID);
            sql.Parameters.Add(paramType);
            sql.Parameters.Add(paramDeviceInfo);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                id = int.Parse(dr["id"].ToString());
            }
            return id;
        }

        public int CheckRoleValidation(string shareCode, string projectName, string objectID, int userID)
        {
            int status = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec check_role_validation " +
                "@pUserID, " +
                "@pObjectID, " +
                "@pProjectName ");

            SqlParameter paramUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            paramUserID.Direction = ParameterDirection.Input;
            paramUserID.Value = userID;
            sql.Parameters.Add(paramUserID);

            SqlParameter paramObjectID = new SqlParameter(@"pObjectID", SqlDbType.VarChar, 255);
            paramObjectID.Direction = ParameterDirection.Input;
            paramObjectID.Value = objectID;
            sql.Parameters.Add(paramObjectID);

            SqlParameter paramProjectName = new SqlParameter(@"pProjectName", SqlDbType.VarChar, 20);
            paramProjectName.Direction = ParameterDirection.Input;
            paramProjectName.Value = projectName;
            sql.Parameters.Add(paramProjectName);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    status = int.Parse(dr["status"].ToString());
                }
            }

            return status;
        }

        public List<NewMenuList> GetMenuHimSelf(string shareCode, int userID, string lang, string projectName)
        {
            List<NewMenuList> list = new List<NewMenuList>();

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_menu_himself @pUserID, @pLang ,@pProjectName");

            SqlParameter paramUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            paramUserID.Direction = ParameterDirection.Input;
            paramUserID.Value = userID;
            sql.Parameters.Add(paramUserID);

            SqlParameter paramProjectName = new SqlParameter(@"pProjectName", SqlDbType.VarChar, 20);
            paramProjectName.Direction = ParameterDirection.Input;
            paramProjectName.Value = projectName;
            sql.Parameters.Add(paramProjectName);

            SqlParameter paramLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 5);
            paramLang.Direction = ParameterDirection.Input;
            paramLang.Value = lang;
            sql.Parameters.Add(paramLang);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            //List<NewMenuList> fieldValidate = GetMenuByParentID(302110002);

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    int is_assign = 1;
                    NewMenuList data = new NewMenuList();
                    data.loadDataMenu(row);

                    list.Add(data);

                }



            }
            return list;
        }

        public List<NewMenuList> GetMenuNoHimSelf(string shareCode, int pUserID, string lang, string projectName)
        {
            List<NewMenuList> list = new List<NewMenuList>();

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_menu_no_himself @pUserID, @pProjectName, @pLang ");

            SqlParameter paramUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            paramUserID.Direction = ParameterDirection.Input;
            paramUserID.Value = pUserID;
            sql.Parameters.Add(paramUserID);

            SqlParameter paramProjectName = new SqlParameter(@"pProjectName", SqlDbType.VarChar, 20);
            paramProjectName.Direction = ParameterDirection.Input;
            paramProjectName.Value = projectName;
            sql.Parameters.Add(paramProjectName);

            SqlParameter paramLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 5);
            paramLang.Direction = ParameterDirection.Input;
            paramLang.Value = lang;
            sql.Parameters.Add(paramLang);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {

                foreach (DataRow row in table.Rows)
                {
                    NewMenuList data = new NewMenuList();
                    data.loadDataMenu(row);

                    list.Add(data);
                }

            }
            return list;
        }

        public int InsertLogReceiveDataWithShareCode(string shareCode, string pServiceName, string pReceiveData, string pTimeStampNow, HeadersDTO headersDTO, int pUserID, string pType)
        {
            string json_header = JsonConvert.SerializeObject(headersDTO);
            InsertLogReceiveData(pServiceName, json_header, pTimeStampNow, headersDTO.authHeader, pUserID, pType);

            int id = 0;

            ipAddress = GetIP();

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_log_receive_data @pServiceName, @pReceiveData, " +
                "@pTimeStampNow, @pAuthorization, @pUserID, " +
                "@pType, @pDeviceInfo");

            SqlParameter paramServiceName = new SqlParameter(@"pServiceName", SqlDbType.VarChar, 50);
            paramServiceName.Direction = ParameterDirection.Input;
            paramServiceName.Value = pServiceName;

            SqlParameter paramReceiveData = new SqlParameter(@"pReceiveData", SqlDbType.Text);
            paramReceiveData.Direction = ParameterDirection.Input;
            paramReceiveData.Value = pReceiveData;

            SqlParameter paramTimeStampNow = new SqlParameter(@"pTimeStampNow", SqlDbType.VarChar, 100);
            paramTimeStampNow.Direction = ParameterDirection.Input;
            paramTimeStampNow.Value = pTimeStampNow;

            SqlParameter paramAuthorization = new SqlParameter(@"pAuthorization", SqlDbType.Text);
            paramAuthorization.Direction = ParameterDirection.Input;
            paramAuthorization.Value = headersDTO.authHeader == null ? "" : headersDTO.authHeader;

            SqlParameter paramUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            paramUserID.Direction = ParameterDirection.Input;
            paramUserID.Value = pUserID;

            SqlParameter paramType = new SqlParameter(@"pType", SqlDbType.VarChar, 100);
            paramType.Direction = ParameterDirection.Input;
            paramType.Value = pType;

            SqlParameter paramDeviceInfo = new SqlParameter(@"pDeviceInfo", SqlDbType.Text);
            paramDeviceInfo.Direction = ParameterDirection.Input;
            paramDeviceInfo.Value = userAgent;

            sql.Parameters.Add(paramServiceName);
            sql.Parameters.Add(paramReceiveData);
            sql.Parameters.Add(paramTimeStampNow);
            sql.Parameters.Add(paramAuthorization);
            sql.Parameters.Add(paramUserID);
            sql.Parameters.Add(paramType);
            sql.Parameters.Add(paramDeviceInfo);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                id = int.Parse(dr["id"].ToString());
            }
            return id;
        }
        public int UpdateStatusLog(int pLogID, int pStatus)
        {
            int id = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_status_log_receive_data @pLogID, @pStatus");

            SqlParameter paramLogID = new SqlParameter(@"pLogID", SqlDbType.Int);
            paramLogID.Direction = ParameterDirection.Input;
            paramLogID.Value = pLogID;

            SqlParameter paramStatus = new SqlParameter(@"pStatus", SqlDbType.Int);
            paramStatus.Direction = ParameterDirection.Input;
            paramStatus.Value = pStatus;

            sql.Parameters.Add(paramLogID);
            sql.Parameters.Add(paramStatus);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                id = int.Parse(dr["id"].ToString());
            }
            return id;
        }
        public int UpdateStatusLogWithShareCode(string shareCode, int pLogID, int pStatus)
        {
            int id = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_status_log_receive_data @pLogID, @pStatus");

            SqlParameter paramLogID = new SqlParameter(@"pLogID", SqlDbType.Int);
            paramLogID.Direction = ParameterDirection.Input;
            paramLogID.Value = pLogID;

            SqlParameter paramStatus = new SqlParameter(@"pStatus", SqlDbType.Int);
            paramStatus.Direction = ParameterDirection.Input;
            paramStatus.Value = pStatus;

            sql.Parameters.Add(paramLogID);
            sql.Parameters.Add(paramStatus);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                id = int.Parse(dr["id"].ToString());
            }
            return id;
        }

        public GetMessageTopicDTO GetMessageLang(string pLang, int pMsgCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_message_lang @pLang, @pMsgCode");

            SqlParameter paramLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 5);
            paramLang.Direction = ParameterDirection.Input;
            paramLang.Value = pLang;

            SqlParameter paramMsgCode = new SqlParameter(@"pMsgCode", SqlDbType.Int);
            paramMsgCode.Direction = ParameterDirection.Input;
            paramMsgCode.Value = pMsgCode;

            sql.Parameters.Add(paramLang);
            sql.Parameters.Add(paramMsgCode);

            table = sql.executeQueryWithReturnTable();

            GetMessageTopicDTO result = new GetMessageTopicDTO();

            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                result.message = dr["text"].ToString();
                result.topic = dr["topic"].ToString();
            }
            return result;
        }
        public GetMessageTopicDTO GetMessageLangWithShareCode(string shareCode, string pLang, int pMsgCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_message_lang @pLang, @pMsgCode");

            SqlParameter paramLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 5);
            paramLang.Direction = ParameterDirection.Input;
            paramLang.Value = pLang;

            SqlParameter paramMsgCode = new SqlParameter(@"pMsgCode", SqlDbType.Int);
            paramMsgCode.Direction = ParameterDirection.Input;
            paramMsgCode.Value = pMsgCode;

            sql.Parameters.Add(paramLang);
            sql.Parameters.Add(paramMsgCode);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            GetMessageTopicDTO result = new GetMessageTopicDTO();

            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                result.message = dr["text"].ToString();
                result.topic = dr["topic"].ToString();
            }
            return result;
        }

        public CheckUserByTokenModel CheckUserID(string pUserName, string pPassword)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec check_user_id @pUserName, @pPassword");

            SqlParameter paramUserName = new SqlParameter(@"pUserName", SqlDbType.VarChar, 150);
            paramUserName.Direction = ParameterDirection.Input;
            paramUserName.Value = pUserName;

            SqlParameter paramPassword = new SqlParameter(@"pPassword", SqlDbType.VarChar, 250);
            paramPassword.Direction = ParameterDirection.Input;
            paramPassword.Value = pPassword;

            sql.Parameters.Add(paramUserName);
            sql.Parameters.Add(paramPassword);

            table = sql.executeQueryWithReturnTable();

            CheckUserByTokenModel data = new CheckUserByTokenModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public LoginData Login(string pUserName, string pPassword, string pTokenID, string pLang)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec login @pUserName, @pPassword, @pTokenID, @pLang");

            SqlParameter paramUserName = new SqlParameter(@"pUserName", SqlDbType.VarChar, 150);
            paramUserName.Direction = ParameterDirection.Input;
            paramUserName.Value = pUserName;

            SqlParameter paramPassword = new SqlParameter(@"pPassword", SqlDbType.VarChar, 250);
            paramPassword.Direction = ParameterDirection.Input;
            paramPassword.Value = pPassword;

            SqlParameter paramTokenID = new SqlParameter(@"pTokenID", SqlDbType.VarChar);
            paramTokenID.Direction = ParameterDirection.Input;
            paramTokenID.Value = pTokenID;

            SqlParameter paramLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 5);
            paramLang.Direction = ParameterDirection.Input;
            paramLang.Value = pLang;

            sql.Parameters.Add(paramUserName);
            sql.Parameters.Add(paramPassword);
            sql.Parameters.Add(paramTokenID);
            sql.Parameters.Add(paramLang);

            table = sql.executeQueryWithReturnTable();

            LoginData data = new LoginData();

            if (table != null && table.Rows.Count > 0)
            {
                data.loadData(table.Rows[0]);
            }

            return data;
        }

        public _ReturnIdModel Logout(int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec logout " +
                "@pUserID ");

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnVerifyModel VerifyToken(string tokenID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec verify_token " +
                "@pTokenID ");

            SqlParameter pTokenID = new SqlParameter(@"pTokenID", SqlDbType.VarChar);
            pTokenID.Direction = ParameterDirection.Input;
            pTokenID.Value = tokenID;
            sql.Parameters.Add(pTokenID);

            table = sql.executeQueryWithReturnTable();

            _ReturnVerifyModel data = new _ReturnVerifyModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public List<AccessRole> GetAllAccessRole(int pRoleID)
        {
            List<AccessRole> list = new List<AccessRole>();

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_access_role @pRoleID");

            SqlParameter paramRoleID = new SqlParameter(@"pRoleID", SqlDbType.Int);
            paramRoleID.Direction = ParameterDirection.Input;
            paramRoleID.Value = pRoleID;

            sql.Parameters.Add(paramRoleID);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                AccessRole data;
                foreach (DataRow row in table.Rows)
                {
                    data = new AccessRole();
                    data.loadDataAccessRole(row);
                    list.Add(data);
                }

            }
            return list;
        }

        public List<AccessRole> GetAllAccessRoleByPosition(string shareCode, string projectName, int pUserID)
        {
            List<AccessRole> list = new List<AccessRole>();

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_access_role @pUserID, @pProjectName ");

            SqlParameter paramUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            paramUserID.Direction = ParameterDirection.Input;
            paramUserID.Value = pUserID;
            sql.Parameters.Add(paramUserID);

            SqlParameter paramProjectName = new SqlParameter(@"pProjectName", SqlDbType.VarChar, 255);
            paramProjectName.Direction = ParameterDirection.Input;
            paramProjectName.Value = projectName;
            sql.Parameters.Add(paramProjectName);

            

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                AccessRole data;
                foreach (DataRow row in table.Rows)
                {
                    data = new AccessRole();
                    data.loadDataAccessRole(row);
                    list.Add(data);
                }

            }
            return list;
        }
        
        public List<ShareHolderList> GetUserShareHolder(int pUserID, string pLang, string pFromProject)
        {
            List<ShareHolderList> list = new List<ShareHolderList>();

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_user_shareholder @pUserID, @pLang");

            SqlParameter paramUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            paramUserID.Direction = ParameterDirection.Input;
            paramUserID.Value = pUserID;

            SqlParameter paramLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 5);
            paramLang.Direction = ParameterDirection.Input;
            paramLang.Value = pLang;

            sql.Parameters.Add(paramUserID);
            sql.Parameters.Add(paramLang);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                ShareHolderList data;
                foreach (DataRow row in table.Rows)
                {
                    data = new ShareHolderList();
                    data.loadDataShareHolder(row);

                    data.agentList = new List<AgentList>();
                    data.agentList = GetUserAgent(pUserID, data.shareID, pLang);

                    data.accessList = new List<AccessRole>();
                    data.accessList = GetAllAccessRoleByPosition(data.shareCode, pFromProject, pUserID);

                    list.Add(data);
                }
            }
            return list;
        }

        public List<AgentList> GetUserAgent(int pUserID, int pShareID, string pLang)
        {
            List<AgentList> list = new List<AgentList>();

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_user_agent @pUserID, @pShareID, @pLang");

            SqlParameter paramUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            paramUserID.Direction = ParameterDirection.Input;
            paramUserID.Value = pUserID;

            SqlParameter paramShareID = new SqlParameter(@"pShareID", SqlDbType.Int);
            paramShareID.Direction = ParameterDirection.Input;
            paramShareID.Value = pShareID;

            SqlParameter paramLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 5);
            paramLang.Direction = ParameterDirection.Input;
            paramLang.Value = pLang;

            sql.Parameters.Add(paramUserID);
            sql.Parameters.Add(paramShareID);
            sql.Parameters.Add(paramLang);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                AgentList data;
                foreach (DataRow row in table.Rows)
                {
                    data = new AgentList();
                    data.loadDataAgent(row);
                    list.Add(data);
                }
            }
            return list;
        }





        public int UpdateLogReceiveDataError(int pLogID, string pErrorText)
        {
            int id = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_log_receive_data_error @pLogID, @pErrorText");

            SqlParameter paramLogID = new SqlParameter(@"pLogID", SqlDbType.Int);
            paramLogID.Direction = ParameterDirection.Input;
            paramLogID.Value = pLogID;

            SqlParameter paramErrorText = new SqlParameter(@"pErrorText", SqlDbType.Text);
            paramErrorText.Direction = ParameterDirection.Input;
            paramErrorText.Value = pErrorText;

            sql.Parameters.Add(paramLogID);
            sql.Parameters.Add(paramErrorText);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                id = int.Parse(dr["id"].ToString());
            }
            return id;
        }
        public int UpdateLogReceiveDataErrorWithShareCode(string shareCode, int pLogID, string pErrorText)
        {
            int id = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_log_receive_data_error @pLogID, @pErrorText");

            SqlParameter paramLogID = new SqlParameter(@"pLogID", SqlDbType.Int);
            paramLogID.Direction = ParameterDirection.Input;
            paramLogID.Value = pLogID;

            SqlParameter paramErrorText = new SqlParameter(@"pErrorText", SqlDbType.Text);
            paramErrorText.Direction = ParameterDirection.Input;
            paramErrorText.Value = pErrorText;

            sql.Parameters.Add(paramLogID);
            sql.Parameters.Add(paramErrorText);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                id = int.Parse(dr["id"].ToString());
            }
            return id;
        }

        public int CheckUserPassword(string pUserName, string pPassword)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec check_user_password @pUserName, @pPassword");

            SqlParameter paramUserName = new SqlParameter(@"pUserName", SqlDbType.VarChar, 100);
            paramUserName.Direction = ParameterDirection.Input;
            paramUserName.Value = pUserName;

            SqlParameter paramPassword = new SqlParameter(@"pPassword", SqlDbType.VarChar, 200);
            paramPassword.Direction = ParameterDirection.Input;
            paramPassword.Value = pPassword;

            sql.Parameters.Add(paramUserName);
            sql.Parameters.Add(paramPassword);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                total = int.Parse(dr["total"].ToString());
            }
            return total;
        }

        public int CheckBusinessByUser(string pUserName, string pBusinessCode)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec check_user_business @pUserName, @pBusinessCode");

            SqlParameter paramUserName = new SqlParameter(@"pUserName", SqlDbType.VarChar, 100);
            paramUserName.Direction = ParameterDirection.Input;
            paramUserName.Value = pUserName;

            SqlParameter paramBusinessCode = new SqlParameter(@"pBusinessCode", SqlDbType.VarChar, 10);
            paramBusinessCode.Direction = ParameterDirection.Input;
            paramBusinessCode.Value = pBusinessCode;

            sql.Parameters.Add(paramUserName);
            sql.Parameters.Add(paramBusinessCode);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                total = int.Parse(dr["total"].ToString());
            }
            return total;
        }

        public bool CheckToken(string pToken)
        {
            bool success = false;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec check_token_id @pToken");

            SqlParameter paramToken = new SqlParameter(@"pToken", SqlDbType.VarChar);
            paramToken.Direction = ParameterDirection.Input;
            paramToken.Value = pToken;

            sql.Parameters.Add(paramToken);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                success = true;
            }
            return success;
        }

        public DataTable CheckDupicateInsertEmp(string shareCode, SaveEmpProfileDTO saveEmpProfileDTO, int pUserID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec check_dupicate_employee " +
                "@pFirstNameTh, " +
                "@pLastNameTh, " +
                "@pEmpCode, " +
                "@pIdentityCard, " +
                "@pUserID ");

            SqlParameter pFirstNameTh = new SqlParameter(@"pFirstNameTh", SqlDbType.VarChar, 150);
            pFirstNameTh.Direction = ParameterDirection.Input;
            pFirstNameTh.Value = saveEmpProfileDTO.firstNameTH;
            sql.Parameters.Add(pFirstNameTh);

            SqlParameter pLastNameTh = new SqlParameter(@"pLastNameTh", SqlDbType.VarChar, 150);
            pLastNameTh.Direction = ParameterDirection.Input;
            pLastNameTh.Value = saveEmpProfileDTO.lastNameTH;
            sql.Parameters.Add(pLastNameTh);

            SqlParameter pEmpCode = new SqlParameter(@"pEmpCode", SqlDbType.VarChar, 10);
            pEmpCode.Direction = ParameterDirection.Input;
            pEmpCode.Value = saveEmpProfileDTO.userName;
            sql.Parameters.Add(pEmpCode);

            SqlParameter pIdentityCard = new SqlParameter(@"pIdentityCard", SqlDbType.VarChar, 30);
            pIdentityCard.Direction = ParameterDirection.Input;
            pIdentityCard.Value = saveEmpProfileDTO.identityCard;
            sql.Parameters.Add(pIdentityCard);

            SqlParameter paramUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            paramUserID.Direction = ParameterDirection.Input;
            paramUserID.Value = pUserID;
            sql.Parameters.Add(paramUserID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            return table;
        }
        public DataTable CheckDupicateUsername(SaveEmpProfileDTO saveEmpProfileDTO, int pUserID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec check_dupicate_username " +
                "@pUserName, " +
                "@pUserID ");

            SqlParameter pUserName = new SqlParameter(@"pUserName", SqlDbType.VarChar, 150);
            pUserName.Direction = ParameterDirection.Input;
            pUserName.Value = saveEmpProfileDTO.userName;
            sql.Parameters.Add(pUserName);

            SqlParameter paramUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            paramUserID.Direction = ParameterDirection.Input;
            paramUserID.Value = pUserID;
            sql.Parameters.Add(paramUserID);

            table = sql.executeQueryWithReturnTable();

            return table;
        }
        public DataTable CheckValidationRoleID(string ObjectID, int RoleID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec check_validation_roleid " +
                "@pObjectID, " +
                "@pRoleID");

            SqlParameter pObjectID = new SqlParameter(@"pObjectID", SqlDbType.VarChar);
            pObjectID.Direction = ParameterDirection.Input;
            pObjectID.Value = ObjectID;
            sql.Parameters.Add(pObjectID);

            SqlParameter pRoleID = new SqlParameter(@"pRoleID", SqlDbType.Int);
            pRoleID.Direction = ParameterDirection.Input;
            pRoleID.Value = RoleID;
            sql.Parameters.Add(pRoleID);

            table = sql.executeQueryWithReturnTable();

            return table;
        }
        public int insertUserLogin(SaveEmpProfileDTO saveEmpProfileDTO, int userID)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_user_login " +
                "@pUserName, " +
                "@pPositionID, " +
                "@pShareID, " +
                "@pAgentID, " +
                "@pCreateBy");

            SqlParameter pUserName = new SqlParameter(@"pUserName", SqlDbType.VarChar, 200);
            pUserName.Direction = ParameterDirection.Input;
            pUserName.Value = saveEmpProfileDTO.userName;
            sql.Parameters.Add(pUserName);

            SqlParameter pPositionID = new SqlParameter(@"pPositionID", SqlDbType.Int);
            pPositionID.Direction = ParameterDirection.Input;
            pPositionID.Value = saveEmpProfileDTO.positionID;
            sql.Parameters.Add(pPositionID);

            SqlParameter pShareID = new SqlParameter(@"pShareID", SqlDbType.Int);
            pShareID.Direction = ParameterDirection.Input;
            pShareID.Value = saveEmpProfileDTO.shareID;
            sql.Parameters.Add(pShareID);

            SqlParameter pAgentID = new SqlParameter(@"pAgentID", SqlDbType.Int);
            pAgentID.Direction = ParameterDirection.Input;
            pAgentID.Value = saveEmpProfileDTO.agentID;
            sql.Parameters.Add(pAgentID);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                total = int.Parse(dr["id"].ToString());
            }
            return total;
        }
        public int getShareIdByShareCode(string shareCode)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_share_id_by_share_code " +
                "@pShareCode");

            SqlParameter pShareCode = new SqlParameter(@"pShareCode", SqlDbType.VarChar, 50);
            pShareCode.Direction = ParameterDirection.Input;
            pShareCode.Value = shareCode;
            sql.Parameters.Add(pShareCode);
            
            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                total = int.Parse(dr["id"].ToString());
            }
            return total;
        }

        public int CheckDupEmergencyName(string shareCode, string fullName, int emergenctContactID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_user_emergency_contact " +
                "@pFullName, " +
                "@pEmergenctContactID");

            SqlParameter paramFullName = new SqlParameter(@"pFullName", SqlDbType.VarChar, 200);
            paramFullName.Direction = ParameterDirection.Input;
            paramFullName.Value = fullName;
            sql.Parameters.Add(paramFullName);

            SqlParameter paramEmergenctContactID = new SqlParameter(@"pEmergenctContactID", SqlDbType.Int);
            paramEmergenctContactID.Direction = ParameterDirection.Input;
            paramEmergenctContactID.Value = emergenctContactID;
            sql.Parameters.Add(paramEmergenctContactID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            int isDupName = 0;

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    isDupName = int.Parse(dr["is_dup_name"].ToString());
                }
            }

            return isDupName;
        }

        public _ReturnIdModel insertWorkTime(string shareCode, EmpWorkTimeRequestDTO empWorkTimeRequestDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_emp_work_time " +
                "@pUserID, " +
                "@pWorkShiftID, " +
                "@pWorkDate, " +
                "@pIsFix, " +
                "@pCreateBy");

            SqlParameter paramUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            paramUserID.Direction = ParameterDirection.Input;
            paramUserID.Value = empWorkTimeRequestDTO.userID;
            sql.Parameters.Add(paramUserID);

            SqlParameter paramWorkShiftID = new SqlParameter(@"pWorkShiftID", SqlDbType.Int);
            paramWorkShiftID.Direction = ParameterDirection.Input;
            paramWorkShiftID.Value = empWorkTimeRequestDTO.workShiftID;
            sql.Parameters.Add(paramWorkShiftID);

            SqlParameter paramWorkDate = new SqlParameter(@"pWorkDate", SqlDbType.VarChar,200);
            paramWorkDate.Direction = ParameterDirection.Input;
            paramWorkDate.Value = empWorkTimeRequestDTO.workDate;
            sql.Parameters.Add(paramWorkDate);

            SqlParameter paramIsFix = new SqlParameter(@"pIsFix", SqlDbType.Bit);
            paramIsFix.Direction = ParameterDirection.Input;
            paramIsFix.Value = empWorkTimeRequestDTO.isFix;
            sql.Parameters.Add(paramIsFix);

            SqlParameter paramCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            paramCreateBy.Direction = ParameterDirection.Input;
            paramCreateBy.Value = userID;
            sql.Parameters.Add(paramCreateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public int InsertFirstEmpWorkTimeTransChangeTrade(string shareCode, SaveChangeWorkShiftTimeRequestDTO saveTransChangeDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_emp_work_time_trans_change_trade " +
                "@pWorkTimeID, " +
                "@pUserID, " +
                "@pNewWorkShiftID, " +
                "@pTradeID, " +
                "@pRemark, " +
                "@pCreateBy");

            SqlParameter paramWorkTimeID = new SqlParameter(@"pWorkTimeID", SqlDbType.Int);
            paramWorkTimeID.Direction = ParameterDirection.Input;
            paramWorkTimeID.Value = saveTransChangeDTO.empWorkTimeID;
            sql.Parameters.Add(paramWorkTimeID);

            SqlParameter paramUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            paramUserID.Direction = ParameterDirection.Input;
            paramUserID.Value = saveTransChangeDTO.userID;
            sql.Parameters.Add(paramUserID);

            SqlParameter paramNewWorkShiftID = new SqlParameter(@"pNewWorkShiftID", SqlDbType.Int);
            paramNewWorkShiftID.Direction = ParameterDirection.Input;
            paramNewWorkShiftID.Value = saveTransChangeDTO.newWorkShiftID;
            sql.Parameters.Add(paramNewWorkShiftID);

            SqlParameter paramTradeID = new SqlParameter(@"pTradeID", SqlDbType.Int);
            paramTradeID.Direction = ParameterDirection.Input;
            paramTradeID.Value = saveTransChangeDTO.newUserID;
            sql.Parameters.Add(paramTradeID);

            SqlParameter paramRemark = new SqlParameter(@"pRemark", SqlDbType.VarChar, 255);
            paramRemark.Direction = ParameterDirection.Input;
            paramRemark.Value = saveTransChangeDTO.remark;
            sql.Parameters.Add(paramRemark);

            SqlParameter paramNewEmpWorkTimeID = new SqlParameter(@"paNewEmpWorkTimeID", SqlDbType.Int);
            paramNewEmpWorkTimeID.Direction = ParameterDirection.Input;
            paramNewEmpWorkTimeID.Value = saveTransChangeDTO.newEmpWorkTimeID;
            sql.Parameters.Add(paramNewEmpWorkTimeID);

            SqlParameter paramCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            paramCreateBy.Direction = ParameterDirection.Input;
            paramCreateBy.Value = userID;
            sql.Parameters.Add(paramCreateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();
            int id = 0;
            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                id = int.Parse(dr["id"].ToString());
            }

            return id;
        }
        public int InsertSecondEmpWorkTimeTransChangeTrade(string shareCode, SaveChangeWorkShiftTimeRequestDTO saveTransChangeDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_emp_work_time_trans_change_trade " +
                "@pWorkTimeID, " +
                "@pUserID, " +
                "@pNewWorkShiftID, " +
                "@pTradeID, " +
                "@pRemark, " +
                "@pCreateBy");

            SqlParameter paramWorkTimeID = new SqlParameter(@"pWorkTimeID", SqlDbType.Int);
            paramWorkTimeID.Direction = ParameterDirection.Input;
            paramWorkTimeID.Value = saveTransChangeDTO.newEmpWorkTimeID;
            sql.Parameters.Add(paramWorkTimeID);

            SqlParameter paramUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            paramUserID.Direction = ParameterDirection.Input;
            paramUserID.Value = saveTransChangeDTO.newUserID;
            sql.Parameters.Add(paramUserID);

            SqlParameter paramNewWorkShiftID = new SqlParameter(@"pNewWorkShiftID", SqlDbType.Int);
            paramNewWorkShiftID.Direction = ParameterDirection.Input;
            paramNewWorkShiftID.Value = saveTransChangeDTO.workShiftID;
            sql.Parameters.Add(paramNewWorkShiftID);

            SqlParameter paramTradeID = new SqlParameter(@"pTradeID", SqlDbType.Int);
            paramTradeID.Direction = ParameterDirection.Input;
            paramTradeID.Value = saveTransChangeDTO.userID;
            sql.Parameters.Add(paramTradeID);

            SqlParameter paramRemark = new SqlParameter(@"pRemark", SqlDbType.VarChar, 255);
            paramRemark.Direction = ParameterDirection.Input;
            paramRemark.Value = saveTransChangeDTO.remark;
            sql.Parameters.Add(paramRemark);

            SqlParameter paramCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            paramCreateBy.Direction = ParameterDirection.Input;
            paramCreateBy.Value = userID;
            sql.Parameters.Add(paramCreateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();
            int id = 0;
            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                id = int.Parse(dr["id"].ToString());
            }

            return id;
        }
        public int UpdateEmpWorkShif(string shareCode, SaveChangeWorkShiftTimeRequestDTO saveTransChangeDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_emp_work_time " +
                "@pId, " +
                "@pWorkShiftID, " +
                "@pUpdateBy");

            SqlParameter paramId = new SqlParameter(@"pId", SqlDbType.Int);
            paramId.Direction = ParameterDirection.Input;
            paramId.Value = saveTransChangeDTO.empWorkTimeID;
            sql.Parameters.Add(paramId);

            SqlParameter paramWorkShiftID = new SqlParameter(@"pWorkShiftID", SqlDbType.Int);
            paramWorkShiftID.Direction = ParameterDirection.Input;
            paramWorkShiftID.Value = saveTransChangeDTO.newWorkShiftID;
            sql.Parameters.Add(paramWorkShiftID);
            
            SqlParameter paramUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            paramUpdateBy.Direction = ParameterDirection.Input;
            paramUpdateBy.Value = userID;
            sql.Parameters.Add(paramUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();
            int id = 0;
            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                id = int.Parse(dr["id"].ToString());
            }

            return id;
        }
        public string ApproveEmpWorkShift(string shareCode, string transChangeList, int statusApprove, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec approve_work_shift_time " +
                "@pTranChangeList, " +
                "@pStatusApprove, " +
                "@pUpdateBy");

            SqlParameter paramTranChangeList = new SqlParameter(@"pTranChangeList", SqlDbType.VarChar, 100);
            paramTranChangeList.Direction = ParameterDirection.Input;
            paramTranChangeList.Value = transChangeList;
            sql.Parameters.Add(paramTranChangeList);

            SqlParameter paramStatusApprove = new SqlParameter(@"pStatusApprove", SqlDbType.Int);
            paramStatusApprove.Direction = ParameterDirection.Input;
            paramStatusApprove.Value = statusApprove;
            sql.Parameters.Add(paramStatusApprove);

            SqlParameter paramUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            paramUpdateBy.Direction = ParameterDirection.Input;
            paramUpdateBy.Value = userID;
            sql.Parameters.Add(paramUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));
            
            string idList = "";
            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                idList = dr["id_list"].ToString();
            }

            return idList;
        }
        public GetTransWorkShiftDTO GetTransChangeWorkShift(string shareCode, int transChangeWorkShiftID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_trans_change_work_shift " +
                "@pTransChangeWorkShiftID");

            SqlParameter paramTransChangeWorkShiftID = new SqlParameter(@"pTransChangeWorkShiftID", SqlDbType.Int);
            paramTransChangeWorkShiftID.Direction = ParameterDirection.Input;
            paramTransChangeWorkShiftID.Value = transChangeWorkShiftID;
            sql.Parameters.Add(paramTransChangeWorkShiftID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            GetTransWorkShiftDTO data = new GetTransWorkShiftDTO();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }
        public int getUserIdByEmpProfileID(string shareCode, int empProfileID)
        {
            int userID = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_user_id_by_emp_id " +
                "@pEmpProfileID");

            SqlParameter paramEmpProfileID = new SqlParameter(@"pEmpProfileID", SqlDbType.Int);
            paramEmpProfileID.Direction = ParameterDirection.Input;
            paramEmpProfileID.Value = empProfileID;
            sql.Parameters.Add(paramEmpProfileID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                userID = int.Parse(dr["user_id"].ToString());
            }
            return userID;
        }
        public InsertLogin InsertEmpProfile(string shareCode, SaveEmpProfileDTO saveEmpProfileDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_emp_profile " +
                "@pUserID, " +
                "@pUserName, " +

                "@pJoinDate, " +
                "@pMonthlySalary, " +
                "@pDailySalary, " +
                "@pDepartmentID, " +
                "@pPositionID, " +
                "@pEmploymentTypeID, " +

                "@pTitleID, " +
                "@pFirstnameTH, " +
                "@pLastnameTH, " +
                "@pNickNameTH, " +
                "@pFirstnameEN, " +
                "@pLastnameEN, " +
                "@pNickNameEN, " +
                "@pNationalityID, " +
                "@pCitizenshipID, " +
                "@pReligionID, " +
                "@pDateOfBirth, " +
                "@pIdentityCard, " +
                "@pIdentityCardExpiry, " +
                "@pHeight, " +
                "@pWeight, " +
                "@pShirtSizeID, " +
                "@pBloodTypeID, " +
                "@pPhoneNumber, " +

                "@pChest, " +
                "@pWaist, " +
                "@pHip, " +

                "@pImageProfileCode, " +
                "@pImageGalleryCode, " +
                "@pImageIdentityCode, " +

                "@pContactTypeID, " +
                "@pContactExpiryDate, " +

                "@pCreateBy ");

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = saveEmpProfileDTO.userID;
            sql.Parameters.Add(pUserID);

            SqlParameter pUserName = new SqlParameter(@"pUserName", SqlDbType.VarChar, 10);
            pUserName.Direction = ParameterDirection.Input;
            pUserName.Value = saveEmpProfileDTO.userName;
            sql.Parameters.Add(pUserName);

            SqlParameter pJoinDate = new SqlParameter(@"pJoinDate", SqlDbType.Date);
            pJoinDate.Direction = ParameterDirection.Input;
            pJoinDate.Value = saveEmpProfileDTO.joinDate;
            sql.Parameters.Add(pJoinDate);

            SqlParameter pMonthlySalary = new SqlParameter(@"pMonthlySalary", SqlDbType.Decimal);
            pMonthlySalary.Direction = ParameterDirection.Input;
            pMonthlySalary.Value = saveEmpProfileDTO.monthlySalary;
            sql.Parameters.Add(pMonthlySalary);

            SqlParameter pDailySalary = new SqlParameter(@"pDailySalary", SqlDbType.Decimal);
            pDailySalary.Direction = ParameterDirection.Input;
            pDailySalary.Value = saveEmpProfileDTO.dailySalary;
            sql.Parameters.Add(pDailySalary);

            SqlParameter pDepartmentID = new SqlParameter(@"pDepartmentID", SqlDbType.Int);
            pDepartmentID.Direction = ParameterDirection.Input;
            pDepartmentID.Value = saveEmpProfileDTO.departmentID;
            sql.Parameters.Add(pDepartmentID);

            SqlParameter pPositionID = new SqlParameter(@"pPositionID", SqlDbType.Int);
            pPositionID.Direction = ParameterDirection.Input;
            pPositionID.Value = saveEmpProfileDTO.positionID;
            sql.Parameters.Add(pPositionID);

            SqlParameter pEmploymentTypeID = new SqlParameter(@"pEmploymentTypeID", SqlDbType.Int);
            pEmploymentTypeID.Direction = ParameterDirection.Input;
            pEmploymentTypeID.Value = saveEmpProfileDTO.employmentTypeID;
            sql.Parameters.Add(pEmploymentTypeID);

            SqlParameter pTitleID = new SqlParameter(@"pTitleID", SqlDbType.Int);
            pTitleID.Direction = ParameterDirection.Input;
            pTitleID.Value = saveEmpProfileDTO.titleID;
            sql.Parameters.Add(pTitleID);

            SqlParameter pFirstnameEN = new SqlParameter(@"pFirstnameEN", SqlDbType.VarChar, 250);
            pFirstnameEN.Direction = ParameterDirection.Input;
            pFirstnameEN.Value = saveEmpProfileDTO.firstNameEN;
            sql.Parameters.Add(pFirstnameEN);

            SqlParameter pFirstnameTH = new SqlParameter(@"pFirstnameTH", SqlDbType.VarChar, 250);
            pFirstnameTH.Direction = ParameterDirection.Input;
            pFirstnameTH.Value = saveEmpProfileDTO.firstNameTH;
            sql.Parameters.Add(pFirstnameTH);

            SqlParameter pNickNameTH = new SqlParameter(@"pNickNameTH", SqlDbType.VarChar, 10);
            pNickNameTH.Direction = ParameterDirection.Input;
            pNickNameTH.Value = saveEmpProfileDTO.nickNameTH;
            sql.Parameters.Add(pNickNameTH);

            SqlParameter pLastnameEN = new SqlParameter(@"pLastnameEN", SqlDbType.VarChar, 250);
            pLastnameEN.Direction = ParameterDirection.Input;
            pLastnameEN.Value = saveEmpProfileDTO.lastNameEN;
            sql.Parameters.Add(pLastnameEN);

            SqlParameter pLastnameTH = new SqlParameter(@"pLastnameTH", SqlDbType.VarChar, 250);
            pLastnameTH.Direction = ParameterDirection.Input;
            pLastnameTH.Value = saveEmpProfileDTO.lastNameTH;
            sql.Parameters.Add(pLastnameTH);

            SqlParameter pNickNameEN = new SqlParameter(@"pNickNameEN", SqlDbType.VarChar, 10);
            pNickNameEN.Direction = ParameterDirection.Input;
            pNickNameEN.Value = saveEmpProfileDTO.nickNameEN;
            sql.Parameters.Add(pNickNameEN);

            SqlParameter pNationalityID = new SqlParameter(@"pNationalityID", SqlDbType.Int);
            pNationalityID.Direction = ParameterDirection.Input;
            pNationalityID.Value = saveEmpProfileDTO.nationalityID;
            sql.Parameters.Add(pNationalityID);

            SqlParameter pCitizenshipID = new SqlParameter(@"pCitizenshipID", SqlDbType.Int);
            pCitizenshipID.Direction = ParameterDirection.Input;
            pCitizenshipID.Value = saveEmpProfileDTO.citizenshipID;
            sql.Parameters.Add(pCitizenshipID);

            SqlParameter pReligionID = new SqlParameter(@"pReligionID", SqlDbType.Int);
            pReligionID.Direction = ParameterDirection.Input;
            pReligionID.Value = saveEmpProfileDTO.religionID;
            sql.Parameters.Add(pReligionID);

            SqlParameter pDateOfBirth = new SqlParameter(@"pDateOfBirth", SqlDbType.Date);
            pDateOfBirth.Direction = ParameterDirection.Input;
            pDateOfBirth.Value = saveEmpProfileDTO.dateOfBirth;
            sql.Parameters.Add(pDateOfBirth);

            SqlParameter pIdentityCard = new SqlParameter(@"pIdentityCard", SqlDbType.VarChar, 30);
            pIdentityCard.Direction = ParameterDirection.Input;
            pIdentityCard.Value = saveEmpProfileDTO.identityCard;
            sql.Parameters.Add(pIdentityCard);

            SqlParameter pIdentityCardExpiry = new SqlParameter(@"pIdentityCardExpiry", SqlDbType.VarChar, 30);
            pIdentityCardExpiry.Direction = ParameterDirection.Input;
            pIdentityCardExpiry.Value = saveEmpProfileDTO.identityCardExpiry;
            sql.Parameters.Add(pIdentityCardExpiry);

            SqlParameter pHeight = new SqlParameter(@"pHeight", SqlDbType.Decimal);
            pHeight.Direction = ParameterDirection.Input;
            pHeight.Value = saveEmpProfileDTO.height;
            sql.Parameters.Add(pHeight);

            SqlParameter pWeight = new SqlParameter(@"pWeight", SqlDbType.Decimal);
            pWeight.Direction = ParameterDirection.Input;
            pWeight.Value = saveEmpProfileDTO.weight;
            sql.Parameters.Add(pWeight);

            SqlParameter pShirtSizeID = new SqlParameter(@"pShirtSizeID", SqlDbType.Int);
            pShirtSizeID.Direction = ParameterDirection.Input;
            pShirtSizeID.Value = saveEmpProfileDTO.shirtSizeID;
            sql.Parameters.Add(pShirtSizeID);

            SqlParameter pBloodTypeID = new SqlParameter(@"pBloodTypeID", SqlDbType.Int);
            pBloodTypeID.Direction = ParameterDirection.Input;
            pBloodTypeID.Value = saveEmpProfileDTO.bloodTypeID;
            sql.Parameters.Add(pBloodTypeID);

            SqlParameter pPhoneNumber = new SqlParameter(@"pPhoneNumber", SqlDbType.VarChar, 15);
            pPhoneNumber.Direction = ParameterDirection.Input;
            pPhoneNumber.Value = saveEmpProfileDTO.phoneNumber;
            sql.Parameters.Add(pPhoneNumber);

            SqlParameter pChest = new SqlParameter(@"pChest", SqlDbType.Decimal);
            pChest.Direction = ParameterDirection.Input;
            pChest.Value = saveEmpProfileDTO.chest;
            sql.Parameters.Add(pChest);

            SqlParameter pWaist = new SqlParameter(@"pWaist", SqlDbType.Decimal);
            pWaist.Direction = ParameterDirection.Input;
            pWaist.Value = saveEmpProfileDTO.waist;
            sql.Parameters.Add(pWaist);

            SqlParameter pHip = new SqlParameter(@"pHip", SqlDbType.Decimal);
            pHip.Direction = ParameterDirection.Input;
            pHip.Value = saveEmpProfileDTO.hip;
            sql.Parameters.Add(pHip);

            SqlParameter pImageProfileCode = new SqlParameter(@"pImageProfileCode", SqlDbType.VarChar, 250);
            pImageProfileCode.Direction = ParameterDirection.Input;
            pImageProfileCode.Value = saveEmpProfileDTO.imageProfileCode;
            sql.Parameters.Add(pImageProfileCode);

            SqlParameter pImageGalleryCode = new SqlParameter(@"pImageGalleryCode", SqlDbType.VarChar, 250);
            pImageGalleryCode.Direction = ParameterDirection.Input;
            pImageGalleryCode.Value = saveEmpProfileDTO.imageGalleryCode;
            sql.Parameters.Add(pImageGalleryCode);

            SqlParameter pImageIdentityCode = new SqlParameter(@"pImageIdentityCode", SqlDbType.VarChar, 250);
            pImageIdentityCode.Direction = ParameterDirection.Input;
            pImageIdentityCode.Value = saveEmpProfileDTO.imageIdentityCode;
            sql.Parameters.Add(pImageIdentityCode);

            SqlParameter pContactTypeID = new SqlParameter(@"pContactTypeID", SqlDbType.Int);
            pContactTypeID.Direction = ParameterDirection.Input;
            pContactTypeID.Value = saveEmpProfileDTO.contractTypeID;
            sql.Parameters.Add(pContactTypeID);

            SqlParameter pContactExpiryDate = new SqlParameter(@"pContactExpiryDate", SqlDbType.VarChar, 10);
            pContactExpiryDate.Direction = ParameterDirection.Input;
            pContactExpiryDate.Value = saveEmpProfileDTO.contractExpiryDate;
            sql.Parameters.Add(pContactExpiryDate);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            InsertLogin data = new InsertLogin();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public InsertLogin InsertEmpAddress(string shareCode, SaveEmpProfileDTO saveEmpProfileDTO, int tokenUserID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_emp_address " +
                "@pUserID, " +
                "@pCAddress, " +
                "@pCCountryID, " +
                "@pCSubDistrictID, " +
                "@pCDistrictID, " +
                "@pCProvinceID, " +
                "@pCZipcode, " +
                "@pCPhoneContact, " +
                "@pIsSamePermanentAddress, " +
                "@pPAddress, " +
                "@pPCountryID, " +
                "@pPSubDistrictID, " +
                "@pPDistrictID, " +
                "@pPProvinceID, " +
                "@pPZipcode, " +
                "@pPPhoneContact, " +
                "@pCreateBy ");

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = saveEmpProfileDTO.userID;
            sql.Parameters.Add(pUserID);

            SqlParameter pCAddress = new SqlParameter(@"pCAddress", SqlDbType.VarChar, 200);
            pCAddress.Direction = ParameterDirection.Input;
            pCAddress.Value = saveEmpProfileDTO.cAddress;
            sql.Parameters.Add(pCAddress);

            SqlParameter pCCountryID = new SqlParameter(@"pCCountryID", SqlDbType.Int);
            pCCountryID.Direction = ParameterDirection.Input;
            pCCountryID.Value = saveEmpProfileDTO.cCountryID;
            sql.Parameters.Add(pCCountryID);

            SqlParameter pCSubDistrictID = new SqlParameter(@"pCSubDistrictID", SqlDbType.Int);
            pCSubDistrictID.Direction = ParameterDirection.Input;
            pCSubDistrictID.Value = saveEmpProfileDTO.cSubDistrictID;
            sql.Parameters.Add(pCSubDistrictID);

            SqlParameter pCDistrictID = new SqlParameter(@"pCDistrictID", SqlDbType.Int);
            pCDistrictID.Direction = ParameterDirection.Input;
            pCDistrictID.Value = saveEmpProfileDTO.cDistrictID;
            sql.Parameters.Add(pCDistrictID);

            SqlParameter pCProvinceID = new SqlParameter(@"pCProvinceID", SqlDbType.Int);
            pCProvinceID.Direction = ParameterDirection.Input;
            pCProvinceID.Value = saveEmpProfileDTO.cProvinceID;
            sql.Parameters.Add(pCProvinceID);

            SqlParameter pCZipcode = new SqlParameter(@"pCZipcode", SqlDbType.VarChar, 10);
            pCZipcode.Direction = ParameterDirection.Input;
            pCZipcode.Value = saveEmpProfileDTO.cZipcode;
            sql.Parameters.Add(pCZipcode);

            SqlParameter pCPhoneContact = new SqlParameter(@"pCPhoneContact", SqlDbType.VarChar, 20);
            pCPhoneContact.Direction = ParameterDirection.Input;
            pCPhoneContact.Value = saveEmpProfileDTO.cPhoneContact;
            sql.Parameters.Add(pCPhoneContact);

            SqlParameter pIsSamePermanentAddress = new SqlParameter(@"pIsSamePermanentAddress", SqlDbType.Int);
            pIsSamePermanentAddress.Direction = ParameterDirection.Input;
            pIsSamePermanentAddress.Value = saveEmpProfileDTO.isSamePermanentAddress;
            sql.Parameters.Add(pIsSamePermanentAddress);

            SqlParameter pPAddress = new SqlParameter(@"pPAddress", SqlDbType.VarChar, 200);
            pPAddress.Direction = ParameterDirection.Input;
            pPAddress.Value = saveEmpProfileDTO.pAddress;
            sql.Parameters.Add(pPAddress);

            SqlParameter pPCountryID = new SqlParameter(@"pPCountryID", SqlDbType.Int);
            pPCountryID.Direction = ParameterDirection.Input;
            pPCountryID.Value = saveEmpProfileDTO.pCountryID;
            sql.Parameters.Add(pPCountryID);

            SqlParameter pPSubDistrictID = new SqlParameter(@"pPSubDistrictID", SqlDbType.Int);
            pPSubDistrictID.Direction = ParameterDirection.Input;
            pPSubDistrictID.Value = saveEmpProfileDTO.pSubDistrictID;
            sql.Parameters.Add(pPSubDistrictID);

            SqlParameter pPDistrictID = new SqlParameter(@"pPDistrictID", SqlDbType.Int);
            pPDistrictID.Direction = ParameterDirection.Input;
            pPDistrictID.Value = saveEmpProfileDTO.pDistrictID;
            sql.Parameters.Add(pPDistrictID);

            SqlParameter pPProvinceID = new SqlParameter(@"pPProvinceID", SqlDbType.Int);
            pPProvinceID.Direction = ParameterDirection.Input;
            pPProvinceID.Value = saveEmpProfileDTO.pProvinceID;
            sql.Parameters.Add(pPProvinceID);

            SqlParameter pPZipcode = new SqlParameter(@"pPZipcode", SqlDbType.VarChar, 10);
            pPZipcode.Direction = ParameterDirection.Input;
            pPZipcode.Value = saveEmpProfileDTO.pZipcode;
            sql.Parameters.Add(pPZipcode);

            SqlParameter pPPhoneContact = new SqlParameter(@"pPPhoneContact", SqlDbType.VarChar, 20);
            pPPhoneContact.Direction = ParameterDirection.Input;
            pPPhoneContact.Value = saveEmpProfileDTO.pPhoneContact;
            sql.Parameters.Add(pPPhoneContact);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = tokenUserID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            InsertLogin data = new InsertLogin();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }
        public InsertLogin InsertEmpBankAccount(string shareCode, SaveEmpProfileDTO saveEmpProfileDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_emp_bank_account " +
                "@pUserID, " +
                "@pBankID, " +
                "@pBankAccountNo, " +
                "@pBankAccountName, " +
                "@pCreateBy ");

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = saveEmpProfileDTO.userID;
            sql.Parameters.Add(pUserID);

            SqlParameter pBankID = new SqlParameter(@"pBankID", SqlDbType.Int);
            pBankID.Direction = ParameterDirection.Input;
            pBankID.Value = saveEmpProfileDTO.bankID;
            sql.Parameters.Add(pBankID);

            SqlParameter pBankAccountNo = new SqlParameter(@"pBankAccountNo", SqlDbType.VarChar, 50);
            pBankAccountNo.Direction = ParameterDirection.Input;
            pBankAccountNo.Value = saveEmpProfileDTO.bankAccountNumber;
            sql.Parameters.Add(pBankAccountNo);

            SqlParameter pBankAccountName = new SqlParameter(@"pBankAccountName", SqlDbType.VarChar, 250);
            pBankAccountName.Direction = ParameterDirection.Input;
            pBankAccountName.Value = saveEmpProfileDTO.bankAccountName;
            sql.Parameters.Add(pBankAccountName);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            InsertLogin data = new InsertLogin();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateEmpBankAccount(string shareCode, SaveEmpProfileDTO saveEmpProfileDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_emp_bank_account " +
                "@pUserID, " +
                "@pBankID, " +
                "@pBankAccountNo, " +
                "@pBankAccountName, " +
                "@pUpdateBy");

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = saveEmpProfileDTO.userID;
            sql.Parameters.Add(pUserID);

            SqlParameter pBankID = new SqlParameter(@"pBankID", SqlDbType.Int);
            pBankID.Direction = ParameterDirection.Input;
            pBankID.Value = saveEmpProfileDTO.bankID;
            sql.Parameters.Add(pBankID);

            SqlParameter pBankAccountNo = new SqlParameter(@"pBankAccountNo", SqlDbType.VarChar, 50);
            pBankAccountNo.Direction = ParameterDirection.Input;
            pBankAccountNo.Value = saveEmpProfileDTO.bankAccountNumber;
            sql.Parameters.Add(pBankAccountNo);

            SqlParameter pBankAccountName = new SqlParameter(@"pBankAccountName", SqlDbType.VarChar, 250);
            pBankAccountName.Direction = ParameterDirection.Input;
            pBankAccountName.Value = saveEmpProfileDTO.bankAccountName;
            sql.Parameters.Add(pBankAccountName);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public InsertLogin InsertEmpEmergencyContact(string shareCode, SaveEmergencyContact saveEmergencyContact, int targetUserID, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_emp_emergency_contact " +
                "@pUserID, " +
                "@pEmerFullName, " +
                "@pEmerRelationShipID, " +
                "@pEmerContact, " +
                "@pCreateBy ");

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = targetUserID;
            sql.Parameters.Add(pUserID);

            SqlParameter pEmerFullName = new SqlParameter(@"pEmerFullName", SqlDbType.VarChar, 250);
            pEmerFullName.Direction = ParameterDirection.Input;
            pEmerFullName.Value = saveEmergencyContact.emerFullName;
            sql.Parameters.Add(pEmerFullName);

            SqlParameter pEmerRelationShipID = new SqlParameter(@"pEmerRelationShipID", SqlDbType.Int);
            pEmerRelationShipID.Direction = ParameterDirection.Input;
            pEmerRelationShipID.Value = saveEmergencyContact.emerRelationShipID;
            sql.Parameters.Add(pEmerRelationShipID);

            SqlParameter pEmerContact = new SqlParameter(@"pEmerContact", SqlDbType.VarChar, 15);
            pEmerContact.Direction = ParameterDirection.Input;
            pEmerContact.Value = saveEmergencyContact.emerContact;
            sql.Parameters.Add(pEmerContact);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            InsertLogin data = new InsertLogin();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }
        public _ReturnIdModel UpdateEmpEmergencyContact(string shareCode, SaveEmergencyContact saveEmergencyContact, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_emp_emergency_contact " +
                "@pEmergencyContactID, " +
                "@pEmerFullName, " +
                "@pEmerRelationShipID, " +
                "@pEmerContact, " +
                "@pUpdateBy");

            SqlParameter pEmergencyContact = new SqlParameter(@"pEmergencyContactID", SqlDbType.Int);
            pEmergencyContact.Direction = ParameterDirection.Input;
            pEmergencyContact.Value = saveEmergencyContact.emerContactID;
            sql.Parameters.Add(pEmergencyContact);

            SqlParameter pEmerFullName = new SqlParameter(@"pEmerFullName", SqlDbType.VarChar, 250);
            pEmerFullName.Direction = ParameterDirection.Input;
            pEmerFullName.Value = saveEmergencyContact.emerFullName;
            sql.Parameters.Add(pEmerFullName);

            SqlParameter pEmerRelationShipID = new SqlParameter(@"pEmerRelationShipID", SqlDbType.Int);
            pEmerRelationShipID.Direction = ParameterDirection.Input;
            pEmerRelationShipID.Value = saveEmergencyContact.emerRelationShipID;
            sql.Parameters.Add(pEmerRelationShipID);

            SqlParameter pEmerContact = new SqlParameter(@"pEmerContact", SqlDbType.VarChar, 15);
            pEmerContact.Direction = ParameterDirection.Input;
            pEmerContact.Value = saveEmergencyContact.emerContact;
            sql.Parameters.Add(pEmerContact);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }
        public _ReturnIdModel InsertEmpRate(string shareCode, SaveEmpRateRequestDTO saveEmpRateDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_emp_rate " +
                "@pEmpID, " +
                "@pServiceNo, " +
                "@pProductGrade, " +
                "@pStartDrink, " +
                "@pFullDrink, " +
                "@pRateStaff, " +
                "@pRateManager," +
                "@pRateOwner, " +
                "@pRateConfirm, " +
                "@pCreateBy");

            SqlParameter paramEmpID = new SqlParameter(@"pEmpID", SqlDbType.Int);
            paramEmpID.Direction = ParameterDirection.Input;
            paramEmpID.Value = saveEmpRateDTO.empID;
            sql.Parameters.Add(paramEmpID);

            SqlParameter paramServiceNo = new SqlParameter(@"pServiceNo", SqlDbType.Int);
            paramServiceNo.Direction = ParameterDirection.Input;
            paramServiceNo.Value = saveEmpRateDTO.serviceNo;
            sql.Parameters.Add(paramServiceNo);

            SqlParameter paramProductGrade = new SqlParameter(@"pProductGrade", SqlDbType.Int);
            paramProductGrade.Direction = ParameterDirection.Input;
            paramProductGrade.Value = saveEmpRateDTO.productGrade;
            sql.Parameters.Add(paramProductGrade);

            SqlParameter paramStartDrink = new SqlParameter(@"pStartDrink", SqlDbType.Int);
            paramStartDrink.Direction = ParameterDirection.Input;
            paramStartDrink.Value = saveEmpRateDTO.startDrink;
            sql.Parameters.Add(paramStartDrink);

            SqlParameter paramFullDrink = new SqlParameter(@"pFullDrink", SqlDbType.Int);
            paramFullDrink.Direction = ParameterDirection.Input;
            paramFullDrink.Value = saveEmpRateDTO.fullDrink;
            sql.Parameters.Add(paramFullDrink);

            SqlParameter paramRateStaff = new SqlParameter(@"pRateStaff", SqlDbType.Float);
            paramRateStaff.Direction = ParameterDirection.Input;
            paramRateStaff.Value = saveEmpRateDTO.rateStaff;
            sql.Parameters.Add(paramRateStaff);

            SqlParameter paramRateManager = new SqlParameter(@"pRateManager", SqlDbType.Float);
            paramRateManager.Direction = ParameterDirection.Input;
            paramRateManager.Value = saveEmpRateDTO.rateManager;
            sql.Parameters.Add(paramRateManager);

            SqlParameter paramRateOwner = new SqlParameter(@"pRateOwner", SqlDbType.Float);
            paramRateOwner.Direction = ParameterDirection.Input;
            paramRateOwner.Value = saveEmpRateDTO.rateOwner;
            sql.Parameters.Add(paramRateOwner);

            SqlParameter paramRateConfirm = new SqlParameter(@"pRateConfirm", SqlDbType.Float);
            paramRateConfirm.Direction = ParameterDirection.Input;
            paramRateConfirm.Value = saveEmpRateDTO.rateConfirm;
            sql.Parameters.Add(paramRateConfirm);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel InsertEmpWorkShift(string shareCode, SaveEmpWorkShiftRequestDTO saveEmpWorkShiftRequestDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_emp_work_shift " +
                "@pWsCode, " +
                "@pTimeStart, " +
                "@pTimeEnd, " +
                "@pWorkTypeID," +
                "@pRemark," +
                "@pStatus," +
                "@pCreateBy");

            SqlParameter paramWsCode = new SqlParameter(@"pWsCode", SqlDbType.VarChar, 50);
            paramWsCode.Direction = ParameterDirection.Input;
            paramWsCode.Value = saveEmpWorkShiftRequestDTO.wsCode;
            sql.Parameters.Add(paramWsCode);

            SqlParameter paramTimeStart = new SqlParameter(@"pTimeStart", SqlDbType.VarChar, 100);
            paramTimeStart.Direction = ParameterDirection.Input;
            paramTimeStart.Value = saveEmpWorkShiftRequestDTO.timeStart;
            sql.Parameters.Add(paramTimeStart);

            SqlParameter paramTimeEnd = new SqlParameter(@"pTimeEnd", SqlDbType.VarChar, 100);
            paramTimeEnd.Direction = ParameterDirection.Input;
            paramTimeEnd.Value = saveEmpWorkShiftRequestDTO.timeEnd;
            sql.Parameters.Add(paramTimeEnd);

            SqlParameter paramWorkTypeID = new SqlParameter(@"pWorkTypeID", SqlDbType.Int);
            paramWorkTypeID.Direction = ParameterDirection.Input;
            paramWorkTypeID.Value = saveEmpWorkShiftRequestDTO.workTypeID;
            sql.Parameters.Add(paramWorkTypeID);

            SqlParameter paramRemark = new SqlParameter(@"pRemark", SqlDbType.VarChar, 200);
            paramRemark.Direction = ParameterDirection.Input;
            paramRemark.Value = saveEmpWorkShiftRequestDTO.remark;
            sql.Parameters.Add(paramRemark);

            SqlParameter paramStatus = new SqlParameter(@"pStatus", SqlDbType.Int);
            paramStatus.Direction = ParameterDirection.Input;
            paramStatus.Value = saveEmpWorkShiftRequestDTO.status;
            sql.Parameters.Add(paramStatus);


            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        #region work time

        public GetEmpWorkTime GetEmpWorkTime(int empWorkTimeID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_emp_work_time " +
                "@pID");

            SqlParameter paramID = new SqlParameter(@"pID", SqlDbType.Int);
            paramID.Direction = ParameterDirection.Input;
            paramID.Value = empWorkTimeID;
            sql.Parameters.Add(paramID);

            table = sql.executeQueryWithReturnTable();

            GetEmpWorkTime data = new GetEmpWorkTime();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }
        public _ReturnIdModel UpdateEmpWorkTime(SaveEmpWorkTimeRequestDTO saveEmpWorkTimeRequestDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_emp_work_time " +
                "@pId, " +
                "@pWorkShiftID, " +
                "@pCreateBy");

            SqlParameter paramId = new SqlParameter(@"pId", SqlDbType.Int);
            paramId.Direction = ParameterDirection.Input;
            paramId.Value = saveEmpWorkTimeRequestDTO.empWorkTimeID;
            sql.Parameters.Add(paramId);

            SqlParameter paramWorkShiftID = new SqlParameter(@"pWorkShiftID", SqlDbType.Int);
            paramWorkShiftID.Direction = ParameterDirection.Input;
            paramWorkShiftID.Value = saveEmpWorkTimeRequestDTO.empWorkShiftID;
            sql.Parameters.Add(paramWorkShiftID);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }
        public _ReturnIdModel UpdateEmpWorkTime_WorkIn(SaveEmpWorkTimeRequestDTO saveEmpWorkTimeRequestDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_emp_work_time_work_in " +
                "@pId, " +
                "@pWorkIn, " +
                "@pCreateBy");

            SqlParameter paramId = new SqlParameter(@"pId", SqlDbType.Int);
            paramId.Direction = ParameterDirection.Input;
            paramId.Value = saveEmpWorkTimeRequestDTO.empWorkTimeID;
            sql.Parameters.Add(paramId);

            SqlParameter paramWorkIn = new SqlParameter(@"pWorkIn", SqlDbType.Time);
            paramWorkIn.Direction = ParameterDirection.Input;
            paramWorkIn.Value = saveEmpWorkTimeRequestDTO.workIn;
            sql.Parameters.Add(paramWorkIn);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }
        public _ReturnIdModel UpdateEmpWorkTime_WorkOut(SaveEmpWorkTimeRequestDTO saveEmpWorkTimeRequestDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_emp_work_time_work_out " +
                "@pId, " +
                "@pWorkOut, " +
                "@pCreateBy");

            SqlParameter paramId = new SqlParameter(@"pId", SqlDbType.Int);
            paramId.Direction = ParameterDirection.Input;
            paramId.Value = saveEmpWorkTimeRequestDTO.empWorkTimeID;
            sql.Parameters.Add(paramId);

            SqlParameter paramWorkOut = new SqlParameter(@"pWorkOut", SqlDbType.Time);
            paramWorkOut.Direction = ParameterDirection.Input;
            paramWorkOut.Value = saveEmpWorkTimeRequestDTO.workOut;
            sql.Parameters.Add(paramWorkOut);

            //SqlParameter paramFloorIn = new SqlParameter(@"pFloorIn", SqlDbType.Time);
            //paramFloorIn.Direction = ParameterDirection.Input;
            //paramFloorIn.Value = saveEmpWorkTimeRequestDTO.floorIn;
            //sql.Parameters.Add(paramFloorIn);

            //SqlParameter paramFloorOut = new SqlParameter(@"pFloorOut", SqlDbType.Time);
            //paramFloorOut.Direction = ParameterDirection.Input;
            //paramFloorOut.Value = saveEmpWorkTimeRequestDTO.floorOut;
            //sql.Parameters.Add(paramFloorOut);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }
        public _ReturnIdModel UpdateEmpWorkTime_FloorIn(SaveEmpWorkTimeRequestDTO saveEmpWorkTimeRequestDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_emp_work_time_floor_in " +
                "@pId, " +
                "@pFloorIn, " +
                "@pCreateBy");

            SqlParameter paramId = new SqlParameter(@"pId", SqlDbType.Int);
            paramId.Direction = ParameterDirection.Input;
            paramId.Value = saveEmpWorkTimeRequestDTO.empWorkTimeID;
            sql.Parameters.Add(paramId);

            SqlParameter paramFloorIn = new SqlParameter(@"pFloorIn", SqlDbType.Time);
            paramFloorIn.Direction = ParameterDirection.Input;
            paramFloorIn.Value = saveEmpWorkTimeRequestDTO.floorIn;
            sql.Parameters.Add(paramFloorIn);

            //SqlParameter paramFloorOut = new SqlParameter(@"pFloorOut", SqlDbType.Time);
            //paramFloorOut.Direction = ParameterDirection.Input;
            //paramFloorOut.Value = saveEmpWorkTimeRequestDTO.floorOut;
            //sql.Parameters.Add(paramFloorOut);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }
        public _ReturnIdModel UpdateEmpWorkTime_FloorOut(SaveEmpWorkTimeRequestDTO saveEmpWorkTimeRequestDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_emp_work_time_floor_in " +
                "@pId, " +
                "@pFloorOut, " +
                "@pCreateBy");

            SqlParameter paramId = new SqlParameter(@"pId", SqlDbType.Int);
            paramId.Direction = ParameterDirection.Input;
            paramId.Value = saveEmpWorkTimeRequestDTO.empWorkTimeID;
            sql.Parameters.Add(paramId);

            SqlParameter paramFloorOut = new SqlParameter(@"pFloorOut", SqlDbType.Time);
            paramFloorOut.Direction = ParameterDirection.Input;
            paramFloorOut.Value = saveEmpWorkTimeRequestDTO.floorOut;
            sql.Parameters.Add(paramFloorOut);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }
        public _ReturnIdModel InsertTransChange_WorkShift(SaveEmpWorkTimeTransChangeRequestDTO saveEmpWorkTimeTransChangeRequestDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_emp_work_time_trans_change " +
                "@pWorkTimeID, " +
                "@pUserID, " +
                "@pOldWorkShiftID, " +
                "@pNewWorkShiftID, " +
                "@pRemark, " +
                "@pCreateBy");

            SqlParameter paramWorkTimeID = new SqlParameter(@"pWorkTimeID", SqlDbType.Int);
            paramWorkTimeID.Direction = ParameterDirection.Input;
            paramWorkTimeID.Value = saveEmpWorkTimeTransChangeRequestDTO.empWorkTimeID;
            sql.Parameters.Add(paramWorkTimeID);

            SqlParameter paramUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            paramUserID.Direction = ParameterDirection.Input;
            paramUserID.Value = saveEmpWorkTimeTransChangeRequestDTO.userID;
            sql.Parameters.Add(paramUserID);

            SqlParameter paramOldWorkShiftID = new SqlParameter(@"pOldWorkShiftID", SqlDbType.Int);
            paramOldWorkShiftID.Direction = ParameterDirection.Input;
            paramOldWorkShiftID.Value = saveEmpWorkTimeTransChangeRequestDTO.OldEmpWorkShiftID;
            sql.Parameters.Add(paramOldWorkShiftID);

            SqlParameter paramNewWorkShiftID = new SqlParameter(@"pNewWorkShiftID", SqlDbType.Int);
            paramNewWorkShiftID.Direction = ParameterDirection.Input;
            paramNewWorkShiftID.Value = saveEmpWorkTimeTransChangeRequestDTO.NewEmpWorkShiftID;
            sql.Parameters.Add(paramNewWorkShiftID);

            SqlParameter paramRemark = new SqlParameter(@"pRemark", SqlDbType.VarChar, 255);
            paramRemark.Direction = ParameterDirection.Input;
            paramRemark.Value = saveEmpWorkTimeTransChangeRequestDTO.remark;
            sql.Parameters.Add(paramRemark);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }
        public _ReturnIdModel ApproveTransChange_WorkShift(SaveWorkTimeTransChangeRequestDTO transChangeRequestDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec approve_emp_work_time_trans_change " +
                "@pId, " +
                "@pStatusApprove, " +
                "@pRemark, " +
                "@pApproveBy");

            SqlParameter paramId = new SqlParameter(@"pId", SqlDbType.Int);
            paramId.Direction = ParameterDirection.Input;
            paramId.Value = transChangeRequestDTO.transChangeID;
            sql.Parameters.Add(paramId);

            SqlParameter paramStatusApprove = new SqlParameter(@"pStatusApprove", SqlDbType.Int);
            paramStatusApprove.Direction = ParameterDirection.Input;
            paramStatusApprove.Value = transChangeRequestDTO.statusApprove;
            sql.Parameters.Add(paramStatusApprove);

            SqlParameter paramRemark = new SqlParameter(@"pRemark", SqlDbType.VarChar, 255);
            paramRemark.Direction = ParameterDirection.Input;
            paramRemark.Value = transChangeRequestDTO.remark;
            sql.Parameters.Add(paramRemark);

            SqlParameter paramApproveBy = new SqlParameter(@"pApproveBy", SqlDbType.Int);
            paramApproveBy.Direction = ParameterDirection.Input;
            paramApproveBy.Value = userID;
            sql.Parameters.Add(paramApproveBy);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }
        public int CheckTransChange(int transChangeID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec check_transchange_by_id " +
                "@pID");

            SqlParameter paramID = new SqlParameter(@"pID", SqlDbType.Int);
            paramID.Direction = ParameterDirection.Input;
            paramID.Value = transChangeID;
            sql.Parameters.Add(paramID);

            table = sql.executeQueryWithReturnTable();

            int total = 0;

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }
        public _ReturnIdModel UpdateEmpWorkTimeNewVer(SaveEmpWorkTimeRequestDTO saveEmpWorkTimeRequestDTO, int userID, string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_emp_work_time " +
                "@pId, " +
                "@pWorkShiftID, " +
                "@pCreateBy");

            SqlParameter paramId = new SqlParameter(@"pId", SqlDbType.Int);
            paramId.Direction = ParameterDirection.Input;
            paramId.Value = saveEmpWorkTimeRequestDTO.empWorkTimeID;
            sql.Parameters.Add(paramId);

            SqlParameter paramWorkShiftID = new SqlParameter(@"pWorkShiftID", SqlDbType.Int);
            paramWorkShiftID.Direction = ParameterDirection.Input;
            paramWorkShiftID.Value = saveEmpWorkTimeRequestDTO.empWorkShiftID;
            sql.Parameters.Add(paramWorkShiftID);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }
        public GetEmpWorkTime GetEmpWorkTimeNewVer(int empWorkTimeID, string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_emp_work_time " +
                "@pID");

            SqlParameter paramID = new SqlParameter(@"pID", SqlDbType.Int);
            paramID.Direction = ParameterDirection.Input;
            paramID.Value = empWorkTimeID;
            sql.Parameters.Add(paramID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            GetEmpWorkTime data = new GetEmpWorkTime();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel InsertEmpWorkTimeTran(SaveEmpWorkTimeRequestDTO saveEmpWorkTimeRequestDTO, int userID, string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_tran_change_work_shift " +
                "@pUserID, " +
                "@pWorkTimeID, " +
                "@pWorkShiftNew, " +
                "@pReMark, " +
                "@pCreateBy");

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = saveEmpWorkTimeRequestDTO.empID;
            sql.Parameters.Add(pUserID);

            SqlParameter pWorkTimeID = new SqlParameter(@"pWorkTimeID", SqlDbType.Int);
            pWorkTimeID.Direction = ParameterDirection.Input;
            pWorkTimeID.Value = saveEmpWorkTimeRequestDTO.empWorkTimeID;
            sql.Parameters.Add(pWorkTimeID);

            SqlParameter pWorkShiftNew = new SqlParameter(@"pWorkShiftNew", SqlDbType.Int);
            pWorkShiftNew.Direction = ParameterDirection.Input;
            pWorkShiftNew.Value = saveEmpWorkTimeRequestDTO.empWorkShiftID;
            sql.Parameters.Add(pWorkShiftNew);

            SqlParameter pReMark = new SqlParameter(@"pReMark", SqlDbType.VarChar,255);
            pReMark.Direction = ParameterDirection.Input;
            pReMark.Value = saveEmpWorkTimeRequestDTO.reason;
            sql.Parameters.Add(pReMark);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        #endregion

        #region leave

        public _ReturnIdModel InsertLeaveDetail(SaveLeaveDetailDTO saveLeaveDetailDTO, int remainDay, int userID, string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_leave " +
                "@pUserID, " +
                "@pLeaveTypeID, " +
                "@pStartDate, " +
                "@pEndDate, " +
                "@pNumberOfDays," +
                "@pLeaveReason," +
                "@pRemaining, " +
                "@pCreateBy");

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = saveLeaveDetailDTO.empId;
            sql.Parameters.Add(pUserID);

            SqlParameter pLeaveTypeID = new SqlParameter(@"pLeaveTypeID", SqlDbType.Int);
            pLeaveTypeID.Direction = ParameterDirection.Input;
            pLeaveTypeID.Value = saveLeaveDetailDTO.leavetypeId;
            sql.Parameters.Add(pLeaveTypeID);

            SqlParameter pStartDate = new SqlParameter(@"pStartDate", SqlDbType.VarChar, 100);
            pStartDate.Direction = ParameterDirection.Input;
            pStartDate.Value = saveLeaveDetailDTO.startdate;
            sql.Parameters.Add(pStartDate);

            SqlParameter pEndDate = new SqlParameter(@"pEndDate", SqlDbType.VarChar, 100);
            pEndDate.Direction = ParameterDirection.Input;
            pEndDate.Value = saveLeaveDetailDTO.enddate;
            sql.Parameters.Add(pEndDate);

            SqlParameter pNumberOfDays = new SqlParameter(@"pNumberOfDays", SqlDbType.Int);
            pNumberOfDays.Direction = ParameterDirection.Input;
            pNumberOfDays.Value = saveLeaveDetailDTO.numdays;
            sql.Parameters.Add(pNumberOfDays);

            SqlParameter pLeaveReason = new SqlParameter(@"pLeaveReason", SqlDbType.VarChar, 250);
            pLeaveReason.Direction = ParameterDirection.Input;
            pLeaveReason.Value = saveLeaveDetailDTO.leavereason;
            sql.Parameters.Add(pLeaveReason);

            SqlParameter pRemaining = new SqlParameter(@"pRemaining", SqlDbType.Int);
            pRemaining.Direction = ParameterDirection.Input;
            pRemaining.Value = remainDay;
            sql.Parameters.Add(pRemaining);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateLeaveDetail(SaveLeaveDetailDTO saveLeaveDetailDTO, int userID, string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_leave " +
                "@pLeaveID, " +
                "@pLeaveTypeID, " +
                "@pStartDate, " +
                "@pEndDate, " +
                "@pNumberOfDays," +
                "@pLeaveReason," +
                "@pUpdateBy");

            SqlParameter pLeaveID = new SqlParameter(@"pLeaveID", SqlDbType.Int);
            pLeaveID.Direction = ParameterDirection.Input;
            pLeaveID.Value = saveLeaveDetailDTO.leaveId;
            sql.Parameters.Add(pLeaveID);

            SqlParameter pLeaveTypeID = new SqlParameter(@"pLeaveTypeID", SqlDbType.VarChar, 50);
            pLeaveTypeID.Direction = ParameterDirection.Input;
            pLeaveTypeID.Value = saveLeaveDetailDTO.leavetypeId;
            sql.Parameters.Add(pLeaveTypeID);

            SqlParameter pStartDate = new SqlParameter(@"pStartDate", SqlDbType.VarChar, 100);
            pStartDate.Direction = ParameterDirection.Input;
            pStartDate.Value = saveLeaveDetailDTO.startdate;
            sql.Parameters.Add(pStartDate);

            SqlParameter pEndDate = new SqlParameter(@"pEndDate", SqlDbType.VarChar, 100);
            pEndDate.Direction = ParameterDirection.Input;
            pEndDate.Value = saveLeaveDetailDTO.enddate;
            sql.Parameters.Add(pEndDate);

            SqlParameter pNumberOfDays = new SqlParameter(@"pNumberOfDays", SqlDbType.Int);
            pNumberOfDays.Direction = ParameterDirection.Input;
            pNumberOfDays.Value = saveLeaveDetailDTO.numdays;
            sql.Parameters.Add(pNumberOfDays);

            SqlParameter pLeaveReason = new SqlParameter(@"pLeaveReason", SqlDbType.VarChar, 250);
            pLeaveReason.Direction = ParameterDirection.Input;
            pLeaveReason.Value = saveLeaveDetailDTO.leavereason;
            sql.Parameters.Add(pLeaveReason);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel CancelLeaveForm(int leaveID, int userID, string cancelReason, string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_cancel_leave " +
                "@pLeaveID," +
                "@pCancelReason," +
                "@pCancelBy");

            SqlParameter pLeaveID = new SqlParameter(@"pLeaveID", SqlDbType.Int);
            pLeaveID.Direction = ParameterDirection.Input;
            pLeaveID.Value = leaveID;
            sql.Parameters.Add(pLeaveID);

            SqlParameter pCancelReason = new SqlParameter(@"pCancelReason", SqlDbType.VarChar);
            pCancelReason.Direction = ParameterDirection.Input;
            pCancelReason.Value = cancelReason;
            sql.Parameters.Add(pCancelReason);

            SqlParameter pCancelBy = new SqlParameter(@"pCancelBy", SqlDbType.Int);
            pCancelBy.Direction = ParameterDirection.Input;
            pCancelBy.Value = userID;
            sql.Parameters.Add(pCancelBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }
            return data;
        }

        public _ReturnIdModel ApproveLeaveForm(int leaveID, int userID, string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_approve_leave " +
                "@pLeaveID, " +
                // "@pRemain," +
                "@pApproveBy");

            SqlParameter pLeaveID = new SqlParameter(@"pLeaveID", SqlDbType.Int);
            pLeaveID.Direction = ParameterDirection.Input;
            pLeaveID.Value = leaveID;
            sql.Parameters.Add(pLeaveID);

            SqlParameter pApproveBy = new SqlParameter(@"pApproveBy", SqlDbType.Int);
            pApproveBy.Direction = ParameterDirection.Input;
            pApproveBy.Value = userID;
            sql.Parameters.Add(pApproveBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }
            return data;
        }

        public _ReturnIdModel RejectLeaveForm(int leaveID, int userID,string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_reject_leave " +
                "@pLeaveID," +
                //"@pRemain, " +
                //"@pRejectReason," +
                "@pRejectBy");

            SqlParameter pLeaveID = new SqlParameter(@"pLeaveID", SqlDbType.Int);
            pLeaveID.Direction = ParameterDirection.Input;
            pLeaveID.Value = leaveID;
            sql.Parameters.Add(pLeaveID);

            SqlParameter pRejectBy = new SqlParameter(@"pRejectBy", SqlDbType.Int);
            pRejectBy.Direction = ParameterDirection.Input;
            pRejectBy.Value = userID;
            sql.Parameters.Add(pRejectBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }
            return data;
        }

        public GetLeaveDetail GetLeaveDetail(GetLeaveDetailRequestDTO leaveDetailDTO, string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_leave_detail " +
                "@pID ," +
                 "@pLang");

            SqlParameter paramID = new SqlParameter(@"pID", SqlDbType.Int);
            paramID.Direction = ParameterDirection.Input;
            paramID.Value = leaveDetailDTO.leaveID;
            sql.Parameters.Add(paramID);

            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 2);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = leaveDetailDTO.pLang;
            sql.Parameters.Add(pLang);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            GetLeaveDetail data = new GetLeaveDetail();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public Pagination<SearchAllLeave> SearchAllLeave(string shareCode, SearchLeaveDTO searchLeaveDTO)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_leave_page " +
                "@pTextSearch, " +
                "@pLeaveTypeID, " +
                "@pDepartmentIDList, " +
                "@pPositionIDList, " +
                "@pEmpTypeList, " +
                "@pFromDate, " +
                "@pToDate, " +
                "@pLang, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = searchLeaveDTO.paramSearch;
            sql.Parameters.Add(pTextSearch);

            SqlParameter pLeaveTypeID = new SqlParameter(@"pLeaveTypeID", SqlDbType.VarChar, 255);
            pLeaveTypeID.Direction = ParameterDirection.Input;
            pLeaveTypeID.Value = searchLeaveDTO.leaveTypeSearch;
            sql.Parameters.Add(pLeaveTypeID);

            SqlParameter pDepartmentIDList = new SqlParameter(@"pDepartmentIDList", SqlDbType.VarChar, 100);
            pDepartmentIDList.Direction = ParameterDirection.Input;
            pDepartmentIDList.Value = searchLeaveDTO.prepairDepartmentSearch;
            sql.Parameters.Add(pDepartmentIDList);

            SqlParameter pPositionIDList = new SqlParameter(@"pPositionIDList", SqlDbType.VarChar, 100);
            pPositionIDList.Direction = ParameterDirection.Input;
            pPositionIDList.Value = searchLeaveDTO.prepairPositionSearch;
            sql.Parameters.Add(pPositionIDList);

            SqlParameter pEmpTypeList = new SqlParameter(@"pEmpTypeList", SqlDbType.VarChar, 100);
            pEmpTypeList.Direction = ParameterDirection.Input;
            pEmpTypeList.Value = searchLeaveDTO.prepairEmpTypeSearch;
            sql.Parameters.Add(pEmpTypeList);

            SqlParameter pFromDate = new SqlParameter(@"pFromDate", SqlDbType.VarChar, 255);
            pFromDate.Direction = ParameterDirection.Input;
            pFromDate.Value = searchLeaveDTO.leaveFrom;
            sql.Parameters.Add(pFromDate);

            SqlParameter pToDate = new SqlParameter(@"pToDate", SqlDbType.VarChar, 255);
            pToDate.Direction = ParameterDirection.Input;
            pToDate.Value = searchLeaveDTO.leaveTo;
            sql.Parameters.Add(pToDate);

            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 255);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = searchLeaveDTO.lang;
            sql.Parameters.Add(pLang);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = searchLeaveDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = searchLeaveDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = searchLeaveDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = searchLeaveDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<SearchAllLeave> pagination = new Pagination<SearchAllLeave>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchAllLeave data = new SearchAllLeave();
                    data.LoadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchAllLeave(shareCode, searchLeaveDTO);

            pagination.SetPagination(total, searchLeaveDTO.perPage, searchLeaveDTO.pageInt);

            return pagination;
        }

        public Pagination<SearchAllMasterDepartmentPosition> SearchAllDepartmentPosition (SearchMasterDepartmentPositionDTO searchMasterDepartmentPositionDTO, string shareCode)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_department_with_position_page " +
                "@pTextSearch, " +
                "@pDepartmentIDList, " +
                "@pPositionIDList, " +
                "@pIsActiveList, " +
                "@pLang, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = searchMasterDepartmentPositionDTO.paramSearch;
            sql.Parameters.Add(pTextSearch);

            SqlParameter pDepartmentIDList = new SqlParameter(@"pDepartmentIDList", SqlDbType.VarChar, 100);
            pDepartmentIDList.Direction = ParameterDirection.Input;
            pDepartmentIDList.Value = searchMasterDepartmentPositionDTO.prepairDepartmentSearch;
            sql.Parameters.Add(pDepartmentIDList);

            SqlParameter pPositionIDList = new SqlParameter(@"pPositionIDList", SqlDbType.VarChar, 100);
            pPositionIDList.Direction = ParameterDirection.Input;
            pPositionIDList.Value = searchMasterDepartmentPositionDTO.prepairPositionSearch;
            sql.Parameters.Add(pPositionIDList);

            SqlParameter pIsActiveList = new SqlParameter(@"pIsActiveList", SqlDbType.VarChar, 100);
            pIsActiveList.Direction = ParameterDirection.Input;
            pIsActiveList.Value = searchMasterDepartmentPositionDTO.prepairIsActiveSearch;
            sql.Parameters.Add(pIsActiveList);
            
            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 255);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = searchMasterDepartmentPositionDTO.lang;
            sql.Parameters.Add(pLang);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = searchMasterDepartmentPositionDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = searchMasterDepartmentPositionDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = searchMasterDepartmentPositionDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = searchMasterDepartmentPositionDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<SearchAllMasterDepartmentPosition> pagination = new Pagination<SearchAllMasterDepartmentPosition>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchAllMasterDepartmentPosition data = new SearchAllMasterDepartmentPosition();
                    data.LoadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchAllDepartmentPosition(searchMasterDepartmentPositionDTO, shareCode);

            pagination.SetPagination(total, searchMasterDepartmentPositionDTO.perPage, searchMasterDepartmentPositionDTO.pageInt);

            return pagination;
        }


        public int GetTotalSearchAllDepartmentPosition(SearchMasterDepartmentPositionDTO searchMasterDepartmentPositionDTO, string shareCode)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_department_with_position_total " +
                 "@pTextSearch, " +
                "@pDepartmentIDList, " +
                "@pPositionIDList, " +
                "@pEmpStatusList ");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = searchMasterDepartmentPositionDTO.paramSearch;
            sql.Parameters.Add(pTextSearch);

            SqlParameter pDepartmentIDList = new SqlParameter(@"pDepartmentIDList", SqlDbType.VarChar, 100);
            pDepartmentIDList.Direction = ParameterDirection.Input;
            pDepartmentIDList.Value = searchMasterDepartmentPositionDTO.prepairDepartmentSearch;
            sql.Parameters.Add(pDepartmentIDList);

            SqlParameter pPositionIDList = new SqlParameter(@"pPositionIDList", SqlDbType.VarChar, 100);
            pPositionIDList.Direction = ParameterDirection.Input;
            pPositionIDList.Value = searchMasterDepartmentPositionDTO.prepairPositionSearch;
            sql.Parameters.Add(pPositionIDList);

            SqlParameter pEmpStatusList = new SqlParameter(@"pEmpStatusList", SqlDbType.VarChar, 100);
            pEmpStatusList.Direction = ParameterDirection.Input;
            pEmpStatusList.Value = searchMasterDepartmentPositionDTO.prepairIsActiveSearch;
            sql.Parameters.Add(pEmpStatusList);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public int GetTotalSearchAllLeave(string shareCode, SearchLeaveDTO searchLeaveDTO)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_leave_total " +
                 "@pTextSearch, " +
                "@pLeaveTypeID, " +
                "@pFromDate, " +
                "@pToDate ");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = searchLeaveDTO.paramSearch;
            sql.Parameters.Add(pTextSearch);

            SqlParameter pLeaveTypeID = new SqlParameter(@"pLeaveTypeID", SqlDbType.VarChar, 255);
            pLeaveTypeID.Direction = ParameterDirection.Input;
            pLeaveTypeID.Value = searchLeaveDTO.leaveTypeSearch;
            sql.Parameters.Add(pLeaveTypeID);

            SqlParameter pFromDate = new SqlParameter(@"pFromDate", SqlDbType.VarChar, 255);
            pFromDate.Direction = ParameterDirection.Input;
            pFromDate.Value = searchLeaveDTO.leaveFrom;
            sql.Parameters.Add(pFromDate);

            SqlParameter pToDate = new SqlParameter(@"pToDate", SqlDbType.VarChar, 255);
            pToDate.Direction = ParameterDirection.Input;
            pToDate.Value = searchLeaveDTO.leaveTo;
            sql.Parameters.Add(pToDate);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public Pagination<SearchAllLeave> SearchAllPendingLeaves(string lang, string shareCode, SearchPendingLeaveDTO searchPendingLeaveDTO)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_leave_pending_page " +
                "@pTextSearch, " +
                "@pLang, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = searchPendingLeaveDTO.paramSearch;
            sql.Parameters.Add(pTextSearch);

            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 255);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = lang;
            sql.Parameters.Add(pLang);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = searchPendingLeaveDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = searchPendingLeaveDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = searchPendingLeaveDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = searchPendingLeaveDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<SearchAllLeave> pagination = new Pagination<SearchAllLeave>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchAllLeave data = new SearchAllLeave();
                    data.LoadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchAllPendingLeave(shareCode, searchPendingLeaveDTO);

            pagination.SetPagination(total, searchPendingLeaveDTO.perPage, searchPendingLeaveDTO.pageInt);

            return pagination;
        }

        public int GetTotalSearchAllPendingLeave(string shareCode, SearchPendingLeaveDTO searchPendingLeaveDTO)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_leave_pending_total " +
                "pTextSearch ");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = searchPendingLeaveDTO.paramSearch;
            sql.Parameters.Add(pTextSearch);
            
            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public int GetTotalDayPerYear(int leaveID)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_day_of_leave " +
                "@pLeaveTypeID ");

            SqlParameter pLeaveTypeID = new SqlParameter(@"pLeaveTypeID", SqlDbType.Int);
            pLeaveTypeID.Direction = ParameterDirection.Input;
            pLeaveTypeID.Value = leaveID;
            sql.Parameters.Add(pLeaveTypeID);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public int GetTotalDayPerYearByLeaveType(int leaveTypeID)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_day_of_leave_by_leave_type " +
                "@pLeaveTypeID ");

            SqlParameter pLeaveTypeID = new SqlParameter(@"pLeaveTypeID", SqlDbType.Int);
            pLeaveTypeID.Direction = ParameterDirection.Input;
            pLeaveTypeID.Value = leaveTypeID;
            sql.Parameters.Add(pLeaveTypeID);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }


        public int GetTotalUseDayPerYear(int leaveID, string shareCode)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_total_use_day_for_leave " +
                "@pLeaveID");

            SqlParameter pLeaveID = new SqlParameter(@"pLeaveID", SqlDbType.Int);
            pLeaveID.Direction = ParameterDirection.Input;
            pLeaveID.Value = leaveID;
            sql.Parameters.Add(pLeaveID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public AllLeaveShift GetWorkShiftLeave(string shareCode, string userList, int workShiftID, string workDate)
        {
            int total = 0;
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_work_time_header " +
                "@pUserIDList, " +
                "@pWorkShiftID, " +
                "@pWorkDate");

            SqlParameter pUserIDList = new SqlParameter(@"pUserIDList", SqlDbType.VarChar, 200);
            pUserIDList.Direction = ParameterDirection.Input;
            pUserIDList.Value = userList;
            sql.Parameters.Add(pUserIDList);

            SqlParameter pWorkShiftID = new SqlParameter(@"pWorkShiftID", SqlDbType.Int);
            pWorkShiftID.Direction = ParameterDirection.Input;
            pWorkShiftID.Value = workShiftID;
            sql.Parameters.Add(pWorkShiftID);

            SqlParameter pWorkDate = new SqlParameter(@"pWorkDate", SqlDbType.VarChar, 25);
            pWorkDate.Direction = ParameterDirection.Input;
            pWorkDate.Value = workDate;
            sql.Parameters.Add(pWorkDate);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            AllLeaveShift data = new AllLeaveShift();

            if (table != null)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.LoadData(row);
                }
            }

            return data;
        }

        public AllLeave GetLeave(string shareCode, string userList, int leaveTypeID, string workDate)
        {
            int total = 0;
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_leave_header " +
                "@pUserIDList, " +
                "@pLeaveTypeID, " +
                "@pWorkDate");

            SqlParameter pUserIDList = new SqlParameter(@"pUserIDList", SqlDbType.VarChar, 200);
            pUserIDList.Direction = ParameterDirection.Input;
            pUserIDList.Value = userList;
            sql.Parameters.Add(pUserIDList);

            SqlParameter pLeaveTypeID = new SqlParameter(@"pLeaveTypeID", SqlDbType.Int);
            pLeaveTypeID.Direction = ParameterDirection.Input;
            pLeaveTypeID.Value = leaveTypeID;
            sql.Parameters.Add(pLeaveTypeID);

            SqlParameter pWorkDate = new SqlParameter(@"pWorkDate", SqlDbType.VarChar, 25);
            pWorkDate.Direction = ParameterDirection.Input;
            pWorkDate.Value = workDate;
            sql.Parameters.Add(pWorkDate);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            AllLeave data = new AllLeave();

            if (table != null)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.LoadData(row);
                }
            }

            return data;
        }


        public List<GetLeave> GetAllLeaveList()
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_leave_type ");

            table = sql.executeQueryWithReturnTable();

            List<GetLeave> listData = new List<GetLeave>();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    GetLeave data = new GetLeave();
                    data.LoadData(row);
                    listData.Add(data);
                }
            }

            return listData;
        }





        #endregion

        public _ReturnIdModel UpdateEmpProfile(string shareCode, SaveEmpProfileDTO saveEmpProfileDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_emp_profile " +
                "@pEmpProfileID, " +

                "@pJoinDate, " +
                "@pMonthlySalary, " +
                "@pDailySalary, " +
                "@pDepartmentID, " +
                "@pPositionID, " +
                "@pEmploymentTypeID, " +

                "@pTitleID, " +
                "@pFirstnameTH, " +
                "@pLastnameTH, " +
                "@pNickNameTH, " +
                "@pFirstnameEN, " +
                "@pLastnameEN, " +
                "@pNickNameEN, " +
                "@pNationalityID, " +
                "@pCitizenshipID, " +
                "@pReligionID, " +
                "@pDateOfBirth, " +
                "@pIdentityCard, " +
                "@pIdentityCardExpiry, " +
                "@pHeight, " +
                "@pWeight, " +
                "@pShirtSizeID, " +
                "@pBloodTypeID, " +
                "@pPhoneNumber, " +

                "@pChest, " +
                "@pWaist, " +
                "@pHip, " +

                "@pImageProfileCode, " +
                "@pImageGalleryCode, " +
                "@pImageIdentityCode, " +

                "@pContactTypeID, " +
                "@pContactExpiryDate, " +

                "@pUpdateBy");

            SqlParameter pEmpProfileID = new SqlParameter(@"pEmpProfileID", SqlDbType.Int);
            pEmpProfileID.Direction = ParameterDirection.Input;
            pEmpProfileID.Value = saveEmpProfileDTO.empProfileID;
            sql.Parameters.Add(pEmpProfileID);

            SqlParameter pJoinDate = new SqlParameter(@"pJoinDate", SqlDbType.Date);
            pJoinDate.Direction = ParameterDirection.Input;
            pJoinDate.Value = saveEmpProfileDTO.joinDate;
            sql.Parameters.Add(pJoinDate);

            SqlParameter pMonthlySalary = new SqlParameter(@"pMonthlySalary", SqlDbType.Decimal);
            pMonthlySalary.Direction = ParameterDirection.Input;
            pMonthlySalary.Value = saveEmpProfileDTO.monthlySalary;
            sql.Parameters.Add(pMonthlySalary);

            SqlParameter pDailySalary = new SqlParameter(@"pDailySalary", SqlDbType.Decimal);
            pDailySalary.Direction = ParameterDirection.Input;
            pDailySalary.Value = saveEmpProfileDTO.dailySalary;
            sql.Parameters.Add(pDailySalary);

            SqlParameter pDepartmentID = new SqlParameter(@"pDepartmentID", SqlDbType.Int);
            pDepartmentID.Direction = ParameterDirection.Input;
            pDepartmentID.Value = saveEmpProfileDTO.departmentID;
            sql.Parameters.Add(pDepartmentID);

            SqlParameter pPositionID = new SqlParameter(@"pPositionID", SqlDbType.Int);
            pPositionID.Direction = ParameterDirection.Input;
            pPositionID.Value = saveEmpProfileDTO.positionID;
            sql.Parameters.Add(pPositionID);

            SqlParameter pEmploymentTypeID = new SqlParameter(@"pEmploymentTypeID", SqlDbType.Int);
            pEmploymentTypeID.Direction = ParameterDirection.Input;
            pEmploymentTypeID.Value = saveEmpProfileDTO.employmentTypeID;
            sql.Parameters.Add(pEmploymentTypeID);

            SqlParameter pTitleID = new SqlParameter(@"pTitleID", SqlDbType.Int);
            pTitleID.Direction = ParameterDirection.Input;
            pTitleID.Value = saveEmpProfileDTO.titleID;
            sql.Parameters.Add(pTitleID);

            SqlParameter pFirstnameEN = new SqlParameter(@"pFirstnameEN", SqlDbType.VarChar, 250);
            pFirstnameEN.Direction = ParameterDirection.Input;
            pFirstnameEN.Value = saveEmpProfileDTO.firstNameEN;
            sql.Parameters.Add(pFirstnameEN);

            SqlParameter pFirstnameTH = new SqlParameter(@"pFirstnameTH", SqlDbType.VarChar, 250);
            pFirstnameTH.Direction = ParameterDirection.Input;
            pFirstnameTH.Value = saveEmpProfileDTO.firstNameTH;
            sql.Parameters.Add(pFirstnameTH);

            SqlParameter pNickNameTH = new SqlParameter(@"pNickNameTH", SqlDbType.VarChar, 10);
            pNickNameTH.Direction = ParameterDirection.Input;
            pNickNameTH.Value = saveEmpProfileDTO.nickNameTH;
            sql.Parameters.Add(pNickNameTH);

            SqlParameter pLastnameEN = new SqlParameter(@"pLastnameEN", SqlDbType.VarChar, 250);
            pLastnameEN.Direction = ParameterDirection.Input;
            pLastnameEN.Value = saveEmpProfileDTO.lastNameEN;
            sql.Parameters.Add(pLastnameEN);

            SqlParameter pLastnameTH = new SqlParameter(@"pLastnameTH", SqlDbType.VarChar, 250);
            pLastnameTH.Direction = ParameterDirection.Input;
            pLastnameTH.Value = saveEmpProfileDTO.lastNameTH;
            sql.Parameters.Add(pLastnameTH);

            SqlParameter pNickNameEN = new SqlParameter(@"pNickNameEN", SqlDbType.VarChar, 10);
            pNickNameEN.Direction = ParameterDirection.Input;
            pNickNameEN.Value = saveEmpProfileDTO.nickNameEN;
            sql.Parameters.Add(pNickNameEN);

            SqlParameter pNationalityID = new SqlParameter(@"pNationalityID", SqlDbType.Int);
            pNationalityID.Direction = ParameterDirection.Input;
            pNationalityID.Value = saveEmpProfileDTO.nationalityID;
            sql.Parameters.Add(pNationalityID);

            SqlParameter pCitizenshipID = new SqlParameter(@"pCitizenshipID", SqlDbType.Int);
            pCitizenshipID.Direction = ParameterDirection.Input;
            pCitizenshipID.Value = saveEmpProfileDTO.citizenshipID;
            sql.Parameters.Add(pCitizenshipID);

            SqlParameter pReligionID = new SqlParameter(@"pReligionID", SqlDbType.Int);
            pReligionID.Direction = ParameterDirection.Input;
            pReligionID.Value = saveEmpProfileDTO.religionID;
            sql.Parameters.Add(pReligionID);

            SqlParameter pDateOfBirth = new SqlParameter(@"pDateOfBirth", SqlDbType.Date);
            pDateOfBirth.Direction = ParameterDirection.Input;
            pDateOfBirth.Value = saveEmpProfileDTO.dateOfBirth;
            sql.Parameters.Add(pDateOfBirth);

            SqlParameter pIdentityCard = new SqlParameter(@"pIdentityCard", SqlDbType.VarChar, 30);
            pIdentityCard.Direction = ParameterDirection.Input;
            pIdentityCard.Value = saveEmpProfileDTO.identityCard;
            sql.Parameters.Add(pIdentityCard);

            SqlParameter pIdentityCardExpiry = new SqlParameter(@"pIdentityCardExpiry", SqlDbType.VarChar, 30);
            pIdentityCardExpiry.Direction = ParameterDirection.Input;
            pIdentityCardExpiry.Value = saveEmpProfileDTO.identityCardExpiry;
            sql.Parameters.Add(pIdentityCardExpiry);

            SqlParameter pHeight = new SqlParameter(@"pHeight", SqlDbType.Decimal);
            pHeight.Direction = ParameterDirection.Input;
            pHeight.Value = saveEmpProfileDTO.height;
            sql.Parameters.Add(pHeight);

            SqlParameter pWeight = new SqlParameter(@"pWeight", SqlDbType.Decimal);
            pWeight.Direction = ParameterDirection.Input;
            pWeight.Value = saveEmpProfileDTO.weight;
            sql.Parameters.Add(pWeight);

            SqlParameter pShirtSizeID = new SqlParameter(@"pShirtSizeID", SqlDbType.Int);
            pShirtSizeID.Direction = ParameterDirection.Input;
            pShirtSizeID.Value = saveEmpProfileDTO.shirtSizeID;
            sql.Parameters.Add(pShirtSizeID);

            SqlParameter pBloodTypeID = new SqlParameter(@"pBloodTypeID", SqlDbType.Int);
            pBloodTypeID.Direction = ParameterDirection.Input;
            pBloodTypeID.Value = saveEmpProfileDTO.bloodTypeID;
            sql.Parameters.Add(pBloodTypeID);

            SqlParameter pPhoneNumber = new SqlParameter(@"pPhoneNumber", SqlDbType.VarChar, 15);
            pPhoneNumber.Direction = ParameterDirection.Input;
            pPhoneNumber.Value = saveEmpProfileDTO.phoneNumber;
            sql.Parameters.Add(pPhoneNumber);

            SqlParameter pChest = new SqlParameter(@"pChest", SqlDbType.Decimal);
            pChest.Direction = ParameterDirection.Input;
            pChest.Value = saveEmpProfileDTO.chest;
            sql.Parameters.Add(pChest);

            SqlParameter pWaist = new SqlParameter(@"pWaist", SqlDbType.Decimal);
            pWaist.Direction = ParameterDirection.Input;
            pWaist.Value = saveEmpProfileDTO.waist;
            sql.Parameters.Add(pWaist);

            SqlParameter pHip = new SqlParameter(@"pHip", SqlDbType.Decimal);
            pHip.Direction = ParameterDirection.Input;
            pHip.Value = saveEmpProfileDTO.hip;
            sql.Parameters.Add(pHip);

            SqlParameter pImageProfileCode = new SqlParameter(@"pImageProfileCode", SqlDbType.VarChar, 250);
            pImageProfileCode.Direction = ParameterDirection.Input;
            pImageProfileCode.Value = saveEmpProfileDTO.imageProfileCode;
            sql.Parameters.Add(pImageProfileCode);

            SqlParameter pImageGalleryCode = new SqlParameter(@"pImageGalleryCode", SqlDbType.VarChar, 250);
            pImageGalleryCode.Direction = ParameterDirection.Input;
            pImageGalleryCode.Value = saveEmpProfileDTO.imageGalleryCode;
            sql.Parameters.Add(pImageGalleryCode);

            SqlParameter pImageIdentityCode = new SqlParameter(@"pImageIdentityCode", SqlDbType.VarChar, 250);
            pImageIdentityCode.Direction = ParameterDirection.Input;
            pImageIdentityCode.Value = saveEmpProfileDTO.imageIdentityCode;
            sql.Parameters.Add(pImageIdentityCode);

            SqlParameter pContactTypeID = new SqlParameter(@"pContactTypeID", SqlDbType.Int);
            pContactTypeID.Direction = ParameterDirection.Input;
            pContactTypeID.Value = saveEmpProfileDTO.contractTypeID;
            sql.Parameters.Add(pContactTypeID);

            SqlParameter pContactExpiryDate = new SqlParameter(@"pContactExpiryDate", SqlDbType.VarChar, 10);
            pContactExpiryDate.Direction = ParameterDirection.Input;
            pContactExpiryDate.Value = saveEmpProfileDTO.contractExpiryDate;
            sql.Parameters.Add(pContactExpiryDate);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateEmpAddress(string shareCode, SaveEmpProfileDTO saveEmpProfileDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_emp_address " +
                "@pUserID, " +
                "@pCAddress, " +
                "@pCCountryID, " +
                "@pCSubDistrictID, " +
                "@pCDistrictID, " +
                "@pCProvinceID, " +
                "@pCZipcode, " +
                "@pCPhoneContact, " +
                "@pIsSamePermanentAddress, " +
                "@pPAddress, " +
                "@pPCountryID, " +
                "@pPSubDistrictID, " +
                "@pPDistrictID, " +
                "@pPProvinceID, " +
                "@pPZipcode, " +
                "@pPPhoneContact, " +
                "@pUpdateBy");

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = saveEmpProfileDTO.userID;
            sql.Parameters.Add(pUserID);

            SqlParameter pCAddress = new SqlParameter(@"pCAddress", SqlDbType.VarChar, 200);
            pCAddress.Direction = ParameterDirection.Input;
            pCAddress.Value = saveEmpProfileDTO.cAddress;
            sql.Parameters.Add(pCAddress);

            SqlParameter pCCountryID = new SqlParameter(@"pCCountryID", SqlDbType.Int);
            pCCountryID.Direction = ParameterDirection.Input;
            pCCountryID.Value = saveEmpProfileDTO.cCountryID;
            sql.Parameters.Add(pCCountryID);

            SqlParameter pCSubDistrictID = new SqlParameter(@"pCSubDistrictID", SqlDbType.Int);
            pCSubDistrictID.Direction = ParameterDirection.Input;
            pCSubDistrictID.Value = saveEmpProfileDTO.cSubDistrictID;
            sql.Parameters.Add(pCSubDistrictID);

            SqlParameter pCDistrictID = new SqlParameter(@"pCDistrictID", SqlDbType.Int);
            pCDistrictID.Direction = ParameterDirection.Input;
            pCDistrictID.Value = saveEmpProfileDTO.cDistrictID;
            sql.Parameters.Add(pCDistrictID);

            SqlParameter pCProvinceID = new SqlParameter(@"pCProvinceID", SqlDbType.Int);
            pCProvinceID.Direction = ParameterDirection.Input;
            pCProvinceID.Value = saveEmpProfileDTO.cProvinceID;
            sql.Parameters.Add(pCProvinceID);

            SqlParameter pCZipcode = new SqlParameter(@"pCZipcode", SqlDbType.VarChar, 10);
            pCZipcode.Direction = ParameterDirection.Input;
            pCZipcode.Value = saveEmpProfileDTO.cZipcode;
            sql.Parameters.Add(pCZipcode);

            SqlParameter pCPhoneContact = new SqlParameter(@"pCPhoneContact", SqlDbType.VarChar, 20);
            pCPhoneContact.Direction = ParameterDirection.Input;
            pCPhoneContact.Value = saveEmpProfileDTO.cPhoneContact;
            sql.Parameters.Add(pCPhoneContact);

            SqlParameter pIsSamePermanentAddress = new SqlParameter(@"pIsSamePermanentAddress", SqlDbType.Int);
            pIsSamePermanentAddress.Direction = ParameterDirection.Input;
            pIsSamePermanentAddress.Value = saveEmpProfileDTO.isSamePermanentAddress;
            sql.Parameters.Add(pIsSamePermanentAddress);

            SqlParameter pPAddress = new SqlParameter(@"pPAddress", SqlDbType.VarChar, 200);
            pPAddress.Direction = ParameterDirection.Input;
            pPAddress.Value = saveEmpProfileDTO.pAddress;
            sql.Parameters.Add(pPAddress);

            SqlParameter pPCountryID = new SqlParameter(@"pPCountryID", SqlDbType.Int);
            pPCountryID.Direction = ParameterDirection.Input;
            pPCountryID.Value = saveEmpProfileDTO.pCountryID;
            sql.Parameters.Add(pPCountryID);

            SqlParameter pPSubDistrictID = new SqlParameter(@"pPSubDistrictID", SqlDbType.Int);
            pPSubDistrictID.Direction = ParameterDirection.Input;
            pPSubDistrictID.Value = saveEmpProfileDTO.pSubDistrictID;
            sql.Parameters.Add(pPSubDistrictID);

            SqlParameter pPDistrictID = new SqlParameter(@"pPDistrictID", SqlDbType.Int);
            pPDistrictID.Direction = ParameterDirection.Input;
            pPDistrictID.Value = saveEmpProfileDTO.pDistrictID;
            sql.Parameters.Add(pPDistrictID);

            SqlParameter pPProvinceID = new SqlParameter(@"pPProvinceID", SqlDbType.Int);
            pPProvinceID.Direction = ParameterDirection.Input;
            pPProvinceID.Value = saveEmpProfileDTO.pProvinceID;
            sql.Parameters.Add(pPProvinceID);

            SqlParameter pPZipcode = new SqlParameter(@"pPZipcode", SqlDbType.VarChar, 10);
            pPZipcode.Direction = ParameterDirection.Input;
            pPZipcode.Value = saveEmpProfileDTO.pZipcode;
            sql.Parameters.Add(pPZipcode);

            SqlParameter pPPhoneContact = new SqlParameter(@"pPPhoneContact", SqlDbType.VarChar, 20);
            pPPhoneContact.Direction = ParameterDirection.Input;
            pPPhoneContact.Value = saveEmpProfileDTO.pPhoneContact;
            sql.Parameters.Add(pPPhoneContact);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateEmpRate(string shareCode, SaveEmpRateRequestDTO saveEmpRateDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_emp_rate " +
                "@pId, " +
                "@pServiceNo, " +
                "@pProductGrade, " +
                "@pStartDrink, " +
                "@pFullDrink, " +
                "@pRateStaff, " +
                "@pRateManager," +
                "@pRateOwner," +
                "@pRateConfirm," +
                "@pUpdateBy");

            SqlParameter paramID = new SqlParameter(@"pId", SqlDbType.Int);
            paramID.Direction = ParameterDirection.Input;
            paramID.Value = saveEmpRateDTO.empRateID;
            sql.Parameters.Add(paramID);

            SqlParameter paramServiceNo = new SqlParameter(@"pServiceNo", SqlDbType.Int);
            paramServiceNo.Direction = ParameterDirection.Input;
            paramServiceNo.Value = saveEmpRateDTO.serviceNo;
            sql.Parameters.Add(paramServiceNo);

            SqlParameter paramProductGrade = new SqlParameter(@"pProductGrade", SqlDbType.Int);
            paramProductGrade.Direction = ParameterDirection.Input;
            paramProductGrade.Value = saveEmpRateDTO.productGrade;
            sql.Parameters.Add(paramProductGrade);

            SqlParameter paramStartDrink = new SqlParameter(@"pStartDrink", SqlDbType.Int);
            paramStartDrink.Direction = ParameterDirection.Input;
            paramStartDrink.Value = saveEmpRateDTO.startDrink;
            sql.Parameters.Add(paramStartDrink);

            SqlParameter paramFullDrink = new SqlParameter(@"pFullDrink", SqlDbType.Int);
            paramFullDrink.Direction = ParameterDirection.Input;
            paramFullDrink.Value = saveEmpRateDTO.fullDrink;
            sql.Parameters.Add(paramFullDrink);

            SqlParameter paramRateStaff = new SqlParameter(@"pRateStaff", SqlDbType.Float);
            paramRateStaff.Direction = ParameterDirection.Input;
            paramRateStaff.Value = saveEmpRateDTO.rateStaff;
            sql.Parameters.Add(paramRateStaff);

            SqlParameter paramRateManager = new SqlParameter(@"pRateManager", SqlDbType.Float);
            paramRateManager.Direction = ParameterDirection.Input;
            paramRateManager.Value = saveEmpRateDTO.rateManager;
            sql.Parameters.Add(paramRateManager);

            SqlParameter paramRateOwner = new SqlParameter(@"pRateOwner", SqlDbType.Float);
            paramRateOwner.Direction = ParameterDirection.Input;
            paramRateOwner.Value = saveEmpRateDTO.rateOwner;
            sql.Parameters.Add(paramRateOwner);

            SqlParameter paramRateConfirm = new SqlParameter(@"pRateConfirm", SqlDbType.Float);
            paramRateConfirm.Direction = ParameterDirection.Input;
            paramRateConfirm.Value = saveEmpRateDTO.rateConfirm;
            sql.Parameters.Add(paramRateConfirm);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateEmpWorkShift(string shareCode, SaveEmpWorkShiftRequestDTO saveEmpWorkShiftRequestDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_emp_work_shift " +
                "@pId, " +
                "@pWsCode, " +
                "@pTimeStart, " +
                "@pTimeEnd, " +
                "@pWorkTypeID," +
                "@pRemark," +
                "@pStatus," +
                "@pUpdateBy");

            SqlParameter paramID = new SqlParameter(@"pId", SqlDbType.Int);
            paramID.Direction = ParameterDirection.Input;
            paramID.Value = saveEmpWorkShiftRequestDTO.workShiftID;
            sql.Parameters.Add(paramID);

            SqlParameter paramWsCode = new SqlParameter(@"pWsCode", SqlDbType.VarChar, 50);
            paramWsCode.Direction = ParameterDirection.Input;
            paramWsCode.Value = saveEmpWorkShiftRequestDTO.wsCode;
            sql.Parameters.Add(paramWsCode);

            SqlParameter paramTimeStart = new SqlParameter(@"pTimeStart", SqlDbType.VarChar, 100);
            paramTimeStart.Direction = ParameterDirection.Input;
            paramTimeStart.Value = saveEmpWorkShiftRequestDTO.timeStart;
            sql.Parameters.Add(paramTimeStart);

            SqlParameter paramTimeEnd = new SqlParameter(@"pTimeEnd", SqlDbType.VarChar, 100);
            paramTimeEnd.Direction = ParameterDirection.Input;
            paramTimeEnd.Value = saveEmpWorkShiftRequestDTO.timeEnd;
            sql.Parameters.Add(paramTimeEnd);

            SqlParameter paramWorkTypeID = new SqlParameter(@"pWorkTypeID", SqlDbType.Int);
            paramWorkTypeID.Direction = ParameterDirection.Input;
            paramWorkTypeID.Value = saveEmpWorkShiftRequestDTO.workTypeID;
            sql.Parameters.Add(paramWorkTypeID);

            SqlParameter paramRemark = new SqlParameter(@"pRemark", SqlDbType.VarChar, 200);
            paramRemark.Direction = ParameterDirection.Input;
            paramRemark.Value = saveEmpWorkShiftRequestDTO.remark;
            sql.Parameters.Add(paramRemark);

            SqlParameter paramStatus = new SqlParameter(@"pStatus", SqlDbType.Int);
            paramStatus.Direction = ParameterDirection.Input;
            paramStatus.Value = saveEmpWorkShiftRequestDTO.status;
            sql.Parameters.Add(paramStatus);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }


        public _ReturnIdModel DeleteEmpWorkShift(string shareCode, SaveEmpWorkShiftRequestDTO saveEmpWorkShiftRequestDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_emp_work_shift " +
                "@pId, " +
                "@pIsCancel," +
                "@pCancelBy");

            SqlParameter paramID = new SqlParameter(@"pId", SqlDbType.Int);
            paramID.Direction = ParameterDirection.Input;
            paramID.Value = saveEmpWorkShiftRequestDTO.workShiftID;
            sql.Parameters.Add(paramID);

            SqlParameter pIsCancel = new SqlParameter(@"pIsCancel", SqlDbType.Int);
            pIsCancel.Direction = ParameterDirection.Input;
            pIsCancel.Value = saveEmpWorkShiftRequestDTO.isCancel;
            sql.Parameters.Add(pIsCancel);

            SqlParameter pCancelBy = new SqlParameter(@"pCancelBy", SqlDbType.Int);
            pCancelBy.Direction = ParameterDirection.Input;
            pCancelBy.Value = userID;
            sql.Parameters.Add(pCancelBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateEmpStatus(string shareCode, SaveEmpStatusDTO saveEmpStatusDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_emp_status " +
                "@pUserID, " +
                "@pEmploymentStatusID, " +
                "@pImageEmploymentCode, " +
                "@pUpdateBy");

            SqlParameter paramUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            paramUserID.Direction = ParameterDirection.Input;
            paramUserID.Value = saveEmpStatusDTO.userID;
            sql.Parameters.Add(paramUserID);

            SqlParameter paramEmploymentStatusID = new SqlParameter(@"pEmploymentStatusID", SqlDbType.Int);
            paramEmploymentStatusID.Direction = ParameterDirection.Input;
            paramEmploymentStatusID.Value = saveEmpStatusDTO.employmentStatusID;
            sql.Parameters.Add(paramEmploymentStatusID);

            SqlParameter paramImageEmploymentCode = new SqlParameter(@"pImageEmploymentCode", SqlDbType.VarChar, 200);
            paramImageEmploymentCode.Direction = ParameterDirection.Input;
            paramImageEmploymentCode.Value = saveEmpStatusDTO.imageEmploymentCode;
            sql.Parameters.Add(paramImageEmploymentCode);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel DeleteEmpProfile(SaveEmpProfileDTO saveEmpProfileDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec delete_emp_profile " +
                "@pEmpProfileID, " +
                "@pUpdateBy");

            SqlParameter pEmpProfileID = new SqlParameter(@"pEmpProfileID", SqlDbType.Int);
            pEmpProfileID.Direction = ParameterDirection.Input;
            pEmpProfileID.Value = saveEmpProfileDTO.empProfileID;
            sql.Parameters.Add(pEmpProfileID);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel DeleteEmpfile(string shareCode, int id, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec delete_emp_file " +
                "@pFileID, " +
                "@pUpdateBy");

            SqlParameter pFileID = new SqlParameter(@"pFileID", SqlDbType.Int);
            pFileID.Direction = ParameterDirection.Input;
            pFileID.Value = id;
            sql.Parameters.Add(pFileID);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel DeleteEmpRate(EmpRateRequestDTO empRateRequestDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec delete_emp_rate " +
                "@pEmpId, " +
                "@pUpdateBy");

            SqlParameter paramEmpId = new SqlParameter(@"pEmpId", SqlDbType.Int);
            paramEmpId.Direction = ParameterDirection.Input;
            paramEmpId.Value = empRateRequestDTO.empID;
            sql.Parameters.Add(paramEmpId);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public BodySet GetBodySet(string shareCode, int id)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_body_set " +
                "@pId");

            SqlParameter pId = new SqlParameter(@"pId", SqlDbType.Int);
            pId.Direction = ParameterDirection.Input;
            pId.Value = id;
            sql.Parameters.Add(pId);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            BodySet data = new BodySet();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public List<DropdownTitleName> GetDropdownTitle(string lang)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_dropdown_title " +
                "@pLang ");

            SqlParameter paramLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 5);
            paramLang.Direction = ParameterDirection.Input;
            paramLang.Value = lang;
            sql.Parameters.Add(paramLang);


            table = sql.executeQueryWithReturnTable();

            List<DropdownTitleName> listData = new List<DropdownTitleName>();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DropdownTitleName data = new DropdownTitleName();
                    data.loadData(row);
                    listData.Add(data);
                }
            }

            return listData;
        }

        public List<_DropdownAllData> GetDropdownPositionFilter(string lang, int departmentID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_dropdown_position_filter " +
                "@pDepartmentID, " +
                "@pLang ");

            SqlParameter pDepartmentID = new SqlParameter(@"pDepartmentID", SqlDbType.Int);
            pDepartmentID.Direction = ParameterDirection.Input;
            pDepartmentID.Value = departmentID;
            sql.Parameters.Add(pDepartmentID);

            SqlParameter paramLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 5);
            paramLang.Direction = ParameterDirection.Input;
            paramLang.Value = lang;
            sql.Parameters.Add(paramLang);


            table = sql.executeQueryWithReturnTable();

            List<_DropdownAllData> listData = new List<_DropdownAllData>();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    _DropdownAllData data = new _DropdownAllData();
                    data.loadData(row);
                    listData.Add(data);
                }
            }

            return listData;
        }

        public _ReturnIdModel InsertUploadFileDetails(string shareCode, string actionName, string fileCode, string fileExtendtion, string fileName, string fileUrl, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_file_details " +
                "@pActionName, " +
                "@pFileCode, " +
                "@pFileExtendtion, " +
                "@pFileName," +
                "@pFileUrl, " +
                "@pCreateBy");

            SqlParameter paramActionName = new SqlParameter(@"pActionName", SqlDbType.VarChar, 50);
            paramActionName.Direction = ParameterDirection.Input;
            paramActionName.Value = actionName;
            sql.Parameters.Add(paramActionName);

            SqlParameter paramFileCode = new SqlParameter(@"pFileCode", SqlDbType.VarChar, 50);
            paramFileCode.Direction = ParameterDirection.Input;
            paramFileCode.Value = fileCode;
            sql.Parameters.Add(paramFileCode);

            SqlParameter paramFileExtendtion = new SqlParameter(@"pFileExtendtion", SqlDbType.VarChar, 20);
            paramFileExtendtion.Direction = ParameterDirection.Input;
            paramFileExtendtion.Value = fileExtendtion;
            sql.Parameters.Add(paramFileExtendtion);

            SqlParameter paramFileName = new SqlParameter(@"pFileName", SqlDbType.VarChar, 20);
            paramFileName.Direction = ParameterDirection.Input;
            paramFileName.Value = fileName;
            sql.Parameters.Add(paramFileName);

            SqlParameter paramFileUrl = new SqlParameter(@"pFileUrl", SqlDbType.VarChar, 250);
            paramFileUrl.Direction = ParameterDirection.Input;
            paramFileUrl.Value = fileUrl;
            sql.Parameters.Add(paramFileUrl);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }
        public _ReturnIdModel UpdateFileDetails(string shareCode,int fileDetailID,  string newFileCode, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_file_details " +
                "@pFileDetailID, " +
                "@pNewFileCode, " +
                "@pUpdateBy");

            SqlParameter paramFileDetailID = new SqlParameter(@"pFileDetailID", SqlDbType.VarChar, 50);
            paramFileDetailID.Direction = ParameterDirection.Input;
            paramFileDetailID.Value = fileDetailID;
            sql.Parameters.Add(paramFileDetailID);

            SqlParameter paramNewFileCode = new SqlParameter(@"pNewFileCode", SqlDbType.VarChar, 50);
            paramNewFileCode.Direction = ParameterDirection.Input;
            paramNewFileCode.Value = newFileCode;
            sql.Parameters.Add(paramNewFileCode);
            
            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel InsertBodySet(string shareCode, SaveBodySetRequestDTO saveBodySetDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_body_set " +
                "@pHeight, " +
                "@pWeight, " +
                "@pChest, " +
                "@pWaist," +
                "@pHip, " +
                "@pCreateBy");

            SqlParameter paramHeight = new SqlParameter(@"pHeight", SqlDbType.Float);
            paramHeight.Direction = ParameterDirection.Input;
            paramHeight.Value = saveBodySetDTO.height;
            sql.Parameters.Add(paramHeight);

            SqlParameter paramWeight = new SqlParameter(@"pWeight", SqlDbType.Float);
            paramWeight.Direction = ParameterDirection.Input;
            paramWeight.Value = saveBodySetDTO.weight;
            sql.Parameters.Add(paramWeight);

            SqlParameter paramChest = new SqlParameter(@"pChest", SqlDbType.Int);
            paramChest.Direction = ParameterDirection.Input;
            paramChest.Value = saveBodySetDTO.chest;
            sql.Parameters.Add(paramChest);

            SqlParameter paramWaist = new SqlParameter(@"pWaist", SqlDbType.Int);
            paramWaist.Direction = ParameterDirection.Input;
            paramWaist.Value = saveBodySetDTO.waist;
            sql.Parameters.Add(paramWaist);

            SqlParameter paramHip = new SqlParameter(@"pHip", SqlDbType.Int);
            paramHip.Direction = ParameterDirection.Input;
            paramHip.Value = saveBodySetDTO.hip;
            sql.Parameters.Add(paramHip);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateBodySet(string shareCode, SaveBodySetRequestDTO saveBodySetDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_body_set " +
                "@pBodyID, " +
                "@pHeight, " +
                "@pWeight, " +
                "@pChest," +
                "@pWaist," +
                "@pHip," +
                "@pUpdateBy");

            SqlParameter paramBodyID = new SqlParameter(@"pBodyID", SqlDbType.Int);
            paramBodyID.Direction = ParameterDirection.Input;
            paramBodyID.Value = saveBodySetDTO.masterID;
            sql.Parameters.Add(paramBodyID);

            SqlParameter paramHeight = new SqlParameter(@"pHeight", SqlDbType.Float);
            paramHeight.Direction = ParameterDirection.Input;
            paramHeight.Value = saveBodySetDTO.height;
            sql.Parameters.Add(paramHeight);

            SqlParameter paramWeight = new SqlParameter(@"pWeight", SqlDbType.Float);
            paramWeight.Direction = ParameterDirection.Input;
            paramWeight.Value = saveBodySetDTO.weight;
            sql.Parameters.Add(paramWeight);

            SqlParameter paramChest = new SqlParameter(@"pChest", SqlDbType.Int);
            paramChest.Direction = ParameterDirection.Input;
            paramChest.Value = saveBodySetDTO.chest;
            sql.Parameters.Add(paramChest);

            SqlParameter paramWaist = new SqlParameter(@"pWaist", SqlDbType.Int);
            paramWaist.Direction = ParameterDirection.Input;
            paramWaist.Value = saveBodySetDTO.waist;
            sql.Parameters.Add(paramWaist);

            SqlParameter paramHip = new SqlParameter(@"pHip", SqlDbType.Int);
            paramHip.Direction = ParameterDirection.Input;
            paramHip.Value = saveBodySetDTO.hip;
            sql.Parameters.Add(paramHip);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel DeleteEmpWorkShift(SaveEmpWorkShiftRequestDTO requestDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec delete_emp_work_shift " +
                "@pId, " +
                "@pUpdateBy");

            SqlParameter paramId = new SqlParameter(@"pId", SqlDbType.Int);
            paramId.Direction = ParameterDirection.Input;
            paramId.Value = requestDTO.workShiftID;
            sql.Parameters.Add(paramId);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel DeleteBodySet(string shareCode, SaveBodySetRequestDTO saveBodySetDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec delete_body_set " +
                "@pBodyID, " +
                "@pUpdateBy");

            SqlParameter paramBodyID = new SqlParameter(@"pBodyID", SqlDbType.Int);
            paramBodyID.Direction = ParameterDirection.Input;
            paramBodyID.Value = saveBodySetDTO.masterID;
            sql.Parameters.Add(paramBodyID);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel InsertFeedback(FeedbackDTO feedbackDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_feedback " +
                "@pEmpID, " +
                "@pRate, " +
                "@pComment, " +
                "@pTranId," +
                "@pCreateBy ");

            SqlParameter paramEmpID = new SqlParameter(@"pEmpID", SqlDbType.Int);
            paramEmpID.Direction = ParameterDirection.Input;
            paramEmpID.Value = feedbackDTO.EmpID;
            sql.Parameters.Add(paramEmpID);

            SqlParameter paramRate = new SqlParameter(@"pRate", SqlDbType.Int);
            paramRate.Direction = ParameterDirection.Input;
            paramRate.Value = feedbackDTO.Rate;
            sql.Parameters.Add(paramRate);

            SqlParameter paramComment = new SqlParameter(@"pComment", SqlDbType.VarChar);
            paramComment.Direction = ParameterDirection.Input;
            paramComment.Value = feedbackDTO.Comment;
            sql.Parameters.Add(paramComment);

            SqlParameter paramTranId = new SqlParameter(@"pTranId", SqlDbType.Int);
            paramTranId.Direction = ParameterDirection.Input;
            paramTranId.Value = feedbackDTO.TranID;
            sql.Parameters.Add(paramTranId);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public DataTable CheckValidationUpdateByID(int ID, string Type)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec check_validation_update " +
                "@pID, " +
                "@pType");

            SqlParameter pID = new SqlParameter(@"pID", SqlDbType.Int);
            pID.Direction = ParameterDirection.Input;
            pID.Value = ID;
            sql.Parameters.Add(pID);

            SqlParameter pType = new SqlParameter(@"pType", SqlDbType.VarChar);
            pType.Direction = ParameterDirection.Input;
            pType.Value = Type;
            sql.Parameters.Add(pType);

            table = sql.executeQueryWithReturnTable();

            return table;
        }

        public DataTable CheckDuplicateMaster(string shareCode, string TableName, MasterDataDTO masterDataDTO)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("");

            sql = new SQLCustomExecute("exec check_duplicate_master @pMasterID,@pTableName,@pNameEN, @pNameTH");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = masterDataDTO.masterID;
            sql.Parameters.Add(pMasterID);

            SqlParameter pTableName = new SqlParameter(@"pTableName", SqlDbType.VarChar);
            pTableName.Direction = ParameterDirection.Input;
            pTableName.Value = TableName;
            sql.Parameters.Add(pTableName);

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            table = sql.executeQueryWithReturnTable();

            return table;
        }

        public DataTable CheckDuplicateMasterKey(string shareCode, string TableName, MasterDataDTO masterDataDTO)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("");

            sql = new SQLCustomExecute("exec check_duplicate_master_key @pMasterID,@pKeyName");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = masterDataDTO.masterID;
            sql.Parameters.Add(pMasterID);

            SqlParameter pKeyName = new SqlParameter(@"pKeyName", SqlDbType.VarChar);
            pKeyName.Direction = ParameterDirection.Input;
            pKeyName.Value = masterDataDTO.keyName;
            sql.Parameters.Add(pKeyName);

            table = sql.executeQueryWithReturnTable();

            return table;
        }


        public Pagination<SearchMasterData> SearchMasterDataPosition(string paramSearch, int perPage, int pageInt, int sortField, string sortType, string shareCode)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_position_page " +
                "@pParamSearch, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = paramSearch;
            sql.Parameters.Add(pParamSearch);

            SqlParameter paramPage = new SqlParameter(@"pPage", SqlDbType.Int);
            paramPage.Direction = ParameterDirection.Input;
            paramPage.Value = pageInt;
            sql.Parameters.Add(paramPage);

            SqlParameter paramPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            paramPerPage.Direction = ParameterDirection.Input;
            paramPerPage.Value = perPage;
            sql.Parameters.Add(paramPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<SearchMasterData> pagination = new Pagination<SearchMasterData>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchMasterData data = new SearchMasterData();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchMasterPosition(paramSearch, shareCode);

            pagination.SetPagination(total, perPage, pageInt);

            return pagination;
        }

        public int GetTotalSearchMasterPosition(string paramSearch, string shareCode)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_position_total " +
                "@pParamSearch ");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = paramSearch;
            sql.Parameters.Add(pParamSearch);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public MasterPosition GetMasterPosition(int id, string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_master_position " +
                "@pPositionID");

            SqlParameter pPositionID = new SqlParameter(@"pPositionID", SqlDbType.Int);
            pPositionID.Direction = ParameterDirection.Input;
            pPositionID.Value = id;
            sql.Parameters.Add(pPositionID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            MasterPosition data = new MasterPosition();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public GetEmpRate GetEmpRate(int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_emp_rate " +
                "@pEmpId");

            SqlParameter paramID = new SqlParameter(@"pEmpId", SqlDbType.Int);
            paramID.Direction = ParameterDirection.Input;
            paramID.Value = userID;
            sql.Parameters.Add(paramID);

            table = sql.executeQueryWithReturnTable();

            GetEmpRate data = new GetEmpRate();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public GetFeedback GetFeedback(int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_feedback " +
                "@pEmpId");

            SqlParameter paramID = new SqlParameter(@"pEmpId", SqlDbType.Int);
            paramID.Direction = ParameterDirection.Input;
            paramID.Value = userID;
            sql.Parameters.Add(paramID);

            table = sql.executeQueryWithReturnTable();

            GetFeedback data = new GetFeedback();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public GetEmpWorkShift GetEmpWorkShift(string shareCode, int empWorkShiftID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_emp_work_shift " +
                "@pID");

            SqlParameter paramID = new SqlParameter(@"pID", SqlDbType.Int);
            paramID.Direction = ParameterDirection.Input;
            paramID.Value = empWorkShiftID;
            sql.Parameters.Add(paramID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            GetEmpWorkShift data = new GetEmpWorkShift();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public SwitchWorkShift UpdateSwitchStatusWorkShift(string shareCode, int empWorkShiftID, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_switch_status_work_shift " +
                "@pID, " +
                "@pUpdateBy");

            SqlParameter paramID = new SqlParameter(@"pID", SqlDbType.Int);
            paramID.Direction = ParameterDirection.Input;
            paramID.Value = empWorkShiftID;
            sql.Parameters.Add(paramID);

            SqlParameter paramUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            paramUpdateBy.Direction = ParameterDirection.Input;
            paramUpdateBy.Value = userID;
            sql.Parameters.Add(paramUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            SwitchWorkShift data = new SwitchWorkShift();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public Pagination<SearchMasterWorkShift> SearchAllMasterWorkShift(string shareCode, int userID, string lang, int positionID, PageRequestDTO pageRequestDTO)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_employee_normal_page " +
                "@pLang, " +
                "@pPositionID, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 10);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = lang;
            sql.Parameters.Add(pLang);

            SqlParameter pPositionID = new SqlParameter(@"pPositionID", SqlDbType.Int);
            pPositionID.Direction = ParameterDirection.Input;
            pPositionID.Value = positionID;
            sql.Parameters.Add(pPositionID);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = pageRequestDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = pageRequestDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = pageRequestDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = pageRequestDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<SearchMasterWorkShift> pagination = new Pagination<SearchMasterWorkShift>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchMasterWorkShift data = new SearchMasterWorkShift();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalMasterWorkShift(shareCode, positionID);

            pagination.SetPagination(total, pageRequestDTO.perPage, pageRequestDTO.pageInt);

            return pagination;
        }

        public int GetTotalMasterWorkShift(string shareCode, int positionID)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_employee_normal_total "
                + "@pPositionID");

            SqlParameter pPositionID = new SqlParameter(@"pPositionID", SqlDbType.Int);
            pPositionID.Direction = ParameterDirection.Input;
            pPositionID.Value = positionID;
            sql.Parameters.Add(pPositionID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }


        public int GetTotalSearchAllEmployee(string shareCode, PageRequestDTO pageRequest)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_employee_total " +
                 "@pTextSearch, " +
                "@pDepartmentIDList, " +
                "@pPositionIDList, " +
                "@pEmpTypeList, " +
                "@pEmpStatusList " );

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = pageRequest.paramSearch;
            sql.Parameters.Add(pTextSearch);

            SqlParameter pDepartmentIDList = new SqlParameter(@"pDepartmentIDList", SqlDbType.VarChar, 100);
            pDepartmentIDList.Direction = ParameterDirection.Input;
            pDepartmentIDList.Value = pageRequest.prepairDepartmentSearch;
            sql.Parameters.Add(pDepartmentIDList);

            SqlParameter pPositionIDList = new SqlParameter(@"pPositionIDList", SqlDbType.VarChar, 100);
            pPositionIDList.Direction = ParameterDirection.Input;
            pPositionIDList.Value = pageRequest.prepairPositionSearch;
            sql.Parameters.Add(pPositionIDList);

            SqlParameter pEmpTypeList = new SqlParameter(@"pEmpTypeList", SqlDbType.VarChar, 100);
            pEmpTypeList.Direction = ParameterDirection.Input;
            pEmpTypeList.Value = pageRequest.prepairEmpTypeSearch;
            sql.Parameters.Add(pEmpTypeList);

            SqlParameter pEmpStatusList = new SqlParameter(@"pEmpStatusList", SqlDbType.VarChar, 100);
            pEmpStatusList.Direction = ParameterDirection.Input;
            pEmpStatusList.Value = pageRequest.prepairEmpStatusSearch;
            sql.Parameters.Add(pEmpStatusList);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }
        public int GetTotalSearchWorkShiftTotal(string shareCode, PageRequestDTO pageRequest)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_work_shift_total " +
                "@pTextSearch, " +
                "@pWorkType, " +
                "@pStatusList ");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = pageRequest.paramSearch;
            sql.Parameters.Add(pTextSearch);

            SqlParameter pWorkType = new SqlParameter(@"pWorkType", SqlDbType.VarChar, 100);
            pWorkType.Direction = ParameterDirection.Input;
            pWorkType.Value = pageRequest.prepairWorkTypeSearch;
            sql.Parameters.Add(pWorkType);

            SqlParameter pStatusList = new SqlParameter(@"pStatusList", SqlDbType.VarChar, 100);
            pStatusList.Direction = ParameterDirection.Input;
            pStatusList.Value = pageRequest.prepairIsActiveSearch;
            sql.Parameters.Add(pStatusList);
            
            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }




        public _ReturnIdModel InsertMasterPosition(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_master_position " +
                "@pNameEN," +
                "@pNameTH," +
                "@pUserID ");

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateMasterPosition(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_master_position " +
                "@pMasterID," +
                "@pNameEN," +
                "@pNameTH," +
                "@pUserID ");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = masterDataDTO.masterID;
            sql.Parameters.Add(pMasterID);

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel DeleteMasterPosition(int id, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec delete_master_position " +
                "@pMasterID," +
                "@pUserID");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = id;
            sql.Parameters.Add(pMasterID);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);


            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public Pagination<SearchMasterData> SearchMasterDataProductArea(string paramSearch, int perPage, int pageInt, int sortField, string sortType)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_product_area_page " +
                "@pParamSearch, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = paramSearch;
            sql.Parameters.Add(pParamSearch);

            SqlParameter paramPage = new SqlParameter(@"pPage", SqlDbType.Int);
            paramPage.Direction = ParameterDirection.Input;
            paramPage.Value = pageInt;
            sql.Parameters.Add(paramPage);

            SqlParameter paramPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            paramPerPage.Direction = ParameterDirection.Input;
            paramPerPage.Value = perPage;
            sql.Parameters.Add(paramPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTable();

            Pagination<SearchMasterData> pagination = new Pagination<SearchMasterData>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchMasterData data = new SearchMasterData();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchMasterProductArea(paramSearch);

            pagination.SetPagination(total, perPage, pageInt);

            return pagination;
        }

        public int GetTotalSearchMasterProductArea(string paramSearch)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_product_area_total " +
                "@pParamSearch ");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = paramSearch;
            sql.Parameters.Add(pParamSearch);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public MasterData GetMasterProductArea(int id)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_master_product_area " +
                "@pMasterID");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = id;
            sql.Parameters.Add(pMasterID);

            table = sql.executeQueryWithReturnTable();

            MasterData data = new MasterData();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel InsertMasterProductArea(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_master_product_area " +
                "@pNameEN," +
                "@pNameTH," +
                "@pUserID ");

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateMasterProductArea(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_master_product_area " +
                "@pMasterID," +
                "@pNameEN," +
                "@pNameTH," +
                "@pUserID ");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = masterDataDTO.masterID;
            sql.Parameters.Add(pMasterID);

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel DeleteMasterProductArea(int id, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec delete_master_product_area " +
                "@pMasterID," +
                "@pUserID");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = id;
            sql.Parameters.Add(pMasterID);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);


            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public Pagination<SearchMasterData> SearchMasterDataProductCategory(string paramSearch, int perPage, int pageInt, int sortField, string sortType)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_product_category_page " +
                "@pParamSearch, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = paramSearch;
            sql.Parameters.Add(pParamSearch);

            SqlParameter paramPage = new SqlParameter(@"pPage", SqlDbType.Int);
            paramPage.Direction = ParameterDirection.Input;
            paramPage.Value = pageInt;
            sql.Parameters.Add(paramPage);

            SqlParameter paramPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            paramPerPage.Direction = ParameterDirection.Input;
            paramPerPage.Value = perPage;
            sql.Parameters.Add(paramPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTable();

            Pagination<SearchMasterData> pagination = new Pagination<SearchMasterData>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchMasterData data = new SearchMasterData();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchMasterProductCategory(paramSearch);

            pagination.SetPagination(total, perPage, pageInt);

            return pagination;
        }

        public int GetTotalSearchMasterProductCategory(string paramSearch)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_product_area_total " +
                "@pParamSearch ");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = paramSearch;
            sql.Parameters.Add(pParamSearch);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public MasterData GetMasterProductCategory(int id)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_master_product_category " +
                "@pMasterID");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = id;
            sql.Parameters.Add(pMasterID);

            table = sql.executeQueryWithReturnTable();

            MasterData data = new MasterData();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel InsertMasterProductCategory(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_master_product_category " +
                "@pNameEN," +
                "@pNameTH," +
                "@pUserID ");

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateMasterProductCategory(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_master_product_category " +
                "@pMasterID," +
                "@pNameEN," +
                "@pNameTH," +
                "@pUserID ");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = masterDataDTO.masterID;
            sql.Parameters.Add(pMasterID);

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel DeleteMasterProductCategory(int id, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec delete_master_product_category " +
                "@pMasterID," +
                "@pUserID");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = id;
            sql.Parameters.Add(pMasterID);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);


            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public Pagination<SearchMasterData> SearchMasterDataProductType(string paramSearch, int perPage, int pageInt, int sortField, string sortType)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_product_type_page " +
                "@pParamSearch, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = paramSearch;
            sql.Parameters.Add(pParamSearch);

            SqlParameter paramPage = new SqlParameter(@"pPage", SqlDbType.Int);
            paramPage.Direction = ParameterDirection.Input;
            paramPage.Value = pageInt;
            sql.Parameters.Add(paramPage);

            SqlParameter paramPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            paramPerPage.Direction = ParameterDirection.Input;
            paramPerPage.Value = perPage;
            sql.Parameters.Add(paramPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTable();

            Pagination<SearchMasterData> pagination = new Pagination<SearchMasterData>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchMasterData data = new SearchMasterData();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchMasterProductType(paramSearch);

            pagination.SetPagination(total, perPage, pageInt);

            return pagination;
        }

        public int GetTotalSearchMasterProductType(string paramSearch)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_product_type_total " +
                "@pParamSearch ");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = paramSearch;
            sql.Parameters.Add(pParamSearch);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public MasterData GetMasterProductType(int id)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_master_product_type " +
                "@pMasterID");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = id;
            sql.Parameters.Add(pMasterID);

            table = sql.executeQueryWithReturnTable();

            MasterData data = new MasterData();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel InsertMasterProductType(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_master_product_type " +
                "@pNameEN," +
                "@pNameTH," +
                "@pUserID ");

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateMasterProductType(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_master_product_type " +
                "@pMasterID," +
                "@pNameEN," +
                "@pNameTH," +
                "@pUserID ");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = masterDataDTO.masterID;
            sql.Parameters.Add(pMasterID);

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel DeleteMasterProductType(int id, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec delete_master_product_type " +
                "@pMasterID," +
                "@pUserID");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = id;
            sql.Parameters.Add(pMasterID);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);


            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public Pagination<SearchMasterData> SearchMasterDataQueMemberType(string paramSearch, int perPage, int pageInt, int sortField, string sortType)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_que_member_type_page " +
                "@pParamSearch, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = paramSearch;
            sql.Parameters.Add(pParamSearch);

            SqlParameter paramPage = new SqlParameter(@"pPage", SqlDbType.Int);
            paramPage.Direction = ParameterDirection.Input;
            paramPage.Value = pageInt;
            sql.Parameters.Add(paramPage);

            SqlParameter paramPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            paramPerPage.Direction = ParameterDirection.Input;
            paramPerPage.Value = perPage;
            sql.Parameters.Add(paramPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTable();

            Pagination<SearchMasterData> pagination = new Pagination<SearchMasterData>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchMasterData data = new SearchMasterData();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchMasterQueMemberType(paramSearch);

            pagination.SetPagination(total, perPage, pageInt);

            return pagination;
        }

        public int GetTotalSearchMasterQueMemberType(string paramSearch)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_que_member_type_total " +
                "@pParamSearch ");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = paramSearch;
            sql.Parameters.Add(pParamSearch);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public MasterData GetMasterQueMemberType(int id)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_master_que_member_type " +
                "@pMasterID");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = id;
            sql.Parameters.Add(pMasterID);

            table = sql.executeQueryWithReturnTable();

            MasterData data = new MasterData();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel InsertMasterQueMemberType(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_master_que_member_type " +
                "@pNameEN," +
                "@pNameTH," +
                "@pUserID ");

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateMasterQueMemberType(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_master_que_member_type " +
                "@pMasterID," +
                "@pNameEN," +
                "@pNameTH," +
                "@pUserID ");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = masterDataDTO.masterID;
            sql.Parameters.Add(pMasterID);

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel DeleteMasterQueMemberType(int id, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec delete_master_que_member_type " +
                "@pMasterID," +
                "@pUserID");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = id;
            sql.Parameters.Add(pMasterID);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);


            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public Pagination<SearchMasterData> SearchMasterDataQueStaffType(string paramSearch, int perPage, int pageInt, int sortField, string sortType)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_que_staff_type_page " +
                "@pParamSearch, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = paramSearch;
            sql.Parameters.Add(pParamSearch);

            SqlParameter paramPage = new SqlParameter(@"pPage", SqlDbType.Int);
            paramPage.Direction = ParameterDirection.Input;
            paramPage.Value = pageInt;
            sql.Parameters.Add(paramPage);

            SqlParameter paramPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            paramPerPage.Direction = ParameterDirection.Input;
            paramPerPage.Value = perPage;
            sql.Parameters.Add(paramPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTable();

            Pagination<SearchMasterData> pagination = new Pagination<SearchMasterData>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchMasterData data = new SearchMasterData();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchMasterQueStaffType(paramSearch);

            pagination.SetPagination(total, perPage, pageInt);

            return pagination;
        }

        public int GetTotalSearchMasterQueStaffType(string paramSearch)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_que_staff_type_total " +
                "@pParamSearch ");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = paramSearch;
            sql.Parameters.Add(pParamSearch);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public MasterData GetMasterQueStaffType(int id)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_master_que_staff_type " +
                "@pMasterID");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = id;
            sql.Parameters.Add(pMasterID);

            table = sql.executeQueryWithReturnTable();

            MasterData data = new MasterData();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel InsertMasterQueStaffType(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_master_que_staff_type " +
                "@pNameEN," +
                "@pNameTH," +
                "@pUserID ");

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateMasterQueStaffType(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_master_que_staff_type " +
                "@pMasterID," +
                "@pNameEN," +
                "@pNameTH," +
                "@pUserID ");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = masterDataDTO.masterID;
            sql.Parameters.Add(pMasterID);

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel DeleteMasterQueStaffType(int id, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec delete_master_que_staff_type " +
                "@pMasterID," +
                "@pUserID");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = id;
            sql.Parameters.Add(pMasterID);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);


            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public Pagination<SearchMasterData> SearchMasterDataRoomType(string paramSearch, int perPage, int pageInt, int sortField, string sortType)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_room_type_page " +
                "@pParamSearch, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = paramSearch;
            sql.Parameters.Add(pParamSearch);

            SqlParameter paramPage = new SqlParameter(@"pPage", SqlDbType.Int);
            paramPage.Direction = ParameterDirection.Input;
            paramPage.Value = pageInt;
            sql.Parameters.Add(paramPage);

            SqlParameter paramPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            paramPerPage.Direction = ParameterDirection.Input;
            paramPerPage.Value = perPage;
            sql.Parameters.Add(paramPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTable();

            Pagination<SearchMasterData> pagination = new Pagination<SearchMasterData>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchMasterData data = new SearchMasterData();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchMasterRoomType(paramSearch);

            pagination.SetPagination(total, perPage, pageInt);

            return pagination;
        }

        public int GetTotalSearchMasterRoomType(string paramSearch)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_room_type_total " +
                "@pParamSearch ");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = paramSearch;
            sql.Parameters.Add(pParamSearch);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public MasterData GetMasterRoomType(int id)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_master_room_type " +
                "@pMasterID");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = id;
            sql.Parameters.Add(pMasterID);

            table = sql.executeQueryWithReturnTable();

            MasterData data = new MasterData();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel InsertMasterRoomType(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_master_room_type " +
                "@pNameEN," +
                "@pNameTH," +
                "@pUserID ");

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateMasterRoomType(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_master_room_type " +
                "@pMasterID," +
                "@pNameEN," +
                "@pNameTH," +
                "@pUserID ");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = masterDataDTO.masterID;
            sql.Parameters.Add(pMasterID);

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel DeleteMasterRoomType(int id, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec delete_master_room_type " +
                "@pMasterID," +
                "@pUserID");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = id;
            sql.Parameters.Add(pMasterID);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);


            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public Pagination<SearchMasterData> SearchMasterDataStockType(string paramSearch, int perPage, int pageInt, int sortField, string sortType)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_stock_type_page " +
                "@pParamSearch, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = paramSearch;
            sql.Parameters.Add(pParamSearch);

            SqlParameter paramPage = new SqlParameter(@"pPage", SqlDbType.Int);
            paramPage.Direction = ParameterDirection.Input;
            paramPage.Value = pageInt;
            sql.Parameters.Add(paramPage);

            SqlParameter paramPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            paramPerPage.Direction = ParameterDirection.Input;
            paramPerPage.Value = perPage;
            sql.Parameters.Add(paramPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTable();

            Pagination<SearchMasterData> pagination = new Pagination<SearchMasterData>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchMasterData data = new SearchMasterData();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchMasterStockType(paramSearch);

            pagination.SetPagination(total, perPage, pageInt);

            return pagination;
        }

        public int GetTotalSearchMasterStockType(string paramSearch)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_stock_type_total " +
                "@pParamSearch ");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = paramSearch;
            sql.Parameters.Add(pParamSearch);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public int GetIdUpdateByUserID(string shareCode, string targetTable, string userID)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_id_update_by_user_id " +
                "@pTargetTable, " +
                "@pUserID ");

            SqlParameter pTargetTable = new SqlParameter(@"pTargetTable", SqlDbType.VarChar, 100);
            pTargetTable.Direction = ParameterDirection.Input;
            pTargetTable.Value = targetTable;
            sql.Parameters.Add(pTargetTable);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.VarChar, 10);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["id"].ToString());
                }
            }

            return total;
        }

        public MasterData GetMasterStockType(int id)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_master_stock_type " +
                "@pMasterID");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = id;
            sql.Parameters.Add(pMasterID);

            table = sql.executeQueryWithReturnTable();

            MasterData data = new MasterData();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel InsertMasterStockType(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_master_stock_type " +
                "@pNameEN," +
                "@pNameTH," +
                "@pUserID ");

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateMasterStockType(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_master_stock_type " +
                "@pMasterID," +
                "@pNameEN," +
                "@pNameTH," +
                "@pUserID ");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = masterDataDTO.masterID;
            sql.Parameters.Add(pMasterID);

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel DeleteMasterStockType(int id, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec delete_master_stock_type " +
                "@pMasterID," +
                "@pUserID");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = id;
            sql.Parameters.Add(pMasterID);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);


            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel InsertSystemLogChange(string shareCode, int actionID, string tableName, string fieldName, string newData, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_system_log_change " +
                "@pActionID," +
                "@pTableName," +
                "@pFieldName," +
                "@pNewData," +
                "@pUserID ");

            SqlParameter pActionID = new SqlParameter(@"pActionID", SqlDbType.Int);
            pActionID.Direction = ParameterDirection.Input;
            pActionID.Value = actionID;
            sql.Parameters.Add(pActionID);

            SqlParameter pTableName = new SqlParameter(@"pTableName", SqlDbType.VarChar, 100);
            pTableName.Direction = ParameterDirection.Input;
            pTableName.Value = tableName;
            sql.Parameters.Add(pTableName);

            SqlParameter pFieldName = new SqlParameter(@"pFieldName", SqlDbType.VarChar, 100);
            pFieldName.Direction = ParameterDirection.Input;
            pFieldName.Value = fieldName;
            sql.Parameters.Add(pFieldName);

            SqlParameter pNewData = new SqlParameter(@"pNewData", SqlDbType.VarChar, 4000);
            pNewData.Direction = ParameterDirection.Input;
            pNewData.Value = newData;
            sql.Parameters.Add(pNewData);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }
        public _ReturnIdModel InsertSystemLogChangeWithShareCode(string shareCode, int actionID, string tableName, string fieldName, string newData, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_system_log_change " +
                "@pActionID," +
                "@pTableName," +
                "@pFieldName," +
                "@pNewData," +
                "@pUserID ");

            SqlParameter pActionID = new SqlParameter(@"pActionID", SqlDbType.Int);
            pActionID.Direction = ParameterDirection.Input;
            pActionID.Value = actionID;
            sql.Parameters.Add(pActionID);

            SqlParameter pTableName = new SqlParameter(@"pTableName", SqlDbType.VarChar, 100);
            pTableName.Direction = ParameterDirection.Input;
            pTableName.Value = tableName;
            sql.Parameters.Add(pTableName);

            SqlParameter pFieldName = new SqlParameter(@"pFieldName", SqlDbType.VarChar, 100);
            pFieldName.Direction = ParameterDirection.Input;
            pFieldName.Value = fieldName;
            sql.Parameters.Add(pFieldName);

            SqlParameter pNewData = new SqlParameter(@"pNewData", SqlDbType.VarChar, 4000);
            pNewData.Direction = ParameterDirection.Input;
            pNewData.Value = newData;
            sql.Parameters.Add(pNewData);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));


            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel InsertMasterData(string shareCode, MasterDataDTO masterDataDTO, string TableName, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_master_data " +
                "@pTableName," +
                "@pNameEN," +
                "@pNameTH," +
                "@pDeptID," +
                "@pUserID," +
                "@pIsActive ");

            SqlParameter pTableName = new SqlParameter(@"pTableName", SqlDbType.VarChar);
            pTableName.Direction = ParameterDirection.Input;
            pTableName.Value = TableName;
            sql.Parameters.Add(pTableName);

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pDeptID = new SqlParameter(@"pDeptID", SqlDbType.Int);
            pDeptID.Direction = ParameterDirection.Input;
            pDeptID.Value = masterDataDTO.deptID;
            sql.Parameters.Add(pDeptID);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            SqlParameter pIsActive = new SqlParameter(@"pIsActive", SqlDbType.Int);
            pIsActive.Direction = ParameterDirection.Input;
            pIsActive.Value = int.Parse(masterDataDTO.isActive);
            sql.Parameters.Add(pIsActive);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateMasterData(string shareCode, MasterDataDTO masterDataDTO, string TableName, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_master_data " +
                "@pMasterID," +
                "@pTableName," +
                "@pNameEN," +
                "@pNameTH," +
                "@pDeptID," +
                "@pUserID," +
                "@pIsActive ");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = masterDataDTO.masterID;
            sql.Parameters.Add(pMasterID);

            SqlParameter pTableName = new SqlParameter(@"pTableName", SqlDbType.VarChar);
            pTableName.Direction = ParameterDirection.Input;
            pTableName.Value = TableName;
            sql.Parameters.Add(pTableName);

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = masterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = masterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pDeptID = new SqlParameter(@"pDeptID", SqlDbType.Int);
            pDeptID.Direction = ParameterDirection.Input;
            pDeptID.Value = masterDataDTO.deptID;
            sql.Parameters.Add(pDeptID);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            SqlParameter pIsActive = new SqlParameter(@"pIsActive", SqlDbType.Int);
            pIsActive.Direction = ParameterDirection.Input;
            pIsActive.Value = masterDataDTO.isActive;
            sql.Parameters.Add(pIsActive);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel DeleteMasterData(string shareCode, MasterDataDTO masterDataDTO, string TableName, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec delete_master_data " +
                "@pMasterID," +
                "@pTableName," +
                "@pUserID ");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = masterDataDTO.masterID;
            sql.Parameters.Add(pMasterID);

            SqlParameter pTableName = new SqlParameter(@"pTableName", SqlDbType.VarChar);
            pTableName.Direction = ParameterDirection.Input;
            pTableName.Value = TableName;
            sql.Parameters.Add(pTableName);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel CancelSystemPosition(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec cancel_system_position " +
                "@pID," +
                "@pIsActive," +
                "@pUpdateBy ");

            SqlParameter pID = new SqlParameter(@"pID", SqlDbType.Int);
            pID.Direction = ParameterDirection.Input;
            pID.Value = masterDataDTO.masterID;
            sql.Parameters.Add(pID);

            SqlParameter pIsActive = new SqlParameter(@"pIsActive", SqlDbType.Int);
            pIsActive.Direction = ParameterDirection.Input;
            pIsActive.Value = masterDataDTO.isActive;
            sql.Parameters.Add(pIsActive);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel CancelSystemDepartment(MasterDataDTO masterDataDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec cancel_system_department " +
                "@pID," +
                "@pIsActive," +
                "@pUpdateBy ");

            SqlParameter pID = new SqlParameter(@"pID", SqlDbType.Int);
            pID.Direction = ParameterDirection.Input;
            pID.Value = masterDataDTO.masterID;
            sql.Parameters.Add(pID);

            SqlParameter pIsActive = new SqlParameter(@"pIsActive", SqlDbType.Int);
            pIsActive.Direction = ParameterDirection.Input;
            pIsActive.Value = masterDataDTO.isActive;
            sql.Parameters.Add(pIsActive);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }


        public MasterData GetMasterData(string shareCode, int id, string TableName)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_master_data " +
                "@pMasterID," +
                "@pTableName");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = id;
            sql.Parameters.Add(pMasterID);

            SqlParameter pTableName = new SqlParameter(@"pTableName", SqlDbType.VarChar);
            pTableName.Direction = ParameterDirection.Input;
            pTableName.Value = TableName;
            sql.Parameters.Add(pTableName);

            table = sql.executeQueryWithReturnTable();

            MasterData data = new MasterData();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public Pagination<SearchMasterData> SearchMaster(string shareCode, SearchMasterDataDTO searchMasterDataDTO, string TableName)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_master_data_page " +
                "@pTableName, " +
                "@pNameEN, " +
                "@pNameTH, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pTableName = new SqlParameter(@"pTableName", SqlDbType.VarChar, 255);
            pTableName.Direction = ParameterDirection.Input;
            pTableName.Value = TableName;
            sql.Parameters.Add(pTableName);

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar, 255);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = searchMasterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar, 255);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = searchMasterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = searchMasterDataDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = searchMasterDataDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = searchMasterDataDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = searchMasterDataDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTable();

            Pagination<SearchMasterData> pagination = new Pagination<SearchMasterData>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchMasterData data = new SearchMasterData();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchMaster(shareCode, searchMasterDataDTO, TableName);

            pagination.SetPagination(total, searchMasterDataDTO.perPage, searchMasterDataDTO.pageInt);

            return pagination;
        }
        public Pagination<SearchMasterDataBodySet> SearchMasterBodySet(string shareCode, SearchMasterDataDTO searchMasterDataDTO)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_body_set_page " +
                "@pParamSearch, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = searchMasterDataDTO.paramSearch;
            sql.Parameters.Add(pParamSearch);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = searchMasterDataDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = searchMasterDataDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = searchMasterDataDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = searchMasterDataDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<SearchMasterDataBodySet> pagination = new Pagination<SearchMasterDataBodySet>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchMasterDataBodySet data = new SearchMasterDataBodySet();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchMasterBodySet(shareCode, searchMasterDataDTO);

            pagination.SetPagination(total, searchMasterDataDTO.perPage, searchMasterDataDTO.pageInt);

            return pagination;
        }

        public int GetTotalSearchMasterBodySet(string shareCode, SearchMasterDataDTO searchMasterDataDTO)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_body_set_total " +
                "@pParamSearch ");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = searchMasterDataDTO.paramSearch;
            sql.Parameters.Add(pParamSearch);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public int GetTotalSearchMaster(string shareCode, SearchMasterDataDTO searchMasterDataDTO, string TableName)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_master_data_total " +
                "@pTableName, " +
                "@pNameEN, " +
                "@pNameTH ");

            SqlParameter pTableName = new SqlParameter(@"pTableName", SqlDbType.VarChar, 255);
            pTableName.Direction = ParameterDirection.Input;
            pTableName.Value = TableName;
            sql.Parameters.Add(pTableName);

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar, 255);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = searchMasterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar, 255);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = searchMasterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public EmpProfile GetEmpProfile(string shareCode, int userID, string lang)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_emp_profile " +
                "@pUserID," +
                "@pLang");

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 10);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = lang;
            sql.Parameters.Add(pLang);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            EmpProfile data = new EmpProfile();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public Pagination<GetAllEmployee> GetAllEmployeePretty(string shareCode, int userID, string lang, PageRequestDTO pageRequestDTO)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_employee_pretty_page " +
                "@pLang, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 10);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = lang;
            sql.Parameters.Add(pLang);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = pageRequestDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = pageRequestDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = pageRequestDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = pageRequestDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<GetAllEmployee> pagination = new Pagination<GetAllEmployee>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    GetAllEmployee data = new GetAllEmployee();
                    data.loadData(row);
                    data.imageGallery = GetImgGallary(shareCode, data.userID);
                    data.employeeRate = GetEmployeePrettyRate(shareCode, data.userID, data.userID);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalAllPretty(shareCode);

            pagination.SetPagination(total, pageRequestDTO.perPage, pageRequestDTO.pageInt);

            return pagination;
        }

        public int GetTotalAllPretty(string shareCode)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_employee_pretty_total ");

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public Pagination<GetAllEmployeeNormal> GetAllEmployeeNormal(string shareCode, int userID, string lang, int positionID, PageRequestDTO pageRequestDTO)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_employee_normal_page " +
                "@pLang, " +
                "@pPositionID, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 10);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = lang;
            sql.Parameters.Add(pLang);

            SqlParameter pPositionID = new SqlParameter(@"pPositionID", SqlDbType.Int);
            pPositionID.Direction = ParameterDirection.Input;
            pPositionID.Value = positionID;
            sql.Parameters.Add(pPositionID);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = pageRequestDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = pageRequestDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = pageRequestDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = pageRequestDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<GetAllEmployeeNormal> pagination = new Pagination<GetAllEmployeeNormal>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    GetAllEmployeeNormal data = new GetAllEmployeeNormal();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalAllEmployeeNormal(shareCode, positionID);

            pagination.SetPagination(total, pageRequestDTO.perPage, pageRequestDTO.pageInt);

            return pagination;
        }

        public int GetTotalAllEmployeeNormal(string shareCode, int positionID)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_employee_normal_total "
                + "@pPositionID");

            SqlParameter pPositionID = new SqlParameter(@"pPositionID", SqlDbType.Int);
            pPositionID.Direction = ParameterDirection.Input;
            pPositionID.Value = positionID;
            sql.Parameters.Add(pPositionID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public List<_GetfileByCode> GetEmpFileByCode(string shareCode, int userID, string lang, string fileCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_file_by_file_code " +
                "@pFileCode");

            SqlParameter pFileCode = new SqlParameter(@"pFileCode", SqlDbType.VarChar, 200);
            pFileCode.Direction = ParameterDirection.Input;
            pFileCode.Value = fileCode;
            sql.Parameters.Add(pFileCode);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            List<_GetfileByCode> listData = new List<_GetfileByCode>();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    _GetfileByCode data = new _GetfileByCode();
                    data.loadData(row);
                    listData.Add(data);
                }
            }

            return listData;
        }

        public List<EmployeeDetails.EmergencyContact> GetEmerContact(string shareCode, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_emergency_contact " +
                "@pUserID"
                );

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            List<EmployeeDetails.EmergencyContact> listData = new List<EmployeeDetails.EmergencyContact>();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    EmployeeDetails.EmergencyContact data = new EmployeeDetails.EmergencyContact();
                    data.loadData(row);
                    listData.Add(data);
                }
            }

            return listData;
        }

        public List<EmployeeDetails.ImageGallery> GetImgGallary(string shareCode, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_image_gallary " +
                "@pUserID"
                );

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            List<EmployeeDetails.ImageGallery> listData = new List<EmployeeDetails.ImageGallery>();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    EmployeeDetails.ImageGallery data = new EmployeeDetails.ImageGallery();
                    data.loadData(row);
                    listData.Add(data);
                }
            }

            return listData;
        }

        public List<EmployeeDetails.ImageGallery> GetImgIdentity(string shareCode, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_image_identity " +
                "@pUserID"
                );

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            List<EmployeeDetails.ImageGallery> listData = new List<EmployeeDetails.ImageGallery>();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    EmployeeDetails.ImageGallery data = new EmployeeDetails.ImageGallery();
                    data.loadData(row);
                    listData.Add(data);
                }
            }

            return listData;
        }

        public List<GetEmployeeRate> GetEmployeePrettyRate(string shareCode, int userID, int empRateID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_employee_pretty_rate " +
                "@pEmpRateID, " +
                "@pUserID"
                );

            SqlParameter pEmpRateID = new SqlParameter(@"pEmpRateID", SqlDbType.Int);
            pEmpRateID.Direction = ParameterDirection.Input;
            pEmpRateID.Value = empRateID;
            sql.Parameters.Add(pEmpRateID);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            List<GetEmployeeRate> listData = new List<GetEmployeeRate>();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    GetEmployeeRate data = new GetEmployeeRate();
                    data.loadData(row);
                    listData.Add(data);
                }
            }

            return listData;
        }

        public EmployeeDetails GetEmpProfile(string shareCode, int userID, string lang, RequestDTO requestDTO)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_employee_details  " +
                "@pUserID, " +
                "@pLang");

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = requestDTO.userID;
            sql.Parameters.Add(pUserID);

            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 10);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = lang;
            sql.Parameters.Add(pLang);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            EmployeeDetails data = new EmployeeDetails();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }
        public Pagination<SearchAllMasterWorkShift> SearchAllWorkShiftPage(string shareCode, PageRequestDTO pageRequest)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_work_shift_page " +
                "@pTextSearch, " +
                "@pWorkType, " +
                "@pStatusList, " +
                "@pLang, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = pageRequest.paramSearch;
            sql.Parameters.Add(pTextSearch);

            SqlParameter pWorkType = new SqlParameter(@"pWorkType", SqlDbType.VarChar, 100);
            pWorkType.Direction = ParameterDirection.Input;
            pWorkType.Value = pageRequest.prepairWorkTypeSearch;
            sql.Parameters.Add(pWorkType);

            SqlParameter pStatusList = new SqlParameter(@"pStatusList", SqlDbType.VarChar, 100);
            pStatusList.Direction = ParameterDirection.Input;
            pStatusList.Value = pageRequest.prepairIsActiveSearch;
            sql.Parameters.Add(pStatusList);
            
            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 255);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = pageRequest.lang;
            sql.Parameters.Add(pLang);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = pageRequest.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = pageRequest.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = pageRequest.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = pageRequest.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<SearchAllMasterWorkShift> pagination = new Pagination<SearchAllMasterWorkShift>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchAllMasterWorkShift data = new SearchAllMasterWorkShift();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchWorkShiftTotal(shareCode, pageRequest);

            pagination.SetPagination(total, pageRequest.perPage, pageRequest.pageInt);

            return pagination;
        }

        public Pagination<SearchAllEmployee> SearchAllEmployee(string shareCode, PageRequestDTO pageRequest)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_employee " +
                "@pTextSearch, " +
                "@pDepartmentIDList, " +
                "@pPositionIDList, " +
                "@pEmpTypeList, " +
                "@pEmpStatusList, " +
                "@pLang, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = pageRequest.paramSearch;
            sql.Parameters.Add(pTextSearch);

            SqlParameter pDepartmentIDList = new SqlParameter(@"pDepartmentIDList", SqlDbType.VarChar, 100);
            pDepartmentIDList.Direction = ParameterDirection.Input;
            pDepartmentIDList.Value = pageRequest.prepairDepartmentSearch;
            sql.Parameters.Add(pDepartmentIDList);

            SqlParameter pPositionIDList = new SqlParameter(@"pPositionIDList", SqlDbType.VarChar, 100);
            pPositionIDList.Direction = ParameterDirection.Input;
            pPositionIDList.Value = pageRequest.prepairPositionSearch;
            sql.Parameters.Add(pPositionIDList);

            SqlParameter pEmpTypeList = new SqlParameter(@"pEmpTypeList", SqlDbType.VarChar, 100);
            pEmpTypeList.Direction = ParameterDirection.Input;
            pEmpTypeList.Value = pageRequest.prepairEmpTypeSearch;
            sql.Parameters.Add(pEmpTypeList);

            SqlParameter pEmpStatusList = new SqlParameter(@"pEmpStatusList", SqlDbType.VarChar, 100);
            pEmpStatusList.Direction = ParameterDirection.Input;
            pEmpStatusList.Value = pageRequest.prepairEmpStatusSearch;
            sql.Parameters.Add(pEmpStatusList);

            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 255);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = pageRequest.lang;
            sql.Parameters.Add(pLang);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = pageRequest.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = pageRequest.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = pageRequest.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = pageRequest.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<SearchAllEmployee> pagination = new Pagination<SearchAllEmployee>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchAllEmployee data = new SearchAllEmployee();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchAllEmployee(shareCode, pageRequest);

            pagination.SetPagination(total, pageRequest.perPage, pageRequest.pageInt);

            return pagination;
        }

        public Pagination<SearchWorkTime> SearchWorkTime(string shareCode, SearchWorkTimeDTO searchWorkTimeDTO)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_work_time_page " +
                "@pTextSearch, " +
                "@pDepartmentIDList, " +
                "@pPositionIDList, " +
                "@pDateSearch, " +
                "@pLang, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = searchWorkTimeDTO.paramSearch;
            sql.Parameters.Add(pTextSearch);

            SqlParameter pPositionIDList = new SqlParameter(@"pPositionIDList", SqlDbType.VarChar, 100);
            pPositionIDList.Direction = ParameterDirection.Input;
            pPositionIDList.Value = searchWorkTimeDTO.prepairPositionSearch;
            sql.Parameters.Add(pPositionIDList);

            SqlParameter pDepartmentIDList = new SqlParameter(@"pDepartmentIDList", SqlDbType.VarChar, 100);
            pDepartmentIDList.Direction = ParameterDirection.Input;
            pDepartmentIDList.Value = searchWorkTimeDTO.prepairDepartmentSearch;
            sql.Parameters.Add(pDepartmentIDList);

            SqlParameter pDateSearch = new SqlParameter(@"pDateSearch", SqlDbType.VarChar, 100);
            pDateSearch.Direction = ParameterDirection.Input;
            pDateSearch.Value = searchWorkTimeDTO.dateSearch;
            sql.Parameters.Add(pDateSearch);

            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 255);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = searchWorkTimeDTO.lang;
            sql.Parameters.Add(pLang);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = searchWorkTimeDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = searchWorkTimeDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = searchWorkTimeDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = searchWorkTimeDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<SearchWorkTime> pagination = new Pagination<SearchWorkTime>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchWorkTime data = new SearchWorkTime();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchWorkTime(shareCode, searchWorkTimeDTO);

            pagination.SetPagination(total, searchWorkTimeDTO.perPage, searchWorkTimeDTO.pageInt);

            return pagination;
        }

        public Pagination<SearchWorkTimePendingPage> SearchWorkTimePending(string shareCode, SearchWorkTimePendingDTO searchWorkTimePendingDTO)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_work_time_pending_page " +
                "@pTextSearch, " +
                "@pLang, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = searchWorkTimePendingDTO.paramSearch;
            sql.Parameters.Add(pTextSearch);
            
            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 255);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = searchWorkTimePendingDTO.lang;
            sql.Parameters.Add(pLang);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = searchWorkTimePendingDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = searchWorkTimePendingDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = searchWorkTimePendingDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = searchWorkTimePendingDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<SearchWorkTimePendingPage> pagination = new Pagination<SearchWorkTimePendingPage>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchWorkTimePendingPage data = new SearchWorkTimePendingPage();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchWorkTimePending(shareCode, searchWorkTimePendingDTO);
            
            pagination.SetPagination(total, searchWorkTimePendingDTO.perPage, searchWorkTimePendingDTO.pageInt);

            return pagination;
        }

        public int GetTotalSearchWorkTimePending(string shareCode, SearchWorkTimePendingDTO searchWorkTimePendingDTO)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_work_time_pending_total " +
                "@pTextSearch ");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = searchWorkTimePendingDTO.paramSearch;
            sql.Parameters.Add(pTextSearch);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }
        public SearchWorktimePendingTotalDTO GetTotalSearchWorkTimePendingList(string shareCode, string userIDList)
        {
            SearchWorktimePendingTotalDTO data = new SearchWorktimePendingTotalDTO();

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_work_time_pending_list " +
                "@pUserIDList ");

            SqlParameter pUserIDList = new SqlParameter(@"pUserIDList", SqlDbType.VarChar, 255);
            pUserIDList.Direction = ParameterDirection.Input;
            pUserIDList.Value = userIDList;
            sql.Parameters.Add(pUserIDList);
            
            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public int GetTotalSearchWorkTime(string shareCode, SearchWorkTimeDTO searchWorkTimeDTO)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_work_time_total " +
                "@pTextSearch, " +
                "@pDepartmentIDList, " +
                "@pPositionIDList, " +
                "@pDateSearch ");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = searchWorkTimeDTO.paramSearch;
            sql.Parameters.Add(pTextSearch);

            SqlParameter pPositionIDList = new SqlParameter(@"pPositionIDList", SqlDbType.VarChar, 100);
            pPositionIDList.Direction = ParameterDirection.Input;
            pPositionIDList.Value = searchWorkTimeDTO.prepairPositionSearch;
            sql.Parameters.Add(pPositionIDList);

            SqlParameter pDepartmentIDList = new SqlParameter(@"pDepartmentIDList", SqlDbType.VarChar, 100);
            pDepartmentIDList.Direction = ParameterDirection.Input;
            pDepartmentIDList.Value = searchWorkTimeDTO.prepairDepartmentSearch;
            sql.Parameters.Add(pDepartmentIDList);

            SqlParameter pDateSearch = new SqlParameter(@"pDateSearch", SqlDbType.VarChar, 100);
            pDateSearch.Direction = ParameterDirection.Input;
            pDateSearch.Value = searchWorkTimeDTO.dateSearch;
            sql.Parameters.Add(pDateSearch);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public string getConnectionEncoded(string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_connection_by_sharecode " +
                "@pShareCode");

            SqlParameter paramShareCode = new SqlParameter(@"pShareCode", SqlDbType.VarChar, 100);
            paramShareCode.Direction = ParameterDirection.Input;
            paramShareCode.Value = shareCode;
            sql.Parameters.Add(paramShareCode);

            table = sql.executeQueryWithReturnTable();

            string connectionEncoded = "";

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    connectionEncoded = dr["connectionString"].ToString();
                }
            }

            return connectionEncoded;
        }

        public DataTable GetAllEmpCode(string shareCode, string pLang)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_emp_code " +
                "@pLang");

            SqlParameter paramLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 10);
            paramLang.Direction = ParameterDirection.Input;
            paramLang.Value = pLang;
            sql.Parameters.Add(paramLang);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            return table;
        }

        public DataTable GetAllWorkShift(string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_work_shift ");

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            return table;
        }

        public List<_DropdownAllData> GetDropdownByModuleName(string lang, string moduleName)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_dropdown_by_module_name " +
                "@pLang, " +
                "@pModuleName");

            SqlParameter paramLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 5);
            paramLang.Direction = ParameterDirection.Input;
            paramLang.Value = lang;
            sql.Parameters.Add(paramLang);

            SqlParameter paramModuleName = new SqlParameter(@"pModuleName", SqlDbType.VarChar, 50);
            paramModuleName.Direction = ParameterDirection.Input;
            paramModuleName.Value = moduleName;
            sql.Parameters.Add(paramModuleName);

            table = sql.executeQueryWithReturnTable();

            List<_DropdownAllData> listData = new List<_DropdownAllData>();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    _DropdownAllData data = new _DropdownAllData();
                    data.loadData(row);
                    listData.Add(data);
                }
            }

            return listData;
        }
        public List<_DropdownAllData> GetDropdownDistrict(string lang, int provinceID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_dropdown_district " +
                "@pProvinceID, " +
                "@pLang");

            SqlParameter pProvinceID = new SqlParameter(@"pProvinceID", SqlDbType.Int);
            pProvinceID.Direction = ParameterDirection.Input;
            pProvinceID.Value = provinceID;
            sql.Parameters.Add(pProvinceID);

            SqlParameter paramLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 5);
            paramLang.Direction = ParameterDirection.Input;
            paramLang.Value = lang;
            sql.Parameters.Add(paramLang);

            table = sql.executeQueryWithReturnTable();

            List<_DropdownAllData> listData = new List<_DropdownAllData>();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    _DropdownAllData data = new _DropdownAllData();
                    data.loadData(row);
                    listData.Add(data);
                }
            }

            return listData;
        }
        public List<EmpTradeWorkShift> GetDropdownEmpTradeWorkShift(string shareCode, string lang, string workDate)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_dropdown_emp_work_shift_trans_change " +
                "@pWorkDate, " +
                "@pLang");

            SqlParameter pWorkDate = new SqlParameter(@"pWorkDate", SqlDbType.Date);
            pWorkDate.Direction = ParameterDirection.Input;
            pWorkDate.Value = workDate;
            sql.Parameters.Add(pWorkDate);

            SqlParameter paramLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 5);
            paramLang.Direction = ParameterDirection.Input;
            paramLang.Value = lang;
            sql.Parameters.Add(paramLang);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            List<EmpTradeWorkShift> listData = new List<EmpTradeWorkShift>();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    EmpTradeWorkShift data = new EmpTradeWorkShift();
                    data.loadData(row);
                    listData.Add(data);
                }
            }

            return listData;
        }

        public List<DropdownSubDistrict> GetDropdownSubDistrict(string lang, int districtID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_dropdown_sub_district " +
                "@pDistrictID, " +
                "@pLang");

            SqlParameter pDistrictID = new SqlParameter(@"pDistrictID", SqlDbType.Int);
            pDistrictID.Direction = ParameterDirection.Input;
            pDistrictID.Value = districtID;
            sql.Parameters.Add(pDistrictID);

            SqlParameter paramLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 5);
            paramLang.Direction = ParameterDirection.Input;
            paramLang.Value = lang;
            sql.Parameters.Add(paramLang);

            table = sql.executeQueryWithReturnTable();

            List<DropdownSubDistrict> listData = new List<DropdownSubDistrict>();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DropdownSubDistrict data = new DropdownSubDistrict();
                    data.loadData(row);
                    listData.Add(data);
                }
            }

            return listData;
        }

        public Department GetDepartment(int id)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_department " +
                "@pId");

            SqlParameter pId = new SqlParameter(@"pId", SqlDbType.Int);
            pId.Direction = ParameterDirection.Input;
            pId.Value = id;
            sql.Parameters.Add(pId);

            table = sql.executeQueryWithReturnTable();

            Department data = new Department();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public Pagination<SearchMasterDataDepartment> SearchMasterDepartment(SearchMasterDataDTO searchMasterDataDTO)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_department_page " +
                "@pNameEN, " +
                "@pNameTH, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar, 255);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = searchMasterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar, 255);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = searchMasterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = searchMasterDataDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = searchMasterDataDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = searchMasterDataDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = searchMasterDataDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTable();

            Pagination<SearchMasterDataDepartment> pagination = new Pagination<SearchMasterDataDepartment>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchMasterDataDepartment data = new SearchMasterDataDepartment();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchMasterDepartment(searchMasterDataDTO);

            pagination.SetPagination(total, searchMasterDataDTO.perPage, searchMasterDataDTO.pageInt);

            return pagination;
        }

        public int GetTotalSearchMasterDepartment(SearchMasterDataDTO searchMasterDataDTO)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_department_total " +
                "@pNameEN, " +
                "@pNameTH ");

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar, 255);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = searchMasterDataDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar, 255);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = searchMasterDataDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public _ReturnIdModel InsertMasterKey(string shareCode, MasterDataDTO masterDataDTO, string TableName, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_master_key " +
                "@pNameKey," +
                "@pUserID, " +
                "@pIsActive");

            SqlParameter pNameKey = new SqlParameter(@"pNameKey", SqlDbType.VarChar);
            pNameKey.Direction = ParameterDirection.Input;
            pNameKey.Value = masterDataDTO.keyName;
            sql.Parameters.Add(pNameKey);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            SqlParameter pIsActive = new SqlParameter(@"pIsActive", SqlDbType.Int);
            pIsActive.Direction = ParameterDirection.Input;
            pIsActive.Value = masterDataDTO.isActive;
            sql.Parameters.Add(pIsActive);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateMasterKey(string shareCode, MasterDataDTO masterDataDTO, string TableName, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_master_key " +
                "@pMasterID," +
                "@pNameKey," +
                "@pUserID, " +
                "@pIsActive");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = masterDataDTO.masterID;
            sql.Parameters.Add(pMasterID);

            SqlParameter pNameKey = new SqlParameter(@"pNameKey", SqlDbType.VarChar);
            pNameKey.Direction = ParameterDirection.Input;
            pNameKey.Value = masterDataDTO.keyName;
            sql.Parameters.Add(pNameKey);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            SqlParameter pIsActive = new SqlParameter(@"pIsActive", SqlDbType.Int);
            pIsActive.Direction = ParameterDirection.Input;
            pIsActive.Value = masterDataDTO.isActive;
            sql.Parameters.Add(pIsActive);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel DeleteMasterKey(string shareCode, MasterDataDTO masterDataDTO, string TableName, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec cancel_master_key " +
                "@pMasterID," +
                "@pUserID, " +
                "@pIsCancel");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = masterDataDTO.masterID;
            sql.Parameters.Add(pMasterID);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            SqlParameter pIsCancel = new SqlParameter(@"pIsCancel", SqlDbType.Int);
            pIsCancel.Direction = ParameterDirection.Input;
            pIsCancel.Value = masterDataDTO.IsCancel;
            sql.Parameters.Add(pIsCancel);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }


        public Pagination<SearchMasterKey> SearchMasterKey(SearchMasterDataDTO searchMasterDataDTO)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_master_key_page " +
                "@pKeyName, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pKeyName = new SqlParameter(@"pKeyName", SqlDbType.VarChar, 255);
            pKeyName.Direction = ParameterDirection.Input;
            pKeyName.Value = searchMasterDataDTO.paramSearch;
            sql.Parameters.Add(pKeyName);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = searchMasterDataDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = searchMasterDataDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = searchMasterDataDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = searchMasterDataDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTable();

            Pagination<SearchMasterKey> pagination = new Pagination<SearchMasterKey>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchMasterKey data = new SearchMasterKey();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchMasterKey(searchMasterDataDTO);

            pagination.SetPagination(total, searchMasterDataDTO.perPage, searchMasterDataDTO.pageInt);

            return pagination;
        }

        public int GetTotalSearchMasterKey(SearchMasterDataDTO searchMasterDataDTO)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_master_key_total " +
                "@pKeyName ");

            SqlParameter pKeyName = new SqlParameter(@"pKeyName", SqlDbType.VarChar, 255);
            pKeyName.Direction = ParameterDirection.Input;
            pKeyName.Value = searchMasterDataDTO.paramSearch;
            sql.Parameters.Add(pKeyName);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public MasterKey GetMasterKey(int id)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_master_key " +
                "@pId");

            SqlParameter pId = new SqlParameter(@"pId", SqlDbType.Int);
            pId.Direction = ParameterDirection.Input;
            pId.Value = id;
            sql.Parameters.Add(pId);

            table = sql.executeQueryWithReturnTable();

            MasterKey data = new MasterKey();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel InsertSystemMaster(string shareCode, SystemMasterDTO systemmasterDTO, string TableName, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_system_master " +
                "@pKeyID," +
                "@pValue," +
                "@pNameEN," +
                "@pNameTH," +
                "@pOrder," +
                "@pUserID ");

            SqlParameter pKeyID = new SqlParameter(@"pKeyID", SqlDbType.Int);
            pKeyID.Direction = ParameterDirection.Input;
            pKeyID.Value = systemmasterDTO.keyID;
            sql.Parameters.Add(pKeyID);

            SqlParameter pValue = new SqlParameter(@"pValue", SqlDbType.Int);
            pValue.Direction = ParameterDirection.Input;
            pValue.Value = systemmasterDTO.value;
            sql.Parameters.Add(pValue);

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar, 255);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = systemmasterDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar, 255);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = systemmasterDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pOrder = new SqlParameter(@"pOrder", SqlDbType.Int);
            pOrder.Direction = ParameterDirection.Input;
            pOrder.Value = systemmasterDTO.order;
            sql.Parameters.Add(pOrder);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateSystemMaster(string shareCode, SystemMasterDTO systemmasterDTO, string TableName, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_system_master " +
                "@pMasterID," +
                "@pKeyID," +
                "@pValue," +
                "@pNameEN," +
                "@pNameTH," +
                "@pOrder," +
                "@pUserID ");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = systemmasterDTO.masterID;
            sql.Parameters.Add(pMasterID);

            SqlParameter pKeyID = new SqlParameter(@"pKeyID", SqlDbType.Int);
            pKeyID.Direction = ParameterDirection.Input;
            pKeyID.Value = systemmasterDTO.keyID;
            sql.Parameters.Add(pKeyID);

            SqlParameter pValue = new SqlParameter(@"pValue", SqlDbType.Int);
            pValue.Direction = ParameterDirection.Input;
            pValue.Value = systemmasterDTO.value;
            sql.Parameters.Add(pValue);

            SqlParameter pNameEN = new SqlParameter(@"pNameEN", SqlDbType.VarChar, 255);
            pNameEN.Direction = ParameterDirection.Input;
            pNameEN.Value = systemmasterDTO.nameEN;
            sql.Parameters.Add(pNameEN);

            SqlParameter pNameTH = new SqlParameter(@"pNameTH", SqlDbType.VarChar, 255);
            pNameTH.Direction = ParameterDirection.Input;
            pNameTH.Value = systemmasterDTO.nameTH;
            sql.Parameters.Add(pNameTH);

            SqlParameter pOrder = new SqlParameter(@"pOrder", SqlDbType.Int);
            pOrder.Direction = ParameterDirection.Input;
            pOrder.Value = systemmasterDTO.order;
            sql.Parameters.Add(pOrder);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public Pagination<SearchSystemMaster> SearchSystemMaster(SearchSystemMasterDTO searchSystemMasterDTO,string lang)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_system_master_page " +
                "@pName, " +
                "@pStatus, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pName = new SqlParameter(@"pName", SqlDbType.VarChar, 255);
            pName.Direction = ParameterDirection.Input;
            pName.Value = searchSystemMasterDTO.name;
            sql.Parameters.Add(pName);

            SqlParameter pStatus = new SqlParameter(@"pStatus", SqlDbType.Int);
            pStatus.Direction = ParameterDirection.Input;
            pStatus.Value = searchSystemMasterDTO.status;
            sql.Parameters.Add(pStatus);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = searchSystemMasterDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = searchSystemMasterDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = searchSystemMasterDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = searchSystemMasterDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTable();

            Pagination<SearchSystemMaster> pagination = new Pagination<SearchSystemMaster>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchSystemMaster data = new SearchSystemMaster();
                    data.loadData(row);

                    data.allMasterInKey = new List<AllMasterInKey>();
                    data.allMasterInKey = GetAllMasterInKey(data.id,lang);

                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchSystemMaster(searchSystemMasterDTO);

            pagination.SetPagination(total, searchSystemMasterDTO.perPage, searchSystemMasterDTO.pageInt);

            return pagination;
        }

        public List<AllMasterInKey> GetAllMasterInKey(int keyID, string lang)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_master_in_key " +
                "@pKeyID, " +
                "@pLang");

            SqlParameter pKeyID = new SqlParameter(@"pKeyID", SqlDbType.Int);
            pKeyID.Direction = ParameterDirection.Input;
            pKeyID.Value = keyID;
            sql.Parameters.Add(pKeyID);

            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar,10);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = lang;
            sql.Parameters.Add(pLang);

            table = sql.executeQueryWithReturnTable();

            List<AllMasterInKey> listData = new List<AllMasterInKey>();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    AllMasterInKey data = new AllMasterInKey();
                    data.loadData(row);
                    listData.Add(data);
                }
            }

            return listData;
        }


        public int GetTotalSearchSystemMaster(SearchSystemMasterDTO searchSystemMasterDTO)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_system_master_total " +
                "@pName, " +
                "@pStatus");

            SqlParameter pName = new SqlParameter(@"pName", SqlDbType.VarChar, 255);
            pName.Direction = ParameterDirection.Input;
            pName.Value = searchSystemMasterDTO.name;
            sql.Parameters.Add(pName);

            SqlParameter pStatus = new SqlParameter(@"pStatus", SqlDbType.VarChar, 255);
            pStatus.Direction = ParameterDirection.Input;
            pStatus.Value = searchSystemMasterDTO.status;
            sql.Parameters.Add(pStatus);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public SystemMaster GetSystemMaster(int id, string lang)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_system_master_detail " +
                "@pKeyID");

            SqlParameter pKeyID = new SqlParameter(@"pKeyID", SqlDbType.Int);
            pKeyID.Direction = ParameterDirection.Input;
            pKeyID.Value = id;
            sql.Parameters.Add(pKeyID);

            table = sql.executeQueryWithReturnTable();

            SystemMaster data = new SystemMaster();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                    data.masterList = GetMasterDetail(id,lang);
                }
            }

            return data;
        }

        public List<MasterDetail> GetMasterDetail(int keyID, string lang)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_system_master_detail_body " +
                "@pKeyID, " +
                "@pLang");

            SqlParameter pKeyID = new SqlParameter(@"pKeyID", SqlDbType.Int);
            pKeyID.Direction = ParameterDirection.Input;
            pKeyID.Value = keyID;
            sql.Parameters.Add(pKeyID);

            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 10);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = lang;
            sql.Parameters.Add(pLang);

            table = sql.executeQueryWithReturnTable();

            List<MasterDetail> listData = new List<MasterDetail>();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    MasterDetail data = new MasterDetail();
                    data.loadData(row);
                    listData.Add(data);
                }
            }

            return listData;
        }

        public Pagination<EmpWorkShiftTimeSearch> GetWorkShiftTime(GetHistoryWorkShiftTimeDTO getHistoryWorkShiftTimeDTO, string Lang, string shareCode)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_history_change_work_shift_time_page " +
                "@pEmpID, " +
                "@pDate, " +
                "@pLang, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pEmpID = new SqlParameter(@"pEmpID", SqlDbType.Int);
            pEmpID.Direction = ParameterDirection.Input;
            pEmpID.Value = getHistoryWorkShiftTimeDTO.empId;
            sql.Parameters.Add(pEmpID);

            SqlParameter pDate = new SqlParameter(@"pDate", SqlDbType.Date);
            pDate.Direction = ParameterDirection.Input;
            pDate.Value = getHistoryWorkShiftTimeDTO.date;
            sql.Parameters.Add(pDate);

            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 2);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = Lang;
            sql.Parameters.Add(pLang);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = getHistoryWorkShiftTimeDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = getHistoryWorkShiftTimeDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = getHistoryWorkShiftTimeDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = getHistoryWorkShiftTimeDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<EmpWorkShiftTimeSearch> pagination = new Pagination<EmpWorkShiftTimeSearch>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    EmpWorkShiftTimeSearch data = new EmpWorkShiftTimeSearch();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalWorkShiftTime(getHistoryWorkShiftTimeDTO,shareCode);

            pagination.SetPagination(total, getHistoryWorkShiftTimeDTO.perPage, getHistoryWorkShiftTimeDTO.pageInt);

            return pagination;
        }

        public int GetTotalWorkShiftTime(GetHistoryWorkShiftTimeDTO getHistoryWorkShiftTimeDTO, string shareCode)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_history_change_work_shift_time_total " +
                "@pEmpID, " +
                "@pDate");

            SqlParameter pEmpID = new SqlParameter(@"pEmpID", SqlDbType.Int);
            pEmpID.Direction = ParameterDirection.Input;
            pEmpID.Value = getHistoryWorkShiftTimeDTO.empId;
            sql.Parameters.Add(pEmpID);

            SqlParameter pDate = new SqlParameter(@"pDate", SqlDbType.Date);
            pDate.Direction = ParameterDirection.Input;
            pDate.Value = getHistoryWorkShiftTimeDTO.date;
            sql.Parameters.Add(pDate);
            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public EmpWorkShiftTimeHeader GetWorkShiftTimeHeader(int empId, string shareCode, string Lang)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_detail_emp_work_shift " +
                "@pEmpID, " +
                "@pLang");

            SqlParameter pEmpID = new SqlParameter(@"pEmpID", SqlDbType.Int);
            pEmpID.Direction = ParameterDirection.Input;
            pEmpID.Value = empId;
            sql.Parameters.Add(pEmpID);

            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar,2);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = Lang;
            sql.Parameters.Add(pLang);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));


            EmpWorkShiftTimeHeader data = new EmpWorkShiftTimeHeader();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public HeaderDetail GetWorkShiftTimeHeaderByWTId(int id, string lang, string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_detail_work_shift_time " +
                "@pId, " +
                "@pLang");

            SqlParameter pId = new SqlParameter(@"pId", SqlDbType.Int);
            pId.Direction = ParameterDirection.Input;
            pId.Value = id;
            sql.Parameters.Add(pId);

            SqlParameter pLang = new SqlParameter(@"pLang", SqlDbType.VarChar,2);
            pLang.Direction = ParameterDirection.Input;
            pLang.Value = lang;
            sql.Parameters.Add(pLang);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            HeaderDetail data = new HeaderDetail();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public List<GetWorkShift> GetAllWorkShiftList(string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_work_shift ");

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            List<GetWorkShift> listData = new List<GetWorkShift>();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    GetWorkShift data = new GetWorkShift();
                    data.loadData(row);
                    listData.Add(data);
                }
            }

            return listData;
        }


        public SearchWorkShiftTimeAllTotalDTO GetWorkShiftTotalHeader(string shareCode, string userList, int workShiftID, string workDate)
        {
            int total = 0;
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_work_time_header " +
                "@pUserIDList, " +
                "@pWorkShiftID, " +
                "@pWorkDate");

            SqlParameter pUserIDList = new SqlParameter(@"pUserIDList", SqlDbType.VarChar, 200);
            pUserIDList.Direction = ParameterDirection.Input;
            pUserIDList.Value = userList;
            sql.Parameters.Add(pUserIDList);

            SqlParameter pWorkShiftID = new SqlParameter(@"pWorkShiftID", SqlDbType.Int);
            pWorkShiftID.Direction = ParameterDirection.Input;
            pWorkShiftID.Value = workShiftID;
            sql.Parameters.Add(pWorkShiftID);

            SqlParameter pWorkDate = new SqlParameter(@"pWorkDate", SqlDbType.VarChar,25);
            pWorkDate.Direction = ParameterDirection.Input;
            pWorkDate.Value = workDate;
            sql.Parameters.Add(pWorkDate);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            SearchWorkShiftTimeAllTotalDTO data = new SearchWorkShiftTimeAllTotalDTO();

            if (table != null)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }



        #region update active master
        public _ReturnIdModel UpdateActiveMaster(string shareCode, MasterDataDTO masterDataDTO, string TableName, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_active_master " +
                "@pMasterID," +
                "@pTableName," +
                "@pIsActive," +
                "@pUserID ");

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = masterDataDTO.masterID;
            sql.Parameters.Add(pMasterID);

            SqlParameter pTableName = new SqlParameter(@"pTableName", SqlDbType.VarChar);
            pTableName.Direction = ParameterDirection.Input;
            pTableName.Value = TableName;
            sql.Parameters.Add(pTableName);

            SqlParameter pIsActive = new SqlParameter(@"pIsActive", SqlDbType.VarChar);
            pIsActive.Direction = ParameterDirection.Input;
            pIsActive.Value = masterDataDTO.isActive;
            sql.Parameters.Add(pIsActive);

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            table = sql.executeQueryWithReturnTable();

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public int CheckIsActiveService(string tableName, int masterID)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec check_is_active " +
                "@pTableName, " +
                "@pMasterID");

            SqlParameter pTableName = new SqlParameter(@"pTableName", SqlDbType.VarChar, 255);
            pTableName.Direction = ParameterDirection.Input;
            pTableName.Value = tableName;
            sql.Parameters.Add(pTableName);

            SqlParameter pMasterID = new SqlParameter(@"pMasterID", SqlDbType.Int);
            pMasterID.Direction = ParameterDirection.Input;
            pMasterID.Value = masterID;
            sql.Parameters.Add(pMasterID);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        #endregion

        #region SystemRole
        public int CheckDuplicateObjID(int objID, string projectName, string shareCode)
        {
            int total = 0;
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec check_duplicate_object_id " +
                "@pObjID, " +
                "@pProjName");

            SqlParameter pObjID = new SqlParameter(@"pObjID", SqlDbType.Int);
            pObjID.Direction = ParameterDirection.Input;
            pObjID.Value = objID;
            sql.Parameters.Add(pObjID);

            SqlParameter pProjName = new SqlParameter(@"pProjName", SqlDbType.VarChar, 255);
            pProjName.Direction = ParameterDirection.Input;
            pProjName.Value = projectName;
            sql.Parameters.Add(pProjName);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public _ReturnIdModel InsertSystemRoleAssign(string shareCode, string projectName, SaveSystemRoleAssignDTO saveSystemRoleAssignDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_system_role_assignment " +
                "@pObjID, " +
                "@pPositionID, " +
                "@pProjName, " +
                "@pCreateBy");

            SqlParameter pObjID = new SqlParameter(@"pObjID", SqlDbType.VarChar, 10);
            pObjID.Direction = ParameterDirection.Input;
            pObjID.Value = saveSystemRoleAssignDTO.objectID;
            sql.Parameters.Add(pObjID);

            SqlParameter pPositionID = new SqlParameter(@"pPositionID", SqlDbType.Int);
            pPositionID.Direction = ParameterDirection.Input;
            pPositionID.Value = saveSystemRoleAssignDTO.positionID;
            sql.Parameters.Add(pPositionID);

            SqlParameter pProjName = new SqlParameter(@"pProjName", SqlDbType.VarChar, 255);
            pProjName.Direction = ParameterDirection.Input;
            pProjName.Value = projectName;
            sql.Parameters.Add(pProjName);

            SqlParameter pIsActive = new SqlParameter(@"pIsActive", SqlDbType.Int);
            pIsActive.Direction = ParameterDirection.Input;
            pIsActive.Value = 1;
            sql.Parameters.Add(pIsActive);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateSystemRoleAssign(string shareCode, string ProjectName, SaveSystemRoleAssignDTO saveSystemRoleAssignDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_system_role_assignment " +
                "@pObjID, " +
                "@pPositionID, " +
                "@pProjName, " +
                "@pIsActive, " +
                "@pUpdateBy");

            SqlParameter pObjID = new SqlParameter(@"pObjID", SqlDbType.VarChar, 10);
            pObjID.Direction = ParameterDirection.Input;
            pObjID.Value = saveSystemRoleAssignDTO.objectID;
            sql.Parameters.Add(pObjID);

            SqlParameter pPositionID = new SqlParameter(@"pPositionID", SqlDbType.Int);
            pPositionID.Direction = ParameterDirection.Input;
            pPositionID.Value = saveSystemRoleAssignDTO.positionID;
            sql.Parameters.Add(pPositionID);

            SqlParameter pProjName = new SqlParameter(@"pProjName", SqlDbType.VarChar, 255);
            pProjName.Direction = ParameterDirection.Input;
            pProjName.Value = ProjectName;
            sql.Parameters.Add(pProjName);

            SqlParameter pIsActive = new SqlParameter(@"pIsActive", SqlDbType.VarChar, 255);
            pIsActive.Direction = ParameterDirection.Input;
            pIsActive.Value = saveSystemRoleAssignDTO.isActive;
            sql.Parameters.Add(pIsActive);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateActiveSystemRoleAssign(string shareCode, string ProjectName, SaveSystemRoleAssignDTO saveSystemRoleAssignDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec cancel_system_role_assign " +
                "@pPId, " +
                "@pProjectName, " +
                "@pIsActive, " +
                "@pUpdateBy");

            SqlParameter pPId = new SqlParameter(@"pPId", SqlDbType.Int);
            pPId.Direction = ParameterDirection.Input;
            pPId.Value = saveSystemRoleAssignDTO.id;
            sql.Parameters.Add(pPId);

            SqlParameter pProjectName = new SqlParameter(@"pProjectName", SqlDbType.VarChar, 255);
            pProjectName.Direction = ParameterDirection.Input;
            pProjectName.Value = ProjectName.ToLower();
            sql.Parameters.Add(pProjectName);

            SqlParameter pIsActive = new SqlParameter(@"pIsActive", SqlDbType.Int);
            pIsActive.Direction = ParameterDirection.Input;
            pIsActive.Value = saveSystemRoleAssignDTO.isActive;
            sql.Parameters.Add(pIsActive);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateActiveSystemRoleTemp(string shareCode, string ProjectName, SaveSystemRoleTempDTO saveSystemRoleTempDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec cancel_system_role_template " +
                "@pObjectID, " +
                "@pParentID, " +
                "@pIsActive, " +
                "@pProjectName, " +
                "@pUpdateBy");

            SqlParameter pObjectID = new SqlParameter(@"pObjectID", SqlDbType.Int);
            pObjectID.Direction = ParameterDirection.Input;
            pObjectID.Value = saveSystemRoleTempDTO.objectID;
            sql.Parameters.Add(pObjectID);

            SqlParameter pParentID = new SqlParameter(@"pParentID", SqlDbType.Int);
            pParentID.Direction = ParameterDirection.Input;
            pParentID.Value = saveSystemRoleTempDTO.parentID;
            sql.Parameters.Add(pParentID);

            SqlParameter pIsActive = new SqlParameter(@"pIsActive", SqlDbType.Int);
            pIsActive.Direction = ParameterDirection.Input;
            pIsActive.Value = saveSystemRoleTempDTO.isActive;
            sql.Parameters.Add(pIsActive);

            SqlParameter pProjectName = new SqlParameter(@"pProjectName", SqlDbType.VarChar, 255);
            pProjectName.Direction = ParameterDirection.Input;
            pProjectName.Value = ProjectName;
            sql.Parameters.Add(pProjectName);
            
            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }


        public Pagination<SearchAllSystemRoleAssign> SearchAllSystemRoleAssign(string shareCode, SearchSystemRoleAssignDTO searchSystemRoleAssignDTO)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_system_role_assign_page " +
                "@pTextSearch, " +
                "@pId, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = searchSystemRoleAssignDTO.paramSearch;
            sql.Parameters.Add(pTextSearch);

            SqlParameter pId = new SqlParameter(@"pId", SqlDbType.Int);
            pId.Direction = ParameterDirection.Input;
            pId.Value = searchSystemRoleAssignDTO.pId;
            sql.Parameters.Add(pId);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = searchSystemRoleAssignDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = searchSystemRoleAssignDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = searchSystemRoleAssignDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = searchSystemRoleAssignDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<SearchAllSystemRoleAssign> pagination = new Pagination<SearchAllSystemRoleAssign>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchAllSystemRoleAssign data = new SearchAllSystemRoleAssign();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchAllSystemRoleAssign(shareCode, searchSystemRoleAssignDTO);

            pagination.SetPagination(total, searchSystemRoleAssignDTO.perPage, searchSystemRoleAssignDTO.pageInt);

            return pagination;
        }

        public int GetTotalSearchAllSystemRoleAssign(string shareCode, SearchSystemRoleAssignDTO searchSystemRoleAssignDTO)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_system_role_assign_total " +
                 "@pTextSearch, " +
                "@pId");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = searchSystemRoleAssignDTO.paramSearch;
            sql.Parameters.Add(pTextSearch);

            SqlParameter pId = new SqlParameter(@"pId", SqlDbType.Int);
            pId.Direction = ParameterDirection.Input;
            pId.Value = searchSystemRoleAssignDTO.pId;
            sql.Parameters.Add(pId);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public Assign GetDetailSystemRoleAssign(int id ,string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_detail_system_role_assign " +
                "@pId");

            SqlParameter pId = new SqlParameter(@"pId", SqlDbType.Int);
            pId.Direction = ParameterDirection.Input;
            pId.Value = id;
            sql.Parameters.Add(pId);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Assign data = new Assign();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel InsertSystemRoleTemp(string shareCode, SaveSystemRoleTempDTO saveSystemRoleTempDTO, int userID, string projectName)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_system_role_template " +
                "@pObjID, " +
                "@pParentID, " +
                "@pObjName, " +
                "@pProjName, " +
                "@pCreateBy");

            SqlParameter pObjID = new SqlParameter(@"pObjID", SqlDbType.VarChar, 10);
            pObjID.Direction = ParameterDirection.Input;
            pObjID.Value = saveSystemRoleTempDTO.objectID;
            sql.Parameters.Add(pObjID);

            SqlParameter pParentID = new SqlParameter(@"pParentID", SqlDbType.VarChar, 10);
            pParentID.Direction = ParameterDirection.Input;
            pParentID.Value = saveSystemRoleTempDTO.parentID;
            sql.Parameters.Add(pParentID);

            SqlParameter pObjName = new SqlParameter(@"pObjName", SqlDbType.VarChar, 255);
            pObjName.Direction = ParameterDirection.Input;
            pObjName.Value = saveSystemRoleTempDTO.objectName;
            sql.Parameters.Add(pObjName);

            SqlParameter pProjName = new SqlParameter(@"pProjName", SqlDbType.VarChar, 255);
            pProjName.Direction = ParameterDirection.Input;
            pProjName.Value = projectName;
            sql.Parameters.Add(pProjName);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public _ReturnIdModel UpdateSystemRoleTemp(string shareCode, SaveSystemRoleTempDTO saveSystemRoleTempDTO, int userID, string projectName)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_system_role_template " +
                "@pObjID, " +
                "@pParentID, " +
                "@pObjName, " +
                "@pProjectName, " +
                "@pIsActive, " +
                "@pUpdateBy");

            SqlParameter pObjID = new SqlParameter(@"pObjID", SqlDbType.VarChar, 10);
            pObjID.Direction = ParameterDirection.Input;
            pObjID.Value = saveSystemRoleTempDTO.objectID;
            sql.Parameters.Add(pObjID);

            SqlParameter pParentID = new SqlParameter(@"pParentID", SqlDbType.VarChar, 10);
            pParentID.Direction = ParameterDirection.Input;
            pParentID.Value = saveSystemRoleTempDTO.parentID;
            sql.Parameters.Add(pParentID);

            SqlParameter pObjName = new SqlParameter(@"pObjName", SqlDbType.VarChar, 255);
            pObjName.Direction = ParameterDirection.Input;
            pObjName.Value = saveSystemRoleTempDTO.objectName;
            sql.Parameters.Add(pObjName);

            SqlParameter pProjectName = new SqlParameter(@"pProjectName", SqlDbType.VarChar, 255);
            pProjectName.Direction = ParameterDirection.Input;
            pProjectName.Value = projectName;
            sql.Parameters.Add(pProjectName);

            SqlParameter pIsActive = new SqlParameter(@"pIsActive", SqlDbType.Int);
            pIsActive.Direction = ParameterDirection.Input;
            pIsActive.Value = saveSystemRoleTempDTO.isActive;
            sql.Parameters.Add(pIsActive);

            SqlParameter pUpdateBy = new SqlParameter(@"pUpdateBy", SqlDbType.Int);
            pUpdateBy.Direction = ParameterDirection.Input;
            pUpdateBy.Value = userID;
            sql.Parameters.Add(pUpdateBy);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            _ReturnIdModel data = new _ReturnIdModel();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public Pagination<SearchAllSystemRoleTemp> SearchAllSystemRoleTemp(string shareCode, SearchSystemRoleTempDTO searchSystemRoleTempDTO)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_system_role_temp_page " +
                "@pTextSearch, " +
                "@pId, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = searchSystemRoleTempDTO.paramSearch;
            sql.Parameters.Add(pTextSearch);

            SqlParameter pId = new SqlParameter(@"pId", SqlDbType.Int);
            pId.Direction = ParameterDirection.Input;
            pId.Value = searchSystemRoleTempDTO.pId;
            sql.Parameters.Add(pId);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = searchSystemRoleTempDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = searchSystemRoleTempDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = searchSystemRoleTempDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = searchSystemRoleTempDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<SearchAllSystemRoleTemp> pagination = new Pagination<SearchAllSystemRoleTemp>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchAllSystemRoleTemp data = new SearchAllSystemRoleTemp();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchAllSystemRoleTemp(shareCode, searchSystemRoleTempDTO);

            pagination.SetPagination(total, searchSystemRoleTempDTO.perPage, searchSystemRoleTempDTO.pageInt);

            return pagination;
        }

        public int GetTotalSearchAllSystemRoleTemp(string shareCode, SearchSystemRoleTempDTO searchSystemRoleTempDTO)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_system_role_temp_total " +
                 "@pTextSearch, " +
                "@pId");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = searchSystemRoleTempDTO.paramSearch;
            sql.Parameters.Add(pTextSearch);

            SqlParameter pId = new SqlParameter(@"pId", SqlDbType.Int);
            pId.Direction = ParameterDirection.Input;
            pId.Value = searchSystemRoleTempDTO.pId;
            sql.Parameters.Add(pId);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public Temp GetDetailSystemRoleTemp(SaveSystemRoleTempDTO saveSystemRoleTempDTO, string projectName, string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_detail_system_role_temp " +
                "@pObjectID, " +
                "@pParentID, " +
                "@pProjectName");

            SqlParameter pObjectID = new SqlParameter(@"pObjectID", SqlDbType.Int);
            pObjectID.Direction = ParameterDirection.Input;
            pObjectID.Value = saveSystemRoleTempDTO.objectID;
            sql.Parameters.Add(pObjectID);

            SqlParameter pParentID = new SqlParameter(@"pParentID", SqlDbType.Int);
            pParentID.Direction = ParameterDirection.Input;
            pParentID.Value = saveSystemRoleTempDTO.parentID;
            sql.Parameters.Add(pParentID);

            SqlParameter pProjectName = new SqlParameter(@"pProjectName", SqlDbType.VarChar,255);
            pProjectName.Direction = ParameterDirection.Input;
            pProjectName.Value = projectName;
            sql.Parameters.Add(pProjectName);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Temp data = new Temp();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }


        public int CheckPositionID(int positionID)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec check_position_id " +
                "@pPositionID");

            SqlParameter pPositionID = new SqlParameter(@"pPositionID", SqlDbType.Int);
            pPositionID.Direction = ParameterDirection.Input;
            pPositionID.Value = positionID;
            sql.Parameters.Add(pPositionID);
            
            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public int CheckPositionIDAssignment(string objID, int positionID, string shareCode)
        {
            int total = 0;
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec check_duplicate_position_id " +
                "@pPosiID, " +
                "@pObjID");

            SqlParameter pPosiID = new SqlParameter(@"pPosiID", SqlDbType.Int);
            pPosiID.Direction = ParameterDirection.Input;
            pPosiID.Value = positionID;
            sql.Parameters.Add(pPosiID);

            SqlParameter pObjID = new SqlParameter(@"pObjID", SqlDbType.VarChar, 10);
            pObjID.Direction = ParameterDirection.Input;
            pObjID.Value = objID;
            sql.Parameters.Add(pObjID);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }



        #endregion

        #region Report
        public Pagination<SearchAllSalaryReport> SearchAllSalaryReport(string shareCode, SearchReportDTO searchReportSalaryDTO)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_salary_report_page " +
                "@pParamSearch, " +
                "@pDepartmentList, " +
                "@pPositionList, " +
                "@pEmptypeList, " +
                "@pEmpStatusList, " +
                "@pSalaryFrom, " +
                "@pSalaryTo, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = searchReportSalaryDTO.paramSearch;
            sql.Parameters.Add(pParamSearch);

            SqlParameter pDepartmentList = new SqlParameter(@"pDepartmentList", SqlDbType.VarChar, 255);
            pDepartmentList.Direction = ParameterDirection.Input;
            pDepartmentList.Value = searchReportSalaryDTO.prepairDepartmentSearch;
            sql.Parameters.Add(pDepartmentList);

            SqlParameter pPositionList = new SqlParameter(@"pPositionList", SqlDbType.VarChar, 255);
            pPositionList.Direction = ParameterDirection.Input;
            pPositionList.Value = searchReportSalaryDTO.prepairPositionSearch;
            sql.Parameters.Add(pPositionList);

            SqlParameter pEmptypeList = new SqlParameter(@"pEmptypeList", SqlDbType.VarChar, 255);
            pEmptypeList.Direction = ParameterDirection.Input;
            pEmptypeList.Value = searchReportSalaryDTO.prepairEmpTypeSearch;
            sql.Parameters.Add(pEmptypeList);

            SqlParameter pEmpStatusList = new SqlParameter(@"pEmpStatusList", SqlDbType.VarChar, 255);
            pEmpStatusList.Direction = ParameterDirection.Input;
            pEmpStatusList.Value = searchReportSalaryDTO.prepairEmpStatusSearch;
            sql.Parameters.Add(pEmpStatusList);

            SqlParameter pSalaryFrom = new SqlParameter(@"pSalaryFrom", SqlDbType.Int);
            pSalaryFrom.Direction = ParameterDirection.Input;
            pSalaryFrom.Value = searchReportSalaryDTO.salaryFrom;
            sql.Parameters.Add(pSalaryFrom);

            SqlParameter pSalaryTo = new SqlParameter(@"pSalaryTo", SqlDbType.Int);
            pSalaryTo.Direction = ParameterDirection.Input;
            pSalaryTo.Value = searchReportSalaryDTO.salaryTo;
            sql.Parameters.Add(pSalaryTo);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = searchReportSalaryDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = searchReportSalaryDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = searchReportSalaryDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = searchReportSalaryDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<SearchAllSalaryReport> pagination = new Pagination<SearchAllSalaryReport>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchAllSalaryReport data = new SearchAllSalaryReport();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchAllSalaryReport(shareCode, searchReportSalaryDTO);

            pagination.SetPagination(total, searchReportSalaryDTO.perPage, searchReportSalaryDTO.pageInt);

            return pagination;
        }

        public int GetTotalSearchAllSalaryReport(string shareCode, SearchReportDTO searchReportSalaryDTO)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_salary_report_total " +
                "@pParamSearch, " +
                "@pDepartmentList, " +
                "@pPositionList, " +
                "@pEmptypeList, " +
                "@pEmpStatusList, " +
                "@pSalaryFrom, " +
                "@pSalaryTo");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = searchReportSalaryDTO.paramSearch;
            sql.Parameters.Add(pParamSearch);

            SqlParameter pDepartmentList = new SqlParameter(@"pDepartmentList", SqlDbType.VarChar, 255);
            pDepartmentList.Direction = ParameterDirection.Input;
            pDepartmentList.Value = searchReportSalaryDTO.prepairDepartmentSearch;
            sql.Parameters.Add(pDepartmentList);

            SqlParameter pPositionList = new SqlParameter(@"pPositionList", SqlDbType.VarChar, 255);
            pPositionList.Direction = ParameterDirection.Input;
            pPositionList.Value = searchReportSalaryDTO.prepairPositionSearch;
            sql.Parameters.Add(pPositionList);

            SqlParameter pEmptypeList = new SqlParameter(@"pEmptypeList", SqlDbType.VarChar, 255);
            pEmptypeList.Direction = ParameterDirection.Input;
            pEmptypeList.Value = searchReportSalaryDTO.prepairEmpTypeSearch;
            sql.Parameters.Add(pEmptypeList);

            SqlParameter pEmpStatusList = new SqlParameter(@"pEmpStatusList", SqlDbType.VarChar, 255);
            pEmpStatusList.Direction = ParameterDirection.Input;
            pEmpStatusList.Value = searchReportSalaryDTO.prepairEmpStatusSearch;
            sql.Parameters.Add(pEmpStatusList);

            SqlParameter pSalaryFrom = new SqlParameter(@"pSalaryFrom", SqlDbType.Int);
            pSalaryFrom.Direction = ParameterDirection.Input;
            pSalaryFrom.Value = searchReportSalaryDTO.salaryFrom;
            sql.Parameters.Add(pSalaryFrom);

            SqlParameter pSalaryTo = new SqlParameter(@"pSalaryTo", SqlDbType.Int);
            pSalaryTo.Direction = ParameterDirection.Input;
            pSalaryTo.Value = searchReportSalaryDTO.salaryTo;
            sql.Parameters.Add(pSalaryTo);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public Pagination<SearchAllEmployeeReportBody> SearchAllEmployeeReport(string shareCode, SearchReportDTO searchReportEmployeeDTO)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_employment_report_page " +
                "@pParamSearch, " +
                "@pDepartmentList, " +
                "@pPositionList, " +
                "@pEmptypeList, " +
                "@pEmpStatusList, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = searchReportEmployeeDTO.paramSearch;
            sql.Parameters.Add(pParamSearch);

            SqlParameter pDepartmentList = new SqlParameter(@"pDepartmentList", SqlDbType.VarChar, 255);
            pDepartmentList.Direction = ParameterDirection.Input;
            pDepartmentList.Value = searchReportEmployeeDTO.prepairDepartmentSearch;
            sql.Parameters.Add(pDepartmentList);

            SqlParameter pPositionList = new SqlParameter(@"pPositionList", SqlDbType.VarChar, 255);
            pPositionList.Direction = ParameterDirection.Input;
            pPositionList.Value = searchReportEmployeeDTO.prepairPositionSearch;
            sql.Parameters.Add(pPositionList);

            SqlParameter pEmptypeList = new SqlParameter(@"pEmptypeList", SqlDbType.VarChar, 255);
            pEmptypeList.Direction = ParameterDirection.Input;
            pEmptypeList.Value = searchReportEmployeeDTO.prepairEmpTypeSearch;
            sql.Parameters.Add(pEmptypeList);

            SqlParameter pEmpStatusList = new SqlParameter(@"pEmpStatusList", SqlDbType.VarChar, 255);
            pEmpStatusList.Direction = ParameterDirection.Input;
            pEmpStatusList.Value = searchReportEmployeeDTO.prepairEmpStatusSearch;
            sql.Parameters.Add(pEmpStatusList);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = searchReportEmployeeDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = searchReportEmployeeDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = searchReportEmployeeDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = searchReportEmployeeDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<SearchAllEmployeeReportBody> pagination = new Pagination<SearchAllEmployeeReportBody>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchAllEmployeeReportBody data = new SearchAllEmployeeReportBody();
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchAllEmployeeReport(shareCode, searchReportEmployeeDTO);

            pagination.SetPagination(total, searchReportEmployeeDTO.perPage, searchReportEmployeeDTO.pageInt);

            return pagination;
        }

        public int GetTotalSearchAllEmployeeReport(string shareCode, SearchReportDTO searchReportEmployeeDTO)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_employment_report_total " +
                "@pParamSearch, " +
                "@pDepartmentList, " +
                "@pPositionList, " +
                "@pEmptypeList, " +
                "@pEmpStatusList");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = searchReportEmployeeDTO.paramSearch;
            sql.Parameters.Add(pParamSearch);

            SqlParameter pDepartmentList = new SqlParameter(@"pDepartmentList", SqlDbType.VarChar, 255);
            pDepartmentList.Direction = ParameterDirection.Input;
            pDepartmentList.Value = searchReportEmployeeDTO.prepairDepartmentSearch;
            sql.Parameters.Add(pDepartmentList);

            SqlParameter pPositionList = new SqlParameter(@"pPositionList", SqlDbType.VarChar, 255);
            pPositionList.Direction = ParameterDirection.Input;
            pPositionList.Value = searchReportEmployeeDTO.prepairPositionSearch;
            sql.Parameters.Add(pPositionList);

            SqlParameter pEmptypeList = new SqlParameter(@"pEmptypeList", SqlDbType.VarChar, 255);
            pEmptypeList.Direction = ParameterDirection.Input;
            pEmptypeList.Value = searchReportEmployeeDTO.prepairEmpTypeSearch;
            sql.Parameters.Add(pEmptypeList);

            SqlParameter pEmpStatusList = new SqlParameter(@"pEmpStatusList", SqlDbType.VarChar, 255);
            pEmpStatusList.Direction = ParameterDirection.Input;
            pEmpStatusList.Value = searchReportEmployeeDTO.prepairEmpStatusSearch;
            sql.Parameters.Add(pEmpStatusList);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public EmployeeReportHeader GetEmployeeReportHeader(string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_employment_report_header ");

           
            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            EmployeeReportHeader data = new EmployeeReportHeader();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    data.loadData(row);
                }
            }

            return data;
        }

        public Pagination<SearchAllWorkTimeReport> SearchAllWorkTimeReport(string shareCode, SearchReportDTO searchReportWorkTimeDTO)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_work_time_header_report_page " +
                "@pParamSearch, " +
                "@pFromDate, " +
                "@pToDate, " +
                "@pDepartmentList, " +
                "@pPositionList, " +
                "@pEmptypeList, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = searchReportWorkTimeDTO.paramSearch;
            sql.Parameters.Add(pParamSearch);

            SqlParameter pFromDate = new SqlParameter(@"pFromDate", SqlDbType.VarChar, 255);
            pFromDate.Direction = ParameterDirection.Input;
            pFromDate.Value = searchReportWorkTimeDTO.dateFrom;
            sql.Parameters.Add(pFromDate);

            SqlParameter pToDate = new SqlParameter(@"pToDate", SqlDbType.VarChar, 255);
            pToDate.Direction = ParameterDirection.Input;
            pToDate.Value = searchReportWorkTimeDTO.dateTo;
            sql.Parameters.Add(pToDate);

            SqlParameter pDepartmentList = new SqlParameter(@"pDepartmentList", SqlDbType.VarChar, 255);
            pDepartmentList.Direction = ParameterDirection.Input;
            pDepartmentList.Value = searchReportWorkTimeDTO.prepairDepartmentSearch;
            sql.Parameters.Add(pDepartmentList);

            SqlParameter pPositionList = new SqlParameter(@"pPositionList", SqlDbType.VarChar, 255);
            pPositionList.Direction = ParameterDirection.Input;
            pPositionList.Value = searchReportWorkTimeDTO.prepairPositionSearch;
            sql.Parameters.Add(pPositionList);

            SqlParameter pEmptypeList = new SqlParameter(@"pEmptypeList", SqlDbType.VarChar, 255);
            pEmptypeList.Direction = ParameterDirection.Input;
            pEmptypeList.Value = searchReportWorkTimeDTO.prepairEmpTypeSearch;
            sql.Parameters.Add(pEmptypeList);

            SqlParameter pPage = new SqlParameter(@"pPage", SqlDbType.Int);
            pPage.Direction = ParameterDirection.Input;
            pPage.Value = searchReportWorkTimeDTO.pageInt;
            sql.Parameters.Add(pPage);

            SqlParameter pPerPage = new SqlParameter(@"pPerPage", SqlDbType.Int);
            pPerPage.Direction = ParameterDirection.Input;
            pPerPage.Value = searchReportWorkTimeDTO.perPage;
            sql.Parameters.Add(pPerPage);

            SqlParameter pSortField = new SqlParameter(@"pSortField", SqlDbType.Int);
            pSortField.Direction = ParameterDirection.Input;
            pSortField.Value = searchReportWorkTimeDTO.sortField;
            sql.Parameters.Add(pSortField);

            SqlParameter pSortType = new SqlParameter(@"pSortType", SqlDbType.VarChar, 1);
            pSortType.Direction = ParameterDirection.Input;
            pSortType.Value = searchReportWorkTimeDTO.sortType;
            sql.Parameters.Add(pSortType);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            Pagination<SearchAllWorkTimeReport> pagination = new Pagination<SearchAllWorkTimeReport>();


            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    SearchAllWorkTimeReport data = new SearchAllWorkTimeReport();
                    data.loadData(row);

                    data.detailList = new List<WorkTimeDetail>();
                    data.detailList = GetAllWorkTimeDetailList(data.empUserID, searchReportWorkTimeDTO.dateFrom, searchReportWorkTimeDTO.dateTo, shareCode);

                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchAllWorkTimeReport(shareCode, searchReportWorkTimeDTO);

            pagination.SetPagination(total, searchReportWorkTimeDTO.perPage, searchReportWorkTimeDTO.pageInt);

            return pagination;
        }

        public int GetTotalSearchAllWorkTimeReport(string shareCode, SearchReportDTO searchReportWorkTimeDTO)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_work_time_header_report_total " +
                "@pParamSearch, " +
                "@pFromDate, " +
                "@pToDate, " +
                "@pDepartmentList, " +
                "@pPositionList, " +
                "@pEmptypeList");

            SqlParameter pParamSearch = new SqlParameter(@"pParamSearch", SqlDbType.VarChar, 255);
            pParamSearch.Direction = ParameterDirection.Input;
            pParamSearch.Value = searchReportWorkTimeDTO.paramSearch;
            sql.Parameters.Add(pParamSearch);

            SqlParameter pFromDate = new SqlParameter(@"pFromDate", SqlDbType.VarChar, 255);
            pFromDate.Direction = ParameterDirection.Input;
            pFromDate.Value = searchReportWorkTimeDTO.dateFrom;
            sql.Parameters.Add(pFromDate);

            SqlParameter pToDate = new SqlParameter(@"pToDate", SqlDbType.VarChar, 255);
            pToDate.Direction = ParameterDirection.Input;
            pToDate.Value = searchReportWorkTimeDTO.dateTo;
            sql.Parameters.Add(pToDate);

            SqlParameter pDepartmentList = new SqlParameter(@"pDepartmentList", SqlDbType.VarChar, 255);
            pDepartmentList.Direction = ParameterDirection.Input;
            pDepartmentList.Value = searchReportWorkTimeDTO.prepairDepartmentSearch;
            sql.Parameters.Add(pDepartmentList);

            SqlParameter pPositionList = new SqlParameter(@"pPositionList", SqlDbType.VarChar, 255);
            pPositionList.Direction = ParameterDirection.Input;
            pPositionList.Value = searchReportWorkTimeDTO.prepairPositionSearch;
            sql.Parameters.Add(pPositionList);

            SqlParameter pEmptypeList = new SqlParameter(@"pEmptypeList", SqlDbType.VarChar, 255);
            pEmptypeList.Direction = ParameterDirection.Input;
            pEmptypeList.Value = searchReportWorkTimeDTO.prepairEmpTypeSearch;
            sql.Parameters.Add(pEmptypeList);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow dr = table.Rows[0];
                    total = int.Parse(dr["total"].ToString());
                }
            }

            return total;
        }

        public List<WorkTimeDetail> GetAllWorkTimeDetailList(int userID,string dateFrom, string dateTo, string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_work_time_report_page " +
                "@pUserID, " +
                "@pFromDate, " +
                "@pToDate");

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Value = userID;
            sql.Parameters.Add(pUserID);

            SqlParameter pFromDate = new SqlParameter(@"pFromDate", SqlDbType.VarChar, 255);
            pFromDate.Direction = ParameterDirection.Input;
            pFromDate.Value = dateFrom;
            sql.Parameters.Add(pFromDate);

            SqlParameter pToDate = new SqlParameter(@"pToDate", SqlDbType.VarChar, 255);
            pToDate.Direction = ParameterDirection.Input;
            pToDate.Value = dateTo;
            sql.Parameters.Add(pToDate);

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            List<WorkTimeDetail> listData = new List<WorkTimeDetail>();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    WorkTimeDetail data = new WorkTimeDetail();
                    data.loadData(row);
                    listData.Add(data);
                }
            }

            return listData;
        }


        #endregion

    }

    public class SQLCustomExecute
    {
        public List<SqlParameter> Parameters { get; set; }
        public string sqlCommand { get; set; }
        
        public SQLCustomExecute(string sqlCommand)
        {
            this.sqlCommand = sqlCommand;
            this.Parameters = new List<SqlParameter>();
        }

        public DataTable executeQueryWithReturnTable()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);

            CultureInfo cultureInfo = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            cultureInfo.DateTimeFormat.DateSeparator = "-";
            cultureInfo.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            Thread.CurrentThread.CurrentCulture = cultureInfo;

            DataTable result = null;

            string connectionString = WebConfigurationManager.AppSettings["connectionStrings"];

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = this.sqlCommand;

                if (this.Parameters != null)
                    foreach (SqlParameter parameter in this.Parameters)
                        command.Parameters.Add(parameter);
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        result = new DataTable();
                        if (reader.HasRows)
                        {
                            result.Load(reader);
                        }
                        command.Parameters.Clear();
                    }
                    command.Parameters.Clear();

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Dispose();
                    connection.Close();
                }
            }
            return result;
        }
        public DataTable executeQueryWithReturnTableOther(string shareCode)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);

            CultureInfo cultureInfo = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            cultureInfo.DateTimeFormat.DateSeparator = "-";
            cultureInfo.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            Thread.CurrentThread.CurrentCulture = cultureInfo;

            DataTable result = null;
            
            DecodeString decode = new DecodeString();

            string connectionString = decode.Connection(shareCode);

            //connectionString = ConfigurationManager.AppSettings["connectionStringsLocal"];

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = this.sqlCommand;

                if (this.Parameters != null)
                    foreach (SqlParameter parameter in this.Parameters)
                        command.Parameters.Add(parameter);
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        result = new DataTable();
                        if (reader.HasRows)
                        {
                            result.Load(reader);
                        }
                        command.Parameters.Clear();
                    }
                    command.Parameters.Clear();

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Dispose();
                    connection.Close();
                }
            }
            return result;
        }


    }
}
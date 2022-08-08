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

        public int InsertLogReceiveData(string pServiceName, string pReceiveData, string pTimeStampNow, string pAuthorization, int pUserID, string pType)
        {
            int id = 0;
            
            ipAddress = GetIP();

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_log_receive_data @pServiceName, @pReceiveData, @pTimeStampNow, @pAuthorization, @pUserID, " +
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
        public int InsertLogReceiveDataWithShareCode(string shareCode, string pServiceName, string pReceiveData, string pTimeStampNow, HeadersDTO headersDTO, int pUserID, string pType)
        {
            string json_header = JsonConvert.SerializeObject(headersDTO);
            InsertLogReceiveData(pServiceName, json_header, pTimeStampNow, headersDTO.authHeader, pUserID, pType);

            int id = 0;

            ipAddress = GetIP();

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_log_receive_data @pServiceName, @pReceiveData, @pTimeStampNow, @pAuthorization, @pUserID, " +
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

        public List<RoleIDList> GetUserRole(int pUserID, string pLang)
        {
            List<RoleIDList> list = new List<RoleIDList>();

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_user_role @pUserID, @pLang");

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
                RoleIDList data;
                foreach (DataRow row in table.Rows)
                {
                    data = new RoleIDList();
                    data.loadDataUserRole(row);
                    list.Add(data);
                }
            }
            return list;
        }

        public List<ShareHolderList> GetUserShareHolder(int pUserID, string pLang)
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
                "@pCreateBy");

            SqlParameter pUserName = new SqlParameter(@"pUserName", SqlDbType.VarChar, 200);
            pUserName.Direction = ParameterDirection.Input;
            pUserName.Value = saveEmpProfileDTO.userName;
            sql.Parameters.Add(pUserName);

            SqlParameter pPositionID = new SqlParameter(@"pPositionID", SqlDbType.Int);
            pPositionID.Direction = ParameterDirection.Input;
            pPositionID.Value = saveEmpProfileDTO.positionID;
            sql.Parameters.Add(pPositionID);

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

                "@pBodySetID, " +
                
                "@pCreateBy ");

            SqlParameter pUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            pUserID.Direction = ParameterDirection.Input;
            pUserID.Value = saveEmpProfileDTO.newUserID;
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

            SqlParameter pHeight = new SqlParameter(@"pHeight", SqlDbType.Float);
            pHeight.Direction = ParameterDirection.Input;
            pHeight.Value = saveEmpProfileDTO.height;
            sql.Parameters.Add(pHeight);

            SqlParameter pWeight = new SqlParameter(@"pWeight", SqlDbType.Float);
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

            SqlParameter pBodySetID = new SqlParameter(@"pBodySetID", SqlDbType.Int);
            pBodySetID.Direction = ParameterDirection.Input;
            pBodySetID.Value = saveEmpProfileDTO.bodySetID;
            sql.Parameters.Add(pBodySetID);

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

        public InsertLogin InsertEmpAddress(string shareCode, SaveEmpProfileDTO saveEmpProfileDTO, int userID)
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
            pUserID.Value = saveEmpProfileDTO.newUserID;
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
            pUserID.Value = saveEmpProfileDTO.newUserID;
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
            pUserID.Value = saveEmpProfileDTO.newUserID;
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
            pEmergencyContact.Value = saveEmergencyContact.emergencyContactID;
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
                "@pProductCd, " +
                "@pRateStaff, " +
                "@pRateManager," +
                "@pRateOwner, " +
                "@pRateConfirm, " +
                "@pCreateBy");

            SqlParameter paramEmpID = new SqlParameter(@"pEmpID", SqlDbType.Int);
            paramEmpID.Direction = ParameterDirection.Input;
            paramEmpID.Value = saveEmpRateDTO.empID;
            sql.Parameters.Add(paramEmpID);

            SqlParameter paramProductCd = new SqlParameter(@"pProductCd", SqlDbType.VarChar, 10);
            paramProductCd.Direction = ParameterDirection.Input;
            paramProductCd.Value = saveEmpRateDTO.productCode;
            sql.Parameters.Add(paramProductCd);

            SqlParameter paramRateStaff = new SqlParameter(@"pRateStaff", SqlDbType.Int);
            paramRateStaff.Direction = ParameterDirection.Input;
            paramRateStaff.Value = saveEmpRateDTO.rateStaff;
            sql.Parameters.Add(paramRateStaff);

            SqlParameter paramRateManager = new SqlParameter(@"pRateManager", SqlDbType.Int);
            paramRateManager.Direction = ParameterDirection.Input;
            paramRateManager.Value = saveEmpRateDTO.rateManager;
            sql.Parameters.Add(paramRateManager);

            SqlParameter paramRateOwner = new SqlParameter(@"pRateOwner", SqlDbType.Int);
            paramRateOwner.Direction = ParameterDirection.Input;
            paramRateOwner.Value = saveEmpRateDTO.rateOwner;
            sql.Parameters.Add(paramRateOwner);

            SqlParameter paramRateConfirm = new SqlParameter(@"pRateConfirm", SqlDbType.Int);
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

        public _ReturnIdModel InsertEmpWorkShift(SaveEmpWorkShiftRequestDTO saveEmpWorkShiftRequestDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_emp_work_shift " +
                "@pWsCode, " +
                "@pTimeStart, " +
                "@pTimeEnd, " +
                "@pWorkTypeID," +
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

                "@pImageProfileCode, " +
                "@pImageGalleryCode, " +

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

            SqlParameter pHeight = new SqlParameter(@"pHeight", SqlDbType.Float);
            pHeight.Direction = ParameterDirection.Input;
            pHeight.Value = saveEmpProfileDTO.height;
            sql.Parameters.Add(pHeight);

            SqlParameter pWeight = new SqlParameter(@"pWeight", SqlDbType.Float);
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

            SqlParameter pImageProfileCode = new SqlParameter(@"pImageProfileCode", SqlDbType.VarChar, 250);
            pImageProfileCode.Direction = ParameterDirection.Input;
            pImageProfileCode.Value = saveEmpProfileDTO.imageProfileCode;
            sql.Parameters.Add(pImageProfileCode);

            SqlParameter pImageGalleryCode = new SqlParameter(@"pImageGalleryCode", SqlDbType.VarChar, 250);
            pImageGalleryCode.Direction = ParameterDirection.Input;
            pImageGalleryCode.Value = saveEmpProfileDTO.imageGalleryCode;
            sql.Parameters.Add(pImageGalleryCode);

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
            pUserID.Value = saveEmpProfileDTO.newUserID;
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
                "@pProductCd, " +
                "@pRateStaff, " +
                "@pRateManager," +
                "@pRateOwner," +
                "@pRateConfirm," +
                "@pUpdateBy");

            SqlParameter paramID = new SqlParameter(@"pId", SqlDbType.Int);
            paramID.Direction = ParameterDirection.Input;
            paramID.Value = saveEmpRateDTO.empRateID;
            sql.Parameters.Add(paramID);

            SqlParameter paramProductCd = new SqlParameter(@"pProductCd", SqlDbType.VarChar, 10);
            paramProductCd.Direction = ParameterDirection.Input;
            paramProductCd.Value = saveEmpRateDTO.productCode;
            sql.Parameters.Add(paramProductCd);

            SqlParameter paramRateStaff = new SqlParameter(@"pRateStaff", SqlDbType.Int);
            paramRateStaff.Direction = ParameterDirection.Input;
            paramRateStaff.Value = saveEmpRateDTO.rateStaff;
            sql.Parameters.Add(paramRateStaff);

            SqlParameter paramRateManager = new SqlParameter(@"pRateManager", SqlDbType.Int);
            paramRateManager.Direction = ParameterDirection.Input;
            paramRateManager.Value = saveEmpRateDTO.rateManager;
            sql.Parameters.Add(paramRateManager);

            SqlParameter paramRateOwner = new SqlParameter(@"pRateOwner", SqlDbType.Int);
            paramRateOwner.Direction = ParameterDirection.Input;
            paramRateOwner.Value = saveEmpRateDTO.rateOwner;
            sql.Parameters.Add(paramRateOwner);

            SqlParameter paramRateConfirm = new SqlParameter(@"pRateConfirm", SqlDbType.Int);
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

        public _ReturnIdModel UpdateEmpWorkShift(SaveEmpWorkShiftRequestDTO saveEmpWorkShiftRequestDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_emp_work_shift " +
                "@pId, " +
                "@pWsCode, " +
                "@pTimeStart, " +
                "@pTimeEnd, " +
                "@pWorkTypeID," +
                "@pUpdateBy");

            SqlParameter paramID = new SqlParameter(@"pId", SqlDbType.Int);
            paramID.Direction = ParameterDirection.Input;
            paramID.Value = saveEmpWorkShiftRequestDTO.empWorkShiftID;
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

        public _ReturnIdModel UpdateLeaveDetail(SaveLeaveDetailDTO saveLeaveDetailDTO, int userID,string shareCode)
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

        public _ReturnIdModel CancelLeaveForm(int leaveID, int userID, string cancelReason,string shareCode)
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
                "@pLeaveID," +
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

        public _ReturnIdModel RejectLeaveForm(int leaveID, int userID, string rejectReason, string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_reject_leave " +
                "@pLeaveID," +
                "@pRejectReason," +
                "@pRejectBy");

            SqlParameter pLeaveID = new SqlParameter(@"pLeaveID", SqlDbType.Int);
            pLeaveID.Direction = ParameterDirection.Input;
            pLeaveID.Value = leaveID;
            sql.Parameters.Add(pLeaveID);

            SqlParameter pRejectReason = new SqlParameter(@"pRejectReason", SqlDbType.VarChar);
            pRejectReason.Direction = ParameterDirection.Input;
            pRejectReason.Value = rejectReason;
            sql.Parameters.Add(pRejectReason);

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


        public _ReturnIdModel UpdateEmpStatus(string shareCode, SaveEmpStatusDTO saveEmpStatusDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_emp_status " +
                "@pEmpId, " +
                "@pStatus, " +
                "@pUpdateBy");

            SqlParameter paramID = new SqlParameter(@"pEmpId", SqlDbType.Int);
            paramID.Direction = ParameterDirection.Input;
            paramID.Value = saveEmpStatusDTO.empID;
            sql.Parameters.Add(paramID);

            SqlParameter paramStatus = new SqlParameter(@"pStatus", SqlDbType.VarChar, 10);
            paramStatus.Direction = ParameterDirection.Input;
            paramStatus.Value = saveEmpStatusDTO.status;
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

            SqlParameter paramActionName = new SqlParameter(@"pActionName", SqlDbType.VarChar, 20);
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
            paramId.Value = requestDTO.empWorkShiftID;
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

            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

            return table;
        }

        public Pagination<SearchMasterData> SearchMasterDataPosition(string paramSearch, int perPage, int pageInt, int sortField, string sortType)
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

            int total = GetTotalSearchMasterPosition(paramSearch);

            pagination.SetPagination(total, perPage, pageInt);

            return pagination;
        }

        public int GetTotalSearchMasterPosition(string paramSearch)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_position_total " +
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

        public MasterData GetMasterPosition(int id)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_master_position " +
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

        public GetLeaveDetail GetLeaveDetail(GetLeaveDetailRequestDTO leaveDetailDTO,string shareCode)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_leave_detail " +
                "@pID ,"+
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

        public GetEmpWorkShift GetEmpWorkShift(int empWorkShiftID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_emp_work_shift " +
                "@pID");

            SqlParameter paramID = new SqlParameter(@"pID", SqlDbType.Int);
            paramID.Direction = ParameterDirection.Input;
            paramID.Value = empWorkShiftID;
            sql.Parameters.Add(paramID);

            table = sql.executeQueryWithReturnTable();

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

        public int GetTotalSearchAllEmployee(string shareCode, PageRequestDTO pageRequest)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_employee_total " +
                 "@pTextSearch, " +
                "@pDepartmentID, " +
                "@pEmpType, " +
                "@pStatus " );

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = pageRequest.paramSearch;
            sql.Parameters.Add(pTextSearch);

            SqlParameter pDepartmentID = new SqlParameter(@"pDepartmentID", SqlDbType.VarChar, 255);
            pDepartmentID.Direction = ParameterDirection.Input;
            pDepartmentID.Value = pageRequest.departmentSearch;
            sql.Parameters.Add(pDepartmentID);

            SqlParameter pEmpType = new SqlParameter(@"pEmpType", SqlDbType.VarChar, 255);
            pEmpType.Direction = ParameterDirection.Input;
            pEmpType.Value = pageRequest.empTypeSearch;
            sql.Parameters.Add(pEmpType);

            SqlParameter pStatus = new SqlParameter(@"pStatus", SqlDbType.VarChar, 255);
            pStatus.Direction = ParameterDirection.Input;
            pStatus.Value = pageRequest.statusSearch;
            sql.Parameters.Add(pStatus);

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

        public _ReturnIdModel InsertSystemLogChange(int actionID, string tableName, string fieldName, string newData, int userID)
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

            SqlParameter pFieldName = new SqlParameter(@"pFieldName", SqlDbType.VarChar,100);
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
                "@pUserID ");

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

        public _ReturnIdModel UpdateMasterData(string shareCode, MasterDataDTO masterDataDTO, string TableName, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec update_master_data " +
                "@pMasterID," +
                "@pTableName," +
                "@pNameEN," +
                "@pNameTH," +
                "@pUserID ");

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
            
            table = sql.executeQueryWithReturnTableOther(getConnectionEncoded(shareCode));

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
                "@pParamSearch " );

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

        public List<EmployeeDetails.ImageGallary> GetImgGallary(string shareCode, int userID)
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

            List<EmployeeDetails.ImageGallary> listData = new List<EmployeeDetails.ImageGallary>();

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    EmployeeDetails.ImageGallary data = new EmployeeDetails.ImageGallary();
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

        public Pagination<SearchAllEmployee> SearchAllEmployee(string shareCode, PageRequestDTO pageRequest)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_employee " +
                "@pTextSearch, " +
                "@pDepartmentID, " +
                "@pEmpType, " +
                "@pStatus, " +
                "@pLang, " +
                "@pPage, " +
                "@pPerPage, " +
                "@pSortField, " +
                "@pSortType");

            SqlParameter pTextSearch = new SqlParameter(@"pTextSearch", SqlDbType.VarChar, 255);
            pTextSearch.Direction = ParameterDirection.Input;
            pTextSearch.Value = pageRequest.paramSearch;
            sql.Parameters.Add(pTextSearch);

            SqlParameter pDepartmentID = new SqlParameter(@"pDepartmentID", SqlDbType.VarChar, 255);
            pDepartmentID.Direction = ParameterDirection.Input;
            pDepartmentID.Value = pageRequest.departmentSearch;
            sql.Parameters.Add(pDepartmentID);

            SqlParameter pEmpType = new SqlParameter(@"pEmpType", SqlDbType.VarChar, 255);
            pEmpType.Direction = ParameterDirection.Input;
            pEmpType.Value = pageRequest.empTypeSearch;
            sql.Parameters.Add(pEmpType);

            SqlParameter pStatus = new SqlParameter(@"pStatus", SqlDbType.VarChar, 255);
            pStatus.Direction = ParameterDirection.Input;
            pStatus.Value = pageRequest.statusSearch;
            sql.Parameters.Add(pStatus);

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

        public Pagination<SearchAllLeave> SearchAllLeave(string shareCode, SearchLeaveDTO searchLeaveDTO)
        {
            DataTable table = new DataTable();

            SQLCustomExecute sql = new SQLCustomExecute("exec get_search_all_leave_page " +
                "@pTextSearch, " +
                "@pLeaveTypeID, " +
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
                    data.loadData(row);
                    pagination.data.Add(data);
                }
            }

            int total = GetTotalSearchAllLeave(shareCode, searchLeaveDTO);

            pagination.SetPagination(total, searchLeaveDTO.perPage, searchLeaveDTO.pageInt);

            return pagination;
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

        public DataTable GetAllEmpCode()
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_emp_code ");

            table = sql.executeQueryWithReturnTable();

            return table;
        }

        public DataTable GetAllWorkShift()
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_work_shift ");

            table = sql.executeQueryWithReturnTable();

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
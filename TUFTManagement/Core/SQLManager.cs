using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
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

        public List<MenuList> GetAllMenuMain(int pRoleID, string pLang)
        {
            List<MenuList> list = new List<MenuList>();

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_menu_main @pRoleID, @pLang");

            SqlParameter paramRoleID = new SqlParameter(@"pRoleID", SqlDbType.Int);
            paramRoleID.Direction = ParameterDirection.Input;
            paramRoleID.Value = pRoleID;

            SqlParameter paramLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 5);
            paramLang.Direction = ParameterDirection.Input;
            paramLang.Value = pLang;

            sql.Parameters.Add(paramRoleID);
            sql.Parameters.Add(paramLang);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                MenuList data;
                foreach (DataRow row in table.Rows)
                {
                    data = new MenuList();
                    data.loadDataMenu(row);

                    data.menuSub = new List<MenuList>();
                    data.menuSub = GetAllMenuSub(pRoleID, data.parentID, pLang);

                    list.Add(data);
                }

            }
            return list;
        }

        public List<MenuList> GetAllMenuSub(int pRoleID, int pParentID, string pLang)
        {
            List<MenuList> list = new List<MenuList>();

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec get_all_menu_sub @pRoleID, @pParentID, @pLang");

            SqlParameter paramRoleID = new SqlParameter(@"pRoleID", SqlDbType.Int);
            paramRoleID.Direction = ParameterDirection.Input;
            paramRoleID.Value = pRoleID;

            SqlParameter paramParentID = new SqlParameter(@"pParentID", SqlDbType.Int);
            paramParentID.Direction = ParameterDirection.Input;
            paramParentID.Value = pParentID;

            SqlParameter paramLang = new SqlParameter(@"pLang", SqlDbType.VarChar, 5);
            paramLang.Direction = ParameterDirection.Input;
            paramLang.Value = pLang;

            sql.Parameters.Add(paramRoleID);
            sql.Parameters.Add(paramParentID);
            sql.Parameters.Add(paramLang);

            table = sql.executeQueryWithReturnTable();

            if (table != null && table.Rows.Count > 0)
            {
                MenuList data;
                foreach (DataRow row in table.Rows)
                {
                    data = new MenuList();
                    data.loadDataMenu(row);
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

        public int CheckUniqueNameInsertEmp(InsertEmpProfileDTO InsertEmpProfileDTO, int pUserID)
        {
            int total = 0;

            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec check_name_first_login " +
                "@pFirstNameTh, " +
                "@pLastNameTh, " +
                "@pUserID ");

            SqlParameter pFirstNameTh = new SqlParameter(@"pFirstNameTh", SqlDbType.VarChar, 150);
            pFirstNameTh.Direction = ParameterDirection.Input;
            pFirstNameTh.Value = InsertEmpProfileDTO.firstNameTH;
            sql.Parameters.Add(pFirstNameTh);

            SqlParameter pLastNameTh = new SqlParameter(@"pLastNameTh", SqlDbType.VarChar, 150);
            pLastNameTh.Direction = ParameterDirection.Input;
            pLastNameTh.Value = InsertEmpProfileDTO.lastNameTH;
            sql.Parameters.Add(pLastNameTh);

            SqlParameter paramUserID = new SqlParameter(@"pUserID", SqlDbType.Int);
            paramUserID.Direction = ParameterDirection.Input;
            paramUserID.Value = pUserID;
            sql.Parameters.Add(paramUserID);

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

        public InsertLogin InsertEmpProfile(InsertEmpProfileDTO insertEmpProfileDTO, int userID)
        {
            DataTable table = new DataTable();
            SQLCustomExecute sql = new SQLCustomExecute("exec insert_emp_profile " +
                "@pEmpCode, " +
                "@pUserName, " +
                "@pPassWord, " +
                "@pIdentityCard, " +
                "@pTitleID, " +
                "@pFirstnameEN," +
                "@pFirstnameTH," +
                "@pLastnameEN," +
                "@pLastnameTH," +
                "@pNickName," +
                "@pContact," +
                "@pPositionID," +
                "@pPersonalCode," +
                "@pPersonalNO," +
                "@pDateOfBirth," +
                "@pProductID," +
                "@pJoinDate," +
                "@pProPassDate," +
                "@pMonthlySalary," +
                "@pDailySalary," +
                "@pCondition," +
                "@pTokenID," +
                "@pRoleID," +
                "@pCreateBy");

            SqlParameter pEmpCode = new SqlParameter(@"pEmpCode", SqlDbType.VarChar, 10);
            pEmpCode.Direction = ParameterDirection.Input;
            pEmpCode.Value = insertEmpProfileDTO.empCode;
            sql.Parameters.Add(pEmpCode);

            SqlParameter pUserName = new SqlParameter(@"pUserName", SqlDbType.VarChar, 200);
            pUserName.Direction = ParameterDirection.Input;
            pUserName.Value = insertEmpProfileDTO.userName;
            sql.Parameters.Add(pUserName);

            SqlParameter pPassWord = new SqlParameter(@"@pPassWord", SqlDbType.VarChar, 250);
            pPassWord.Direction = ParameterDirection.Input;
            pPassWord.Value = insertEmpProfileDTO.password;
            sql.Parameters.Add(pPassWord);

            SqlParameter pIdentityCard = new SqlParameter(@"pIdentityCard", SqlDbType.VarChar, 30);
            pIdentityCard.Direction = ParameterDirection.Input;
            pIdentityCard.Value = insertEmpProfileDTO.identityCard;
            sql.Parameters.Add(pIdentityCard);

            SqlParameter pTitleID = new SqlParameter(@"pTitleID", SqlDbType.Int);
            pTitleID.Direction = ParameterDirection.Input;
            pTitleID.Value = insertEmpProfileDTO.titleID;
            sql.Parameters.Add(pTitleID);

            SqlParameter pFirstnameEN = new SqlParameter(@"pFirstnameEN", SqlDbType.VarChar, 250);
            pFirstnameEN.Direction = ParameterDirection.Input;
            pFirstnameEN.Value = insertEmpProfileDTO.firstNameEN;
            sql.Parameters.Add(pFirstnameEN);

            SqlParameter pFirstnameTH = new SqlParameter(@"pFirstnameTH", SqlDbType.VarChar, 250);
            pFirstnameTH.Direction = ParameterDirection.Input;
            pFirstnameTH.Value = insertEmpProfileDTO.firstNameTH;
            sql.Parameters.Add(pFirstnameTH);

            SqlParameter pLastnameEN = new SqlParameter(@"pLastnameEN", SqlDbType.VarChar, 250);
            pLastnameEN.Direction = ParameterDirection.Input;
            pLastnameEN.Value = insertEmpProfileDTO.lastNameEN;
            sql.Parameters.Add(pLastnameEN);
        
            SqlParameter pLastnameTH = new SqlParameter(@"pLastnameTH", SqlDbType.VarChar, 250);
            pLastnameTH.Direction = ParameterDirection.Input;
            pLastnameTH.Value = insertEmpProfileDTO.lastNameTH;
            sql.Parameters.Add(pLastnameTH);

            SqlParameter pNickName = new SqlParameter(@"pNickName", SqlDbType.VarChar, 100);
            pNickName.Direction = ParameterDirection.Input;
            pNickName.Value = insertEmpProfileDTO.roleID;
            sql.Parameters.Add(pNickName);

            SqlParameter pContact = new SqlParameter(@"pContact", SqlDbType.VarChar, 15);
            pContact.Direction = ParameterDirection.Input;
            pContact.Value = insertEmpProfileDTO.contact;
            sql.Parameters.Add(pContact);

            SqlParameter pPositionID = new SqlParameter(@"pPositionID", SqlDbType.Int);
            pPositionID.Direction = ParameterDirection.Input;
            pPositionID.Value = insertEmpProfileDTO.positionID;
            sql.Parameters.Add(pPositionID);

            SqlParameter pPersonalCode = new SqlParameter(@"pPersonalCode", SqlDbType.VarChar, 10);
            pPersonalCode.Direction = ParameterDirection.Input;
            pPersonalCode.Value = insertEmpProfileDTO.personalCode;
            sql.Parameters.Add(pPersonalCode);

            SqlParameter pPersonalNO = new SqlParameter(@"pPersonalNO", SqlDbType.Int);
            pPersonalNO.Direction = ParameterDirection.Input;
            pPersonalNO.Value = insertEmpProfileDTO.personalNO;
            sql.Parameters.Add(pPersonalNO);

            SqlParameter pDateOfBirth = new SqlParameter(@"pDateOfBirth", SqlDbType.Date);
            pDateOfBirth.Direction = ParameterDirection.Input;
            pDateOfBirth.Value = insertEmpProfileDTO.dateOfBirth;
            sql.Parameters.Add(pDateOfBirth);

            SqlParameter pProductID = new SqlParameter(@"pProductID", SqlDbType.Int);
            pProductID.Direction = ParameterDirection.Input;
            pProductID.Value = insertEmpProfileDTO.productID;
            sql.Parameters.Add(pProductID);

            SqlParameter pJoinDate = new SqlParameter(@"pJoinDate", SqlDbType.Date);
            pJoinDate.Direction = ParameterDirection.Input;
            pJoinDate.Value = insertEmpProfileDTO.joinDate;
            sql.Parameters.Add(pJoinDate);

            SqlParameter pProPassDate = new SqlParameter(@"pProPassDate", SqlDbType.Date);
            pProPassDate.Direction = ParameterDirection.Input;
            pProPassDate.Value = insertEmpProfileDTO.proPassDate;
            sql.Parameters.Add(pProPassDate);

            SqlParameter pMonthlySalary = new SqlParameter(@"pMonthlySalary", SqlDbType.Decimal);
            pMonthlySalary.Direction = ParameterDirection.Input;
            pMonthlySalary.Value = insertEmpProfileDTO.monthlySalary;
            sql.Parameters.Add(pMonthlySalary);

            SqlParameter pDailySalary = new SqlParameter(@"pDailySalary", SqlDbType.Decimal);
            pDailySalary.Direction = ParameterDirection.Input;
            pDailySalary.Value = insertEmpProfileDTO.dailySalary;
            sql.Parameters.Add(pDailySalary);

            SqlParameter pCondition = new SqlParameter(@"pCondition", SqlDbType.VarChar, 20);
            pCondition.Direction = ParameterDirection.Input;
            pCondition.Value = insertEmpProfileDTO.condition;
            sql.Parameters.Add(pCondition);

            SqlParameter pTokenID = new SqlParameter(@"pTokenID", SqlDbType.VarChar, 4000);
            pTokenID.Direction = ParameterDirection.Input;
            pTokenID.Value = insertEmpProfileDTO.tokenID;
            sql.Parameters.Add(pTokenID);
            
            SqlParameter pRoleID = new SqlParameter(@"pRoleID", SqlDbType.Int);
            pRoleID.Direction = ParameterDirection.Input;
            pRoleID.Value = insertEmpProfileDTO.productID;
            sql.Parameters.Add(pRoleID);

            SqlParameter pCreateBy = new SqlParameter(@"pCreateBy", SqlDbType.Int);
            pCreateBy.Direction = ParameterDirection.Input;
            pCreateBy.Value = userID;
            sql.Parameters.Add(pCreateBy);

            table = sql.executeQueryWithReturnTable();

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
    }
}
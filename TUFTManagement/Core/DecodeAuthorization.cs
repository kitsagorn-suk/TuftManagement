using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TUFTManagement.Models;

namespace TUFTManagement.Core
{
    public class DecodeAuthorization
    {
        public static AuthorizationModel AuthorizationDecode(string auth)
        {
            AuthorizationModel dataAuth = new AuthorizationModel();

            try
            {
                string[] data = auth.Split('.');
                if (data.Length != 3)
                {
                    throw new Exception();
                }
                string header = data[0];
                string payload = data[1];
                string signature = data[2];

                string decodeHeader = Utility.Base64ForUrlDecode(header);
                string decodePayload = Utility.Base64ForUrlDecode(payload);

                JToken tokenHeader = JObject.Parse(decodeHeader);
                JToken tokenPayload = JObject.Parse(decodePayload);

                dataAuth.userID = int.Parse(tokenPayload["user_id"].ToString());
                dataAuth.roleIDList = tokenPayload["role_id"].ToString();
                dataAuth.createDate = Convert.ToDouble(tokenPayload["create_date"].ToString());
                dataAuth.expireDate = Convert.ToDouble(tokenPayload["expire_date"].ToString());
                dataAuth.userName = tokenHeader["username"].ToString();
                dataAuth.password = tokenHeader["password"].ToString();
                dataAuth.typeLogin = tokenHeader["platform_login"].ToString();
                dataAuth.signal = tokenHeader["signal"].ToString();
                dataAuth.shareCodeList = tokenPayload["business_code"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return dataAuth;
        }
    }
}
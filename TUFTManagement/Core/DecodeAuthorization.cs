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
                dataAuth.userName = tokenHeader["username"].ToString();
                dataAuth.password = tokenHeader["password"].ToString();                
                dataAuth.createDate = Convert.ToDouble(tokenPayload["create_date"].ToString());
                dataAuth.expireDate = Convert.ToDouble(tokenPayload["expire_date"].ToString());
                dataAuth.fromProject = tokenHeader["fromProject"].ToString();
                dataAuth.signal = tokenHeader["signal"].ToString();
                dataAuth.roleIDList = tokenPayload["roleIDList"].ToString();
                dataAuth.shareCodeList = tokenPayload["shareCodeList"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return dataAuth;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using TUFTManagement.Models;

namespace TUFTManagement.Core
{
    public class GenAuthorization
    {
        public static string GetAuthorization(string username, string password, string signal, string fromProject, CheckUserByTokenModel dataFormToken)
        {
            SQLManager _sql = SQLManager.Instance;
            int expire_token = int.Parse(WebConfigurationManager.AppSettings["expire_token"]);

            double timestampNow = Utility.DateTimeToUnixTimestamp(DateTime.Now);
            double timestampExpire = Utility.DateTimeToUnixTimestamp(DateTime.Now.AddHours(expire_token));
            
            string header = "";
            header = "{";
            header += " \"username\":\"" + username + "\",";
            header += " \"password\":\"" + password + "\",";
            header += " \"fromProject\":\"" + fromProject + "\",";
            header += " \"signal\":\"" + signal + "\"";
            header += " }";

            string payload = "";
            payload = "{";
            payload += " \"expire_date\":" + timestampExpire + ",";
            payload += " \"create_date\":" + timestampNow + ",";
            payload += " \"user_id\":" + dataFormToken.userID + ",";
            payload += " \"roleIDList\":" + dataFormToken.roleIDList + ",";
            payload += " \"shareCodeList\":\"" + dataFormToken.shareCodeList + "\",";
            payload += " }";

            string encryptedHeader = Utility.Base64UrlEncode(header);
            string encryptedPayload = Utility.Base64UrlEncode(payload);
            string encryptedSignature = Utility.sha256(encryptedHeader + "." + encryptedPayload + signal);
            string result = encryptedHeader + "." + encryptedPayload + "." + encryptedSignature;

            return result;
        }
    }
}
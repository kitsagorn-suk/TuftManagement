using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TUFTManagement.Core;
using TUFTManagement.DTO;

namespace TUFTManagement.Models
{
    public class ValidationModel
    {
        public bool Success { get; set; }
        public int InvalidCode { get; set; }
        public string InvalidText { get; set; }
        public string InvalidMessage { get; set; }

        public enum InvalidState
        {
            E501, E502, E503, E504, E505, E506,
            S200001, S201001, S200002, S200003, S200004,
            E300001, E300002, E300003, E300004, E300005, E300006, E300007, E300008,
            E301001, E301002, E301003, E301004, E301005, E301006, E301007, E301008, E301009,
            E302001, E302002, E302003,
            E303001, E303002

        }

        public static int GetInvalidCode(InvalidState state)
        {
            switch (state)
            {
                case InvalidState.E501:
                    return 501;
                case InvalidState.E502:
                    return 502;
                case InvalidState.E503:
                    return 503;
                case InvalidState.E504:
                    return 504;
                case InvalidState.E505:
                    return 505;
                case InvalidState.E506:
                    return 506;
                case InvalidState.S201001:
                    return 201001;
                case InvalidState.E302001:
                    return 302001;
                case InvalidState.E301001:
                    return 301001;
                case InvalidState.E301002:
                    return 301002;
                case InvalidState.E301003:
                    return 301003;
                case InvalidState.E301004:
                    return 301004;
                case InvalidState.E301005:
                    return 301005;
                case InvalidState.E301006:
                    return 301006;
                case InvalidState.E301007:
                    return 301007;
                case InvalidState.E301008:
                    return 301008;
                case InvalidState.E301009:
                    return 301009;
                default:
                    return 0;
            }
        }

        public static GetMessageTopicDTO GetInvalidMessage(InvalidState state, string lang)
        {
            SQLManager _sql = SQLManager.Instance;
            GetMessageTopicDTO message = new GetMessageTopicDTO();
            int messagecode = 0;
            switch (state)
            {
                case InvalidState.E501:
                    message.message = "เข้าสู่ระบบด้วยวิธีการไม่ถูกต้อง";
                    return message;
                case InvalidState.E502:
                    message.message = "Authorization หมดอายุ";
                    return message;
                case InvalidState.E503:
                    message.message = "Authorization ไม่ถูกต้อง";
                    return message;
                case InvalidState.E504:
                    message.message = "ProjectToken ไม่ตรงกัน";
                    return message;
                case InvalidState.E505:
                    message.message = "มีการเข้าสู่ระบบจากเครื่องอื่นกรุณา login ใหม่";
                    return message;
                case InvalidState.E506:
                    message.message = "ShareCode ไม่ตรงกัน";
                    return message;
                case InvalidState.S201001:
                    messagecode = 201001;
                    message = _sql.GetMessageLang(lang.ToLower(), messagecode);
                    return message;
                case InvalidState.E301001:
                    messagecode = 301001;
                    message = _sql.GetMessageLang(lang.ToLower(), messagecode);
                    //message = "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง";
                    return message;
                case InvalidState.E301002:
                    messagecode = 301002;
                    message = _sql.GetMessageLang(lang.ToLower(), messagecode);
                    //message = "คุณยังไม่ได้เปลี่ยนรหัสผ่านครั้งแรก";
                    return message;
                case InvalidState.E301003:
                    messagecode = 301003;
                    message = _sql.GetMessageLang(lang.ToLower(), messagecode);
                    //message = "บัญชีของคุณถูกระงับ โปรดติดต่อฝ่ายบุคคล";
                    return message;
                case InvalidState.E301004:
                    messagecode = 301004;
                    message = _sql.GetMessageLang(lang.ToLower(), messagecode);
                    return message;
                case InvalidState.E301005:
                    messagecode = 301005;
                    message = _sql.GetMessageLang(lang.ToLower(), messagecode);
                    return message;
                case InvalidState.E301006:
                    messagecode = 301006;
                    message = _sql.GetMessageLang(lang.ToLower(), messagecode);
                    return message;
                case InvalidState.E301007:
                    messagecode = 301007;
                    message = _sql.GetMessageLang(lang.ToLower(), messagecode);
                    return message;
                case InvalidState.E301008:
                    messagecode = 301008;
                    message = _sql.GetMessageLang(lang.ToLower(), messagecode);
                    return message;
                case InvalidState.E301009:
                    messagecode = 301009;
                    message = _sql.GetMessageLang(lang.ToLower(), messagecode);
                    return message;
                case InvalidState.E302001:
                    messagecode = 302001;
                    message = _sql.GetMessageLang(lang.ToLower(), messagecode);
                    return message;
                
                default:
                    return message;

            }
        }
    }
}
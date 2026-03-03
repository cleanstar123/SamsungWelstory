using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class UserModel
    {
        public string USER_ID         { get; set; }
        public string RESTAURANT_CODE { get; set; }
        public string RESTAURANT_NM   { get; set; }
        public string USER_NM         { get; set; }
        public string GROUP_CD        { get; set; }
        public string USER_PW         { get; set; }
        public string USE_YN          { get; set; }
        public string REG_ID          { get; set; }
        public string REG_DTM         { get; set; }
        public string MOD_ID          { get; set; }
        public string MOD_DTM         { get; set; }
    }
}
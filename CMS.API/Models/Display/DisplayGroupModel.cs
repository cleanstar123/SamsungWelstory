using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class DisplayGroupModel
    {
        public string RESTAURANT_CODE    { get; set; }
        public string DISPLAY_GROUP_ID   { get; set; }
        public string DISPLAY_GROUP_NM   { get; set; }
        public string DISPLAY_GROUP_DESC { get; set; }
        public string REG_ID             { get; set; }
        public string REG_DTM            { get; set; }
        public string MOD_ID             { get; set; }
        public string MOD_DTM            { get; set; }
    }
}
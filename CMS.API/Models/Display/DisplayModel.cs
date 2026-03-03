using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.API.Models
{
    public class DisplayModel
    {
        public string RESTAURANT_CODE { get; set; }
        public string DISPLAY_ID      { get; set; }
        public string DISPLAY_NM      { get; set; }
        public string DISPLAY_DESC    { get; set; }
        public string SCREEN_W        { get; set; }
        public string SCREEN_H        { get; set; }
        public string DISPLAY_OS      { get; set; }
        public string DISPLAY_IP      { get; set; }
        public string DISPLAY_MAC     { get; set; }
        public string USE_YN          { get; set; }
        public string REG_ID          { get; set; }
        public string REG_DTM         { get; set; }
        public string MOD_ID          { get; set; }
        public string MOD_DTM         { get; set; }

        public string DISPLAY_OS_LIST_SELECTED { get; set; }
        public IEnumerable<SelectListItem> DISPLAY_OS_LIST = new List<SelectListItem>();

        public string USE_YN_SELECTED { get; set; }
        public IEnumerable<SelectListItem> USE_YN_LIST = new List<SelectListItem>();

        public string DISPLAY_SELECTED { get; set; }
        public IEnumerable<SelectListItem> DISPLAY_LIST = new List<SelectListItem>();
    }
}
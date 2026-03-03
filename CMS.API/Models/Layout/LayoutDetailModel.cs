using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class LayoutDetailModel
    {
        public string RESTAURANT_CODE { get; set; }
        public string LAYOUT_ID       { get; set; }
        public string LAYOUT_SEQ      { get; set; }
        public string CONTENT_TYPE    { get; set; }
        public string CONTENT_TYPE_NM { get; set; }

    }
}
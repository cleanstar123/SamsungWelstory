using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class TemplateModel
    {
        public string RESTAURANT_CODE { get; set; }
        public string TEMPLATE_ID     { get; set; }
        public string TEMPLATE_NM     { get; set; }
        public string TEMPLATE_DESC   { get; set; }
        public string FILE_PATH       { get; set; }
        public string FILE_NM         { get; set; }
        public string FILE_EXT        { get; set; }
        public string FILE_SIZE       { get; set; }
        public string TEMPLATE_URL    { get; set; }
        public string REG_ID          { get; set; }
        public string REG_DTM         { get; set; }
        public string MOD_ID          { get; set; }
        public string MOD_DTM         { get; set; }
        public string THUMBNAIL_NM    { get; set; }

        public string TEMPLATE_HTML   { get; set; }
        public string LAYOUT_ID       { get; set; }
        public string LAYOUT_FILE_NM  { get; set; }

        public string PAGE_CNT        { get; set; }
        public string PAGE_NO         { get; set; }

        public string SCREEN_W        { get; set; }
        public string SCREEN_H        { get; set; }

        public List<TemplateMapModel> templateMapModels = new List<TemplateMapModel>();

        public LayoutModel layoutModel { get; set; }
        public List<LayoutDetailModel> layoutDetailModels = new List<LayoutDetailModel>();
    }
}
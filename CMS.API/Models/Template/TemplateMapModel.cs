using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class TemplateMapModel
    {
        public string RESTAURANT_CODE   { get; set; }
        public string TEMPLATE_ID       { get; set; }
        public string LAYOUT_SEQ        { get; set; }
        public string CONTENT_ID        { get; set; }
        public string CONTENT_TYPE      { get; set; }
        public string CONNECTION_TYPE   { get; set; }
        public string CONNECTION_NM     { get; set; }
        public string CONTENT_FILE_PATH { get; set; }
        public string CONTENT_FILE_NM   { get; set; }
        public string FILE_NM           { get; set; }
        public string REG_ID            { get; set; }
        public string REG_DTM           { get; set; }
        public bool   IS_UPDATE         { get; set; }  /* 템플릿 수정시 연결 정보가 수정되었는지 판단 (true : 수정됨, false : 수정않됨) */
    }
}
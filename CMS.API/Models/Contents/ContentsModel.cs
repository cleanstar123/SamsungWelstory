using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace CMS.API.Models
{
    public class ContentsModel
    {
        private string _TEXT_HTML = string.Empty;

        public string RESTAURANT_CODE  { get; set; }
        public string CONTENT_ID       { get; set; }
        public string CONTENT_NM       { get; set; }
        public string CONTENT_TYPE     { get; set; }
        public string FILE_PATH        { get; set; }    /* 원본 파일 경로 */
        public string FILE_NM          { get; set; }    /* 원본 파일 이름 */
        public string FILE_EXT         { get; set; }    /* 파일 확장자 */
        public string FILE_SIZE        { get; set; }    /* 파일 크기 */
        public string VIDEO_DURATION   { get; set; }    /* 동영상 재생 시간 */
        public string FILE_URL         { get; set; }    /* 웹에서 사용할 URL */
        public string ORGIN_FILE_URL   { get; set; }    /* 웹에서 사용할 URL (수정에서 사용) */
        public string THUMBNAIL_PATH   { get; set; }    /* 썸네일 파일 경로 */
        public string THUMBNAIL_NM     { get; set; }    /* 썸네일 파일 이름 */
        public string THUMBNAIL_URL    { get; set; }    /* 웹에서 사용할 URL */
        public string TEMPLATE_ORG_URL { get; set; }    /* 템플릿 생성시 참조할 URL */
        public string REG_ID           { get; set; }
        public string REG_DTM          { get; set; }
        public string MOD_ID           { get; set; }
        public string MOD_DTM          { get; set; }
        public string PAGE_CNT         { get; set; }
        public string PAGE_NO          { get; set; }
        public bool   IS_FILE_UPDATE   = false;    /* 파일 컨텐츠 수정 여부 */
        public string TEXT_HTML
        {
            get { return _TEXT_HTML; }
            set { this._TEXT_HTML = WebUtility.HtmlEncode(value); }
        }    /* 텍스트 컨텐츠일 경우 HTML이 들어감 */


        
    }
}
namespace CMS.API.Models
{
    public class LayoutModel
    {
        
        public string RESTAURANT_CODE { get; set; }
        public string LAYOUT_ID       { get; set; }
        public string LAYOUT_TYPE     { get; set; }
        public string LAYOUT_NM       { get; set; }
        public string LAYOUT_DESC     { get; set; }
        public string FILE_NM         { get; set; }
        public string CONTENT_CNT     { get; set; }
        public string SCREEN_W        { get; set; }
        public string SCREEN_H        { get; set; }
        public string SCREEN_W_H      { get; set; }
        public string LAYOUT_HV_TYPE  { get; set; }
        public string LAYOUT_H_BY_V   { get; set; }
        public string EVAL_USE_YN     { get; set; }
        public string REG_ID          { get; set; }
        public string REG_DTM         { get; set; }
        public string MOD_ID          { get; set; }
        public string MOD_DTM         { get; set; }
        public string THUMBNAIL_NM    { get; set; }

        /* 페이징 */
        public string PAGE_CNT        { get; set; }
        public string PAGE_NO         { get; set; }
    }
}
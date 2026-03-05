using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Configuration;
using CMS.API.Models;

namespace CMS.API.App_Code
{
    public static class CommonProperties
    {
        #region DB(PostgreSQL) 연결 정보
        public static string ConnectionString          { get { return ConfigurationManager.ConnectionStrings["CMS_DB"].ToString(); } }
        #endregion

        #region 저장 경로

        // LAYOUT_UPLOAD_PATH

        public static string LAYOUT_UPLOAD_PATH         { get { return string.Format("upload/layout");   } }
        public static string CONTENTS_UPLOAD_PATH       { get { return string.Format("upload/contents"); } }
        public static string TEMPLATE_WRAPPER_SAVE_PATH { get { return string.Format("upload/wrapper");  } }
        public static string TEMPLATE_SAVE_PATH(string restaurantCode, string templateId) { return Path.Combine("upload/template", restaurantCode, templateId); }   // 템플릿 저장 경로
        
        #endregion

        #region 도메인 정보
        public static string HTTP_DOMAIN_URL  = WebConfigurationManager.AppSettings["HTTP_DOMAIN_URL"];
        public static string HTTPS_DOMAIN_URL = WebConfigurationManager.AppSettings["HTTPS_DOMAIN_URL"];
        #endregion

        #region 썸네일 크기 정보
        public static int    CONTETNS_THUMBNAIL_WIDTH  { get { return 335; } }
        public static int    CONTETNS_THUMBNAIL_HEIGHT { get { return 218; } }
        public static int    LAYOUT_THUMBNAIL_WIDTH    { get { return 416; } }
        public static int    LAYOUT_THUMBNAIL_HEIGHT   { get { return 243; } }
        #endregion

        #region 비밀번호 암호화키
        public static string key = "UMACITDIDCMS2026";   // 암호화 키
        #endregion

        #region 특수기호 체크
        //public static bool speSymbol(string str)
        //{
        //    ResultModel resultModel = new ResultModel();
        //    str = string.IsNullOrEmpty(str) ? "" : str;

        //    // 2019-05-14 특수기호 입력 체크
        //    string chkStr = @"[~!@\#$%^&*\()\=+|\\/?""<>']";
        //    System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(chkStr);

        //    if (rex.IsMatch(str))
        //    {
        //        return true;
        //    }

        //    return false;
        //}
        #endregion

        #region XSS (script 공격) 방어를 위한 특수문자 및 script 문구 치환

        /// <summary>
        /// CodeGroupModels list XSS 체크
        /// </summary>
        /// <param name="codeGroupModel"></param>
        public static void XSSCheck_CodeGroupModel(List<CodeGroupModel> codeGroupModels)
        {
            if (codeGroupModels == null) return;

            foreach (CodeGroupModel codeGroupModel in codeGroupModels)
            {
                codeGroupModel.CODE_GROUP_NM = getXssString(codeGroupModel.CODE_GROUP_NM);
                codeGroupModel.CODE_GROUP_DESC = getXssString(codeGroupModel.CODE_GROUP_DESC);
                codeGroupModel.CODE_GROUP_ATTR1 = getXssString(codeGroupModel.CODE_GROUP_ATTR1);
                codeGroupModel.CODE_GROUP_ATTR2 = getXssString(codeGroupModel.CODE_GROUP_ATTR2);
                codeGroupModel.CODE_GROUP_ATTR3 = getXssString(codeGroupModel.CODE_GROUP_ATTR3);
            }
        }

        /// <summary>
        /// CodeModel list XSS 체크
        /// </summary>
        /// <param name="codeModels"></param>
        public static void XSSCheck_CodeModel(List<CodeModel> codeModels)
        {
            if (codeModels == null) return;

            foreach (CodeModel codeModel in codeModels)
            {
                codeModel.CODE_NAME = getXssString(codeModel.CODE_NAME);
                codeModel.CODE_DESC = getXssString(codeModel.CODE_DESC);
                codeModel.CODE_ATTR1 = getXssString(codeModel.CODE_ATTR1);
                codeModel.CODE_ATTR2 = getXssString(codeModel.CODE_ATTR2);
                codeModel.CODE_ATTR3 = getXssString(codeModel.CODE_ATTR3);
            }
        }

        /// <summary>
        /// DisplayModel XSS 체크
        /// </summary>
        /// <param name="displayModel"></param>
        public static void XSSCheck_DisplayModel(DisplayModel displayModel)
        {
            if (displayModel == null) return;

            displayModel.DISPLAY_NM = getXssString(displayModel.DISPLAY_NM);
            displayModel.DISPLAY_DESC = getXssString(displayModel.DISPLAY_DESC);
            displayModel.DISPLAY_OS = getXssString(displayModel.DISPLAY_OS);
            displayModel.DISPLAY_IP = getXssString(displayModel.DISPLAY_IP);
            displayModel.DISPLAY_MAC = getXssString(displayModel.DISPLAY_MAC);
            displayModel.DISPLAY_OS_LIST_SELECTED = getXssString(displayModel.DISPLAY_OS_LIST_SELECTED);
            displayModel.USE_YN_SELECTED = getXssString(displayModel.USE_YN_SELECTED);
            displayModel.DISPLAY_SELECTED = getXssString(displayModel.DISPLAY_SELECTED);
        }

        /// <summary>
        /// DisplayModel list XSS 체크
        /// </summary>
        /// <param name="displayModels"></param>
        public static void XSSCheck_DisplayModel(List<DisplayModel> displayModels)
        {
            if (displayModels == null) return;

            foreach (DisplayModel displayModel in displayModels)
            {
                displayModel.DISPLAY_NM = getXssString(displayModel.DISPLAY_NM);
                displayModel.DISPLAY_DESC = getXssString(displayModel.DISPLAY_DESC);
                displayModel.DISPLAY_OS = getXssString(displayModel.DISPLAY_OS);
                displayModel.DISPLAY_IP = getXssString(displayModel.DISPLAY_IP);
                displayModel.DISPLAY_MAC = getXssString(displayModel.DISPLAY_MAC);
                displayModel.DISPLAY_OS_LIST_SELECTED = getXssString(displayModel.DISPLAY_OS_LIST_SELECTED);
                displayModel.USE_YN_SELECTED = getXssString(displayModel.USE_YN_SELECTED);
                displayModel.DISPLAY_SELECTED = getXssString(displayModel.DISPLAY_SELECTED);
            }
        }

        /// <summary>
        /// DisplayGroupModel XSS 체크
        /// </summary>
        /// <param name="displayGroupModel"></param>
        public static void XSSCheck_DisplayGroupModel(DisplayGroupModel displayGroupModel)
        {
            if (displayGroupModel == null) return;

            displayGroupModel.DISPLAY_GROUP_NM = getXssString(displayGroupModel.DISPLAY_GROUP_NM);
            displayGroupModel.DISPLAY_GROUP_DESC = getXssString(displayGroupModel.DISPLAY_GROUP_DESC);
        }

        /// <summary>
        /// DisplayGroupModel list XSS 체크
        /// </summary>
        /// <param name="displayGroupModels"></param>
        public static void XSSCheck_DisplayGroupModel(List<DisplayGroupModel> displayGroupModels)
        {
            if (displayGroupModels == null) return;

            foreach (DisplayGroupModel displayGroupModel in displayGroupModels)
            {
                displayGroupModel.DISPLAY_GROUP_NM = getXssString(displayGroupModel.DISPLAY_GROUP_NM);
                displayGroupModel.DISPLAY_GROUP_DESC = getXssString(displayGroupModel.DISPLAY_GROUP_DESC);
            }
        }

        /// <summary>
        /// LayoutModel XSS 체크
        /// </summary>
        /// <param name="layoutModel"></param>
        public static void XSSCheck_LayoutModel(LayoutModel layoutModel)
        {
            if (layoutModel == null) return;

            layoutModel.LAYOUT_NM = getXssString(layoutModel.LAYOUT_NM);
            layoutModel.LAYOUT_DESC = getXssString(layoutModel.LAYOUT_DESC);
            layoutModel.FILE_NM = getXssString(layoutModel.FILE_NM);
            layoutModel.LAYOUT_H_BY_V = getXssString(layoutModel.LAYOUT_H_BY_V);
            layoutModel.THUMBNAIL_NM = getXssString(layoutModel.THUMBNAIL_NM);
        }

        /// <summary>
        /// LayoutDetailModel list XSS 체크
        /// </summary>
        /// <param name="layoutDetailModels"></param>
        public static void XSSCheck_LayoutDetailModel(List<LayoutDetailModel> layoutDetailModels)
        {
            if (layoutDetailModels == null) return;

            foreach (LayoutDetailModel layoutDetailModel in layoutDetailModels)
            {
                layoutDetailModel.LAYOUT_SEQ = getXssString(layoutDetailModel.LAYOUT_SEQ);
                layoutDetailModel.CONTENT_TYPE_NM = getXssString(layoutDetailModel.CONTENT_TYPE_NM);
            }
        }

        /// <summary>
        /// LayoutImageModel list XSS 체크
        /// </summary>
        /// <param name="layoutDetailModels"></param>
        public static void XSSCheck_LayoutImageModel(List<LayoutImageModel> layoutImageModels)
        {
            if (layoutImageModels == null) return;

            foreach (LayoutImageModel layoutImageModel in layoutImageModels)
            {
                layoutImageModel.FILE_NM = getXssString(layoutImageModel.FILE_NM);
            }
        }

        /// <summary>
        /// ContentsModel XSS 체크
        /// </summary>
        /// <param name="contentsModel"></param>
        public static void XSSCheck_ContentsModel(ContentsModel contentsModel)
        {
            if (contentsModel == null) return;

            contentsModel.CONTENT_NM = getXssString(contentsModel.CONTENT_NM);
            contentsModel.FILE_PATH = getXssString(contentsModel.FILE_PATH);
            contentsModel.FILE_NM = getXssString(contentsModel.FILE_NM);
            contentsModel.VIDEO_DURATION = getXssString(contentsModel.VIDEO_DURATION);
            contentsModel.FILE_URL = getXssString(contentsModel.FILE_URL);
            contentsModel.ORGIN_FILE_URL = getXssString(contentsModel.ORGIN_FILE_URL);
            contentsModel.THUMBNAIL_PATH = getXssString(contentsModel.THUMBNAIL_PATH);
            contentsModel.THUMBNAIL_NM = getXssString(contentsModel.THUMBNAIL_NM);
            contentsModel.THUMBNAIL_URL = getXssString(contentsModel.THUMBNAIL_URL);
            contentsModel.TEMPLATE_ORG_URL = getXssString(contentsModel.TEMPLATE_ORG_URL);
            //TEXT_HTML 정보는 Contents 등록 시 Text로 입력된 정보임 (사용하지 않는 것으로 보임)
            contentsModel.TEXT_HTML = getXssString(contentsModel.TEXT_HTML);
        }

        /// <summary>
        /// ScheduleModel XSS 체크
        /// </summary>
        /// <param name="scheduleModel"></param>
        public static void XSSCheck_ScheduleModel(ScheduleModel scheduleModel)
        {
            if (scheduleModel == null) return;

            scheduleModel.SCHEDULE_NM = getXssString(scheduleModel.SCHEDULE_NM);
            scheduleModel.SCHEDULE_DESC = getXssString(scheduleModel.SCHEDULE_DESC);
            scheduleModel.PLAY_TIME = getXssString(scheduleModel.PLAY_TIME);
            scheduleModel.ROLLING_YN = getXssString(scheduleModel.ROLLING_YN);
            scheduleModel.URL = getXssString(scheduleModel.URL);
        }

        /// <summary>
        /// ScheduleTemplateMapModel list XSS 체크
        /// </summary>
        /// <param name="scheduleTemplateMapModels"></param>
        public static void XSSCheck_ScheduleTemplateMapModel(List<ScheduleTemplateMapModel> scheduleTemplateMapModels)
        {
            if (scheduleTemplateMapModels == null) return;

            foreach (ScheduleTemplateMapModel scheduleTemplateMapModel in scheduleTemplateMapModels)
            {
                scheduleTemplateMapModel.TEMPLATE_URL = getXssString(scheduleTemplateMapModel.TEMPLATE_URL);
            }
        }

        /// <summary>
        /// TemplateModel XSS 체크
        /// </summary>
        /// <param name="templateModel"></param>
        public static void XSSCheck_TemplateModel(TemplateModel templateModel)
        {
            if (templateModel == null) return;

            templateModel.TEMPLATE_NM = getXssString(templateModel.TEMPLATE_NM);
            templateModel.TEMPLATE_DESC = getXssString(templateModel.TEMPLATE_DESC);
            templateModel.FILE_PATH = getXssString(templateModel.FILE_PATH);
            templateModel.FILE_NM = getXssString(templateModel.FILE_NM);
            templateModel.TEMPLATE_URL = getXssString(templateModel.TEMPLATE_URL);
            templateModel.THUMBNAIL_NM = getXssString(templateModel.THUMBNAIL_NM);
            //ManageTemplate.cshtml에 있는 templateArea 정보라서 처리하면 오류가 발생할 수 있음
            //templateModel.TEMPLATE_HTML = getXssString(templateModel.TEMPLATE_HTML);
            templateModel.LAYOUT_FILE_NM = getXssString(templateModel.LAYOUT_FILE_NM);
        }

        /// <summary>
        /// TemplateMapModel list XSS 체크
        /// </summary>
        /// <param name="templateMapModels"></param>
        public static void XSSCheck_TemplateMapModel(List<TemplateMapModel> templateMapModels)
        {
            if (templateMapModels == null) return;

            foreach (TemplateMapModel templateMapModel in templateMapModels)
            {
                templateMapModel.RESTAURANT_CODE = getXssString(templateMapModel.RESTAURANT_CODE);
                templateMapModel.LAYOUT_SEQ = getXssString(templateMapModel.LAYOUT_SEQ);
                templateMapModel.CONNECTION_NM = getXssString(templateMapModel.CONNECTION_NM);
                templateMapModel.CONTENT_FILE_PATH = getXssString(templateMapModel.CONTENT_FILE_PATH);
                templateMapModel.CONTENT_FILE_NM = getXssString(templateMapModel.CONTENT_FILE_NM);
                templateMapModel.FILE_NM = getXssString(templateMapModel.FILE_NM);
            }
        }

        /// <summary>
        /// UserModel XSS 체크
        /// </summary>
        /// <param name="userModel"></param>
        public static void XSSCheck_UserModel(UserModel userModel)
        {
            if (userModel == null) return;

            userModel.USER_ID = getXssString(userModel.USER_ID);
            userModel.RESTAURANT_CODE = getXssString(userModel.RESTAURANT_CODE);
            userModel.RESTAURANT_NM = getXssString(userModel.RESTAURANT_NM);
            userModel.USER_NM = getXssString(userModel.USER_NM);
            userModel.GROUP_CD = getXssString(userModel.GROUP_CD);
            userModel.USER_PW = getXssString(userModel.USER_PW);
            userModel.USE_YN = getXssString(userModel.USE_YN);
        }

        /// <summary>
        /// XSS 공격에 대응하기 위해 특정 문구에 대해서 replace하는 기능
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string getXssString(string str)
        {
            str = string.IsNullOrEmpty(str) ? "" : str;

            str = str.Replace("<script", "");
            //str = str.Replace("onload", "");
            //str = str.Replace("expression", "");
            //str = str.Replace("onmouseover", "");
            //str = str.Replace("onmouseout", "");
            //str = str.Replace("onclick", "");
            str = str.Replace("<iframe", "");
            str = str.Replace("<object", "");
            str = str.Replace("<embed", "");
            str = str.Replace("document.cookie", "");
            str = str.Replace("<", "");
            str = str.Replace(">", "");
            str = str.Replace("\"", "");
            str = str.Replace("'", "");

            return str;
        }
        #endregion
    }
}
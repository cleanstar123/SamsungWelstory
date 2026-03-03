using System.Collections.Generic;
using System.Data;

using CMS.API.Models;
using CMS.API.App_Code;
using Oracle.ManagedDataAccess.Client;

namespace CMS.API.Biz
{
    public class TemplateBiz
    {
        /// <summary>
        /// 템플릿 조회
        /// </summary>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        public static TemplateModel getTemplateList(TemplateModel templateModel)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_RESTAURANT_CODE", templateModel.RESTAURANT_CODE),
                                          new OracleParameter("P_TEMPLATE_ID",     templateModel.TEMPLATE_ID),
                                          new OracleParameter("P_TEMPLATE_NM",     templateModel.TEMPLATE_NM),
                                          new OracleParameter("CUR",               OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<TemplateModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_TEMPLATE.PR_TEMPLATE_LIST", param).Tables[0])[0];
        }

        /// <summary>
        /// 템플릿 페이징 조회
        /// </summary>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        public static DataSet getTemplatePageList(TemplateModel templateModel)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_RESTAURANT_CODE", templateModel.RESTAURANT_CODE),
                                          new OracleParameter("P_TEMPLATE_ID",     templateModel.TEMPLATE_ID),
                                          new OracleParameter("P_TEMPLATE_NM",     templateModel.TEMPLATE_NM),
                                          new OracleParameter("P_PAGE_CNT",        templateModel.PAGE_CNT),
                                          new OracleParameter("P_PAGE_NO",         templateModel.PAGE_NO),
                                          new OracleParameter("CUR",               OracleDbType.RefCursor),
                                          new OracleParameter("CUR_COUNT",         OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;
            param[param.Length - 2].Direction = ParameterDirection.Output;

            return OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_TEMPLATE.PR_TEMPLATE_LIST_PAGE", param);
        }

        /// <summary>
        /// 템플릿과 연결된 컨텐츠 정보 조회
        /// </summary>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        public static List<TemplateMapModel> getTemplateMapList(TemplateModel templateModel)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_RESTAURANT_CODE", templateModel.RESTAURANT_CODE),
                                          new OracleParameter("P_TEMPLATE_ID",     templateModel.TEMPLATE_ID),
                                          new OracleParameter("P_TEMPLATE_NM",     templateModel.TEMPLATE_NM),
                                          new OracleParameter("CUR",               OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<TemplateMapModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_TEMPLATE.PR_TEMPLATE_MAP_LIST", param).Tables[0]);
        }

        /// <summary>
        /// 템플릿 저장 (템플릿 기본정보 및 템플릿과 연결된 컨텐츠 정보)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userId"></param>
        /// <param name="templateModel"></param>
        /// <param name="templateMapModels"></param>
        /// <returns></returns>
        public static ResultModel ManageTemplateAll(string type, string userId, TemplateModel templateModel, List<TemplateMapModel> templateMapModels)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_TYPE",            type),
                                          new OracleParameter("P_RESTAURANT_CODE", templateModel.RESTAURANT_CODE),
                                          new OracleParameter("P_TEMPLATE_ID",     templateModel.TEMPLATE_ID),
                                          new OracleParameter("P_TEMPLATE_NM",     templateModel.TEMPLATE_NM),
                                          new OracleParameter("P_FILE_PATH",       templateModel.FILE_PATH),
                                          new OracleParameter("P_FILE_NM",         templateModel.FILE_NM),
                                          new OracleParameter("P_FILE_EXT",        templateModel.FILE_EXT),
                                          new OracleParameter("P_FILE_SIZE",       templateModel.FILE_SIZE),
                                          new OracleParameter("P_TEMPLATE_URL",    templateModel.TEMPLATE_URL),
                                          new OracleParameter("P_TEMPLATE_DESC",   templateModel.TEMPLATE_DESC),
                                          new OracleParameter("P_LAYOUT_ID",       templateModel.LAYOUT_ID),
                                          new OracleParameter("P_THUMBNAIL_NM",    templateModel.THUMBNAIL_NM),
                                          new OracleParameter("P_REG_ID",          userId),
                                          new OracleParameter("P_XML_REQ",         JsonHelper.GetJsonToXmlString<TemplateMapModel>(templateMapModels)),
                                          new OracleParameter("CUR",               OracleDbType.RefCursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_TEMPLATE.PR_TEMPLATE_MANAGE_ALL", param).Tables[0])[0];
        }

        /// <summary>
        /// 템플릿 수정/삭제
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userId"></param>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        public static ResultModel ManageTemplate(string type, string userId, TemplateModel templateModel)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_TYPE",            type),
                                          new OracleParameter("P_RESTAURANT_CODE", templateModel.RESTAURANT_CODE),
                                          new OracleParameter("P_TEMPLATE_ID",     templateModel.TEMPLATE_ID),
                                          new OracleParameter("P_TEMPLATE_NM",     templateModel.TEMPLATE_NM),
                                          new OracleParameter("P_FILE_PATH",       templateModel.FILE_PATH),
                                          new OracleParameter("P_FILE_NM",         templateModel.FILE_NM),
                                          new OracleParameter("P_FILE_EXT",        templateModel.FILE_EXT),
                                          new OracleParameter("P_FILE_SIZE",       templateModel.FILE_SIZE),
                                          new OracleParameter("P_TEMPLATE_URL",    templateModel.TEMPLATE_URL),
                                          new OracleParameter("P_TEMPLATE_DESC",   templateModel.TEMPLATE_DESC),
                                          new OracleParameter("P_LAYOUT_ID",       templateModel.LAYOUT_ID),
                                          new OracleParameter("P_THUMBNAIL_NM",    templateModel.THUMBNAIL_NM),
                                          new OracleParameter("P_REG_ID",          userId),
                                          new OracleParameter("P_XML_REQ",         null),
                                          new OracleParameter("CUR",               OracleDbType.RefCursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_TEMPLATE.PR_TEMPLATE_MANAGE", param).Tables[0])[0];
        }

        /// <summary>
        /// 템플릿 연결 정보 수정
        /// </summary>
        /// <param name="type"></param>
        /// <param name="restaurantCode"></param>
        /// <param name="templateMapModels"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ResultModel ManageTemplateMap(string type, string restaurantCode, List<TemplateMapModel> templateMapModels, string userId)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_TYPE",            type),
                                          new OracleParameter("P_RESTAURANT_CODE", restaurantCode),
                                          new OracleParameter("P_REG_ID",          userId),
                                          new OracleParameter("P_XML_REQ",         JsonHelper.GetJsonToXmlString<TemplateMapModel>(templateMapModels)),
                                          new OracleParameter("CUR",               OracleDbType.RefCursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_TEMPLATE.PR_TEMPLATE_MAP_MANAGE", param).Tables[0])[0];
        }
    }
}
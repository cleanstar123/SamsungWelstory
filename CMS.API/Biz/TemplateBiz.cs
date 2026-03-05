using System;
using System.Collections.Generic;
using System.Data;

using CMS.API.Models;
using CMS.API.App_Code;
using Npgsql;
using NpgsqlTypes;

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
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_RESTAURANT_CODE", templateModel.RESTAURANT_CODE),
                                          new NpgsqlParameter("P_TEMPLATE_ID",     templateModel.TEMPLATE_ID),
                                          new NpgsqlParameter("P_TEMPLATE_NM",     templateModel.TEMPLATE_NM),
                                          new NpgsqlParameter("CUR",               NpgsqlDbType.Refcursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<TemplateModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_TEMPLATE.PR_TEMPLATE_LIST", param).Tables[0])[0];
        }

        /// <summary>
        /// 템플릿 페이징 조회
        /// </summary>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        public static DataSet getTemplatePageList(TemplateModel templateModel)
        {
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_RESTAURANT_CODE", templateModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_TEMPLATE_ID",     templateModel.TEMPLATE_ID ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_TEMPLATE_NM",     templateModel.TEMPLATE_NM ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_PAGE_CNT",        templateModel.PAGE_CNT ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_PAGE_NO",         templateModel.PAGE_NO ??(object) DBNull.Value)
                                      };

            // 템플릿 목록 조회
            string sqlList = "SELECT * FROM publicdata.pr_template_list_page(@P_RESTAURANT_CODE, @P_TEMPLATE_ID, @P_TEMPLATE_NM, @P_PAGE_CNT, @P_PAGE_NO)";
            DataSet ds = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sqlList, param);

            // 페이지 카운트 조회
            NpgsqlParameter[] paramCount =  {
                                                new NpgsqlParameter("P_RESTAURANT_CODE", templateModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                                                new NpgsqlParameter("P_TEMPLATE_ID",     templateModel.TEMPLATE_ID ?? (object)DBNull.Value),
                                                new NpgsqlParameter("P_TEMPLATE_NM",     templateModel.TEMPLATE_NM ?? (object)DBNull.Value),
                                                new NpgsqlParameter("P_PAGE_CNT",        templateModel.PAGE_CNT ?? (object) DBNull.Value)
                                            };

            string sqlCount = "SELECT * FROM publicdata.pr_template_list_page_count(@P_RESTAURANT_CODE, @P_TEMPLATE_ID, @P_TEMPLATE_NM, @P_PAGE_CNT)";
            DataSet dsCount = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sqlCount, paramCount);

            // 두 결과를 하나의 DataSet으로 합침 (Table = 목록, Table1 = 카운트)
            DataTable countTable = dsCount.Tables[0].Copy();
            countTable.TableName = "Table1";
            ds.Tables.Add(countTable);

            return ds;
        }

        /// <summary>
        /// 템플릿과 연결된 컨텐츠 정보 조회
        /// </summary>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        public static List<TemplateMapModel> getTemplateMapList(TemplateModel templateModel)
        {
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_RESTAURANT_CODE", templateModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_TEMPLATE_ID",     templateModel.TEMPLATE_ID ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_TEMPLATE_NM",     templateModel.TEMPLATE_NM ?? (object)DBNull.Value)
                                      };

            return Util.ConvertDataTable<TemplateMapModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "publicdata.pr_template_map_list", param).Tables[0]);
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
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_TYPE",            type ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_RESTAURANT_CODE", templateModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_TEMPLATE_ID",     templateModel.TEMPLATE_ID ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_TEMPLATE_NM",     templateModel.TEMPLATE_NM ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_FILE_PATH",       templateModel.FILE_PATH ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_FILE_NM",         templateModel.FILE_NM ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_FILE_EXT",        templateModel.FILE_EXT ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_FILE_SIZE",       templateModel.FILE_SIZE ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_TEMPLATE_URL",    templateModel.TEMPLATE_URL ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_TEMPLATE_DESC",   templateModel.TEMPLATE_DESC ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_LAYOUT_ID",       templateModel.LAYOUT_ID ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_THUMBNAIL_NM",    templateModel.THUMBNAIL_NM ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_REG_ID",          userId ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_XML_REQ",         JsonHelper.GetJsonToXmlString<TemplateMapModel>(templateMapModels) ?? (object)DBNull.Value)
                                      };

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "publicdata.pr_template_manage_all", param).Tables[0])[0];
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
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_TYPE",            type ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_RESTAURANT_CODE", templateModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_TEMPLATE_ID",     templateModel.TEMPLATE_ID ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_TEMPLATE_NM",     templateModel.TEMPLATE_NM ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_FILE_PATH",       templateModel.FILE_PATH ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_FILE_NM",         templateModel.FILE_NM ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_FILE_EXT",        templateModel.FILE_EXT ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_FILE_SIZE",       templateModel.FILE_SIZE ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_TEMPLATE_URL",    templateModel.TEMPLATE_URL ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_TEMPLATE_DESC",   templateModel.TEMPLATE_DESC ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_LAYOUT_ID",       templateModel.LAYOUT_ID ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_THUMBNAIL_NM",    templateModel.THUMBNAIL_NM ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_REG_ID",          userId ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_XML_REQ",         (object)DBNull.Value)
                                      };

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "publicdata.pr_template_manage", param).Tables[0])[0];
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
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_TYPE",            type ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_RESTAURANT_CODE", restaurantCode ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_REG_ID",          userId ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_XML_REQ",         JsonHelper.GetJsonToXmlString<TemplateMapModel>(templateMapModels) ?? (object)DBNull.Value)
                                      };

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "publicdata.pr_template_map_manage", param).Tables[0])[0];
        }
    }
}
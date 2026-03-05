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
            // PostgreSQL 함수가 TEXT 타입을 받으므로 string 그대로 전달
            NpgsqlParameter[] param = {
                new NpgsqlParameter("P_RESTAURANT_CODE", NpgsqlDbType.Text) { Value = templateModel.RESTAURANT_CODE ?? (object)DBNull.Value },
                new NpgsqlParameter("P_TEMPLATE_ID", NpgsqlDbType.Text) { Value = templateModel.TEMPLATE_ID ?? (object)DBNull.Value },
                new NpgsqlParameter("P_TEMPLATE_NM", NpgsqlDbType.Text) { Value = templateModel.TEMPLATE_NM ?? (object)DBNull.Value }
            };

            var result = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "publicdata.pr_template_list", param);

            if (result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
            {
                return templateModel;
            }

            return Util.ConvertDataTable<TemplateModel>(result.Tables[0])[0];
        }

        /// <summary>
        /// 템플릿 페이징 조회
        /// </summary>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        public static DataSet getTemplatePageList(TemplateModel templateModel)
        {
            // PostgreSQL 함수가 TEXT 타입을 받으므로 string 그대로 전달
            NpgsqlParameter[] param = {
                new NpgsqlParameter("P_RESTAURANT_CODE", NpgsqlDbType.Text) { Value = templateModel.RESTAURANT_CODE ?? (object)DBNull.Value },
                new NpgsqlParameter("P_TEMPLATE_ID", NpgsqlDbType.Text) { Value = templateModel.TEMPLATE_ID ?? (object)DBNull.Value },
                new NpgsqlParameter("P_TEMPLATE_NM", NpgsqlDbType.Text) { Value = templateModel.TEMPLATE_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("P_PAGE_CNT", NpgsqlDbType.Text) { Value = templateModel.PAGE_CNT ?? (object)DBNull.Value },
                new NpgsqlParameter("P_PAGE_NO", NpgsqlDbType.Text) { Value = templateModel.PAGE_NO ?? (object)DBNull.Value }
            };

            // 템플릿 목록 조회
            string sqlList = "SELECT * FROM publicdata.pr_template_list_page(@P_RESTAURANT_CODE, @P_TEMPLATE_ID, @P_TEMPLATE_NM, @P_PAGE_CNT, @P_PAGE_NO)";
            DataSet result = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sqlList, param);

            // JavaScript가 Table과 Table1을 기대하므로 분리
            if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
            {
                DataSet ds = new DataSet();
                
                // Table: 템플릿 목록
                DataTable dtList = result.Tables[0].Copy();
                dtList.TableName = "Table";
                ds.Tables.Add(dtList);
                
                // Table1: 카운트 정보 (첫 번째 행의 카운트 정보만)
                DataTable dtCount = new DataTable("Table1");
                dtCount.Columns.Add("TOT_ROW_COUNT", typeof(int));
                dtCount.Columns.Add("TOT_PAGE_COUNT", typeof(int));
                DataRow countRow = dtCount.NewRow();
                countRow["TOT_ROW_COUNT"] = result.Tables[0].Rows[0]["TOT_ROW_COUNT"];
                countRow["TOT_PAGE_COUNT"] = result.Tables[0].Rows[0]["TOT_PAGE_COUNT"];
                dtCount.Rows.Add(countRow);
                ds.Tables.Add(dtCount);
                
                return ds;
            }
            
            return result;
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
            // TEMPLATE_ID, LAYOUT_ID, FILE_SIZE를 integer로 변환
            int? templateId = null;
            if (!string.IsNullOrEmpty(templateModel.TEMPLATE_ID) && int.TryParse(templateModel.TEMPLATE_ID, out int parsedTemplateId))
            {
                templateId = parsedTemplateId;
            }

            int? layoutId = null;
            if (!string.IsNullOrEmpty(templateModel.LAYOUT_ID) && int.TryParse(templateModel.LAYOUT_ID, out int parsedLayoutId))
            {
                layoutId = parsedLayoutId;
            }

            int? fileSize = null;
            if (!string.IsNullOrEmpty(templateModel.FILE_SIZE) && int.TryParse(templateModel.FILE_SIZE, out int parsedFileSize))
            {
                fileSize = parsedFileSize;
            }

            NpgsqlParameter[] param = {
                new NpgsqlParameter("P_TYPE", NpgsqlDbType.Varchar) { Value = type ?? (object)DBNull.Value },
                new NpgsqlParameter("P_RESTAURANT_CODE", NpgsqlDbType.Varchar) { Value = templateModel.RESTAURANT_CODE ?? (object)DBNull.Value },
                new NpgsqlParameter("P_TEMPLATE_ID", NpgsqlDbType.Integer) { Value = templateId ?? (object)DBNull.Value },
                new NpgsqlParameter("P_TEMPLATE_NM", NpgsqlDbType.Varchar) { Value = templateModel.TEMPLATE_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("P_FILE_PATH", NpgsqlDbType.Varchar) { Value = templateModel.FILE_PATH ?? (object)DBNull.Value },
                new NpgsqlParameter("P_FILE_NM", NpgsqlDbType.Varchar) { Value = templateModel.FILE_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("P_FILE_EXT", NpgsqlDbType.Varchar) { Value = templateModel.FILE_EXT ?? (object)DBNull.Value },
                new NpgsqlParameter("P_FILE_SIZE", NpgsqlDbType.Integer) { Value = fileSize ?? (object)DBNull.Value },
                new NpgsqlParameter("P_TEMPLATE_URL", NpgsqlDbType.Varchar) { Value = templateModel.TEMPLATE_URL ?? (object)DBNull.Value },
                new NpgsqlParameter("P_TEMPLATE_DESC", NpgsqlDbType.Varchar) { Value = templateModel.TEMPLATE_DESC ?? (object)DBNull.Value },
                new NpgsqlParameter("P_LAYOUT_ID", NpgsqlDbType.Integer) { Value = layoutId ?? (object)DBNull.Value },
                new NpgsqlParameter("P_THUMBNAIL_NM", NpgsqlDbType.Varchar) { Value = templateModel.THUMBNAIL_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("P_REG_ID", NpgsqlDbType.Varchar) { Value = userId ?? (object)DBNull.Value },
                new NpgsqlParameter("P_XML_REQ", NpgsqlDbType.Text) { Value = JsonHelper.GetJsonToXmlString<TemplateMapModel>(templateMapModels) ?? (object)DBNull.Value }
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
            // TEMPLATE_ID, LAYOUT_ID, FILE_SIZE를 integer로 변환
            int? templateId = null;
            if (!string.IsNullOrEmpty(templateModel.TEMPLATE_ID) && int.TryParse(templateModel.TEMPLATE_ID, out int parsedTemplateId))
            {
                templateId = parsedTemplateId;
            }

            int? layoutId = null;
            if (!string.IsNullOrEmpty(templateModel.LAYOUT_ID) && int.TryParse(templateModel.LAYOUT_ID, out int parsedLayoutId))
            {
                layoutId = parsedLayoutId;
            }

            int? fileSize = null;
            if (!string.IsNullOrEmpty(templateModel.FILE_SIZE) && int.TryParse(templateModel.FILE_SIZE, out int parsedFileSize))
            {
                fileSize = parsedFileSize;
            }

            NpgsqlParameter[] param = {
                new NpgsqlParameter("P_TYPE", NpgsqlDbType.Varchar) { Value = type ?? (object)DBNull.Value },
                new NpgsqlParameter("P_RESTAURANT_CODE", NpgsqlDbType.Varchar) { Value = templateModel.RESTAURANT_CODE ?? (object)DBNull.Value },
                new NpgsqlParameter("P_TEMPLATE_ID", NpgsqlDbType.Integer) { Value = templateId ?? (object)DBNull.Value },
                new NpgsqlParameter("P_TEMPLATE_NM", NpgsqlDbType.Varchar) { Value = templateModel.TEMPLATE_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("P_FILE_PATH", NpgsqlDbType.Varchar) { Value = templateModel.FILE_PATH ?? (object)DBNull.Value },
                new NpgsqlParameter("P_FILE_NM", NpgsqlDbType.Varchar) { Value = templateModel.FILE_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("P_FILE_EXT", NpgsqlDbType.Varchar) { Value = templateModel.FILE_EXT ?? (object)DBNull.Value },
                new NpgsqlParameter("P_FILE_SIZE", NpgsqlDbType.Integer) { Value = fileSize ?? (object)DBNull.Value },
                new NpgsqlParameter("P_TEMPLATE_URL", NpgsqlDbType.Varchar) { Value = templateModel.TEMPLATE_URL ?? (object)DBNull.Value },
                new NpgsqlParameter("P_TEMPLATE_DESC", NpgsqlDbType.Varchar) { Value = templateModel.TEMPLATE_DESC ?? (object)DBNull.Value },
                new NpgsqlParameter("P_LAYOUT_ID", NpgsqlDbType.Integer) { Value = layoutId ?? (object)DBNull.Value },
                new NpgsqlParameter("P_THUMBNAIL_NM", NpgsqlDbType.Varchar) { Value = templateModel.THUMBNAIL_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("P_REG_ID", NpgsqlDbType.Varchar) { Value = userId ?? (object)DBNull.Value },
                new NpgsqlParameter("P_XML_REQ", NpgsqlDbType.Text) { Value = (object)DBNull.Value }
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
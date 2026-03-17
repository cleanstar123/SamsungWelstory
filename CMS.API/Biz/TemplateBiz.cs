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
        public static TemplateModel getTemplateList(TemplateModel templateModel)
        {
            NpgsqlParameter[] param = {
                new NpgsqlParameter("p_restaurant_code", templateModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                new NpgsqlParameter("p_template_id",     templateModel.TEMPLATE_ID ?? (object)DBNull.Value),
                new NpgsqlParameter("p_template_nm",     templateModel.TEMPLATE_NM ?? (object)DBNull.Value)
            };

            string sql = "SELECT * FROM did.pr_template_list(@p_restaurant_code, @p_template_id, @p_template_nm)";
            var result = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);

            if (result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                return templateModel;

            return Util.ConvertDataTable<TemplateModel>(result.Tables[0])[0];
        }

        /// <summary>
        /// 템플릿 페이징 조회
        /// </summary>
        public static DataSet getTemplatePageList(TemplateModel templateModel)
        {
            NpgsqlParameter[] param = {
                new NpgsqlParameter("p_restaurant_code", templateModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                new NpgsqlParameter("p_template_id",     templateModel.TEMPLATE_ID ?? (object)DBNull.Value),
                new NpgsqlParameter("p_template_nm",     templateModel.TEMPLATE_NM ?? (object)DBNull.Value),
                new NpgsqlParameter("p_page_cnt",        templateModel.PAGE_CNT ?? (object)DBNull.Value),
                new NpgsqlParameter("p_page_no",         templateModel.PAGE_NO ?? (object)DBNull.Value)
            };

            string sqlList = "SELECT * FROM did.pr_template_list_page(@p_restaurant_code, @p_template_id, @p_template_nm, @p_page_cnt, @p_page_no)";
            DataSet ds = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sqlList, param);

            NpgsqlParameter[] paramCount = {
                new NpgsqlParameter("p_restaurant_code", templateModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                new NpgsqlParameter("p_template_id",     templateModel.TEMPLATE_ID ?? (object)DBNull.Value),
                new NpgsqlParameter("p_template_nm",     templateModel.TEMPLATE_NM ?? (object)DBNull.Value),
                new NpgsqlParameter("p_page_cnt",        templateModel.PAGE_CNT ?? (object)DBNull.Value)
            };

            string sqlCount = "SELECT * FROM did.pr_template_list_page_count(@p_restaurant_code, @p_template_id, @p_template_nm, @p_page_cnt)";
            DataSet dsCount = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sqlCount, paramCount);

            DataTable countTable = dsCount.Tables[0].Copy();
            countTable.TableName = "Table1";
            ds.Tables.Add(countTable);

            return ds;
        }

        /// <summary>
        /// 템플릿과 연결된 컨텐츠 정보 조회
        /// </summary>
        public static List<TemplateMapModel> getTemplateMapList(TemplateModel templateModel)
        {
            NpgsqlParameter[] param = {
                new NpgsqlParameter("P_RESTAURANT_CODE", templateModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                new NpgsqlParameter("P_TEMPLATE_ID",     templateModel.TEMPLATE_ID ?? (object)DBNull.Value),
                new NpgsqlParameter("P_TEMPLATE_NM",     templateModel.TEMPLATE_NM ?? (object)DBNull.Value)
            };

            return Util.ConvertDataTable<TemplateMapModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "did.pr_template_map_list", param).Tables[0]);
        }

        /// <summary>
        /// 템플릿 저장 (기본정보 및 연결된 컨텐츠 정보)
        /// </summary>
        public static ResultModel ManageTemplateAll(string type, string userId, TemplateModel templateModel, List<TemplateMapModel> templateMapModels)
        {
            System.Diagnostics.Debug.WriteLine($"[ManageTemplateAll] type={type}, templateMapModels count={templateMapModels?.Count ?? 0}");

            string xmlString = JsonHelper.GetJsonToXmlString<TemplateMapModel>(templateMapModels);

            int? templateId = null;
            if (!string.IsNullOrEmpty(templateModel.TEMPLATE_ID) && int.TryParse(templateModel.TEMPLATE_ID, out int parsedTemplateId))
                templateId = parsedTemplateId;

            int? layoutId = null;
            if (!string.IsNullOrEmpty(templateModel.LAYOUT_ID) && int.TryParse(templateModel.LAYOUT_ID, out int parsedLayoutId))
                layoutId = parsedLayoutId;

            int? fileSize = null;
            if (!string.IsNullOrEmpty(templateModel.FILE_SIZE) && int.TryParse(templateModel.FILE_SIZE, out int parsedFileSize))
                fileSize = parsedFileSize;

            NpgsqlParameter[] param = {
                new NpgsqlParameter("p_type", NpgsqlDbType.Varchar) { Value = type ?? (object)DBNull.Value },
                new NpgsqlParameter("p_restaurant_code", NpgsqlDbType.Varchar) { Value = templateModel.RESTAURANT_CODE ?? (object)DBNull.Value },
                new NpgsqlParameter("p_template_id", NpgsqlDbType.Integer) { Value = templateId ?? (object)DBNull.Value },
                new NpgsqlParameter("p_template_nm", NpgsqlDbType.Varchar) { Value = templateModel.TEMPLATE_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("p_file_path", NpgsqlDbType.Varchar) { Value = templateModel.FILE_PATH ?? (object)DBNull.Value },
                new NpgsqlParameter("p_file_nm", NpgsqlDbType.Varchar) { Value = templateModel.FILE_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("p_file_ext", NpgsqlDbType.Varchar) { Value = templateModel.FILE_EXT ?? (object)DBNull.Value },
                new NpgsqlParameter("p_file_size", NpgsqlDbType.Integer) { Value = fileSize ?? (object)DBNull.Value },
                new NpgsqlParameter("p_template_url", NpgsqlDbType.Varchar) { Value = templateModel.TEMPLATE_URL ?? (object)DBNull.Value },
                new NpgsqlParameter("p_template_desc", NpgsqlDbType.Varchar) { Value = templateModel.TEMPLATE_DESC ?? (object)DBNull.Value },
                new NpgsqlParameter("p_layout_id", NpgsqlDbType.Integer) { Value = layoutId ?? (object)DBNull.Value },
                new NpgsqlParameter("p_thumbnail_nm", NpgsqlDbType.Varchar) { Value = templateModel.THUMBNAIL_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("p_reg_id", NpgsqlDbType.Varchar) { Value = userId ?? (object)DBNull.Value },
                new NpgsqlParameter("p_xml_req", NpgsqlDbType.Text) { Value = xmlString ?? (object)DBNull.Value }
            };

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "did.pr_template_manage_all", param).Tables[0])[0];
        }

        /// <summary>
        /// 템플릿 수정/삭제
        /// </summary>
        public static ResultModel ManageTemplate(string type, string userId, TemplateModel templateModel)
        {
            int? templateId = null;
            if (!string.IsNullOrEmpty(templateModel.TEMPLATE_ID) && int.TryParse(templateModel.TEMPLATE_ID, out int parsedTemplateId))
                templateId = parsedTemplateId;

            int? layoutId = null;
            if (!string.IsNullOrEmpty(templateModel.LAYOUT_ID) && int.TryParse(templateModel.LAYOUT_ID, out int parsedLayoutId))
                layoutId = parsedLayoutId;

            int? fileSize = null;
            if (!string.IsNullOrEmpty(templateModel.FILE_SIZE) && int.TryParse(templateModel.FILE_SIZE, out int parsedFileSize))
                fileSize = parsedFileSize;

            NpgsqlParameter[] param = {
                new NpgsqlParameter("p_type", NpgsqlDbType.Varchar) { Value = type ?? (object)DBNull.Value },
                new NpgsqlParameter("p_restaurant_code", NpgsqlDbType.Varchar) { Value = templateModel.RESTAURANT_CODE ?? (object)DBNull.Value },
                new NpgsqlParameter("p_template_id", NpgsqlDbType.Integer) { Value = templateId ?? (object)DBNull.Value },
                new NpgsqlParameter("p_template_nm", NpgsqlDbType.Varchar) { Value = templateModel.TEMPLATE_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("p_file_path", NpgsqlDbType.Varchar) { Value = templateModel.FILE_PATH ?? (object)DBNull.Value },
                new NpgsqlParameter("p_file_nm", NpgsqlDbType.Varchar) { Value = templateModel.FILE_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("p_file_ext", NpgsqlDbType.Varchar) { Value = templateModel.FILE_EXT ?? (object)DBNull.Value },
                new NpgsqlParameter("p_file_size", NpgsqlDbType.Integer) { Value = fileSize ?? (object)DBNull.Value },
                new NpgsqlParameter("p_template_url", NpgsqlDbType.Varchar) { Value = templateModel.TEMPLATE_URL ?? (object)DBNull.Value },
                new NpgsqlParameter("p_template_desc", NpgsqlDbType.Varchar) { Value = templateModel.TEMPLATE_DESC ?? (object)DBNull.Value },
                new NpgsqlParameter("p_layout_id", NpgsqlDbType.Integer) { Value = layoutId ?? (object)DBNull.Value },
                new NpgsqlParameter("p_thumbnail_nm", NpgsqlDbType.Varchar) { Value = templateModel.THUMBNAIL_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("p_reg_id", NpgsqlDbType.Varchar) { Value = userId ?? (object)DBNull.Value },
                new NpgsqlParameter("p_xml_req", NpgsqlDbType.Text) { Value = (object)DBNull.Value }
            };

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "did.pr_template_manage", param).Tables[0])[0];
        }

        /// <summary>
        /// 템플릿 연결 정보 수정
        /// </summary>
        public static ResultModel ManageTemplateMap(string type, string restaurantCode, List<TemplateMapModel> templateMapModels, string userId)
        {
            NpgsqlParameter[] param = {
                new NpgsqlParameter("p_type",            type ?? (object)DBNull.Value),
                new NpgsqlParameter("p_restaurant_code", restaurantCode ?? (object)DBNull.Value),
                new NpgsqlParameter("p_reg_id",          userId ?? (object)DBNull.Value),
                new NpgsqlParameter("p_xml_req",         JsonHelper.GetJsonToXmlString<TemplateMapModel>(templateMapModels) ?? (object)DBNull.Value)
            };

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "did.pr_template_map_manage", param).Tables[0])[0];
        }
    }
}

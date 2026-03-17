using System.Collections.Generic;
using System.Data;
using System;
using System.Linq;

using CMS.API.Models;
using CMS.API.App_Code;
using Npgsql;
using NpgsqlTypes;

namespace CMS.API.Biz
{
    public class CodeBiz
    {
        /// <summary>
        /// 그룹코드 조회
        /// </summary>
        public static DataSet getGroupCodes(string pCodeGroup, string pCodeGroupNm)
        {
            string sql = "SELECT * FROM did.pr_code_group_list(@p_code_group, @p_code_group_nm)";

            NpgsqlParameter[] param = {
                new NpgsqlParameter("@p_code_group", NpgsqlDbType.Varchar) { Value = pCodeGroup ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_code_group_nm", NpgsqlDbType.Varchar) { Value = pCodeGroupNm ?? (object)DBNull.Value }
            };

            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);
        }

        /// <summary>
        /// 코드 조회
        /// </summary>
        public static DataSet getCodes(string pCodeGroup)
        {
            string sql = "SELECT * FROM did.pr_code_list(@p_code_group)";

            NpgsqlParameter[] param = {
                new NpgsqlParameter("@p_code_group", pCodeGroup ?? "")
            };

            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);
        }

        /// <summary>
        /// 그룹코드 생성, 수정, 삭제
        /// </summary>
        public static ResultModel codeGroupManage(string type, string userId, List<CodeGroupModel> codeGroupModelList)
        {
            System.Diagnostics.Debug.WriteLine($"=== codeGroupManage 시작 ===");
            System.Diagnostics.Debug.WriteLine($"TYPE: {type}, userId: {userId}, Count: {codeGroupModelList?.Count ?? 0}");

            CodeGroupModel model = (type.ToUpper() != "D" && codeGroupModelList.Count > 0) ? codeGroupModelList[0] : null;

            string xmlReq = null;
            if (type.ToUpper() == "D" && codeGroupModelList != null && codeGroupModelList.Count > 0)
                xmlReq = JsonHelper.GetJsonToXmlString<CodeGroupModel>(codeGroupModelList);

            string sql = "SELECT * FROM did.pr_code_group_manage(@p_type, @p_code_group, @p_code_group_nm, @p_display_seq, @p_code_group_desc, @p_code_group_attr1, @p_code_group_attr2, @p_code_group_attr3, @p_reg_id, @p_xml_req)";

            NpgsqlParameter[] param = {
                new NpgsqlParameter("@p_type", type ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_code_group", model?.CODE_GROUP ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_code_group_nm", model?.CODE_GROUP_NM ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_display_seq", NpgsqlDbType.Integer) { Value = (model?.DISPLAY_SEQ ?? 0) == 0 ? (object)DBNull.Value : model.DISPLAY_SEQ },
                new NpgsqlParameter("@p_code_group_desc", model?.CODE_GROUP_DESC ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_code_group_attr1", model?.CODE_GROUP_ATTR1 ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_code_group_attr2", model?.CODE_GROUP_ATTR2 ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_code_group_attr3", model?.CODE_GROUP_ATTR3 ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_reg_id", userId ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_xml_req", NpgsqlDbType.Text) { Value = (object)xmlReq ?? DBNull.Value }
            };

            try
            {
                DataSet ds = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var result = Util.ConvertDataTable<ResultModel>(ds.Tables[0]);
                    if (result != null && result.Count > 0)
                        return result[0];
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"예외 발생: {ex.Message}");
                return new ResultModel { ERR_CODE = "9999", ERROR_MSG = ex.Message };
            }

            return new ResultModel { ERR_CODE = "9999", ERROR_MSG = "프로시저 실행 결과를 받지 못했습니다." };
        }

        /// <summary>
        /// 코드 생성, 수정, 삭제
        /// </summary>
        public static ResultModel codeManage(string type, string userId, List<CodeModel> codeModelList)
        {
            System.Diagnostics.Debug.WriteLine($"=== codeManage 시작 ===");
            System.Diagnostics.Debug.WriteLine($"TYPE: {type}, userId: {userId}, Count: {codeModelList?.Count ?? 0}");

            CodeModel model = (type.ToUpper() != "D" && codeModelList.Count > 0) ? codeModelList[0] : null;

            int? displaySeq = null;
            if (model != null && !string.IsNullOrEmpty(model.DISPLAY_SEQ) && int.TryParse(model.DISPLAY_SEQ, out int parsedSeq))
                displaySeq = parsedSeq;

            string xmlReq = null;
            if (type.ToUpper() == "D" && codeModelList != null && codeModelList.Count > 0)
                xmlReq = JsonHelper.GetJsonToXmlString<CodeModel>(codeModelList);

            string sql = "SELECT * FROM did.pr_code_manage(@p_type, @p_code_group, @p_code, @p_code_name, @p_display_seq, @p_code_desc, @p_code_attr1, @p_code_attr2, @p_code_attr3, @p_reg_id, @p_xml_req)";

            NpgsqlParameter[] param = {
                new NpgsqlParameter("@p_type", type ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_code_group", model?.CODE_GROUP ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_code", model?.CODE ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_code_name", model?.CODE_NAME ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_display_seq", NpgsqlDbType.Integer) { Value = displaySeq ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_code_desc", model?.CODE_DESC ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_code_attr1", model?.CODE_ATTR1 ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_code_attr2", model?.CODE_ATTR2 ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_code_attr3", model?.CODE_ATTR3 ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_reg_id", userId ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_xml_req", NpgsqlDbType.Text) { Value = (object)xmlReq ?? DBNull.Value }
            };

            try
            {
                DataSet ds = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var result = Util.ConvertDataTable<ResultModel>(ds.Tables[0]);
                    if (result != null && result.Count > 0)
                        return result[0];
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"예외 발생: {ex.Message}");
                return new ResultModel { ERR_CODE = "9999", ERROR_MSG = ex.Message };
            }

            return new ResultModel { ERR_CODE = "9999", ERROR_MSG = "프로시저 실행 결과를 받지 못했습니다." };
        }
    }
}

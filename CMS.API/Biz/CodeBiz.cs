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
        /// <param name="pCodeGroup">그룹코드</param>
        /// <param name="pCodeGroupNm">그룹코드명</param>
        /// <returns></returns>
        public static DataSet getGroupCodes(string pCodeGroup, string pCodeGroupNm)
        {
            string sql = "SELECT * FROM publicdata.pr_code_group_list(@p_code_group, @p_code_group_nm)";

            NpgsqlParameter[] param = {
                new NpgsqlParameter("@p_code_group", NpgsqlDbType.Varchar) { Value = pCodeGroup ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_code_group_nm", NpgsqlDbType.Varchar) { Value = pCodeGroupNm ?? (object)DBNull.Value }
            };

            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);
        }

            /// <summary>
            /// 코드 조회
            /// </summary>
            /// <param name="pCodeGroup">그룹 코드</param>
            /// <returns></returns>
            public static DataSet getCodes(string pCodeGroup)
            {
                // CommandType.Text로 직접 함수 호출
                string sql = "SELECT * FROM publicdata.pr_code_list(@p_code_group)";

                NpgsqlParameter[] param = {
            new NpgsqlParameter("@p_code_group", pCodeGroup ?? "")
        };

                return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);
            }



        /// <summary>
        /// 그룹코드 생성, 수정, 삭제 메서드
        /// </summary>
        /// <param name="TYPE">I : INSERT, U : UPDATE, D : DELETE</param>
        /// <param name="userId">사용자 아이디</param>
        /// <param name="codeModelList">대상 그룹코드 목록</param>
        /// <returns></returns>
        public static ResultModel codeGroupManage(string type, string userId, List<CodeGroupModel> codeGroupModelList)
        {
            // 디버깅 로그
            System.Diagnostics.Debug.WriteLine($"=== codeGroupManage 시작 ===");
            System.Diagnostics.Debug.WriteLine($"TYPE: {type}");
            System.Diagnostics.Debug.WriteLine($"userId: {userId}");
            System.Diagnostics.Debug.WriteLine($"codeGroupModelList Count: {codeGroupModelList?.Count ?? 0}");
            
            if (codeGroupModelList != null && codeGroupModelList.Count > 0)
            {
                for (int i = 0; i < codeGroupModelList.Count; i++)
                {
                    var item = codeGroupModelList[i];
                    System.Diagnostics.Debug.WriteLine($"[{i}] CODE_GROUP: '{item.CODE_GROUP}', CODE_GROUP_NM: '{item.CODE_GROUP_NM}', DISPLAY_SEQ: {item.DISPLAY_SEQ}");
                }
            }

            // 삭제가 아닐 때만 첫 번째 항목 사용
            CodeGroupModel model = (type.ToUpper() != "D" && codeGroupModelList.Count > 0) 
                ? codeGroupModelList[0] 
                : null;

            // XML은 삭제 시에만 전달 (리스트가 있을 때만)
            string xmlReq = null;
            if (type.ToUpper() == "D" && codeGroupModelList != null && codeGroupModelList.Count > 0)
            {
                xmlReq = JsonHelper.GetJsonToXmlString<CodeGroupModel>(codeGroupModelList);
                System.Diagnostics.Debug.WriteLine($"XML 생성됨: {xmlReq}");
            }

            System.Diagnostics.Debug.WriteLine($"최종 model - CODE_GROUP: '{model?.CODE_GROUP}', CODE_GROUP_NM: '{model?.CODE_GROUP_NM}', DISPLAY_SEQ: {model?.DISPLAY_SEQ ?? 0}");
            System.Diagnostics.Debug.WriteLine($"XML: {xmlReq ?? "NULL"}");

            string sql = "SELECT * FROM publicdata.pr_code_group_manage(@p_type, @p_code_group, @p_code_group_nm, @p_display_seq, @p_code_group_desc, @p_code_group_attr1, @p_code_group_attr2, @p_code_group_attr3, @p_reg_id, @p_xml_req)";

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
                
                System.Diagnostics.Debug.WriteLine($"프로시저 실행 완료. Tables Count: {ds?.Tables.Count ?? 0}");
                
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"결과 행 수: {ds.Tables[0].Rows.Count}");
                    System.Diagnostics.Debug.WriteLine($"컬럼: {string.Join(", ", ds.Tables[0].Columns.Cast<DataColumn>().Select(c => c.ColumnName))}");
                    
                    var result = Util.ConvertDataTable<ResultModel>(ds.Tables[0]);
                    if (result != null && result.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"결과: ERR_CODE={result[0].ERR_CODE}, ERROR_MSG={result[0].ERROR_MSG}");
                        return result[0];
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"예외 발생: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                return new ResultModel { ERR_CODE = "9999", ERROR_MSG = ex.Message };
            }
            
            return new ResultModel { ERR_CODE = "9999", ERROR_MSG = "프로시저 실행 결과를 받지 못했습니다." };
        }

        /// <summary>
        /// 코드 생성, 수정, 삭제 메서드
        /// </summary>
        /// <param name="TYPE">I : INSERT, U : UPDATE, D : DELETE</param>
        /// <param name="userId">사용자 아이디</param>
        /// <param name="codeModelList">대상 코드 목록</param>
        /// <returns></returns>
        public static ResultModel codeManage(string type, string userId, List<CodeModel> codeModelList)
        {
            // 디버깅 로그
            System.Diagnostics.Debug.WriteLine($"=== codeManage 시작 ===");
            System.Diagnostics.Debug.WriteLine($"TYPE: {type}");
            System.Diagnostics.Debug.WriteLine($"userId: {userId}");
            System.Diagnostics.Debug.WriteLine($"codeModelList Count: {codeModelList?.Count ?? 0}");
            
            if (codeModelList != null && codeModelList.Count > 0)
            {
                for (int i = 0; i < codeModelList.Count; i++)
                {
                    var item = codeModelList[i];
                    System.Diagnostics.Debug.WriteLine($"[{i}] CODE_GROUP: '{item.CODE_GROUP}', CODE: '{item.CODE}', CODE_NAME: '{item.CODE_NAME}', DISPLAY_SEQ: {item.DISPLAY_SEQ}");
                }
            }

            // 삭제가 아닐 때만 첫 번째 항목 사용
            CodeModel model = (type.ToUpper() != "D" && codeModelList.Count > 0) 
                ? codeModelList[0] 
                : null;

            // DISPLAY_SEQ를 integer로 변환 (CodeModel은 string 타입)
            int? displaySeq = null;
            if (model != null && !string.IsNullOrEmpty(model.DISPLAY_SEQ) && int.TryParse(model.DISPLAY_SEQ, out int parsedSeq))
            {
                displaySeq = parsedSeq;
            }

            // XML은 삭제 시에만 전달 (리스트가 있을 때만)
            string xmlReq = null;
            if (type.ToUpper() == "D" && codeModelList != null && codeModelList.Count > 0)
            {
                xmlReq = JsonHelper.GetJsonToXmlString<CodeModel>(codeModelList);
                System.Diagnostics.Debug.WriteLine($"XML 생성됨: {xmlReq}");
            }

            System.Diagnostics.Debug.WriteLine($"최종 model - CODE_GROUP: '{model?.CODE_GROUP}', CODE: '{model?.CODE}', CODE_NAME: '{model?.CODE_NAME}', DISPLAY_SEQ: {model?.DISPLAY_SEQ}");
            System.Diagnostics.Debug.WriteLine($"XML: {xmlReq ?? "NULL"}");

            string sql = "SELECT * FROM publicdata.pr_code_manage(@p_type, @p_code_group, @p_code, @p_code_name, @p_display_seq, @p_code_desc, @p_code_attr1, @p_code_attr2, @p_code_attr3, @p_reg_id, @p_xml_req)";

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
                
                System.Diagnostics.Debug.WriteLine($"프로시저 실행 완료. Tables Count: {ds?.Tables.Count ?? 0}");
                
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"결과 행 수: {ds.Tables[0].Rows.Count}");
                    System.Diagnostics.Debug.WriteLine($"컬럼: {string.Join(", ", ds.Tables[0].Columns.Cast<DataColumn>().Select(c => c.ColumnName))}");
                    
                    var result = Util.ConvertDataTable<ResultModel>(ds.Tables[0]);
                    if (result != null && result.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"결과: ERR_CODE={result[0].ERR_CODE}, ERROR_MSG={result[0].ERROR_MSG}");
                        return result[0];
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"예외 발생: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                return new ResultModel { ERR_CODE = "9999", ERROR_MSG = ex.Message };
            }
            
            return new ResultModel { ERR_CODE = "9999", ERROR_MSG = "프로시저 실행 결과를 받지 못했습니다." };
        }
    }
}
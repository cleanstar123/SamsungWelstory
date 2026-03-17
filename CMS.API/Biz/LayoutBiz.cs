using System.Collections.Generic;
using System.Data;
using System;


using CMS.API.Models;
using CMS.API.App_Code;
using Npgsql;
using NpgsqlTypes;

namespace CMS.API.Biz
{
    public class LayoutBiz
    {
        /// <summary>
        /// 레이아웃 조회(페이징)
        /// </summary>
        /// <param name="layoutModel"></param>
        /// <returns></returns>
        public static DataSet getLayoutPageList(LayoutModel layoutModel)
        {
            // PAGE_CNT, PAGE_NO를 integer로 변환
            int? pageCnt = null;
            if (!string.IsNullOrEmpty(layoutModel.PAGE_CNT) && int.TryParse(layoutModel.PAGE_CNT, out int parsedCnt))
            {
                pageCnt = parsedCnt;
            }

            int? pageNo = null;
            if (!string.IsNullOrEmpty(layoutModel.PAGE_NO) && int.TryParse(layoutModel.PAGE_NO, out int parsedNo))
            {
                pageNo = parsedNo;
            }

            NpgsqlParameter[] param = {
                new NpgsqlParameter("P_RESTAURANT_CODE", NpgsqlDbType.Varchar) { Value = layoutModel.RESTAURANT_CODE ?? (object)DBNull.Value },
                new NpgsqlParameter("P_LAYOUT_NM", NpgsqlDbType.Varchar) { Value = layoutModel.LAYOUT_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("P_PAGE_CNT", NpgsqlDbType.Integer) { Value = pageCnt ?? (object)DBNull.Value },
                new NpgsqlParameter("P_PAGE_NO", NpgsqlDbType.Integer) { Value = pageNo ?? (object)DBNull.Value }
            };

            DataSet result = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "did.pr_layout_list_page", param);
            
            // JavaScript가 Table과 Table1을 기대하므로 분리
            if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
            {
                DataSet ds = new DataSet();
                
                // Table: 레이아웃 목록
                DataTable dtList = result.Tables[0].Clone();
                dtList.TableName = "Table";
                foreach (DataRow row in result.Tables[0].Rows)
                {
                    dtList.ImportRow(row);
                }
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
        /// 레이아웃 상세
        /// </summary>
        /// <param name="layoutModel"></param>
        /// <returns></returns>
        public static DataSet layoutDetail(LayoutModel layoutModel)
        {
            // LAYOUT_ID를 integer로 변환
            int? layoutId = null;
            if (!string.IsNullOrEmpty(layoutModel.LAYOUT_ID) && int.TryParse(layoutModel.LAYOUT_ID, out int parsedId))
            {
                layoutId = parsedId;
            }

            NpgsqlParameter[] param = {
                new NpgsqlParameter("P_RESTAURANT_CODE", NpgsqlDbType.Varchar) { Value = layoutModel.RESTAURANT_CODE ?? (object)DBNull.Value },
                new NpgsqlParameter("P_LAYOUT_ID", NpgsqlDbType.Integer) { Value = layoutId ?? (object)DBNull.Value }
            };

            // 레이아웃 기본 정보 조회
            DataSet ds = new DataSet();
            DataSet dsLayout = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "did.pr_layout_list", param);
            if (dsLayout.Tables.Count > 0)
            {
                DataTable dtLayout = dsLayout.Tables[0].Copy();
                dtLayout.TableName = "Table";
                ds.Tables.Add(dtLayout);
            }

            // 레이아웃 상세 정보 조회
            DataSet dsDetail = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "did.pr_layout_detail_list", param);
            if (dsDetail.Tables.Count > 0)
            {
                DataTable dtDetail = dsDetail.Tables[0].Copy();
                dtDetail.TableName = "Table1";
                ds.Tables.Add(dtDetail);
            }

            return ds;
        }

        /// <summary>
        /// 레이아웃 저장
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userId"></param>
        /// <param name="layoutModel"></param>
        /// <param name="layoutDetailModels"></param>
        /// <param name="layoutImageModels"></param>
        /// <returns></returns>
        public static ResultModel layoutManage(string type, string userId, LayoutModel layoutModel, List<LayoutDetailModel> layoutDetailModels, List<LayoutImageModel> layoutImageModels)
        {
            // 디버깅: 입력 데이터 로깅
            System.Diagnostics.Debug.WriteLine("=== LayoutBiz.layoutManage 시작 ===");
            System.Diagnostics.Debug.WriteLine($"Type: {type}");
            System.Diagnostics.Debug.WriteLine($"layoutDetailModels count: {layoutDetailModels?.Count ?? 0}");
            System.Diagnostics.Debug.WriteLine($"layoutImageModels count: {layoutImageModels?.Count ?? 0}");

            // 문자열을 정수로 변환
            int? layoutId = string.IsNullOrEmpty(layoutModel.LAYOUT_ID) ? (int?)null : int.Parse(layoutModel.LAYOUT_ID);
            int? contentCnt = string.IsNullOrEmpty(layoutModel.CONTENT_CNT) ? (int?)null : int.Parse(layoutModel.CONTENT_CNT);
            int? screenW = string.IsNullOrEmpty(layoutModel.SCREEN_W) ? (int?)null : int.Parse(layoutModel.SCREEN_W);
            int? screenH = string.IsNullOrEmpty(layoutModel.SCREEN_H) ? (int?)null : int.Parse(layoutModel.SCREEN_H);

            // XML 변환
            string xmlImg = JsonHelper.GetJsonToXmlString<LayoutImageModel>(layoutImageModels);
            string xmlDetail = JsonHelper.GetJsonToXmlString<LayoutDetailModel>(layoutDetailModels);

            // 디버깅: XML 변환 결과 로깅
            System.Diagnostics.Debug.WriteLine("=== XML 변환 결과 ===");
            System.Diagnostics.Debug.WriteLine($"xmlImg length: {xmlImg?.Length ?? 0}");
            System.Diagnostics.Debug.WriteLine($"xmlImg: {xmlImg}");
            System.Diagnostics.Debug.WriteLine($"xmlDetail length: {xmlDetail?.Length ?? 0}");
            System.Diagnostics.Debug.WriteLine($"xmlDetail: {xmlDetail}");

            string sql = @"SELECT * FROM did.pr_layout_manage(
                @p_type, @p_restaurant_code, @p_layout_id, @p_layout_type, 
                @p_layout_nm, @p_layout_desc, @p_file_nm, @p_content_cnt, 
                @p_screen_w, @p_screen_h, @p_layout_hv_type, @p_layout_h_by_v, 
                @p_eval_use_yn, @p_thumbnail_nm, @p_reg_id, @p_xml_req_img, @p_xml_req_detail
            )";

            NpgsqlParameter[] param = {
                new NpgsqlParameter("@p_type", type ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_restaurant_code", layoutModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_layout_id", NpgsqlDbType.Integer) { Value = layoutId ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_layout_type", layoutModel.LAYOUT_TYPE ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_layout_nm", layoutModel.LAYOUT_NM ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_layout_desc", layoutModel.LAYOUT_DESC ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_file_nm", layoutModel.FILE_NM ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_content_cnt", NpgsqlDbType.Integer) { Value = contentCnt ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_screen_w", NpgsqlDbType.Integer) { Value = screenW ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_screen_h", NpgsqlDbType.Integer) { Value = screenH ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_layout_hv_type", layoutModel.LAYOUT_HV_TYPE ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_layout_h_by_v", layoutModel.LAYOUT_H_BY_V ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_eval_use_yn", layoutModel.EVAL_USE_YN ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_thumbnail_nm", layoutModel.THUMBNAIL_NM ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_reg_id", userId ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_xml_req_img", xmlImg ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_xml_req_detail", xmlDetail ?? (object)DBNull.Value)
            };

            // 디버깅: 파라미터 로깅
            System.Diagnostics.Debug.WriteLine("=== 프로시저 파라미터 ===");
            foreach (var p in param)
            {
                System.Diagnostics.Debug.WriteLine($"{p.ParameterName}: {p.Value}");
            }

            var result = Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param).Tables[0])[0];

            // 디버깅: 결과 로깅
            System.Diagnostics.Debug.WriteLine("=== 프로시저 실행 결과 ===");
            System.Diagnostics.Debug.WriteLine($"ERR_CODE: {result.ERR_CODE}");
            System.Diagnostics.Debug.WriteLine($"ERROR_MSG: {result.ERROR_MSG}");
            System.Diagnostics.Debug.WriteLine($"ID: {result.ID}");

            return result;
        }

        /// <summary>
        /// 레이아웃 수정/삭제 (단일 정보 수정)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userId"></param>
        /// <param name="layoutModel"></param>
        /// <returns></returns>
        public static ResultModel layoutManage(string type, string userId, LayoutModel layoutModel)
        {
            // 문자열을 정수로 변환
            int? layoutId = string.IsNullOrEmpty(layoutModel.LAYOUT_ID) ? (int?)null : int.Parse(layoutModel.LAYOUT_ID);
            int? contentCnt = string.IsNullOrEmpty(layoutModel.CONTENT_CNT) ? (int?)null : int.Parse(layoutModel.CONTENT_CNT);
            int? screenW = string.IsNullOrEmpty(layoutModel.SCREEN_W) ? (int?)null : int.Parse(layoutModel.SCREEN_W);
            int? screenH = string.IsNullOrEmpty(layoutModel.SCREEN_H) ? (int?)null : int.Parse(layoutModel.SCREEN_H);

            string sql = @"SELECT * FROM did.pr_layout_manage(
                @p_type, @p_restaurant_code, @p_layout_id, @p_layout_type, 
                @p_layout_nm, @p_layout_desc, @p_file_nm, @p_content_cnt, 
                @p_screen_w, @p_screen_h, @p_layout_hv_type, @p_layout_h_by_v, 
                @p_eval_use_yn, @p_thumbnail_nm, @p_reg_id, @p_xml_req_img, @p_xml_req_detail
            )";

            NpgsqlParameter[] param = {
                new NpgsqlParameter("@p_type", type ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_restaurant_code", layoutModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_layout_id", NpgsqlDbType.Integer) { Value = layoutId ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_layout_type", layoutModel.LAYOUT_TYPE ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_layout_nm", layoutModel.LAYOUT_NM ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_layout_desc", layoutModel.LAYOUT_DESC ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_file_nm", layoutModel.FILE_NM ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_content_cnt", NpgsqlDbType.Integer) { Value = contentCnt ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_screen_w", NpgsqlDbType.Integer) { Value = screenW ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_screen_h", NpgsqlDbType.Integer) { Value = screenH ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_layout_hv_type", layoutModel.LAYOUT_HV_TYPE ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_layout_h_by_v", layoutModel.LAYOUT_H_BY_V ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_eval_use_yn", layoutModel.EVAL_USE_YN ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_thumbnail_nm", layoutModel.THUMBNAIL_NM ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_reg_id", userId ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_xml_req_img", DBNull.Value),
                new NpgsqlParameter("@p_xml_req_detail", DBNull.Value)
            };

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param).Tables[0])[0];
        }
    }
}

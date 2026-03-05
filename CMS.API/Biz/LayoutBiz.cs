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

            DataSet result = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "publicdata.pr_layout_list_page", param);
            
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
            DataSet dsLayout = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "publicdata.pr_layout_list", param);
            if (dsLayout.Tables.Count > 0)
            {
                DataTable dtLayout = dsLayout.Tables[0].Copy();
                dtLayout.TableName = "Table";
                ds.Tables.Add(dtLayout);
            }

            // 레이아웃 상세 정보 조회
            DataSet dsDetail = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "publicdata.pr_layout_detail_list", param);
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
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_TYPE",            type),
                                          new NpgsqlParameter("P_RESTAURANT_CODE", layoutModel.RESTAURANT_CODE),
                                          new NpgsqlParameter("P_LAYOUT_ID",       layoutModel.LAYOUT_ID),
                                          new NpgsqlParameter("P_LAYOUT_TYPE",     layoutModel.LAYOUT_TYPE),
                                          new NpgsqlParameter("P_LAYOUT_NM",       layoutModel.LAYOUT_NM),
                                          new NpgsqlParameter("P_LAYOUT_DESC",     layoutModel.LAYOUT_DESC),
                                          new NpgsqlParameter("P_FILE_NM",         layoutModel.FILE_NM),
                                          new NpgsqlParameter("P_CONTENT_CNT",     layoutModel.CONTENT_CNT),
                                          new NpgsqlParameter("P_SCREEN_W",        layoutModel.SCREEN_W),
                                          new NpgsqlParameter("P_SCREEN_H",        layoutModel.SCREEN_H),
                                          new NpgsqlParameter("P_LAYOUT_HV_TYPE",  layoutModel.LAYOUT_HV_TYPE),
                                          new NpgsqlParameter("P_LAYOUT_H_BY_V",   layoutModel.LAYOUT_H_BY_V),
                                          new NpgsqlParameter("P_EVAL_USE_YN",     layoutModel.EVAL_USE_YN),
                                          new NpgsqlParameter("P_THUMBNAIL_NM",    layoutModel.THUMBNAIL_NM),
                                          new NpgsqlParameter("P_REG_ID",          userId),
                                          new NpgsqlParameter("P_XML_REQ_IMG",     JsonHelper.GetJsonToXmlString<LayoutImageModel>(layoutImageModels)),
                                          new NpgsqlParameter("P_XML_REQ_DETAIL",  JsonHelper.GetJsonToXmlString<LayoutDetailModel>(layoutDetailModels)),
                                          new NpgsqlParameter("CUR",               NpgsqlDbType.Refcursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "PKG_CMS_LAYOUT.PR_LAYOUT_MANAGE", param).Tables[0])[0];
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
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_TYPE",            type),
                                          new NpgsqlParameter("P_RESTAURANT_CODE", layoutModel.RESTAURANT_CODE),
                                          new NpgsqlParameter("P_LAYOUT_ID",       layoutModel.LAYOUT_ID),
                                          new NpgsqlParameter("P_LAYOUT_TYPE",     layoutModel.LAYOUT_TYPE),
                                          new NpgsqlParameter("P_LAYOUT_NM",       layoutModel.LAYOUT_NM),
                                          new NpgsqlParameter("P_LAYOUT_DESC",     layoutModel.LAYOUT_DESC),
                                          new NpgsqlParameter("P_FILE_NM",         layoutModel.FILE_NM),
                                          new NpgsqlParameter("P_CONTENT_CNT",     layoutModel.CONTENT_CNT),
                                          new NpgsqlParameter("P_SCREEN_W",        layoutModel.SCREEN_W),
                                          new NpgsqlParameter("P_SCREEN_H",        layoutModel.SCREEN_H),
                                          new NpgsqlParameter("P_LAYOUT_HV_TYPE",  layoutModel.LAYOUT_HV_TYPE),
                                          new NpgsqlParameter("P_LAYOUT_H_BY_V",   layoutModel.LAYOUT_H_BY_V),
                                          new NpgsqlParameter("P_EVAL_USE_YN",     layoutModel.EVAL_USE_YN),
                                          new NpgsqlParameter("P_THUMBNAIL_NM",    layoutModel.THUMBNAIL_NM),
                                          new NpgsqlParameter("P_REG_ID",          userId),
                                          new NpgsqlParameter("P_XML_REQ_IMG",     null),
                                          new NpgsqlParameter("P_XML_REQ_DETAIL",  null),
                                          new NpgsqlParameter("CUR",               NpgsqlDbType.Refcursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "PKG_CMS_LAYOUT.PR_LAYOUT_MANAGE", param).Tables[0])[0];
        }
    }
}
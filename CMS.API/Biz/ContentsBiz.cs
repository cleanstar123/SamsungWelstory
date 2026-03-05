using System.Collections.Generic;
using System.Data;
using System;

using CMS.API.Models;
using CMS.API.App_Code;
using Npgsql;
using NpgsqlTypes;

namespace CMS.API.Biz
{
    public class ContentsBiz
    {
        /// <summary>
        /// 컨텐츠 조회(페이징)
        /// </summary>
        /// <param name="contentsModel"></param>
        /// <returns></returns>
        public static DataSet getContentsPageList(ContentsModel contentsModel)
        {
            // PAGE_CNT, PAGE_NO를 integer로 변환
            int? pageCnt = null;
            if (!string.IsNullOrEmpty(contentsModel.PAGE_CNT) && int.TryParse(contentsModel.PAGE_CNT, out int parsedCnt))
            {
                pageCnt = parsedCnt;
            }

            int? pageNo = null;
            if (!string.IsNullOrEmpty(contentsModel.PAGE_NO) && int.TryParse(contentsModel.PAGE_NO, out int parsedNo))
            {
                pageNo = parsedNo;
            }

            NpgsqlParameter[] param = {
                new NpgsqlParameter("P_RESTAURANT_CODE", NpgsqlDbType.Varchar) { Value = contentsModel.RESTAURANT_CODE ?? (object)DBNull.Value },
                new NpgsqlParameter("P_CONTENT_NM", NpgsqlDbType.Varchar) { Value = contentsModel.CONTENT_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("P_CONTENT_TYPE", NpgsqlDbType.Varchar) { Value = contentsModel.CONTENT_TYPE ?? (object)DBNull.Value },
                new NpgsqlParameter("P_PAGE_CNT", NpgsqlDbType.Integer) { Value = pageCnt ?? (object)DBNull.Value },
                new NpgsqlParameter("P_PAGE_NO", NpgsqlDbType.Integer) { Value = pageNo ?? (object)DBNull.Value }
            };

            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "publicdata.pr_content_list_page", param);
        }

        /// <summary>
        /// 컨텐츠 저장/수정
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="userId"></param>
        /// <param name="restaurantCode"></param>
        /// <param name="contentsFileModels"></param>
        /// <returns></returns>
        public static ResultModel manageContents(string actionType, string userId, string restaurantCode, List<ContentsModel> contentsFileModels)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[manageContents] actionType: {actionType}");
                System.Diagnostics.Debug.WriteLine($"[manageContents] userId: {userId}");
                System.Diagnostics.Debug.WriteLine($"[manageContents] restaurantCode: {restaurantCode}");
                System.Diagnostics.Debug.WriteLine($"[manageContents] contentsFileModels count: {contentsFileModels?.Count}");

                string xmlReq = JsonHelper.GetJsonToXmlString(contentsFileModels);
                System.Diagnostics.Debug.WriteLine($"[manageContents] XML: {xmlReq}");

                NpgsqlParameter[] param = {
                    new NpgsqlParameter("P_TYPE", NpgsqlDbType.Varchar) { Value = actionType ?? (object)DBNull.Value },
                    new NpgsqlParameter("P_RESTAURANT_CODE", NpgsqlDbType.Varchar) { Value = restaurantCode ?? (object)DBNull.Value },
                    new NpgsqlParameter("P_REG_ID", NpgsqlDbType.Varchar) { Value = userId ?? (object)DBNull.Value },
                    new NpgsqlParameter("P_XML_REQ", NpgsqlDbType.Text) { Value = xmlReq ?? (object)DBNull.Value }
                };

                var result = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "publicdata.pr_content_manage", param);
                return Util.ConvertDataTable<ResultModel>(result.Tables[0])[0];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[manageContents ERROR] {ex.GetType().Name}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[manageContents ERROR] StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[manageContents ERROR] InnerException: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        /// <summary>
        /// 컨텐츠 삭제
        /// </summary>
        /// <param name="restaurantCode"></param>
        /// <param name="userId"></param>
        /// <param name="contentsModels"></param>
        /// <returns></returns>
        public static ResultModel manageDelete(string restaurantCode, string userId, List<ContentsModel> contentsModels)
        {
            NpgsqlParameter[] param = {
                new NpgsqlParameter("P_RESTAURANT_CODE", NpgsqlDbType.Varchar) { Value = restaurantCode ?? (object)DBNull.Value },
                new NpgsqlParameter("P_REG_ID", NpgsqlDbType.Varchar) { Value = userId ?? (object)DBNull.Value },
                new NpgsqlParameter("P_XML_REQ", NpgsqlDbType.Text) { Value = JsonHelper.GetJsonToXmlString(contentsModels) ?? (object)DBNull.Value }
            };

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "publicdata.pr_content_manage_delete", param).Tables[0])[0];
        }
    }
}
using System.Collections.Generic;
using System.Data;

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
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_RESTAURANT_CODE", layoutModel.RESTAURANT_CODE),
                                          new NpgsqlParameter("P_LAYOUT_NM",       layoutModel.LAYOUT_NM),
                                          new NpgsqlParameter("P_PAGE_CNT",        layoutModel.PAGE_CNT),
                                          new NpgsqlParameter("P_PAGE_NO",         layoutModel.PAGE_NO),
                                          new NpgsqlParameter("CUR",               NpgsqlDbType.Refcursor),
                                          new NpgsqlParameter("CUR_COUNT",         NpgsqlDbType.Refcursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;
            param[param.Length - 2].Direction = ParameterDirection.Output;

            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "PKG_CMS_LAYOUT.PR_LAYOUT_LIST_PAGE", param);
        }

        /// <summary>
        /// 레이아웃 상세
        /// </summary>
        /// <param name="layoutModel"></param>
        /// <returns></returns>
        public static DataSet layoutDetail(LayoutModel layoutModel)
        {
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_RESTAURANT_CODE", layoutModel.RESTAURANT_CODE),
                                          new NpgsqlParameter("P_LAYOUT_ID",       layoutModel.LAYOUT_ID),
                                          new NpgsqlParameter("CUR",               NpgsqlDbType.Refcursor),
                                          new NpgsqlParameter("CUR_DETAIL",        NpgsqlDbType.Refcursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;
            param[param.Length - 2].Direction = ParameterDirection.Output;

            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "PKG_CMS_LAYOUT.PR_LAYOUT_ALL_LIST", param);
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
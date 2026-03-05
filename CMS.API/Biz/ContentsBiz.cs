using System.Collections.Generic;
using System.Data;

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
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_RESTAURANT_CODE", contentsModel.RESTAURANT_CODE),
                                          new NpgsqlParameter("P_CONTENT_NM",      contentsModel.CONTENT_NM),
                                          new NpgsqlParameter("P_CONTENT_TYPE",    contentsModel.CONTENT_TYPE),
                                          new NpgsqlParameter("P_PAGE_CNT",        contentsModel.PAGE_CNT),
                                          new NpgsqlParameter("P_PAGE_NO",         contentsModel.PAGE_NO),
                                          new NpgsqlParameter("CUR",               NpgsqlDbType.Refcursor),
                                          new NpgsqlParameter("CUR_COUNT",         NpgsqlDbType.Refcursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;
            param[param.Length - 2].Direction = ParameterDirection.Output;

            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_CONTENTS.PR_CONTENT_LIST_PAGE", param);
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
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_TYPE",            actionType),
                                          new NpgsqlParameter("P_RESTAURANT_CODE", restaurantCode),
                                          new NpgsqlParameter("P_REG_ID",          userId),
                                          new NpgsqlParameter("P_XML_REQ",         JsonHelper.GetJsonToXmlString(contentsFileModels)),
                                          new NpgsqlParameter("CUR", NpgsqlDbType.Refcursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_CONTENTS.PR_CONTENT_MANAGE", param).Tables[0])[0];
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
                                          new NpgsqlParameter("P_RESTAURANT_CODE", restaurantCode),
                                          new NpgsqlParameter("P_REG_ID",          userId),
                                          new NpgsqlParameter("P_XML_REQ",         JsonHelper.GetJsonToXmlString(contentsModels)),
                                          new NpgsqlParameter("CUR", NpgsqlDbType.Refcursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_CONTENTS.PR_CONTENT_MANAGE_DELETE", param).Tables[0])[0];
        }
    }
}
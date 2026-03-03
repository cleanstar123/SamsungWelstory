using System.Collections.Generic;
using System.Data;

using CMS.API.Models;
using CMS.API.App_Code;
using Oracle.ManagedDataAccess.Client;

namespace CMS.API.Biz
{
    public class DisplayBiz
    {
        /// <summary>
        /// 디스플레이 조회
        /// </summary>
        /// <param name="displayModel"></param>
        /// <returns></returns>
        public static DataSet getDisplays(DisplayModel displayModel)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_RESTAURANT_CODE", displayModel.RESTAURANT_CODE),
                                          new OracleParameter("P_DISPLAY_ID",      displayModel.DISPLAY_ID),
                                          new OracleParameter("P_DISPLAY_NM",      displayModel.DISPLAY_NM),
                                          new OracleParameter("CUR",               OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_DISPLAY.PR_DISPLAY_LIST", param);
        }

        /// <summary>
        /// 디스플레이 저장/수정/삭제
        /// </summary>
        /// <param name="type"></param>
        /// <param name="restaurantCode"></param>
        /// <param name="displayModels"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ResultModel displayManage(string type, string restaurantCode, List<DisplayModel> displayModels, string userId)
        {
            DisplayModel model = (type.ToUpper() != "D" ? displayModels[0] : new DisplayModel());

            OracleParameter[] param = {
                                          new OracleParameter("P_TYPE",            type),
                                          new OracleParameter("P_RESTAURANT_CODE", restaurantCode),
                                          new OracleParameter("P_DISPLAY_ID",      model.DISPLAY_ID),
                                          new OracleParameter("P_DISPLAY_NM",      model.DISPLAY_NM),
                                          new OracleParameter("P_DISPLAY_DESC",    model.DISPLAY_DESC),
                                          new OracleParameter("P_SCREEN_W",        model.SCREEN_W),
                                          new OracleParameter("P_SCREEN_H",        model.SCREEN_H),
                                          new OracleParameter("P_DISPLAY_IP",      model.DISPLAY_OS),
                                          new OracleParameter("P_DISPLAY_IP",      model.DISPLAY_IP),
                                          new OracleParameter("P_DISPLAY_MAC",     model.DISPLAY_MAC),
                                          new OracleParameter("P_USE_YN",          model.USE_YN),
                                          new OracleParameter("P_REG_ID",          userId),
                                          new OracleParameter("P_XML_REQ",         type == "D" ? JsonHelper.GetJsonToXmlString<DisplayModel>(displayModels) : null),
                                          new OracleParameter("CUR",               OracleDbType.RefCursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_DISPLAY.PR_DISPLAY_MANAGE", param).Tables[0])[0];
        }

        /// <summary>
        /// 디스플레이 그룹 조회
        /// </summary>
        /// <param name="displayGroupModel"></param>
        /// <returns></returns>
        public static DataSet getDisplayGroups(DisplayGroupModel displayGroupModel)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_RESTAURANT_CODE", displayGroupModel.RESTAURANT_CODE),
                                          new OracleParameter("P_DISPLAY_ID",      displayGroupModel.DISPLAY_GROUP_ID),
                                          new OracleParameter("P_DISPLAY_NM",      displayGroupModel.DISPLAY_GROUP_NM),
                                          new OracleParameter("CUR",               OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_DISPLAY.PR_DISPLAY_GROUP_LIST", param);
        }

        /// <summary>
        /// 디스플레이 그룹에 속한 디스플레이 및 속하지 않은 디스플레이 조회
        /// </summary>
        /// <param name="displayGroupModel"></param>
        /// <returns></returns>
        public static DataSet getDisplayMaps(DisplayGroupModel displayGroupModel)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_RESTAURANT_CODE",  displayGroupModel.RESTAURANT_CODE),
                                          new OracleParameter("P_DISPLAY_GROUP_ID", displayGroupModel.DISPLAY_GROUP_ID),
                                          new OracleParameter("CUR",                OracleDbType.RefCursor),
                                          new OracleParameter("CUR1",               OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;
            param[param.Length - 2].Direction = ParameterDirection.Output;

            return OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_DISPLAY.PR_DISPLAY_MAP_LIST", param);
        }
        
        /// <summary>
        /// 디스플레이 그룹 저장/수정/삭제
        /// </summary>
        /// <param name="type"></param>
        /// <param name="restaurantCode"></param>
        /// <param name="displayGroupModels"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ResultModel displayGroupManage(string type, string restaurantCode, List<DisplayGroupModel> displayGroupModels, string userId)
        {
            DisplayGroupModel model = (type.ToUpper() == "D" ? new DisplayGroupModel() : displayGroupModels[0]);

            OracleParameter[] param = {
                                          new OracleParameter("P_TYPE",               type),
                                          new OracleParameter("P_RESTAURANT_CODE",    restaurantCode),
                                          new OracleParameter("P_DISPLAY_GROUP_ID",   model.DISPLAY_GROUP_ID),
                                          new OracleParameter("P_DISPLAY_GROUP_NM",   model.DISPLAY_GROUP_NM),
                                          new OracleParameter("P_DISPLAY_GROUP_DESC", model.DISPLAY_GROUP_DESC),
                                          new OracleParameter("P_REG_ID",             userId),
                                          new OracleParameter("P_XML_REQ",            type == "D" ? JsonHelper.GetJsonToXmlString<DisplayGroupModel>(displayGroupModels) : null),
                                          new OracleParameter("CUR",                  OracleDbType.RefCursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_DISPLAY.PR_DISPLAY_GROUP_MANAGE", param).Tables[0])[0];
        }

        /// <summary>
        /// 디스플레이 그룹에 속한 디스플레이 저장/수정/삭제
        /// </summary>
        /// <param name="displayGroupModel"></param>
        /// <param name="displayMapModels"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ResultModel displayMapManage(DisplayGroupModel displayGroupModel, List<DisplayMapModel> displayMapModels, string userId)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_RESTAURANT_CODE",  displayGroupModel.RESTAURANT_CODE),
                                          new OracleParameter("P_DISPLAY_GROUP_ID", displayGroupModel.DISPLAY_GROUP_ID),
                                          new OracleParameter("P_REG_ID",           userId),
                                          new OracleParameter("P_XML_REQ",          JsonHelper.GetJsonToXmlString<DisplayMapModel>(displayMapModels)),
                                          new OracleParameter("CUR",                OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_DISPLAY.PR_DISPLAY_MAP_MANAGE", param).Tables[0])[0];
        }

        /// <summary>
        /// 디스플레이 재시작
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="displayModel"></param>
        /// <returns></returns>
        public static ResultModel displayRestart(string userId, DisplayModel displayModel)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_RESTAURANT_CODE",  displayModel.RESTAURANT_CODE),
                                          new OracleParameter("P_DISPLAY_ID",       displayModel.DISPLAY_ID),
                                          new OracleParameter("P_REG_ID",           userId),
                                          new OracleParameter("CUR",                OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_DISPLAY.PR_DISPLAY_RESTART", param).Tables[0])[0];
        }
    }
}
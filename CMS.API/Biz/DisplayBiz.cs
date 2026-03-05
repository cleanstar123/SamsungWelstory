using CMS.API.App_Code;
using CMS.API.Models;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

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
            int? displayId = null;
            if (!string.IsNullOrEmpty(displayModel.DISPLAY_ID) && int.TryParse(displayModel.DISPLAY_ID, out int tempId))
            {
                displayId = tempId;
            }

            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_RESTAURANT_CODE", displayModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_DISPLAY_ID",      (object)displayId ?? DBNull.Value),
                                          new NpgsqlParameter("P_DISPLAY_NM",      displayModel.DISPLAY_NM ?? (object)DBNull.Value)
                                      };

            string sql = "SELECT * FROM publicdata.pr_display_list(@P_RESTAURANT_CODE, @P_DISPLAY_ID, @P_DISPLAY_NM)";
            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);
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

            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_TYPE",            type ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_RESTAURANT_CODE", restaurantCode ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_DISPLAY_ID",      model.DISPLAY_ID ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_DISPLAY_NM",      model.DISPLAY_NM ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_DISPLAY_DESC",    model.DISPLAY_DESC ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_SCREEN_W",        model.SCREEN_W ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_SCREEN_H",        model.SCREEN_H ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_DISPLAY_OS",      model.DISPLAY_OS ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_DISPLAY_IP",      model.DISPLAY_IP ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_DISPLAY_MAC",     model.DISPLAY_MAC ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_USE_YN",          model.USE_YN ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_REG_ID",          userId ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_XML_REQ",         type == "D" ? JsonHelper.GetJsonToXmlString<DisplayModel>(displayModels) : (object)DBNull.Value)
                                      };

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "publicdata.pr_display_manage", param).Tables[0])[0];
        }

        /// <summary>
        /// 디스플레이 그룹 조회
        /// </summary>
        /// <param name="displayGroupModel"></param>
        /// <returns></returns>
        public static DataSet getDisplayGroups(DisplayGroupModel displayGroupModel)
        {
            int? displayGroupId = null;
            if (!string.IsNullOrEmpty(displayGroupModel.DISPLAY_GROUP_ID) && int.TryParse(displayGroupModel.DISPLAY_GROUP_ID, out int tempId))
            {
                displayGroupId = tempId;
            }

            NpgsqlParameter[] param =   {
                                              new NpgsqlParameter("P_RESTAURANT_CODE", displayGroupModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                                              new NpgsqlParameter("P_DISPLAY_GROUP_ID", (object)displayGroupId ?? DBNull.Value),
                                              new NpgsqlParameter("P_DISPLAY_GROUP_NM", displayGroupModel.DISPLAY_GROUP_NM ?? (object)DBNull.Value)
                                        };

            string sql = "SELECT * FROM publicdata.pr_display_group_list(@P_RESTAURANT_CODE, @P_DISPLAY_GROUP_ID, @P_DISPLAY_GROUP_NM)";
            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);
        }

        /// <summary>
        /// 디스플레이 그룹에 속한 디스플레이 및 속하지 않은 디스플레이 조회
        /// </summary>
        /// <param name="displayGroupModel"></param>
        /// <returns></returns>
        public static DataSet getDisplayMaps(DisplayGroupModel displayGroupModel)
        {
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_RESTAURANT_CODE",  displayGroupModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_DISPLAY_GROUP_ID", displayGroupModel.DISPLAY_GROUP_ID ?? (object)DBNull.Value)
                                      };

            string sql = "SELECT * FROM publicdata.pr_display_map_list(@P_RESTAURANT_CODE, @P_DISPLAY_GROUP_ID)";
            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);
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

            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_TYPE",               type ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_RESTAURANT_CODE",    restaurantCode ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_DISPLAY_GROUP_ID",   model.DISPLAY_GROUP_ID ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_DISPLAY_GROUP_NM",   model.DISPLAY_GROUP_NM ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_DISPLAY_GROUP_DESC", model.DISPLAY_GROUP_DESC ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_REG_ID",             userId ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_XML_REQ",            type == "D" ? JsonHelper.GetJsonToXmlString<DisplayGroupModel>(displayGroupModels) : (object)DBNull.Value)
                                      };

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "publicdata.pr_display_group_manage", param).Tables[0])[0];
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
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_RESTAURANT_CODE",  displayGroupModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_DISPLAY_GROUP_ID", displayGroupModel.DISPLAY_GROUP_ID ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_REG_ID",           userId ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_XML_REQ",          JsonHelper.GetJsonToXmlString<DisplayMapModel>(displayMapModels) ?? (object)DBNull.Value)
                                      };

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "publicdata.pr_display_map_manage", param).Tables[0])[0];
        }

        /// <summary>
        /// 디스플레이 재시작
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="displayModel"></param>
        /// <returns></returns>
        public static ResultModel displayRestart(string userId, DisplayModel displayModel)
        {
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_RESTAURANT_CODE",  displayModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_DISPLAY_ID",       displayModel.DISPLAY_ID ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_REG_ID",           userId ?? (object)DBNull.Value)
                                      };

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "publicdata.pr_display_restart", param).Tables[0])[0];
        }
    }
}
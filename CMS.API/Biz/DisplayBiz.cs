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
        public static DataSet getDisplays(DisplayModel displayModel)
        {
            int? displayId = null;
            if (!string.IsNullOrEmpty(displayModel.DISPLAY_ID) && int.TryParse(displayModel.DISPLAY_ID, out int tempId))
                displayId = tempId;

            NpgsqlParameter[] param = {
                new NpgsqlParameter("P_RESTAURANT_CODE", displayModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                new NpgsqlParameter("P_DISPLAY_ID",      (object)displayId ?? DBNull.Value),
                new NpgsqlParameter("P_DISPLAY_NM",      displayModel.DISPLAY_NM ?? (object)DBNull.Value)
            };

            string sql = "SELECT * FROM did.pr_display_list(@P_RESTAURANT_CODE, @P_DISPLAY_ID, @P_DISPLAY_NM)";
            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);
        }

        /// <summary>
        /// 디스플레이 저장/수정/삭제
        /// </summary>
        public static ResultModel displayManage(string type, string restaurantCode, List<DisplayModel> displayModels, string userId)
        {
            DisplayModel model = (type.ToUpper() != "D" ? displayModels[0] : new DisplayModel());

            string sql = @"SELECT * FROM did.pr_display_manage(
                @p_type, @p_restaurant_code, @p_display_id, @p_display_nm, 
                @p_display_desc, @p_screen_w, @p_screen_h, @p_display_os, 
                @p_display_ip, @p_display_mac, @p_use_yn, @p_reg_id, @p_xml_req
            )";

            NpgsqlParameter[] param = {
                new NpgsqlParameter("@p_type", type ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_restaurant_code", restaurantCode ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_display_id", NpgsqlDbType.Integer) { Value = model.DISPLAY_ID ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_display_nm", model.DISPLAY_NM ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_display_desc", model.DISPLAY_DESC ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_screen_w", NpgsqlDbType.Integer) { Value = model.SCREEN_W ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_screen_h", NpgsqlDbType.Integer) { Value = model.SCREEN_H ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_display_os", model.DISPLAY_OS ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_display_ip", model.DISPLAY_IP ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_display_mac", model.DISPLAY_MAC ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_use_yn", model.USE_YN ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_reg_id", userId ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_xml_req", type == "D" ? JsonHelper.GetJsonToXmlString<DisplayModel>(displayModels) : (object)DBNull.Value)
            };

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param).Tables[0])[0];
        }

        /// <summary>
        /// 디스플레이 그룹 조회
        /// </summary>
        public static DataSet getDisplayGroups(DisplayGroupModel displayGroupModel)
        {
            int? displayGroupId = null;
            if (!string.IsNullOrEmpty(displayGroupModel.DISPLAY_GROUP_ID) && int.TryParse(displayGroupModel.DISPLAY_GROUP_ID, out int tempId))
                displayGroupId = tempId;

            NpgsqlParameter[] param = {
                new NpgsqlParameter("P_RESTAURANT_CODE", displayGroupModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                new NpgsqlParameter("P_DISPLAY_GROUP_ID", (object)displayGroupId ?? DBNull.Value),
                new NpgsqlParameter("P_DISPLAY_GROUP_NM", displayGroupModel.DISPLAY_GROUP_NM ?? (object)DBNull.Value)
            };

            string sql = "SELECT * FROM did.pr_display_group_list(@P_RESTAURANT_CODE, @P_DISPLAY_GROUP_ID, @P_DISPLAY_GROUP_NM)";
            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);
        }

        /// <summary>
        /// 디스플레이 그룹에 속한 디스플레이 및 속하지 않은 디스플레이 조회
        /// </summary>
        public static DataSet getDisplayMaps(DisplayGroupModel displayGroupModel)
        {
            NpgsqlParameter[] param = {
                new NpgsqlParameter("P_RESTAURANT_CODE",  displayGroupModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                new NpgsqlParameter("P_DISPLAY_GROUP_ID", displayGroupModel.DISPLAY_GROUP_ID ?? (object)DBNull.Value)
            };

            string sql = "SELECT * FROM did.pr_display_map_list(@P_RESTAURANT_CODE, @P_DISPLAY_GROUP_ID)";
            DataSet ds = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);
            
            DataTable mappedTable = null;
            DataTable unmappedTable = null;
            
            if (ds.Tables.Count > 0)
            {
                mappedTable = ds.Tables[0].Clone();
                mappedTable.TableName = "Table";
                unmappedTable = ds.Tables[0].Clone();
                unmappedTable.TableName = "Table1";
                
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        string resultType = row["RESULT_TYPE"]?.ToString() ?? "";
                        if (resultType == "MAPPED")
                            mappedTable.ImportRow(row);
                        else if (resultType == "UNMAPPED")
                            unmappedTable.ImportRow(row);
                    }
                }
                
                ds.Tables.Clear();
                ds.Tables.Add(mappedTable);
                ds.Tables.Add(unmappedTable);
            }
            
            return ds;
        }

        /// <summary>
        /// 디스플레이 그룹 저장/수정/삭제
        /// </summary>
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

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "did.pr_display_group_manage", param).Tables[0])[0];
        }

        /// <summary>
        /// 디스플레이 그룹에 속한 디스플레이 저장/수정/삭제
        /// </summary>
        public static ResultModel displayMapManage(DisplayGroupModel displayGroupModel, List<DisplayMapModel> displayMapModels, string userId)
        {
            string sql = @"SELECT * FROM did.pr_display_map_manage(
                @p_restaurant_code, @p_display_group_id, @p_reg_id, @p_xml_req
            )";

            NpgsqlParameter[] param = {
                new NpgsqlParameter("@p_restaurant_code", displayGroupModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_display_group_id", NpgsqlDbType.Integer) { Value = displayGroupModel.DISPLAY_GROUP_ID ?? (object)DBNull.Value },
                new NpgsqlParameter("@p_reg_id", userId ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_xml_req", JsonHelper.GetJsonToXmlString<DisplayMapModel>(displayMapModels) ?? (object)DBNull.Value)
            };

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param).Tables[0])[0];
        }

        /// <summary>
        /// 디스플레이 재시작
        /// </summary>
        public static ResultModel displayRestart(string userId, DisplayModel displayModel)
        {
            NpgsqlParameter[] param = {
                new NpgsqlParameter("P_RESTAURANT_CODE",  displayModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                new NpgsqlParameter("P_DISPLAY_ID",       displayModel.DISPLAY_ID ?? (object)DBNull.Value),
                new NpgsqlParameter("P_REG_ID",           userId ?? (object)DBNull.Value)
            };

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "did.pr_display_restart", param).Tables[0])[0];
        }
    }
}

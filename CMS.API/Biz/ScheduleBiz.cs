using System.Collections.Generic;
using System.Data;

using CMS.API.Models;
using CMS.API.App_Code;
using Npgsql;
using NpgsqlTypes;
using System;

namespace CMS.API.Biz
{
    public class ScheduleBiz
    {
        /// <summary>
        /// 스케줄 조회 (단일)
        /// </summary>
        /// <param name="scheduleModel"></param>
        /// <returns></returns>
        public static DataSet getSchedule(ScheduleModel scheduleModel)
        {
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_RESTAURANT_CODE", scheduleModel.RESTAURANT_CODE),
                                          new NpgsqlParameter("P_SCHEDULE_ID",     scheduleModel.SCHEDULE_ID),
                                          new NpgsqlParameter("CUR",               NpgsqlDbType.Refcursor),
                                          new NpgsqlParameter("CUR_DISP",          NpgsqlDbType.Refcursor),
                                          new NpgsqlParameter("CUR_TEMPLATE",      NpgsqlDbType.Refcursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;
            param[param.Length - 2].Direction = ParameterDirection.Output;
            param[param.Length - 3].Direction = ParameterDirection.Output;

            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_SCHEDULE.PR_SCHEDULE_ALL_LIST", param);
        }

        /// <summary>
        /// 스케줄 조회 (다중)
        /// </summary>
        /// <param name="scheduleModel"></param>
        /// <returns></returns>
        public static List<ScheduleResultModel> getSchedules(ScheduleModel scheduleModel)
        {
            object displayId = string.IsNullOrEmpty(scheduleModel.DISPLAY_ID)
                    ? (object)DBNull.Value
                    : int.Parse(scheduleModel.DISPLAY_ID);

            NpgsqlParameter[] param = {
                                              new NpgsqlParameter("P_RESTAURANT_CODE", scheduleModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                                              new NpgsqlParameter("P_START_DATE",      scheduleModel.START_DATE ?? (object)DBNull.Value),
                                              new NpgsqlParameter("P_END_DATE",        scheduleModel.END_DATE ?? (object)DBNull.Value),
                                              new NpgsqlParameter("P_DISPLAY_ID",      displayId)
                                          };

            string sql = "SELECT * FROM publicdata.pr_schedule_list_cal(@P_RESTAURANT_CODE, @P_START_DATE, @P_END_DATE, @P_DISPLAY_ID)";
            return Util.ConvertDataTable<ScheduleResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param).Tables[0]);

        }

        /// <summary>
        /// 스케줄 저장/수정/삭제
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userId"></param>
        /// <param name="restaurantCode"></param>
        /// <param name="scheduleModels"></param>
        /// <param name="displayModels"></param>
        /// <returns></returns>
        public static ResultModel ManageSchedule(string type, string userId, string restaurantCode, List<ScheduleModel> scheduleModels, List<ScheduleTemplateMapModel> scheduleTemplateMapModels, List<DisplayModel> displayModels)
        {
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_TYPE",            type ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_RESTAURANT_CODE", restaurantCode ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_SCHEDULE_ID",     scheduleModels[0].SCHEDULE_ID ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_REG_ID",          userId ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_XML_REQ",         JsonHelper.GetJsonToXmlString(scheduleModels) ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_XML_REQ_DISP",    JsonHelper.GetJsonToXmlString<DisplayModel>(displayModels) ?? (object)DBNull.Value),
                                          new NpgsqlParameter("P_XML_REQ_MAP",     JsonHelper.GetJsonToXmlString<ScheduleTemplateMapModel>(scheduleTemplateMapModels) ?? (object)DBNull.Value),
                                      };

            string sql = "SELECT * FROM publicdata.pr_schedule_manage(@P_TYPE, @P_RESTAURANT_CODE, @P_SCHEDULE_ID, @P_REG_ID, @P_XML_REQ, @P_XML_REQ_DISP, @P_XML_REQ_MAP)";
            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param).Tables[0])[0];
        }

        /// <summary>
        /// 스케줄 중복 체크
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userId"></param>
        /// <param name="restaurantCode"></param>
        /// <param name="scheduleModels"></param>
        /// <param name="displayModels"></param>
        /// <returns></returns>
        public static ResultModel CheckScheduleOverlap(string type, string userId, string restaurantCode, List<ScheduleModel> scheduleModels, List<DisplayModel> displayModels)
        {
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_TYPE",            type),
                                          new NpgsqlParameter("P_RESTAURANT_CODE", restaurantCode),
                                          new NpgsqlParameter("P_SCHEDULE_ID",     scheduleModels[0].SCHEDULE_ID),
                                          new NpgsqlParameter("P_REG_ID",          userId),
                                          new NpgsqlParameter("P_XML_REQ",         JsonHelper.GetJsonToXmlString(scheduleModels)),
                                          new NpgsqlParameter("P_XML_REQ_DISP",    JsonHelper.GetJsonToXmlString<DisplayModel>(displayModels)),
                                          new NpgsqlParameter("CUR",               NpgsqlDbType.Refcursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_SCHEDULE.PR_SCHEDULE_OVERLAP_CHECK", param).Tables[0])[0];
        }

        /// <summary>
        /// 롤링 스케줄에 맵핑된 템플릿 정보 조회
        /// </summary>
        /// <param name="RESTAURANT_CODE"></param>
        /// <param name="SCHEDULE_ID"></param>
        /// <returns></returns>
        public static DataSet getScheduleTemplateMapList(string RESTAURANT_CODE, string SCHEDULE_ID)
        {
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_RESTAURANT_CODE", RESTAURANT_CODE),
                                          new NpgsqlParameter("P_SCHEDULE_ID",     SCHEDULE_ID),
                                          new NpgsqlParameter("CUR",               NpgsqlDbType.Refcursor),
                                          new NpgsqlParameter("CUR_TOT",           NpgsqlDbType.Refcursor),
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;
            param[param.Length - 2].Direction = ParameterDirection.Output;

            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_SCHEDULE.PR_SCHEDULE_TEMPLATE_MAP_LIST", param);
        }

    }
}
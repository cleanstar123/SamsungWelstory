using System.Collections.Generic;
using System.Data;

using CMS.API.Models;
using CMS.API.App_Code;
using Oracle.ManagedDataAccess.Client;

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
            OracleParameter[] param = {
                                          new OracleParameter("P_RESTAURANT_CODE", scheduleModel.RESTAURANT_CODE),
                                          new OracleParameter("P_SCHEDULE_ID",     scheduleModel.SCHEDULE_ID),
                                          new OracleParameter("CUR",               OracleDbType.RefCursor),
                                          new OracleParameter("CUR_DISP",          OracleDbType.RefCursor),
                                          new OracleParameter("CUR_TEMPLATE",      OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;
            param[param.Length - 2].Direction = ParameterDirection.Output;
            param[param.Length - 3].Direction = ParameterDirection.Output;

            return OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_SCHEDULE.PR_SCHEDULE_ALL_LIST", param);
        }

        /// <summary>
        /// 스케줄 조회 (다중)
        /// </summary>
        /// <param name="scheduleModel"></param>
        /// <returns></returns>
        public static List<ScheduleResultModel> getSchedules(ScheduleModel scheduleModel)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_RESTAURANT_CODE", scheduleModel.RESTAURANT_CODE),
                                          new OracleParameter("P_START_DATE",      scheduleModel.START_DATE),
                                          new OracleParameter("P_END_DATE",        scheduleModel.END_DATE),
                                          new OracleParameter("P_DISPLAY_ID",      scheduleModel.DISPLAY_ID),
                                          new OracleParameter("CUR",               OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ScheduleResultModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_SCHEDULE.PR_SCHEDULE_LIST_CAL", param).Tables[0]);

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
            OracleParameter[] param = {
                                          new OracleParameter("P_TYPE",            type),
                                          new OracleParameter("P_RESTAURANT_CODE", restaurantCode),
                                          new OracleParameter("P_SCHEDULE_ID",     scheduleModels[0].SCHEDULE_ID),
                                          new OracleParameter("P_REG_ID",          userId),
                                          new OracleParameter("P_XML_REQ",         JsonHelper.GetJsonToXmlString(scheduleModels)),
                                          new OracleParameter("P_XML_REQ_DISP",    JsonHelper.GetJsonToXmlString<DisplayModel>(displayModels)),
                                          new OracleParameter("P_XML_REQ_MAP",     JsonHelper.GetJsonToXmlString<ScheduleTemplateMapModel>(scheduleTemplateMapModels)),
                                          new OracleParameter("CUR",               OracleDbType.RefCursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_SCHEDULE.PR_SCHEDULE_MANAGE", param).Tables[0])[0];
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
            OracleParameter[] param = {
                                          new OracleParameter("P_TYPE",            type),
                                          new OracleParameter("P_RESTAURANT_CODE", restaurantCode),
                                          new OracleParameter("P_SCHEDULE_ID",     scheduleModels[0].SCHEDULE_ID),
                                          new OracleParameter("P_REG_ID",          userId),
                                          new OracleParameter("P_XML_REQ",         JsonHelper.GetJsonToXmlString(scheduleModels)),
                                          new OracleParameter("P_XML_REQ_DISP",    JsonHelper.GetJsonToXmlString<DisplayModel>(displayModels)),
                                          new OracleParameter("CUR",               OracleDbType.RefCursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_SCHEDULE.PR_SCHEDULE_OVERLAP_CHECK", param).Tables[0])[0];
        }

        /// <summary>
        /// 롤링 스케줄에 맵핑된 템플릿 정보 조회
        /// </summary>
        /// <param name="RESTAURANT_CODE"></param>
        /// <param name="SCHEDULE_ID"></param>
        /// <returns></returns>
        public static DataSet getScheduleTemplateMapList(string RESTAURANT_CODE, string SCHEDULE_ID)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_RESTAURANT_CODE", RESTAURANT_CODE),
                                          new OracleParameter("P_SCHEDULE_ID",     SCHEDULE_ID),
                                          new OracleParameter("CUR",               OracleDbType.RefCursor),
                                          new OracleParameter("CUR_TOT",           OracleDbType.RefCursor),
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;
            param[param.Length - 2].Direction = ParameterDirection.Output;

            return OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_SCHEDULE.PR_SCHEDULE_TEMPLATE_MAP_LIST", param);
        }

    }
}
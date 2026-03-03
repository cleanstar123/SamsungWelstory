using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using CMS.API.Models;
using CMS.API.App_Code;
using Oracle.ManagedDataAccess.Client;
using System.Diagnostics;

namespace CMS.API.Biz
{
    public class SensorBiz
    {
        /// <summary>
        /// 현재 연결된 센서 유닛의 정보를 return 한다
        /// </summary>
        /// <param name="areaModel"></param>
        /// <returns></returns>
        public static object getSensorList(AreaModel areaModel)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_RESTAURANT_CODE", areaModel.RESTAURANT_CODE),
                                          new OracleParameter("P_AREA_NM", areaModel.AREA_NM),
                                          new OracleParameter("CUR", OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<SensorModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_SENSOR.PR_SENSOR_INFO_SELECT", param).Tables[0]);
        }

        /// <summary>
        /// 현재 영역 유닛의 정보를 return 한다
        /// </summary>
        /// <param name="areaModel"></param>
        /// <returns></returns>
        public static object getAreaList(AreaModel areaModel)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_RESTAURANT_CODE", areaModel.RESTAURANT_CODE),
                                          new OracleParameter("P_AREA_NM", areaModel.AREA_NM),
                                          new OracleParameter("CUR", OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<AreaModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_SENSOR.PR_AREA_INFO_SELECT", param).Tables[0]);
        }

        /// <summary>
        /// 현재 매핑 정보를 return 한다
        /// </summary>
        /// <param name="sensorAreaModel"></param>
        /// <returns></returns>
        public static object getSensorAreaList(SensorAreaModel sensorAreaModel)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_RESTAURANT_CODE", sensorAreaModel.RESTAURANT_CODE),
                                          new OracleParameter("P_AREA_CD", sensorAreaModel.AREA_CD),
                                          new OracleParameter("CUR", OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<SensorAreaModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_SENSOR.PR_SENSOR_AREA_MAPPING_SELECT", param).Tables[0]);
        }
       

        /// <summary>
        /// 센서 저장/수정/삭제
        /// </summary>
        /// <param name="type"></param>
        /// <param name="restaurantCode"></param>
        /// <param name="sensorModels"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ResultModel sensorManage(string type, string restaurantCode, List<SensorModel> sensorModels, string userId)
        {
            SensorModel model = (type.ToUpper() != "D" ? sensorModels[0] : new SensorModel());

            OracleParameter[] param = {
                                          new OracleParameter("P_TYPE",            type),
                                          new OracleParameter("P_RESTAURANT_CODE", restaurantCode),
                                          new OracleParameter("P_SERIAL_NO",       model.SERIAL_NO),
                                          new OracleParameter("P_USE_YN",          model.USE_YN),
                                          new OracleParameter("P_REG_ID",          userId),
                                          new OracleParameter("P_XML_REQ",         type == "D" ? JsonHelper.GetJsonToXmlString<SensorModel>(sensorModels) : null),
                                          new OracleParameter("CUR",               OracleDbType.RefCursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_SENSOR.PR_SENSOR_MANAGE", param).Tables[0])[0];
        }

        /// <summary>
        /// 센서 영역 저장/수정/삭제
        /// </summary>
        /// <param name="type"></param>
        /// <param name="restaurantCode"></param>
        /// <param name="areaModels"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ResultModel areaManage(string type, string restaurantCode, List<AreaModel> areaModels, string userId)
        {
            AreaModel model = areaModels[0];

            OracleParameter[] param = {
                                          new OracleParameter("P_TYPE",            type),
                                          new OracleParameter("P_RESTAURANT_CODE", restaurantCode),
                                          new OracleParameter("P_AREA_CD",         model.AREA_CD),
                                          new OracleParameter("P_AREA_NM",         model.AREA_NM),
                                          //new OracleParameter("P_NORMAL",        model.NORMAL),
                                          new OracleParameter("P_UNCROWDED",       model.UNCROWDED),
                                          new OracleParameter("P_CROWDED",         model.CROWDED),
                                          new OracleParameter("P_REG_ID",          userId),
                                          new OracleParameter("P_XML_REQ",         type == "D" ? JsonHelper.GetJsonToXmlString<AreaModel>(areaModels) : null),
                                          new OracleParameter("CUR",               OracleDbType.RefCursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_SENSOR.PR_AREA_MANAGE", param).Tables[0])[0];
        }

        /// <summary>
        /// 센서 영역 저장/수정/삭제
        /// </summary>
        /// <param name="type"></param>
        /// <param name="restaurantCode"></param>
        /// <param name="sensorAreaModels"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ResultModel sensorAreaManage(string type, string restaurantCode, List<SensorAreaModel> sensorAreaModels, string userId)
        {
            SensorAreaModel model = sensorAreaModels[0];

            OracleParameter[] param = {
                                          new OracleParameter("P_TYPE",            type),
                                          new OracleParameter("P_RESTAURANT_CODE", restaurantCode),/*
                                          new OracleParameter("P_AREA_CD",         model.AREA_CD),
                                          new OracleParameter("P_SERIAL_NO",       model.SERIAL_NO),*/
                                          new OracleParameter("P_REG_ID",          userId),
                                          new OracleParameter("P_XML_REQ",         JsonHelper.GetJsonToXmlString<SensorAreaModel>(sensorAreaModels)),
                                          new OracleParameter("CUR",               OracleDbType.RefCursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;
            Debug.WriteLine(JsonHelper.GetJsonToXmlString<SensorAreaModel>(sensorAreaModels));
            return Util.ConvertDataTable<ResultModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_SENSOR.PR_SENSOR_AREA_UPDATE", param).Tables[0])[0];
        }
    }
}
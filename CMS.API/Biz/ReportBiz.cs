using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CMS.API.App_Code;
using CMS.API.Models;
using Npgsql;
using NpgsqlTypes;

namespace CMS.API.Biz
{
    public class ReportBiz
    {
        /// <summary>
        /// 리포트
        /// </summary>
        /// <param name="areaModel"></param>
        /// <returns></returns>
        public static DataSet CongestionSection(CongestionTermModel congestionTermModel)
        {
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_RESTAURANT_CODE", congestionTermModel.RESTAURANT_CODE),
                                          new NpgsqlParameter("P_S_DATE", congestionTermModel.S_DATE),
                                          new NpgsqlParameter("P_E_DATE", congestionTermModel.E_DATE),
                                          new NpgsqlParameter("P_INTERVAL", congestionTermModel.INTERVAL),
                                          new NpgsqlParameter("CUR_COL", NpgsqlDbType.Refcursor),
                                          new NpgsqlParameter("CUR", NpgsqlDbType.Refcursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;
            param[param.Length - 2].Direction = ParameterDirection.Output;
            DataSet ds = new DataSet();
            ds = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_SENSOR.PR_CONGESTION_REPORT", param);
            return ds;
        }
    }
}
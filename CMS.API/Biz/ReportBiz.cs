using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CMS.API.App_Code;
using CMS.API.Models;
using Oracle.ManagedDataAccess.Client;

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
            OracleParameter[] param = {
                                          new OracleParameter("P_RESTAURANT_CODE", congestionTermModel.RESTAURANT_CODE),
                                          new OracleParameter("P_S_DATE", congestionTermModel.S_DATE),
                                          new OracleParameter("P_E_DATE", congestionTermModel.E_DATE),
                                          new OracleParameter("P_INTERVAL", congestionTermModel.INTERVAL),
                                          new OracleParameter("CUR_COL", OracleDbType.RefCursor),
                                          new OracleParameter("CUR", OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;
            param[param.Length - 2].Direction = ParameterDirection.Output;
            DataSet ds = new DataSet();
            ds = OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_SENSOR.PR_CONGESTION_REPORT", param);
            return ds;
        }
    }
}
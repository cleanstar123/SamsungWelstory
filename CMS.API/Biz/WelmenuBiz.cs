using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using CMS.API.Models;
using CMS.API.App_Code;
using Npgsql;
using NpgsqlTypes;


namespace CMS.API.Biz
{
    public class WelmenuBiz
    {

        public static List<WelmenuModel> welmenuList(WelmenuModel welmenuModel)
        {
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_RESTAURANT_CODE", welmenuModel.RESTAURANT_CODE),
                                          new NpgsqlParameter("P_MENU_DT",         welmenuModel.MENU_DT),
                                          new NpgsqlParameter("P_MENU_MEAL_TYPE",  welmenuModel.MEAL_TYPE),
                                          new NpgsqlParameter("CUR", NpgsqlDbType.Refcursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<WelmenuModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_WELMENU.PR_WELMENU_LIST", param).Tables[0]);
        }

        public static DataSet getSatisfactionlist(List<WelmenuModel> welmenuModels)
        {
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_XML_REQ", JsonHelper.GetJsonToXmlString<WelmenuModel>(welmenuModels)),
                                          new NpgsqlParameter("CUR",       NpgsqlDbType.Refcursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_WELMENU.PR_WELMENU_EVAL_RESULT", param);
        }

        public static DataSet getSatisfactionlist2(List<WelmenuModel> welmenuModels)
        {
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_XML_REQ", JsonHelper.GetJsonToXmlString<WelmenuModel>(welmenuModels)),
                                          new NpgsqlParameter("CUR",       NpgsqlDbType.Refcursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_WELMENU.PR_WELMENU_EVAL_RESULT_VIEW ", param);
        }

        public static ResultModel setSatisfaction(WelmenuModel welmenuModel)
        {
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_RESTAURANT_CODE", welmenuModel.RESTAURANT_CODE),
                                          new NpgsqlParameter("P_HALL_NO",         welmenuModel.HALL_NO),
                                          new NpgsqlParameter("P_MENU_DT",         welmenuModel.MENU_DT),
                                          new NpgsqlParameter("P_MEAL_TYPE",       welmenuModel.MEAL_TYPE),
                                          new NpgsqlParameter("P_COURSE_TYPE",     welmenuModel.COURSE_TYPE),
                                          new NpgsqlParameter("P_MENU_CODE",       welmenuModel.MENU_CODE),
                                          new NpgsqlParameter("P_EVAL_SCORE",      welmenuModel.EVAL_SCORE),
                                          new NpgsqlParameter("P_REG_ID",          "DID"),
                                          new NpgsqlParameter("CUR", NpgsqlDbType.Refcursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;

            DataSet ds = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_WELMENU.PR_WELMENU_EVAL_IN_KIOSK", param);

            ResultModel result = new ResultModel();

            if (Util.IsNullDataset(ds))
            {
                result.ERR_CODE = ds.Tables[0].Rows[0]["ERR_CODE"].ToString();
                result.ERROR_MSG = ds.Tables[0].Rows[0]["ERROR_MSG"].ToString();
            }

            return result;
        }

    }
}
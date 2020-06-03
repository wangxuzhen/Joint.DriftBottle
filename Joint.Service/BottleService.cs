using Joint.Common;
using Joint.DLLFactory;
using Joint.Entity;
using Joint.IRepository;
using Joint.IService;
using Joint.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace Joint.Service
{
    public partial class BottleService : BaseService<Bottle>, IBottleService
    {
        public DataTable GetBottleListByRd(int userID, DateTime beginTime)
        {


            List<SqlParameter> listSqlParameter = new List<SqlParameter>();
            listSqlParameter.Add(new SqlParameter { ParameterName = "userID", Value = userID });
            listSqlParameter.Add(new SqlParameter { ParameterName = "beginTime", Value = beginTime.ToString("yyyy-MM-dd HH:mm:ss") });

            //string sql = @"SELECT TOP 10 * FROM [dbo].[Bottle] where CreateTime>@beginTime ORDER BY NEWID()";

            string sql = @"SELECT TOP 10 * FROM [dbo].[Bottle] where FirstReplyUserID is null and CreateUserID<>@userID and CreateTime>=@beginTime ORDER BY NEWID()";

            IDbSession dbSession = DbSessionFactory.GetCurrentDbSession();
            DataTable dtData = dbSession.SqlQueryForDataSet(sql, listSqlParameter.ToArray()).Tables[0];
            return dtData;
        }

        public List<int> GetNoReadCount(int userID)
        {
            //我捞起来的瓶子，有回复的条数
            string fishingNoReadSql = @"SELECT count(1) FROM [JointDriftBottleDB].[dbo].[Bottle] where FirstReplyUserID=" + userID + " and LastReplyUserID=CreateUserID And [ReplyViewTime]<[LastReplyTime]";
            //我丢出去的瓶子有回复的条数
            string throwNoReadSql = @"SELECT count(1) FROM [JointDriftBottleDB].[dbo].[Bottle] where CreateUserID=" + userID + " and LastReplyUserID<>CreateUserID And CreateViewTime<[LastReplyTime]";
            IDbSession dbSession = DbSessionFactory.GetCurrentDbSession();
            DataTable dtData1 = dbSession.SqlQueryForDataSet(fishingNoReadSql, null).Tables[0];
            DataTable dtData2 = dbSession.SqlQueryForDataSet(throwNoReadSql, null).Tables[0];
            List<int> countResult = new List<int>();
            countResult.Add(Convert.ToInt32(dtData1.Rows[0][0]));
            countResult.Add(Convert.ToInt32(dtData2.Rows[0][0]));
            return countResult;
        }
    }
}

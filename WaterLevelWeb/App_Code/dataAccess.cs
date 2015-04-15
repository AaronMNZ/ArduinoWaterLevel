using DotNet.Highcharts.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace WaterLevelWeb
{
    public class graphPoint
    {
        public string WaterLevel { get; set; }
        public string Temp { get; set; }
        public string LogTime { get; set; }
    }

    public class dataAccess
    {
        public enum QueryGroupBy
        {
            Month, Week, Day, Hour, Minute
        }


        private static SQLiteConnection sqlConnection()
        {
            string dir = HostingEnvironment.MapPath("\\App_Data\\");
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=" + dir + "history.db;Version=3;");
            return m_dbConnection;
        }

        public static int insertData(Decimal Level, Decimal Temp)
        {
            int result = 0;
            using (SQLiteConnection dbcon = sqlConnection())
            {
                decimal levelDB = Convert.ToDecimal(Level);
                decimal tempDB = Convert.ToDecimal(Temp);

                string query = "INSERT INTO history (level, temp, timestamp) VALUES (@level, @temp, @timestamp)";

                dbcon.Open();
                SQLiteCommand InsertCommand = new SQLiteCommand(query, dbcon);
                InsertCommand.Parameters.AddWithValue("level", levelDB);
                InsertCommand.Parameters.AddWithValue("temp", tempDB);
                InsertCommand.Parameters.AddWithValue("timestamp", String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.UtcNow));
                result = InsertCommand.ExecuteNonQuery();
                dbcon.Close();
            }
            return result;
        }


        public static List<graphPoint> getData(dataAccess.QueryGroupBy groupBy)
        {
            List<graphPoint> results = new List<graphPoint>();

            using (SQLiteConnection dbcon = sqlConnection())
            {
                using (SQLiteCommand command = dbcon.CreateCommand())
                {
                    dbcon.Open();

                    switch (groupBy)
                    {
                        case QueryGroupBy.Month:
                            command.CommandText = @"SELECT AVG([level]) AS level, AVG([temp]) AS temp, strftime('%m', timestamp) AS [time]
                                           FROM [history] 
                                           WHERE timestamp > datetime('now','-12 month')
                                           GROUP BY strftime('%m', timestamp) 
                                           ORDER BY [timestamp] ASC";
                            break;
                        case QueryGroupBy.Week:
                            command.CommandText = @"SELECT AVG([level]) AS level, AVG([temp]) AS temp, strftime('%W', timestamp) AS [time]
                                           FROM [history] 
                                           WHERE timestamp > datetime('now','-365 day')
                                           GROUP BY strftime('%W', timestamp) 
                                           ORDER BY [timestamp] ASC";
                            break;
                        case QueryGroupBy.Day:
                            command.CommandText = @"SELECT AVG([level]) AS level, AVG([temp]) AS temp, strftime('%d', timestamp) AS [time]
                                           FROM [history] 
                                           WHERE timestamp > datetime('now','-7 day')
                                           GROUP BY strftime('%d', timestamp) 
                                           ORDER BY [timestamp] ASC";
                            break;
                        case QueryGroupBy.Hour:
                            command.CommandText = @"SELECT AVG([level]) AS level, AVG([temp]) AS temp, strftime('%H', timestamp) AS [time]
                                           FROM [history] 
                                           WHERE timestamp > datetime('now','-24 hour')
                                           GROUP BY strftime('%H', timestamp) 
                                           ORDER BY [timestamp] ASC";
                            break;
                        case QueryGroupBy.Minute:
                            command.CommandText = @"SELECT AVG([level]) AS level, AVG([temp]) AS temp, strftime('%M', timestamp) AS [time]
                                           FROM [history] 
                                           WHERE timestamp > datetime('now','-360 minute')
                                           GROUP BY strftime('%M', timestamp) 
                                           ORDER BY [timestamp] ASC";
                            break;
                        default:
                            break;
                    }


                    SQLiteDataReader sdr = command.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            results.Add(new graphPoint()
                            {
                                WaterLevel = sdr.GetValue(0).ToString(),
                                Temp = sdr.GetValue(1).ToString(),
                                LogTime = sdr.GetValue(2).ToString()
                            });
                        }
                    }
                    dbcon.Close();
                }
            }

            return results;
        }


    }
}
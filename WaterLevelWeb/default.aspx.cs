using DotNet.Highcharts;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WaterLevelWeb
{
    public partial class _default : System.Web.UI.Page
    {

        //Had to create SQLite DB first then attach to project
        //Had to 'show hidden files' then 'include in project' & 'Copy to output:Always" for bin\x64\SQLite.Interop.dll &  bin\x86\SQLite.Interop.dll to allow it to work on Azure web apps.

        //Install-Package System.Data.SQLite
        //Install-Package DotNet.Highcharts
        //Install-Package jQuery

        private const int tankHeight = 2300;
        private const int sensorHeight = 110;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                renderGraph(dataAccess.QueryGroupBy.Day);
            }
        }

        private void renderGraph(dataAccess.QueryGroupBy groupBy)
        {
            List<graphPoint> graphPoints = dataAccess.getData(groupBy);

            Object[] chartLevel = new Object[graphPoints.Count];
            Object[] chartTemp = new Object[graphPoints.Count];
            int i = 0;

            foreach (graphPoint point in graphPoints)
            {
                chartLevel[i] = (100 * ((tankHeight - 1) - (Convert.ToDecimal(point.WaterLevel) - sensorHeight)) / tankHeight).ToString();
                chartTemp[i] = point.Temp;
                i++;
            }

            string levelString = string.Join(",", graphPoints.Select(x => x.WaterLevel).ToArray());

            Highcharts chart = new DotNet.Highcharts.Highcharts("chart")
                .SetXAxis(new XAxis
                {
                    Categories = graphPoints.Select(x => x.LogTime).ToArray()
                })
                .SetSeries(new[]
                {
                    new Series { Name = "Level", Data = new Data(chartLevel) },
                    new Series { Name = "Temp", Data = new Data(chartTemp) },
                });

            DotNet.Highcharts.Options.Title title = new DotNet.Highcharts.Options.Title();
            title.Text = "Level Meter";
            chart.SetTitle(title);

            DotNet.Highcharts.Options.Chart cht = new DotNet.Highcharts.Options.Chart();

            cht.Height = 600;
            cht.Type = DotNet.Highcharts.Enums.ChartTypes.Area;

            chart.InitChart(cht);
            ltrChart.Text = chart.ToHtmlString();
        }


        protected void groupBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (groupBy.Text)
            {
                case "Minute":
                    renderGraph(dataAccess.QueryGroupBy.Minute);
                    break;

                case "Day":
                    renderGraph(dataAccess.QueryGroupBy.Day);
                    break;

                case "Hour":
                    renderGraph(dataAccess.QueryGroupBy.Hour);
                    break;

                case "Month":
                    renderGraph(dataAccess.QueryGroupBy.Month);
                    break;

                case "Week":
                    renderGraph(dataAccess.QueryGroupBy.Week);
                    break;
            }
        }


    }
}
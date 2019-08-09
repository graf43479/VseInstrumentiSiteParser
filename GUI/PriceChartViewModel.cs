// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="OxyPlot">
//   The MIT License (MIT)
//   
//   Copyright (c) 2014 OxyPlot contributors
//   
//   Permission is hereby granted, free of charge, to any person obtaining a
//   copy of this software and associated documentation files (the
//   "Software"), to deal in the Software without restriction, including
//   without limitation the rights to use, copy, modify, merge, publish,
//   distribute, sublicense, and/or sell copies of the Software, and to
//   permit persons to whom the Software is furnished to do so, subject to
//   the following conditions:
//   
//   The above copyright notice and this permission notice shall be included
//   in all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
//   OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//   MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//   IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//   CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//   TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
//   SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;


namespace GUI
{
    public class PriceChartViewModel
    {
        public PriceChartViewModel()
        {
            //this.Title = "Date/Prices";
            //this.Points = new List<DataPoint> {
            //                        new DataPoint(0, 4),
            //                      new DataPoint(10, 13),
            //                      new DataPoint(20, 15),
            //                      new DataPoint(30, 16),
            //                      new DataPoint(40, 12),
            //                      new DataPoint(50, 12)
            //};

            MyModel = new PlotModel { Title = "Товар не выбран" };
            /*
            var startDate = DateTime.Now.Date.AddDays(-10);
            var endDate = DateTime.Now.Date;

            var minValue = DateTimeAxis.ToDouble(startDate);
            var maxValue = DateTimeAxis.ToDouble(endDate);

            MyModel.Axes.Add(new DateTimeAxis {Position = AxisPosition.Bottom, Minimum = minValue, Maximum = maxValue, StringFormat = "dd-MM-yyyy", MinimumMinorStep=1 });
            //MyModel.Series.Add(new FunctionSeries())
            var areaSeries = new AreaSeries();
            Points = new List<DataPoint>();
            for (int i = 5; i > 0; i--)
            {
                //MyModel.Series.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.Date.AddDays(-5)), (double)i));
                DataPoint dp = new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.Date.AddDays(-i)), (double)i);
                //Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.Date.AddDays(-5)), (double)i));
                this.Points.Add(dp);
                areaSeries.Points.Add(dp);
                // Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(-i)), (double)i));
                //MyModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minValue, Maximum = maxValue, StringFormat = "dd" });
            }


            //areaSeries.Points.Add(Points);
            MyModel.Series.Add(areaSeries);

            //MyModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minValue, Maximum = maxValue, StringFormat = "dd" });
            */
        }
        

        public string Title { get; private set; }

        //public PlotModel MyModel { get; private set; }
        public PlotModel MyModel { get; private set; }
        public IList<DataPoint> Points { get; private set; }
    }
}

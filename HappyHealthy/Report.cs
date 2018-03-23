using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using Java.Text;
using OxyPlot.Xamarin.Android;
using System.Data;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Annotations;
using SQLite;

namespace HappyHealthyCSharp
{
    [Activity(Theme = "@style/MyMaterialTheme.Base", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    class Report : Activity
    {
        TextView reportStatus;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.activity_report);
            reportStatus = FindViewById<TextView>(Resource.Id.reportStatus);
            PlotView view = FindViewById<PlotView>(Resource.Id.plot_view);
            var mainSpinner = FindViewById<Spinner>(Resource.Id.spinner);
            var mainAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.report_label, Android.Resource.Layout.SimpleSpinnerItem);
            mainSpinner.Adapter = mainAdapter;
            var secondSpinner = FindViewById<Spinner>(Resource.Id.spinnerField);
            ArrayAdapter secondAdapter;
            mainSpinner.ItemSelected += delegate
            {
                if (mainSpinner.SelectedItemPosition == 0)
                {
                    secondAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.diabetes, Android.Resource.Layout.SimpleSpinnerItem);
                    secondSpinner.Adapter = secondAdapter;
                }
                else if (mainSpinner.SelectedItemPosition == 1)
                {
                    secondAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.pressure, Android.Resource.Layout.SimpleSpinnerItem);
                    secondSpinner.Adapter = secondAdapter;
                }
                else if (mainSpinner.SelectedItemPosition == 2)
                {
                    secondAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.kidney, Android.Resource.Layout.SimpleSpinnerItem);
                    secondSpinner.Adapter = secondAdapter;
                }
            };
            secondSpinner.ItemSelected += delegate
            {
                if (mainSpinner.SelectedItemPosition == 0)
                {
                    string key = "fbs_fbs";
                    if (secondSpinner.SelectedItemPosition == 0)
                        key = "fbs_fbs";
                    else if (secondSpinner.SelectedItemPosition == 1)
                        key = "fbs_fbs_sum";
                    view.Model = CreatePlotModel(
                        "รายงานค่าเบาหวาน",
                        new DiabetesTABLE().GetJavaList<DiabetesTABLE>($"SELECT * FROM DiabetesTABLE WHERE UD_ID = {Extension.getPreference("ud_id", 0, this)} ORDER BY FBS_TIME",
                        new DiabetesTABLE().Column),
                        "fbs_time",
                        key,
                        key == "fbs_fbs"?DiabetesTABLE.caseLevel.High:7);
                }
                else if (mainSpinner.SelectedItemPosition == 1)
                {
                    string key = "bp_up";
                    if (secondSpinner.SelectedItemPosition == 0)
                        key = "bp_up";
                    else if (secondSpinner.SelectedItemPosition == 1)
                        key = "bp_lo";
                    else
                        key = "bp_hr";
                    view.Model = CreatePlotModel(
                        "รายงานค่าความดัน",
                        new PressureTABLE().GetJavaList<PressureTABLE>($@"SELECT * FROM PressureTABLE WHERE UD_ID = {Extension.getPreference("ud_id", 0, this)} ORDER BY BP_TIME",
                        new PressureTABLE().Column),
                        "bp_time",
                        key,
                        PressureTABLE.caseLevel.uHigh);
                }
                else if (mainSpinner.SelectedItemPosition == 2)
                {
                    string key = "ckd_gfr";
                    if (secondSpinner.SelectedItemPosition == 0)
                        key = "ckd_gfr";
                    else if (secondSpinner.SelectedItemPosition == 1)
                        key = "ckd_creatinine";
                    else if (secondSpinner.SelectedItemPosition == 2)
                        key = "ckd_bun";
                    else if (secondSpinner.SelectedItemPosition == 3)
                        key = "ckd_sodium";
                    else if (secondSpinner.SelectedItemPosition == 4)
                        key = "ckd_potassium";
                    else if (secondSpinner.SelectedItemPosition == 5)
                        key = "ckd_phosphorus_blood";
                    else if (secondSpinner.SelectedItemPosition == 6)
                        key = "ckd_albumin_blood";
                    else if (secondSpinner.SelectedItemPosition == 7)
                        key = "ckd_albumin_urine";
                    view.Model = CreatePlotModel(
                        "รายงานค่าโรคไต",
                        new KidneyTABLE().GetJavaList<KidneyTABLE>($@"SELECT * FROM KidneyTABLE WHERE UD_ID = {Extension.getPreference("ud_id", 0, this)} ORDER BY CKD_TIME",
                        new KidneyTABLE().Column),
                        "ckd_time",
                        key,
                        KidneyTABLE.caseLevel.High);
                }
            };
            ViewFirstInit(view);
        }

        private void ViewFirstInit(PlotView v)
        {
            v.Model = CreatePlotModel(
    "รายงานค่าเบาหวาน",
    new DiabetesTABLE().GetJavaList<DiabetesTABLE>($"SELECT * FROM DiabetesTABLE WHERE UD_ID = {Extension.getPreference("ud_id", 0, this)} ORDER BY FBS_TIME",
    new DiabetesTABLE().Column),
    "fbs_time",
    "fbs_fbs",
    DiabetesTABLE.caseLevel.High);
        }

        private PlotModel CreatePlotModel(string title, JavaList<IDictionary<string, object>> dataset, string key_time, string key_value, int exceedValue = 150)
        {
            var size = Resources.GetDimension(Resource.Dimension.text_size);
            var datalength = dataset.Count();
            var plotModel = new PlotModel
            {
                Title = title,
                TitleFontSize = Resources.GetDimension(Resource.Dimension.text_size)
            };
            object LastDateOnDataset = DateTime.Now;
            var maxValue = 0.0;
            var minValue = 0.0;
            if (datalength > 0)
            {
                dataset.Last().TryGetValue(key_time, out LastDateOnDataset);
            }
            var startDate = DateTime.Parse(LastDateOnDataset.ToString()).AddDays(-15);
            var endDate = DateTime.Parse(LastDateOnDataset.ToString()).AddDays(5);
            var minDate = DateTimeAxis.ToDouble(startDate);
            var maxDate = DateTimeAxis.ToDouble(endDate);
            var x = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = minDate,
                Maximum = maxDate,
                MajorStep = 10,
                StringFormat = "d-MMM",
                FontSize = size
            };
            var y = new LinearAxis
            {
                Position = AxisPosition.Left,
                Maximum = maxValue,
                Minimum = minValue,
                FontSize = size
            };
            y.IsPanEnabled = false;
            y.IsZoomEnabled = false;
            plotModel.Axes.Add(x);
            plotModel.Axes.Add(y);
            var dataSeries = new AreaSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerStroke = OxyColors.White,
                StrokeThickness = 10,
                Color = OxyColors.Green,
                MarkerFill = OxyColors.Red,
                Fill = OxyColors.LightGreen,
            };
            for (var i = 0; i < datalength; i++)
            {
                dataset[i].TryGetValue(key_time, out object Time);
                dataset[i].TryGetValue(key_value, out object Value);
                var dCandidateValue = Convert.ToDouble(Value);
                if (dCandidateValue > maxValue) //determine new max-min for each row
                    maxValue = dCandidateValue;
                if (dCandidateValue < minValue)
                    minValue = dCandidateValue;
                DateTime.TryParse(Time.ToString(), out DateTime dateResult);
                double value = Convert.ToDouble(Value.ToString());
                dataSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(dateResult), value));
                dataSeries.Points2.Add(new DataPoint(DateTimeAxis.ToDouble(dateResult), 0)); //to hook the fill area down to the X-axis
                var textAnnotations = new TextAnnotation()
                {
                    TextPosition = new DataPoint(
                        dataSeries.Points.Last().X - 0.5, //make it a little western-north over the datapoint
                        dataSeries.Points.Last().Y + 1.5),
                    Text = value.ToString(),
                    Stroke = OxyColors.Transparent,
                    FontSize = Resources.GetDimension(Resource.Dimension.text_size)
                };

                plotModel.Annotations.Add(textAnnotations);
            }
            #region Conclusion-Initial
            y.Minimum = minValue;
            y.Maximum = maxValue + 5;
            reportStatus.SetTextColor(Android.Graphics.Color.Black);
            reportStatus.Text = "สถานะ : ปกติ"; //base predict cases
            if (maxValue > exceedValue)
            {
                dataSeries.Color = OxyColors.Red;
                dataSeries.Fill = OxyColor.FromArgb((byte)255, (byte)255, (byte)135, (byte)132);
                reportStatus.SetTextColor(Android.Graphics.Color.Red);
                reportStatus.Text = "สถานะ : ผิดปกติ กรุณาพบแพทย์เพื่อรับคำแนะนำเพิ่มเติม";
            }
            #endregion  
            plotModel.Series.Add(dataSeries);
            return plotModel;
        }
        private PlotModel CreatePlotModelLS(string title, JavaList<IDictionary<string, object>> dataset, string key_time, List<string> columns, List<int> exceedValues, List<OxyColor> colors)
        {
            var size = Resources.GetDimension(Resource.Dimension.text_size);
            var datalength = dataset.Count();
            var plotModel = new PlotModel
            {
                Title = title,
                TitleFontSize = Resources.GetDimension(Resource.Dimension.text_size)
            };
            object LastDateOnDataset = DateTime.Now;
            var maxValue = 0.0;
            var minValue = 0.0;
            if (datalength > 0)
            {
                dataset.Last().TryGetValue(key_time, out LastDateOnDataset);
            }
            var startDate = DateTime.Parse(LastDateOnDataset.ToString()).AddDays(-15);
            var endDate = DateTime.Parse(LastDateOnDataset.ToString()).AddDays(5);
            var minDate = DateTimeAxis.ToDouble(startDate);
            var maxDate = DateTimeAxis.ToDouble(endDate);
            var x = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = minDate,
                Maximum = maxDate,
                MajorStep = 10,
                StringFormat = "d-MMM",
                FontSize = size
            };
            var y = new LinearAxis
            {
                Position = AxisPosition.Left,
                Maximum = maxValue,
                Minimum = minValue,
                FontSize = size
            };
            y.IsPanEnabled = false;
            y.IsZoomEnabled = false;
            plotModel.Axes.Add(x);
            plotModel.Axes.Add(y);
            List<AreaSeries> dataSeriesList = new List<AreaSeries>();
            int index = 0;
            foreach (var col in columns)
            {
                if (col == key_time)
                    continue;
                var dataSeries = new AreaSeries
                {
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 4,
                    MarkerStroke = OxyColors.White,
                    StrokeThickness = 10,
                    Color = colors[index],
                    MarkerFill = OxyColors.Red,
                    Fill = OxyColors.LightGreen,
                };
                for (var i = 0; i < datalength; i++)
                {
                    dataset[i].TryGetValue(key_time, out object Time);
                    dataset[i].TryGetValue(col, out object Value);
                    var dCandidateValue = Convert.ToDouble(Value);
                    if (dCandidateValue > maxValue) //determine new max-min for each row
                        maxValue = dCandidateValue;
                    if (dCandidateValue < minValue)
                        minValue = dCandidateValue;
                    DateTime.TryParse(Time.ToString(), out DateTime dateResult);
                    double value = Convert.ToDouble(Value.ToString());
                    dataSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(dateResult), value));
                    //dataSeries.Points2.Add(new DataPoint(DateTimeAxis.ToDouble(dateResult), 0)); //to hook the fill area down to the X-axis
                    var textAnnotations = new TextAnnotation()
                    {
                        TextPosition = new DataPoint(
                            dataSeries.Points.Last().X - 0.5, //make it a little western-north over the datapoint
                            dataSeries.Points.Last().Y + 1.5),
                        Text = value.ToString(),
                        Stroke = OxyColors.Transparent,
                        FontSize = Resources.GetDimension(Resource.Dimension.text_size)
                    };

                    plotModel.Annotations.Add(textAnnotations);
                }
                y.Minimum = minValue;
                y.Maximum = maxValue + 5;
                reportStatus.SetTextColor(Android.Graphics.Color.Black);
                reportStatus.Text = "สถานะ : ปกติ"; //base predict cases
                if (maxValue > exceedValues[index++])
                {
                    dataSeries.Color = OxyColors.Red;
                    dataSeries.Fill = OxyColor.FromArgb((byte)255, (byte)255, (byte)135, (byte)132);
                    reportStatus.SetTextColor(Android.Graphics.Color.Red);
                    reportStatus.Text = "สถานะ : ผิดปกติ กรุณาพบแพทย์เพื่อรับคำแนะนำเพิ่มเติม";
                }
                dataSeriesList.Add(dataSeries);
            };
            foreach (var ds in dataSeriesList)
            {
                plotModel.Series.Add(ds);
            }
            return plotModel;
        }
    }
}
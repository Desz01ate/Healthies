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
using System.Threading.Tasks;

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
            reportStatus.Text = "กำลังประมวลผลข้อมูลของท่าน กรุณารอสักครู่";
            reportStatus.SetTextColor(Android.Graphics.Color.White);
            reportStatus.SetBackgroundColor(Android.Graphics.Color.Black);
            PlotView view = FindViewById<PlotView>(Resource.Id.plot_view);
            var mainSpinner = FindViewById<Spinner>(Resource.Id.spinner);
            var mainAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.report_label, Android.Resource.Layout.SimpleSpinnerItem);
            mainSpinner.Adapter = mainAdapter;
            var secondSpinner = FindViewById<Spinner>(Resource.Id.spinnerField);
            ArrayAdapter secondAdapter;
            var textResource = new[] { Resource.Array.diabetes, Resource.Array.pressure, Resource.Array.kidney };
            mainSpinner.ItemSelected += delegate
            {
                secondAdapter = ArrayAdapter.CreateFromResource(this, textResource[mainSpinner.SelectedItemPosition], Android.Resource.Layout.SimpleSpinnerItem);
                secondSpinner.Adapter = secondAdapter;
            };
            secondSpinner.ItemSelected += delegate
            {
                string[] keys = null;
                double[] minimum = null;
                double[] maximum = null;
                string timeExtractor = string.Empty;
                JavaList<IDictionary<string, object>> data = null;

                if (mainSpinner.SelectedItemPosition == 0)
                {

                    //a1c ref : http://www.yaandyou.net/content-view.php?conid=531
                    keys = DiabetesTABLE.reportKeys;
                    minimum = DiabetesTABLE.reportValuesMinimum;
                    maximum = DiabetesTABLE.reportValuesMaximum;
                    timeExtractor = "fbs_time";
                    data = new DiabetesTABLE().SelectAll(x => x.ud_id == this.GetPreference("ud_id", 0)).OrderBy(x => x.fbs_time).ToJavaList();
                }
                else if (mainSpinner.SelectedItemPosition == 1)
                {
                    keys = PressureTABLE.reportKeys;
                    minimum = PressureTABLE.reportValuesMinimum();
                    maximum = PressureTABLE.reportValuesMaximum();
                    timeExtractor = "bp_time";
                    data = new PressureTABLE().SelectAll(x => x.ud_id == this.GetPreference("ud_id", 0)).OrderBy(x => x.bp_time).ToJavaList();
                    
                }
                else if (mainSpinner.SelectedItemPosition == 2)
                {
                    keys = KidneyTABLE.reportKeys;
                    minimum = KidneyTABLE.reportValuesMinimum;
                    maximum = KidneyTABLE.reportValuesMaximum;
                    timeExtractor = "ckd_time";
                    data = new KidneyTABLE().SelectAll(x => x.ud_id == this.GetPreference("ud_id", 0)).OrderBy(x => x.ckd_time).ToJavaList();
                }
                view.Model = CreatePlotModel(
                    $@"รายงานค่า{secondSpinner.SelectedItem.ToString()}",
                    data,
                    timeExtractor,
                    keys[secondSpinner.SelectedItemPosition],
                    minimum[secondSpinner.SelectedItemPosition],
                    maximum[secondSpinner.SelectedItemPosition]);
            };
            var firstIndex = Intent.GetIntExtra("first", 0); 
            var secondIndex = Intent.GetIntExtra("second", 0);
            mainSpinner.SetSelection(firstIndex);
            secondSpinner.SetSelection(secondIndex);
        }

        private PlotModel CreatePlotModel(string title, JavaList<IDictionary<string, object>> dataset, string key_time, string key_value,double lowestValue, double exceedValue,double exceptValue = 0)
        {
            var size = Resources.GetDimension(Resource.Dimension.text_size);
            var datalength = dataset.Count();
            var plotModel = new PlotModel
            {
                Title = title,
                TitleFontSize = Resources.GetDimension(Resource.Dimension.text_size)
            };
            object LastDateOnDataset = DateTime.Now;
            var maxValue = exceedValue;
            var minValue = lowestValue;
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
                else if (dCandidateValue < minValue)
                    minValue = dCandidateValue;
                DateTime.TryParse(Time.ToString(), out DateTime dateResult);
                double value = Convert.ToDouble(Value.ToString());
                dataSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(dateResult), value));
                dataSeries.Points2.Add(new DataPoint(DateTimeAxis.ToDouble(dateResult), 0)); //to hook the fill area down to the X-axis
                var textAnnotations = new TextAnnotation()
                {
                    TextPosition = new DataPoint(
                        dataSeries.Points.Last().X - 0.5, //make it a little W over the datapoint
                        dataSeries.Points.Last().Y),
                    Text = value.ToString(),
                    Stroke = OxyColors.Transparent,
                    FontSize = Resources.GetDimension(Resource.Dimension.text_size)
                };

                plotModel.Annotations.Add(textAnnotations);
            }
            #region Conclusion-Initial
            y.Minimum = minValue < 0?0:minValue;
            y.Maximum = maxValue + 5;
            reportStatus.SetTextColor(Android.Graphics.Color.Green);
            reportStatus.Text = "สถานะ : ปกติ"; //base predict cases
            //determine(x,y) :  x < Safe-Zone(lowest,exceed) < y , while both x and y is not in except value and the dataset must be larger than 0
            if (((minValue != exceptValue && minValue < lowestValue) || (maxValue != exceptValue && exceedValue < maxValue)) && datalength > 0)
            {
                dataSeries.Color = OxyColors.Red;
                dataSeries.Fill = OxyColor.FromArgb((byte)255, (byte)255, (byte)135, (byte)132);
                reportStatus.SetTextColor(Android.Graphics.Color.Red);
                plotModel.PlotAreaBorderColor = OxyColors.Red;
                reportStatus.Text = "สถานะ : ผิดปกติ กรุณาพบแพทย์เพื่อรับคำแนะนำเพิ่มเติม";
            }
            #endregion  
            plotModel.Series.Add(dataSeries);
            return plotModel;
        }
    }
}
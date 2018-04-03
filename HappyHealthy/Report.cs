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
                double[] lwst = null;
                double[] exc = null;
                string timeExtractor = string.Empty;
                JavaList<IDictionary<string, object>> data = null;
                var user = new UserTABLE().SelectOne(x => x.ud_id == Extension.getPreference("ud_id", 0, this));
                var userAge = DateTime.Now.Year - user.ud_birthdate.Year;
                if (mainSpinner.SelectedItemPosition == 0)
                {

                    //a1c ref : http://www.yaandyou.net/content-view.php?conid=531
                    keys = new[] { "fbs_fbs", "fbs_fbs_sum" };
                    lwst = new[] {      70.0,          5.7  };
                    exc =  new[] {     150.0,            7  };
                    timeExtractor = "fbs_time";
                    data = new DiabetesTABLE().SelectAll(x => x.ud_id == Extension.getPreference("ud_id", 0, this)).OrderBy(x => x.fbs_time).ToJavaList();
                }
                else if (mainSpinner.SelectedItemPosition == 1)
                {

                    //heart rate ref : http://heartratezone.com/what-is-my-pulse-rate-supposed-to-be/
                    keys = new[] { "bp_up", "bp_lo", "bp_hr" };
                    lwst = new[] { 90.0   ,   60.0 ,    -1.0};
                    exc =  new[] { 180.0  ,   110.0,    -1.0};
                    if (userAge < 30)
                    {
                        lwst[2] = 120;
                        exc[2] = 160;
                    }
                    else if (userAge < 40)
                    {
                        lwst[2] = 114;
                        exc[2] = 152;
                    }
                    else if (userAge < 50)
                    {
                        lwst[2] = 108;
                        exc[2] = 144;
                    }
                    else if (userAge < 60)
                    {
                        lwst[2] = 102;
                        exc[2] = 136;
                    }
                    else if (userAge < 70)
                    {
                        lwst[2] = 96;
                        exc[2] = 128;
                    }
                    else
                    {
                        lwst[2] = 90;
                        exc[2] = 120;
                    }
                    timeExtractor = "bp_time";
                    data = new PressureTABLE().SelectAll(x => x.ud_id == Extension.getPreference("ud_id", 0, this)).OrderBy(x => x.bp_time).ToJavaList();
                    
                }
                else if (mainSpinner.SelectedItemPosition == 2)
                {

                    //GFR ref : https://medlineplus.gov/ency/article/007305.htm
                    //Creatinine ref : https://www.medicinenet.com/creatinine_blood_test/article.htm
                    //BUN ref : https://emedicine.medscape.com/article/2073979-overview#a1
                    //sodium ref : https://www.kidney.org/atoz/content/hyponatremia
                    //phosphorus ref : https://www.kidney.org/atoz/content/phosphorus
                    //potassium ref : https://www.davita.com/kidney-disease/diet-and-nutrition/diet-basics/sodium-and-chronic-kidney-disease/e/5310
                    //blood-albumin ref : https://www.davita.com/kidney-disease/diet-and-nutrition/diet-basics/what-is-albumin?/e/5317
                    //urine-albumin ref : https://www.niddk.nih.gov/health-information/professionals/clinical-tools-patient-education-outreach/quick-reference-uacr-gfr
                    keys = new[] { "ckd_gfr", "ckd_creatinine", "ckd_bun", "ckd_sodium", "ckd_potassium", "ckd_phosphorus_blood", "ckd_albumin_blood", "ckd_albumin_urine" };
                    lwst = new[] {        60,             -1.0,         3,         135,             3.5,                    2.5,                 4.0,                    0};
                    exc =  new[] {       120,             -1.0,        20,         145,             5.5,                    4.5,                 999,                   30};
                    if(user.ud_gender == "M")
                    {
                        lwst[1] = 97.0;
                        exc[1] = 137.0;
                    }
                    else
                    {
                        lwst[1] = 88.0;
                        exc[1] = 128.0;
                    }
                    timeExtractor = "ckd_time";
                    data = new KidneyTABLE().SelectAll(x => x.ud_id == Extension.getPreference("ud_id", 0, this)).OrderBy(x => x.ckd_time).ToJavaList();
                }
                view.Model = CreatePlotModel(
                    $@"รายงานค่า{secondSpinner.SelectedItem.ToString()}",
                    data,
                    timeExtractor,
                    keys[secondSpinner.SelectedItemPosition],
                    lwst[secondSpinner.SelectedItemPosition],
                    exc[secondSpinner.SelectedItemPosition]);
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
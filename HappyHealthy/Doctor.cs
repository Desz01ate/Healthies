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
using Java.IO;
using Android.Graphics;
using Android.Provider;
using Newtonsoft.Json;
using Java.Util;
using Android.Icu.Text;
using Android.Views.InputMethods;

namespace HappyHealthyCSharp
{
    [Activity(Label = "Doctor", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.StateHidden)]
    public class Doctor : Activity, ILocalActivity
    {
        public struct App
        {
            public static File _file;
            public static File _dir;
            public static Bitmap bitmap;
        }
        DatePickerDialog docDatePicker;
        Calendar docCarlendar;
        ImageView docAttendPicture;
        TextView docAttendDate;//, docRegisTime, docAppointmentTime;
        EditText et_docName, et_deptName, et_place, et_hospital, et_comment;
        private ImageView saveButton;
        DoctorTABLE docObject;
        private TextView header;
        private ImageView addhiding;
        private ImageView backbtt;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_add_doc);
            var flagObjectJson = Intent.GetStringExtra("targetObject") ?? string.Empty;
            docObject = string.IsNullOrEmpty(flagObjectJson) ? new DoctorTABLE() { da_id = -1 } : JsonConvert.DeserializeObject<DoctorTABLE>(flagObjectJson);
            InitializeControl();
            InitializeControlEvent();
            InitializeData();
            if (IsAppToTakePicturesAvailable())
            {
                CreateDirForPictures();
  
            }

            // Create your application here

        }

        private void InitializeData()
        {
            addhiding.Visibility = ViewStates.Gone;
            header.Text = "บันทึกการนัดพบแพทย์";
            if (docObject.da_id == -1)
                docAttendDate.Text = "คลิกที่นี่เพื่อเลือกวันที่";
            else
                docAttendDate.Text = docObject.da_date.ToString("dd/MM/yyyy");
        }

        private void InitializeControlEvent()
        {
            if (docObject.da_id == -1)
            {
                saveButton.Click += SaveValue;
            }
            else
            {
                InitialForUpdateEvent();
                saveButton.Click += UpdateValue;
            }
            backbtt.Click += delegate
            {
                this.Finish();
            };
            docAttendDate.Click += delegate
            {
                docCarlendar = Calendar.GetInstance(Java.Util.TimeZone.GetTimeZone("GMT+7"));
                docDatePicker = new DatePickerDialog(this, delegate
                {
                    docCarlendar.Set(docDatePicker.DatePicker.Year, docDatePicker.DatePicker.Month, docDatePicker.DatePicker.DayOfMonth);
                    Date date = docCarlendar.Time;
                    var textDate = new SimpleDateFormat("MM-dd-yyyy").Format(date);
                    docObject.da_date = Convert.ToDateTime(textDate).AddDays(1);
                    docAttendDate.Text = Convert.ToDateTime(textDate).ToString("dd/MM/yyyy");
                    //docAttendDate.Text = Convert.ToDateTime(textDate).ToThaiLocale().ToString("dd/MM/yyyy");
                }, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                docDatePicker.Show();
            };
        }

        private void InitializeControl()
        {
            header = FindViewById<TextView>(Resource.Id.textView_header_name_doc);
            addhiding = FindViewById<ImageView>(Resource.Id.imageview_button_add_doc);
            backbtt = FindViewById<ImageView>(Resource.Id.imageViewbackdoc);
            docAttendDate = FindViewById<TextView>(Resource.Id.choosedate_doc);
            et_docName = FindViewById<EditText>(Resource.Id.da_name);
            et_deptName = FindViewById<EditText>(Resource.Id.da_dept);
            et_place = FindViewById<EditText>(Resource.Id.da_place);
            et_hospital = FindViewById<EditText>(Resource.Id.da_hospital);
            et_comment = FindViewById<EditText>(Resource.Id.da_comment);
            saveButton = FindViewById<ImageView>(Resource.Id.imageView_button_save_doc);
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
        private void HideKeyboard()
        {
            InputMethodManager inputManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
            if (CurrentFocus != null)
                inputManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, HideSoftInputFlags.None);
        }
        public void DeleteValue(object sender, EventArgs e)
        {
            Extension.CreateDialogue2(
                 this
                 , "ต้องการลบข้อมูลนี้หรือไม่?"
                 , Android.Graphics.Color.White, Android.Graphics.Color.LightGreen
                 , Android.Graphics.Color.White, Android.Graphics.Color.Red
                 , Extension.adFontSize
                 , delegate
                 {
                     var deleteUri = CalendarHelper.GetDeleteEventURI(docObject.da_calendar_uri);
                     ContentResolver.Delete(deleteUri, null, null);
                     docObject.Delete();
                     Finish();
                 }
                 , delegate { }
                 , "\u2713"
                 , "X");
        }

        public void InitialForUpdateEvent()
        {
            docAttendDate.Text = docObject.da_date.AddDays(-1).ToString("dd/MM/yyyy");
            et_docName.Text = docObject.da_name;
            et_deptName.Text = docObject.da_dept;
            //docRegisTime.Text = docObject.da_reg_time;
            //docAppointmentTime.Text = docObject.da_appt_time;
            et_comment.Text = docObject.da_comment;
            et_place.Text = docObject.da_place;
            et_hospital.Text = docObject.da_hospital;
            //Waiting for image initialize
        }

        public void UpdateValue(object sender, EventArgs e)
        {
            var deleteUri = CalendarHelper.GetDeleteEventURI(docObject.da_calendar_uri);
            ContentResolver.Delete(deleteUri, null, null);
            var year = Convert.ToInt32(docObject.da_date.ToString("yyyy"));
            var month = Convert.ToInt32(docObject.da_date.ToString("MM"));
            var date = Convert.ToInt32(docObject.da_date.ToString("dd"));
            var eventValues = CalendarHelper.GetEventContentValues(4, et_hospital.Text, et_comment.Text, year, month - 1, date - 1, 0, 23);
            System.Console.WriteLine(CalendarContract.Events.ContentUri.ToString() + eventValues.ToString());
            var uri = ContentResolver.Insert(CalendarContract.Events.ContentUri, eventValues);
            docObject.da_calendar_uri = uri.ToString();
            docObject.da_name = et_docName.Text;
            docObject.da_dept = et_deptName.Text;
            //docObject.da_date = DateTime.Parse(docAttendDate.Text).RevertThaiLocale();
            //docObject.da_reg_time = docRegisTime.Text;
            //docObject.da_appt_time = docAppointmentTime.Text;
            docObject.da_comment = et_comment.Text;
            docObject.da_pic = App._file != null ? App._file.AbsolutePath : docObject.da_pic;
            docObject.da_place = et_place.Text;
            docObject.da_hospital = et_hospital.Text;
            docObject.Update();
            Finish();
        }

        public void SaveValue(object sender, EventArgs e)
        {
            if (!Extension.TextFieldValidate(new List<object>() {
                et_docName,et_deptName,et_comment,et_place,et_hospital
            }))
            {
                Toast.MakeText(this, "กรุณากรอกค่าให้ครบ ก่อนทำการบันทึก", ToastLength.Short).Show();
                return;
            }

            var picPath = string.Empty;
            if (App._file != null)
            {
                picPath = App._file.AbsolutePath;
            }
            //pillTable.InsertPillToSQL(medName.Text, medDesc.Text, DateTime.Now,picPath , GlobalFunction.getPreference("ud_id", "", this));
            docObject.da_name = et_docName.Text;
            docObject.da_dept = et_deptName.Text;
            //docObject.da_reg_time = docRegisTime.Text;
            //docObject.da_appt_time = docAppointmentTime.Text;
            docObject.da_comment = et_comment.Text;
            docObject.da_pic = picPath;
            docObject.da_place = et_place.Text;
            docObject.da_hospital = et_hospital.Text;
            docObject.Insert();
            //CustomNotification.SetAlarmManager(this, $"ได้เวลาทานยา {docObject.ma_name}", docObject.ma_set_time, Resource.Raw.notialert);
            //test
            var year = Convert.ToInt32(docObject.da_date.ToString("yyyy"));
            var month = Convert.ToInt32(docObject.da_date.ToString("MM"));
            var date = Convert.ToInt32(docObject.da_date.ToString("dd"));
            var eventValues = CalendarHelper.GetEventContentValues(4, et_hospital.Text, et_comment.Text, year, month - 1, date - 1, 0, 23);
            System.Console.WriteLine(CalendarContract.Events.ContentUri.ToString() + eventValues.ToString());
            var uri = ContentResolver.Insert(CalendarContract.Events.ContentUri, eventValues);
            docObject.da_calendar_uri = uri.ToString();
            docObject.Update();
            //Extension.CreateDialogue(this, $@"Uri for new event: {uri }",delegate { this.Finish(); }).Show();
            //end test
            this.Finish();
        }

        private void cameraClickEvent(object sender, EventArgs e)
        {
            var intent = new Intent(MediaStore.ActionImageCapture);
            //App._file = new File(App._dir, string.Format($@"HappyHealthyCS_{Guid.NewGuid()}.jpg"));
            var filePath = System.IO.Path.Combine(App._dir.AbsolutePath, $@"HappyHealthyCS_{Guid.NewGuid()}.jpg");
            App._file = new File(filePath);
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {

            base.OnActivityResult(requestCode, resultCode, data);
            var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            var contentUri = Android.Net.Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);
            var height = Resources.DisplayMetrics.HeightPixels;
            var width = docAttendPicture.Width;
            App.bitmap = App._file.Path.LoadBitmap(width, height);//LoadBitmap(App._file.Path, width, height);
            if (App.bitmap != null)
            {
                docAttendPicture.SetImageBitmap(App.bitmap);
                App.bitmap = null;
            }
            GC.Collect();
        }

        private void CreateDirForPictures()
        {
            App._dir = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "HappyHealthyCSharp");
            if (!App._dir.Exists())
                App._dir.Mkdirs();
        }
        private bool IsAppToTakePicturesAvailable()
        {
            var intent = new Intent(MediaStore.ActionImageCapture);
            var availableActivities = PackageManager.QueryIntentActivities(intent, Android.Content.PM.PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private long GetDateTimeMS(int yr, int month, int day, int hr, int min)
        {
            Calendar c = Calendar.GetInstance(Java.Util.TimeZone.Default);

            c.Set(Java.Util.CalendarField.DayOfMonth, day - 1);
            c.Set(Java.Util.CalendarField.HourOfDay, hr);
            c.Set(Java.Util.CalendarField.Minute, min);
            c.Set(Java.Util.CalendarField.Month, month - 1);
            c.Set(Java.Util.CalendarField.Year, yr);

            return c.TimeInMillis;
        }

    }
}
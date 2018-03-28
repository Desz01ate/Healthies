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

namespace HappyHealthyCSharp
{
    [Activity(Label = "Pill", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.StateHidden)]
    public class Medicine : Activity, ILocalActivity
    {
        public struct App
        {
            public static File _file;
            public static File _dir;
            public static Bitmap bitmap;
        }
        ImageView medImage, saveButton;
        TextView header;
        private ImageView addhiding;
        private ImageView backbtt;
        EditText medName;
        EditText medDesc;
        MedicineTABLE medObject;
        CheckBox breakfast, lunch, dinner, sleep;
        RadioButton before, after;
        EditText timeText, afterText;
        string filePath;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_add_pill);
            var flagObjectJson = Intent.GetStringExtra("targetObject") ?? string.Empty;
            medObject = string.IsNullOrEmpty(flagObjectJson) ? new MedicineTABLE() { ma_name = string.Empty } : JsonConvert.DeserializeObject<MedicineTABLE>(flagObjectJson);
            InitializeControl();
            InitializeControlEvent();
            InitializeData();
            before.Click += EnableTimeText;
            after.Click += EnableTimeText;
            // Create your application here
            if (IsAppToTakePicturesAvailable())
            {
                CreateDirForPictures();
            }
        }

        private void InitializeControlEvent()
        {
            if (medObject.ma_name == string.Empty)
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
        }

        private void InitializeData()
        {
            timeText.Enabled = false;
            addhiding.Visibility = ViewStates.Gone;
            header.Text = "บันทึกการทานยา";
        }

        private void InitializeControl()
        {
            header = FindViewById<TextView>(Resource.Id.textView_header_name_pill);
            addhiding = FindViewById<ImageView>(Resource.Id.imageViewAddPill);
            backbtt = FindViewById<ImageView>(Resource.Id.imageViewbackpill);
            medName = FindViewById<EditText>(Resource.Id.ma_name);
            medDesc = FindViewById<EditText>(Resource.Id.ma_desc);
            breakfast = FindViewById<CheckBox>(Resource.Id.CheckBox_button_breakfast);
            lunch = FindViewById<CheckBox>(Resource.Id.CheckBox_button_lunch);
            dinner = FindViewById<CheckBox>(Resource.Id.CheckBox_button_dinner);
            sleep = FindViewById<CheckBox>(Resource.Id.CheckBox_button_af_sleep);
            before = FindViewById<RadioButton>(Resource.Id.Radio_button_bf_food);
            after = FindViewById<RadioButton>(Resource.Id.Radio_button_af_food);
            timeText = FindViewById<EditText>(Resource.Id.edit_af_food);
            saveButton = FindViewById<ImageView>(Resource.Id.imageView_button_save_pill);
        }

        private void EnableTimeText(object sender, EventArgs e)
        {
            if (timeText.Enabled == false)
            {
                timeText.Enabled = true;
            }
        }

        public void DeleteValue(object sender, EventArgs e)
        {
            throw new NotImplementedException();
            /*
            Extension.CreateDialogue2(
                 this
                 , "ต้องการลบข้อมูลนี้หรือไม่?"
                 , Android.Graphics.Color.White, Android.Graphics.Color.LightGreen
                 , Android.Graphics.Color.White, Android.Graphics.Color.Red
                 , Extension.adFontSize
                 , delegate
                 {
                     var deleteUri = CalendarHelper.GetDeleteEventURI(medObject.ma_calendar_uri);
                     ContentResolver.Delete(deleteUri, null, null);
                     var time = MedicineTABLE.Morning;
                     CustomNotification.CancelAllAlarmManager(this, medObject.ma_id, medObject.ma_name, time);
                     medObject.Delete<MedicineTABLE>(medObject.ma_id);
                     Finish();
                 }
                 , delegate { }
                 , "\u2713"
                 , "X");
                 */
        }

        public void InitialForUpdateEvent()
        {
            header.Text = $@"บันทึกการทานยา{medObject.ma_name} (สำหรับดู)";
            medName.Text = medObject.ma_name;
            medDesc.Text = medObject.ma_desc;
            breakfast.Checked = medObject.ma_bf;
            lunch.Checked = medObject.ma_lu;
            dinner.Checked = medObject.ma_dn;
            sleep.Checked = medObject.ma_sl;
            if (medObject.ma_before_or_after)
                before.Checked = true;
            else
                after.Checked = true;
            timeText.Text = medObject.ma_before_or_after_minute.ToString();

            //We will not allow this page to be editable on some field
            breakfast.Enabled = false;
            lunch.Enabled = false;
            dinner.Enabled = false;
            sleep.Enabled = false;
            before.Enabled = false;
            after.Enabled = false;
            timeText.Enabled = false;
            //Waiting for image initialize
        }

        public void UpdateValue(object sender, EventArgs e)
        {
            medObject.ma_name = medName.Text;
            medObject.ma_desc = medDesc.Text;
            //medObject.ma_bf = breakfast.Checked;
            //medObject.ma_lu = lunch.Checked;
            //medObject.ma_dn = dinner.Checked;
            //medObject.ma_sl = dinner.Checked;
            //medObject.ma_before_or_after_minute = Convert.ToInt32(timeText.Text);
            //medObject.ma_pic = App._file != null ? App._file.AbsolutePath : medObject.ma_pic;
            //medObject.ud_id = Extension.getPreference("ud_id", 0, this);
            medObject.Update();
            Finish();
        }

        public void SaveValue(object sender, EventArgs e)
        {
            if (!Extension.TextFieldValidate(new List<object>() {
                medName,timeText
            }) || !Extension.CheckBoxValidate(new List<object>() {
                breakfast,lunch,dinner,sleep
            }))
            {
                Toast.MakeText(this, "กรุณากรอกค่าให้ครบ ก่อนทำการบันทึก", ToastLength.Short).Show();
                return;
            }
            var picPath = string.Empty;
            if (App._file != null)
            {
                picPath = filePath;
                //picPath = App._file.AbsolutePath;
            }
            //pillTable.InsertPillToSQL(medName.Text, medDesc.Text, DateTime.Now,picPath , GlobalFunction.getPreference("ud_id", "", this));
            medObject.ma_name = medName.Text;
            medObject.ma_desc = medDesc.Text;
            medObject.ma_pic = picPath;
            medObject.ud_id = Extension.getPreference("ud_id", 0, this);
            medObject.Insert();
            var idHeader = medObject.ma_id.ToString();
            var beforeAfterDecision = Convert.ToInt32(timeText.Text);//before.Checked ? Convert.ToInt32(timeText.Text) : Convert.ToInt32(afterText.Text);
            var user = new UserTABLE().SelectOne(x => x.ud_id == medObject.ud_id);
            DateTime time;
            if (breakfast.Checked)
            {
                time = user.ud_bf_time.AddMinutes(before.Checked ? -beforeAfterDecision : beforeAfterDecision);
                CustomNotification.SetAlarmManager(this, Convert.ToInt32(idHeader + "1"), medObject.ma_name, time);
            }
            if (lunch.Checked)
            {
                time = user.ud_lu_time.AddMinutes(before.Checked ? -beforeAfterDecision : beforeAfterDecision);
                CustomNotification.SetAlarmManager(this, Convert.ToInt32(idHeader + "2"), medObject.ma_name, time);
            }
            if (dinner.Checked)
            {
                time = user.ud_dn_time.AddMinutes(before.Checked ? -beforeAfterDecision : beforeAfterDecision);
                CustomNotification.SetAlarmManager(this, Convert.ToInt32(idHeader + "3"), medObject.ma_name, time);
            }
            if (sleep.Checked)
            {
                time = user.ud_sl_time.AddMinutes(before.Checked ? -beforeAfterDecision : beforeAfterDecision);
                CustomNotification.SetAlarmManager(this, Convert.ToInt32(idHeader + "4"), medObject.ma_name, time);
            }
            medObject.ma_bf = breakfast.Checked;
            medObject.ma_lu = lunch.Checked;
            medObject.ma_dn = dinner.Checked;
            medObject.ma_sl = sleep.Checked;
            medObject.ma_before_or_after = before.Checked ? true : false;
            medObject.ma_before_or_after_minute = beforeAfterDecision;
            medObject.Update();
            /*
            var time = MedicineTABLE.GetCustom;
            CustomNotification.SetAlarmManager(this,medObject.ma_id, medObject.ma_name,time );
            */
            //if(a) morning if(b) lunch and so on...
            //medObject.Update();

            //CustomNotification.SetAlarmManager(this, $"ได้เวลาทานยา {medObject.ma_name}",(int)DateTime.Now.DayOfWeek,medObject.ma_set_time, Resource.Raw.notialert);
            this.Finish();
        }

        private void cameraClickEvent(object sender, EventArgs e)
        {
            var intent = new Intent(MediaStore.ActionImageCapture);
            //App._file = new File(App._dir, string.Format($@"HappyHealthyCS_{Guid.NewGuid()}.jpg"));
            filePath = System.IO.Path.Combine(App._dir.AbsolutePath, $@"HappyHealthyCS_{Guid.NewGuid()}.jpg");
            App._file = new File(filePath);
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {

            base.OnActivityResult(requestCode, resultCode, data);
            LoadImage();
        }

        private void LoadImage()
        {
            var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            var contentUri = Android.Net.Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);
            var height = Resources.DisplayMetrics.HeightPixels;
            var width = medImage.Width;
            App.bitmap = App._file.Path.LoadBitmap(width, height);//LoadBitmap(App._file.Path, width, height);
            if (App.bitmap != null)
            {
                medImage.SetImageBitmap(App.bitmap);
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
    }
}
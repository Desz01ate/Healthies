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
//using Android.Icu.Text;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Java.Text;
using System.Security.Cryptography;
using SQLite;


namespace HappyHealthyCSharp
{
    [Activity(Label = "Register",ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class Register : Activity
    {
        DatePickerDialog mDatePicker;
        Calendar mCarlendar;
        string textDate, sysDate;
        TextView bdate;
        EditText email, pw, name, idNo;
        RadioButton mRadio, fRadio;
        ImageView register, backbtt;
        string insertDate;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_register);
            bdate = FindViewById<TextView>(Resource.Id.chooseBirthday);
            email = FindViewById<EditText>(Resource.Id.userID);
            pw = FindViewById<EditText>(Resource.Id.userPW);
            name = FindViewById<EditText>(Resource.Id.userName);
            mRadio = FindViewById<RadioButton>(Resource.Id.register_radio_male);
            fRadio = FindViewById<RadioButton>(Resource.Id.register_radio_female);
            register = FindViewById<ImageView>(Resource.Id.register_button);
            backbtt = FindViewById<ImageView>(Resource.Id.register_back_btt);
            //Code goes here
            mRadio.Checked = true;
            mCarlendar = Calendar.GetInstance(Java.Util.TimeZone.GetTimeZone("GMT+7"));
            bdate.Click += delegate {
                mDatePicker = new DatePickerDialog(this, delegate {
                    mCarlendar.Set(mDatePicker.DatePicker.Year, mDatePicker.DatePicker.Month, mDatePicker.DatePicker.DayOfMonth);
                    Date date = mCarlendar.Time;
                    textDate = new SimpleDateFormat("dd-MMMM-yyyy").Format(date);
                    insertDate = new SimpleDateFormat("yyyy-MM-dd").Format(date);
                    bdate.Text = textDate;

                }, 2000, DateTime.Now.Month, DateTime.Now.Day);
                mDatePicker.Show();
            };
            register.Click += async delegate {
                if (isFieldValid())
                {
                    var user = new UserTABLE() {
                        ud_name = name.Text
                        , ud_gender = mRadio.Checked ? "M" : "F"
                        , ud_birthdate = DateTime.Parse(insertDate)
                        , ud_email = email.Text
                        ,ud_pass = AccountHelper.CreatePasswordHash(pw.Text)
                    };
                    var Service = new HHCSService.HHCSService();
                    ProgressDialog progressDialog = new ProgressDialog(this);
                    progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progressDialog.SetMessage("ระบบกำลังทำการลงทะเบียนข้อมูลของท่าน กรุณารอสักครู่");
                    progressDialog.Indeterminate = true;
                    progressDialog.SetCancelable(false);
                    progressDialog.Show();
                    object[] returnData = null;
                    await System.Threading.Tasks.Task.Run(delegate {
                        returnData = Service.Register(email.Text, pw.Text
                        , user.ud_iden_number
                        , user.ud_gender
                        , user.ud_name
                        , user.ud_birthdate);
                    });
                    progressDialog.Dismiss();
                    if (returnData.Length == 2)
                    {
                        user.ud_id = (int)returnData[0];
                        user.ud_pass = (string)returnData[1];
                        if (user.Insert())
                        {
                            var conn = SQLiteInstance.GetConnection;//new SQLiteConnection(Extension.sqliteDBPath);
                            var sql = $@"select * from UserTABLE where ud_email = '{email}'";
                            var result = conn.Query<UserTABLE>(sql);
                            this.SetPreference("ud_email", email.Text);
                            this.SetPreference("ud_pass", pw.Text);
                            this.SetPreference("ud_id", user.ud_id);
                            Extension.CreateDialogue(this, "การลงทะเบียนเสร็จสมบูรณ์", delegate
                            {
                                StartActivity(typeof(MainActivity));
                                this.Finish();
                            }).SetCancelable(false).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "การลงทะเบียนล้มเหลว กรุณาตรวจสอบข้อมูลผู้ใช้อีกครั้ง",ToastLength.Short).Show();
                    }      
                }
            };

        

        // Create your application here
        backbtt.Click += delegate
            {
                this.Finish();
            };
        }
        public bool isFieldValid()
        {
            if (string.IsNullOrEmpty(email.Text) || string.IsNullOrEmpty(pw.Text) || string.IsNullOrEmpty(name.Text) || string.IsNullOrEmpty(insertDate))
            {
                Extension.CreateDialogue(this, "กรุณากรอกค่าในช่องกรอกข้อมูลให้ครบทุกค่า (อีเมลล์,รหัสผ่าน,ชื่อผู้ใช้,เพศ และ วันเกิด)").Show();
                //Toast.MakeText(this, "กรุณากรอกค่า", ToastLength.Long).Show();
                return false;
            }
            if (!email.Text.IsValidEmailFormat())
            {
                Extension.CreateDialogue(this, "กรุณากรอกข้อมูลอีเมลล์ที่ใช้จริง").Show();
                return false;
            }
            return true;
        }
        
    }
}
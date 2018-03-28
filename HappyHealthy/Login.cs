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
using System.Data;
using Plugin.LocalNotifications;
using SQLite;
using System.Threading;


namespace HappyHealthyCSharp
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class Login : Activity
    {
        private static Context _loginContext;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            SetContentView(Resource.Layout.activity_login);
            // Create your application here
            _loginContext = this;
            //is Cache data available?
            if ((Extension.getPreference("ud_id", 0, this) != 0))
            {
                StartActivity(typeof(MainActivity));
                Finish();
            }
            //
            var id = FindViewById<EditText>(Resource.Id.userID);
            var pw = FindViewById<EditText>(Resource.Id.userPW);
            var login = FindViewById<ImageView>(Resource.Id.loginBtt);
            var register = FindViewById<TextView>(Resource.Id.textViewRegis);
            var forgot = FindViewById<TextView>(Resource.Id.textViewForget);
            forgot.Visibility = ViewStates.Invisible;
            //id.Text = "kunvutloveza@hotmail.com";
            //pw.Text = "123456";
            ProgressDialog progressDialog = new ProgressDialog(this);
            login.Click += async delegate
            {
                Extension.setPreference("ud_email", id.Text, this);
                Extension.setPreference("ud_pass", pw.Text, this);
                if (new UserTABLE().SelectAll(x=>x.ud_email == id.Text).Count == 0)
                {
                    progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progressDialog.SetTitle("ดาวน์โหลดข้อมูล");
                    progressDialog.SetMessage("กำลังดาวน์โหลดข้อมูลของท่าน กรุณารอสักครู่");
                    progressDialog.Indeterminate = true;
                    progressDialog.SetCancelable(false);
                    progressDialog.Show();
                    await MySQLDatabaseHelper.GetDataFromMySQLToSQLite(id.Text, pw.Text);
                    LogIn(id.Text, pw.Text);
                    progressDialog.Dismiss();
                }
                else
                {
                    LogIn(id.Text, pw.Text);
                }
            };
            register.Click += delegate
            {
                StartActivity(new Intent(this, typeof(Register)));
            };
            //forgot.Visibility = ViewStates.Invisible;
            forgot.Click += delegate
            {
                StartActivity(new Intent(this, typeof(PasswordResetActivity)));
            };
        }

        private void LogIn(string id, string pw)
        {
            try
            {
                if (AccountHelper.ComparePassword(pw, new UserTABLE().SelectOne(x=>x.ud_email == id).ud_pass))
                {
                    Initialization(id, pw);
                    StartActivity(typeof(MainActivity));
                    this.Finish();
                }
                else
                {
                    Extension.CreateDialogue(this, "ข้อมูลเข้าสู่ระบบของท่านผิดพลาด กรุณาตรวจสอบอีกครั้ง").Show();
                }
            }
            catch(Exception ex)
            {
                
                Extension.CreateDialogue(this, "ข้อมูลเข้าสู่ระบบของท่านผิดพลาด กรุณาตรวจสอบอีกครั้ง").Show();
            }
        }

        public void Initialization(string id, string password)
        {
            var conn = SQLiteInstance.GetConnection;//new SQLiteConnection(Extension.sqliteDBPath);
            //var sql = $@"select * from UserTABLE where ud_email = '{id}'";
            var result = new UserTABLE().SelectOne( x => x.ud_email == id);//conn.Query<UserTABLE>(sql);
            //Extension.setPreference("ud_email", id, this);
            //Extension.setPreference("ud_pass", password, this);
            Extension.setPreference("ud_id", result.ud_id, this);
        }
        public static Context getContext()
        {
            return _loginContext;
        }
    }
}
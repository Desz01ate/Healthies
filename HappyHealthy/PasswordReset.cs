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


namespace HappyHealthyCSharp
{
    [Activity(Label = "Forgot")]
    public class PasswordResetActivity : Activity
    {
        const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_forget_password);
            // Create your application here
            var email = FindViewById<EditText>(Resource.Id.forget_email);
            var backbtt = FindViewById<ImageView>(Resource.Id.forget_back_btt);
            var mailSend = FindViewById<ImageView>(Resource.Id.forget_send_button);
            backbtt.Click += delegate
            {
                this.Finish();
            };
            mailSend.Click += delegate {
                var Thread = new System.Threading.Thread(() => {
                    /*
                    var conn = new MySqlConnection(GlobalFunction.remoteAccess);
                    object result = null;
                    try
                    {
                        conn.Open();
                        var r = new Random();
                        var newPassword = new string(Enumerable.Repeat(chars, 10).Select(s => s[r.Next(s.Length)]).ToArray());
                        var sql = $@"UPDATE user_detail SET ud_pass = '{newPassword}' WHERE ud_email = '{(email.Text)}'";
                        Console.WriteLine(sql);
                        var cmd = new MySqlCommand(sql, conn);
                        cmd.CommandText = sql;
                        cmd.ExecuteScalar();
                        GlobalFunction.SendMail("securapp.assist@gmail.com", "securapp7421", email.Text, "Hello", $@"<body><h2>Password for your account has been changed to <b>{newPassword}.<br>Use this password to login and update your password using HappyHealthy application.</b></h2><br><h2>{(string)result}</h2></body>");
                    }
                    catch
                    {
                        return;
                    }
                    */
                    try
                    {
                        object result = null;
                        var r = new Random();
                        var newPassword = new string(Enumerable.Repeat(chars, 6).Select(s => s[r.Next(s.Length)]).ToArray());
                        var user = new UserTABLE().SelectOne(x=>x.ud_email == email.Text);
                        user.ud_pass = newPassword;
                        if(Extension.SendMail("securapp.assist@gmail.com", "securapp7421", email.Text, "Hello", $@"<body><h2>Password for your account has been changed to <b>{newPassword}.<br>Use this password to login and update your password using HappyHealthy application.</b></h2><br><h2>{(string)result}</h2></body>"))
                            user.Update();
                        Console.WriteLine("send completed");
                    }
                    catch
                    {
                        return;
                    }

                });
                //Toast.MakeText(this, this.Resources.GetResourceName(Resource.Raw.notialert), ToastLength.Long).Show();
                Thread.Start();
                Toast.MakeText(this, $@"Email has been send to {email.Text}",ToastLength.Short).Show();
            };
        }
    }
}
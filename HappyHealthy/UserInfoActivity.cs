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
using SQLite;
using Java.Text;

namespace HappyHealthyCSharp
{

    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    class UserInfoActivity : Activity
    {
        EditText name, age, sex;
        TextView breakfast, lunch, dinner, sleep;
        CheckBox autoSound;
        ImageView updateButton, logoutButton;
        UserTABLE user = new UserTABLE();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_display_user);
            user = user.SelectOne(x => x.ud_id == this.GetPreference("ud_id", 0));
            InitializeControl();
            InitializeData();
            InitializeControlEvent();
            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
            if (rec != "android.hardware.microphone")
            {
                // no microphone, no recording. Disable the button and output an alert
                autoSound.Enabled = false;
            }
        }

        private void InitializeControlEvent()
        {
            updateButton.Click += UpdateUserInfo;
            breakfast.Click += SetTime;
            lunch.Click += SetTime;
            dinner.Click += SetTime;
            sleep.Click += SetTime;
            logoutButton.Click += delegate
            {
                Extension.CreateDialogue2(this, "คุณต้องการออกจากระบบหรือไม่", Android.Graphics.Color.Red, Android.Graphics.Color.White,
                    Android.Graphics.Color.White, Android.Graphics.Color.Black, 42, Logout, delegate { }, "ใช่", "ไม่ใช่");
            };
            autoSound.CheckedChange += delegate
            {
                this.SetPreference("autosound", autoSound.Checked);
            };
            autoSound.Checked = this.GetPreference("autosound", false);
        }

        private void InitializeControl()
        {
            name = FindViewById<EditText>(Resource.Id.user_detail_name);
            age = FindViewById<EditText>(Resource.Id.user_detail_age);
            sex = FindViewById<EditText>(Resource.Id.ud_gen);
            breakfast = FindViewById<TextView>(Resource.Id.ud_bf_time);
            lunch = FindViewById<TextView>(Resource.Id.ud_lu_time);
            dinner = FindViewById<TextView>(Resource.Id.ud_dn_time);
            sleep = FindViewById<TextView>(Resource.Id.ud_sl_time);
            autoSound = FindViewById<CheckBox>(Resource.Id.CheckBox_auto_save);
            logoutButton = FindViewById<ImageView>(Resource.Id.logout);
            updateButton = FindViewById<ImageView>(Resource.Id.save_button_user);
            FindViewById<TextView>(Resource.Id.savedatauser).Visibility = ViewStates.Gone;//just hiding without remove it from the xml, please kindly delete this if you willing to delete this control from xml
        }
        private void InitializeData()
        {
            name.Text = user.ud_name;
            age.Text = (DateTime.Now.Year - user.ud_birthdate.Year).ToString();
            sex.Text = Convert.ToString(Extension.StringValidation(user.ud_gender)).ToLower() == "m" ? "ชาย" : "หญิง";
            breakfast.Text = user.ud_bf_time.TimeValidate() ? user.ud_bf_time.ToString("hh:mm tt") : "คลิกที่นี่เพื่อตั้งเวลา";
            lunch.Text = user.ud_lu_time.TimeValidate() ? user.ud_lu_time.ToString("hh:mm tt") : "คลิกที่นี่เพื่อตั้งเวลา";
            dinner.Text = user.ud_dn_time.TimeValidate() ? user.ud_dn_time.ToString("hh:mm tt") : "คลิกที่นี่เพื่อตั้งเวลา";
            sleep.Text = user.ud_sl_time.TimeValidate() ? user.ud_sl_time.ToString("hh:mm tt") : "คลิกที่นี่เพื่อตั้งเวลา";
        }
        private void Logout(object sender, DialogClickEventArgs e)
        {
            Extension.ClearAllReferences(this);
            StartActivity(new Intent(this, typeof(Login)));
            this.Finish();
        }

        private void SetTime(object sender, EventArgs e)
        {
            var tpickerFragment = TimePickerFragment.NewInstance(
            delegate (DateTime time)
            {
                ((TextView)sender).Text = time.ToShortTimeString();
                var id = ((TextView)sender).Id;
                if (id == Resource.Id.ud_bf_time)
                    user.ud_bf_time = time;
                else if (id == Resource.Id.ud_lu_time)
                    user.ud_lu_time = time;
                else if (id == Resource.Id.ud_dn_time)
                    user.ud_dn_time = time;
                else if (id == Resource.Id.ud_sl_time)
                    user.ud_sl_time = time;
                //user.Update();
            });
            tpickerFragment.Show(FragmentManager, TimePickerFragment.TAG);
        }

        private void UpdateUserInfo(object sender, EventArgs e)
        {
            user.Update();
            Extension.CreateDialogue(this, "บันทึกการตั้งค่าผู้ใช้เรียบร้อยแล้ว").Show();
        }


    }
}
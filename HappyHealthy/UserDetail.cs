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
    class UserDetail : Activity
    {
        EditText name, age, sex;
        TextView breakfast, lunch, dinner, sleep;
        CheckBox autoSound;
        UserTABLE user = new UserTABLE();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_display_user);
            name = FindViewById<EditText>(Resource.Id.user_detail_name);
            age = FindViewById<EditText>(Resource.Id.user_detail_age);
            sex = FindViewById<EditText>(Resource.Id.ud_gen);
            breakfast = FindViewById<TextView>(Resource.Id.ud_bf_time);
            lunch = FindViewById<TextView>(Resource.Id.ud_lu_time);
            dinner = FindViewById<TextView>(Resource.Id.ud_dn_time);
            sleep = FindViewById<TextView>(Resource.Id.ud_sl_time);
            autoSound = FindViewById<CheckBox>(Resource.Id.CheckBox_auto_save);
            var logoutBtn = FindViewById<ImageView>(Resource.Id.logout);
            user = user.Select<UserTABLE>($@"SELECT * FROM UserTABLE WHERE UD_ID = '{Extension.getPreference("ud_id", 0, this)}'")[0];
            logoutBtn.Click += delegate
            {
                Extension.clearAllPreference(this);
                StartActivity(new Intent(this, typeof(Login)));
                this.Finish();
            };
            autoSound.CheckedChange += delegate {
                Extension.setPreference("autosound", autoSound.Checked, this);
            };
            InitializeUserData();
            var updateButton = FindViewById<ImageView>(Resource.Id.save_button_user);
            updateButton.Click += UpdateUserInfo;
            name.TextChanged += delegate {
                user.ud_name = name.Text;
                //user.Update();
            };
            breakfast.Click += SetTime;
            lunch.Click += SetTime;
            dinner.Click += SetTime;
            sleep.Click += SetTime;
            autoSound.Checked = Extension.getPreference("autosound", false, this);
            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
            if (rec != "android.hardware.microphone")
            {
                // no microphone, no recording. Disable the button and output an alert
                autoSound.Enabled = false;
            }
            FindViewById<TextView>(Resource.Id.savedatauser).Visibility = ViewStates.Gone;//just hiding without remove it from the xml, please kindly delete this if you willing to delete this control from xml
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
            //UserTABLE.UpdateUserToSQL(txtName.Text, txtSex.Text[0], txtIdenNo.Text,null, null, this);
            //var user = new UserTABLE().Select<UserTABLE>($@"SELECT * FROM UserTABLE WHERE UD_ID = '{Extension.getPreference("ud_id", 0, this)}'")[0];
            user.ud_name = name.Text;
            //user.ud_iden_number = txtIdenNo.Text;
            user.ud_gender = sex.Text;
            user.Update();
            Extension.CreateDialogue(this, "บันทึกการตั้งค่าผู้ใช้เรียบร้อยแล้ว").Show();
        }

        private void InitializeUserData()
        {
            //var user = new UserTABLE().Select<UserTABLE>($@"SELECT * FROM UserTABLE WHERE UD_ID = '{Extension.getPreference("ud_id", 0, this)}'")[0];
            //user = user.getUserDetail(GlobalFunction.getPreference("ud_id", string.Empty, this));
            age.Text = (DateTime.Now.Year - user.ud_birthdate.Year).ToString();
            name.Text = user.ud_name;
            //txtIdenNo.Text = user.ud_iden_number;
            sex.Text = Convert.ToString(Extension.StringValidation(user.ud_gender)) == "M"?"ชาย":"หญิง";
            breakfast.Text = user.ud_bf_time.TimeValidate()?user.ud_bf_time.ToString("hh:mm tt"):"คลิกที่นี่เพื่อตั้งเวลา";
            lunch.Text = user.ud_lu_time.TimeValidate()?user.ud_lu_time.ToString("hh:mm tt") : "คลิกที่นี่เพื่อตั้งเวลา";
            dinner.Text = user.ud_dn_time.TimeValidate()?user.ud_dn_time.ToString("hh:mm tt") : "คลิกที่นี่เพื่อตั้งเวลา";
            sleep.Text = user.ud_sl_time.TimeValidate()?user.ud_sl_time.ToString("hh:mm tt") : "คลิกที่นี่เพื่อตั้งเวลา";
        }
    }
}
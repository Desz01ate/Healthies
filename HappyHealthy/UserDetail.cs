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
            var tempLogOut = FindViewById<ImageView>(Resource.Id.logout);
            user = user.Select<UserTABLE>($@"SELECT * FROM UserTABLE WHERE UD_ID = '{Extension.getPreference("ud_id", 0, this)}'")[0];
            tempLogOut.Click += delegate
            {
                Extension.clearAllPreference(this);
                StartActivity(new Intent(this, typeof(Login)));
                this.Finish();
            };
            InitializeUserData();
            var updateButton = FindViewById<ImageView>(Resource.Id.save_button_user);
            updateButton.Click += UpdateUserInfo;
            breakfast.Click += SetTime;
            lunch.Click += SetTime;
            dinner.Click += SetTime;
            sleep.Click += SetTime;
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
            sex.Text = Convert.ToString(Extension.StringValidation(user.ud_gender));
            breakfast.Text = user.ud_bf_time.ToString("hh:MM tt");
            lunch.Text = user.ud_lu_time.ToString("hh:MM tt");
            dinner.Text = user.ud_dn_time.ToString("hh:MM tt");
            sleep.Text = user.ud_sl_time.ToString("hh:MM tt");
        }
    }
}
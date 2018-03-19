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
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class Develop : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_develop);
            var back = FindViewById<ImageView>(Resource.Id.imageViewbackpill);
            back.Click += delegate {
                Finish();
            };
            var header = FindViewById<TextView>(Resource.Id.textView_header_name_pill);
            header.Text = "ผู้พัฒนา";
            var addhiding = FindViewById<ImageView>(Resource.Id.imageViewAddPill);
            addhiding.Visibility = ViewStates.Gone;
            // Create your application here
        }
    }
}
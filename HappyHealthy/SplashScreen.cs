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
using Android.Graphics.Drawables;
using static Android.Support.V4.View.ViewPropertyAnimatorCompat;
using Java.Lang;
using Android.Views.Animations;
using Android.Speech.Tts;

namespace HappyHealthyCSharp
{
    [Activity(Theme = "@style/MyMaterialTheme.Base", MainLauncher = true,NoHistory = true,ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SplashScreen : Activity
    {
        Animation view_animation;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var en_USLocale = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = en_USLocale;
            DatabaseHelperExtension.CreateSQLiteTableIfNotExists();
        }
        public override void OnBackPressed() { } //override back button to prevent loading process cancellation
        protected override void OnResume()
        {
            base.OnResume();
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            RequestWindowFeature(Android.Views.WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.activity_splash_screen);
            var imageView = FindViewById<ImageView>(Resource.Id.splashScreenImg);
            view_animation = AnimationUtils.LoadAnimation(this, Resource.Animation.fade_in);
            imageView.StartAnimation(view_animation);
            view_animation.AnimationEnd += delegate {
                StartActivity(typeof(Login));
            };
        }
    }
}
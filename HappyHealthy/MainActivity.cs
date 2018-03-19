//comment
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Renderscripts;
using System.IO;

namespace HappyHealthyCSharp
{
    [Activity(Label = "Loading...", MainLauncher = false, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]

    public class MainActivity : TabActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            
            base.OnCreate(bundle);
            CreateTab(typeof(Home), "Home", "", Resource.Drawable.ic_home2);
            CreateTab(typeof(Report), "Report", "", Resource.Drawable.ic_report2);
            CreateTab(typeof(User), "AlertHealthy", "", Resource.Drawable.ic_foodexe2);
            CreateTab(typeof(IntroHealthy), "Intro","" , Resource.Drawable.ic_heart2);
            CreateTab(typeof(UserDetail), "User", "", Resource.Drawable.ic_user2);
            TabHost.TabWidget.GetChildTabViewAt(2).Enabled = false; //disable User TAB
        }
        private void CreateTab(System.Type activityType, string tag, string label, int drawableId)
        {
            var intent = new Intent(this, activityType);
            //intent.AddFlags(ActivityFlags.NewTask);
            intent.AddFlags(ActivityFlags.ClearTop);
            var spec = TabHost.NewTabSpec(tag);
            var drawableIcon = Resources.GetDrawable(drawableId);
            spec.SetIndicator(label, drawableIcon);
            spec.SetContent(intent);
            TabHost.AddTab(spec);
        }
    }
}


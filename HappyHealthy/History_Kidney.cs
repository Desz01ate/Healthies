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
using Java.Interop;
using Newtonsoft.Json;

namespace HappyHealthyCSharp
{
    [Activity(Label = "History_Diabetes", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class History_Kidney : ListActivity
    {
        ImageView add, back;
        KidneyTABLE kidneyTable;
        JavaList<IDictionary<string, object>> kidneyList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_history_kidney);
            InitializeControl();
            InitializeControlEvent();
            kidneyTable = new KidneyTABLE();
            SetListView();
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
            DatabaseHelperExtension.TrySyncWithMySQL(this);
            Finish();
        }
        private void InitializeControlEvent()
        {
            back.Click += delegate {
                DatabaseHelperExtension.TrySyncWithMySQL(this);
                Finish(); };
            add.Click += delegate { StartActivity(new Intent(this, typeof(Kidney))); };
            ListView.ItemClick += onItemClick;
        }

        private void InitializeControl()
        {
            back = FindViewById<ImageView>(Resource.Id.imageView38);
            add = FindViewById<ImageView>(Resource.Id.imageView41);
        }

        private void onItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            kidneyList[e.Position].TryGetValue("ckd_id", out dynamic ckdID);
            var kidneyObject = new KidneyTABLE();
            kidneyObject = kidneyObject.SelectOne(x=>x.ckd_id == ckdID);
            Extension.CreateDialogue(this, "กรุณาเลือกรายการที่ต้องการจะดำเนินการ",
                delegate
                {
                    var jsonObject = JsonConvert.SerializeObject(kidneyObject);
                    var Intent = new Intent(this, typeof(Kidney));
                    Intent.PutExtra("targetObject", jsonObject);
                    StartActivity(Intent);
                }, 
                delegate
                {
                    Extension.CreateDialogue2(
                    this
                    , "คุณต้องการลบข้อมูลนี้ใช่หรือไม่?"
                    , Android.Graphics.Color.White, Android.Graphics.Color.LightGreen
                    , Android.Graphics.Color.White, Android.Graphics.Color.Red
                    , Extension.adFontSize
                    , delegate
                    {
                        kidneyObject.Delete();
                        SetListView();
                    }
                    , delegate { }
                    , "\u2713"
                    , "X");
                }, "ดูข้อมูล", "ลบข้อมูล").Show();
        }
        protected override void OnResume()
        {
            base.OnResume();
            SetListView();
        }
        [Export("ClickAddKid")]
        public void ClickAddKid(View v)
        {
            StartActivity(new Intent(this, typeof(Kidney)));
        }
        [Export("ClickBackHisKidHome")]
        public void ClickBackHisKidHome(View v)
        {
            this.Finish();
        }
        public void SetListView()
        {
            kidneyList = kidneyTable.SelectAll(x => x.ud_id == this.GetPreference("ud_id", 0)).OrderBy(x => x.ckd_time).ToJavaList();
            ListAdapter = new SimpleAdapter(this, kidneyList, Resource.Layout.history_kidney, new string[] { "ckd_time" }, new int[] { Resource.Id.dateKidney }); //"D_DateTime",date
            ListView.Adapter = ListAdapter;
        }
    }
}
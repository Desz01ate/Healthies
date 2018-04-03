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
    public class History_Pressure : ListActivity
    {
        ListView listView;
        PressureTABLE bpTable;
        ImageView add, back;
        JavaList<IDictionary<string, object>> bpList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_history_pressure);
            InitializeControl();
            InitializeControlEvent();

            bpTable = new PressureTABLE();

            SetListView();
        }

        private void InitializeControlEvent()
        {
            add.Click += delegate { StartActivity(new Intent(this, typeof(Pressure))); };
            back.Click += delegate { Finish(); };
            ListView.ItemClick += onItemClick;
        }

        private void InitializeControl()
        {
            add = FindViewById<ImageView>(Resource.Id.imageView41);
            back = FindViewById<ImageView>(Resource.Id.imageView38);
        }

        protected override void OnResume()
        {
            base.OnResume();
            SetListView();
        }
        private void onItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //bpList[e.Position].TryGetValue("bp_up", out object bpValue);
            bpList[e.Position].TryGetValue("bp_id", out dynamic bpID);
            var pressureObject = new PressureTABLE();
            pressureObject = pressureObject.SelectOne(x => x.bp_id == bpID);
            //pressureObject = pressureObject.SelectAll<PressureTABLE>($"PressureTABLE",x=>x.bp_id == bpID)[0];
            Extension.CreateDialogue(this, "กรุณาเลือกรายการที่ต้องการจะดำเนินการ",
                delegate
                {
                    var jsonObject = JsonConvert.SerializeObject(pressureObject);
                    var Intent = new Intent(this, typeof(Pressure));
                    Intent.PutExtra("targetObject", jsonObject);
                    StartActivity(Intent);
                }, delegate
                {
                    Extension.CreateDialogue2(
                    this
                    , "คุณต้องการลบข้อมูลนี้ใช่หรือไม่?"
                    , Android.Graphics.Color.White, Android.Graphics.Color.LightGreen
                    , Android.Graphics.Color.White, Android.Graphics.Color.Red
                    , Extension.adFontSize
                    , delegate
                    {
                        pressureObject.Delete();
                        //pressureObject.TrySyncWithMySQL(this);
                        SetListView();
                    }
                    , delegate { }
                    , "\u2713"
                    , "X");
                }, "ดูข้อมูล", "ลบข้อมูล").Show();
        }
        public void SetListView()
        {
            bpList = bpTable.SelectAll(x => x.ud_id == Extension.getPreference("ud_id", 0, this)).OrderBy(x => x.bp_time).ToJavaList(); //bpTable.GetJavaList<PressureTABLE>($"SELECT * FROM PressureTABLE WHERE UD_ID = {Extension.getPreference("ud_id", 0, this)} ORDER BY BP_TIME", bpTable.Column);
            //bpList = bpTable.getPressureList($"SELECT * FROM PressureTABLE WHERE UD_ID = {GlobalFunction.getPreference("ud_id", "", this)} ORDER BY BP_TIME");
            ListAdapter = new SimpleAdapter(this, bpList, Resource.Layout.history_diabetes, new string[] { "bp_time" }, new int[] { Resource.Id.date }); //"D_DateTime",date
            ListView.Adapter = ListAdapter;
        }
    }
}
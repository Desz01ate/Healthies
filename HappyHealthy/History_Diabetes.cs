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
    [Activity(Label = "History_Diabetes",ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class History_Diabetes : ListActivity
    {
        ListView listView;
        DiabetesTABLE diaTable;
        JavaList<IDictionary<string, object>> diabList;
        string[] Choice;
        string DateDiabetes, Level, CostStatus, People;
        int D_id, Cost1Diabetes;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_history_diabetes);
            var add = FindViewById<ImageView>(Resource.Id.ClickAddDia);
            add.Click += delegate { StartActivity(new Intent(this,typeof(Diabetes))); };
            var back = FindViewById<ImageView>(Resource.Id.imageView38);
            back.Click += delegate { Finish(); };
            diaTable = new DiabetesTABLE();
            ListView.ItemClick += onItemClick;
            SetListView();
        }
        protected override void OnResume()
        {
            base.OnResume();
            SetListView();
        }
        private void onItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            diabList[e.Position].TryGetValue("fbs_id", out object fbsID);
            var diaObject = new DiabetesTABLE();
            diaObject = diaObject.Select<DiabetesTABLE>($@"SELECT * FROM DiabetesTABLE WHERE fbs_id = {fbsID}")[0];
            Extension.CreateDialogue(this, "กรุณาเลือกรายการที่ต้องการจะดำเนินการ",
                delegate
                {      
                    var jsonObject = JsonConvert.SerializeObject(diaObject);
                    var diabetesIntent = new Intent(this, typeof(Diabetes));
                    diabetesIntent.PutExtra("targetObject", jsonObject);
                    StartActivity(diabetesIntent);
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
                        diaObject.Delete<DiabetesTABLE>(diaObject.fbs_id);
                        diaObject.TrySyncWithMySQL(this);
                        SetListView();
                    }
                    , delegate { }
                    , "\u2713"
                    , "X");
                }, "ดูข้อมูล", "ลบข้อมูล").Show();
        }
        
        [Export("ClickAddDia")]
        public void ClickAddDia(View v)
        {
            StartActivity(new Intent(this, typeof(Diabetes)));
        }
        [Export("ClickBackHisDiaHome")]
        public void ClickBackHisDiaHome(View v)
        {
            this.Finish();
        }
        public void SetListView()
        {
            diabList = diaTable.GetJavaList<DiabetesTABLE>($"SELECT * FROM DiabetesTABLE WHERE UD_ID = {Extension.getPreference("ud_id", 0, this)} ORDER BY FBS_TIME",new DiabetesTABLE().Column);
            ListAdapter = new SimpleAdapter(this, diabList, Resource.Layout.history_diabetes, new string[] { "fbs_time" }, new int[] { Resource.Id.date }); //"D_DateTime",date
            ListView.Adapter = ListAdapter;
            /* for reference on how to work with simpleadapter (it's ain't simple as its name, fuck off)
            var data = new JavaList<IDictionary<string, object>>();
            data.Add(new JavaDictionary<string, object> {
                {"name","Bruce Banner" },{ "status","Bruce Banner feels like SMASHING!"}
            });
            var adapter = new SimpleAdapter(this, data, Android.Resource.Layout.SimpleListItem1, new[] { "name","status" }, new[] { Android.Resource.Id.Text1,Android.Resource.Id.Text2 });
            ListView.Adapter = adapter;
            */
        }

    }
}
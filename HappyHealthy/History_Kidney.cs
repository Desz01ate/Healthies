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
            //ListView = FindViewById<ListView>(Resource.Id.listView);
            InitializeControl();
            InitializeControlEvent();
            kidneyTable = new KidneyTABLE();
            // Create your application here
            SetListView();
        }

        private void InitializeControlEvent()
        {
            back.Click += delegate { Finish(); };
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
                        kidneyObject.TrySyncWithMySQL(this);
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
            //kidneyList = kidneyTable.getKidneyList($"SELECT * FROM KidneyTABLE WHERE UD_ID = {GlobalFunction.getPreference("ud_id", "", this)}"); //must changed
            kidneyList = kidneyTable.SelectAll(x => x.ud_id == Extension.getPreference("ud_id", 0, this)).OrderBy(x => x.ckd_time).ToJavaList();//kidneyTable.GetJavaList<KidneyTABLE>($"SELECT * FROM KidneyTABLE WHERE UD_ID = {Extension.getPreference("ud_id", 0, this)}", new KidneyTABLE().Column); //must changed
            ListAdapter = new SimpleAdapter(this, kidneyList, Resource.Layout.history_kidney, new string[] { "ckd_time" }, new int[] { Resource.Id.dateKidney }); //"D_DateTime",date
            ListView.Adapter = ListAdapter;

            /* for reference on how to work with simpleadapter (it's ain't simple as its name, fuck off)
            var data = new JavaList<IDictionary<string, object>>();
            data.Add(new JavaDictionary<string, object> {
                {"name","Bruce Banner" },{ "status","Bruce Banner feels like SMASHING!"}
            });/*
            var adapter = new SimpleAdapter(this, data, Android.Resource.Layout.SimpleListItem1, new[] { "name","status" }, new[] { Android.Resource.Id.Text1,Android.Resource.Id.Text2 });
            ListView.Adapter = adapter;
            */
        }
    }
}
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
        public override void OnBackPressed()
        {
            base.OnBackPressed();
            DatabaseHelperExtension.TrySyncWithMySQL(this);
            Finish();
        }
        private void InitializeControlEvent()
        {
            add.Click += delegate { StartActivity(new Intent(this, typeof(Pressure))); };
            back.Click += delegate {
                DatabaseHelperExtension.TrySyncWithMySQL(this);
                Finish(); };
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
            bpList[e.Position].TryGetValue("bp_id", out dynamic bpID);
            var pressureObject = new PressureTABLE();
            pressureObject = pressureObject.SelectOne(x => x.bp_id == bpID);
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
                        SetListView();
                    }
                    , delegate { }
                    , "\u2713"
                    , "X");
                }, "ดูข้อมูล", "ลบข้อมูล").Show();
        }
        public void SetListView()
        {
            var data = bpTable.SelectAll(x => x.ud_id == this.GetPreference("ud_id", 0)).OrderBy(x => x.bp_time);
            bpList = data.ToJavaList();
            var textList = new List<string>();
            var boolList = new List<bool>();
            data.ToList().ForEach(x => {
                textList.Add(x.bp_time.ToString("dd-MMMM-yyyy hh:mm:ss tt"));
                boolList.Add(x.bp_state.IsNull() ? true : false);
            });
            ListAdapter = new CAdapter(textList, boolList);
            ListView.Adapter = ListAdapter;
        }
    }
}
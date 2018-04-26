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
    public class History_Diabetes : ListActivity
    {
        DiabetesTABLE diaTable;
        JavaList<IDictionary<string, object>> diabList;
        ImageView add, back;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_history_diabetes);
            InitializeControl();
            InitializeControlEvent();
            diaTable = new DiabetesTABLE();
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
            add.Click += delegate { StartActivity(new Intent(this, typeof(Diabetes))); };
            back.Click += delegate {
                DatabaseHelperExtension.TrySyncWithMySQL(this);
                Finish(); };
            ListView.ItemClick += onItemClick;
        }

        private void InitializeControl()
        {
            add = FindViewById<ImageView>(Resource.Id.ClickAddDia);
            back = FindViewById<ImageView>(Resource.Id.imageView38);
        }

        protected override void OnResume()
        {
            base.OnResume();
            SetListView();
        }
        private void onItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            diabList[e.Position].TryGetValue("fbs_id", out dynamic fbsID);
            var diaObject = new DiabetesTABLE();
            diaObject = diaObject.SelectOne(x => x.fbs_id == fbsID);
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
                        diaObject.Delete();
                        //diaObject.TrySyncWithMySQL(this);
                        SetListView();
                    }
                    , delegate { }
                    , "\u2713"
                    , "X");
                }, "ดูข้อมูล", "ลบข้อมูล").Show();
        }
        public void SetListView()
        {
            var data = diaTable.SelectAll(x => x.ud_id == this.GetPreference("ud_id", 0)).OrderBy(x => x.fbs_time);
            diabList = data.ToJavaList();
            var textList = new List<string>();
            var boolList = new List<bool>();
            data.ToList().ForEach(x => {
                textList.Add(x.fbs_time.ToString("dd-MMMM-yyyy hh:mm:ss tt"));
                boolList.Add(x.fbs_state.IsNull() ? true: false);
            });
            ListAdapter = new CAdapter(textList,boolList);
            ListView.Adapter = ListAdapter;
        }

    }
}
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
using Android.Provider;
using Android.Database;

namespace HappyHealthyCSharp
{
    [Activity(Label = "Doctor", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class History_Doctor : ListActivity
    {
        DoctorTABLE docTable;
        JavaList<IDictionary<string, object>> docList;
        private ImageView backbtt;
        private ImageView addbtt;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_history_doc);
            InitializeControl();
            InitializeControlEvent();
            docTable = new DoctorTABLE();
        }

        private void InitializeControlEvent()
        {
            backbtt.Click += delegate
            {
                this.Finish();
            };

            addbtt.Click += delegate
            {
                StartActivity(typeof(Doctor));
            };
            ListView.ItemClick += onItemClick;
        }

        private void InitializeControl()
        {
            backbtt = FindViewById<ImageView>(Resource.Id.imageViewbackdoc);
            addbtt = FindViewById<ImageView>(Resource.Id.imageview_button_add_doc);
        }

        protected override void OnResume()
        {
            base.OnResume();
            SetListView();
        }
        private void onItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            docList[e.Position].TryGetValue("da_id", out dynamic docID);
            var docObject = new DoctorTABLE();
            docObject = docObject.SelectOne(x => x.da_id == docID);
            Extension.CreateDialogue(this, "กรุณาเลือกรายการที่ต้องการจะดำเนินการ",
                delegate
                {
                    var jsonObject = JsonConvert.SerializeObject(docObject);
                    var DoctorIntent = new Intent(this, typeof(Doctor));
                    DoctorIntent.PutExtra("targetObject", jsonObject);
                    StartActivity(DoctorIntent);
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
                        if (docObject.da_calendar_uri != null)
                        {
                            var deleteUri = CalendarHelper.GetDeleteEventURI(docObject.da_calendar_uri);
                            ContentResolver.Delete(deleteUri, null, null);
                        }
                        docObject.Delete();
                        SetListView();
                    }
                    , delegate { }
                    , "\u2713"
                    , "X");
                }, "ดูข้อมูล", "ลบข้อมูล").Show();
        }
        public void SetListView()
        {
            docList = docTable.SelectAll().ToJavaList();//docTable.GetJavaList<DoctorTABLE>($"SELECT * FROM DoctorTABLE", new DoctorTABLE().Column);
            //pillList = pillTable.getPillList($"SELECT * FROM PillTABLE WHERE UD_ID = {GlobalFunction.getPreference("ud_id", "", this)}");
            ListAdapter = new SimpleAdapter(this, docList, Resource.Layout.history_doc, new string[] { "da_name", "da_comment" }, new int[] { Resource.Id.his_doc_name, Resource.Id.docdetail }); //"D_DateTime",date
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
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
    [Activity(Label = "Doctor",ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class History_Doctor : ListActivity
    {
        DoctorTABLE docTable;
        JavaList<IDictionary<string, object>> docList;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_history_doc);
            // Create your application here
            var backbtt = FindViewById<ImageView>(Resource.Id.imageViewbackdoc);
            backbtt.Click += delegate {
                this.Finish();
            };
            var addbtt = FindViewById<ImageView>(Resource.Id.imageview_button_add_doc);
            addbtt.Click += delegate
            {
                StartActivity(typeof(Doctor));
            };
            ListView.ItemClick += onItemClick;
            docTable = new DoctorTABLE();
        }
        protected override void OnResume()
        {
            base.OnResume();
            SetListView();
        }
        private void onItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            /*
            //pillList[e.Position].TryGetValue("ma_name", out object pillValue);
            docList[e.Position].TryGetValue("da_id", out object docID);
            var docObject = new DoctorTABLE();
            docObject = docObject.Select<DoctorTABLE>($"SELECT * From DoctorTABLE where da_id = {docID}")[0];
            /*
            Extension.CreateDialogue(this, $@"The value for this one : {pillObject.ma_desc}", null, delegate
            {
                var jsonObject = JsonConvert.SerializeObject(pillObject);
                var Intent = new Intent(this, typeof(Pill));
                Intent.PutExtra("targetObject", jsonObject);
                StartActivity(Intent);
            }, "OK", "Edit").Show();
            var jsonObject = JsonConvert.SerializeObject(docObject);
            var Intent = new Intent(this, typeof(Doctor));
            Intent.PutExtra("targetObject", jsonObject);
            StartActivity(Intent);
            */
            docList[e.Position].TryGetValue("da_id", out object docID);
            var docObject = new DoctorTABLE();
            docObject = docObject.Select<DoctorTABLE>($"SELECT * From DoctorTABLE where da_id = {docID}")[0];
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
                        //var deleteUri = CalendarHelper.GetDeleteEventURI(docObject.ma_calendar_uri);
                        //ContentResolver.Delete(deleteUri, null, null);
                        if (docObject.da_calendar_uri != null)
                        {
                            var deleteUri = CalendarHelper.GetDeleteEventURI(docObject.da_calendar_uri);
                            ContentResolver.Delete(deleteUri, null, null);
                        }
                        docObject.Delete<DoctorTABLE>(docObject.da_id);
                        SetListView();
                    }
                    , delegate { }
                    , "\u2713"
                    , "X");
                }, "ดูข้อมูล", "ลบข้อมูล").Show();
        }
        public void SetListView()
        {
            docList = docTable.GetJavaList<DoctorTABLE>($"SELECT * FROM DoctorTABLE", new DoctorTABLE().Column);
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
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
    [Activity(Label = "Pill",ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class History_Medicine : ListActivity
    {
        MedicineTABLE pillTable;
        JavaList<IDictionary<string, object>> medList;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_history_pill);
            // Create your application here
            var backbtt = FindViewById<ImageView>(Resource.Id.imageViewbackpill);
            backbtt.Click += delegate {
                this.Finish();
            };
            var addbtt = FindViewById<ImageView>(Resource.Id.imageViewAddPill);
            addbtt.Click += delegate
            {
                StartActivity(typeof(Medicine));
            };
            ListView.ItemClick += onItemClick;
            pillTable = new MedicineTABLE();
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
            pillList[e.Position].TryGetValue("ma_id", out object pillID);
            var pillObject = new MedicineTABLE();
            pillObject = pillObject.Select<MedicineTABLE>($"SELECT * From MedicineTABLE where ma_id = {pillID}")[0];
            var jsonObject = JsonConvert.SerializeObject(pillObject);
            var Intent = new Intent(this, typeof(Medicine));
            Intent.PutExtra("targetObject", jsonObject);
            StartActivity(Intent);
            */
            medList[e.Position].TryGetValue("ma_id", out object medID);
            var medObject = new MedicineTABLE();
            medObject = medObject.Select<MedicineTABLE>($"SELECT * From MedicineTABLE where ma_id = {medID}")[0];
            Extension.CreateDialogue(this, "กรุณาเลือกรายการที่ต้องการจะดำเนินการ",
                delegate
                {
                    var jsonObject = JsonConvert.SerializeObject(medObject);
                    var medicineIntent = new Intent(this, typeof(Medicine));
                    medicineIntent.PutExtra("targetObject", jsonObject);
                    StartActivity(medicineIntent);
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
                        //var deleteUri = CalendarHelper.GetDeleteEventURI(medObject.ma_calendar_uri);
                        //ContentResolver.Delete(deleteUri, null, null);
                        var time = MedicineTABLE.GetCustom;
                        CustomNotification.CancelAlarmManager(this, medObject.ma_id, medObject.ma_name, time);
                        medObject.Delete<MedicineTABLE>(medObject.ma_id);
                        SetListView();
                    }
                    , delegate { }
                    , "\u2713"
                    , "X");
                }, "ดูข้อมูล", "ลบข้อมูล").Show();
        }
        public void SetListView()
        {
            medList = pillTable.GetJavaList<MedicineTABLE>($"SELECT * FROM MedicineTABLE WHERE UD_ID = {Extension.getPreference("ud_id", 0, this)}",new MedicineTABLE().Column);
            //pillList = pillTable.getPillList($"SELECT * FROM PillTABLE WHERE UD_ID = {GlobalFunction.getPreference("ud_id", "", this)}");
            ListAdapter = new SimpleAdapter(this, medList, Resource.Layout.history_pill, new string[] { "ma_name","ma_desc" }, new int[] { Resource.Id.his_pill_name,Resource.Id.his_pill_desc }); //"D_DateTime",date
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
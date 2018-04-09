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
using Android.Support.V7.App;
using Android.Speech.Tts;
using Java.Interop;

namespace HappyHealthyCSharp
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class History_Food : ListActivity
    {
        FoodTABLE foodTable;
        //List<Dictionary<string, string>> foodList;
        JavaList<IDictionary<string, object>> foodList;
        Button btn_search;
        EditText txt_search;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_food__type_1);
            btn_search = FindViewById<Button>(Resource.Id.bFoodSearch);
            txt_search = FindViewById<EditText>(Resource.Id.tv_Sfood);
            var img_back = FindViewById<ImageView>(Resource.Id.imageViewFoodBackState);
            var addBtt = FindViewById<ImageView>(Resource.Id.imageViewFoodAdd);
            addBtt.Click += delegate {
                StartActivity(new Intent(this, typeof(Add_Food)));
            };
            btn_search.Click += delegate
            {
                setListFood(txt_search.Text);
            };

            txt_search.TextChanged += delegate
            {
                //setListFood(txt_search.Text);
            };
            img_back.Click += delegate
            {
                //StartActivity(new Intent(this, typeof(MainActivity)));
                this.Finish();
            };
            base.ListView.ItemClick += onItemClick;
            foodTable = new FoodTABLE();
            setListFood("");

            // Create your application here

        }
        private void onItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            foodList[e.Position].TryGetValue("food_id", out object FoodID);
            foodList[e.Position].TryGetValue("food_name", out object FoodName);
            var intFoodID = Convert.ToInt32(FoodID);
            //GlobalFunction.createDialog(this, intFoodID.ToString()).Show(); //for debugging only
            var foodDetailIntent = new Intent(this, typeof(FoodDetail));
            foodDetailIntent.PutExtra("food_id", intFoodID);
            foodDetailIntent.AddFlags(ActivityFlags.ClearTop);
            TTS.GetInstance(this).Speak(FoodName.ToString());
            StartActivity(foodDetailIntent);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void setListFood(string what_to_search)
        {
            foodList = foodTable.GetFoodList(this,what_to_search);
            ListAdapter = new SimpleAdapter(this, foodList, Resource.Layout.food_1, new string[] { "food_name", "food_calories", "food_unit", "food_detail" }, new int[] { Resource.Id.food_name, Resource.Id.food_calories, Resource.Id.food_unit, Resource.Id.food_detail });
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
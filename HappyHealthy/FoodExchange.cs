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

namespace HappyHealthyCSharp
{
    [Activity(Label = "FoodExchange",ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class FoodExchange : ListActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_food_exchange);
            var title = FindViewById<TextView>(Resource.Id.textView24);
            title.Text = "รายการอาหารทดแทน";
            var backBtt = FindViewById<ImageView>(Resource.Id.imageViewFoodBackState);
            backBtt.Click +=delegate{
                Finish();
            };
            var addBtt = FindViewById<ImageView>(Resource.Id.imageViewFoodAdd);
            addBtt.Visibility = ViewStates.Gone;
            setListFood(Intent.GetIntExtra("id", 0));
            // Create your application here
        }
        public void setListFood(int id)
        {
            var foodList = new FoodTABLE().GetFoodList(this,id);
            ListAdapter = new SimpleAdapter(this, foodList, Resource.Layout.food_1_for_exchange, 
                new string[] { "food_name", "food_calories", "food_unit", "food_detail" }, 
                new int[] { Resource.Id.his_foodex_name, Resource.Id.his_foodex_calories, Resource.Id.textView, Resource.Id.his_foodex_detail });
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
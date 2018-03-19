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
using Java.Text;
using Java.Util;
using Android.Speech.Tts;
using SQLite;

namespace HappyHealthyCSharp
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class FoodDetail : Activity 
    {
        [PrimaryKey]
        public int food_id { get; set; }
        public string food_name { get; set; }
        public int food_amt { get; set; }
        public decimal food_cal { get; set; }
        public string food_unit { get; set; }
        public decimal food_netweight { get; set; }
        public string food_netunit { get; set; }
        public decimal food_protein { get; set; }
        public decimal food_fat { get; set; }
        public decimal food_carbohydrate { get; set; }
        public decimal food_sugars { get; set; }
        public decimal food_sodium { get; set; }
        public string food_detail { get; set; }
        public int food_is_allow { get; set; }
        public int food_for_ckd { get; set; }
        //reconstruct of sqlite keys + attributes
        FoodTABLE foodTABLE;
        Dictionary<string, string> detailFood;
        double total;
        EditText editCal_Total;
        TextView header,f_name, f_cal, f_unit, f_netweight, f_netunit, f_pro, f_fat, f_car, f_sugar, f_sodium, f_amount, f_detail;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_food_detail);
            //var addhiding = FindViewById<ImageView>(Resource.Id.imageViewFoodAdd);
            //addhiding.Visibility = ViewStates.Gone;
            var img_back = FindViewById<ImageView>(Resource.Id.imageViewFoodBackState);
            img_back.Click += delegate {
                //StartActivity(new Intent(this, typeof(Food_Type_1)));
                this.Finish();
            };//back button
            total = 1;
            foodTABLE = new FoodTABLE();
            f_name = FindViewById<TextView>(Resource.Id.food_name2);
            f_cal = FindViewById<TextView>(Resource.Id.food_cal2);
            f_unit = FindViewById<TextView>(Resource.Id.food_unit2);
            f_netweight = FindViewById<TextView>(Resource.Id.food_netweight2);
            f_netunit = FindViewById<TextView>(Resource.Id.food_netunit2);
            f_pro = FindViewById<TextView>(Resource.Id.food_protein2);
            f_fat = FindViewById<TextView>(Resource.Id.food_fat2);
            f_car = FindViewById<TextView>(Resource.Id.food_carbohydrate2);
            f_sugar = FindViewById<TextView>(Resource.Id.food_sugar2);
            f_sodium = FindViewById<TextView>(Resource.Id.food_sodium2);
            f_detail = FindViewById<TextView>(Resource.Id.tv_food_detail);
            editCal_Total = FindViewById<EditText>(Resource.Id.et_exe2);
            var foodExchange = FindViewById<ImageView>(Resource.Id.foodExchangeBtt);
            foodExchange.Click += delegate {
                var intent = new Intent(this,typeof(FoodExchange));
                intent.PutExtra("id", Intent.GetIntExtra("food_id", 0));
                StartActivity(intent);
            };
            editCal_Total.Click += delegate {
                total = Convert.ToDouble(editCal_Total.Text);
                SetFoodDetail(total);
            };
            detailFood = foodTABLE.selectDetailByID(Intent.GetIntExtra("food_id", 0));
            SetFoodDetail(total);
            //GlobalFunction.createDialog(this, Intent.GetIntExtra("food_id", 0).ToString()).Show();
            // Create your application here

        }
        public void SetFoodDetail(double t)
        {
            f_name.Text = detailFood["food_name"];
            header = FindViewById<TextView>(Resource.Id.textView24);
            header.Text = detailFood["food_name"];
            
        }
        protected override void OnPause()
        {
            base.OnPause();
            //Finish();
        }

    }
}
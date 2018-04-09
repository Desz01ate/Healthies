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
        TextView header,name,detail, sodium, phosphorus, potassium, protein, magnesium;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_food_detail);
            //var addhiding = FindViewById<ImageView>(Resource.Id.imageViewFoodAdd);
            //addhiding.Visibility = ViewStates.Gone;
            var addBtt = FindViewById<ImageView>(Resource.Id.imageViewFoodAdd);
            addBtt.Visibility = ViewStates.Gone;
            var img_back = FindViewById<ImageView>(Resource.Id.imageViewFoodBackState);
            img_back.Click += delegate {
                //StartActivity(new Intent(this, typeof(Food_Type_1)));
                this.Finish();
            };//back button
            foodTABLE = new FoodTABLE();
            name = FindViewById<TextView>(Resource.Id.food_name);
            detail = FindViewById<TextView>(Resource.Id.food_note_detail);
            sodium = FindViewById<TextView>(Resource.Id.food_sodium);
            phosphorus = FindViewById<TextView>(Resource.Id.food_phosphorus);
            potassium = FindViewById<TextView>(Resource.Id.food_potassium);
            protein = FindViewById<TextView>(Resource.Id.food_protein);
            magnesium = FindViewById<TextView>(Resource.Id.food_magnesium);
            editCal_Total = FindViewById<EditText>(Resource.Id.et_exe2);
            var foodExchange = FindViewById<ImageView>(Resource.Id.foodExchangeBtt);
            foodExchange.Click += delegate {
                var intent = new Intent(this,typeof(FoodExchange));
                intent.PutExtra("id", Intent.GetIntExtra("food_id", 0));
                StartActivity(intent);
            };
            detailFood = foodTABLE.SelectFoodDetailByID(Intent.GetIntExtra("food_id", 0));
            SetFoodDetail();
            //GlobalFunction.createDialog(this, Intent.GetIntExtra("food_id", 0).ToString()).Show();
            // Create your application here

        }
        public void SetFoodDetail()
        {
            header = FindViewById<TextView>(Resource.Id.textView24);
            header.Text = detailFood["food_name"];
            name.Text = detailFood["food_name"];
            detail.Text = detailFood["food_note_detail"];
            sodium.Text = detailFood["food_sodium_str"];
            phosphorus.Text = detailFood["food_phosphorus_str"];
            potassium.Text = detailFood["food_potassium_str"];
            protein.Text = detailFood["food_protein_str"];
            magnesium.Text = detailFood["food_magnesium_str"];
            
        }
        protected override void OnPause()
        {
            base.OnPause();
            //Finish();
        }

    }
}
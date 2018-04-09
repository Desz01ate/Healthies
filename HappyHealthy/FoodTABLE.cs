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
using System.IO;
using System.Data;

namespace HappyHealthyCSharp
{
    class FoodTABLE
    {
        public List<string> Column => new List<string> {
                "food_id",
                "food_name",
                "food_note_detail",
                "food_sodium_lvl",
                "food_sodium_str",
                "food_phosphorus_lvl",
                "food_phosphorus_str",
                "food_potassium_lvl",
                "food_potassium_str",
                "food_protein_lvl",
                "food_protein_str",
                "food_magnesium_lvl",
                "food_magnesium_str",
                "food_fat",
                "food_carbohydrate",
                "food_sugars"
        };
        public int food_id { get; set; }
        public string food_name { get; set; }
        public string food_note_detail { get; set; }
        public double food_sodium_lvl { get; set; }
        public string food_sodium_str { get; set; }
        public string food_phosphorus_lvl { get; set; }
        public string food_phosphorus_str { get; set; }
        public string food_potassium_lvl { get; set; }
        public string food_potassium_str { get; set; }
        public string food_protein_lvl { get; set; }
        public string food_protein_str { get; set; }
        public string food_magnesium_lvl { get; set; }
        public string food_magnesium_str { get; set; }
        public double food_fat { get; set; }
        public double food_carbohydrate { get; set; }
        public double food_sugars { get; set; }
        //16 columns
        public FoodTABLE()
        {

        }
        /// <summary>
        /// Get food table list by using keyword
        /// </summary>
        /// <param name="word"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public JavaList<IDictionary<string, object>> GetFoodList(Context c,string word)
        {
             var service = new HHCSService.HHCSService();
            var tickets = service.GetFoodData(Service.GetInstance.WebServiceAuthentication, word);
            var foodList = new JavaList<IDictionary<string, object>>();
            foreach (DataRow x in tickets.Tables["FoodTABLE"].Rows)
            {
                var food = new JavaDictionary<string, object>();
                for (var i = 0; i < Column.Count; i++)
                {
                    food.Add(Column[i], Extension.StringValidation(x[i].ToString()));
                }
                foodList.Add(food);
                
            }
            return foodList;
        }
        public JavaList<IDictionary<string, object>> GetFoodList(Context c,int exchangeId)
        {
            //var sodium = new KidneyTABLE().Select<double>($@"SELECT SUM(ckd_sodium) FROM KidneyTABLE WHERE UD_ID = '{Extension.getPreference("ud_id", 0, c)}")[0];
            //var potassium = new KidneyTABLE().Select<double>($@"SELECT SUM(ckd_potassium) FROM KidneyTABLE WHERE UD_ID = '{Extension.getPreference("ud_id", 0, c)}")[0];
            //var phosphorus = new KidneyTABLE().Select<double>($@"SELECT SUM(ckd_phosphorus_blood) FROM KidneyTABLE WHERE UD_ID = '{Extension.getPreference("ud_id", 0, c)}")[0];
            var service = new HHCSService.HHCSService();
            var tickets = service.GetFoodExchangeData(Service.GetInstance.WebServiceAuthentication,exchangeId);
            var foodList = new JavaList<IDictionary<string, object>>();
            foreach (DataRow x in tickets.Tables["FoodTABLE"].Rows)
            {
                var food = new JavaDictionary<string, object>();
                for (var i = 0; i < Column.Count; i++)
                {
                    food.Add(Column[i], Extension.StringValidation(x[i].ToString()));
                }
                foodList.Add(food);
            }
            return foodList;
        }
        public Dictionary<string, string> SelectFoodDetailByID(int id)
        {
            var service = new HHCSService.HHCSService();
            var tickets = service.GetFoodData(Service.GetInstance.WebServiceAuthentication, "");
            var query = $@"SELECT * FROM Food where Food_ID = {id}";
            var retValue = new Dictionary<string, string>();
            foreach (DataRow x in tickets.Tables["FoodTABLE"].Rows)
            {
                Console.WriteLine($@"{x[0].ToString()} match {id}");
                if (x[0].ToString() == id.ToString())
                {
                    retValue = new Dictionary<string, string>();
                    for (var i = 0; i < Column.Count; i++)
                    {
                        retValue.Add(Column[i], Extension.StringValidation(x[i].ToString()));
                    }
                    return retValue;
                }
            }
            return null;
        }
    }
}
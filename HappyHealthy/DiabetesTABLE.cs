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
using SQLite;
using System.IO;
using System.Data;
using Xamarin.Forms.Platform.Android;
using System.Threading;
using System.Threading.Tasks;
using HappyHealthyCSharp.HHCSService;
using System.Reflection;

namespace HappyHealthyCSharp
{
    class DiabetesTABLE : DatabaseHelper
    {
        public static List<string> Column => new List<string>()
        {
            "fbs_id",
            "fbs_time",
            "fbs_time_string",
            "fbs_fbs",
            "fbs_fbs_lvl",
            "fbs_fbs_sum",
            //"fbs_state",
            "ud_id"
        };
        public static dynamic caseLevel = new { Low = 100, Mid = 125, High = 126 };
        public static string[] reportKeys => new[] { "fbs_fbs", "fbs_fbs_sum" }; 
        public static double[] reportValuesMinimum => new[] { 70.0, 0 }; //5.7
        public static double[] reportValuesMaximum => new[] { 150.0, 7 };
        public bool IsInDangerousState()
        {
            for (var pIndex = 0; pIndex < reportKeys.Length; pIndex++)
            {
                var prop = reportKeys[pIndex];
                var value = this.GetType().GetProperty(prop).GetValue(this);
                var realvalue = Convert.ToDouble(value);
                if (realvalue < reportValuesMinimum[pIndex])
                {
                    return true;
                }
                else if (realvalue > reportValuesMaximum[pIndex])
                {
                    return true;
                }
            }
            return false;
        }
        [SQLite.PrimaryKey]
        public int fbs_id { get; set; }
        public DateTime fbs_time { get; set; }
        public string fbs_time_string { get;set; }
        public decimal fbs_fbs_sum { get; set; }
        private decimal _fbs;
        public decimal fbs_fbs
        {
            get
            {
                return _fbs;
            }
            set
            {
                _fbs = value;
                if (!IsInDangerousState())
                {
                    fbs_fbs_lvl = (int)HealthState.Fine;
                }
                else
                {
                    fbs_fbs_lvl = (int)HealthState.Danger;
                }
            }
        }
        public int fbs_fbs_lvl { get; private set; }
        public int ud_id { get; set; }
        [Ignore]
        public UserTABLE UserTABLE { get; set; }

        //reconstruct of sqlite keys + attributes
        public DiabetesTABLE()
        {
            //constructor - no need for args since naming convention for instances variable mapping can be use : CB
        }
        public override bool Delete()
        {
            try
            {
                var conn = SQLiteInstance.GetConnection;//new SQLiteConnection(Extension.sqliteDBPath);
                var result = conn.Delete<DiabetesTABLE>(this.fbs_id);
                //conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return false;
            }
        }
    }
}
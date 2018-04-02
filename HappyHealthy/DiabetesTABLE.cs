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
            "ud_id"
        };
        public static dynamic caseLevel = new { Low = 100, Mid = 125, High = 126 };
        [SQLite.PrimaryKey]
        public int fbs_id { get; set; }
        public DateTime fbs_time { get; set; }
        public string fbs_time_string { get; set; }
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
                if (_fbs < caseLevel.Low)
                    fbs_fbs_lvl = 0;
                else if (_fbs <= caseLevel.Mid)
                    fbs_fbs_lvl = 1;
                else if (_fbs >= caseLevel.High)
                    fbs_fbs_lvl = 2;
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
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
using System.Threading;
using System.Threading.Tasks;

namespace HappyHealthyCSharp
{
    class KidneyTABLE : DatabaseHelper
    {
        public static List<string> Column => new List<string>() {
            "ckd_id",
            "ckd_time",
            "ckd_time_string",
            "ckd_gfr",
            "ckd_gfr_level",
            "ckd_creatinine",
            "ckd_bun",
            "ckd_sodium",
            "ckd_potassium",
            "ckd_albumin_blood",
            "ckd_albumin_urine",
            "ckd_phosphorus_blood"
        };
        public static dynamic caseLevel = new { Low = 100, Mid = 125, High = 126 };
        [SQLite.PrimaryKey]
        public int ckd_id { get; set; }
        public DateTime ckd_time { get; set; }
        public string ckd_time_string { get; set; }
        private decimal _gfr;
        [SQLite.MaxLength(3)]
        public decimal ckd_gfr {
            get
            {
                return _gfr;
            }
            set
            {
                _gfr = value;
                if (_gfr < caseLevel.Low)
                    ckd_gfr_level = 0;
                else if (_gfr <= caseLevel.Mid)
                    ckd_gfr_level = 1;
                else if (_gfr >= caseLevel.High)
                    ckd_gfr_level = 2;
            }
        }      
        [SQLite.MaxLength(4)]
        public int ckd_gfr_level{ get;private set;}
        [SQLite.MaxLength(3)]
        public decimal ckd_creatinine { get; set; }
        [SQLite.MaxLength(3)]
        public decimal ckd_bun { get; set; }
        [SQLite.MaxLength(3)]
        public decimal ckd_sodium { get; set; }
        [SQLite.MaxLength(3)]
        public decimal ckd_potassium { get; set; }
        [SQLite.MaxLength(3)]
        public decimal ckd_albumin_blood { get; set; }
        [SQLite.MaxLength(3)]
        public decimal ckd_albumin_urine { get; set; }
        [SQLite.MaxLength(3)]
        public decimal ckd_phosphorus_blood { get; set; }
        public int ud_id { get; set; }
        [Ignore]
        public UserTABLE UserTABLE { get; set; }
        //reconstruct of sqlite keys + attributes
        public KidneyTABLE()
        {
            //constructor - no need for args since naming convention for instances variable mapping can be use : CB
        }
        public override bool Delete()
        {
            try
            {
                var conn = SQLiteInstance.GetConnection;//new SQLiteConnection(Extension.sqliteDBPath);
                var result = conn.Delete<KidneyTABLE>(this.ckd_id);
                //conn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
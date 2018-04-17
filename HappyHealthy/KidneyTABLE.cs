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
            "ckd_phosphorus_blood",
            "ckd_state"
        };
        public static dynamic caseLevel = new { Low = 100, Mid = 125, High = 126 };
        //GFR ref : https://medlineplus.gov/ency/article/007305.htm
        //Creatinine ref : https://www.medicinenet.com/creatinine_blood_test/article.htm
        //BUN ref : https://emedicine.medscape.com/article/2073979-overview#a1
        //sodium ref : https://www.kidney.org/atoz/content/hyponatremia
        //phosphorus ref : https://www.kidney.org/atoz/content/phosphorus
        //potassium ref : https://www.davita.com/kidney-disease/diet-and-nutrition/diet-basics/sodium-and-chronic-kidney-disease/e/5310
        public static string[] reportKeys => new[] { "ckd_gfr", "ckd_creatinine", "ckd_bun", "ckd_sodium", "ckd_potassium", "ckd_phosphorus_blood", "ckd_albumin_blood", "ckd_albumin_urine" };
        public static double[] reportValuesMinimum => new[] { 60, new UserTABLE().ud_gender == "M" ? 97.0 : 88.0, 3, 135, 3.5, 2.5, 4.0, 0 };
        public static double[] reportValuesMaximum => new[] { 120, new UserTABLE().ud_gender == "M" ? 137.0 : 128.0, 20, 145, 5.5, 4.5, 999, 30 };
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
        public int ckd_id { get; set; }
        public DateTime ckd_time { get; set; }
        public string ckd_time_string { get; set; }
        private decimal _gfr;
        [SQLite.MaxLength(3)]
        public decimal ckd_gfr
        {
            get
            {
                return _gfr;
            }
            set
            {
                _gfr = value;
                if (!IsInDangerousState())
                {
                    ckd_gfr_level = (int)HealthState.Fine;
                }
                else
                {
                    ckd_gfr_level = (int)HealthState.Danger;
                }
                ckd_state = Extension.HealthStateCheck((HealthState)ckd_gfr_level);
            }
        }
        public string ckd_state { get; private set; }
        [SQLite.MaxLength(4)]
        public int ckd_gfr_level { get; private set; }
        private decimal _creatinine;
        [SQLite.MaxLength(3)]
        public decimal ckd_creatinine
        {
            get
            {
                return _creatinine;
            }
            set
            {
                _creatinine = value;                
                if (!IsInDangerousState())
                    ckd_state = string.Empty;
                else
                    ckd_state = "!!!";
            }
        }
        private decimal _bun;
        [SQLite.MaxLength(3)]
        public decimal ckd_bun
        {
            get
            {
                return _bun;
            }
            set
            {
                _bun = value;
                if (!IsInDangerousState())
                    ckd_state = string.Empty;
                else
                    ckd_state = "!!!";
            }
        }
        private decimal _sodium;
        [SQLite.MaxLength(3)]
        public decimal ckd_sodium
        {
            get
            {
                return _sodium;
            }
            set
            {
                _sodium = value;
                if (!IsInDangerousState())
                    ckd_state = string.Empty;
                else
                    ckd_state = "!!!";
            }
        }
        private decimal _potassium;
        [SQLite.MaxLength(3)]
        public decimal ckd_potassium
        {
            get
            {
                return _potassium;
            }
            set
            {
                _potassium = value;
                if (!IsInDangerousState())
                    ckd_state = string.Empty;
                else
                    ckd_state = "!!!";
            }
        }
        private decimal _alb_blood;
        [SQLite.MaxLength(3)]
        public decimal ckd_albumin_blood {
            get
            {
                return _alb_blood;
            }
            set
            {
                _alb_blood = value;
                if (!IsInDangerousState())
                    ckd_state = string.Empty;
                else
                    ckd_state = "!!!";
            }
        }
        private decimal _alb_urine;
        [SQLite.MaxLength(3)]
        public decimal ckd_albumin_urine {
            get
            {
                return _alb_urine;
            }
            set
            {
                _alb_urine = value;
                if (!IsInDangerousState())
                    ckd_state = string.Empty;
                else
                    ckd_state = "!!!";
            }
        }
        private decimal _phos_blood;
        [SQLite.MaxLength(3)]
        public decimal ckd_phosphorus_blood {
            get
            {
                return _phos_blood;
            }
            set
            {
                _phos_blood = value;
                if (!IsInDangerousState())
                    ckd_state = string.Empty;
                else
                    ckd_state = "!!!";
            }
        }
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
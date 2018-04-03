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
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace HappyHealthyCSharp
{
    [Serializable]
    class TEMP_KidneyTABLE : DatabaseHelper
    {

        public int      ckd_id_pointer { get; set; }

        public DateTime ckd_time_new   { get; set; }

        public DateTime ckd_time_old   { get; set; }

        public string   ckd_time_string_new { get; set; }

        public double   ckd_gfr_new    { get; set; }

        public double   ckd_gfr_old    { get; set; }

        public int      ckd_gfr_level_new { get; set; }

        public int      ckd_gfr_level_old { get; set; }

        public double   ckd_creatinine_new { get; set; }

        public double   ckd_creatinine_old { get; set; }

        public double   ckd_bun_new { get; set; }

        public double   ckd_bun_old { get; set; }

        public double   ckd_sodium_new { get; set; }

        public double   ckd_sodium_old { get; set; }

        public double   ckd_potassium_new { get; set; }

        public double   ckd_potassium_old { get; set; }

        public double   ckd_albumin_blood_new { get; set; }

        public double   ckd_albumin_blood_old { get; set; }

        public double   ckd_albumin_urine_new { get; set; }

        public double   ckd_albumin_urine_old { get; set; }

        public double   ckd_phosphorus_blood_new { get; set; }

        public double   ckd_phosphorus_blood_old { get; set; }

        public string   mode { get; set; }

        public int ud_id { get; set; }
        public static List<string> Column => new List<string>() {
            "ckd_id_pointer",
            "ckd_time_new",
            "ckd_time_old",
            "ckd_time_string_new",
            "ckd_gfr_new",
            "ckd_gfr_old",
            "ckd_gfr_level_new",
            "ckd_gfr_level_old",
            "ckd_creatinine_new",
            "ckd_creatinine_old",
            "ckd_bun_new",
            "ckd_bun_old",
            "ckd_sodium_new",
            "ckd_sodium_old",
            "ckd_potassium_new",
            "ckd_potassium_old",
            "ckd_albumin_blood_new",
            "ckd_albumin_blood_old",
            "ckd_albumin_urine_new",
            "ckd_albumin_urine_old",
            "ckd_phosphorus_blood_new",
            "ckd_phosphorus_blood_old",
            "mode"
        };

        public TEMP_KidneyTABLE() { }
    }
}
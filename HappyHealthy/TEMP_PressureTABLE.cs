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
    class TEMP_PressureTABLE : DatabaseHelper
    {

        public int      bp_id_pointer   { get; set; }

        public DateTime bp_time_new     { get; set; }

        public DateTime bp_time_old     { get; set; }

        public string   bp_time_string_new { get; set; }

        public int      bp_up_new     { get; set; }

        public int      bp_up_old     { get; set; }

        public int      bp_lo_new     { get; set; }

        public int      bp_lo_old     { get; set; }

        public int      bp_hr_new     { get; set; }

        public int      bp_hr_old     { get; set; }

        public int      bp_up_lvl_new { get; set; }

        public int      bp_up_lvl_old { get; set; }

        public int      bp_lo_lvl_new { get; set; }

        public int      bp_lo_lvl_old { get; set; }

        public int      bp_hr_lvl_new { get; set; }

        public int      bp_hr_lvl_old { get; set; }

        public string   mode          { get; set; }

        public int ud_id { get; set; }
        public static List<string> Column => new List<string>() {
            "bp_id_pointer",
            "bp_time_new",
            "bp_time_old",
            "bp_time_string_new",
            "bp_up_new",
            "bp_up_old",
            "bp_lo_new",
            "bp_lo_old",
            "bp_hr_new",
            "bp_hr_old",
            "bp_up_lvl_new",
            "bp_up_lvl_old",
            "bp_lo_lvl_new",
            "bp_lo_lvl_old",
            "bp_hr_lvl_new",
            "bp_hr_lvl_old",
            "mode"
        };
    }
}
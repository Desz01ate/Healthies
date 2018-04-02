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
    class TEMP_DiabetesTABLE : DatabaseHelper
    {
        public int fbs_id_pointer { get; set; }
        public DateTime fbs_time_new { get; set; }

        public string fbs_time_string_new { get; set; }
        public DateTime fbs_time_old { get; set; }
        public int fbs_fbs_new { get; set; }
        public int fbs_fbs_old { get; set; }
        public int fbs_fbs_lvl_new { get; set; }
        public int fbs_fbs_lvl_old { get; set; }
        public int fbs_fbs_sum_new { get; set; }
        public int fbs_fbs_sum_old { get; set; }
        public string mode { get; set; }
        public int ud_id { get; set; }
        public static List<string> Column => new List<string>(){
            "fbs_id_pointer",
            "fbs_time_new",
            "fbs_time_old",
            "fbs_time_string_new",
            "fbs_fbs_new",
            "fbs_fbs_old",
            "fbs_fbs_lvl_new",
            "fbs_fbs_lvl_old",
            "fbs_fbs_sum_new",
            "fbs_fbs_sum_old",
            "mode"
        };

        public TEMP_DiabetesTABLE() { }
    }
}
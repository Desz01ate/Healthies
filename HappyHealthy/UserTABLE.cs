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

namespace HappyHealthyCSharp
{
    class UserTABLE : DatabaseHelper
    {
        public override List<string> Column => throw new NotImplementedException();
        [SQLite.PrimaryKey]
        public int ud_id { get; set; }
        public string ud_iden_number { get; set; }
        public string ud_name { get; set; }
        public string ud_gender { get; set; }
        public DateTime ud_birthdate { get; set; }
        public DateTime ud_bf_time { get; set; }
        public DateTime ud_lu_time { get; set; }
        public DateTime ud_dn_time { get; set; }
        public DateTime ud_sl_time { get; set; }
        public DateTime ud_usually_meal_time { get; set; }
        [SQLite.Unique]
        public string ud_email { get; set; }
        public string ud_pass { get; set; }
        [Ignore]
        public List<DiabetesTABLE> diabetesList { get; set; }
        [Ignore]
        public List<KidneyTABLE> kidneyList { get; set; }
        [Ignore]
        public List<MedicineTABLE> pillList { get; set; }
        [Ignore]
        public List<PressureTABLE> pressureList { get; set; }
        public UserTABLE()
        {

        }
    }
}
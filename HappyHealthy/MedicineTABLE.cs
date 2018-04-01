using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace HappyHealthyCSharp
{
    class MedicineTABLE : DatabaseHelper
    {
        public override List<string> Column => new List<string> {
            "ma_id",
            "ma_name",
            "ma_desc",
            "ma_bf",
            "ma_lu",
            "ma_dn",
            "ma_sl",
            "ma_before_or_after",
            "ma_before_or_after_minute",
            "ma_calendar_uri",
            "ma_pic"
        };
        public static DateTime Morning => DateTime.Parse("07:00 AM");
        public static DateTime Lunch => DateTime.Parse("12:00 PM");
        public static DateTime Dinner => DateTime.Parse("6:00 PM");
        public static DateTime Sleep => DateTime.Parse("10:00 PM");
        public static DateTime GetCustom => DateTime.Now.AddSeconds(30);
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int ma_id { get; set; }
        [SQLite.MaxLength(100)]
        public string ma_name { get; set; }
        [SQLite.MaxLength(255)]
        public string ma_desc { get; set; }
        public bool ma_bf { get; set; }
        public bool ma_lu { get; set; }
        public bool ma_dn { get; set; }
        public bool ma_sl { get; set; }
        public bool ma_before_or_after { get; set; }
        public int ma_before_or_after_minute { get; set; }
        [SQLite.NotNull]
        public bool ma_repeat_monday { get; set; }
        [SQLite.NotNull]
        public bool ma_repeat_tuesday { get; set; }
        [SQLite.NotNull]
        public bool ma_repeat_wednesday { get; set; }
        [SQLite.NotNull]
        public bool ma_repeat_thursday { get; set; }
        [SQLite.NotNull]
        public bool ma_repeat_friday { get; set; }
        [SQLite.NotNull]
        public bool ma_repeat_saturday { get; set; }
        [SQLite.NotNull]
        public bool ma_repeat_sunday { get; set; }
        public string ma_calendar_uri { get; set; }
        [SQLite.MaxLength(255)]
        public string ma_pic { get; set; }
        public int ud_id { get; set; }
        [Ignore]
        public UserTABLE UserTABLE { get; set; }
        //reconstruct of sqlite keys + attributes
        public MedicineTABLE()
        {

            //constructor - no need for args since naming convention for instances variable mapping can be use : CB
        }
        public override bool Delete()
        {
            try
            {
                var conn = SQLiteInstance.GetConnection;//new SQLiteConnection(Extension.sqliteDBPath);
                var result = conn.Delete<MedicineTABLE>(this.ma_id);
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
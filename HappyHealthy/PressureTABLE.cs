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

    class PressureTABLE : DatabaseHelper
    {
        public static List<string> Column => new List<string>()
        {
            "bp_id",
            "bp_time",
            "bp_time_string",
            "bp_up",
            "bp_lo",
            "bp_hr",
            "bp_up_lvl",
            "bp_lo_lvl",
            "bp_hr_lvl",
        };
        public static dynamic caseLevel = new
        {
            lLow = 80,
            lMidLow = 84,
            lMid = 89,
            lMidHigh = 99,
            lHigh = 109,
            lVeryHigh = 110,
            uLow = 120,
            uMidLow = 129,
            uMid = 139,
            uMidHigh = 159,
            uHigh = 179,
            uVeryHigh = 180
        };
        [SQLite.PrimaryKey]
        public int bp_id { get; set; }
        public DateTime bp_time { get; set; }
        public string bp_time_string { get; set; }
        private decimal _upValue;
        [SQLite.MaxLength(3)]
        public decimal bp_up
        {
            get
            {
                return _upValue;
            }
            set
            {
                _upValue = value;
                if (_upValue < caseLevel.uLow)
                    bp_up_lvl = 0;
                else if (_upValue <= caseLevel.uMidLow)
                    bp_up_lvl = 1;
                else if (_upValue <= caseLevel.uMid)
                    bp_up_lvl = 2;
                else if (_upValue <= caseLevel.uMidHigh)
                    bp_up_lvl = 3;
                else if (_upValue <= caseLevel.uHigh)
                    bp_up_lvl = 4;
                else if (_upValue >= caseLevel.uVeryHigh)
                    bp_up_lvl = 5;
            }
        }
        private decimal _lowValue;
        [SQLite.MaxLength(3)]
        public decimal bp_lo
        {
            get
            {
                return _lowValue;
            }
            set
            {
                _lowValue = value;
                if (_lowValue < caseLevel.lLow)
                    bp_lo_lvl = 0;
                else if (_lowValue <= caseLevel.lMidLow)
                    bp_lo_lvl = 1;
                else if (_lowValue <= caseLevel.lMid)
                    bp_lo_lvl = 2;
                else if (_lowValue <= caseLevel.lMidHigh)
                    bp_lo_lvl = 3;
                else if (_lowValue <= caseLevel.lHigh)
                    bp_lo_lvl = 4;
                else if (_lowValue >= caseLevel.lVeryHigh)
                    bp_lo_lvl = 5;
            }
        }
        [SQLite.MaxLength(3)]
        public int bp_hr { get; set; }
        [SQLite.MaxLength(4)]
        public int bp_up_lvl { get; private set; }
        [SQLite.MaxLength(4)]
        public int bp_lo_lvl { get; private set; }
        [SQLite.MaxLength(4)]
        public int bp_hr_lvl { get; set; }
        public int ud_id { get; set; }
        [Ignore]
        public UserTABLE UserTABLE { get; set; }
        //reconstruct of sqlite keys + attributes
        public PressureTABLE()
        {
            //constructor - no need for args since naming convention for instances variable mapping can be use : CB
        }
        public override bool Delete()
        {
            try
            {
                var conn = SQLiteInstance.GetConnection;//new SQLiteConnection(Extension.sqliteDBPath);
                var result = conn.Delete<PressureTABLE>(this.bp_id);
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
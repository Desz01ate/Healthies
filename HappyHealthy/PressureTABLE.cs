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
        public override List<string> Column => new List<string>()
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
        public override void TrySyncWithMySQL(Context c)
        {
            var t = new Thread(() =>
            {
                try
                {
                    var ws = new HHCSService.HHCSService();
                    var presList = new List<HHCSService.TEMP_PressureTABLE>();
                    new TEMP_PressureTABLE().SelectAll(x => x.ud_id == Extension.getPreference("ud_id", 0, c)).ForEach(row =>
                    {
                        var wsObject = new HHCSService.TEMP_PressureTABLE();
                        wsObject.bp_id_pointer = row.bp_id_pointer;
                        wsObject.bp_time_new = row.bp_time_new;
                        wsObject.bp_time_old = row.bp_time_old;
                        wsObject.bp_time_string_new = row.bp_time_string_new;
                        wsObject.bp_up_new = row.bp_up_new;
                        wsObject.bp_up_old = row.bp_up_old;
                        wsObject.bp_lo_new = row.bp_lo_new;
                        wsObject.bp_lo_old = row.bp_lo_old;
                        wsObject.bp_hr_new = row.bp_hr_new;
                        wsObject.bp_hr_old = row.bp_hr_old;
                        wsObject.bp_up_lvl_new = row.bp_up_lvl_new;
                        wsObject.bp_up_lvl_old = row.bp_up_lvl_old;
                        wsObject.bp_lo_lvl_new = row.bp_lo_lvl_new;
                        wsObject.bp_lo_lvl_old = row.bp_lo_lvl_old;
                        wsObject.bp_hr_lvl_new = row.bp_hr_lvl_new;
                        wsObject.bp_hr_lvl_old = row.bp_hr_lvl_old;
                        wsObject.mode = row.mode;
                        presList.Add(wsObject);
                    });
                    ws.SynchonizeData(
                        Service.GetInstance.WebServiceAuthentication
                        , new List<HHCSService.TEMP_DiabetesTABLE>().ToArray()
                        , new List<HHCSService.TEMP_KidneyTABLE>().ToArray()
                        , presList.ToArray());
                    presList.Clear();
                    //var sqliteInstance = new SQLiteConnection(Extension.sqliteDBPath);
                    SQLiteInstance.GetConnection.Execute($"DELETE FROM TEMP_PressureTABLE WHERE ud_id = {Extension.getPreference("ud_id", 0, c)}");
                    //sqliteInstance.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });
            t.Start();
        }
    }
}
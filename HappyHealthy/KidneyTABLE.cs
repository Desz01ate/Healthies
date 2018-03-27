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
        public override List<string> Column => new List<string>() {
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
        public override void TrySyncWithMySQL(Context c)
        {
            var t = new Thread(() =>
            {
                try
                {
                    var ws = new HHCSService.HHCSService();
                    var kidList = new List<HHCSService.TEMP_KidneyTABLE>();
                    new TEMP_KidneyTABLE().Select<TEMP_KidneyTABLE>($"SELECT * FROM TEMP_KidneyTABLE WHERE ud_id = '{Extension.getPreference("ud_id", 0, c)}'").ForEach(row =>
                    {
                        var wsObject = new HHCSService.TEMP_KidneyTABLE();
                        wsObject.ckd_id_pointer = row.ckd_id_pointer;
                        wsObject.ckd_time_new = row.ckd_time_new;
                        wsObject.ckd_time_old = row.ckd_time_old;
                        wsObject.ckd_time_string_new = row.ckd_time_string_new;
                        wsObject.ckd_gfr_new = row.ckd_gfr_new;
                        wsObject.ckd_gfr_old = row.ckd_gfr_old;
                        wsObject.ckd_gfr_level_new = row.ckd_gfr_level_new;
                        wsObject.ckd_gfr_level_old = row.ckd_gfr_level_old;
                        wsObject.ckd_creatinine_new = row.ckd_creatinine_new;
                        wsObject.ckd_creatinine_old = row.ckd_creatinine_old;
                        wsObject.ckd_bun_new = row.ckd_bun_new;
                        wsObject.ckd_bun_old = row.ckd_bun_old;
                        wsObject.ckd_sodium_new = row.ckd_sodium_new;
                        wsObject.ckd_sodium_old = row.ckd_sodium_old;
                        wsObject.ckd_potassium_new = row.ckd_potassium_new;
                        wsObject.ckd_potassium_old = row.ckd_potassium_old;
                        wsObject.ckd_albumin_blood_new = row.ckd_albumin_blood_new;
                        wsObject.ckd_albumin_blood_old = row.ckd_albumin_blood_old;
                        wsObject.ckd_albumin_urine_new = row.ckd_albumin_urine_new;
                        wsObject.ckd_albumin_urine_old = row.ckd_albumin_urine_old;
                        wsObject.ckd_phosphorus_blood_new = row.ckd_phosphorus_blood_new;
                        wsObject.ckd_phosphorus_blood_old = row.ckd_phosphorus_blood_old;
                        wsObject.mode = row.mode;
                        kidList.Add(wsObject);
                    });
                    ws.SynchonizeData(
                        Service.GetInstance.WebServiceAuthentication
                        , new List<HHCSService.TEMP_DiabetesTABLE>().ToArray()
                        , kidList.ToArray()
                        , new List<HHCSService.TEMP_PressureTABLE>().ToArray());
                    kidList.Clear();
                    var sqliteInstance = new SQLiteConnection(Extension.sqliteDBPath);
                    sqliteInstance.Execute($"DELETE FROM TEMP_KidneyTABLE WHERE ud_id = {Extension.getPreference("ud_id", 0, c)}");
                    sqliteInstance.Close();
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
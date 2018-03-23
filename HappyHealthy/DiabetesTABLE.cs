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
using Xamarin.Forms.Platform.Android;
using System.Threading;
using System.Threading.Tasks;

namespace HappyHealthyCSharp
{
    class DiabetesTABLE : DatabaseHelper
    {
        public override List<string> Column => new List<string>()
        {
            "fbs_id",
            "fbs_time",
            "fbs_time_string",
            "fbs_fbs",
            "fbs_fbs_lvl",
            "fbs_fbs_sum",
            "ud_id"
        };
        public static dynamic caseLevel = new { Low = 100,Mid = 125,High = 126};
        [SQLite.PrimaryKey]
        public int fbs_id { get; set; }
        public DateTime fbs_time { get; set; }
        public string fbs_time_string { get; set; }
        public decimal fbs_fbs_sum { get; set; }
        private decimal _fbs;
        public decimal fbs_fbs
        {
            get
            {
                return _fbs;
            }
            set
            {
                _fbs = value;
                if (_fbs < caseLevel.Low)
                    fbs_fbs_lvl = 0;
                else if (_fbs <= caseLevel.Mid)
                    fbs_fbs_lvl = 1;
                else if (_fbs >= caseLevel.High)
                    fbs_fbs_lvl = 2;
            }
        }
        public int fbs_fbs_lvl { get; private set; }
        public int ud_id { get; set; }
        [Ignore]
        public UserTABLE UserTABLE { get; set; }

        //reconstruct of sqlite keys + attributes
        public DiabetesTABLE()
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
                    var diaList = new List<HHCSService.TEMP_DiabetesTABLE>();
                    new TEMP_DiabetesTABLE().Select<TEMP_DiabetesTABLE>($"SELECT * FROM TEMP_DiabetesTABLE WHERE ud_id = '{Extension.getPreference("ud_id", 0, c)}'").ForEach(row =>
                    {
                        var wsObject = new HHCSService.TEMP_DiabetesTABLE();
                        wsObject.fbs_id_pointer = row.fbs_id_pointer;
                        wsObject.fbs_time_new = row.fbs_time_new;
                        wsObject.fbs_time_old = row.fbs_time_old;
                        wsObject.fbs_time_string_new = row.fbs_time_string_new;
                        wsObject.fbs_fbs_new = row.fbs_fbs_new;
                        wsObject.fbs_fbs_old = row.fbs_fbs_old;
                        wsObject.fbs_fbs_lvl_new = row.fbs_fbs_lvl_new;
                        wsObject.fbs_fbs_lvl_old = row.fbs_fbs_lvl_old;
                        wsObject.fbs_fbs_sum_new = row.fbs_fbs_sum_new;
                        wsObject.fbs_fbs_sum_old = row.fbs_fbs_sum_old;
                        wsObject.mode = row.mode;
                        diaList.Add(wsObject);
                    });
                    var result = ws.SynchonizeData(
                        Service.GetInstance.WebServiceAuthentication
                        , diaList.ToArray()
                        , new List<HHCSService.TEMP_KidneyTABLE>().ToArray()
                        , new List<HHCSService.TEMP_PressureTABLE>().ToArray());
                    diaList.Clear();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message); //Exception mostly throw only when the server is down
                    //or device is not able to reach the server
                }
            });
            t.Start();
        }
    }
}
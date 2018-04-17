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
            "bp_state"
        };

        //heart rate ref : http://heartratezone.com/what-is-my-pulse-rate-supposed-to-be/
        public static string[] reportKeys => new[] { "bp_up", "bp_lo", "bp_hr" };
        public static double[] reportValuesMinimum()
        {
            var baseValues = new[] { 90.0, 60.0, -1.0 };
            try
            {
                var user = new UserTABLE().SelectOne(x => x.ud_id == Login.getContext().GetPreference("ud_id", 0));
                var userAge = DateTime.Now.Year - user.ud_birthdate.Year;
                if (userAge < 30)
                {
                    baseValues[2] = 120;
                    //maximum[2] = 160;
                }
                else if (userAge < 40)
                {
                    baseValues[2] = 114;
                    //maximum[2] = 152;
                }
                else if (userAge < 50)
                {
                    baseValues[2] = 108;
                    //maximum[2] = 144;
                }
                else if (userAge < 60)
                {
                    baseValues[2] = 102;
                    //maximum[2] = 136;
                }
                else if (userAge < 70)
                {
                    baseValues[2] = 96;
                    //maximum[2] = 128;
                }
                else
                {
                    baseValues[2] = 90;
                    //maximum[2] = 120;
                }
                baseValues[2] = 90; //overwrite all values due to T.PK request, simply remove this to make it work the same.
            }
            catch
            {
                //it may occured only when user is not set ie. null user at login activity
            }
            return baseValues;
        }
        public static double[] reportValuesMaximum()
        {
            {
                var baseValues = new[] { 180.0, 110.0, -1.0 };
                try
                {
                    var user = new UserTABLE().SelectOne(x => x.ud_id == Login.getContext().GetPreference("ud_id", 0));
                    var userAge = DateTime.Now.Year - user.ud_birthdate.Year;
                    if (userAge < 30)
                    {
                        baseValues[2] = 160;
                    }
                    else if (userAge < 40)
                    {
                        baseValues[2] = 152;
                    }
                    else if (userAge < 50)
                    {
                        baseValues[2] = 144;
                    }
                    else if (userAge < 60)
                    {
                        baseValues[2] = 136;
                    }
                    else if (userAge < 70)
                    {
                        baseValues[2] = 128;
                    }
                    else
                    {
                        baseValues[2] = 120;
                    }
                }
                catch
                {
                    //it may occured only when user is not set ie. null user at login activity
                }
                return baseValues;
            }
        }
        public string IsInDangerousState()
        {
            /*
            for (var pIndex = 0; pIndex < reportKeys.Length; pIndex++)
            {
                var prop = reportKeys[pIndex];
                var value = this.GetType().GetProperty(prop).GetValue(this);
                var realvalue = Convert.ToDouble(value);
                if (realvalue < reportValuesMinimum()[pIndex])
                {
                    return true;
                }
                else if (realvalue > reportValuesMaximum()[pIndex])
                {
                    return true;
                }
            }
            return false;
            */
            if ((HealthState)bp_up_lvl > HealthState.BeAware && (HealthState)bp_lo_lvl > HealthState.BeAware && (HealthState)bp_hr_lvl > HealthState.BeAware) //if everything is above 2, it's mean that user is fine
                return string.Empty;
            var healthState = new[] { "พบแพทย์ทันที พบค่าที่บันทึกในระดับอันตราย", "พบแพทย์ทันที พบค่าที่บันทึกในระดับสูงมาก", "ปรึกษาแพทย์ พบค่าที่บันทึกในระดับสูง", "ปรึกษาแพทย์ พบค่าที่บันทึกในระดับค่อนข้างสูง", "ระดับปกติ", "ระดับเหมาะสม" };
            if (bp_up_lvl > bp_lo_lvl)
            {
                return healthState[bp_lo_lvl];
            }
            else
            {
                return healthState[bp_up_lvl];
            }
        }
        [SQLite.PrimaryKey]
        public int bp_id { get; set; }
        public DateTime bp_time { get; set; }
        public string bp_time_string { get; set; }
        private decimal _upValue;
        public string bp_state { get; private set; }
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
                if (_upValue >= 180) //change anything above 180 to be a maximum case
                    bp_up_lvl = (int)HealthState.VeryDanger;
                else if (160 <= _upValue && _upValue < 180)
                    bp_up_lvl = (int)HealthState.Danger;
                else if (140 <= _upValue && _upValue < 160) //change 140 to be a minimum case
                    bp_up_lvl = (int)HealthState.BeAware;
                else if (130 <= _upValue && _upValue < 140)
                    bp_up_lvl = (int)HealthState.Normal;
                else if (120 <= _upValue && _upValue < 130)
                    bp_up_lvl = (int)HealthState.Fine;
                else if (90 <= _upValue && _upValue < 120)
                    bp_up_lvl = (int)HealthState.VeryFine;
                else
                    bp_up_lvl = (int)HealthState.VeryDanger;

                StateChanger();
                /*
                if (!IsInDangerousState())
                {
                    bp_up_lvl = 1;
                    bp_state = string.Empty;
                }
                else
                {
                    bp_up_lvl = 2;
                    bp_state = "!!!";
                }
                */
            }
        }

        private void StateChanger()
        {
            bp_state = Extension.HealthStateCheck((HealthState)bp_up_lvl, (HealthState)bp_lo_lvl, (HealthState)bp_hr_lvl);
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
                if (_lowValue >= 110) //change anything above 110 to be a maximum case
                    bp_lo_lvl = (int)HealthState.VeryDanger;
                else if (100 <= _lowValue && _lowValue < 110)
                    bp_lo_lvl = (int)HealthState.Danger;
                else if (90 <= _lowValue && _lowValue < 100) //change 90 to be a minimum case
                    bp_lo_lvl = (int)HealthState.BeAware;
                else if (85 <= _lowValue && _lowValue < 90)
                    bp_lo_lvl = (int)HealthState.Normal;
                else if (80 <= _lowValue && _lowValue < 85)
                    bp_lo_lvl = (int)HealthState.Fine;
                else if (60 <= _lowValue && _lowValue < 80)
                    bp_lo_lvl = (int)HealthState.VeryFine;
                else
                    bp_lo_lvl = (int)HealthState.VeryDanger;

                StateChanger();
                /*
                if (!IsInDangerousState())
                {
                    bp_lo_lvl = 1;
                    bp_state = string.Empty;
                }
                else
                {
                    bp_lo_lvl = 2;
                    bp_state = "!!!";
                }
                */
            }
        }
        private int _heartRate;
        [SQLite.MaxLength(3)]
        public int bp_hr
        {
            get
            {
                return _heartRate;
            }
            set
            {
                _heartRate = value;
                if (_heartRate >= reportValuesMaximum()[2])
                    bp_hr_lvl = (int)HealthState.VeryDanger;
                else if (100 <= _heartRate && _heartRate < 110)
                    bp_hr_lvl = (int)HealthState.Danger;
                else if (90 <= _heartRate && _heartRate < 100) //change 90 to be a minimum case
                    bp_hr_lvl = (int)HealthState.BeAware;
                else if (85 <= _heartRate && _heartRate < 90)
                    bp_hr_lvl = (int)HealthState.Normal;
                else if (80 <= _heartRate && _heartRate < 85)
                    bp_hr_lvl = (int)HealthState.Fine;
                else if (41 <= _heartRate && _heartRate < 80)
                    bp_hr_lvl = (int)HealthState.VeryFine;
                else
                    bp_hr_lvl = (int)HealthState.VeryDanger;

                StateChanger();
            }
        }
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
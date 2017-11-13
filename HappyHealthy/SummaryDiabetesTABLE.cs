﻿using System;
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
using MySql.Data.MySqlClient;
using System.IO;
using System.Data;
using SQLiteNetExtensions.Attributes;
using SQLite.Net.Platform.XamarinAndroid;
using SQLite.Net;
using Xamarin.Forms.Platform.Android;
using System.Threading;
using SQLite.Net.Attributes;

namespace HappyHealthyCSharp
{
    [Table("SummaryDiabetesTABLE")]
    class SummaryDiabetesTABLE : DatabaseHelper
    {
        public override List<string> Column => new List<string>()
        {
            "sfbs_id",
            "sfbs_time",
            "sfbs_sfbs",
            "sfbs_sfbs_lvl",
            "ud_id"
        };
        [PrimaryKey, AutoIncrement]
        public int sfbs_id { get; set; }
        public DateTime sfbs_time { get; set; }
        [MaxLength(3)]
        public decimal sfbs_sfbs { get; set; }
        [MaxLength(4)]
        public int sfbs_sfbs_lvl { get; set; }
        [ForeignKey(typeof(UserTABLE))]
        public int ud_id { get; set; }
        [ManyToOne]
        public UserTABLE UserTABLE { get; set; }

        //reconstruct of sqlite keys + attributes
        public SummaryDiabetesTABLE()
        {
            
            //constructor - no need for args since naming convention for instances variable mapping can be use : CB
        }
    }
}
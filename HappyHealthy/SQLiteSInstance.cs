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

namespace HappyHealthyCSharp
{
    public sealed class SQLiteInstance
    {
        static readonly SQLiteInstance _instance = new SQLiteInstance();
        private static readonly string dbPath = Extension.sqliteDBPath;
        static readonly SQLite.SQLiteConnection _conn = new SQLite.SQLiteConnection(dbPath);
        public static SQLite.SQLiteConnection GetConnection
        {
            get
            {
                return _conn;
            }
        }
        SQLiteInstance()
        {
            
        }
    }
}
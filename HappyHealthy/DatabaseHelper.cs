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
using System.Data;
using System.Threading;
using System.Reflection;
using SQLite;

namespace HappyHealthyCSharp
{
    abstract class DatabaseHelper
    {
        //public abstract List<string> Column { get; }
        public virtual bool Delete() { throw new NotImplementedException(); }
        [Obsolete("Please use ToJavaList() extension instead")]
        public virtual JavaList<IDictionary<string, object>> GetJavaList<T>(string queryCustomized, List<string> columnTag) where T : new()
        {
            var dataList = new JavaList<IDictionary<string, object>>();
            var conn = SQLiteInstance.GetConnection;
            var queryResult = conn.Query<T>(queryCustomized);
            queryResult.ForEach(dataRow =>
            {
                var data = new JavaDictionary<string, object>();
                columnTag.ForEach(attribute =>
                {
                    data.Add(attribute, dataRow.GetType().GetProperty(attribute).PropertyType == typeof(DateTime) ? ((DateTime)dataRow.GetType().GetProperty(attribute).GetValue(dataRow)).ToLocalTime() : dataRow.GetType().GetProperty(attribute).GetValue(dataRow));
                });
                dataList.Add(data);
            }); //a little obfuscate code, try solve it for a little challenge :P
            return dataList;
        }
    }
    static class DatabaseHelperExtension
    {
        private static bool isSyncing = false;
        /// <summary>
        /// This function will perform an SQL-like in LINQ manner to get the result for 1 row, no matter how much the data it got
        /// </summary>
        /// <typeparam name="T">Generic type of caller instance</typeparam>
        /// <param name="baseobj">Base caller</param>
        /// <param name="tableName">Table name that is going to query</param>
        /// <param name="predicate">A predicate of the data, think of it as WHERE clause on SQL</param>
        /// <returns></returns>
        public static T SelectOne<T>(this T baseobj, Func<T, bool> predicate) where T : new()
        {
            var conn = SQLiteInstance.GetConnection;//new SQLiteConnection(Extension.sqliteDBPath);
            var result = conn.Query<T>($@"SELECT * FROM {baseobj.GetType().Name}").Where(predicate).FirstOrDefault();
            //a basic code would be like : conn.Query<T>($@"SELECT * FROM Foo").Where(x=>x.id == 1).FirstOrDefault();
            //that is similar to : SELECT * FROM Foo WHERE id = 1
            //please give a predicate that will return only one row.
            //conn.Close();
            return result;
        }
        /// <summary>
        /// This function will perform an SQL-like in LINQ manner to get the result for many rows.
        /// </summary>
        /// <typeparam name="T">Generic type of caller instance</typeparam>
        /// <param name="baseobj">Base caller</param>
        /// <param name="tableName">Table name that is going to query</param>
        /// <param name="predicate">A predicate of the data, think of it as WHERE clause on SQL</param>
        /// <returns></returns>
        public static List<T> SelectAll<T>(this T baseobj, Func<T, bool> predicate = null) where T : new()
        {
            var conn = SQLiteInstance.GetConnection;//new SQLiteConnection(Extension.sqliteDBPath);
            var result = conn.Query<T>($@"SELECT * FROM {baseobj.GetType().Name}");
            //a basic code would be like : conn.Query<T>($@"SELECT * FROM Foo");
            //that is similar to : SELECT * FROM Foo;
            //then we determine the predicate parameter if it's null or not
            //if it's not then we filter the data with predicate or else we return the full-dataset
            //conn.Close();
            if (predicate == null)
                //return SELECT * FROM Foo;
                return result;
            else
                //assume that the predicate is x=>x.id >= 1 && x.id <= 10
                //return SELECT * FROM Foo WHERE id >= 1 AND id <= 10
                //by using this method we can easily apply the logic onto the dataset (but that can't do the complex query ofcourse)
                return result.Where(predicate).ToList();
        }
        [Obsolete("Due to this function required an id that can lead to MISDELETE value on runtime, to solve this please implement the delete function by inherit DatabaseHelper class into child class (but you can still use this function if you're not intended to implement on your own ofcourse)")]
        public static bool Delete<T>(this T baseobj,int id) where T : new()
        {
            try
            {
                var conn = SQLiteInstance.GetConnection;//new SQLiteConnection(Extension.sqliteDBPath);
                var result = conn.Delete<T>(id);
                //conn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool Update(this DatabaseHelper data)
        {
            try
            {
                var conn = SQLiteInstance.GetConnection;//new SQLiteConnection(Extension.sqliteDBPath);
                var result = conn.Update(data);
                //conn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool Insert(this DatabaseHelper data)
        {
            try
            {
                var conn = SQLiteInstance.GetConnection;//new SQLiteConnection(Extension.sqliteDBPath);
                var result = conn.Insert(data);
                //conn.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
        public static bool CreateSQLiteTableIfNotExists()
        {

            try
            {
                var sqliteConn = SQLiteInstance.GetConnection;
                sqliteConn.CreateTable<DiabetesTABLE>();
                sqliteConn.CreateTable<DoctorTABLE>();
                sqliteConn.CreateTable<FoodTABLE>();
                sqliteConn.CreateTable<KidneyTABLE>();
                sqliteConn.CreateTable<MedicineTABLE>();
                sqliteConn.CreateTable<PressureTABLE>();
                sqliteConn.CreateTable<SummaryDiabetesTABLE>();
                sqliteConn.CreateTable<UserTABLE>();
                CreateTriggers(sqliteConn);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($@"DEBUG LOG : {e.ToString()}");
                return false;
            }
        }
        private static void CreateTriggers(SQLiteConnection c)
        {
            c.CreateTable<TEMP_DiabetesTABLE>();
            c.Execute($@"CREATE TRIGGER DiabetesTABLE_After_Insert_Trigger AFTER INSERT ON DiabetesTABLE 
                           BEGIN 
                                INSERT INTO TEMP_DiabetesTABLE(fbs_id_pointer, fbs_time_new, fbs_fbs_new, fbs_fbs_lvl_new,fbs_fbs_sum_new, MODE,ud_id) 
                                VALUES(NEW.fbs_id, NEW.fbs_time, NEW.fbs_fbs, NEW.fbs_fbs_lvl,NEW.fbs_fbs_sum, 'I',NEW.ud_id); 
                                UPDATE DiabetesTABLE 
                                SET 
                                    fbs_time_string = DATETIME('now','7 hours')
                                WHERE 
                                    fbs_id = NEW.fbs_id; 
                                END;");
            c.Execute($@"CREATE TRIGGER DiabetesTABLE_After_Update_Trigger AFTER UPDATE ON DiabetesTABLE 
                             BEGIN
                             	INSERT INTO TEMP_DiabetesTABLE(fbs_id_pointer, fbs_time_old, fbs_fbs_old, fbs_fbs_lvl_old, fbs_time_new, fbs_fbs_new, fbs_fbs_lvl_new,fbs_time_string_new,fbs_fbs_sum_old,fbs_fbs_sum_new,MODE,ud_id)
                             	VALUES(OLD.fbs_id, OLD.fbs_time, OLD.fbs_fbs, OLD.fbs_fbs_lvl, NEW.fbs_time, NEW.fbs_fbs, NEW.fbs_fbs_lvl,NEW.fbs_time_string,OLD.fbs_fbs_sum,NEW.fbs_fbs_sum, 'U',NEW.ud_id);
                             END");
            c.Execute($@"CREATE TRIGGER DiabetesTABLE_After_Delete_Trigger AFTER DELETE ON DiabetesTABLE 
                            BEGIN 
                                INSERT INTO TEMP_DiabetesTABLE(fbs_id_pointer, fbs_time_old, fbs_fbs_old, fbs_fbs_lvl_old,fbs_fbs_sum_old, MODE,ud_id) 
                                VALUES(OLD.fbs_id, OLD.fbs_time, OLD.fbs_fbs, OLD.fbs_fbs_lvl,OLD.fbs_fbs_sum, 'D',OLD.ud_id); 
                            END");

            c.CreateTable<TEMP_PressureTABLE>();
            c.Execute($@"CREATE TRIGGER PressureTABLE_After_Insert_Trigger AFTER INSERT ON PressureTABLE 
                            BEGIN 
                                INSERT INTO TEMP_PressureTABLE(bp_id_pointer, bp_time_new ,bp_up_new ,bp_lo_new ,bp_hr_new ,bp_up_lvl_new ,bp_lo_lvl_new ,bp_hr_lvl_new ,MODE,ud_id) 
                                VALUES(NEW.bp_id, NEW.bp_time ,NEW.bp_up ,NEW.bp_lo ,NEW.bp_hr ,NEW.bp_up_lvl ,NEW.bp_lo_lvl ,NEW.bp_hr_lvl ,'I',NEW.ud_id); 
                            UPDATE PressureTABLE
                            SET
                                bp_time_string = DATETIME('now','7 hours')
                            WHERE
                                bp_id = NEW.bp_id;
                            END");
            c.Execute($@"CREATE TRIGGER PressureTABLE_After_UPDATE_Trigger AFTER UPDATE ON PressureTABLE 
                            BEGIN 
                                INSERT INTO TEMP_PressureTABLE(bp_id_pointer, bp_time_old ,bp_up_old ,bp_lo_old ,bp_hr_old ,bp_up_lvl_old ,bp_lo_lvl_old ,bp_hr_lvl_old ,bp_time_new ,bp_up_new ,bp_lo_new ,bp_hr_new ,bp_up_lvl_new ,bp_lo_lvl_new ,bp_hr_lvl_new,bp_time_string_new ,MODE,ud_id) 
                                VALUES(OLD.bp_id, OLD.bp_time ,OLD.bp_up ,OLD.bp_lo ,OLD.bp_hr ,OLD.bp_up_lvl ,OLD.bp_lo_lvl ,OLD.bp_hr_lvl ,NEW.bp_time ,NEW.bp_up ,NEW.bp_lo ,NEW.bp_hr ,NEW.bp_up_lvl ,NEW.bp_lo_lvl ,NEW.bp_hr_lvl,NEW.bp_time_string ,'U',OLD.ud_id); 
                            END");
            c.Execute($@"CREATE TRIGGER PressureTABLE_After_DELETE_Trigger AFTER DELETE ON PressureTABLE 
                            BEGIN 
                                INSERT INTO TEMP_PressureTABLE(bp_id_pointer, bp_time_old ,bp_up_old ,bp_lo_old ,bp_hr_old ,bp_up_lvl_old ,bp_lo_lvl_old ,bp_hr_lvl_old ,MODE,ud_id) 
                                values(OLD.bp_id, OLD.bp_time ,OLD.bp_up ,OLD.bp_lo ,OLD.bp_hr ,OLD.bp_up_lvl ,OLD.bp_lo_lvl ,OLD.bp_hr_lvl ,'D',OLD.ud_id); 
                            END");

            c.CreateTable<TEMP_KidneyTABLE>();
            c.Execute($@"CREATE TRIGGER KidneyTABLE_After_Insert_Trigger AFTER INSERT ON KidneyTABLE 
                            BEGIN 
                                INSERT INTO `TEMP_KidneyTABLE`(`ckd_id_pointer`, `ckd_time_new` , `ckd_gfr_new` , `ckd_gfr_level_new` , `ckd_creatinine_new` , `ckd_bun_new` , `ckd_sodium_new` , `ckd_potassium_new` , `ckd_albumin_blood_new` , `ckd_albumin_urine_new` , `ckd_phosphorus_blood_new` , `MODE`,ud_id) 
                                VALUES(NEW.ckd_id, NEW.ckd_time , NEW.ckd_gfr , NEW.ckd_gfr_level , NEW.ckd_creatinine , NEW.ckd_bun , NEW.ckd_sodium , NEW.ckd_potassium , NEW.ckd_albumin_blood , NEW.ckd_albumin_urine , NEW.ckd_phosphorus_blood , 'I',NEW.ud_id ); 
                                UPDATE KidneyTABLE
                                SET 
                                    ckd_time_string = DATETIME('now','7 hours')
                                WHERE 
                                    ckd_id = NEW.ckd_id; 
                            END");
            c.Execute($@"CREATE TRIGGER KidneyTABLE_After_UPDATE_Trigger AFTER UPDATE ON KidneyTABLE 
                            BEGIN 
                                INSERT INTO `TEMP_KidneyTABLE`(`ckd_id_pointer`, `ckd_time_new` , `ckd_time_old` , `ckd_gfr_new` , `ckd_gfr_old` , `ckd_gfr_level_new` , `ckd_gfr_level_old` , `ckd_creatinine_new` , `ckd_creatinine_old` , `ckd_bun_new` , `ckd_bun_old` , `ckd_sodium_new` , `ckd_sodium_old` , `ckd_potassium_new` , `ckd_potassium_old` , `ckd_albumin_blood_new` , `ckd_albumin_blood_old` , `ckd_albumin_urine_new` , `ckd_albumin_urine_old` , `ckd_phosphorus_blood_new` , `ckd_phosphorus_blood_old` ,`ckd_time_string_new`, `MODE`,ud_id) 
                                VALUES(OLD.ckd_id, NEW.ckd_time , OLD.ckd_time , NEW.ckd_gfr , OLD.ckd_gfr , NEW.ckd_gfr_level , OLD.ckd_gfr_level , NEW.ckd_creatinine , OLD.ckd_creatinine , NEW.ckd_bun , OLD.ckd_bun , NEW.ckd_sodium , OLD.ckd_sodium , NEW.ckd_potassium , OLD.ckd_potassium , NEW.ckd_albumin_blood , OLD.ckd_albumin_blood , NEW.ckd_albumin_urine , OLD.ckd_albumin_urine , NEW.ckd_phosphorus_blood , OLD.ckd_phosphorus_blood ,NEW.ckd_time_string, 'U' ,OLD.ud_id); 
                            END");
            c.Execute($@"CREATE TRIGGER KidneyTABLE_After_DELETE_Trigger AFTER DELETE ON KidneyTABLE 
                            BEGIN 
                                INSERT INTO `TEMP_KidneyTABLE`(`ckd_id_pointer`, `ckd_time_old` , `ckd_gfr_old` , `ckd_gfr_level_old` , `ckd_creatinine_old` , `ckd_bun_old` , `ckd_sodium_old` , `ckd_potassium_old` , `ckd_albumin_blood_old` , `ckd_albumin_urine_old` , `ckd_phosphorus_blood_old`, `MODE`,ud_id) 
                                VALUES(OLD.ckd_id, OLD.ckd_time , OLD.ckd_gfr , OLD.ckd_gfr_level , OLD.ckd_creatinine , OLD.ckd_bun , OLD.ckd_sodium , OLD.ckd_potassium , OLD.ckd_albumin_blood , OLD.ckd_albumin_urine , OLD.ckd_phosphorus_blood , 'D',OLD.ud_id ); 
                            END");
            //c.Execute("CREATE TABLE `TEMP_KidneyTABLE` (`ckd_id_pointer` int, `ckd_time_new` bigint, `ckd_time_old` bigint, `ckd_gfr_new` float, `ckd_gfr_old` float, `ckd_gfr_level_new` INTEGER, `ckd_gfr_level_old` INTEGER, `ckd_creatinine_new` float, `ckd_creatinine_old` float, `ckd_bun_new` float, `ckd_bun_old` float, `ckd_sodium_new` float, `ckd_sodium_old` float, `ckd_potassium_new` float, `ckd_potassium_old` float, `ckd_albumin_blood_new` float, `ckd_albumin_blood_old` float, `ckd_albumin_urine_new` float, `ckd_albumin_urine_old` float, `ckd_phosphorus_blood_new` Float, `ckd_phosphorus_blood_old` Float, `MODE` varchar(255) )");
            c.Execute("CREATE TABLE `TEMP_SummaryDiabetesTABLE` (`sfbs_id_pointer` int, `sfbs_time_new` bigint, `sfbs_time_old` bigint, `sfbs_sfbs_new` float, `sfbs_sfbs_old` float, `sfbs_sfbs_lvl_new` INT, `sfbs_sfbs_lvl_old` INT, `MODE` varchar(255) )");
            c.Execute("CREATE TRIGGER SummaryDiabetesTABLE_After_Delete_Trigger AFTER DELETE ON SummaryDiabetesTABLE BEGIN insert into TEMP_SummaryDiabetesTABLE(sfbs_id_pointer, sfbs_time_old, sfbs_sfbs_old, sfbs_sfbs_lvl_old, MODE) values(OLD.bp_id, OLD.sfbs_time, OLD.sfbs_sfbs, OLD.sfbs_sfbs_lvl, 'D'); END");
            c.Execute("CREATE TRIGGER SummaryDiabetesTABLE_After_Insert_Trigger AFTER INSERT ON SummaryDiabetesTABLE BEGIN insert into TEMP_SummaryDiabetesTABLE(sfbs_id_pointer, sfbs_time_new, sfbs_sfbs_new, sfbs_sfbs_lvl_new, MODE) values(NEW.bp_id, NEW.sfbs_time, NEW.sfbs_sfbs, NEW.sfbs_sfbs_lvl, 'I'); END");
            c.Execute("CREATE TRIGGER SummaryDiabetesTABLE_After_Update_Trigger AFTER UPDATE ON SummaryDiabetesTABLE BEGIN insert into TEMP_SummaryDiabetesTABLE(sfbs_id_pointer, sfbs_time_old, sfbs_sfbs_old, sfbs_sfbs_lvl_old, sfbs_time_new, sfbs_sfbs_new, sfbs_sfbs_lvl_new, MODE) values(OLD.bp_id, OLD.sfbs_time, OLD.sfbs_sfbs, OLD.sfbs_sfbs_lvl, NEW.sfbs_time, NEW.sfbs_sfbs, NEW.sfbs_sfbs_lvl, 'U'); END");
            //c.Execute("CREATE TABLE `TEMP_DiabetesTABLE` (`fbs_id_pointer` int, `fbs_time_new` bigint, `fbs_time_old` bigint, `fbs_fbs_new` float, `fbs_fbs_old` float, `fbs_fbs_lvl_new` integer, `fbs_fbs_lvl_old` integer, `MODE` varchar(255) )");
        }
        public static JavaList<IDictionary<string, object>> ToJavaList<T>(this IEnumerable<T> dataset) where T : new()
        {
            var type = typeof(T);
            var prop = type.GetProperty("Column", BindingFlags.Public | BindingFlags.Static);
            var value = prop.GetValue(null, null);
            var columnTag = (List<string>)value;
            var dataList = new JavaList<IDictionary<string, object>>();
            foreach(var dataRow in dataset)
            {
                var data = new JavaDictionary<string, object>();
                foreach(var attribute in columnTag)
                {
                    var currentProp = dataRow.GetType().GetProperty(attribute);
                    if (attribute.EndsWith("time"))
                    {
                        //dirty way to format a date..., but I have no time to do it elegantly :(
                        try
                        {
                            data.Add(attribute, ((DateTime)currentProp.GetValue(dataRow)).ToString("dd-MMMM-yyyy hh:mm:ss tt"));
                        }
                        catch
                        {
                            data.Add(attribute, DateTime.Now.ToString("dd-MMMM-yyyy hh:mm:ss tt"));
                        }
                    }
                    else
                    {
                        data.Add(attribute, currentProp.GetValue(dataRow));
                    }
                };
                dataList.Add(data);
            };
            return dataList;
        }
        public static void TrySyncWithMySQL(Context c)
        {
            var t = new Thread(() =>
            {
                try
                {
                    if (!isSyncing) //locking mechanics to make a thread-safe synchronization
                    {
                        isSyncing = true; //lock
                        var ws = new HHCSService.HHCSService();
                        var diaList = new List<HHCSService.TEMP_DiabetesTABLE>();
                        new TEMP_DiabetesTABLE().SelectAll(x => x.ud_id == c.GetPreference("ud_id", 0)).ForEach(row =>
                        {
                            var wsObject = new HHCSService.TEMP_DiabetesTABLE();
                            SetValues(row, ref wsObject);
                            diaList.Add(wsObject);
                        });
                        var kidneyList = new List<HHCSService.TEMP_KidneyTABLE>();
                        new TEMP_KidneyTABLE().SelectAll(x => x.ud_id == c.GetPreference("ud_id", 0)).ForEach(row => {
                            var wsObject = new HHCSService.TEMP_KidneyTABLE();
                            SetValues(row, ref wsObject);
                            kidneyList.Add(wsObject);
                        });
                        var pressureList = new List<HHCSService.TEMP_PressureTABLE>();
                        new TEMP_PressureTABLE().SelectAll(x => x.ud_id == c.GetPreference("ud_id", 0)).ForEach(row => {
                            var wsObject = new HHCSService.TEMP_PressureTABLE();
                            SetValues(row, ref wsObject);
                            pressureList.Add(wsObject);
                        });
                        var result = ws.SynchonizeData(
                            Service.GetInstance.WebServiceAuthentication
                            , diaList.ToArray()
                            , kidneyList.ToArray()
                            , pressureList.ToArray());
                        diaList.Clear();
                        kidneyList.Clear();
                        pressureList.Clear();
                        SQLiteInstance.GetConnection.Execute($"DELETE FROM TEMP_DiabetesTABLE WHERE ud_id = {c.GetPreference("ud_id", 0)}");
                        SQLiteInstance.GetConnection.Execute($"DELETE FROM TEMP_KidneyTABLE WHERE ud_id = {c.GetPreference("ud_id", 0)}");
                        SQLiteInstance.GetConnection.Execute($"DELETE FROM TEMP_PressureTABLE WHERE ud_id = {c.GetPreference("ud_id", 0)}");
                        isSyncing = false; //unlock
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message); //Exception mostly throw only when the server is down
                    //or device is not able to reach the server
                }
            });
            t.Start();
        }
        private static void SetValues<T1, T2>(T1 baseObject, ref T2 wsObject)
        {
            var ctype = typeof(T1);
            var cprop = ctype.GetProperty("Column", BindingFlags.Public | BindingFlags.Static);
            var cvalue = cprop.GetValue(null, null);
            var columnTag = (List<string>)cvalue;
            foreach (var property in columnTag)
            {
                var type = baseObject.GetType();
                var prop = type.GetProperty(property);
                var value = prop.GetValue(baseObject);
                wsObject.GetType().GetProperty(property).SetValue(wsObject, value);
            }
        }
    }
    static class MySQLDatabaseHelper
    {
        public static bool TestConnection(string url)
        {
            try
            {
                //var MySQLConn = new MySqlConnection(url);
                //MySQLConn.Open();
                //MySQLConn.Close();
                return true;

            }
            catch
            {
                return false;
            }
        }
        public static async System.Threading.Tasks.Task<bool> GetDataFromMySQLToSQLite(string email, string password)
        {
            try
            {
                DataSet userData = null;
                DataSet diabetesData = null;
                DataSet kidneyData = null;
                DataSet pressureData = null;
                var ws = new HHCSService.HHCSService();

                await System.Threading.Tasks.Task.Run(delegate {
                    userData = ws.GetData(Service.GetInstance.WebServiceAuthentication, "UserTABLE");
                    diabetesData = ws.GetData(Service.GetInstance.WebServiceAuthentication, "DiabetesTABLE");
                    kidneyData = ws.GetData(Service.GetInstance.WebServiceAuthentication, "KidneyTABLE");
                    pressureData = ws.GetData(Service.GetInstance.WebServiceAuthentication, "PressureTABLE");
                });
                if (userData != null)
                {
                    var userID = -999;
                    var sqliteInstance = SQLiteInstance.GetConnection;//new SQLiteConnection(Extension.sqliteDBPath);
                    foreach (DataRow row in ((DataSet)userData).Tables["UserTABLE"].Rows)
                    {
                        var tempUser = new UserTABLE();
                        tempUser.ud_id = Convert.ToInt32(row[0].ToString());
                        tempUser.ud_email = row[1].ToString();
                        tempUser.ud_pass = row[2].ToString();
                        tempUser.ud_iden_number = row[3].ToString();
                        tempUser.ud_gender = row[4].ToString();
                        tempUser.ud_name = row[5].ToString();
                        tempUser.ud_birthdate = DateTime.Parse(row[6].ToString());
                        userID = tempUser.ud_id;
                        tempUser.Insert();
                    }
                    foreach (DataRow row in ((DataSet)diabetesData).Tables["DiabetesTABLE"].Rows)
                    {
                        var tempDiabetes = new DiabetesTABLE();
                        tempDiabetes.fbs_id = Convert.ToInt32(row[0].ToString());
                        tempDiabetes.fbs_time = ((DateTime)row[1]).AddHours(-14);//.ToThaiLocale();
                        //tempDiabetes.fbs_time_string = row[1].ToString();
                        tempDiabetes.fbs_fbs = Convert.ToDecimal(row[2].ToString());
                        //tempDiabetes.fbs_fbs_lvl = Convert.ToInt32(row[3].ToString());
                        tempDiabetes.fbs_fbs_sum = Convert.ToDecimal(row[4].ToString());
                        tempDiabetes.ud_id = Convert.ToInt32(row[5].ToString());
                        tempDiabetes.Insert();
                    }
                    foreach (DataRow row in ((DataSet)kidneyData).Tables["KidneyTABLE"].Rows)
                    {
                        var tempKidney = new KidneyTABLE();
                        tempKidney.ckd_id = Convert.ToInt32(row[0].ToString());
                        tempKidney.ckd_time = ((DateTime)row[1]).AddHours(-14);//.ToThaiLocale();
                        //tempKidney.ckd_time_string = "NONE";
                        tempKidney.ckd_gfr = Convert.ToDecimal(row[2].ToString());
                        //tempKidney.ckd_gfr_level = Convert.ToInt32(row[3].ToString());
                        tempKidney.ckd_creatinine = Convert.ToDecimal(row[4].ToString());
                        tempKidney.ckd_bun = Convert.ToDecimal(row[5].ToString());
                        tempKidney.ckd_sodium = Convert.ToDecimal(row[6].ToString());
                        tempKidney.ckd_potassium = Convert.ToDecimal(row[7].ToString());
                        tempKidney.ckd_albumin_blood = Convert.ToDecimal(row[8].ToString());
                        tempKidney.ckd_albumin_urine = Convert.ToDecimal(row[9].ToString());
                        tempKidney.ckd_phosphorus_blood = Convert.ToDecimal(row[10].ToString());
                        tempKidney.ud_id = Convert.ToInt32(row[11].ToString());
                        tempKidney.Insert();
                    }
                    foreach (DataRow row in ((DataSet)pressureData).Tables["PressureTABLE"].Rows)
                    {
                        var tempPressure = new PressureTABLE();
                        tempPressure.bp_id = Convert.ToInt32(row[0].ToString());
                        tempPressure.bp_time = ((DateTime)row[1]).AddHours(-14);//.ToThaiLocale();
                        //tempPressure.bp_time_string = "NONE";
                        tempPressure.bp_up = Convert.ToDecimal(row[2].ToString());
                        tempPressure.bp_lo = Convert.ToDecimal(row[3].ToString());
                        tempPressure.bp_hr = Convert.ToInt32(row[4].ToString());
                        //tempPressure.bp_up_lvl = Convert.ToInt32(row[5].ToString());
                        //tempPressure.bp_lo_lvl = Convert.ToInt32(row[6].ToString());
                        tempPressure.bp_hr_lvl = Convert.ToInt32(row[7].ToString());
                        tempPressure.ud_id = Convert.ToInt32(row[8].ToString());
                        tempPressure.Insert();
                    }
                    sqliteInstance.Execute($"DELETE FROM TEMP_DiabetesTABLE WHERE ud_id = {userID}");
                    sqliteInstance.Execute($"DELETE FROM TEMP_KidneyTABLE   WHERE ud_id = {userID}");
                    sqliteInstance.Execute($"DELETE FROM TEMP_PressureTABLE WHERE ud_id = {userID}");
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
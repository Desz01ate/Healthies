using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech.Tts;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HappyHealthyCSharp
{
    public static class Extension
    {
        public static readonly string fileStorePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        public static readonly string sqliteDBPath = Path.Combine(fileStorePath, "hhcs.db3");
        public static readonly int flagValue = -9521;
        public static readonly int adFontSize = 70;
        /// <summary>
        /// Simple dialog box for just showing the message.
        /// </summary>
        /// <param name="t">Base Caller</param>
        /// <param name="message">Dialog Message</param>
        public static AlertDialog.Builder CreateDialogue(Context t, string message, EventHandler<DialogClickEventArgs> action = null, EventHandler<DialogClickEventArgs> cancelAction = null, string actionMessage = "OK", string cancelActionMessage = "Cancel")
        {
            var isCancelAble = cancelAction == null ? false : true; //is cancel action function defined? if yes then the cancel button should be available.
            var nDLG = new AlertDialog.Builder(t);
            nDLG
                .SetMessage(message)
                .SetNeutralButton(actionMessage, action);
            if (isCancelAble == true)
            {
                nDLG
                .SetCancelable(isCancelAble)
                .SetNegativeButton(cancelActionMessage, cancelAction);
            }
            return nDLG;
        }
        /// <summary>
        /// Similar to CreateDialogue function but is able to modify the color of background,text and text size 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="message"></param>
        /// <param name="neutralBtnColor"></param>
        /// <param name="neutralTxtColor"></param>
        /// <param name="negativeBtnColor"></param>
        /// <param name="negativeTxtColor"></param>
        /// <param name="action"></param>
        /// <param name="cancelAction"></param>
        /// <param name="actionMessage"></param>
        /// <param name="cancelActionMessage"></param>
        public static void CreateDialogue2(
            Context t, string message,
            Android.Graphics.Color neutralBtnColor, Android.Graphics.Color neutralTxtColor,
            Android.Graphics.Color negativeBtnColor, Android.Graphics.Color negativeTxtColor,
            float fontSize = 42,
            EventHandler<DialogClickEventArgs> action = null,
            EventHandler<DialogClickEventArgs> cancelAction = null,
            string actionMessage = "OK",
            string cancelActionMessage = "Cancel")
        {
            var isCancelAble = cancelAction == null ? false : true; //is cancel action function defined? if yes then the cancel button should be available.
            var nDLG = new AlertDialog.Builder(t);
            nDLG
                .SetMessage(message)
                .SetNeutralButton(actionMessage, action);
            if (isCancelAble == true)
            {
                nDLG
                .SetCancelable(isCancelAble)
                .SetNegativeButton(cancelActionMessage, cancelAction);
            }
            var createdButton = nDLG.Show();
            //Console.WriteLine(createdButton.GetButton((int)DialogButtonType.Positive).TextSize); //default font size is 42px
            createdButton.GetButton((int)DialogButtonType.Neutral).SetTextSize(Android.Util.ComplexUnitType.Px, fontSize);
            createdButton.GetButton((int)DialogButtonType.Negative).SetTextSize(Android.Util.ComplexUnitType.Px, fontSize);
            createdButton.GetButton((int)DialogButtonType.Neutral).SetBackgroundColor(neutralBtnColor);
            createdButton.GetButton((int)DialogButtonType.Negative).SetBackgroundColor(negativeBtnColor);
            createdButton.GetButton((int)DialogButtonType.Neutral).SetTextColor(neutralTxtColor);
            createdButton.GetButton((int)DialogButtonType.Negative).SetTextColor(negativeTxtColor);
        }
        public static void SetPreference(this Context c, string key, string value)
        {
            ISharedPreferences prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(c);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(key, value);
            editor.Apply();
        }
        public static void SetPreference(this Context c, string key, int value)
        {
            ISharedPreferences prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(c);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutInt(key, value);
            editor.Apply();
        }
        public static void SetPreference(this Context c, string key, bool value)
        {
            ISharedPreferences prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(c);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean(key, value);
            editor.Apply();
        }
        public static void SetPreference(this Context c, string key, float value)
        {
            ISharedPreferences prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(c);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutFloat(key, value);
            editor.Apply();
        }
        public static string GetPreference(this Context c, string key, string defVal)
        {
            ISharedPreferences prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(c);
            return prefs.GetString(key, defVal);
        }
        public static int GetPreference(this Context c, string key, int defVal)
        {
            ISharedPreferences prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(c);
            return prefs.GetInt(key, defVal);
        }
        public static bool GetPreference(this Context c, string key, bool defVal)
        {
            ISharedPreferences prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(c);
            return prefs.GetBoolean(key, defVal);
        }
        public static float GetPreference(this Context c, string key, float defVal)
        {
            ISharedPreferences prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(c);
            return prefs.GetFloat(key, defVal);
        }
        public static void ClearAllReferences(this Context c)
        {
            ISharedPreferences prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(c);
            prefs.Edit().Clear().Commit();
        }
        public static string StringValidation<T>(this T t)
        {
            if (Double.TryParse(t.ToString(), out double val) == true)
            {
                return (Math.Round(Convert.ToDouble(t), 2)).ToString();
            }
            if (ReferenceEquals(t, null))
            {
                return "";
            }
            return t.ToString();
        }
        public static DateTime ToThaiLocale(this DateTime n)
        {
            var thaiCalendar = new System.Globalization.ThaiBuddhistCalendar();
            var now = n;
            var thaiTime = new DateTime(thaiCalendar.GetYear(now), thaiCalendar.GetMonth(now), thaiCalendar.GetDayOfMonth(now), thaiCalendar.GetHour(now), thaiCalendar.GetMinute(now), thaiCalendar.GetSecond(now));
            return thaiTime;
        }
        public static DateTime RevertThaiLocale(this DateTime n)
        {
            var gregorianCalendar = new System.Globalization.GregorianCalendar();
            var now = n;
            var globalTime = new DateTime(gregorianCalendar.GetYear(now), gregorianCalendar.GetMonth(now), gregorianCalendar.GetDayOfMonth(now), gregorianCalendar.GetHour(now), gregorianCalendar.GetMinute(now), gregorianCalendar.GetSecond(now));
            return globalTime;
        }
        public static string GetValidPathForFileStore(string filename)
        {
            return Path.Combine(fileStorePath, filename);
        }
        #region Test
        // '' <summary>
        // '' Send email using SMTP can be use with @hotmail,@outlook,@gmail and @icloud and return True when send complete, return False when failed to send.
        // '' </summary>
        // '' <param name="sendfrom">Email address sender</param>
        // '' <param name="psswd">Password for sender (Recommend to use MailLib.Security.Encode() with password if you don't have any encryption for this parameter to grab some secure)</param>
        // '' <param name="sendto">Type both address and email-provider for receiver (ie. someone@somewhere.com)</param>
        // '' <param name="subject">Email subject</param>
        // '' <param name="body">Email body</param>
        // '' <param name="fileloc">Email attachment file location</param>
        public static bool SendMail(string sendfrom, string psswd, string sendto, string subject, string body)
        {
            try
            {
                MailMessage Msg = new MailMessage()
                {
                    From = new MailAddress(sendfrom),
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = body
                };
                Msg.To.Add(new MailAddress(sendto));
                SmtpClient smtpserver = new SmtpClient();
                if (((sendfrom.Substring(sendfrom.IndexOf("@")) == "@hotmail.com")
                || (sendfrom.Substring(sendfrom.IndexOf("@")) == "@outlook.com")))
                {
                    smtpserver = new SmtpClient("smtp.live.com", 587);
                }
                else if ((sendfrom.Substring(sendfrom.IndexOf("@")) == "@icloud.com"))
                {
                    smtpserver = new SmtpClient("smtp.mail.me.com", 587);
                }
                else if ((sendfrom.Substring(sendfrom.IndexOf("@")) == "@gmail.com"))
                {
                    smtpserver = new SmtpClient("smtp.gmail.com", 587);
                }

                smtpserver.UseDefaultCredentials = false;
                smtpserver.Credentials = new NetworkCredential(sendfrom, psswd);
                smtpserver.EnableSsl = true;
                smtpserver.Send(Msg);
                Msg = null;
                smtpserver = null;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"*#*:{ex.ToString()}");
                return false;
            }

        }
        #endregion
        public static bool IsValidEmailFormat(this string email)
        {
            var emailRegex = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
            if (System.Text.RegularExpressions.Regex.IsMatch(email, emailRegex))
            {
                return true;
            }
            return false;
        }
        public static Dictionary<string, string> CreateDictionaryPair(List<string> textInputList)
        {
            var dataNLPList = new Dictionary<string, string>();
            for (var i = 0; i < textInputList.Count; i += 2)
            {
                try
                {
                    dataNLPList.Add(textInputList[i].ToUpper(), textInputList[i + 1]);
                }
                catch
                {

                }
            }
            return dataNLPList;
        }
        public static Dictionary<string, string> CreateDictionaryPair(string  textInput)
        {
            var textInputList = textInput.Split().ToList();
            var dataNLPList = new Dictionary<string, string>();
            for (var i = 0; i < textInputList.Count(); i += 2)
            {
                try
                {
                    dataNLPList.Add(textInputList[i].ToUpper(), textInputList[i + 1]);
                }
                catch
                {

                }
            }
            return dataNLPList;
        }
        public static void MapDictToControls(string[] keyword, EditText[] etArray, Dictionary<string, string> data)
        {
            var listKeywords = new List<string>(data.Keys);
            for (var index = 0; index < keyword.Length; index++) //index to iterate over keyword array
            {
                for (var keyIndex = 0; keyIndex < listKeywords.Count; keyIndex++) //kindex to iterate over dictionary key
                {
                    if (listKeywords[keyIndex].Contains(keyword[index])) //the dictionary keys seem to contain the keyword
                    {
                        data.TryGetValue(listKeywords[keyIndex], out var value); //let's extract the value
                        etArray[index].Text = value; //assign it to that control
                        listKeywords.RemoveAt(keyIndex); //the keyword has been used, so jut remove these fcking keyword
                        goto breakloop; //we found the value we're looking for, so just get out of the loop
                        //these 2 above lines is about to make the code run 2 times faster at best case, so just leave it be
                    }
                }
                breakloop:;
            }
        }
        public static bool IsNull(this string objValue)
        {
            return objValue == string.Empty;
        }
        public static bool TextFieldValidate(List<object> controlList)
        {
            foreach (var control in controlList)
            {
                if (((string)control.GetType().GetProperty("Text").GetValue(control)).IsNull())
                    return false;
            };
            return true;
        }
        public static bool CheckBoxValidate(List<object> controlList)
        {
            foreach(var control in controlList)
            {
                if (((bool)control.GetType().GetProperty("Checked").GetValue(control)) == true)
                    return true;
            }
            return false;
        }
        public static bool TimeValidate(this DateTime dt)
        {
            return dt.Hour != 0 || dt.Minute != 0;
        }

    }
}
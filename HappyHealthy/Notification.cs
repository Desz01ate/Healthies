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
using NotificationCompat = Android.Support.V4.App.NotificationCompat;
using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;
using Java.Lang;
using Android.Media;
using Android.Icu.Util;
using System.Globalization;
using Newtonsoft.Json;

namespace HappyHealthyCSharp
{
    class CustomNotification : Activity
    {
        private static readonly int ButtonClickNotificationId = 1000;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        private static NotificationCompat.Builder setNotification(Context c, string content, Type secondActivity)
        {
            var stackBuilder = TaskStackBuilder.Create(c);
            var valuesForActivity = new Bundle();
            var secondIntent = new Intent(c, secondActivity);
            stackBuilder.AddParentStack(Class.FromType(secondActivity));
            stackBuilder.AddNextIntent(secondIntent);
            var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);
            var builder = new NotificationCompat.Builder(c)
                .SetPriority((int)NotificationPriority.Max)
                .SetAutoCancel(true)
                .SetContentIntent(resultPendingIntent)
                .SetContentTitle("Happy Healthy")
                .SetSmallIcon(Resource.Drawable.iconfood)
                .SetVibrate(new long[0])
                .SetContentText(content);
            return builder;
        }
        public static void Show(Context c, string content, Type target, int customSoundResourceID = -9999)
        {
            var filePath = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
            if (customSoundResourceID != -9999)
            {
                filePath = Android.Net.Uri.Parse($@"android.resource://{c.PackageName}/{customSoundResourceID}");
            }
            var notificationManager = (NotificationManager)c.GetSystemService(Context.NotificationService);
            notificationManager.Notify(ButtonClickNotificationId, CustomNotification.setNotification(c, content, target)
                //.SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Alarm))
                //.SetSound(Android.Net.Uri.Parse(c.Resources.GetResourceName(Resource.Raw.notialert)))
                .SetSound(filePath)
                .Build());
        }
        /// <summary>
        /// Set alarm manager for system notificatiion
        /// </summary>
        /// <param name="c">Base caller</param>
        /// <param name="content">Notification string value</param>
        /// <param name="repeatDateOfWeek">Repeating date for speciific date</param>
        /// <param name="alertTime">Repeating time for specific time</param>
        /// <param name="customSoundResourceID">Resource id for sound</param>
        public static void SetAlarmManager(Context c, int requestId, string content, DateTime alertTime)
        {
            /*
            var filePath = RingtoneManager.GetDefaultUri(RingtoneType.Notification).Path;
            if (customSoundResourceID != -9999)
            {
                filePath = $@"android.resource://{c.PackageName}/{customSoundResourceID}";
            }
            var alarmIntent = new Intent(c, typeof(AlarmReceiver));
            alarmIntent.PutExtra("message", $"{content}");
            alarmIntent.PutExtra("title", "HappyHealthy");
            alarmIntent.PutExtra("sound", filePath);
            var pendingIntent = PendingIntent.GetBroadcast(c, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);
            var time = alertTime.AddSeconds(5);
            var manager = (AlarmManager)c.GetSystemService(Context.AlarmService);
            var calendar = Java.Util.Calendar.Instance;
            calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
            calendar.Set(Java.Util.CalendarField.DayOfWeek, repeatDateOfWeek);
            calendar.Set(Java.Util.CalendarField.HourOfDay, time.Hour);
            calendar.Set(Java.Util.CalendarField.Minute, time.Minute);
            calendar.Set(Java.Util.CalendarField.Second, time.Second);
            //This code snippet is worked!!!, so let's assume it's work for IntervalDay as well as halfMin
            var fiveMin =  1 * 1000;
            manager.SetRepeating(AlarmType.RtcWakeup, SystemClock.ElapsedRealtime() + calendar.TimeInMillis, fiveMin, pendingIntent);
            //reference : https://stackoverflow.com/questions/42237920/xamarin-android-how-to-schedule-and-alarm-with-a-broadcastreceiver
            */
            /*
            var calendar = Calendar.GetInstance(Android.Icu.Util.TimeZone.GetTimeZone("GMT+7"));
            //calendar.Clear(CalendarField.Date); calendar.Clear(CalendarField.Month); calendar.Clear(CalendarField.Year);
            calendar.Clear(CalendarField.Hour); calendar.Clear(CalendarField.Minute); calendar.Clear(CalendarField.Second);
            calendar.Set(CalendarField.Hour, alertTime.Hour);
            calendar.Set(CalendarField.Minute, alertTime.Minute);
            calendar.Set(CalendarField.Second, alertTime.Second);
            */
            var realId = Convert.ToInt32(Convert.ToString(requestId).Substring(0, requestId.ToString().Length - 2));
            var calendar = ToJavaCalendar(alertTime);
            var intent = new Intent(c, typeof(AlarmReceiver));
            intent.PutExtra("content", content);
            intent.PutExtra("mid",realId);
            var pendingIntent = PendingIntent.GetBroadcast(c, requestId, intent, PendingIntentFlags.UpdateCurrent);
            var am = (AlarmManager)c.GetSystemService(AlarmService);
            am.SetRepeating(AlarmType.RtcWakeup, calendar.TimeInMillis, AlarmManager.IntervalDay, pendingIntent);
        }
        public static bool CancelAlarmManager(Context c, int requestId, string content, DateTime alertTime)
        {
            var requestIdStr = requestId.ToString();
            try
            {
                var calendar = ToJavaCalendar(alertTime);
                var intent = new Intent(c, typeof(AlarmReceiver));
                intent.PutExtra("content", content);
                var pendingIntent = PendingIntent.GetBroadcast(c, Convert.ToInt32(requestIdStr+"1"), intent, PendingIntentFlags.UpdateCurrent);
                var am = (AlarmManager)c.GetSystemService(AlarmService);
                am.Cancel(pendingIntent);
                //
                calendar = ToJavaCalendar(alertTime);
                intent = new Intent(c, typeof(AlarmReceiver));
                intent.PutExtra("content", content);
                pendingIntent = PendingIntent.GetBroadcast(c, Convert.ToInt32(requestIdStr + "2"), intent, PendingIntentFlags.UpdateCurrent);
                am = (AlarmManager)c.GetSystemService(AlarmService);
                am.Cancel(pendingIntent);
                //
                calendar = ToJavaCalendar(alertTime);
                intent = new Intent(c, typeof(AlarmReceiver));
                intent.PutExtra("content", content);
                pendingIntent = PendingIntent.GetBroadcast(c, Convert.ToInt32(requestIdStr + "3"), intent, PendingIntentFlags.UpdateCurrent);
                am = (AlarmManager)c.GetSystemService(AlarmService);
                am.Cancel(pendingIntent);
                //
                calendar = ToJavaCalendar(alertTime);
                intent = new Intent(c, typeof(AlarmReceiver));
                intent.PutExtra("content", content);
                pendingIntent = PendingIntent.GetBroadcast(c, Convert.ToInt32(requestIdStr + "4"), intent, PendingIntentFlags.UpdateCurrent);
                am = (AlarmManager)c.GetSystemService(AlarmService);
                am.Cancel(pendingIntent);
                return true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }
        private static Java.Util.Calendar ToJavaCalendar(DateTime date)
        {
            Java.Util.Calendar calendar = Java.Util.Calendar.GetInstance(Java.Util.TimeZone.Default);

            //calendar.Set(date.Year, date.Month - 1, date.Day, date.Hour, date.Minute, date.Second);
            var am_pm = date.ToString("tt", CultureInfo.InvariantCulture) == "AM" ? Android.Icu.Util.Calendar.Am : Android.Icu.Util.Calendar.Pm; 
            //Ok, the above line is VERY VERY IMPORTANT PART of this functiom. Don't mess with it unless you know what you're doing.
            calendar.Set(Java.Util.CalendarField.AmPm, am_pm);
            calendar.Set(Java.Util.CalendarField.Hour, date.Hour % 12);
            calendar.Set(Java.Util.CalendarField.Minute, date.Minute);
            calendar.Set(Java.Util.CalendarField.Second, date.Second);
            return calendar;
        }
    }
    [BroadcastReceiver]
    class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            /*
            var message = intent.GetStringExtra("message");
            var title = intent.GetStringExtra("title");
            var sPath = intent.GetStringExtra("sound");
            var soundPath = Android.Net.Uri.Parse(sPath);
            var resultIntent = new Intent(context, typeof(EmptyDemo));
            resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            var pending = PendingIntent.GetActivity(context, 0,resultIntent,PendingIntentFlags.CancelCurrent);
            var builder = new Notification.Builder(context)
                             .SetContentTitle(title)
                             .SetContentText(message)
                             .SetSmallIcon(Resource.Drawable.Icon)
                             .SetSound(soundPath)
                             .SetVibrate(new long[0]);
            builder.SetContentIntent(pending);
            var notification = builder.Build();
            var manager = NotificationManager.FromContext(context);
            manager.Notify(1337, notification);
            */
            var alertContent = intent.GetStringExtra("content");
            var realId = intent.GetStringExtra("mid");
            var medObject = new MedicineTABLE().Select<MedicineTABLE>($"SELECT * From MedicineTABLE where ma_id = {realId}")[0];
            var medJson = JsonConvert.SerializeObject(medObject);
            var when = JavaSystem.CurrentTimeMillis();

            var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            var notificationIntent = new Intent(context, typeof(Medicine));
            notificationIntent.PutExtra("targetObject", medJson);
            notificationIntent.SetFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(context, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            var alarmSound = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
            var mNotifyBuilder = new NotificationCompat.Builder(context);
            mNotifyBuilder.SetSmallIcon(Resource.Drawable.logormutsb);
            mNotifyBuilder.SetContentTitle("ถึงเวลาทานยา " + alertContent);
            mNotifyBuilder.SetContentText($@"ถึงเวลาทานยา {alertContent} ({DateTime.Now.ToString("HH:mm")})").SetSound(alarmSound);
            mNotifyBuilder.SetAutoCancel(true).SetWhen(when);
            mNotifyBuilder.SetContentIntent(pendingIntent);
            mNotifyBuilder.SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 });
            notificationManager.Notify(1000, mNotifyBuilder.Build());
        }

    }
}
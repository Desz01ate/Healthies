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
using Android.Provider;
using Java.Util;
using Android.Database;

namespace HappyHealthyCSharp
{
    public class CalendarHelper
    {
        public static ContentValues GetEventContentValues(int calendarId, string title, string description, int year, int month, int date, int startHour, int endHour, string timeZone = "UTC+7")
        {
            var calId = GetProperId();
            ContentValues eventValues = new ContentValues();
            eventValues.Put(CalendarContract.Events.InterfaceConsts.CalendarId, calId);
            eventValues.Put(CalendarContract.Events.InterfaceConsts.Title, title);
            eventValues.Put(CalendarContract.Events.InterfaceConsts.Description, description);
            eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtstart, CalendarMillisecConverter(year, month, date, startHour, 0));
            eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtend, CalendarMillisecConverter(year, month, date, endHour, 0));
            eventValues.Put(CalendarContract.Events.InterfaceConsts.EventTimezone, timeZone);
            eventValues.Put(CalendarContract.Events.InterfaceConsts.EventEndTimezone, timeZone);
            //eventValues.Put(CalendarContract.Events.InterfaceConsts.AllDay, true);
            //Interface All-Day cause the ContentValues to crash on adding to calendar, avoid using it at all cost unless you have enough idea of what you are going to do.
            return eventValues;
        }

        private static int GetProperId()
        {
            var usageId = 0;
            var calendars = CalendarContract.Calendars.ContentUri;
            var projection = new[] {
                CalendarContract.Events.InterfaceConsts.Id,
                CalendarContract.Events.InterfaceConsts.CalendarDisplayName
            };
            var loader = new CursorLoader(Login.getContext(), calendars, projection, null, null, null);
            var cursor = (ICursor)loader.LoadInBackground();
            if (cursor.MoveToFirst())
            {
                do
                {
                    var id = cursor.GetColumnIndex(projection[0]);
                    var name = cursor.GetColumnIndex(projection[1]);

                    if (cursor.GetString(name).Contains("@"))
                    {
                        usageId = cursor.GetInt(id);
                        break;

                    }
                } while (cursor.MoveToNext());
                cursor.Close();
            }
            return usageId;
        }

        public static Android.Net.Uri GetDeleteEventURI(string uri, string baseEvent = "content://com.android.calendar/events")
        {
            Android.Net.Uri eventUri = Android.Net.Uri.Parse(baseEvent);
            var deleteUri = ContentUris.WithAppendedId(eventUri, Convert.ToInt32(uri.Substring(uri.LastIndexOf(@"/") + 1)));
            return deleteUri;

        }
        private static long CalendarMillisecConverter(int yr, int month, int day, int hr, int min)
        {
            Calendar c = Calendar.GetInstance(Java.Util.TimeZone.Default);

            c.Set(Java.Util.CalendarField.DayOfMonth, day);
            c.Set(Java.Util.CalendarField.HourOfDay, hr);
            c.Set(Java.Util.CalendarField.Minute, min);
            c.Set(Java.Util.CalendarField.Month, month);
            c.Set(Java.Util.CalendarField.Year, yr);

            return c.TimeInMillis;
        }

    }
}
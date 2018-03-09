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
    public class TimePickerFragment : DialogFragment, TimePickerDialog.IOnTimeSetListener
    {
        // TAG used for logging
        public static readonly string TAG = "MyTimePickerFragment";

        // Initialize handler to an empty delegate to prevent null reference exceptions:
        Action<DateTime> timeSelectedHandler = delegate { };

        // Factory method used to create a new TimePickerFragment:
        public static TimePickerFragment NewInstance(Action<DateTime> onTimeSelected)
        {
            // Instantiate a new TimePickerFragment:
            TimePickerFragment frag = new TimePickerFragment();

            // Set its event handler to the passed-in delegate:
            frag.timeSelectedHandler = onTimeSelected;

            // Return the new TimePickerFragment:
            return frag;
        }

        // Create and return a TimePickerDemo:
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            // Get the current time
            DateTime currentTime = DateTime.Now;

            // Determine whether this activity uses 24-hour time format or not:
            bool is24HourFormat = Android.Text.Format.DateFormat.Is24HourFormat(Activity);

            //Uncomment to force 24-hour time format:
            is24HourFormat = true;

            // Instantiate a new TimePickerDemo, passing in the handler, the current 
            // time to display, and whether or not to use 24 hour format:
            TimePickerDialog dialog = new TimePickerDialog
                (Activity, this, currentTime.Hour, currentTime.Minute, is24HourFormat);

            // Return the created TimePickerDemo:
            return dialog;
        }

        // Called when the user sets the time in the TimePicker: 
        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            // Get the current time:
            DateTime currentTime = DateTime.Now;

            // Create a DateTime that contains today's date and the time selected by the user:
            DateTime selectedTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hourOfDay, minute, 0);
            // Invoke the handler to update the Activity's time display to the selected time:
            timeSelectedHandler(selectedTime);
        }
    }
}
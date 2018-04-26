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
    public static class ExceptionHandlerClass
    {
        public static void IgnoreError(Action a)
        {
            try
            {
                a();
            }
            catch
            {
                //ignore;
            }
        }
    }
}
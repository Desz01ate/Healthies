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
    public sealed class Service
    {
        private Context context;
        public HHCSService.AuthHeader WebServiceAuthentication => new HHCSService.AuthHeader() {
            Username = context.GetPreference("ud_email", string.Empty),
            Password = context.GetPreference("ud_pass", string.Empty)
        };
        static readonly Service _instance = new Service(Login.getContext());
        public static Service GetInstance
        {
            get
            {
                return _instance;
            }
        }
        Service(Context c)
        {
            context = c;
        }
    }
}
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
using Java.Interop;
using Android.Speech;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;

namespace HappyHealthyCSharp
{
    [Activity(Theme = "@style/MyMaterialTheme.Base", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    class Home : Activity
    {
        TextView labelTest;
        TextView homeHeaderText;
        UserTABLE user = new UserTABLE();
        #region Experimental Section
        private bool isRecording;
        private readonly int VOICE = 10;
        private Button recButton;
        private EditText testResult;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_home);
            ImageView DiabetesButton = FindViewById<ImageView>(Resource.Id.imageView_button_diabetes);
            ImageView KidneyButton = FindViewById<ImageView>(Resource.Id.imageView_button_ckd);
            ImageView PressureButton = FindViewById<ImageView>(Resource.Id.imageView_button_pressure);
            ImageView FoodButton = FindViewById<ImageView>(Resource.Id.imageView_button_food);
            ImageView MedicineButton = FindViewById<ImageView>(Resource.Id.imageView_button_pill);
            ImageView DoctorButton = FindViewById<ImageView>(Resource.Id.imageView_button_doctor);
            ImageView DevButton = FindViewById<ImageView>(Resource.Id.imageView4);
            homeHeaderText = FindViewById<TextView>(Resource.Id.textView18);

            DiabetesButton.Click += ClickDiabetes;
            KidneyButton.Click += ClickKidney;
            PressureButton.Click += ClickPressure;
            FoodButton.Click += ClickFood;
            MedicineButton.Click += ClickMedicine;
            DoctorButton.Click += ClickDoctor;
            DevButton.Click += ClickDev;
            var imageView = FindViewById<ImageView>(Resource.Id.imageView4);
            imageView.Click += NotImplemented;
            HomeHeaderStateChange();
            //TestSTTImplementation(imageView);
        }

        private async void HomeHeaderStateChange()
        {
            var r = new Random();
            user = user.SelectOne(x => x.ud_id == Extension.getPreference("ud_id", 0, this));
            var data = new List<string>() { $"ยินดีต้อนรับกลับมา {user.ud_name}" };
            var action = new List<EventHandler>() { delegate { } };
            new DoctorTABLE().SelectAll().ForEach(x => {
                if ((x.da_date - DateTime.Now).Days < 3)
                {
                    data.Add($"มีนัดพบแพทย์ใน 3 วัน (คลิก)");
                    action.Add(delegate {
                        var jsonObject = JsonConvert.SerializeObject(x);
                        var DoctorIntent = new Intent(this, typeof(Doctor));
                        DoctorIntent.PutExtra("targetObject", jsonObject);
                        StartActivity(DoctorIntent);
                    });
                }
            });
            /*
            new DiabetesTABLE().SelectAll().ForEach(x => {
                if(x.fbs_fbs_lvl > 1)
                {
                    data.Add($"พบค่าเบาหวานผิดปกติ (คลิก)");
                    action.Add(delegate
                    {

                    });
                }
            });
            */
            while (true)
            {
                var index = r.Next(0, data.Count);
                homeHeaderText.Text = data[index];
                homeHeaderText.Click += action[index];
                await Task.Delay(30000);
            }
        }

        private void ClickDev(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(Develop)));
        }

        private void TestSTTImplementation(ImageView imageView)
        {
            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
            if (rec != "android.hardware.microphone")
            {
                // no microphone, no recording. Disable the button and output an alert
                Extension.CreateDialogue(this, "ไม่พบไมโครโฟนบนระบบของคุณ").Show();
            }
            else
                imageView.Click += delegate
                {
                    isRecording = !isRecording;
                    if (isRecording)
                    {
                        var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Speak Now!");
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                        StartActivityForResult(voiceIntent, VOICE);
                    }
                };
        }

        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            if (requestCode == VOICE)
            {
                if (resultVal == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        string textInput = matches[0];

                        // limit the output to 500 characters
                        if (textInput.Length > 500)
                            textInput = textInput.Substring(0, 500);
                        //GlobalFunction.createDialog(this, textInput).Show();
                        Toast.MakeText(this, textInput, ToastLength.Short);
                        labelTest = FindViewById<TextView>(Resource.Id.textView18);
                        labelTest.Text = textInput;
                    }
                    else
                        Toast.MakeText(this, "Unrecognized", ToastLength.Short);
                }
            }
            base.OnActivityResult(requestCode, resultVal, data);
        }

        public async void ClickFood(object sender, EventArgs e)
        {
            Extension.CreateDialogue(this, "ระบบยังไม่เปิดให้บริการ").Show();
            return;
            ProgressDialog progressDialog = null;
            try
            {
                progressDialog = ProgressDialog.Show(this, "ดาวน์โหลดข้อมูล", "กำลังดาวน์โหลดข้อมูล กรุณารอสักครู่", true);
                var service = new HHCSService.HHCSService();
                service.Timeout = 30 * 1000;
                var result = await TestConnectionValidate(this,service);
                if (result == true)
                {
                    StartActivity(new Intent(this, typeof(History_Food)));
                }
                else
                {
                    Extension.CreateDialogue(this, "เกิดความผิดพลาดในการเชื่อมต่อฐานข้อมูล").Show();
                }
            }
            catch(Exception ex)
            {
                Extension.CreateDialogue(this, ex.Message).Show();
            }
            finally
            {
                progressDialog.Dismiss();
            }
            //NotImplemented(sender, e);
        }


        public static async Task<bool> TestConnectionValidate(Context c,HHCSService.HHCSService serviceInstance)
        {
            bool result = false;
            await Task.Run(delegate {
                result = serviceInstance.TestConnection(Service.GetInstance.WebServiceAuthentication);
            });
            return result;
        }
        public void ClickDiabetes(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(History_Diabetes)));
        }
        public void ClickKidney(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(History_Kidney)));
        }
        public void ClickPressure(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(History_Pressure)));
        }
        public void ClickMedicine(object sender, EventArgs e)
        {
            var user = new UserTABLE().SelectOne(x=>x.ud_id == Extension.getPreference("ud_id", 0, this)); 
            if (!user.ud_bf_time.TimeValidate() || !user.ud_lu_time.TimeValidate() || !user.ud_dn_time.TimeValidate() || !user.ud_sl_time.TimeValidate())
            { //by using this technique a set-up time can't be an 0.00AM
                Extension.CreateDialogue(this, "กรุณาตั้งค่าเวลาทานอาหาร และเวลาเข้านอน ก่อนใช้งานการบันทึกแจ้งเตือนทานยา").Show();
                var act = (MainActivity)this.Parent;
                var th = act.TabHost;
                th.SetCurrentTabByTag("User");
            } 
            else
            {
                StartActivity(new Intent(this, typeof(History_Medicine)));
            }
        }
        public void ClickDoctor(object sender, EventArgs e)
        {
            StartActivity(typeof(History_Doctor));
        }
        public void NotImplemented(object sender, EventArgs e)
        {
            return;
        }
    }
}
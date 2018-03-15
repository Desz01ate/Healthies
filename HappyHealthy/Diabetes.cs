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
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using Android.Speech.Tts;

namespace HappyHealthyCSharp
{

    [Activity(Label = "Diabetes", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.StateHidden)]
    public class Diabetes : Activity, TextToSpeech.IOnInitListener
    {
        TextToSpeech textToSpeech;
        private readonly int CheckCode = 101, NeedLang = 103;
        Java.Util.Locale lang;
        ImageView imageView;
        TTS t2sEngine;
        EditText currentControl;
        private bool isRecording;
        private readonly int VOICE = 10;
        //Edit below
        EditText BloodValue;
        ImageView micButton, saveButton, deleteButton;
        
        DiabetesTABLE diaObject = null;
        Dictionary<string, string> dataNLPList;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_diabetes);
            BloodValue = FindViewById<EditText>(Resource.Id.sugar_value);
            micButton = FindViewById<ImageView>(Resource.Id.ic_microphone_diabetes);
            saveButton = FindViewById<ImageView>(Resource.Id.imageView_button_save_diabetes);
            var header = FindViewById<TextView>(Resource.Id.textView_header_name_diabetes);
            header.Text = "บันทึกค่าเบาหวาน";
            var addhiding = FindViewById<ImageView>(Resource.Id.ClickAddDia);
            addhiding.Visibility = ViewStates.Gone;
            var backBtt = FindViewById<ImageView>(Resource.Id.imageView38);
            backBtt.Click += delegate { Finish(); };
           //deleteButton = FindViewById<ImageView>(Resource.Id.imageView_button_delete_diabetes);
            // Create your application here
            var flagObjectJson = Intent.GetStringExtra("targetObject") ?? string.Empty;
            diaObject = string.IsNullOrEmpty(flagObjectJson) ? new DiabetesTABLE() { fbs_fbs = Extension.flagValue } : JsonConvert.DeserializeObject<DiabetesTABLE>(flagObjectJson);
            if (diaObject.fbs_fbs == Extension.flagValue)
            {
                //deleteButton.Visibility = ViewStates.Invisible;
                saveButton.Click += SaveValue;
            }
            else
            {
                InitialValueForUpdateEvent();
                saveButton.Click += UpdateValue;
                //deleteButton.Click += DeleteValue;
            }
            //end

            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
            if (rec != "android.hardware.microphone")
            {
                // no microphone, no recording. Disable the button and output an alert
                Extension.CreateDialogue(this, "ไม่พบไมโครโฟนบนระบบของคุณ").Show();
            }
            else
                micButton.Click += delegate
                {
                    isRecording = !isRecording;
                    if (isRecording)
                    {
                        AutomationTalker();
                    }
                };
            t2sEngine = new TTS(this);
        }

        private void StartMicrophone(string speakValue)
        {
            try
            {
                var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Speak Now!");
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
                voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                //t2sEngine.Speak(speakValue);
                //Thread.Sleep(1000);
                StartActivityForResult(voiceIntent, VOICE);
            }
            catch
            {
                Extension.CreateDialogue(this, "อุปกรณ์ของคุณไม่รองรับการสั่งการด้วยเสียง").Show();
            }
        }

        private void DeleteValue(object sender, EventArgs e)
        {
            Extension.CreateDialogue2(
                 this
                 , "Do you want to delete this value?"
                 , Android.Graphics.Color.White, Android.Graphics.Color.LightGreen
                 , Android.Graphics.Color.White, Android.Graphics.Color.Red
                 , Extension.adFontSize
                 , delegate
                 {
                     diaObject.Delete<DiabetesTABLE>(diaObject.fbs_id);
                     diaObject.TrySyncWithMySQL(this);
                     Finish();
                 }
                 , delegate { }
                 , "\u2713"
                 , "X");
        }

        private void InitialValueForUpdateEvent()
        {
            BloodValue.Text = diaObject.fbs_fbs.ToString();
        }

        private void UpdateValue(object sender, EventArgs e)
        {
            diaObject.fbs_fbs = (decimal)double.Parse(BloodValue.Text);
            diaObject.ud_id = Extension.getPreference("ud_id", 0, this);
            diaObject.fbs_time = DateTime.Now.ToThaiLocale();
            diaObject.Update();
            diaObject.TrySyncWithMySQL(this);
            this.Finish();
        }
        private void AutomationTalker()
        {
            currentControl = BloodValue;
            
            StartMicrophone("กรุณาบอกค่าน้ำตาล");

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
                        //var textInputList = textInput.Split().ToList();
                        //dataNLPList = Extension.CreateDictionaryPair(textInputList);
                        dataNLPList = Extension.CreateDictionaryPair(textInput);
                        Extension.MapDictToControls(new[] { "น้ำตาล" }, new[] { BloodValue }, dataNLPList);
                        Toast.MakeText(this, textInput, ToastLength.Long).Show();
                    }
                    else
                        Toast.MakeText(this, "Unrecognized value", ToastLength.Short);
                }
            }
            base.OnActivityResult(requestCode, resultVal, data);
            /*
            if (requestCode == VOICE)
            {
                if (resultVal == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        
                        Extension.CreateDialogue2(
                            this
                            , $@"ค่าน้ำตาลที่ต้องการบันทึกคือ {matches[0]}?"
                            , Android.Graphics.Color.White, Android.Graphics.Color.LightGreen
                            , Android.Graphics.Color.White, Android.Graphics.Color.Red
                            , Extension.adFontSize
                            , delegate
                            {
                                currentControl.Text = matches[0];
                            }
                            , delegate { }
                            , "\u2713"
                            , "X"
                        );
                    }
                }
            }
            base.OnActivityResult(requestCode, resultVal, data);
            */
        }



        public void SaveValue(object sender, EventArgs e)
        {
            if (!Extension.TextFieldValidate(new List<object>() {
                BloodValue
            }))
            {
                Toast.MakeText(this, "กรุณากรอกค่าให้ครบ ก่อนทำการบันทึก", ToastLength.Short).Show();
                return;
            }
            var diaTable = new DiabetesTABLE();
            try
            {
                diaTable.fbs_id = new SQLite.SQLiteConnection(Extension.sqliteDBPath).ExecuteScalar<int>($"SELECT MAX(fbs_id)+1 FROM DiabetesTABLE");
            }
            catch
            {
                diaTable.fbs_id = 1;
            }
            diaTable.fbs_fbs = (decimal)double.Parse(BloodValue.Text);
            diaTable.ud_id = Extension.getPreference("ud_id", 0, this);
            diaTable.fbs_time = DateTime.Now.ToThaiLocale();
            diaTable.Insert();
            diaTable.TrySyncWithMySQL(this);
            this.Finish();
        }

        [Export("ClickBackDiaHome")]
        public void ClickBackDiaHome(View v)
        {
            this.Finish();
        }

        #region Experiment TTS methods
        public void OnInit([GeneratedEnum] OperationResult status)
        {
            if (status == OperationResult.Error)
                textToSpeech.SetLanguage(Java.Util.Locale.Default);
            // if the listener is ok, set the lang
            if (status == OperationResult.Success)
                textToSpeech.SetLanguage(lang);
        }
        #endregion
    }
}
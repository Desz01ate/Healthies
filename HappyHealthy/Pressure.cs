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
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using Android.Speech;
using Android.Media;

namespace HappyHealthyCSharp
{
    [Activity(Label = "Pressure", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.StateHidden)]
    public class Pressure : Activity
    {
        private EditText BPLow;
        private EditText BPUp;
        private EditText HeartRate;
        private ImageView saveButton;
        private TextView header;
        private ImageView addhiding;
        private ImageView back;
        private ImageView micButton;
        PressureTABLE pressureObject;
        private bool isRecording, LetsVoiceRunning;
        private readonly int VOICE = 10;
        Dictionary<string, string> dataNLPList;

        private EditText currentControl;
        private static AutoResetEvent autoEvent = new AutoResetEvent(false);
        private bool onSaveState;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_pressure);
            var flagObjectJson = Intent.GetStringExtra("targetObject") ?? string.Empty;
            pressureObject = string.IsNullOrEmpty(flagObjectJson) ? new PressureTABLE() { bp_hr = Extension.flagValue } : JsonConvert.DeserializeObject<PressureTABLE>(flagObjectJson);

            InitializeControl();
            InitializeControlEvent();
            InitializeData();
            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
            if (rec != "android.hardware.microphone")
            {
                Extension.CreateDialogue(this, "ไม่พบไมโครโฟนบนระบบของคุณ").Show();
            }
            else
            {
                micButton.Click += delegate
                {
                    isRecording = !isRecording;
                    if (isRecording)
                    {
                        AutomationTalker();
                    }
                };
            }

            if (Extension.getPreference("autosound", false, this) && onSaveState)
                AutomationTalker();
        }

        private void InitializeControlEvent()
        {
            back.Click += delegate
            {
                LetsVoiceRunning = false;
                Finish();
            };
            if (pressureObject.bp_hr == Extension.flagValue)
            {
                //deleteButton.Visibility = ViewStates.Invisible;
                saveButton.Click += SaveValue;
                onSaveState = true;
            }
            else
            {
                InitialValueForUpdateEvent();
                saveButton.Click += UpdateValue;
                //deleteButton.Click += DeleteValue;
            }
        }

        private void InitializeData()
        {
            header.Text = "บันทึกค่าความดัน";
            addhiding.Visibility = ViewStates.Gone;
        }

        private void InitializeControl()
        {
            BPLow = FindViewById<EditText>(Resource.Id.P_costPressureDown);
            BPUp = FindViewById<EditText>(Resource.Id.P_costPressureTop);
            HeartRate = FindViewById<EditText>(Resource.Id.P_HeartRate);
            saveButton = FindViewById<ImageView>(Resource.Id.imageView_button_save_pressure);
            header = FindViewById<TextView>(Resource.Id.textView_header_name_pressure);
            addhiding = FindViewById<ImageView>(Resource.Id.imageView41);
            back = FindViewById<ImageView>(Resource.Id.imageView38);
            micButton = FindViewById<ImageView>(Resource.Id.ic_microphone_pressure);
        }

        protected override void OnPause()
        {
            base.OnPause();
            LetsVoiceRunning = false;
        }
        protected override void OnStop()
        {
            base.OnStop();
            LetsVoiceRunning = false;
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
            LetsVoiceRunning = false;
        }
        private async Task AutomationTalker()
        {
            LetsVoiceRunning = true;
            currentControl = BPUp;
            if (AllowToRun(currentControl))
                await StartMicrophoneAsync("ความดันตัวบน", Resource.Raw.pressureUp);
            currentControl = BPLow;
            if (AllowToRun(currentControl))
                await StartMicrophoneAsync("ความดันตัวล่าง", Resource.Raw.pressureDown);
            currentControl = HeartRate;
            if (AllowToRun(currentControl))
                await StartMicrophoneAsync("อัตราการเต้นของหัวใจ", Resource.Raw.heartRate);
            LetsVoiceRunning = false;
        }

        private bool AllowToRun(EditText currentControl)
        {
            return currentControl.Text == string.Empty && LetsVoiceRunning;
        }

        private async Task<bool> StartMicrophoneAsync(string speakValue, int soundRawResource)
        {
            try
            {
                //await t2sEngine.SpeakAsync($@"กรุณาบอกระดับค่า{speakValue}");
                MediaPlayer mPlayer = MediaPlayer.Create(this, soundRawResource);
                mPlayer.Start();
                mPlayer.Completion += delegate
                {
                    var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                    voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                    voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, $@"กรุณาบอกระดับค่า{speakValue}");
                    voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, "th-TH");
                    voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                    voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                    voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 50000);//15000);
                    voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
                    //voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                    //Thread.Sleep(1000);
                    StartActivityForResult(voiceIntent, VOICE);
                };
                await Task.Run(() => autoEvent.WaitOne(new TimeSpan(0, 2, 0)));
            }
            catch
            {
                Extension.CreateDialogue(this, "อุปกรณ์ของคุณไม่รองรับการสั่งการด้วยเสียง").Show();
                return false;
            }
            return true;
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
        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            base.OnActivityResult(requestCode, resultVal, data);
            if (requestCode == VOICE)
            {
                if (resultVal == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        string textInput = matches[0];
                        if (int.TryParse(textInput, out int numericData))
                        {
                            currentControl.Text = numericData.ToString();
                        }
                        else
                        {
                            currentControl.Text = "0";
                        }

                    }
                    else
                        Toast.MakeText(this, "Unrecognized value", ToastLength.Short);
                }
            }
            autoEvent.Set();
        }
        private void DeleteValue(object sender, EventArgs e)
        {
            /*
            Extension.CreateDialogue(this, "Do you want to delete this value?", delegate
            {
                pressureObject.Delete<PressureTABLE>(pressureObject.bp_id);
                PressureTABLE.TrySyncWithMySQL(this);
                Finish();
            }, delegate { }, "Yes", "No").Show();
            */
            Extension.CreateDialogue2(
                 this
                 , "ต้องการลบข้อมูลนี้หรือไม่?"
                 , Android.Graphics.Color.White, Android.Graphics.Color.LightGreen
                 , Android.Graphics.Color.White, Android.Graphics.Color.Red
                 , Extension.adFontSize
                 , delegate
                 {
                     pressureObject.Delete();
                     pressureObject.TrySyncWithMySQL(this);
                     Finish();
                 }
                 , delegate { }
                 , "\u2713"
                 , "X");
        }

        private void InitialValueForUpdateEvent()
        {
            BPLow.Text = pressureObject.bp_lo.ToString();
            BPUp.Text = pressureObject.bp_up.ToString();
            HeartRate.Text = pressureObject.bp_hr.ToString();
        }

        private void UpdateValue(object sender, EventArgs e)
        {
            pressureObject.bp_up = Convert.ToDecimal(BPUp.Text);
            pressureObject.bp_lo = Convert.ToDecimal(BPLow.Text);
            pressureObject.bp_hr = Convert.ToInt32(HeartRate.Text);
            pressureObject.Update();
            pressureObject.TrySyncWithMySQL(this);
            Finish();

        }

        public void SaveValue(object sender, EventArgs e)
        {
            if (!Extension.TextFieldValidate(new List<object>() {
                BPUp,BPLow,HeartRate
            }))
            {
                Toast.MakeText(this, "กรุณากรอกค่าให้ครบ ก่อนทำการบันทึก", ToastLength.Short).Show();
                return;
            }
            var bpTable = new PressureTABLE();
            try
            {
                bpTable.bp_id = SQLiteInstance.GetConnection.ExecuteScalar<int>($"SELECT MAX(bp_id)+1 FROM PressureTABLE");
            }
            catch
            {
                bpTable.bp_id = 1;
            }
            bpTable.bp_up = Convert.ToDecimal(BPUp.Text);
            bpTable.bp_lo = Convert.ToDecimal(BPLow.Text);
            bpTable.bp_hr = Convert.ToInt32(HeartRate.Text);
            bpTable.bp_time = DateTime.Now.ToThaiLocale();
            bpTable.ud_id = Extension.getPreference("ud_id", 0, this);
            bpTable.Insert();
            bpTable.TrySyncWithMySQL(this);
            this.Finish();

        }
        [Export("ClickBackPreHome")]
        public void ClickBackPreHome(View v)
        {
            this.Finish();
        }

    }
}
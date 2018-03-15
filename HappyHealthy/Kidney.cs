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
using Android.Speech.Tts;
using Android.Speech;

namespace HappyHealthyCSharp
{
    [Activity(Label = "Kidney", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.StateHidden)]
    public class Kidney : Activity
    {
        TextToSpeech textToSpeech;
        private readonly int CheckCode = 101, NeedLang = 103;
        Java.Util.Locale lang;
        ImageView imageView;
        TTS t2sEngine;
        EditText currentControl;
        private bool isRecording;
        private readonly int VOICE = 10;
        //Edit Below
        KidneyTABLE kidneyObject = null;
        EditText field_gfr;
        EditText field_creatinine;
        EditText field_bun;
        EditText field_sodium;
        EditText field_potassium;
        EditText field_albumin_blood;
        EditText field_albumin_urine;
        EditText field_phosphorus_blood;
        ImageView saveButton, deleteButton;
        ImageView micButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.Base_Theme_AppCompat_Light);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_kidney);
            var header = FindViewById<TextView>(Resource.Id.textView_header_name_kidey);
            header.Text = "บันทึกค่าไต";
            var addhiding = FindViewById<ImageView>(Resource.Id.imageView41);
            addhiding.Visibility = ViewStates.Gone;
            var back = FindViewById<ImageView>(Resource.Id.imageView38);
            back.Click += delegate { Finish(); };
            field_gfr = FindViewById<EditText>(Resource.Id.ckd_gfr);
            field_creatinine = FindViewById<EditText>(Resource.Id.ckd_creatinine);
            field_bun = FindViewById<EditText>(Resource.Id.ckd_bun);
            field_sodium = FindViewById<EditText>(Resource.Id.ckd_sodium);
            field_potassium = FindViewById<EditText>(Resource.Id.ckd_potassium);
            field_albumin_blood = FindViewById<EditText>(Resource.Id.ckd_albumin_blood);
            field_albumin_urine = FindViewById<EditText>(Resource.Id.ckd_albumin_urine);
            field_phosphorus_blood = FindViewById<EditText>(Resource.Id.ckd_phosphorus_blood);
            saveButton = FindViewById<ImageView>(Resource.Id.imageView_button_save_kidney);
            //deleteButton = FindViewById<ImageView>(Resource.Id.imageView_button_delete_kidney);
            micButton = FindViewById<ImageView>(Resource.Id.ic_microphone_diabetes);
            //code goes below
            var flagObjectJson = Intent.GetStringExtra("targetObject") ?? string.Empty;
            kidneyObject = string.IsNullOrEmpty(flagObjectJson) ? new KidneyTABLE() { ckd_gfr = Extension.flagValue } : JsonConvert.DeserializeObject<KidneyTABLE>(flagObjectJson);
            if (kidneyObject.ckd_gfr == Extension.flagValue)
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
                        StartMicrophone();
                    }
                };
            t2sEngine = new TTS(this);
        }
        private void StartMicrophone()
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
                 , "ต้องการลบข้อมูลนี้หรือไม่?"
                 , Android.Graphics.Color.White, Android.Graphics.Color.LightGreen
                 , Android.Graphics.Color.White, Android.Graphics.Color.Red
                 , Extension.adFontSize
                 , delegate
                 {
                     kidneyObject.Delete<KidneyTABLE>(kidneyObject.ckd_id);
                     kidneyObject.TrySyncWithMySQL(this);
                     Finish();
                 }
                 , delegate { }
                 , "\u2713"
                 , "X");
        }

        private void InitialValueForUpdateEvent()
        {
            field_gfr.Text = kidneyObject.ckd_gfr.ToString();
            field_creatinine.Text = kidneyObject.ckd_creatinine.ToString();
            field_bun.Text = kidneyObject.ckd_bun.ToString();
            field_sodium.Text = kidneyObject.ckd_sodium.ToString();
            field_potassium.Text = kidneyObject.ckd_potassium.ToString();
            field_albumin_blood.Text = kidneyObject.ckd_albumin_blood.ToString();
            field_albumin_urine.Text = kidneyObject.ckd_albumin_urine.ToString();
            field_phosphorus_blood.Text = kidneyObject.ckd_phosphorus_blood.ToString();
        }

        private void SaveValue(object sender, EventArgs e)
        {
            if (!Extension.TextFieldValidate(new List<object>() {
                field_gfr,field_creatinine,field_bun,field_sodium,field_potassium,field_albumin_blood,field_albumin_urine,field_phosphorus_blood
            }))
            {
                Toast.MakeText(this, "กรุณากรอกค่าให้ครบ ก่อนทำการบันทึก", ToastLength.Short).Show();
                return;
            }
            var kidney = new KidneyTABLE();
            try
            {
                kidney.ckd_id = new SQLite.SQLiteConnection(Extension.sqliteDBPath).ExecuteScalar<int>($"SELECT MAX(ckd_id)+1 FROM KidneyTABLE");
            }
            catch
            {
                kidney.ckd_id = 1;
            }
            kidney.ckd_gfr = Convert.ToDecimal(field_gfr.Text);
            kidney.ckd_creatinine = Convert.ToDecimal(field_creatinine.Text);
            kidney.ckd_bun = Convert.ToDecimal(field_bun.Text);
            kidney.ckd_sodium = Convert.ToDecimal(field_sodium.Text);
            kidney.ckd_potassium = Convert.ToDecimal(field_potassium.Text);
            kidney.ckd_albumin_blood = Convert.ToDecimal(field_albumin_blood.Text);
            kidney.ckd_albumin_urine = Convert.ToDecimal(field_albumin_urine.Text);
            kidney.ckd_phosphorus_blood = Convert.ToDecimal(field_phosphorus_blood.Text);
            kidney.ckd_time = DateTime.Now.ToThaiLocale();
            kidney.ud_id = Extension.getPreference("ud_id", 0, this);
            kidney.Insert();
            kidney.TrySyncWithMySQL(this);
            this.Finish();
        }



        private void UpdateValue(object sender, EventArgs e)
        {

            kidneyObject.ckd_gfr = Convert.ToDecimal(field_gfr.Text);
            kidneyObject.ckd_creatinine = Convert.ToDecimal(field_creatinine.Text);
            kidneyObject.ckd_bun = Convert.ToDecimal(field_bun.Text);
            kidneyObject.ckd_sodium = Convert.ToDecimal(field_sodium.Text);
            kidneyObject.ckd_potassium = Convert.ToDecimal(field_potassium.Text);
            kidneyObject.ckd_albumin_blood = Convert.ToDecimal(field_albumin_blood.Text);
            kidneyObject.ckd_albumin_urine = Convert.ToDecimal(field_albumin_urine.Text);
            kidneyObject.ckd_phosphorus_blood = Convert.ToDecimal(field_phosphorus_blood.Text);
            kidneyObject.ud_id = Extension.getPreference("ud_id", 0, this);
            kidneyObject.Update();
            kidneyObject.TrySyncWithMySQL(this);
            this.Finish();
        }

        [Export("ClickBackKidHome")]
        public void ClickBackKidHome(View v)
        {
            this.Finish();
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
                        var textInputList = textInput.Split().ToList();
                        var dataNLPList = new Dictionary<string, string>();
                        for (var i = 0; i < textInputList.Count; i += 2)
                        {
                            try
                            {
                                dataNLPList.Add(textInputList[i].ToUpper(), textInputList[i + 1]);
                                Console.WriteLine(textInputList[i].ToUpper());
                            }
                            catch
                            {

                            }
                        }
                        Extension.MapDictToControls(new[] {
                            "GFR","จีเอฟอาร์",
                            "Creatinine","ครีอาตินิน",
                            "BUN","บัน","บียูเอ็น",
                            "Sodium","โซเดียม",
                            "Potassium","โพแทสเซียม",
                            "Phosphorus","ฟอสฟอรัส",
                            "เลือด",
                            "ปัสสาวะ"
                        }, new[] {
                            field_gfr,field_gfr,
                            field_creatinine,field_creatinine,
                            field_bun,field_bun,
                            field_sodium,field_sodium,
                            field_potassium,field_potassium,
                            field_phosphorus_blood,field_phosphorus_blood,
                            field_albumin_blood,
                            field_albumin_urine
                        }, dataNLPList);

                    }
                    else
                        Toast.MakeText(this, "ไม่สามารถทำงานด้วยเสียงได้ในขณะนี้", ToastLength.Short);
                }
            }
            base.OnActivityResult(requestCode, resultVal, data);
        }
    }

}
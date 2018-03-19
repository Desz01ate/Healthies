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
using Java.Util;
using Android.Speech.Tts;
using Java.IO;
using System.Threading.Tasks;

namespace HappyHealthyCSharp
{
    class TTS : UtteranceProgressListener, TextToSpeech.IOnInitListener, TextToSpeech.IOnUtteranceCompletedListener
    {
        public static TTS myTTS { get; set; }
        private Context context;
        private TextToSpeech tts;
        private Locale locale = Locale.Default;
        private string enginePackageName = "com.google.android.tts";
        private string message;
        private Boolean isRunning;
        private int speakCount;

        public TTS(Context c)
        {
    
            this.context = c;
        }
        public static TTS GetInstance(Context c)
        {
            if(myTTS == null)
            {
                myTTS = new TTS(c);
            }
            return myTTS;
        }
        public async Task SpeakAsync(string message)
        {
            this.message = message;
            if (tts == null || !isRunning)
            {
                speakCount = 0;
                if (enginePackageName != null && !string.IsNullOrEmpty(enginePackageName))
                {
                    tts = new TextToSpeech(context, this, enginePackageName);
                }
                else
                {
                    tts = new TextToSpeech(context, this);
                }
                if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwichMr1)
                {
                    tts.SetOnUtteranceProgressListener(this);
                }
                else
                {
                    tts.SetOnUtteranceCompletedListener(this);
                }
                tts.SetPitch(1f);
                tts.SetSpeechRate(0.9f);
                isRunning = true;
            }
            else
            {
                await StartSpeakAsync();
            }
            await Task.Delay(3000);
        }
        public void Speak(string message)
        {
            this.message = message;
            if (tts == null || !isRunning)
            {
                speakCount = 0;
                if (enginePackageName != null && !string.IsNullOrEmpty(enginePackageName))
                {
                    tts = new TextToSpeech(context, this, enginePackageName);
                }
                else
                {
                    tts = new TextToSpeech(context, this);
                }
                if(Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwichMr1){
                    tts.SetOnUtteranceProgressListener(this);
                }
                else
                {
                    tts.SetOnUtteranceCompletedListener(this);
                }
                tts.SetPitch(1f);
                tts.SetSpeechRate(0.9f);
                isRunning = true;
            }
            else
            {
                StartSpeak();
            }
        }
        private async Task StartSpeakAsync()
        {
            speakCount++;
            if (locale != null)
            {
                tts.SetLanguage(locale);
            }
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                tts.Speak(message, QueueMode.Flush, null, "");
            }
            else
            {
                tts.Speak(message, QueueMode.Flush, null);
            }
            await Task.CompletedTask;
        }
        private void StartSpeak()
        {
            speakCount++;
            if(locale != null)
            {
                tts.SetLanguage(locale);
            }
            if(Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                tts.Speak(message, QueueMode.Flush, null, "");
            }
            else
            {
                tts.Speak(message, QueueMode.Flush, null);
            }
        }
        private void clear()
        {
            speakCount--;
            if (speakCount == 0)
            {
                tts.Shutdown();
                isRunning = false;
            }
        }
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
        public void OnInit([GeneratedEnum] OperationResult status)
        {
            if(status == OperationResult.Success)
            {
                StartSpeak();
            }
        }
        public void OnUtteranceCompleted(string utteranceId)
        {
            clear();
        }
        public override void OnDone(string utteranceId)
        {
            clear();
        }
        public override void OnError(string utteranceId)
        {
            clear();
        }
        public override void OnStart(string utteranceId)
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// Use this method to set a custom Locale for Text-to-Speech engine recognization, in most cases you don't have to custom the Locale since it will use default locale for device.
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public TTS SetLocale(Locale newLocale)
        {
            locale = newLocale;
            return this;
        }
        /// <summary>
        /// Use this method to set a custom Text-to-Speech engine, if you have no idea what are you going to do please don't call this method.
        /// </summary>
        /// <param name="engine">Engine name</param>
        /// <returns></returns>
        public TTS SetEngine(string engine)
        {
            enginePackageName = engine;
            return this;
        }

    }
}
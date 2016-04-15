using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SmartMirror.TestUCs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SpeechRecoginitionTest : Page
    {

        //SpeechSynthesizer _synthesizer;                             // The speech synthesizer (text-to-speech, TTS) object 
        SpeechRecognizer _recognizer;                               // The speech recognition object 
        //IAsyncOperation<SpeechRecognitionResult> _recoOperation;    // Used to canel the current asynchronous speech recognition operation 

        //bool _recoEnabled = false;                                  // When this is true, we will continue to recognize  

        public SpeechRecoginitionTest()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {


           // initSpeechParts();
            
        }


        private async void initSpeechParts()
        {
            try
            {
                if (_recognizer == null)
                {
                    

                    _recognizer = new SpeechRecognizer();
                    var lang= _recognizer.CurrentLanguage;
                    var functionGrammar = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "Functioncall");
                    _recognizer.UIOptions.AudiblePrompt = "Give me the name of a Function";
                    _recognizer.UIOptions.ExampleText = "Function A";
                    _recognizer.Constraints.Add(functionGrammar) ;
                    await _recognizer.CompileConstraintsAsync();
                }
                   
                try
                {
                    // Start recognition.

                    var messageDialog = new Windows.UI.Popups.MessageDialog("Give me a Function to call", "Command me");
                    var dialogAsync = messageDialog.ShowAsync();
                    SpeechRecognitionResult speechRecognitionResult = await _recognizer.RecognizeAsync();
                    dialogAsync.Cancel();
                    //seems a bit buggy, need to test a bit more
                    //SpeechRecognitionResult speechRecognitionResult = await _recognizer.RecognizeWithUIAsync();
                    var recognizedText = speechRecognitionResult.Text;
                recognizedText = removetokens(recognizedText);
                callTestfunction(recognizedText);
                }
                catch (Exception ex2)
                {
                    var ee = ex2.Message;
                    throw;
                }
                
            }
            catch
            (Exception ex)
            {
                //Check if user has approved the privacy policy
                const uint HresultPrivacyStatementDeclined = 0x80045509;
                if ((uint)ex.HResult == HresultPrivacyStatementDeclined)
                {
                    var messageDialog = new Windows.UI.Popups.MessageDialog(
                        "You must accept the speech privacy policy to continue", "Speech Exception");
                    messageDialog.ShowAsync().GetResults();
                }
            }
        }

        private string removetokens(string recognizedText)
        {
            return recognizedText.Replace("Funnction", "").Replace("Funktion", "").Replace(".","").Trim();
        }


        #region "testFunctions"
        #region"Textual Output"
        private void callTestfunction(String letter)
        {
           var upper=letter.ToUpper();
            switch (upper)
            {
                case "A":  testFunctionA(); break;
                case "B":  testFunctionB();break;
                case "C": testFunctionC();break;
                default: testFunctionNotAvailable(letter);break;
            }
        }

        
        private void testFunctionA()
        {
            showMessageBox("Function A was called", "Functioncall");
        }

        private void testFunctionB()
        {
            showMessageBox("Function B was called", "Functioncall");
        }

        private void testFunctionC()
        {
            showMessageBox("Function B was called", "Functioncall");
        }

        private void testFunctionNotAvailable(String letter)
        {
            showMessageBox("Named function, " + letter + " was not available",  "Functioncall");
        }
        
        private void showMessageBox(String message,string title)
        {
            var messageDialog = new Windows.UI.Popups.MessageDialog(
           message, "Functioncall");
            messageDialog.ShowAsync().GetResults();
        }

        #endregion

        #region "Vocal Output"

        #endregion

        #endregion

        private void button_Click(object sender, RoutedEventArgs e)
        {
            initSpeechParts();
        }
    }
}

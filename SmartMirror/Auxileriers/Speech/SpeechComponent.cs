using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using Windows.UI.Core;

namespace SmartMirror.Auxileriers.Speech
{
    class SpeechComponent
    {
        /// <summary>
        /// the HResult 0x8004503a typically represents the case where a recognizer for a particular language cannot
        /// be found. This may occur if the language is installed, but the speech pack for that language is not.
        /// See Settings -> Time & Language -> Region & Language -> *Language* -> Options -> Speech Language Options.
        /// </summary>
        private static uint HResultRecognizerNotFound = 0x8004503a;
        //SpeechSynthesizer _synthesizer;                             // The speech synthesizer (text-to-speech, TTS) object 
        SpeechRecognizer speechRecognizer;                               // The speech recognition object 
                                                                         //IAsyncOperation<SpeechRecognitionResult> _recoOperation;    // Used to canel the current asynchronous speech recognition operation 

        public delegate void CommandsGeneratedHandler(Dictionary<String, List<String>> commands);

        public delegate void SessionFinishedHandler();

        public event CommandsGeneratedHandler commandsGenerated;
        public event SessionFinishedHandler sessionsExpired;

        /// <summary>
        /// Initialize Speech Recognizer and compile constraints.
        /// </summary>
        /// <param name="recognizerLanguage">Language to use for the speech recognizer</param>
        /// <returns>Awaitable task.</returns>
        public async Task initRecognizer(Language recognizedLanguage)
        {


            if (speechRecognizer != null)
            {
                // cleanup prior to re-initializing this scenario.
                speechRecognizer.ContinuousRecognitionSession.Completed -= ContinuousRecognitionSession_Completed;
                speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
                //speechRecognizer.ContinuousRecognitionSession.AutoStopSilenceTimeout = new TimeSpan(24, 0, 0);
                speechRecognizer.StateChanged -= SpeechRecognizer_StateChanged;
                this.speechRecognizer.Dispose();
                this.speechRecognizer = null;
            }

            try
            {
                // Initialize the SpeechRecognizer and add the grammar.
                speechRecognizer = new SpeechRecognizer(recognizedLanguage);

                // Provide feedback to the user about the state of the recognizer. This can be used to provide
                // visual feedback to help the user understand whether they're being heard.
                speechRecognizer.StateChanged += SpeechRecognizer_StateChanged;

                SpeechRecognitionTopicConstraint dictation = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "command to execute");
                speechRecognizer.Constraints.Add(dictation);
                SpeechRecognitionCompilationResult compilationResult = await speechRecognizer.CompileConstraintsAsync();

                // Check to make sure that the constraints were in a proper format and the recognizer was able to compile them.
                if (compilationResult.Status != SpeechRecognitionResultStatus.Success)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to compile grammar, " + compilationResult.Status.ToString());
                }
                else
                {

                    // Set EndSilenceTimeout to give users more time to complete speaking a phrase.
                    speechRecognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromHours(24);
                    speechRecognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromHours(24);
                    speechRecognizer.Timeouts.BabbleTimeout = TimeSpan.FromHours(24);
                    // Handle continuous recognition events. Completed fires when various error states occur. ResultGenerated fires when
                    // some recognized phrases occur, or the garbage rule is hit.
                    speechRecognizer.ContinuousRecognitionSession.Completed += ContinuousRecognitionSession_Completed;
                    speechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;


                    //btnContinuousRecognize.IsEnabled = true;

                    //resultTextBlock.Text = speechResourceMap.GetValue("SRGSHelpText", speechContext).ValueAsString;
                    //resultTextBlock.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if ((uint)ex.HResult == HResultRecognizerNotFound)
                {
                    //btnContinuousRecognize.IsEnabled = false;

                    message = "Speech Language pack for selected language not installed.";
                }
                var messageDialog = new Windows.UI.Popups.MessageDialog(message, "Exception");
                await messageDialog.ShowAsync();
            }
        }



        //tbd
        private void SpeechRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            //Nothing ATM
        }

        private void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            
           commandsGenerated(extractCommands(args.Result.Text));
        }
        //endTBD
        private void ContinuousRecognitionSession_Completed(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
        {
              sessionsExpired();
        }

        public async void startSession()
        {
            await initRecognizer(SpeechRecognizer.SystemSpeechLanguage);
            // The recognizer can only start listening in a continuous fashion if the recognizer is currently idle.
            // This prevents an exception from occurring.
            if (speechRecognizer.State == SpeechRecognizerState.Idle)
            {
                // Reset the text to prompt the user.
                try
                {
                    await speechRecognizer.ContinuousRecognitionSession.StartAsync();
                }
                catch (Exception ex)
                {
                    var messageDialog = new Windows.UI.Popups.MessageDialog(ex.Message, "StartAsync Exception");
                    await messageDialog.ShowAsync();
                }
            }
            else
            {
                throw new IllegaleStateException("not idle");
            }
        }

        public async void endSession()
        {
            try
            {
                // Cancelling recognition prevents any currently recognized speech from
                // generating a ResultGenerated event. StopAsync() will allow the final session to 
                // complete.
                await speechRecognizer.ContinuousRecognitionSession.CancelAsync();
            }
            catch (Exception ex)
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog(ex.Message, "CancelAsync Exception");
                await messageDialog.ShowAsync();
            }
        }



       #region "map Commands"
        private Dictionary<String, List<string>> extractCommands(string input)
        {
            var tag = speechRecognizer.CurrentLanguage.LanguageTag;
            tag = tag.Remove(tag.IndexOf("-"));
            switch (tag.ToLower())
            {
                case "en": return extractCommandsEN(input);
                case "de":return extractCommandsDE(input);
                default: return null; 
            }           
        }
        #region "DE"
        private Dictionary<String, List<string>> extractCommandsDE(string input)
        {
            Dictionary<String, List<String>> retval = new Dictionary<String, List<String>>();
            extractMailCommandsDE(input, retval);
            extractCalenderCommandsDE(input, retval);
            return retval;
        }

        private void extractMailCommandsDE(string input, Dictionary<string, List<string>> retval)
        {

            //make it easier to filter
            input = input.ToLower();
            Regex rxShowMailList = new Regex("(zeig|gib (mir )?(alle )?(meine )?mails)");
            Regex rxParams = new Regex("(oder|und)? (von|an) ([A-Za-z0-9.@-]+ ?[A-Za-z0-9]*)");
            if (rxShowMailList.IsMatch(input))
            {
                if (!rxParams.IsMatch(input))
                    retval.Add("showMailList", null);
                else
                {
                    List<string> filters = new List<string>();
                    foreach (Match item in rxParams.Matches(input))
                    {
                        if (!(item.Groups.Count >= 3))
                        { continue; }

                        var filter = "";
                        int pointer = 1;
                        string transmittedText="";
                        //if there was an "and" or an "or" there are 4 groups
                        //else only 3
                        if (item.Groups.Count == 4)
                        {
                            switch (item.Groups[pointer].Value)
                            {
                                case "oder": transmittedText = "or ";break;
                                case "und":transmittedText = "and ";break;
                                default: transmittedText = ""; break;
                            }
                            filter = transmittedText;
                            pointer++;
                        }
                        switch (item.Groups[pointer].Value)
                        {
                            case "von": transmittedText = "from";break;
                            case "an": transmittedText = "to";break;
                            default: transmittedText = ""; break;
                        }
                        filter += transmittedText+": ";
                        filter += item.Groups[pointer + 1].Value;
                        filters.Add(filter);
                    }
                    retval.Add("showMails", filters);
                }
            }
        }


        private void extractCalenderCommandsDE(string input, Dictionary<string, List<string>> retval)
        {
            //make it easier to filter
            input = input.ToLower();
            Regex rxOpenCalender = new Regex("(zeig|zeige|gib|öffne )mir? meinen? kalender)");
            Regex rxCloseCalender = new Regex("(schließ|mach (meinen )?kalender( wieder)?( zu)?)|"+
                "kalender( wieder)? schließen");
            if (rxCloseCalender.IsMatch(input))
            { retval.Add("closeCalender", null); }
            else if (rxOpenCalender.IsMatch(input))
            { retval.Add("openCalender", null); }
        }
        #endregion

        #region "EN"

        private Dictionary<String, List<string>> extractCommandsEN(string input)
        {
            Dictionary<String, List<String>> retval = new Dictionary<String, List<String>>();
            extractMailCommandsEN(input, retval);
            return retval;
        }

        private void extractMailCommandsEN(string input, Dictionary<string, List<string>> retval)
        {

            //make it easier to filter
            input = input.ToLower();
            Regex rxShowMailList = new Regex("(show (me )?(all )?(my )?mails)");
            Regex rxParams = new Regex("(or|and)? (from|to) ([A-Za-z0-9.@-]+ ?[A-Za-z0-9]*)");
            if (rxShowMailList.IsMatch(input))
            {
                if (!rxParams.IsMatch(input))
                    retval.Add("showMailList", null);
                else
                {
                    List<string> filters = new List<string>();
                    foreach (Match item in rxParams.Matches(input))
                    {
                        if (!(item.Groups.Count >= 3))
                        { continue; }

                        var filter = "";
                        int pointer = 1;
                        //if there was an "and" or an "or" there are 4 groups
                        //else only 3
                        if (item.Groups.Count == 4)
                        {
                            filter = item.Groups[pointer].Value;
                            pointer++;
                        }
                        filter += item.Groups[pointer].Value + ": ";
                        filter += item.Groups[pointer + 1].Value;
                        filters.Add(filter);
                    }
                    retval.Add("showMails", filters);
                }
            }
        }
        private void extractCalenderCommandsEN(string input, Dictionary<string, List<string>> retval)
        {
            //make it easier to filter
            input = input.ToLower();
            Regex rxOpenCalender = new Regex("give|show|display (me )?(my )?calender)");
            Regex rxCloseCalender = new Regex("close (my )?calender( again)?");
            if (rxCloseCalender.IsMatch(input))
            { retval.Add("closeCalender", null); }
            else if (rxOpenCalender.IsMatch(input))
            { retval.Add("openCalender", null); }
        }
        #endregion



        #endregion
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

namespace Masters.UI
{
    public static class UIUtils
    {
        public static IEnumerator SlowTextLoad(this TextMeshProUGUI target, string text, float delayPerChar)
        {
            // create delay here to minimize 'new' keyword
            WaitForSeconds delay = new WaitForSeconds(delayPerChar);

            // Loop through each character, updating the text team time
            // Wait for the delay for each character
            string fullTxt = "";
            for (int i = 0; i < text.Length; i++)
            {
                fullTxt += text[i];
                target.text = fullTxt;
                yield return delay;
            }

            // Just to be sure, assign the text to the full thing at the end
            target.text = text;

            yield return null;
        }

        public static IEnumerator SlowTextUnload(this TextMeshProUGUI target, float delayPerChar) 
        {
            // create delay here to minimize 'new' keyword
            WaitForSeconds delay = new WaitForSeconds(delayPerChar);

            // Loop through each character, updating the text team time
            // Wait for the delay for each character
            string fullTxt = target.text;
            for (int i = 0; i < fullTxt.Length; i++)
            {
                fullTxt = fullTxt.Substring(0, fullTxt.Length-i);
                target.text = fullTxt;
                yield return delay;
            }

            // Just to be sure, assign the text to the full thing at the end
            target.text = "";

            yield return null;
        }

        public static IEnumerator SlowTextLoadRealtime(this TextMeshProUGUI target, string text, float delayPerChar)
        {
            // create delay here to minimize 'new' keyword
            WaitForSecondsRealtime delay = new WaitForSecondsRealtime(delayPerChar);

            // Loop through each character, updating the text team time
            // Wait for the delay for each character
            string fullTxt = "";
            for (int i = 0; i < text.Length; i++)
            {
                // Check for a richtext tag
                if (text[i] == '<')
                {
                    // Build a string until hitting end of the richtext '>'
                    string richtextBuffer = "<";
                    int j = i + 1;
                    while (text[j] != '>' && j < text.Length)
                    {
                        richtextBuffer += text[j];
                        j++;
                        yield return null;
                    }
                    richtextBuffer += '>';

                    // Apply it to the text, update the i index so it doesnt re-read it
                    fullTxt += richtextBuffer;
                    target.text = fullTxt;
                    i = j;

                    // dont wait for a delay, just keep going
                    yield return null;
                }
                else
                {
                    // If normal text, just add it to the screen
                    fullTxt += text[i];
                    target.text = fullTxt;
                    yield return delay;
                }
            }

            // Just to be sure, assign the text to the full thing at the end
            target.text = text;

            yield return null;
        }

        public static IEnumerator SlowTextUnloadRealtime(this TextMeshProUGUI target, float delayPerChar)
        {
            // create delay here to minimize 'new' keyword
            WaitForSecondsRealtime delay = new WaitForSecondsRealtime(delayPerChar);

            // Loop through each character, updating the text team time
            // Wait for the delay for each character
            string fullTxt = target.text;
            for (int i = 0; i < fullTxt.Length; i++)
            {
                fullTxt = fullTxt.Substring(0, fullTxt.Length - i);
                target.text = fullTxt;
                yield return delay;
            }

            // Just to be sure, assign the text to the full thing at the end
            target.text = "";

            yield return null;
        }
    }
}


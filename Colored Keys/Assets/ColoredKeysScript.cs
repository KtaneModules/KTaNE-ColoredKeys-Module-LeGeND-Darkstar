using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using KModkit;

public class ColoredKeysScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMBombModule BombModule;
    public KMAudio Audio;

    public KMSelectable[] buttons;
    public Renderer[] buttonmat;
    public Material[] buttonmats;
    public string[] letters;
    public string[] words;
    public TextMesh display;
    public Color[] displaycolors;

    public string[] loggingWords;

    int b1LetIndex;
    int b2LetIndex;
    int b3LetIndex;
    int b4LetIndex;
    int b1ColIndex;
    int b2ColIndex;
    int b3ColIndex;
    int b4ColIndex;
    int displayIndex;
    int displayColIndex;

    bool TLcorrect;
    bool TRcorrect;
    bool BLcorrect;
    bool BRcorrect;

    static int moduleIdCounter = 1;

    int moduleId;

    private bool moduleSolved;

    //Twitch Plays
#pragma warning disable 414
    private readonly string TwitchHelpMessage = "Type '!{0} press top left', '!{0} press tl', '!{0} top left' or '!{0} tl' to press the button in that position. Can also use numbers 1-4 instead (buttons are ordered TL, TR, BL, BR).";
#pragma warning restore 414

    public KMSelectable[] ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*(press|)\s*(top left|tl|1)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            return new KMSelectable[] { buttons[0] };
        }
        else if (Regex.IsMatch(command, @"^\s*(press|)\s*(top right|tr|2)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            return new KMSelectable[] { buttons[1] };
        }
        else if (Regex.IsMatch(command, @"^\s*(press|)\s*(bottom left|bl|3)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            return new KMSelectable[] { buttons[2] };
        }
        else if (Regex.IsMatch(command, @"^\s*(press|)\s*(bottom right|br|4)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            return new KMSelectable[] { buttons[3] };
        }
        return null;
    }

    void Awake()
    {
        moduleId = moduleIdCounter++;
        buttons[0].OnInteract += delegate () { PressedTL(); return false; };
        buttons[1].OnInteract += delegate () { PressedTR(); return false; };
        buttons[2].OnInteract += delegate () { PressedBL(); return false; };
        buttons[3].OnInteract += delegate () { PressedBR(); return false; };
    }

    void Start()
    {
        CalculateAnswer();
    }

    void CalculateAnswer()
    {
        TLcorrect = false;
        TRcorrect = false;
        BLcorrect = false;
        BRcorrect = false;

        b1LetIndex = UnityEngine.Random.Range(0, 26);
        b2LetIndex = UnityEngine.Random.Range(0, 26);
        b3LetIndex = UnityEngine.Random.Range(0, 26);
        b4LetIndex = UnityEngine.Random.Range(0, 26);
        b1ColIndex = UnityEngine.Random.Range(0, 6);
        b2ColIndex = UnityEngine.Random.Range(0, 6);
        b3ColIndex = UnityEngine.Random.Range(0, 6);
        b4ColIndex = UnityEngine.Random.Range(0, 6);
        displayIndex = UnityEngine.Random.Range(0, 6);
        displayColIndex = UnityEngine.Random.Range(0, 6);

        TextMesh TLText = buttons[0].GetComponentInChildren<TextMesh>();
        TextMesh TRText = buttons[1].GetComponentInChildren<TextMesh>();
        TextMesh BLText = buttons[2].GetComponentInChildren<TextMesh>();
        TextMesh BRText = buttons[3].GetComponentInChildren<TextMesh>();
        TLText.text = letters[b1LetIndex];
        TRText.text = letters[b2LetIndex];
        BLText.text = letters[b3LetIndex];
        BRText.text = letters[b4LetIndex];
        buttonmat[0].material = buttonmats[b1ColIndex];
        buttonmat[1].material = buttonmats[b2ColIndex];
        buttonmat[2].material = buttonmats[b3ColIndex];
        buttonmat[3].material = buttonmats[b4ColIndex];
        display.text = words[displayIndex];
        display.color = displaycolors[displayColIndex];
        Debug.LogFormat("[Colored Keys #{0}] The display says '{1}' and the color of the word is {2}.", moduleId, loggingWords[displayIndex], loggingWords[displayColIndex]);
        Debug.LogFormat("[Colored Keys #{0}] The top left key is {1} and the letter is {2}.", moduleId, buttonmats[b1ColIndex].name, letters[b1LetIndex]);
        Debug.LogFormat("[Colored Keys #{0}] The top right key is {1} and the letter is {2}.", moduleId, buttonmats[b2ColIndex].name, letters[b2LetIndex]);
        Debug.LogFormat("[Colored Keys #{0}] The bottom left key is {1} and the letter is {2}.", moduleId, buttonmats[b3ColIndex].name, letters[b3LetIndex]);
        Debug.LogFormat("[Colored Keys #{0}] The bottom right key is {1} and the letter is {2}.", moduleId, buttonmats[b4ColIndex].name, letters[b4LetIndex]);

        int TLtotal = 0;
        int TRtotal = 0;
        int BLtotal = 0;
        int BRtotal = 0;

        if (Bomb.IsIndicatorOn("MSA"))
        {
            TLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TL: A lit MSA indicator is present.", moduleId);
        }
        if (Bomb.IsIndicatorOn("SIG"))
        {
            TRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TR: A lit SIG indicator is present.", moduleId);
        }
        if (Bomb.IsIndicatorOn("NSA"))
        {
            BLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BL: A lit NSA indicator is present.", moduleId);
        }
        if (Bomb.IsIndicatorOn("CLR"))
        {
            BRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BR: A lit CLR indicator is present.", moduleId);
        }

        if (Bomb.IsIndicatorOff("CAR"))
        {
            TLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TL: An unlit CAR indicator is present.", moduleId);
        }
        if (Bomb.IsIndicatorOff("SND"))
        {
            TRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TR: An unlit SND indicator is present.", moduleId);
        }
        if (Bomb.IsIndicatorOff("TRN"))
        {
            BLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BL: An unlit TRN indicator is present.", moduleId);
        }
        if (Bomb.IsIndicatorOff("BOB"))
        {
            BRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BR: An unlit BOB indicator is present.", moduleId);
        }

        if (Bomb.GetBatteryHolderCount() % 2 != 0)
        {
            TLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TL: There is an odd number of battery holders.", moduleId);
        }
        if (Bomb.GetBatteryCount() > 3)
        {
            TRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TR: There are more than 3 batteries.", moduleId);
        }
        if (Bomb.GetBatteryCount() == 0)
        {
            BLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BL: There are no batteries.", moduleId);
        }
        if (Bomb.GetBatteryCount() % 2 == 0)
        {
            BRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BR: There is an even number of batteries.", moduleId);
        }

        if (Bomb.IsPortPresent(Port.RJ45))
        {
            TLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TL: There is an RJ-45 port.", moduleId);
        }
        if (Bomb.IsPortPresent(Port.DVI) || Bomb.IsPortPresent(Port.Parallel))
        {
            TRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TR: There is a DVI-D or parallel port.", moduleId);
        }
        if (Bomb.IsPortPresent(Port.PS2) || Bomb.IsPortPresent(Port.Serial))
        {
            BLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BL: There is a PS/2 or serial port.", moduleId);
        }
        if (Bomb.IsPortPresent(Port.StereoRCA))
        {
            BRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BR: There is a Stereo RCA port.", moduleId);
        }

        if (b1ColIndex == b2ColIndex || b1ColIndex == b3ColIndex || b1ColIndex == b4ColIndex)
        {
            TLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TL: The top left key shares its color with another key.", moduleId);
        }
        if ((b1ColIndex == b2ColIndex && b1ColIndex != b3ColIndex && b1ColIndex != b4ColIndex) || (b1ColIndex == b3ColIndex && b1ColIndex != b2ColIndex && b1ColIndex != b4ColIndex) || (b1ColIndex == b4ColIndex && b1ColIndex != b3ColIndex && b1ColIndex != b2ColIndex) || (b2ColIndex == b3ColIndex && b2ColIndex != b1ColIndex && b2ColIndex != b4ColIndex) || (b2ColIndex == b4ColIndex && b2ColIndex != b1ColIndex && b2ColIndex != b3ColIndex) || (b3ColIndex == b4ColIndex && b3ColIndex != b1ColIndex && b3ColIndex != b2ColIndex))
        {
            TRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TR: There is at least one pair of colors on the keys.", moduleId);
        }
        if ((b1ColIndex == b2ColIndex && b1ColIndex == b3ColIndex && b1ColIndex != b4ColIndex) || (b1ColIndex == b2ColIndex && b1ColIndex == b4ColIndex && b1ColIndex != b3ColIndex) || (b2ColIndex == b3ColIndex && b2ColIndex == b4ColIndex && b2ColIndex != b1ColIndex) || (b1ColIndex == b3ColIndex && b1ColIndex == b4ColIndex && b1ColIndex != b2ColIndex) || (b1ColIndex == b2ColIndex && b1ColIndex == b3ColIndex && b1ColIndex == b4ColIndex))
        {
            BLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BL: Three or more keys share the same color.", moduleId);
        }
        if (b1ColIndex != b2ColIndex && b1ColIndex != b3ColIndex && b1ColIndex != b4ColIndex && b2ColIndex != b3ColIndex && b2ColIndex != b4ColIndex && b3ColIndex != b4ColIndex)
        {
            BRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BR: All key colors are unique.", moduleId);
        }

        if (b1ColIndex == displayIndex)
        {
            TLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TL: The top left key shares its color with the displayed word.", moduleId);
        }
        if (b2ColIndex == displayIndex)
        {
            TRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TR: The top right key shares its color with the displayed word.", moduleId);
        }
        if (b3ColIndex == displayIndex)
        {
            BLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BL: The bottom left key shares its color with the displayed word.", moduleId);
        }
        if (b4ColIndex == displayIndex)
        {
            BRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BR: The bottom right key shares its color with the displayed word.", moduleId);
        }

        if (b1ColIndex == displayColIndex)
        {
            TLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TL: The top left key shares its color with the color of the displayed word.", moduleId);
        }
        if (b2ColIndex == displayColIndex)
        {
            TRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TR: The top right key shares its color with the color of the displayed word.", moduleId);
        }
        if (b3ColIndex == displayColIndex)
        {
            BLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BL: The bottom left key shares its color with the color of the displayed word.", moduleId);
        }
        if (b4ColIndex == displayColIndex)
        {
            BRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BR: The bottom right key shares its color with the color of the displayed word.", moduleId);
        }

        if (displayIndex == 0 && (b1LetIndex == 3 || b1LetIndex == 4 || b1LetIndex == 17))
        {
            TLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TL: The top left key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 1 && (b1LetIndex == 1 || b1LetIndex == 4 || b1LetIndex == 11 || b1LetIndex == 20))
        {
            TLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TL: The top left key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 2 && (b1LetIndex == 4 || b1LetIndex == 6 || b1LetIndex == 13 || b1LetIndex == 17))
        {
            TLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TL: The top left key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 3 && (b1LetIndex == 4 || b1LetIndex == 11 || b1LetIndex == 14 || b1LetIndex == 22 || b1LetIndex == 24))
        {
            TLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TL: The top left key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 4 && (b1LetIndex == 4 || b1LetIndex == 11 || b1LetIndex == 15 || b1LetIndex == 17 || b1LetIndex == 20))
        {
            TLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TL: The top left key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 5 && (b1LetIndex == 4 || b1LetIndex == 7 || b1LetIndex == 8 || b1LetIndex == 19 || b1LetIndex == 22))
        {
            TLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TL: The top left key shares its letter with a letter in the displayed word.", moduleId);
        }

        if (displayIndex == 0 && (b2LetIndex == 3 || b2LetIndex == 4 || b2LetIndex == 17))
        {
            TRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TR: The top right key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 1 && (b2LetIndex == 1 || b2LetIndex == 4 || b2LetIndex == 11 || b2LetIndex == 20))
        {
            TRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TR: The top right key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 2 && (b2LetIndex == 4 || b2LetIndex == 6 || b2LetIndex == 13 || b2LetIndex == 17))
        {
            TRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TR: The top right key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 3 && (b2LetIndex == 4 || b2LetIndex == 11 || b2LetIndex == 14 || b2LetIndex == 22 || b2LetIndex == 24))
        {
            TRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TR: The top right key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 4 && (b2LetIndex == 4 || b2LetIndex == 11 || b2LetIndex == 15 || b2LetIndex == 17 || b2LetIndex == 20))
        {
            TRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TR: The top right key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 5 && (b2LetIndex == 4 || b2LetIndex == 7 || b2LetIndex == 8 || b2LetIndex == 19 || b2LetIndex == 22))
        {
            TRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TR: The top right key shares its letter with a letter in the displayed word.", moduleId);
        }

        if (displayIndex == 0 && (b3LetIndex == 3 || b3LetIndex == 4 || b3LetIndex == 17))
        {
            BLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BL: The bottom left key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 1 && (b3LetIndex == 1 || b3LetIndex == 4 || b3LetIndex == 11 || b3LetIndex == 20))
        {
            BLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BL: The bottom left key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 2 && (b3LetIndex == 4 || b3LetIndex == 6 || b3LetIndex == 13 || b3LetIndex == 17))
        {
            BLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BL: The bottom left key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 3 && (b3LetIndex == 4 || b3LetIndex == 11 || b3LetIndex == 14 || b3LetIndex == 22 || b3LetIndex == 24))
        {
            BLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BL: The bottom left key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 4 && (b3LetIndex == 4 || b3LetIndex == 11 || b3LetIndex == 15 || b3LetIndex == 17 || b3LetIndex == 20))
        {
            BLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BL: The bottom left key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 5 && (b3LetIndex == 4 || b3LetIndex == 7 || b3LetIndex == 8 || b3LetIndex == 19 || b3LetIndex == 22))
        {
            BLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BL: The bottom left key shares its letter with a letter in the displayed word.", moduleId);
        }

        if (displayIndex == 0 && (b4LetIndex == 3 || b4LetIndex == 4 || b4LetIndex == 17))
        {
            BRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BR: The bottom right key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 1 && (b4LetIndex == 1 || b4LetIndex == 4 || b4LetIndex == 11 || b4LetIndex == 20))
        {
            BRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BR: The bottom right key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 2 && (b4LetIndex == 4 || b4LetIndex == 6 || b4LetIndex == 13 || b4LetIndex == 17))
        {
            BRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BR: The bottom right key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 3 && (b4LetIndex == 4 || b4LetIndex == 11 || b4LetIndex == 14 || b4LetIndex == 22 || b4LetIndex == 24))
        {
            BRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BR: The bottom right key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 4 && (b4LetIndex == 4 || b4LetIndex == 11 || b4LetIndex == 15 || b4LetIndex == 17 || b4LetIndex == 20))
        {
            BRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BR: The bottom right key shares its letter with a letter in the displayed word.", moduleId);
        }
        if (displayIndex == 5 && (b4LetIndex == 4 || b4LetIndex == 7 || b4LetIndex == 8 || b4LetIndex == 19 || b4LetIndex == 22))
        {
            BRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BR: The bottom right key shares its letter with a letter in the displayed word.", moduleId);
        }

        if (Bomb.GetSerialNumber().Contains(letters[b1LetIndex]))
        {
            TLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TL: The top left key shares its letter with a letter in the serial number.", moduleId);
        }
        if (Bomb.GetSerialNumber().Contains(letters[b2LetIndex]))
        {
            TRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] TR: The top right key shares its letter with a letter in the serial number.", moduleId);
        }
        if (Bomb.GetSerialNumber().Contains(letters[b3LetIndex]))
        {
            BLtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BL: The bottom left key shares its letter with a letter in the serial number.", moduleId);
        }
        if (Bomb.GetSerialNumber().Contains(letters[b4LetIndex]))
        {
            BRtotal++;
            Debug.LogFormat("[Colored Keys #{0}] BR: The bottom right key shares its letter with a letter in the serial number.", moduleId);
        }

        Debug.LogFormat("[Colored Keys #{0}] The top left key has {1} condition(s) met.", moduleId, TLtotal);
        Debug.LogFormat("[Colored Keys #{0}] The top right key has {1} condition(s) met.", moduleId, TRtotal);
        Debug.LogFormat("[Colored Keys #{0}] The bottom left key has {1} condition(s) met.", moduleId, BLtotal);
        Debug.LogFormat("[Colored Keys #{0}] The bottom right key has {1} condition(s) met.", moduleId, BRtotal);

        if (TLtotal >= TRtotal && TLtotal >= BLtotal && TLtotal >= BRtotal)
        {
            TLcorrect = true;
        }
        if (TRtotal >= TLtotal && TRtotal >= BLtotal && TRtotal >= BRtotal)
        {
            TRcorrect = true;
        }
        if (BLtotal >= TRtotal && BLtotal >= TLtotal && BLtotal >= BRtotal)
        {
            BLcorrect = true;
        }
        if (BRtotal >= TRtotal && BRtotal >= BLtotal && BRtotal >= TLtotal)
        {
            BRcorrect = true;
        }

        if ((TLcorrect && TRcorrect) || (TLcorrect && BLcorrect) || (TLcorrect && BRcorrect))
        {
            TRcorrect = false;
            BLcorrect = false;
            BRcorrect = false;
        }
        else if ((TRcorrect && TLcorrect) || (TRcorrect && BLcorrect) || (TRcorrect && BRcorrect))
        {
            TLcorrect = false;
            BLcorrect = false;
            BRcorrect = false;
        }
        else if ((BLcorrect && TRcorrect) || (BLcorrect && TLcorrect) || (BLcorrect && BRcorrect))
        {
            TRcorrect = false;
            TLcorrect = false;
            BRcorrect = false;
        }
        else if ((BRcorrect && TRcorrect) || (BRcorrect && BLcorrect) || (BRcorrect && TLcorrect))
        {
            TRcorrect = false;
            BLcorrect = false;
            BRcorrect = false;
        }

        if (TLcorrect)
        {
            Debug.LogFormat("[Colored Keys #{0}] The correct key is top left.", moduleId);
        }
        if (TRcorrect)
        {
            Debug.LogFormat("[Colored Keys #{0}] The correct key is top right.", moduleId);
        }
        if (BLcorrect)
        {
            Debug.LogFormat("[Colored Keys #{0}] The correct key is bottom left.", moduleId);
        }
        if (BRcorrect)
        {
            Debug.LogFormat("[Colored Keys #{0}] The correct key is bottom right.", moduleId);
        }
        moduleSolved = false;
    }

    void PressedTL()
    {
        if (!moduleSolved)
        {
            moduleSolved = true;
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            Debug.LogFormat("[Colored Keys #{0}] Pressed the top left key.", moduleId);
            if (TLcorrect)
            {
                GetComponent<KMBombModule>().HandlePass();
                Debug.LogFormat("[Colored Keys #{0}] Correct! Module disarmed!", moduleId);
                StartCoroutine(SolveAnimation());
            }
            else
            {
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Colored Keys #{0}] Incorrect! Resetting module.", moduleId);
                StartCoroutine(StrikeAnimation());
            }
        }
    }

    void PressedTR()
    {
        if (!moduleSolved)
        {
            moduleSolved = true;
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            Debug.LogFormat("[Colored Keys #{0}] Pressed the top right key.", moduleId);
            if (TRcorrect)
            {
                GetComponent<KMBombModule>().HandlePass();
                Debug.LogFormat("[Colored Keys #{0}] Correct! Module disarmed!", moduleId);
                StartCoroutine(SolveAnimation());
            }
            else
            {
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Colored Keys #{0}] Incorrect! Resetting module.", moduleId);
                StartCoroutine(StrikeAnimation());
            }
        }
    }

    void PressedBL()
    {
        if (!moduleSolved)
        {
            moduleSolved = true;
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            Debug.LogFormat("[Colored Keys #{0}] Pressed the bottom left key.", moduleId);
            if (BLcorrect)
            {
                GetComponent<KMBombModule>().HandlePass();
                Debug.LogFormat("[Colored Keys #{0}] Correct! Module disarmed!", moduleId);
                StartCoroutine(SolveAnimation());
            }
            else
            {
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Colored Keys #{0}] Incorrect! Resetting module.", moduleId);
                StartCoroutine(StrikeAnimation());
            }
        }
    }

    void PressedBR()
    {
        if (!moduleSolved)
        {
            moduleSolved = true;
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            Debug.LogFormat("[Colored Keys #{0}] Pressed the bottom right key.", moduleId);
            if (BRcorrect)
            {
                GetComponent<KMBombModule>().HandlePass();
                Debug.LogFormat("[Colored Keys #{0}] Correct! Module disarmed!", moduleId);
                StartCoroutine(SolveAnimation());
            }
            else
            {
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Colored Keys #{0}] Incorrect! Resetting module.", moduleId);
                StartCoroutine(StrikeAnimation());
            }
        }
    }

    IEnumerator SolveAnimation()
    {
        Audio.PlaySoundAtTransform("success", transform);
        int i = 0;
        int ranLet = 0;
        int ranCol = 0;
        display.text = "CORRECT";
        display.color = displaycolors[2];
        yield return new WaitForSeconds(0.5f);
        while (i != 20)
        {
            ranLet = UnityEngine.Random.Range(0, 6);
            ranCol = UnityEngine.Random.Range(0, 6);
            display.text = words[ranLet];
            display.color = displaycolors[ranCol];
            i++;
            yield return new WaitForSeconds(0.05f);
        }
        display.text = "";
    }

    IEnumerator StrikeAnimation()
    {
        int i = 0;
        int ranLet = 0;
        int ranCol = 0;
        display.text = "INCORRECT";
        display.color = displaycolors[0];
        yield return new WaitForSeconds(0.5f);
        while (i != 20)
        {
            ranLet = UnityEngine.Random.Range(0, 6);
            ranCol = UnityEngine.Random.Range(0, 6);
            display.text = words[ranLet];
            display.color = displaycolors[ranCol];
            i++;
            yield return new WaitForSeconds(0.05f);
        }
        CalculateAnswer();
    }
}
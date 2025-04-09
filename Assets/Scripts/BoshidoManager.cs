using System.Collections;
using System.Collections.Generic;
using UnityHawk;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class BoshidoManager : MonoBehaviour
{
    [Header("Save States")]
    public Savestate startingState;

    [Header("Score UIs")]
    public TextMeshProUGUI boshiScoreUI;
    public TextMeshProUGUI yellowYoshiScoreUI;

    //scores
    private int boshiScore;
    private int yellowYoshiScore;

    //game state
    private static bool boshiWins = false;
    private static bool yellowYoshiWins = false;
    private bool matchEnded = false;

    [SerializeField] private Emulator emulator;

    // Start is called before the first frame update
    void OnEnable()
    {
        emulator.RegisterLuaCallback("IncrementBoshiScore", IncrementBoshiScore);
        emulator.RegisterLuaCallback("IncrementYellowYoshiScore", IncrementYellowYoshiScore);
    }

    private void Update()
    {
        if (!matchEnded && (boshiWins || yellowYoshiWins))
        {
            matchEnded = true;

            if (boshiWins) ScoreBoshi();
            if (yellowYoshiWins) ScoreYellowYoshi();

            Invoke("Reload", 13);
        }
    }

    private void Reload()
    {
        emulator.LoadState(startingState);

        boshiWins = false;
        yellowYoshiWins = false;
        matchEnded = false;
    }

    private void ScoreBoshi()
    {
        //reset streak
        if (yellowYoshiScore > boshiScore)
            yellowYoshiScore = 0;

        boshiScore += 1;
        boshiScoreUI.text = "" + boshiScore;
    }

    private void ScoreYellowYoshi()
    {
        //reset streak
        if (boshiScore > yellowYoshiScore)
            boshiScore = 0;

        yellowYoshiScore += 1;
        yellowYoshiScoreUI.text = "" + yellowYoshiScore;
    }

    static string IncrementBoshiScore(string arg)
    {
        boshiWins = true;
        print("boshi is " + arg);
        return "";
    }

    static string IncrementYellowYoshiScore(string arg)
    {
        yellowYoshiWins = true;
        print("yellow yoshi is " + arg);
        return "";
    }
}

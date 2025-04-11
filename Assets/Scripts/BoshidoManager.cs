using System.Collections;
using System.Collections.Generic;
using UnityHawk;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Runtime.CompilerServices;

[ExecuteInEditMode]
public class BoshidoManager : MonoBehaviour
{
    [Header("Save States")]
    public Savestate startingState;

    [Header("Score UIs")]
    public TextMeshProUGUI boshiScoreUI;
    public TextMeshProUGUI yellowYoshiScoreUI;

    [Header("Models")]
    public Transform boshiModel;
    public Transform yellowYoshiModel;
    private Sequence currentSequence;

    [Header("Streak Settings")]
    public float animStartLength;
    public float animSpeedupFactor;
    private float currentAnimLength;

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

            if (boshiWins) Score("boshi");
            if (yellowYoshiWins) Score("yellow yoshi");

            Invoke("Reload", 14);
        }
    }

    private void Reload()
    {
        emulator.LoadState(startingState);

        boshiWins = false;
        yellowYoshiWins = false;
        matchEnded = false;
    }

    private void Score(string who)
    {
        Transform model = null;
        TextMeshProUGUI scoreUI = null;
        int score = 0;
        int dir = 1;
        if (who == "boshi")
        {
            model = boshiModel;
            scoreUI = boshiScoreUI;

            //if this score is coming from zero, they broke the other player's streak
            if (boshiScore == 0) ZeroScore("yellow yoshi");
            score = ++boshiScore;
            dir = -1;
        }
        else if (who == "yellow yoshi")
        {
            model = yellowYoshiModel;
            scoreUI = yellowYoshiScoreUI;

            //if this score is coming from zero, they broke the other player's streak
            if (yellowYoshiScore == 0) ZeroScore("boshi");
            score = ++yellowYoshiScore;
        }
        else Debug.LogError("Typo when calling Score");

        //update score ui
        scoreUI.gameObject.SetActive(true);
        scoreUI.text = score.ToString();

        //speed up animation
        currentAnimLength *= 1/animSpeedupFactor;
        Sequence currentSequence = DOTween.Sequence();
        currentSequence.SetLoops(-1);
        currentSequence.Append(model.DOLocalRotate(new Vector3(0, 360 * dir, 0), currentAnimLength, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        currentSequence.Play();

        switch (score)
        {
            case int n when n == 1:
                
                break;
        }
    }

    private void ZeroScore(string who)
    {
        //reset the anim length
        currentAnimLength = animStartLength;

        if (who == "yellow yoshi")
        {
            yellowYoshiScoreUI.gameObject.SetActive(false);
            yellowYoshiScore = 0;
            DOTween.PauseAll();
        }
        else if (who == "boshi")
        {
            boshiScoreUI.gameObject.SetActive(false);
            boshiScore = 0;
            DOTween.PauseAll();
        }
    }

    #region Unityhawk Communication

    static string IncrementBoshiScore(string arg)
    {
        boshiWins = true;
        return "";
    }

    static string IncrementYellowYoshiScore(string arg)
    {
        yellowYoshiWins = true;
        return "";
    }

    #endregion
}

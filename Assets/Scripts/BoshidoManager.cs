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

    [Header("Highest Streak")]
    public TextMeshProUGUI streakUI;
    private int highestStreak = 0;

    [Header("Effects Settings")]
    public float animStartLength;
    public float animSpeedupFactor;
    private float currentAnimLength;
    public Transform bigBoshi;
    public Transform bigYellowYoshi;
    public GameObject boshiWinPointUI;
    public GameObject yellowYoshiWinPointUI;
    public float bigAppearanceLength;
    public float bigPunchPower;

    [Header("Videos")]
    public List<GameObject> videos = new List<GameObject>();
    private int videoIndex = 0;

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

            Invoke("Reload", 3.5f);
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

            StartCoroutine("BoshiWinPoint");
        }
        else if (who == "yellow yoshi")
        {
            model = yellowYoshiModel;
            scoreUI = yellowYoshiScoreUI;

            //if this score is coming from zero, they broke the other player's streak
            if (yellowYoshiScore == 0) ZeroScore("boshi");
            score = ++yellowYoshiScore;

            StartCoroutine("YellowYoshiWinPoint");
        }
        else Debug.LogError("Typo when calling Score");

        //update score ui
        scoreUI.gameObject.SetActive(true);
        scoreUI.text = score.ToString();

        //update high score if relevant
        if (score > highestStreak)
        {
            streakUI.text = "best - " + who + " with " + score;
            highestStreak = score;
        }

        //speed up animation
        currentAnimLength *= 1/animSpeedupFactor;
        Sequence currentSequence = DOTween.Sequence();
        currentSequence.stringId = "spinning";
        currentSequence.SetLoops(-1);
        currentSequence.Append(model.DOLocalRotate(new Vector3(0, 360 * dir, 0), currentAnimLength, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        currentSequence.Play();


        //change video
        if (score == 1)
        {
            videos[videoIndex].SetActive(false);
            videoIndex = 0;
        }
        else
        {
            if (score % 5 == 0)
            {
                videos[videoIndex].SetActive(false);
                videoIndex = ++videoIndex % videos.Count;
                videos[videoIndex].SetActive(true);
            }
        }
    }

    private IEnumerator BoshiWinPoint()
    {
        yield return new WaitForSeconds(0.5f);
        bigBoshi.gameObject.SetActive(true);
        boshiWinPointUI.SetActive(true);
        bigBoshi.DOPunchPosition(new Vector3(1, 1, 1) * bigPunchPower, bigAppearanceLength, 50);
        yield return new WaitForSeconds(bigAppearanceLength);
        bigBoshi.gameObject.SetActive(false);
        boshiWinPointUI.SetActive(false);
    }

    private IEnumerator YellowYoshiWinPoint()
    {
        yield return new WaitForSeconds(0.5f);
        bigYellowYoshi.gameObject.SetActive(true);
        yellowYoshiWinPointUI.SetActive(true);
        bigYellowYoshi.DOPunchPosition(new Vector3(1, 1, 1) * bigPunchPower, bigAppearanceLength, 50);
        yield return new WaitForSeconds(bigAppearanceLength);
        bigYellowYoshi.gameObject.SetActive(false);
        yellowYoshiWinPointUI.SetActive(false);
    }

    private void ZeroScore(string who)
    {
        //reset the anim length
        currentAnimLength = animStartLength;

        if (who == "yellow yoshi")
        {
            yellowYoshiScoreUI.gameObject.SetActive(false);
            yellowYoshiScore = 0;
            DOTween.Pause("spinning");
        }
        else if (who == "boshi")
        {
            boshiScoreUI.gameObject.SetActive(false);
            boshiScore = 0;
            DOTween.Pause("spinning");
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

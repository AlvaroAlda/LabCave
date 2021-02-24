using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using KartGame.KartSystems;
using KartGame.Track;
using UnityEngine.SceneManagement;
using TMPro;

public enum GameState { Play, Won, Lost }

public class GameFlowManager : MonoBehaviour
{
    [Header("Parameters")]
    [Tooltip("Duration of the fade-to-black at the end of the game")]
    public float endSceneLoadDelay = 3f;
    [Tooltip("The canvas group of the fade-to-black screen")]
    public CanvasGroup endGameFadeCanvasGroup;

    [Header("Win")]
    [Tooltip("This string has to be the name of the scene you want to load when winning")]
    public string winSceneName = "WinScene";
    [Tooltip("Duration of delay before the fade-to-black, if winning")]
    public float delayBeforeFadeToBlack = 4f;
    [Tooltip("Duration of delay before the win message")]
    public float delayBeforeWinMessage = 2f;
    [Tooltip("Sound played on win")]
    public AudioClip victorySound;

    [Tooltip("Prefab for the win game message")]
    public DisplayMessage winDisplayMessage;

    public PlayableDirector raceCountdownTrigger;

    [Header("Lose")]
    [Tooltip("This string has to be the name of the scene you want to load when losing")]
    public string loseSceneName = "LoseScene";
    [Tooltip("Prefab for the lose game message")]
    public DisplayMessage loseDisplayMessage;

    public GameState gameState { get; private set; }

    public bool autoFindKarts = true;
    public ArcadeKart playerKart;

    [Header("GameOver PopUp")]
    public GameOverMenuController gameOverMenu;

    [Header("Coins")]
    public TextMeshProUGUI coinsTextinMenu;

    ArcadeKart[] karts;
    ObjectiveManager m_ObjectiveManager;
    TimeManager m_TimeManager;
    TimeDisplay timeDisplay;

    float m_TimeLoadEndGameScene;
    string m_SceneToLoad;
    float elapsedTimeBeforeEndScene = 0;

    float previousBestTime;
    float bestTime;

    int totalCoins;

    void Start()
    {
        if (autoFindKarts)
        {
            //karts[0] es el creado por defecto
            karts = FindObjectsOfType<ArcadeKart>();
            if (karts.Length > 0)
            {
                if (!playerKart) playerKart = karts[0];
            }
            DebugUtility.HandleErrorIfNullFindObject<ArcadeKart, GameFlowManager>(playerKart, this);
        }

        m_ObjectiveManager = FindObjectOfType<ObjectiveManager>();
        DebugUtility.HandleErrorIfNullFindObject<ObjectiveManager, GameFlowManager>(m_ObjectiveManager, this);

        m_TimeManager = FindObjectOfType<TimeManager>();
        DebugUtility.HandleErrorIfNullFindObject<TimeManager, GameFlowManager>(m_TimeManager, this);

        timeDisplay = FindObjectOfType<TimeDisplay>();
        DebugUtility.HandleErrorIfNullFindObject<TimeDisplay, GameFlowManager>(timeDisplay, this);

        AudioUtility.SetMasterVolume(1);

        winDisplayMessage.gameObject.SetActive(false);
        loseDisplayMessage.gameObject.SetActive(false);

        m_TimeManager.StopRace();

        foreach (ArcadeKart k in karts)
        {
            k.SetCanMove(false);
        }

        //Obtiene el anterior best time
        previousBestTime = PlayerPrefs.GetFloat("BestTime", -1);

        //Nuevo valor de best time
        bestTime = -1;

        //Obtiene la cantidad de monedas almacenadas durante el resto de partidas
        totalCoins = PlayerPrefs.GetInt("Coins", 0);

        //run race countdown animation
        ShowRaceCountdownAnimation();
        StartCoroutine(ShowObjectivesRoutine());

        StartCoroutine(CountdownThenStartRaceRoutine());
    }

    IEnumerator CountdownThenStartRaceRoutine()
    {
        yield return new WaitForSeconds(3f);
        StartRace();
    }

    void StartRace()
    {
        foreach (ArcadeKart k in karts)
        {
            k.SetCanMove(true);
        }
        m_TimeManager.StartRace();
    }

    void ShowRaceCountdownAnimation()
    {
        raceCountdownTrigger.Play();
    }

    IEnumerator ShowObjectivesRoutine()
    {
        while (m_ObjectiveManager.Objectives.Count == 0)
            yield return null;
        yield return new WaitForSecondsRealtime(0.2f);
        for (int i = 0; i < m_ObjectiveManager.Objectives.Count; i++)
        {
            if (m_ObjectiveManager.Objectives[i].displayMessage) m_ObjectiveManager.Objectives[i].displayMessage.Display();
            yield return new WaitForSecondsRealtime(1f);
        }
    }


    void Update()
    {
        ///BORRAR
        if (Input.GetKeyDown(KeyCode.X))
        {
            print("PLAYERPREFBSDELETED");

            PlayerPrefs.DeleteKey("BestTime");
        }
        ///

        //Asigna las monedas al texto
        coinsTextinMenu.text = karts[0].temporalCoins.ToString();

        //Se evalua los estados de win y loose
        if (gameState != GameState.Play)
        {
            elapsedTimeBeforeEndScene += Time.deltaTime;

            //Se espera al delay definido
            if (elapsedTimeBeforeEndScene >= endSceneLoadDelay)
            {
                float timeRatio = 1 - (m_TimeLoadEndGameScene - Time.time) / endSceneLoadDelay;
                endGameFadeCanvasGroup.alpha = timeRatio;

                float volumeRatio = Mathf.Abs(timeRatio);
                float volume = Mathf.Clamp(1 - volumeRatio, 0, 1);
                AudioUtility.SetMasterVolume(volume);

                ////See if it's time to load the end scene (after the delay)
                if (Time.time >= m_TimeLoadEndGameScene)
                {
                    //Activa el menu de gameOver
                    gameOverMenu.gameObject.SetActive(true);
                }
            }
        }
        else 
        {
            if (m_ObjectiveManager.AreAllObjectivesCompleted())
                EndGame(true);

            if (m_TimeManager.IsFinite && m_TimeManager.IsOver)
                EndGame(false);
        }
    }

    void EndGame(bool win)
    {
        // unlocks the cursor before leaving the scene, to be able to click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        m_TimeManager.StopRace();

        // Remember that we need to load the appropriate end scene after a delay
        gameState = win ? GameState.Won : GameState.Lost;

        //endGameFadeCanvasGroup.gameObject.SetActive(true);  // desactivo el fade, no hay que cambiar de escena

        //SE GANA
        if (win)
        {
            m_SceneToLoad = winSceneName;
            m_TimeLoadEndGameScene = Time.time + endSceneLoadDelay + delayBeforeFadeToBlack;

            // play a sound on win
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = victorySound;
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.HUDVictory);
            audioSource.PlayScheduled(AudioSettings.dspTime + delayBeforeWinMessage);

            // create a game message
            winDisplayMessage.delayBeforeShowing = delayBeforeWinMessage;
            winDisplayMessage.gameObject.SetActive(true);

            // best time of the current Session
            bestTime = timeDisplay.GetSessionBestLapTime();

            // Si has superado el tiempo previo o no hay tiempo previo registrado
            if (bestTime < previousBestTime || previousBestTime < 0)
            {
                //Se guarda el tiempo como MEJOR tiempo
                PlayerPrefs.SetFloat("BestTime", bestTime);
            }

            //Actualizo y registro las monedas
            totalCoins += karts[0].temporalCoins;
            print(totalCoins);
            PlayerPrefs.SetInt("Coins", totalCoins);
        }

        //SE PIERDE
        else
        {
            m_SceneToLoad = loseSceneName;
            m_TimeLoadEndGameScene = Time.time + endSceneLoadDelay + delayBeforeFadeToBlack;

            // create a game message
            loseDisplayMessage.delayBeforeShowing = delayBeforeWinMessage;
            loseDisplayMessage.gameObject.SetActive(true);
        }
    }
}

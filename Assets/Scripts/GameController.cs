using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    #region Sprites Stickers

    public Sprite SPRITE_stickers_0;
    public Sprite SPRITE_stickers_1;
    public Sprite SPRITE_stickers_2;
    public Sprite SPRITE_stickers_3;
    public Sprite SPRITE_stickers_4;
    public Sprite SPRITE_stickers_5;
    public Sprite SPRITE_stickers_6;
    public Sprite SPRITE_stickers_7;
    public Sprite SPRITE_stickers_8;
    public Sprite SPRITE_stickers_9;
    public Sprite SPRITE_stickers_10;
    public Sprite SPRITE_stickers_11;
    public Sprite SPRITE_stickers_12;
    public Sprite SPRITE_stickers_13;
    public Sprite SPRITE_stickers_14;
    public Sprite SPRITE_stickers_15;
    public Sprite SPRITE_stickers_16;

    #endregion

    #region AudioClip

    public AudioClip AUDIO_BeepPoints;
    public AudioClip AUDIO_BeepTimer;
    public AudioClip AUDIO_ButtonBorne;
    public AudioClip AUDIO_WhoosheSpawnPlaceHolder;
    public AudioClip AUDIO_WhoosheTimesUp;
    public AudioClip AUDIO_WhoosheMoveStickersStart;
    public AudioClip AUDIO_GameOver;
    public AudioClip AUDIO_ObjectifOK;

    #endregion

    public static int STICKERS_COUNT = 0;
    public static float TIMER_ADD_POINTS = 2;
    public static float MEMORIZE_TIME = 3f;
    public static float PLACING_TIME = 5f;
    public static float PLACEHOLDER_OPACITY = 0.188f;

    public static string MESSAGE_START_1 = "Mémorise !";
    public static string MESSAGE_START_2 = "Prêt ?";
    public static string MESSAGE_START_3 = "Partez !";
    public static string MESSAGE_END_1 = "Time's Up !";

    public static float CAMERA_GAME_SIZE = 4.23f;

    public enum GameState { Initialisation = 0, Started = 1, Ended = 2, GameOver = 3, Menu = 4};

    public GameObject m_borne;
    public GameObject m_PossiblePositions;
    public GameObject m_canvas;
    public Camera cam;
    public AudioSource BackgroundAudio;
    public float PositionStartLeft;
    public float PositionStartRight;
    public float PositionEndTop;
    public float PositionEndBottom;

    private BorneController m_borneController;
    private CanvasController m_canvasController;

    private List<GameObject> lstPossiblePositions;

    private List<GameObject> lstPossiblePositionsPicked;
    private List<GameObject> lstStickers;

    private List<Sprite> lstSprites;

    private static GameState m_state = GameState.Initialisation;
    private int m_nStickersCount = 0;

    private float m_fStickerDiameter;

    //Timer
    private bool m_bSliderDirection = true;
    private float m_fTimerDuration = 0f;
    private float m_fCurrentTimer = 0f;

    //Jeu
    private int m_nPoints = 0;
    private int m_nObjectif = 0;
    private int m_nCurrentLevel = 1;
    private float m_fTimeToAddNextLevel = 0;
    private float m_fMemorizeTime = 5;
    private float m_fPlacingTime = 5;

    private int m_nStickersMoved = 0;
    
    private int m_nLastSecondDrawn = -1;
    private bool m_nBackgroundMusicEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        lstPossiblePositions = new List<GameObject>();
        lstPossiblePositionsPicked = new List<GameObject>();
        lstStickers = new List<GameObject>();
        lstSprites = new List<Sprite>();
        InitSpriteList();

        m_nPoints = 0;
        m_state = GameState.Menu;

        m_borneController = m_borne.GetComponent<BorneController>();
        m_borneController.InitBorne();
        m_canvasController = m_canvas.GetComponent<CanvasController>();

        STICKERS_COUNT = m_PossiblePositions.transform.childCount;

        m_fStickerDiameter = m_PossiblePositions.transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x;

        for (int i = 0; i < m_PossiblePositions.transform.childCount; i++)
        {
            lstPossiblePositions.Add(m_PossiblePositions.transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && m_state == GameState.Menu)
        {
            m_state = GameState.Initialisation;
            m_nCurrentLevel = 1;
            ExitMenu();
        }

        if(Input.GetKeyDown(KeyCode.Space) && m_state == GameState.GameOver)
        {
            m_state = GameState.Initialisation;
            m_nCurrentLevel = 1;
            m_canvasController.ShowGameOverText(false, 0);
            m_canvasController.ShowGameOverText(false, 0);

            ExitMenu();
        }

        if(Input.GetKeyDown(KeyCode.Escape) && m_state == GameState.GameOver)
        {
            m_state = GameState.Menu;
            m_canvasController.ShowMenu(true);
            m_canvasController.ShowPermanentUI(false);
            m_canvasController.ShowGameOverText(false, 0);
        }

        if (m_state == GameState.Menu) return;

        if (m_fCurrentTimer > 0)
        {
            m_fCurrentTimer -= Time.deltaTime;
            UpdateUITimer();

            int nCurrentSecondDisplayed = (int)m_fCurrentTimer + 1;

            if (m_state != GameState.Started)
            {
                UpdateStartMessage();
            }
            else
            {
                UpdatePlaceHolderOpacity();
            }

            if (nCurrentSecondDisplayed <= 3 && nCurrentSecondDisplayed != m_nLastSecondDrawn)//Beep pour les 3 dernières secondes du timer
                PlaySound(AUDIO_BeepTimer);

            m_nLastSecondDrawn = nCurrentSecondDisplayed;
        }

        if (m_fCurrentTimer <= 0)
        {
            m_fCurrentTimer = 0;

            UpdateUITimer();

            if (m_state == GameState.Started)
            {
                StopGame();
            }
        }
    }

    public bool IsGameEnded()
    {
        return m_state == GameState.Ended;
    }

    public static GameState GetGameState()
    {
        return m_state;
    }

    public void StartTimer(float f, bool bDirection)
    {
        m_fCurrentTimer = m_fTimerDuration = f;
        m_bSliderDirection = bDirection;

        UpdateUITimer();
    }

    #region Game Management

    private void InitNewLevel(bool bStartGame = true)
    {
        foreach(GameObject go in lstPossiblePositionsPicked)
        {
            go.SetActive(false);
        }

        lstPossiblePositionsPicked = new List<GameObject>();

        foreach(GameObject go in lstStickers)
        {
            Destroy(go);
        }

        lstStickers = new List<GameObject>();

        if (bStartGame)
        {
            m_nCurrentLevel++;

            //Le temps de placement et de mémorisation augmente de 1 à chaque round
            m_fPlacingTime = PLACING_TIME + (m_nCurrentLevel / 2);
            m_fMemorizeTime = MEMORIZE_TIME + (m_nCurrentLevel / 2);//La temps de mémorisation augmente de 1 seconde tous les 2 niveaux

            StartGame();
        }
    }

    private void StartGame()
    {
        //Copie des positions pour pouvoir les enlever au fur et à mesure et ne pas prendre 2 positions identiques
        List<GameObject> vTemps = new List<GameObject>();
        vTemps.AddRange(lstPossiblePositions);

        m_nStickersCount = m_nCurrentLevel > lstPossiblePositions.Count ? lstPossiblePositions.Count : m_nCurrentLevel;
        m_nObjectif = GetObjectif(m_nCurrentLevel);
        m_nStickersMoved = 0;
        m_canvasController.SetLevelText(m_nCurrentLevel);

        //On prend X positions au hasard
        for (int i = 0; i < m_nStickersCount; i++)
        {
            int nRand = Random.Range(0, vTemps.Count);
            int nRandSprite = Random.Range(0, lstSprites.Count);

            GameObject goPicked = vTemps[nRand];

            goPicked.GetComponent<SpriteRenderer>().sprite = lstSprites[nRandSprite];
            lstPossiblePositionsPicked.Add(goPicked);
            goPicked.transform.localScale = new Vector3(0.24f, 0.24f, 0.24f);
            Transform tChild = goPicked.transform.GetChild(0);
            tChild.localScale = new Vector3(1f, 1f, 1f);
            tChild.GetComponent<SpriteRenderer>().sprite = lstSprites[nRandSprite];
            goPicked.transform.position = new Vector3(goPicked.transform.position.x, goPicked.transform.position.y, -0.5f);
            goPicked.GetComponent<CircleCollider2D>().radius = 1.24f;

            vTemps.RemoveAt(nRand);//Suprresion pour ne plus utiliser cette position
        }

        StartCoroutine(Coroutine_PlaySoundAfterSeconds(AUDIO_WhoosheMoveStickersStart, m_fMemorizeTime));

        foreach(GameObject goPicked in lstPossiblePositionsPicked)
        {
            GameObject go = SpawnSticker(goPicked.transform.position);
            StickerController stickerController = go.GetComponent<StickerController>();
            stickerController.SetCounted(false);
            go.transform.localScale = new Vector3(0.24f, 0.24f, 0.24f);

            //Choix de la couleur
            goPicked.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 48f/255f);
            go.GetComponent<SpriteRenderer>().sprite = goPicked.GetComponent<SpriteRenderer>().sprite;

            //Fin choix couleur

            goPicked.SetActive(true);

            lstStickers.Add(go);

            MoveStickers(go, goPicked);
        }
    }

    private void StopGame()
    {
        m_state = GameState.Ended;

        PlaySound(AUDIO_WhoosheTimesUp);
        m_canvasController.ShowEndMessage(MESSAGE_END_1);

        StartScoreCount();
    }

    private void GameOver()
    {
        PlaySound(AUDIO_GameOver);

        InitNewLevel(false);

        m_canvasController.ShowGameOverText(true, m_nPoints);
        m_canvasController.ShowPanelObjectif(false);

        m_state = GameState.GameOver;
    }

    private void StartScoreCount()
    {
        StartCoroutine(Coroutine_StickerPlaceholderSpawn());
    }

    private void ExitMenu()
    {
        m_canvasController.ShowPermanentUI(true);
        m_canvasController.ShowMenu(false);
        cam.orthographicSize = CAMERA_GAME_SIZE;

        m_nCurrentLevel = 1;
        m_fPlacingTime = PLACING_TIME;
        m_fMemorizeTime = MEMORIZE_TIME;

        StartGame();
    }

    private void MoveStickers(GameObject goSticker, GameObject goPicked)
    {
        StartTimer(m_fMemorizeTime, true);

        m_canvasController.ShowStartMessage(MESSAGE_START_1);

        int nSide = Random.Range(0, 2);

        float x = 0;
        float y = 0;

        if (nSide == 0)//Glissement à droite
        {
            x = Random.Range(PositionStartRight, PositionStartRight + 2);
        }
        else if (nSide == 1)//Glissement à gauche
        {
            x = Random.Range(PositionStartLeft, PositionStartLeft - 2);
        }

        y = Random.Range(PositionEndBottom, PositionEndTop);
        Vector3 vNewPos = new Vector3(x, y);

        StartCoroutine(Coroutine_Glissement(vNewPos, goSticker, goPicked));
    }

    #endregion

    #region Gestion/calcul des points

    private GameObject SpawnSticker(Vector3 v)
    {
        GameObject goSticker = (GameObject)GameObject.Instantiate(Resources.Load("sticker"));

        goSticker.transform.position = new Vector3(v.x, v.y, -0.5f);
        goSticker.GetComponent<StickerController>().SetGameController(this);

        return goSticker;
    }

    public float GetIntersactionStickerAndPosition(GameObject goPicked, GameObject goSticker)
    {
        float fDistance = Vector2.Distance(goPicked.transform.position, goSticker.transform.position);

        float fPercentage = 0;

        if (fDistance < m_fStickerDiameter)
            fPercentage = 100 - (fDistance / m_fStickerDiameter * 100);

        return fPercentage;
    }

    public bool IsSameSprite(GameObject goPicked, GameObject goSticker)
    {
        string sprtPicked = goPicked.GetComponent<SpriteRenderer>().sprite.name;
        string sprtSticker = goSticker.GetComponent<SpriteRenderer>().sprite.name;

        return sprtPicked.Equals(sprtSticker);
    }

    private int CountPointForPosition(GameObject goPicked, out float fPositionPoints, out float fStickerPoints)
    {
        float fPointPicked = 0;

        fPositionPoints = 0;
        fStickerPoints = 0;

        List<GameObject> lstColliding = goPicked.GetComponent<PositionPickedController>().GetCollidingList();

        if (lstColliding.Count > 0)
        {
            //S'il y a plusieurs sitcker sur la même position, on prend le sticker qui donne le plus de points
            //Pour ne pas pouvoir mettre tous les sticker à une seule position
            foreach (GameObject goCol in lstColliding)
            {
                float fPositionPointsTemp = 0;
                float fStickerPointsTemp = 0;


                StickerController stickerCtrl = goCol.GetComponent<StickerController>();

                if (stickerCtrl.IsCounted()) continue;//Si on l'a déjà compté pour une position, on le traite plus

                fPositionPointsTemp = GetIntersactionStickerAndPosition(goPicked, goCol);

                bool bIsSame = IsSameSprite(goPicked, goCol);

                if (bIsSame)
                {
                    fStickerPointsTemp += 100f;
                }

                if(fPositionPointsTemp > 50 && bIsSame)
                {
                    fPositionPointsTemp += bIsSame ? 25 : 10;
                }

                if (fPositionPointsTemp > 75 && bIsSame)
                {
                    fPositionPointsTemp += bIsSame ? 25 : 10;
                }

                if (fPositionPointsTemp > 95 && bIsSame)
                {
                    fPositionPointsTemp += bIsSame ? 50 : 20;
                }

                if (fPositionPointsTemp + fStickerPointsTemp > fPointPicked)
                {
                    fPointPicked = fPositionPointsTemp + fStickerPointsTemp;
                    fPositionPoints = fPositionPointsTemp;
                    fStickerPoints = fStickerPointsTemp;
                }

                stickerCtrl.SetCounted(true);//Ce sticker a été traité pour cette position
            }
        }

        return (int)fPointPicked;
    }

    private int GetObjectif(int nLevel)
    {
        return nLevel * 150;
    }

    #endregion

    #region Sprite

    private void InitSpriteList()
    {
        lstSprites = new List<Sprite>();
        lstSprites.Add(SPRITE_stickers_0);
        lstSprites.Add(SPRITE_stickers_1);
        lstSprites.Add(SPRITE_stickers_2);
        lstSprites.Add(SPRITE_stickers_3);
        lstSprites.Add(SPRITE_stickers_4);
        lstSprites.Add(SPRITE_stickers_5);
        lstSprites.Add(SPRITE_stickers_6);
        lstSprites.Add(SPRITE_stickers_7);
        lstSprites.Add(SPRITE_stickers_8);
        lstSprites.Add(SPRITE_stickers_9);
        lstSprites.Add(SPRITE_stickers_10);
        lstSprites.Add(SPRITE_stickers_11);
        lstSprites.Add(SPRITE_stickers_12);
        lstSprites.Add(SPRITE_stickers_13);
        lstSprites.Add(SPRITE_stickers_14);
        lstSprites.Add(SPRITE_stickers_15);
        lstSprites.Add(SPRITE_stickers_16);
    }

    private void UpdatePlaceHolderOpacity()
    {
        float fTierDuration = (m_fTimerDuration * 0.66f) - (m_fTimeToAddNextLevel / 4);

        if (m_fCurrentTimer <= fTierDuration) return;

        foreach (GameObject go in lstPossiblePositionsPicked)
        {
            GameObject goPlaceholder = go.transform.GetChild(0).gameObject;
            SpriteRenderer sprtRenderer = goPlaceholder.GetComponent<SpriteRenderer>();

            if (m_fCurrentTimer > fTierDuration)
            {
                float fValue = (m_fCurrentTimer - fTierDuration) / fTierDuration;
                fValue = fValue * PLACEHOLDER_OPACITY;

                if (fValue < 0.01)
                    fValue = 0;

                sprtRenderer.color = new Color(sprtRenderer.color.r, sprtRenderer.color.g, sprtRenderer.color.b, fValue);
            }
        }
    }

    #endregion

    #region Coroutine

    private IEnumerator Coroutine_Glissement(Vector3 vPos, GameObject goSticker, GameObject goPicked, float fDuration = 0.25f)
    {
        yield return new WaitForSeconds(m_fMemorizeTime);

        goPicked.GetComponent<SpriteRenderer>().sprite = null;

        //Smooth Lerp
        Vector3 startingPos = goSticker.transform.position;
        Vector3 finalPos = vPos;

        float elapsedTime = 0;

        while (elapsedTime < fDuration)
        {
            goSticker.transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / fDuration));

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        //------------

        m_nStickersMoved++;

        if(m_nStickersMoved == lstPossiblePositionsPicked.Count)//Si tous les stickers ont été placés, on commence le jeu
        {
            m_state = GameState.Started;
            StartTimer(m_fPlacingTime, false);
            m_canvasController.HideStartMessage();
        }
    }

    private IEnumerator Coroutine_StickerPlaceholderSpawn()
    {
        yield return new WaitForSeconds(1);

        m_canvasController.ShowPanelObjectif(true);
        m_canvasController.UpdateObjectif(m_nObjectif, 0);

        yield return new WaitForSeconds(0.5f);

        m_canvasController.HideEndMessage();
        float fPointsCourants = 0;
        bool bSoundPlayed = false;

        foreach (GameObject go in lstPossiblePositionsPicked)
        {
            Color c = go.GetComponent<SpriteRenderer>().color;

            PlaySound(AUDIO_WhoosheSpawnPlaceHolder);
            go.GetComponent<SpriteRenderer>().sprite = go.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite;
            float fOldScale = go.transform.localScale.x;
            go.transform.localScale = new Vector3(go.transform.localScale.x * 2, go.transform.localScale.y * 2, go.transform.localScale.z);

            //Mouvement du placeholder
            float fDuration = 0.5f;
            float elapsedTime = 0;

            while (elapsedTime < fDuration)
            {
                float fScale = Mathf.Lerp(go.transform.localScale.x, fOldScale, (elapsedTime / fDuration));

                go.transform.localScale = new Vector3(fScale, fScale, go.transform.localScale.z);

                elapsedTime += Time.deltaTime;

                yield return null;
            }
            //---

            go.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, 48f/255f);

            float fPosition = 0;
            float fSticker = 0;
            float fScore = CountPointForPosition(go, out fPosition, out fSticker);

            fPointsCourants += fScore;
            m_nPoints += (int)fScore;
            m_canvasController.UpdateScore(m_nPoints);

            if(fPointsCourants > m_nObjectif && !bSoundPlayed)
            {
                PlaySound(AUDIO_ObjectifOK);
                m_canvasController.DisplayGreenCross(true);
                bSoundPlayed = true;
            }

            DisplayPoints(go, fPosition, fSticker);

            m_canvasController.UpdateObjectif(m_nObjectif, (int)fPointsCourants);

            yield return new WaitForSeconds(0.5f);//On attend 1 seconde avec de passer à la prochaine position
        }

        if (fPointsCourants < m_nObjectif)//Moins de 50% des points max possible -> GameOver
        {
            GameOver();
        }
        else
        {
            m_canvasController.ShowPanelObjectif(false);
            m_canvasController.DisplayGreenCross(false);

            m_canvasController.ShowNewLevelText(true, GetObjectif(m_nCurrentLevel + 1), m_nCurrentLevel + 1);

            PlaySound(AUDIO_WhoosheTimesUp);

            yield return new WaitForSeconds(1f);

            m_canvasController.ShowNewLevelText(false, 0, 0);

            InitNewLevel();
        }
    }

    private IEnumerator Coroutine_MoveText(Text text, Vector3 vNewPos, float fNewScale, float fDuration = 0.3f)
    {
        Vector3 startingPos = text.transform.position;
        Vector3 finalPos = vNewPos;

        float elapsedTime = 0;

        while (elapsedTime < fDuration)
        {
            text.transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / fDuration));

            float fScale = Mathf.Lerp(1f, fNewScale, (elapsedTime / fDuration));
            text.transform.localScale = new Vector3(fScale, fScale, text.transform.localScale.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        //Disparition
        fDuration = 0.3f;
        elapsedTime = 0f;

        while (elapsedTime < fDuration)
        {
            float fOpacity = Mathf.Lerp(1f, 0f, (elapsedTime / fDuration));

            text.color = new Color(text.color.r, text.color.g, text.color.b, fOpacity);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator Coroutine_PlaySoundAfterSeconds(AudioClip audio, float fTime)
    {
        yield return new WaitForSeconds(fTime);

        PlaySound(audio);
    }

    #endregion

    #region UI

    public void UpdateUITimer()
    {
        m_canvasController.UpdateTimer(m_fCurrentTimer, m_fTimerDuration, m_bSliderDirection);
    }

    public void UpdateStartMessage()
    {
        if (m_fCurrentTimer < 3 && m_fCurrentTimer >= 1f &&  m_canvasController.GetStartMessage() != MESSAGE_START_2)
            m_canvasController.ShowStartMessage(MESSAGE_START_2);
        else if(m_fCurrentTimer < 1f && m_canvasController.GetStartMessage() != MESSAGE_START_3)
            m_canvasController.ShowStartMessage(MESSAGE_START_3);
    }

    public void DisplayPoints(GameObject goPicked, float fPositionPoints, float fStickerPoints)
    {
        Vector3 vScreenPos = cam.WorldToScreenPoint(goPicked.transform.position);

        if (fPositionPoints > 0)
        {
            GameObject goTextPosition = (GameObject)GameObject.Instantiate(Resources.Load("AddPointsPositionDisplay 1"));

            goTextPosition.transform.position = new Vector3(vScreenPos.x, vScreenPos.y + 20, 0);
            Text txtPosition = goTextPosition.GetComponent<Text>();
            txtPosition.text = "Position : +" + (int)fPositionPoints + " points";
            goTextPosition.transform.SetParent(m_canvas.transform);

            Vector3 vNew = new Vector3(goTextPosition.transform.position.x - 150f, goTextPosition.transform.position.y + 50, goTextPosition.transform.position.z);
            StartCoroutine(Coroutine_MoveText(txtPosition, vNew, 2f));
        }

        if (fStickerPoints > 0)
        {
            GameObject goTextSticker = (GameObject)GameObject.Instantiate(Resources.Load("AddPointsStrickerDisplay"));
            goTextSticker.transform.position = new Vector3(vScreenPos.x, vScreenPos.y + 20, 0);
            Text txtSticker = goTextSticker.GetComponent<Text>();
            txtSticker.text = "Sticker : +" + (int)fStickerPoints + " points";
            goTextSticker.transform.SetParent(m_canvas.transform);

            Vector3 vNew = new Vector3(goTextSticker.transform.position.x + 150f, goTextSticker.transform.position.y + 80, goTextSticker.transform.position.z);
            StartCoroutine(Coroutine_MoveText(txtSticker, vNew, 2f));
        }

        PlaySound(AUDIO_BeepPoints);
    }

    public void StopButtonClicked()
    {
        PlaySound(AUDIO_ButtonBorne);

        m_fTimeToAddNextLevel = m_fCurrentTimer;
        m_fCurrentTimer = 0;
    }

    #endregion

    #region Son

    public void PlaySound(AudioClip audioClip)
    {
        AudioSource.PlayClipAtPoint(audioClip, Vector3.zero);
    }

    public void EnableMusic(bool bEnabled)
    {
        m_nBackgroundMusicEnabled = !m_nBackgroundMusicEnabled;

        if (m_nBackgroundMusicEnabled)
            BackgroundAudio.volume = 0.1f;
        else
            BackgroundAudio.volume = 0f;

        m_canvasController.SetAudioEnable(m_nBackgroundMusicEnabled);
    }

    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Current { get; private set; }

    public static Color[] colors =
    {
        Color.cyan,
        Color.blue,
        Color.red,
        Color.yellow,
        Color.green,
        new Color(1, 0.64f, 0, 1),
        new Color(0.54f, 0.16f, 0.88f, 1),
    };

    public event Action<bool> OnFewerStateChanged;

    public SnakeHead player;
    [SerializeField]
    private CameraController camController;
    [SerializeField]
    private TMP_Text gemsCountText, foodCountText, GameOverText;
    [SerializeField]
    private ParticleSystem gemsParticles, foodParticles;
    [SerializeField]
    private GameObject segmentPrefab, startSegment, popUpTextPrefab, textDummy, gameOverScreen;
    [SerializeField]
    private MenuManager menuManager;
    [SerializeField]
    private float fewerDuration = 5f;
    [SerializeField]
    private bool needSpawn = false;

    private Coroutine fewerCoroutine, gemTextBlop, foodTextBlop;
    private Queue<GameObject> Segments;

    private int gemsCount, foodCount;
    private readonly int gemsToFewer = 3;
    private float nextSpawn;

    private void Awake()
    {
        Segments = new Queue<GameObject>();
        Current = this;
        nextSpawn = startSegment.transform.position.z;
        Segments.Enqueue(startSegment);
        StartCoroutine(StartGame());
    }
    private void Gameover()
    {
        player.GetComponent<PlayerMovement>().enabled = false;
        GameOverText.text = foodCount.ToString();
        camController.curMode = CameraMode.RotateAround;
        gameOverScreen.SetActive(true);
    }

    private void Update()
    {
        if (needSpawn)
        {
            //Не входило в тестовое, но тут все готово для бесконечного спавна дороги
            if (player.transform.position.z >= nextSpawn)
            {
                if (Segments.Count > 1)
                {
                    Destroy(Segments.Dequeue());
                }
                nextSpawn += 50;
                Segments.Enqueue(Instantiate(segmentPrefab, new Vector3(0, 0, nextSpawn), Quaternion.identity));
            }
        }
    }
    private void UpdateGemsUI()
    {
        gemsCountText.text = gemsCount.ToString();
        if (gemTextBlop != null) 
        { 
            StopCoroutine(gemTextBlop);
        }
        gemTextBlop = StartCoroutine(TextBlop(gemsCountText.transform));
        gemsParticles.Emit(5);
    }
    private void GemEaten()
    {
        gemsCount++;
        if (gemsCount >= gemsToFewer && fewerCoroutine == null)
        {
            fewerCoroutine = StartCoroutine(fewerMode());
        }
        UpdateGemsUI();
    }
    private void UpdateFoodUI()
    {
        foodCountText.text = foodCount.ToString();
        Instantiate(popUpTextPrefab, textDummy.transform);
        if (foodTextBlop != null)
        {
            StopCoroutine(foodTextBlop);
        }
        foodTextBlop = StartCoroutine(TextBlop(foodCountText.transform));
        foodParticles.Emit(5);
    }
    private void FoodEaten()
    {
        foodCount++;
        if (foodCount % 10 == 0)
        {
            player.AddTailNode();
        }
        UpdateFoodUI();
    }
    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1.5f);
        camController.curMode = CameraMode.Follow;

        yield break;
    }
    private IEnumerator TextBlop(Transform target)
    {
        target.localScale = Vector3.one;
        target.localScale *= 1.4f;
        yield return new WaitForSeconds(0.2f);
        target.localScale = Vector3.one;
        yield break;
    }
    private IEnumerator fewerMode()
    {
        OnFewerStateChanged?.Invoke(true);
        yield return new WaitForSeconds(fewerDuration);
        OnFewerStateChanged?.Invoke(false);
        gemsCount = 0;
        UpdateGemsUI();
        fewerCoroutine = null;

        yield break;
    }

    public void Restart()
    {
        menuManager.ReloadLevel();
    }
    private void OnDestroy()
    {
        Current = null;
    }
    private void OnEnable()
    {
        player.OnEatFood += FoodEaten;
        player.OnEatGems += GemEaten;
        player.OnEatBad += Gameover;
    }
    private void OnDisable()
    {
        player.OnEatFood -= FoodEaten;
        player.OnEatGems -= GemEaten;
        player.OnEatBad -= Gameover;
    }
}
public enum SegmentColor
{
    BrightBlue,
    DarkBlue,
    Red,
    Yellow,
    Green,
    Orange,
    Purple
}
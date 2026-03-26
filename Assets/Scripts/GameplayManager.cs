using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameplayManager : MonoBehaviour
{
    public TMP_Text timerText;
    public TMP_Text coinText;
    public TMP_Text customersText;

    public TMP_Text countdownText;

    public GameObject pausePanel;
    public GameObject winPanel;
    public GameObject losePanel;

    public CustomerSpawner spawner;
    public RecipeBook recipeBook;

    [Header("Fallback Time (Difficulty)")]
    public float easyTime = 300f;
    public float mediumTime = 240f;
    public float hardTime = 180f;

    [Header("Per-Level Settings")]
    public int[] customersPerLevel = { 5, 8, 11, 14, 17, 20 };
    public float[] timeSecondsPerLevel = { 180f, 170f, 160f, 150f, 140f, 130f };

    public float basePatienceSeconds = 30f;
    public float easyPatience = 1f;
    public float mediumPatience = 0.8f;
    public float hardPatience = 0.6f;

    int totalCustomers = 5;
    int served;
    int lost;
    int coins;
    float timeLeft;
    float patienceSeconds;
    bool gameOver;
    bool gameStarted;
    int currentLevel;

    float levelTime;

    public int CustomersTotal => totalCustomers;
    public float PatienceSeconds => patienceSeconds;
    public bool IsGameRunning => gameStarted && !gameOver;
    public int Coins => coins;

    void Start()
    {
        int difficulty = PlayerPrefs.GetInt("Difficulty", 0);
        currentLevel = PlayerPrefs.GetInt("SelectedLevel", 1);

        float levelTimeSeconds = GetLevelTimeSeconds(currentLevel);
        if (levelTimeSeconds > 0f)
            timeLeft = levelTimeSeconds;
        else
            timeLeft = difficulty == 0 ? easyTime : (difficulty == 1 ? mediumTime : hardTime);

        levelTime = timeLeft;
        float patienceMult = difficulty == 0 ? easyPatience : (difficulty == 1 ? mediumPatience : hardPatience);

        patienceSeconds = basePatienceSeconds * patienceMult;

        int levelCustomers = GetLevelCustomerCount(currentLevel);
        if (levelCustomers > 0)
            totalCustomers = levelCustomers;

        spawner.totalCustomers = totalCustomers;
        spawner.Init(this);

        UpdateUI();

        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        gameStarted = false;
        Time.timeScale = 0f;

        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        float t = 3f;
        while (t > 0f)
        {
            if (countdownText != null)
                countdownText.text = Mathf.CeilToInt(t).ToString();

            AudioManager.Instance?.PlayCountdownTick();

            yield return new WaitForSecondsRealtime(1f);
            t -= 1f;
        }

        if (countdownText != null)
            countdownText.text = "GO!";

        AudioManager.Instance?.PlayCountdownGo();

        yield return new WaitForSecondsRealtime(0.5f);

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        Time.timeScale = 1f;
        gameStarted = true;
    }

    void Update()
    {
        if (gameOver || !gameStarted) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            LoseGame();
        }

        UpdateUI();
    }

    public void RegisterCustomer(Customer customer, CustomerSpawner sp)
    {
        customer.gameObject.AddComponent<CustomerTracker>().Init(sp);
    }

    public void OnDishServed(FoodType food)
    {
        int earned = GetCoinsForFood(food);
        coins += earned;
        CoinBank.Add(earned);
    }

    public void OnCustomerServed()
    {
        served++;
        if (served >= totalCustomers)
            WinGame();
    }

    public FoodType GetRandomOrderFood()
    {
        int max = System.Enum.GetValues(typeof(FoodType)).Length;
        return (FoodType)Random.Range(0, max);
    }

    public void OnCustomerFailed()
    {
        lost++;
        LoseGame();
    }

    void WinGame()
    {
        if (gameOver) return;
        gameOver = true;

        int stars = 2;
        float timePercent = levelTime > 0f ? timeLeft / levelTime : 0f;
        if (timePercent >= 0.30f)
            stars = 3;

        int level = PlayerPrefs.GetInt("SelectedLevel", 1);
        LevelSelectManager.SaveLevelResult(level, stars);

        if (winPanel != null)
        {
            var win = winPanel.GetComponent<WinPanelController>();
            if (win != null)
                win.Show(coins, 0, stars);
            else
                winPanel.SetActive(true);

            Time.timeScale = 0f;
        }
    }

    void LoseGame()
    {
        if (gameOver) return;
        gameOver = true;

        if (losePanel != null)
        {
            var lose = losePanel.GetComponent<LosePanelController>();
            if (lose != null)
            {
                int customersLost = totalCustomers - served;
                lose.Show(customersLost, timeLeft, coins);
            }
            else
            {
                losePanel.SetActive(true);
            }

            Time.timeScale = 0f;
        }
    }

    void UpdateUI()
    {
        if (timerText != null)
            timerText.text = $"{Mathf.CeilToInt(timeLeft / 60f):00}:{Mathf.CeilToInt(timeLeft % 60f):00}";
        if (coinText != null)
            coinText.text = coins.ToString();
        if (customersText != null)
            customersText.text = $"{served}/{totalCustomers}";
    }

    int GetCoinsForFood(FoodType food)
    {
        int baseCoins = 10;
        if (recipeBook != null)
            baseCoins = recipeBook.GetRecipe(food).baseCoins;

        return baseCoins + UpgradeState.GetPriceBonus(food);
    }

    int GetLevelCustomerCount(int level)
    {
        if (customersPerLevel == null || customersPerLevel.Length == 0) return 0;
        int index = Mathf.Clamp(level - 1, 0, customersPerLevel.Length - 1);
        return customersPerLevel[index];
    }

    float GetLevelTimeSeconds(int level)
    {
        if (timeSecondsPerLevel == null || timeSecondsPerLevel.Length == 0) return 0f;
        int index = Mathf.Clamp(level - 1, 0, timeSecondsPerLevel.Length - 1);
        return timeSecondsPerLevel[index];
    }

    public void Pause()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public bool TryServeGarnish(IngredientType garnish)
    {
        if (spawner == null) return false;
        return spawner.TryServeGarnish(garnish);
    }

    public List<IngredientType> GetSisigGarnishOptions()
    {
        if (recipeBook == null) return new List<IngredientType>();
        return recipeBook.GetGarnish(FoodType.Sisig, currentLevel);
    }
}
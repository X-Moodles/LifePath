using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text statsText;   // 显示：价值、负重、压力
    public TMP_Text timeText;    // 显示：倒计时、年龄
    public TMP_Text gameOverText;// 游戏结束面板 (默认隐藏)
    public GameObject gameOverPanel;

    [Header("Panels")]
    public GameObject levelUpPanel;
    public TMP_Text levelUpTitle;

    [Header("Strategy Buttons")]
    public Button btnManual;
    public Button btnGreedy;
    public Button btnDP;
    public Button btnBacktrack;

    public void ShowLevelUpPanel(int age)
    {
        levelUpPanel.SetActive(true);
        levelUpTitle.text = $"{age} 岁结算";

        // --- 核心：根据年龄解锁策略 ---
        // 每次打开面板前，先重置所有按钮为不可用，再根据年龄打开
        btnManual.interactable = true; // 手动永远可用
        btnGreedy.interactable = (age >= 10);    // 10岁解锁贪心
        btnDP.interactable = (age >= 20);        // 20岁解锁动态规划
        btnBacktrack.interactable = (age >= 30); // 30岁解锁回溯
    }

    public void HideLevelUpPanel()
    {
        levelUpPanel.SetActive(false);
    }
    void OnEnable()
    {
        // 订阅事件
        EventManager.OnStatsChanged += UpdateStatsUI;
        EventManager.OnTimeChanged += UpdateTimeUI;
        EventManager.OnGameOver += ShowGameOver;
    }

    void OnDisable()
    {
        // 取消订阅 (防止内存泄漏)
        EventManager.OnStatsChanged -= UpdateStatsUI;
        EventManager.OnTimeChanged -= UpdateTimeUI;
        EventManager.OnGameOver -= ShowGameOver;
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void UpdateStatsUI(int value, int weight, int stress)
    {
        statsText.text = $"价值: {value}\n负重: {weight}\n压力: {stress}";
    }

    void UpdateTimeUI(float time, int age)
    {
        timeText.text = $"年龄: {age}岁\n倒计时: {time:F1}s"; // F1 表示保留1位小数
    }

    void ShowGameOver(string reason)
    {
        Debug.Log("UI 接收到游戏结束信号！"); // 加这句日志！
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverText.text = $"游戏结束\n\n{reason}";
        }
    }
}
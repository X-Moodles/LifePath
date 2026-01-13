using UnityEngine;
using System.Collections; // 用于协程

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("流程配置")]
    public int currentAge = 0; // 从0岁开始
    public int ageStep = 10;   // 每关10岁
    public float timeLimit = 60f; // 每关60秒

    [Header("运行时状态")]
    public float timeRemaining;
    public bool isPaused = false; // 暂停标志

    [Header("引用")]
    public PlayerEntity player;
    public MapSystem mapSystem;
    public AISystem aiSystem;
    public UIManager uiManager; // 需要在 UIManager 里加个 public GameObject levelUpPanel;

    void Awake() { Instance = this; }

    void Start()
    {
        // 游戏开始，初始化
        currentAge = 0;
        StartNextDecade();
    }

    void Update()
    {
        if (isPaused) return; // 暂停时不走时间

        // 1. 时间流逝 (压力加速逻辑)
        float timeMultiplier = 1f + (player.currentStress / 50f);

        // 检查压力是否爆表 (比如大于100就死)
        if (player.currentStress > 100)
        {
            GameOver("你承受了太多压力，\n在中途崩溃倒下了...");
            return; // 这里的 return 很重要，防止后面继续跑
        }

        timeRemaining -= Time.deltaTime * timeMultiplier;

        // 2. 更新 UI
        EventManager.OnTimeChanged?.Invoke(timeRemaining, currentAge);
        EventManager.OnStatsChanged?.Invoke(player.currentValue, player.currentWeight, player.currentStress);

        // 3. 检查关卡结束
        if (timeRemaining <= 0)
        {
            EndDecade();
        }
    }

    // --- 核心流程 ---

    // A. 开始新的十年
    public void StartNextDecade()
    {
        isPaused = false;
        timeRemaining = timeLimit;

        // 1. 地图重置
        mapSystem.GenerateWorld();

        // 2. 玩家位置重置 (可选，看你想不想让他接着跑)
        player.transform.position = Vector3.zero;
        player.rb.velocity = Vector2.zero; // 清除惯性
        player.moveInput = Vector2.zero;   // 清除输入

        // 3. UI 隐藏
        uiManager.HideLevelUpPanel();

        Debug.Log($"--- 进入 {currentAge} - {currentAge + ageStep} 岁 ---");
    }

    // B. 十年结束，暂停结算
    void EndDecade()
    {
        isPaused = true; // 暂停游戏
        currentAge += ageStep;
        // --- 新增：强制刹车 ---
        if (player != null)
        {
            player.moveInput = Vector2.zero;    // 清除输入
            player.rb.velocity = Vector2.zero;  // 清除物理惯性
        }
        // --------------------


        if (currentAge >= 60)
        {
            // 确保调用了 GameOver，并且 UIManager 能够显示它
            GameOver("光荣退休！\n你活到了60岁。");
        }
        else
        {
            // 还没到 60，显示升级面板
            uiManager.ShowLevelUpPanel(currentAge);
        }
    }

    // C. 玩家在面板上点击按钮后调用此方法
    public void OnStrategySelected(string strategyName)
    {
        // 1. 设置策略
        aiSystem.SetStrategyByName(strategyName); // 需要在 AISystem 里写这个方法

        // 2. 开始下一关
        StartNextDecade();
    }
    // --- 把这个方法补回 GameManager.cs ---

    // 当 ItemEntity 碰到玩家时会调用这个方法
    public void OnItemCollected(ItemEntity itemEntity)
    {
        // 1. 数据进入玩家背包
        if (player != null)
        {
            player.AddToBackpack(itemEntity.eventData);
        }

        // 2. 销毁场景里的物体
        if (itemEntity != null)
        {
            Destroy(itemEntity.gameObject);
        }
    }
    // --- 请把这段代码复制到 GameManager 类的最后面 ---

    public void GameOver(string reason)
    {
        Debug.Log($"游戏结束触发: {reason}"); // 调试日志，帮你看清是不是真的触发了

        // 1. 停止游戏逻辑
        isPaused = true;

        // 2. 发送事件给 UIManager (让它弹窗)
        // 注意：这里用 ?.Invoke 是防止没有人订阅时报错
        EventManager.OnGameOver?.Invoke(reason);

        // 3. 彻底按住玩家，防止还在背景里乱跑
        if (player != null)
        {
            player.moveInput = Vector2.zero;
            player.rb.velocity = Vector2.zero;
        }
    }
}
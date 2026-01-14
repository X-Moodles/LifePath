using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("基本配置")]
    public int currentAge = 0; 
    public int ageStep = 10;   
    public float timeLimit = 60f; 
    public float stressLimit = 100f; // 修正压力上限为 100

    [Header("实时状态")]
    public float timeRemaining;
    public bool isPaused = false; 

    // 记录上一次发送给 UI 的数值，用于减少重复刷新
    private float lastTimeSent;
    private int lastValueSent;
    private int lastWeightSent;
    private int lastStressSent;

    [Header("引用")]
    public PlayerEntity player;
    public MapSystem mapSystem;
    public AISystem aiSystem;
    public UIManager uiManager; 

    void Awake() { Instance = this; }

    void Start()
    {
        currentAge = 0;
        StartNextDecade();
    }

    void Update()
    {
        if (isPaused) return;

        // 1. 时间流逝 (压力影响逻辑)
        float timeMultiplier = 1f + (player.currentStress / 50f);

        // 检查压力是否爆表
        if (player.currentStress > stressLimit)
        {
            GameOver("因为承受了太大的压力\n你的旅程提前结束了...");
            return; 
        }

        timeRemaining -= Time.deltaTime * timeMultiplier;

        // 2. 优化 UI 更新 (频率控制)
        // 时间每 0.1 秒更新一次
        if (Mathf.Abs(timeRemaining - lastTimeSent) > 0.1f)
        {
            EventManager.OnTimeChanged?.Invoke(timeRemaining, currentAge);
            lastTimeSent = timeRemaining;
        }

        // 属性变化时触发
        if (player.currentValue != lastValueSent || player.currentWeight != lastWeightSent || player.currentStress != lastStressSent)
        {
            EventManager.OnStatsChanged?.Invoke(player.currentValue, player.currentWeight, player.currentStress);
            lastValueSent = player.currentValue;
            lastWeightSent = player.currentWeight;
            lastStressSent = player.currentStress;
        }

        // 3. 检查关卡结束
        if (timeRemaining <= 0)
        {
            EndDecade();
        }
    }

    public void StartNextDecade()
    {
        isPaused = false;
        timeRemaining = timeLimit;
        lastTimeSent = timeLimit; // 初始化记录器

        mapSystem.GenerateWorld();

        player.transform.position = Vector3.zero;
        player.rb.velocity = Vector2.zero; 
        player.moveInput = Vector2.zero;   

        uiManager.HideLevelUpPanel();

        Debug.Log($"--- 第 {currentAge} - {currentAge + ageStep} 岁 ---");
    }

    void EndDecade()
    {
        isPaused = true; 
        currentAge += ageStep;
        
        if (player != null)
        {
            player.moveInput = Vector2.zero;    
            player.rb.velocity = Vector2.zero;  
        }

        if (currentAge >= 60)
        {
            GameOver("岁月不饶人：\n你已经活到了60岁。");
        }
        else
        {
            uiManager.ShowLevelUpPanel(currentAge);
        }
    }

    public void OnStrategySelected(string strategyName)
    {
        aiSystem.SetStrategyByName(strategyName); 
        StartNextDecade();
    }

    public void OnItemCollected(ItemEntity itemEntity)
    {
        if (player != null)
        {
            player.AddToBackpack(itemEntity.eventData);
        }

        if (itemEntity != null)
        {
            // 从活动列表中移除，防止 AI 继续追踪
            if (mapSystem.spawnedItems.Contains(itemEntity))
            {
                mapSystem.spawnedItems.Remove(itemEntity);
            }
            // 使用对象池回收，而不是直接销毁
            mapSystem.RecycleItem(itemEntity);
        }
    }

    public void GameOver(string reason)
    {
        Debug.Log($"游戏结束: {reason}"); 
        isPaused = true;
        EventManager.OnGameOver?.Invoke(reason);

        if (player != null)
        {
            player.moveInput = Vector2.zero;
            player.rb.velocity = Vector2.zero;
        }
    }
}

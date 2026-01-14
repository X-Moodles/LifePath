using UnityEngine;
using System.Collections.Generic;

public class AISystem : MonoBehaviour
{
    [Header("References")]
    public PlayerEntity player;
    public MapSystem mapSystem;

    // 当前激活的策略
    private IStrategy currentStrategy;

    // 场景中所有物品的列表 (AI 的感知范围)
    // 注意：这需要 GameManager 维护，或者我们在这里简单搜索
    private List<ItemEntity> perceivedItems = new List<ItemEntity>();

    void Start()
    {
        // 默认策略：手动
        SetStrategy(new ManualStrategy());

        // 初始化感知列表 (暂时简单处理，每帧找太费性能，建议让 GameManager 传进来)
        // Step 4 我们会完善地图管理，这里先暂时留空
    }

    void Update()
    {
        // --- 新增：如果游戏暂停或没开始，AI 就不工作 ---
        if (GameManager.Instance == null || GameManager.Instance.isPaused)
        {
            // 顺便把玩家的输入清零，防止惯性滑行
            if (player != null) player.moveInput = Vector2.zero;
            return;
        }
        // ---------------------------------------------

        if (player == null || currentStrategy == null) return;

        // 1. 获取感知到的物品 (简单粗暴版：找场景里所有 ItemEntity)
        // 优化建议：以后改为从 MapSystem 获取
        //perceivedItems.Clear();
        //perceivedItems.AddRange(FindObjectsOfType<ItemEntity>());
        // [优化后] 直接从 MapSystem 获取列表，不用全场景搜索了
        // 如果 MapSystem 还没赋值，为了防止报错，先判空
        List<ItemEntity> items = mapSystem != null ? mapSystem.spawnedItems : new List<ItemEntity>();

        // 2. 策略模式：计算方向
        Vector2 intention = currentStrategy.CalculateMove(player, items);

        // 3. 输出信号给 PlayerEntity
        player.moveInput = intention;

        // 测试代码：按键切换策略
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetStrategy(new ManualStrategy());
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetStrategy(new GreedyStrategy());
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetStrategy(new BacktrackStrategy());
    }

    // 公开方法：供 UI 按钮切换策略
    public void SetStrategy(IStrategy newStrategy)
    {
        currentStrategy = newStrategy;
        Debug.Log($"切换策略为：{currentStrategy.GetStrategyName()}");
    }
    public void SetStrategyByName(string name)
    {
        switch (name)
        {
            case "Manual": SetStrategy(new ManualStrategy()); break;
            case "Greedy": SetStrategy(new GreedyStrategy()); break;
            case "DP": SetStrategy(new DPStrategy()); break; // 还没写DP移动版的话，可以先用Greedy代替
            case "Backtrack": SetStrategy(new BacktrackStrategy()); break;
        }
    }
}
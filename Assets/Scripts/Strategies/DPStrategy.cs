using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DPStrategy : IStrategy
{
    // --- 缓存控制 ---
    private ItemEntity currentTarget;
    private float lastCalculationTime;
    private const float RECALC_INTERVAL = 0.5f; 

    // --- 算法参数 ---
    private const int MAX_DEPTH = 3; 
    
    // --- 估算参数 ---
    private const float BASE_SPEED = 5f;
    private const float MIN_SPEED = 1f;

    // 复用 HashSet 以减少 GC
    private HashSet<ItemEntity> visitedSet = new HashSet<ItemEntity>();

    public Vector2 CalculateMove(PlayerEntity player, List<ItemEntity> allItems)
    {
        if (currentTarget != null)
        {
            if (!allItems.Contains(currentTarget) || currentTarget == null || !currentTarget.gameObject.activeInHierarchy) 
            {
                currentTarget = null;
            }
        }

        if (currentTarget == null || Time.time - lastCalculationTime > RECALC_INTERVAL)
        {
            currentTarget = FindBestPath(player, allItems);
            lastCalculationTime = Time.time;
        }

        if (currentTarget != null)
        {
            return (currentTarget.transform.position - player.transform.position).normalized;
        }

        return Vector2.zero;
    }

    private ItemEntity FindBestPath(PlayerEntity player, List<ItemEntity> items)
    {
        // 过滤无效物品
        List<ItemEntity> availableItems = items.Where(i => i != null && i.gameObject.activeInHierarchy).ToList();
        if (availableItems.Count == 0) return null;

        SimState startState = new SimState(
            player.transform.position,
            GameManager.Instance.timeRemaining,
            player.currentStress,
            player.currentWeight,
            player.maxWeight
        );

        float maxScore = float.MinValue;
        ItemEntity bestFirstStep = null;

        visitedSet.Clear();

        foreach (var item in availableItems)
        {
            float score = GetMaxScoreRecursive(item, availableItems, startState, 1, visitedSet);
            
            if (score > maxScore)
            {
                maxScore = score;
                bestFirstStep = item;
            }
        }

        return bestFirstStep;
    }

    private float GetMaxScoreRecursive(ItemEntity target, List<ItemEntity> allItems, SimState currentState, int depth, HashSet<ItemEntity> visited)
    {
        // 1. 计算移动成本
        float distance = Vector2.Distance(currentState.position, target.transform.position);
        float weightRatio = Mathf.Clamp01((float)currentState.currentWeight / currentState.maxWeight);
        float estimatedSpeed = Mathf.Lerp(BASE_SPEED, MIN_SPEED, weightRatio);
        
        float travelTimeBase = distance / estimatedSpeed;
        float timeMultiplier = 1f + (currentState.currentStress / 50f);
        float actualTimeCost = travelTimeBase * timeMultiplier;

        // --- 剪枝条件 ---
        if (currentState.remainingTime < actualTimeCost) return 0; 
        if (currentState.currentWeight + target.eventData.weight > currentState.maxWeight) return -100;

        // --- 状态更新 ---
        float newTime = currentState.remainingTime - actualTimeCost;
        int newStress = currentState.currentStress + target.eventData.stress;
        int newWeight = currentState.currentWeight + target.eventData.weight;
        int gain = target.eventData.value;

        if (newStress > 100) return -500; 

        // --- 递归终止 ---
        if (depth >= MAX_DEPTH)
        {
            return gain;
        }

        // --- 继续搜索下一层 ---
        SimState newState = new SimState(target.transform.position, newTime, newStress, newWeight, currentState.maxWeight);
        
        // 【优化】：使用回溯逻辑，避免每次递归 new HashSet
        visited.Add(target); 

        float maxFutureScore = 0;
        foreach (var nextItem in allItems)
        {
            if (nextItem != null && !visited.Contains(nextItem))
            {
                float futureScore = GetMaxScoreRecursive(nextItem, allItems, newState, depth + 1, visited);
                if (futureScore > maxFutureScore)
                {
                    maxFutureScore = futureScore;
                }
            }
        }

        visited.Remove(target); // 【优化】：回溯移除标记

        return gain + maxFutureScore;
    }

    private struct SimState
    {
        public Vector2 position;
        public float remainingTime;
        public int currentStress;
        public int currentWeight;
        public int maxWeight;

        public SimState(Vector2 pos, float time, int stress, int weight, int maxW)
        {
            position = pos;
            remainingTime = time;
            currentStress = stress;
            currentWeight = weight;
            maxWeight = maxW;
        }
    }

    public string GetStrategyName() => "动态规划 (DP)";
}
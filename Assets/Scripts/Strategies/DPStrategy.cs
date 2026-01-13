using UnityEngine;
using System.Collections.Generic;

public class DPStrategy : IStrategy
{
    // DP 策略：极其功利，只看绝对价值，不顾距离和压力
    // 模拟一种“为了高回报不惜一切代价”的行动模式

    public Vector2 CalculateMove(PlayerEntity player, List<ItemEntity> allItems)
    {
        ItemEntity bestTarget = null;
        float maxVal = float.MinValue;

        foreach (var item in allItems)
        {
            if (item == null) continue;

            // 简单粗暴：只找全图价值最高的那个
            // 这种行为会导致玩家满地图乱跑去追高分，效率低且累，符合“为了虚名而奔波”的隐喻
            if (item.eventData.value > maxVal)
            {
                maxVal = item.eventData.value;
                bestTarget = item;
            }
        }

        if (bestTarget != null)
        {
            return (bestTarget.transform.position - player.transform.position).normalized;
        }
        return Vector2.zero;
    }

    public string GetStrategyName() => "功利主义 (AI)";
}
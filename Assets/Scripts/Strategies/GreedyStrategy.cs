using UnityEngine;
using System.Collections.Generic;

public class GreedyStrategy : IStrategy
{
    public Vector2 CalculateMove(PlayerEntity player, List<ItemEntity> allItems)
    {
        ItemEntity bestTarget = null;
        float maxScore = float.MinValue;

        foreach (var item in allItems)
        {
            if (item == null) continue;

            float dist = Vector2.Distance(player.transform.position, item.transform.position);

            // 贪心核心公式：价值 / 距离 (追求单位距离的最大回报)
            // 防止除以0
            if (dist < 0.1f) dist = 0.1f;

            float score = item.eventData.value / dist;

            if (score > maxScore)
            {
                maxScore = score;
                bestTarget = item;
            }
        }

        if (bestTarget != null)
        {
            return (bestTarget.transform.position - player.transform.position).normalized;
        }
        return Vector2.zero; // 没东西就发呆
    }

    public string GetStrategyName() => "贪心直觉 (AI)";
}
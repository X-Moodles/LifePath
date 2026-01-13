using UnityEngine;
using System.Collections.Generic;

public class BacktrackStrategy : IStrategy
{
    private float thinkTimer = 0f;

    public Vector2 CalculateMove(PlayerEntity player, List<ItemEntity> allItems)
    {
        // 模拟回溯算法的耗时：每隔 1 秒才动一次
        thinkTimer += Time.deltaTime;
        if (thinkTimer < 1f)
        {
            return Vector2.zero; // 正在思考 (剪枝中...)
        }
        // 思考完毕，重置计时器 (为了平滑，这里简单处理)
        if (thinkTimer > 1.2f) thinkTimer = 0f;

        ItemEntity bestTarget = null;
        float minStress = float.MaxValue;

        foreach (var item in allItems)
        {
            if (item == null) continue;

            // 回溯剪枝逻辑：只看低压力的安全选项
            if (item.eventData.stress < minStress)
            {
                minStress = item.eventData.stress;
                bestTarget = item;
            }
        }

        if (bestTarget != null)
        {
            return (bestTarget.transform.position - player.transform.position).normalized;
        }
        return Vector2.zero;
    }

    public string GetStrategyName() => "完美主义 (AI)";
}
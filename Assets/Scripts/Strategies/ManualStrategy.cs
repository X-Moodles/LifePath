using UnityEngine;
using System.Collections.Generic;

public class ManualStrategy : IStrategy
{
    public Vector2 CalculateMove(PlayerEntity player, List<ItemEntity> allItems)
    {
        // 直接读取键盘输入
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        return new Vector2(x, y);
    }

    public string GetStrategyName() => "自由探索 (手动)";
}

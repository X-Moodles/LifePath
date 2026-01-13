using UnityEngine;
using System.Collections.Generic;

public interface IStrategy
{
    // 接口方法：传入玩家实体和所有物品，返回一个移动方向
    Vector2 CalculateMove(PlayerEntity player, List<ItemEntity> allItems);

    // 获取策略名称 (用于 UI 显示)
    string GetStrategyName();
}


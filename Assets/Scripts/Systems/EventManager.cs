using UnityEngine;
using System; // 必须引用，用于 Action

public static class EventManager
{
    // 定义事件：当玩家数据变化时触发
    public static Action<int, int, int> OnStatsChanged; // 参数：价值, 负重, 压力

    // 定义事件：当时间/年龄变化时触发
    public static Action<float, int> OnTimeChanged; // 参数：剩余时间, 当前年龄

    // 定义事件：当游戏结束时触发
    public static Action<string> OnGameOver; // 参数：结局描述
}
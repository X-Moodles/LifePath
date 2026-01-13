using UnityEngine;
using System.Collections.Generic;

public class PlayerEntity : MonoBehaviour
{
    [Header("Runtime Data (运行时数据)")]
    public int currentWeight = 0;
    public int currentValue = 0;
    public int currentStress = 0;
    public int maxWeight = 50;

    // 背包列表
    public List<LifeEvent_SO> backpack = new List<LifeEvent_SO>();

    [Header("Component References (组件引用)")]
    public Rigidbody2D rb;
    public Transform visionRoot; // 视野光圈对象

    // ... 之前的代码 ...

    [Header("Control Signals (控制信号)")]
    public Vector2 moveInput; // 无论是键盘还是AI，都把方向写到这里
    public bool isAIActive = false; // 是否开启 AI 托管

    // 供 System 调用：添加物品数据
    public void AddToBackpack(LifeEvent_SO data)
    {
        backpack.Add(data);
        currentWeight += data.weight;
        currentValue += data.value;
        currentStress += data.stress;
    }

    // 供 System 调用：移除物品数据 (用于贪心/DP策略)
    public void RemoveFromBackpack(LifeEvent_SO data)
    {
        if (backpack.Contains(data))
        {
            backpack.Remove(data);
            currentWeight -= data.weight;
            currentValue -= data.value;
            currentStress -= data.stress;
        }
    }
}

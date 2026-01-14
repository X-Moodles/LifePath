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
    // 在 PlayerEntity.cs 中添加这个方法

    void LateUpdate()
    {
        // 获取地图边界 (如果没有 GameManager 引用，暂时写死 20x15 测试)
        Vector2 mapSize = new Vector2(20, 15);
        if (GameManager.Instance != null && GameManager.Instance.mapSystem != null)
        {
            mapSize = GameManager.Instance.mapSystem.mapSize;
        }

        // 关键修改：把边界稍微放宽 1.0f
        // 这样正常撞墙时，主要靠 BoxCollider 挡住，代码不会插手（不打架）
        // 只有真的飞出去了，代码才救场
        float halfW = mapSize.x / 2f + 1.0f;
        float halfH = mapSize.y / 2f + 1.0f;

        Vector3 pos = transform.position;

        // --- 强制钳制坐标 ---
        // 如果 x 大于边界，就等于边界；如果 x 小于负边界，就等于负边界
        pos.x = Mathf.Clamp(pos.x, -halfW, halfW);
        pos.y = Mathf.Clamp(pos.y, -halfH, halfH);

        // 如果坐标被修正了，说明越界了，强制赋值回去
        if (transform.position != pos)
        {
            transform.position = pos;
            // 顺便把速度清零，防止它还在往墙外冲
            if (rb != null) rb.velocity = Vector2.zero;
        }
    }
}

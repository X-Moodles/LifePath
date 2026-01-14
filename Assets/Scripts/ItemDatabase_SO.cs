using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "LifePath/Item Database")]
public class ItemDatabase_SO : ScriptableObject
{
    public List<LifeEvent_SO> allEvents;

    // 工具方法：随机获取一个事件
    public LifeEvent_SO GetRandomEvent()
    {
        if (allEvents.Count == 0) return null;
        return allEvents[Random.Range(0, allEvents.Count)];
    }
}
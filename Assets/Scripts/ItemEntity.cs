using UnityEngine;

public class ItemEntity : MonoBehaviour
{
    [Header("Data Reference")]
    public LifeEvent_SO eventData; // 引用我们之前写的 ScriptableObject

    [Header("Components")]
    private SpriteRenderer sr;
    private CircleCollider2D col;

    // 初始化方法：工厂生成时调用此方法填充数据
    public void Setup(LifeEvent_SO data)
    {
        eventData = data;
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<CircleCollider2D>();

        if (sr != null)
        {
            // --- 修改开始 ---
            // 只有当数据里真的配了图标时，才替换
            if (data.icon != null)
            {
                sr.sprite = data.icon;
            }
            // --- 修改结束 ---

            sr.color = data.tintColor;
        }

        float scale = 0.5f + (data.weight / 10f);
        transform.localScale = Vector3.one * scale;
        gameObject.name = $"Item_{data.eventName}";
    }

    // 触发逻辑：只负责通知，不负责处理
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 通知全局事件系统 (Observer模式的雏形)
            GameManager.Instance.OnItemCollected(this);
        }
    }
}

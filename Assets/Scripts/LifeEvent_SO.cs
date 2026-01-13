using UnityEngine;

// [CreateAssetMenu] 让我们可以直接在 Unity 编辑器右键创建这个数据文件
[CreateAssetMenu(fileName = "NewLifeEvent", menuName = "LifePath/Life Event Data")]
public class LifeEvent_SO : ScriptableObject
{
    [Header("基本信息")]
    public string eventName;      // 例如：考研、通宵游戏
    [TextArea] public string description; // 描述：一段简短的故事

    [Header("核心数值")]
    public int value;    // 价值 (影响视野)
    public int weight;   // 耗时/重量 (影响速度)
    public int stress;   // 压力 (影响寿命/倒计时)

    [Header("表现")]
    public Sprite icon;  // 图标
    public Color tintColor = Color.white; // 区分类型的颜色 (如红色代表高压)
}

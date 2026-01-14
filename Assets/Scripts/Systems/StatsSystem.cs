using UnityEngine;

public class StatsSystem : MonoBehaviour
{
    [Header("Target Entity")]
    public PlayerEntity playerEntity; // 引用玩家实体

    [Header("Config")]
    public float baseSpeed = 5f;
    public float minSpeed = 1f;
    public float baseVisionScale = 3f;

    // 每一帧计算一次状态
    void FixedUpdate()
    {
        if (playerEntity == null) return;

        UpdateMovementPhysics();
        UpdateVisionPhysics();
    }

    // 子系统 1：动力系统
    void UpdateMovementPhysics()
    {
        // 1. 获取输入 (Input 也可以分离，但暂且放在这里或外部传入)
        //float x = Input.GetAxisRaw("Horizontal");
        //float y = Input.GetAxisRaw("Vertical");
        //Vector2 inputDir = new Vector2(x, y).normalized;


        // 如果 AI 接管了 (由 AISystem 控制)，这里可以扩展逻辑
        // 目前先只处理物理转换...
        Vector2 inputDir = playerEntity.moveInput.normalized;

        // 2. 计算速度 (公式：负重越高，速度越慢)
        float weightRatio = (float)playerEntity.currentWeight / playerEntity.maxWeight;
        weightRatio = Mathf.Clamp01(weightRatio); // 限制在 0-1

        float currentSpeed = Mathf.Lerp(baseSpeed, minSpeed, weightRatio);

        // 3. 应用到 Rigidbody
        playerEntity.rb.MovePosition(playerEntity.rb.position + inputDir * currentSpeed * Time.fixedDeltaTime);

        
    }

    // 子系统 2：视野系统
    // 修改 StatsSystem.cs 中的 UpdateVisionPhysics 方法
    void UpdateVisionPhysics()
    {
        // 1. 获取当前价值，但加上“保底限制”
        // Mathf.Max(0, ...) 确保参与计算的数值永远不会小于 0
        // 这样即使 currentValue 是 -500，这里也会当成 0 来算，不会报错
        float safeValue = Mathf.Max(0f, playerEntity.currentValue);

        // 2. 使用安全的值进行计算 (不管是除法还是对数，现在都安全了)
        // 如果你用的是除法：
        //float targetScale = baseVisionScale + (safeValue / 20f);

        // 如果你用的是对数 (Mathf.Log)：
        float targetScale = baseVisionScale + Mathf.Log(safeValue + 1f);

        // 3. 【双重保险】检查计算结果是否有效
        if (float.IsNaN(targetScale) || float.IsInfinity(targetScale))
        {
            targetScale = baseVisionScale; // 如果算崩了，就恢复默认值
        }

        // 4. 赋值
        playerEntity.visionRoot.localScale = Vector3.Lerp(
            playerEntity.visionRoot.localScale,
            new Vector3(targetScale, targetScale, 1),
            Time.fixedDeltaTime * 2f // 注意：在 FixedUpdate 里最好用 fixedDeltaTime
        );
    }
}
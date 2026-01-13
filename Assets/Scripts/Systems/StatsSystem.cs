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
    void UpdateVisionPhysics()
    {
        // 旧公式：价值越高，看得越远
        // 假设每 20 点价值增加 1 单位半径
        //float targetScale = baseVisionScale + (playerEntity.currentValue / 20f);

        // --- 【修改代码】更科学的视野公式 ---

        // 方案：对数增长 + 封顶限制
        // Mathf.Log(x) 会随着 x 变大而增长得越来越慢
        // 举例：价值100时增加4.6，价值1000时增加6.9，不会无限膨胀
        float addedScale = Mathf.Log(playerEntity.currentValue + 1f);

        // 基础视野(3) + 增长值
        float targetScale = baseVisionScale + addedScale;

        // 【关键】强制封顶！最大不超过 10 (你可以根据屏幕大小调整这个数)
        targetScale = Mathf.Clamp(targetScale, baseVisionScale, 10f);

        // 平滑过渡效果
        playerEntity.visionRoot.localScale = Vector3.Lerp(
            playerEntity.visionRoot.localScale,
            new Vector3(targetScale, targetScale, 1),
            Time.deltaTime * 2f
        );
    }
}
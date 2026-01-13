using UnityEngine;
using System.Collections.Generic;

public class MapSystem : MonoBehaviour
{
    [Header("Config (配置)")]
    public Vector2 mapSize = new Vector2(20, 15); // 地图总大小
    public int recursionDepth = 3; // 递归深度 (3层 = 4^3 = 64个格子)

    [Header("References (引用)")]
    public GameObject itemPrefab; // 物品的预制体 (Step 2 做的)
    public ItemDatabase_SO itemDatabase; // 物品数据库 (Step 1 做的)
    public Transform itemRoot; // 生成物品的父节点 (为了整洁)

    // 存储生成的物品引用，供 AI 系统使用
    public List<ItemEntity> spawnedItems = new List<ItemEntity>();

    void Start()
    {
        GenerateWorld();
    }

    public void GenerateWorld()
    {
        // 1. 清理旧物品 (如果有)
        foreach (var item in spawnedItems)
        {
            if (item != null) Destroy(item.gameObject);
        }
        spawnedItems.Clear();

        // 生成墙壁 (新增!)
        CreateBoundaries();

        // 2. 开始分治递归 (入口)
        // 定义初始区域为整个地图
        Rect fullMap = new Rect(-mapSize.x / 2, -mapSize.y / 2, mapSize.x, mapSize.y);
        DivideAndConquer(fullMap, recursionDepth);

        Debug.Log($"地图生成完毕！生成了 {spawnedItems.Count} 个物品。");
    }
    // 在 MapSystem.cs 中添加这个方法
    void CreateBoundaries()
    {
        // 1. 创建一个空物体装墙壁 (如果已有先删除)
        Transform wallParent = transform.Find("Walls");
        if (wallParent != null) Destroy(wallParent.gameObject);

        wallParent = new GameObject("Walls").transform;
        wallParent.SetParent(transform);

        // 2. 定义墙壁的厚度
        float thickness = 2f;
        float halfW = mapSize.x / 2f;
        float halfH = mapSize.y / 2f;

        // 3. 生成四面墙 (上、下、左、右)
        // 这里的逻辑是：位置 + 大小
        CreateOneWall(new Vector2(0, halfH + thickness / 2), new Vector2(mapSize.x + thickness * 2, thickness), wallParent); // Top
        CreateOneWall(new Vector2(0, -halfH - thickness / 2), new Vector2(mapSize.x + thickness * 2, thickness), wallParent); // Bottom
        CreateOneWall(new Vector2(-halfW - thickness / 2, 0), new Vector2(thickness, mapSize.y), wallParent); // Left
        CreateOneWall(new Vector2(halfW + thickness / 2, 0), new Vector2(thickness, mapSize.y), wallParent); // Right
    }

    void CreateOneWall(Vector2 pos, Vector2 size, Transform parent)
    {
        GameObject wall = new GameObject("Wall");
        wall.transform.SetParent(parent);
        wall.transform.position = pos;

        // 添加碰撞体
        BoxCollider2D bc = wall.AddComponent<BoxCollider2D>();
        bc.size = size;

        // 设置层级 (Wall) 或者是 Default，确保 Player 能撞上
    }


    // --- 核心算法：分治法 ---
    void DivideAndConquer(Rect currentArea, int depth)
    {
        // [Conquer] 递归终止条件：深度为0，不再分割，直接在这里生成物品
        if (depth == 0)
        {
            SpawnItemInArea(currentArea);
            return;
        }

        // [Divide] 分割：将当前区域切成 4 份 (四叉树思想)
        float halfW = currentArea.width / 2f;
        float halfH = currentArea.height / 2f;

        // 递归调用 4 个子象限
        // 左下
        DivideAndConquer(new Rect(currentArea.x, currentArea.y, halfW, halfH), depth - 1);
        // 右下
        DivideAndConquer(new Rect(currentArea.x + halfW, currentArea.y, halfW, halfH), depth - 1);
        // 左上
        DivideAndConquer(new Rect(currentArea.x, currentArea.y + halfH, halfW, halfH), depth - 1);
        // 右上
        DivideAndConquer(new Rect(currentArea.x + halfW, currentArea.y + halfH, halfW, halfH), depth - 1);
    }

    // 在指定区域内随机生成一个物品
    void SpawnItemInArea(Rect area)
    {
        if (itemDatabase == null || itemDatabase.allEvents.Count == 0) return;

        // 1. 随机坐标 (加一点内缩 padding，防止生成在格线边缘)
        float padding = 0.5f;
        Vector2 randomPos = new Vector2(
            Random.Range(area.xMin + padding, area.xMax - padding),
            Random.Range(area.yMin + padding, area.yMax - padding)
        );

        // 2. 随机数据 (从数据库里抽卡)
        LifeEvent_SO randomData = itemDatabase.GetRandomEvent();

        // 3. 实例化
        GameObject go = Instantiate(itemPrefab, randomPos, Quaternion.identity, itemRoot);
        ItemEntity entity = go.GetComponent<ItemEntity>();

        // 4. 初始化实体数据
        entity.Setup(randomData);

        // 5. 注册到列表
        spawnedItems.Add(entity);
    }

    // [加分项] 在 Scene 窗口画出分治的网格线，答辩时给老师看这个！
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        DrawGizmoRect(new Rect(-mapSize.x / 2, -mapSize.y / 2, mapSize.x, mapSize.y), recursionDepth);
    }

    void DrawGizmoRect(Rect rect, int depth)
    {
        if (depth == 0)
        {
            // 画格子边框
            Gizmos.DrawWireCube(rect.center, rect.size);
            return;
        }
        float halfW = rect.width / 2f;
        float halfH = rect.height / 2f;
        DrawGizmoRect(new Rect(rect.x, rect.y, halfW, halfH), depth - 1);
        DrawGizmoRect(new Rect(rect.x + halfW, rect.y, halfW, halfH), depth - 1);
        DrawGizmoRect(new Rect(rect.x, rect.y + halfH, halfW, halfH), depth - 1);
        DrawGizmoRect(new Rect(rect.x + halfW, rect.y + halfH, halfW, halfH), depth - 1);
    }
}
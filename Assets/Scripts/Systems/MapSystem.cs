using UnityEngine;
using System.Collections.Generic;

public class MapSystem : MonoBehaviour
{
    [Header("配置")]
    public Vector2 mapSize = new Vector2(20, 15); 
    public int recursionDepth = 3; 

    [Header("引用")]
    public GameObject itemPrefab; 
    public ItemDatabase_SO itemDatabase; 
    public Transform itemRoot; 

    public List<ItemEntity> spawnedItems = new List<ItemEntity>();
    
    // 对象池
    private Queue<ItemEntity> itemPool = new Queue<ItemEntity>();

    void Start()
    {
        GenerateWorld();
    }

    public void GenerateWorld()
    {
        // 1. 回收当前屏幕上的所有物品到对象池
        foreach (var item in spawnedItems)
        {
            if (item != null) RecycleItem(item);
        }
        spawnedItems.Clear();

        CreateBoundaries();

        // 2. 重新生成
        Rect fullMap = new Rect(-mapSize.x / 2, -mapSize.y / 2, mapSize.x, mapSize.y);
        DivideAndConquer(fullMap, recursionDepth);

        Debug.Log($"地图生成完毕，生成了 {spawnedItems.Count} 个物品");
    }

    // 回收物品到对象池
    public void RecycleItem(ItemEntity item)
    {
        item.gameObject.SetActive(false);
        itemPool.Enqueue(item);
        
        // 如果是从 spawnedItems 中手动删除，需要外部处理列表清理
    }

    // 从对象池获取或创建新物品
    private ItemEntity GetItemFromPool(Vector2 position)
    {
        ItemEntity entity;
        if (itemPool.Count > 0)
        {
            entity = itemPool.Dequeue();
            entity.gameObject.SetActive(true);
            entity.transform.position = position;
        }
        else
        {
            GameObject go = Instantiate(itemPrefab, position, Quaternion.identity, itemRoot);
            entity = go.GetComponent<ItemEntity>();
        }
        return entity;
    }

    void CreateBoundaries()
    {
        Transform wallParent = transform.Find("Walls");
        if (wallParent != null) Destroy(wallParent.gameObject);

        wallParent = new GameObject("Walls").transform;
        wallParent.SetParent(transform);

        float thickness = 2f;
        float halfW = mapSize.x / 2f;
        float halfH = mapSize.y / 2f;

        CreateOneWall(new Vector2(0, halfH + thickness / 2), new Vector2(mapSize.x + thickness * 2, thickness), wallParent); 
        CreateOneWall(new Vector2(0, -halfH - thickness / 2), new Vector2(mapSize.x + thickness * 2, thickness), wallParent); 
        CreateOneWall(new Vector2(-halfW - thickness / 2, 0), new Vector2(thickness, mapSize.y), wallParent); 
        CreateOneWall(new Vector2(halfW + thickness / 2, 0), new Vector2(thickness, mapSize.y), wallParent); 
    }

    void CreateOneWall(Vector2 pos, Vector2 size, Transform parent)
    {
        GameObject wall = new GameObject("Wall");
        wall.transform.SetParent(parent);
        wall.transform.position = pos;

        BoxCollider2D bc = wall.AddComponent<BoxCollider2D>();
        bc.size = size;

        SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Background");
    }

    void DivideAndConquer(Rect currentArea, int depth)
    {
        if (depth == 0)
        {
            SpawnItemInArea(currentArea);
            return;
        }

        float halfW = currentArea.width / 2f;
        float halfH = currentArea.height / 2f;

        DivideAndConquer(new Rect(currentArea.x, currentArea.y, halfW, halfH), depth - 1);
        DivideAndConquer(new Rect(currentArea.x + halfW, currentArea.y, halfW, halfH), depth - 1);
        DivideAndConquer(new Rect(currentArea.x, currentArea.y + halfH, halfW, halfH), depth - 1);
        DivideAndConquer(new Rect(currentArea.x + halfW, currentArea.y + halfH, halfW, halfH), depth - 1);
    }

    void SpawnItemInArea(Rect area)
    {
        if (itemDatabase == null || itemDatabase.allEvents.Count == 0) return;

        float padding = 0.5f;
        Vector2 randomPos = new Vector2(
            Random.Range(area.xMin + padding, area.xMax - padding),
            Random.Range(area.yMin + padding, area.yMax - padding)
        );

        LifeEvent_SO randomData = itemDatabase.GetRandomEvent();

        // 使用对象池获取
        ItemEntity entity = GetItemFromPool(randomPos);
        entity.Setup(randomData);
        spawnedItems.Add(entity);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        DrawGizmoRect(new Rect(-mapSize.x / 2, -mapSize.y / 2, mapSize.x, mapSize.y), recursionDepth);
    }

    void DrawGizmoRect(Rect rect, int depth)
    {
        if (depth == 0)
        {
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

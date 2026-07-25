using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Coll : AbstractGimmick
{
    private Tilemap tileMap;
    private Collider2D tileCollider;
    private CompositeCollider2D compositeCollider; 
    
    private void Awake()
    {
        tileMap = GetComponent<Tilemap>();
        
        // 💡 CompositeCollider2D를 먼저 확인하고, 없으면 일반 Collider2D를 가져옵니다.
        compositeCollider = GetComponent<CompositeCollider2D>();
        tileCollider = GetComponent<Collider2D>();

        if (tileMap == null) 
            Debug.LogError($"[Coll] {gameObject.name}에 Tilemap 컴포넌트가 없습니다!");
        if (compositeCollider == null && tileCollider == null) 
            Debug.LogError($"[Coll] {gameObject.name}에 Collider2D 또는 CompositeCollider2D 컴포넌트가 없습니다!");
    }

    protected override void HandleStateChanged(bool isProcessOn)
    {
        if (tileMap == null) return;

        // 1. CompositeCollider2D가 있다면 우선 제어
        if (compositeCollider != null)
        {
            compositeCollider.isTrigger = !isProcessOn;
        }
        // 2. Composite가 없을 경우 일반 TilemapCollider2D 제어
        else if (tileCollider != null)
        {
            tileCollider.isTrigger = !isProcessOn;
        }

        // 색상 및 상태(레이어) 처리
        if (isProcessOn) 
        {
            // 💡 켜짐: 바닥으로 감지되도록 Ground 레이어(예: 6번)로 복구
            gameObject.layer = 6; 
            
            tileMap.color = new Color(1f, 1f, 1f, 1f); 
            ConsoleManager.Instance?.LogSystem($"[SYS_UPDATE] {gameObject.name} collision mesh restored.");
        }
        else 
        {
            // 💡 꺼짐: Raycast가 무시하도록 Ignore 레이어(예: 7번)로 변경
            gameObject.layer = 7; 
            
            tileMap.color = new Color(1f, 1f, 1f, 0.25f); 
            ConsoleManager.Instance?.Log($"[SYS_UPDATE] {gameObject.name} collision mesh bypassed.");
        }
    }
}
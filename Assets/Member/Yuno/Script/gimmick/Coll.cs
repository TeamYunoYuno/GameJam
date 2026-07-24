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

        // 💡 1. CompositeCollider2D가 있다면 우선 제어
        if (compositeCollider != null)
        {
            compositeCollider.isTrigger = !isProcessOn;
        }
        // 💡 2. Composite가 없을 경우 일반 TilemapCollider2D 제어
        else if (tileCollider != null)
        {
            tileCollider.isTrigger = !isProcessOn;
        }

        // 색상 및 상태 처리
        if (isProcessOn) 
        {
            // 켜짐: 불투명한 기본 흰색 (벽 상태)
            tileMap.color = new Color(1f, 1f, 1f, 1f); 
            
            // 콜라이더 복구 로그
            ConsoleManager.Instance?.LogSystem($"[SYS_UPDATE] {gameObject.name} collision mesh restored.");
        }
        else 
        {
            // 꺼짐: 반투명한 흰색 (통과 가능 상태)
            tileMap.color = new Color(1f, 1f, 1f, 0.25f); 
            
            // 콜라이더 무시 로그
            ConsoleManager.Instance?.Log($"[SYS_UPDATE] {gameObject.name} collision mesh bypassed.");
        }
    }
}
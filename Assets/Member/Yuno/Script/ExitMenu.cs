using UnityEngine;
using System.Collections.Generic; // 리스트 사용을 위해 필요

public class ExitMenu: MonoBehaviour
{
    [Header("프리팹 및 생성 설정")]
    [Tooltip("종료 확인 팝업 프리팹")]
    public GameObject exitWindowPrefab; 
    
    [Tooltip("창이 생성될 부모 Canvas Transform (비어있으면 이 스크립트 오브젝트의 자식으로)")]
    public Transform windowParent;

    [Header("배치 설정")]
    [Tooltip("창이 생성될 때마다 이동할 거리 (X, Y)")]
    public Vector2 offsetPerWindow = new Vector2(50f, -50f);
    [Tooltip("첫 번째 창의 시작 위치")]
    public Vector2 startPosition = Vector2.zero;

    // 현재 생성된 모든 창을 관리하는 리스트 (나중에 사용)
    private List<GameObject> activeWindows = new List<GameObject>();

    void Start()
    {
        // 부모 Transform이 지정되지 않았다면, 이 스크립트가 붙은 오브젝트로 설정
        if (windowParent == null)
        {
            windowParent = this.transform;
        }
    }

    // 1. ExitGame 버튼에 연결할 함수 (누를 때마다 창 생성)
    public void ExitGame()
    {
        if (exitWindowPrefab != null)
        {
            // 1) 창 생성 (부모 설정 필수)
            GameObject newWindow = Instantiate(exitWindowPrefab, windowParent);

            // 2) 리스트에 추가 (관리용)
            activeWindows.Add(newWindow);

            // 3) 위치 계산 (밀려나게 하기)
            RectTransform rectTransform = newWindow.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // 생성된 창의 순서에 따라 오프셋 적용
                int windowIndex = activeWindows.Count - 1; // 0부터 시작
                Vector2 newPos = startPosition + (offsetPerWindow * windowIndex);
                
                // 윈도우 창처럼 화면 중앙 기준이 아닌, '누적된 좌표'로 설정
                rectTransform.anchoredPosition = newPos;

                // 4) [중요] 생성된 창의 닫기 버튼에 이벤트 연결 (스크립트로 제어)
                SetupCloseButton(newWindow);
            }
        }
        else
        {
            Debug.LogError("FakeExitManager: Exit Window Prefab이 연결되지 않았습니다!");
        }
    }

    // 5) [중요] 생성된 창의 닫기 버튼을 찾아서 클릭 이벤트를 연결합니다.
    private void SetupCloseButton(GameObject window)
    {
        // 프리팹 내부에 '닫기' 또는 '아니오' 버튼이 있다는 가정이 필요합니다.
        // 예: 이름이 "CloseButton"인 버튼을 찾거나, GetComponentsInChildren로 찾습니다.
        // 여기서는 예시로 가장 간단하게 'Find'로 이름으로 찾겠습니다. (좋은 방법은 아니지만 이해용)
        UnityEngine.UI.Button closeButton = null;

        // 예시 1: 이름으로 찾기
        Transform closeBtnTransform = window.transform.Find("CloseButton"); 
        if (closeBtnTransform != null)
        {
            closeButton = closeBtnTransform.GetComponent<UnityEngine.UI.Button>();
        }
        // 예시 2: 전체에서 찾기 (조금 더 안전하지만 느릴 수 있음)
        if(closeButton == null)
        {
             closeButton = window.GetComponentInChildren<UnityEngine.UI.Button>(); // 버튼 컴포넌트를 가진 첫 번째 자식을 가져옴
        }

        if (closeButton != null)
        {
            // [중요] 버튼 클릭 시, 자기 자신(newWindow)을 삭제하는 함수를 연결
            closeButton.onClick.AddListener(() => CloseWindow(window));
        }
        else
        {
            Debug.LogError($"윈도우 창 '{window.name}' 내부에 '닫기 버튼'을 찾을 수 없습니다.");
        }
    }


    // 6) [중요] 닫기 버튼에 연결할 함수 (실제 창 삭제 및 리스트 정리)
    public void CloseWindow(GameObject window)
    {
        // 리스트에서 제거
        if (activeWindows.Contains(window))
        {
            activeWindows.Remove(window);
        }

        // 실제 오브젝트 삭제
        Destroy(window);

        // [선택 사항] 창이 삭제된 후, 나머지 창들의 위치를 재정렬하고 싶다면?
        // RearrangeWindows(); // 이 함수는 직접 구현해야 함
    }
}
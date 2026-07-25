using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; // Button 사용을 위해 필요

public class GameOver : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject gameOverPanel;

    [Header("종료 윈도우 연출 설정")]
    public GameObject exitWindowPrefab; // 프리팹으로 만든 윈도우 창
    public Transform canvasTransform;   // 윈도우가 생성될 Canvas

    [Header("밀려나는 거리 설정")]
    public Vector2 offset = new Vector2(40f, -40f); // 누를 때마다 오른쪽 아래로 밀려남
    private int windowCount = 0; // 창 카운트

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        Time.timeScale = 1f; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) Die();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) Die();
    }

    private void Die()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        SoundManager.instance.PlaySFX("Over");
        
        Time.timeScale = 0f; 
    }

    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // ==========================================
    // 💡 [Exit 버튼 연출: 생성된 창의 모든 버튼을 닫기 버튼으로 지정]
    // ==========================================
    public void ExitGame()
    {
        SoundManager.instance.PlaySFX("Error");
        if (exitWindowPrefab == null || canvasTransform == null) return;

        // 1. 윈도우 창 생성
        GameObject newWindow = Instantiate(exitWindowPrefab, canvasTransform);

        // 2. 창 위치 밀려나게 배치
        RectTransform rect = newWindow.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition += offset * windowCount;
            windowCount++;
        }

        // 3. 패널 안에 있는 '모든 버튼'을 찾아서 전부 닫기(Destroy) 기능 연결!
        Button[] allButtons = newWindow.GetComponentsInChildren<Button>();
        foreach (Button btn in allButtons)
        {
            btn.onClick.AddListener(() => Destroy(newWindow));
        }
    }
}
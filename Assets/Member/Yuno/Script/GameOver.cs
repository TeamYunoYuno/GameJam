using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameOver : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject gameOverPanel;

    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        Time.timeScale = 1f; 
    }

    // 1. 트리거에 닿았을 때 (Is Trigger 체크된 오브젝트)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 그냥 CompareTag로 바로 문자열 검사
        if (collision.CompareTag("Player"))
        {
            Die();
        }
    }

    // 2. 물리적으로 부딪혔을 때
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 여기도 마찬가지로 CompareTag로 바로 검사
        if (collision.gameObject.CompareTag("Player"))
        {
            Die();
        }
    }

    private void Die()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f; 
    }

    // 버튼에 연결할 재시작 함수
    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
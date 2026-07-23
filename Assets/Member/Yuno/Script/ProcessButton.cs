using UnityEngine;
using UnityEngine.UI;

public class ProcessButton : MonoBehaviour
{
    // 이 버튼이 클릭될 때 매니저에게 넘겨줄 대상 무전기(SO)를 인스펙터에서 할당받습니다.
    [SerializeField] private ProcessData myProcess; 

    // 클릭 시마다 GetComponent를 호출하는 성능 낭비를 막기 위해 멤버 변수로 선언합니다.
    private Button _button; 

    private void Awake()
    {
        // 씬 시작 시 단 한 번만 버튼 컴포넌트를 메모리에 캐싱합니다.
        _button = GetComponent<Button>(); 

        // 인스펙터 OnClick에 함수를 하드코딩하는 대신, 코드 단에서 리스너를 달아 휴먼 에러를 방지합니다.
        _button.onClick.AddListener(OnClickProcessButton); 
    }

    // 버튼을 클릭하는 순간 실행됩니다.
    private void OnClickProcessButton()
    {
        // 매니저에게 "내가 쥐고 있는 이 에셋(myProcess)을 심사해 줘!"라고 요청합니다.
        MemoryManager.Instance.RequestToggleProcess(myProcess); 
    }
}
using UnityEngine;

public abstract class AbstractGimmick : MonoBehaviour
{
    [SerializeField] private ProcessData targetProcess;
    
    protected virtual void OnEnable()
    {
        targetProcess.OnStateChanged += HandleStateChanged;
    }

    protected virtual void OnDisable()
    {
        targetProcess.OnStateChanged -= HandleStateChanged;
    }

    protected abstract void HandleStateChanged(bool isProcessOn);
}
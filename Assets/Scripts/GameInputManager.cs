using UnityEngine;
using System;

public class GameInputManager : MonoBehaviour
{ 
    public static GameInputManager Current { get; private set; } // самый простой синглтон, но в рамках этого прототипа не должно вызвать проблем.

    public event Action<Vector2> OnTouchHappend;
    public event Action OnTouchEnded;
    private void Awake()
    {
        Current = this;
    }
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch firstTouch = Input.GetTouch(0);
            if(firstTouch.phase == TouchPhase.Moved || firstTouch.phase == TouchPhase.Stationary || firstTouch.phase == TouchPhase.Began )
            {
                OnTouchHappend?.Invoke(firstTouch.position);
            }
            else
            {
                Debug.Log("End");
                OnTouchEnded?.Invoke();
            }
        }
#if UNITY_EDITOR
        else if (Input.GetMouseButton(0)) 
        {
            OnTouchHappend?.Invoke(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnTouchEnded?.Invoke();
        }
#endif
    }

    private void OnDestroy()
    {
        Current = null;
    }
}

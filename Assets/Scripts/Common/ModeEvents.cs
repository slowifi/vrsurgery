using UnityEngine;
using UnityEditor;

public class ModeEvents : MonoBehaviour
{
    /// <summary>
    /// NOTE: 뭔가 조금 일관성이 없는 경우가 있을 수 있는데, 그런 애들은 이름을 고쳐야 한다.
    /// 유저의 액션이나 상황에 의해 어떤 결과를 기대하고 요청을 할때의 이벤트는
    /// xxxxRequest이다. 이런 경우는 대부분 한 곳에서만 listening하다가 실제로 관련
    /// 처리를 하거나 취소한다. 이 listner가 처리를 하고 나면 xxxxSelected, xxxxChanged라던가
    /// 하는 이벤트가 추가로 발생한다. 많은 UI요소들이 이런 이벤트를 받아서 거기에 맞는
    /// 동작을 각자 알아서 하게 된다...로 정리가 되었는데 초기에 만든 애들은
    /// 그냥 막 되어 있는 경우가 있다. 시간 날때? 고치도록 하자.
    /// 이노핏에서 펌
    /// </summary>
    public event ModeChangedHandler OnModeChanged;
    public delegate void ModeChangedHandler(string mode);
    public void InvokeModeChanged(string mode)
    {
        OnModeChanged?.Invoke(mode);
    }

    public event ModeManipulateHandler OnModeManipulate;
    public delegate void ModeManipulateHandler(string action);
    public void InvokeModeManipulate(string action)
    {
        OnModeManipulate?.Invoke(action);
    }

    public event UIChangeHandler OnUIChanged;
    public delegate void UIChangeHandler();
    public void InvokeUIChanged()
    {
        OnUIChanged?.Invoke();
    }

    public event UIFixedHandler OnUIFixed;
    public delegate void UIFixedHandler();
    public void InvokeUIFixed()
    {
        OnUIFixed?.Invoke();
    }
}
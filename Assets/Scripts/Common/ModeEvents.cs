using UnityEngine;
using UnityEditor;

public class ModeEvents : MonoBehaviour
{
    public event ModeChangedHandler OnModeChanged;
    public delegate void ModeChangedHandler();
    
    public void InvokeModeChanged()
    {
        OnModeChanged?.Invoke();
    }
}
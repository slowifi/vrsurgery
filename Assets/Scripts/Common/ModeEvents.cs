using UnityEngine;
using UnityEditor;

public class ModeEvents : MonoBehaviour
{
    public event ModeChangedHandler OnModeChanged;
    public delegate void ModeChangedHandler(string mode);
    public void InvokeModeChanged(string mode)
    {
        OnModeChanged?.Invoke(mode);
    }

    
    //public event ModeChangedHandler OnModeChanged;
    //public delegate void ModeChangedHandler(string mode);
    //public void InvokeModeChanged(string mode)
    //{
    //    OnModeChanged?.Invoke(mode);
    //}

}
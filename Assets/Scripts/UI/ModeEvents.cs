using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeEvents : MonoBehaviour
{
    public delegate void PlayerControlEventHandler();
    public delegate void ButtonEventHandler();
    public delegate void MethodChangedEventHandler();

    public event PlayerControlEventHandler OnPlayerControl;
    public event ButtonEventHandler OnButton;
    public event MethodChangedEventHandler OnMethodChanged;








}



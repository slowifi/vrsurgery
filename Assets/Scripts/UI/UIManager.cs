using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public Scrollbar cuttingSizeBar;

    public Scrollbar curveBar;
    public Scrollbar heightBar;

    public Scrollbar extendBar;

    public Text distance;

    public Button controlRotationScaleButton;

    public Button eraseButton;
    public Button boundaryCutButton;
    public Button incisionButton;

    public Button patchButton;
    public List<Button> patchIndex;

    public Button measureDistanceButton;
    
    public Button resetButton;
    public Button resetPositionScaleButton;

    
    // 각각에 대한 event handle 부분 설정 필요.
}

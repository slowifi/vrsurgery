using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class UIManager : Singleton<UIManager>
{
    // 여기서 하는게 뭐지?
    public Scrollbar cuttingSizeBar;

    public Scrollbar curveBar;
    public Scrollbar heightBar;

    public Scrollbar extendBar;

    public Text Distance;
    public Text Diameter;
    public Text FileName;

    public Button controlRotationScaleButton;

    public Button eraseButton;
    public Button boundaryCutButton;
    public Button incisionButton;

    public Button patchButton;
    public List<Button> patchIndex;

    public Button measureDistanceButton;
    
    public Button resetButton;
    public Button resetPositionScaleButton;

    public void SetFileName(string arg)
    {
        FileName.text = arg;
    }

    public void SetDistance(float arg)
    {
        Distance.text = arg + "mm";
    }

    public void SetDiameter(float arg)
    {
        Diameter.text = arg + "mm";
    }

    private void Awake()
    {

#if UNITY_STANDALONE_WIN
#endif

#if UNITY_ANDROID
        extendBar.gameObject.SetActive(false);
#endif




    }
}

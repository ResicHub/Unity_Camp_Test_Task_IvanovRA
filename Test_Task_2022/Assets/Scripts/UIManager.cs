using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains methods for processing UI events and contains links to all used UI objects on scene.
/// </summary>
public class UIManager : MonoBehaviour
{
    /// <summary>
    /// An object instance implemented with a singleton.
    /// </summary>
    public static UIManager Instance;

    private Color TextColor = new Color(0, 231, 255);

    [SerializeField]
    private Slider sldAnchorCount;

    [SerializeField]
    private TextMeshProUGUI anchorCountValue;

    [SerializeField]
    private Toggle tglLoop;

    [SerializeField]
    private Toggle tglNonCrossing;

    [SerializeField]
    private Slider sldArrowSpeed;

    [SerializeField]
    private TextMeshProUGUI arrowSpeedValue;

    [SerializeField]
    private TextMeshProUGUI arrowPassageTimeValue;

    [SerializeField]
    private TextMeshProUGUI hotkeysText;

    [SerializeField]
    private TextMeshProUGUI messageText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        sldAnchorCount.onValueChanged.AddListener(OnAnchorCountValueChanged);
        tglLoop.onValueChanged.AddListener(OnLoopToggleChanged);
        sldArrowSpeed.onValueChanged.AddListener(OnArrowSpeedValueChanged);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            InputHandler();
        }
    }

    /// <summary>
    /// Checks pressed buttons.
    /// </summary>
    private void InputHandler()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            OnButtonGenerateClick();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            OnButtonStartClick();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            OnButtonSaveClick();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            OnButtonLoadClick();
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            OnButtonHotKeysClick();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnButtonQuitClick();
        }
    }

    /// <summary>
    /// Writes the message on scene.
    /// </summary>
    /// <param name="message"></param>
    public void WriteMessage(string message, bool isError = false)
    {
        messageText.text = message;
        StartCoroutine(DeleteMessageAfterTime());
        if (!isError) 
        {
            if (Equals(messageText.color, Color.red))
            {
                messageText.color = TextColor;
            }
            return;
        }
        messageText.color = Color.red;
    }

    /// <summary>
    /// Deletes messages after 10 seconds.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeleteMessageAfterTime()
    {
        yield return new WaitForSeconds(10);
        messageText.text = "";
    }

    /// <summary>
    /// On/Off the hotkeys tooltip.
    /// </summary>
    public void OnButtonHotKeysClick()
    {
        hotkeysText.enabled = !hotkeysText.enabled;
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void OnButtonQuitClick()
    {
        Application.Quit();
    }

    /// <summary>
    /// Starts SaveLoadManager.Save() method.
    /// </summary>
    public void OnButtonSaveClick()
    {
        SaveLoadManager.Instance.Save();
    }

    /// <summary>
    /// Starts SaveLoadManager.Load() method.
    /// </summary>
    public void OnButtonLoadClick()
    {
        SaveLoadManager.Instance.Load();
    }

    /// <summary>
    /// Uploads UI values about anchors count, arrow's movement speed and loop parameter.
    /// </summary>
    public void OnLoadHasDone()
    {
        sldAnchorCount.value = CurveGenerator.Instance.AnchorsCount;
        sldArrowSpeed.value = ArrowController.Instance.MovementSpeed;
        tglLoop.isOn = CurveGenerator.Instance.Loop;
    }

    private void OnAnchorCountValueChanged(float arg0)
    {
        anchorCountValue.text = sldAnchorCount.value.ToString();
    }

    private void OnLoopToggleChanged(bool arg0)
    {
        CurveGenerator.Instance.Loop = tglLoop.isOn;
        BezierSmoother.Instance.CreateCurve();
    }

    /// <summary>
    /// Starts CurveGenerator.Generate() method.
    /// </summary>
    public void OnButtonGenerateClick()
    {
        CurveGenerator.Instance.AnchorsCount = Convert.ToInt32(sldAnchorCount.value);
        CurveGenerator.Instance.NonCrossingCurve = tglNonCrossing.isOn;
        CurveGenerator.Instance.Generate();
    }

    private void OnArrowSpeedValueChanged(float arg0)
    {
        arrowSpeedValue.text = sldArrowSpeed.value.ToString();
        ArrowController.Instance.MovementSpeed = sldArrowSpeed.value;
        ArrowController.Instance.CalculatePassageTime();
        arrowPassageTimeValue.text = Math.Round(ArrowController.Instance.PassageTime, 2).ToString() + " sec.";
    }

    /// <summary>
    /// Starts ArrowController.StartMoving() method.
    /// </summary>
    public void OnButtonStartClick()
    {
        ArrowController.Instance.StartMoving();
    }

    /// <summary>
    /// Upload UI arrow passage time value.
    /// </summary>
    public void PassageTimeHasCHanged()
    {
        arrowPassageTimeValue.text = Math.Round(ArrowController.Instance.PassageTime, 2).ToString() + " sec.";
    }
}

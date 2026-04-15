using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BrushUI : MonoBehaviour
{
    [SerializeField] private SettingsChanger settingsChanger;

    [Header("Brush type buttons")]
    [SerializeField] private Button brushSelector;
    [SerializeField] private Button eraserSelector;
    [SerializeField] private Button fillSelector;

    [Header("Brush shape buttons")]
    [SerializeField] private Button circleSelector;
    [SerializeField] private Button squareSelector;
    [SerializeField] private Button diamondSelector;
    [SerializeField] private Button calligraphySelector;

    [Header("Input Fields")]
    [SerializeField] private TMP_InputField brushSizeInput;
    [SerializeField] private GameObject brushSizeText;

    [SerializeField] private TMP_InputField opacityInput;
    [SerializeField] private GameObject opacityText;

    [SerializeField] private TMP_InputField hardnessInput;
    [SerializeField] private GameObject hardnessText;

    [SerializeField] private TMP_InputField spacingInput;
    [SerializeField] private GameObject spacingText;

    [SerializeField] private TMP_InputField calligraphyAngleInput;
    [SerializeField] private GameObject calligraphyAngleText;

    [SerializeField] private TMP_InputField calligraphyAspectInput;
    [SerializeField] private GameObject calligraphyAspectText;

    private void Awake()
    {
        brushSelector.onClick.AddListener(() => { settingsChanger.SetTool(ToolType.Brush); UpdateVisibleSettings(); });
        eraserSelector.onClick.AddListener(() => { settingsChanger.SetTool(ToolType.Eraser); UpdateVisibleSettings(); });
        fillSelector.onClick.AddListener(() => { settingsChanger.SetTool(ToolType.Fill); UpdateVisibleSettings(); });

        circleSelector.onClick.AddListener(() => { settingsChanger.SetBrushShape(BrushShape.Circle); UpdateVisibleSettings(); SetCalligraphyUIActive(false); });
        squareSelector.onClick.AddListener(() => { settingsChanger.SetBrushShape(BrushShape.Square); UpdateVisibleSettings(); SetCalligraphyUIActive(false); });
        diamondSelector.onClick.AddListener(() => { settingsChanger.SetBrushShape(BrushShape.Diamond); UpdateVisibleSettings(); SetCalligraphyUIActive(false); });
        calligraphySelector.onClick.AddListener(() => { settingsChanger.SetBrushShape(BrushShape.Calligraphy); UpdateVisibleSettings(); SetCalligraphyUIActive(true); });

        RegisterBrushSizeInput();
        RegisterPercentInput(opacityInput, settingsChanger.SetOpacity);
        RegisterPercentInput(hardnessInput, settingsChanger.SetHardness);
        RegisterPercentInput(spacingInput, settingsChanger.SetSpacing);
        RegisterDegreeInput(calligraphyAngleInput, settingsChanger.SetCalligraphyAngle);
        RegisterPercentInput(calligraphyAspectInput, settingsChanger.SetCalligraphyAspect);
    }

    private void Start()
    {
        UpdateVisibleSettings();
    }
    
    public void RefreshFromProjectSettings()
    {
        UpdateVisibleSettings();
    }

    private void RegisterBrushSizeInput()
    {
        brushSizeInput.onEndEdit.AddListener(text =>
        {
            if (!int.TryParse(text, out int value))
                return;

            value = Mathf.Clamp(value, 1, 512);
            brushSizeInput.text = value.ToString();
            settingsChanger.SetBrushSize(value);
        });
    }

    private void RegisterPercentInput(
        TMP_InputField input,
        System.Action<float> onValueChanged)
    {
        input.text = "100";

        input.onEndEdit.AddListener(text =>
        {
            if (!int.TryParse(text, out int value))
                return;

            value = Mathf.Clamp(value, 1, 100);
            input.text = value.ToString();
            onValueChanged(value / 100f);
        });
    }

    private void RegisterDegreeInput(
        TMP_InputField input,
        System.Action<float> onValueChanged)
    {
        input.text = "0";

        input.onEndEdit.AddListener(text =>
        {
            if (!float.TryParse(text, out float value))
                return;

            value = Mathf.Clamp(value, 0f, 360f);
            input.text = Mathf.RoundToInt(value).ToString();
            onValueChanged(value);
        });
    }

    private void UpdateVisibleSettings()
    {
        var ps = ProjectSettingsManager.Instance.currentProjectSettings;

        ToolType tool = ps.currentTool;
        BrushShape shape = ps.brushShape;

        bool showShapes = tool != ToolType.Fill;
        circleSelector.gameObject.SetActive(showShapes);
        squareSelector.gameObject.SetActive(showShapes);
        diamondSelector.gameObject.SetActive(showShapes);
        calligraphySelector.gameObject.SetActive(showShapes);

        SetField(brushSizeInput, brushSizeText, tool != ToolType.Fill, ps.brushSize);
        SetPercentField(opacityInput, opacityText, tool != ToolType.Fill, ps.brushOpacity);
        SetPercentField(hardnessInput, hardnessText, tool != ToolType.Fill, ps.brushHardness);
        SetPercentField(spacingInput, spacingText, tool != ToolType.Fill, ps.brushSpacing);

        bool isCalligraphy = shape == BrushShape.Calligraphy;
        SetDegreeField(calligraphyAngleInput, calligraphyAngleText, isCalligraphy, ps.calligraphyAngle);
        SetPercentField(calligraphyAspectInput, calligraphyAspectText, isCalligraphy, ps.calligraphyAspect);
    }

    private void SetField(
        TMP_InputField input,
        GameObject label,
        bool visible,
        int value)
    {
        input.gameObject.SetActive(visible);
        label.SetActive(visible);

        if (visible)
            input.text = value.ToString();
    }

    private void SetPercentField(
        TMP_InputField input,
        GameObject label,
        bool visible,
        float value)
    {
        input.gameObject.SetActive(visible);
        label.SetActive(visible);

        if (visible)
            input.text = Mathf.RoundToInt(value * 100f).ToString();
    }

    private void SetDegreeField(
        TMP_InputField input,
        GameObject label,
        bool visible,
        float value)
    {
        input.gameObject.SetActive(visible);
        label.SetActive(visible);

        if (visible)
            input.text = Mathf.RoundToInt(value).ToString();
    }
    
    public void SetCalligraphyUIActive(bool isActive)
    {
        calligraphyAngleInput.gameObject.SetActive(isActive);
        calligraphyAngleText.SetActive(isActive);

        calligraphyAspectInput.gameObject.SetActive(isActive);
        calligraphyAspectText.SetActive(isActive);
    }
}

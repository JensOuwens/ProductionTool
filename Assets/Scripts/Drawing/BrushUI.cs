using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RebuildCursor : MonoBehaviour
{
    [SerializeField] private DrawingManager drawingManager;

    [Header("Brush type buttons")]
    [SerializeField] private Button brushSelector;
    [SerializeField] private Button eraserSelector;
    [SerializeField] private Button fillSelector;

    [Header("Brush shape buttons")]
    [SerializeField] private Button circleSelector;
    [SerializeField] private Button squareSelector;
    [SerializeField] private Button diamondSelector;
    [SerializeField] private Button calligraphySelector;

    [Header("Sliders and their parent GameObjects")]
    [SerializeField] private Slider brushSizeSlider;
    [SerializeField] private GameObject brushSizeText;

    [SerializeField] private Slider opacitySlider;
    [SerializeField] private GameObject opacityText;

    [SerializeField] private Slider hardnessSlider;
    [SerializeField] private GameObject hardnessText;

    [SerializeField] private Slider spacingSlider;
    [SerializeField] private GameObject spacingText;

    [SerializeField] private Slider calligraphyAngleSlider;
    [SerializeField] private GameObject calligraphyAngleText;

    [SerializeField] private Slider calligraphyAspectSlider;
    [SerializeField] private GameObject calligraphyAspectText;
    
    private void Awake()
    {
        // Tool buttons
        brushSelector.onClick.AddListener(() => { drawingManager.SetTool(ToolType.Brush); UpdateVisibleSettings(); });
        eraserSelector.onClick.AddListener(() => { drawingManager.SetTool(ToolType.Eraser); UpdateVisibleSettings(); });
        fillSelector.onClick.AddListener(() => { drawingManager.SetTool(ToolType.Fill); UpdateVisibleSettings(); });

        // Shape buttons
        circleSelector.onClick.AddListener(() => { drawingManager.SetBrushShape(BrushShape.Circle); UpdateVisibleSettings(); });
        squareSelector.onClick.AddListener(() => { drawingManager.SetBrushShape(BrushShape.Square); UpdateVisibleSettings(); });
        diamondSelector.onClick.AddListener(() => { drawingManager.SetBrushShape(BrushShape.Diamond); UpdateVisibleSettings(); });
        calligraphySelector.onClick.AddListener(() => { drawingManager.SetBrushShape(BrushShape.Calligraphy); UpdateVisibleSettings(); });

        // Sliders
        brushSizeSlider.onValueChanged.AddListener(val => drawingManager.SetBrushSize(Mathf.RoundToInt(val)));
        opacitySlider.onValueChanged.AddListener(drawingManager.SetOpacity);
        hardnessSlider.onValueChanged.AddListener(drawingManager.SetHardness);
        spacingSlider.onValueChanged.AddListener(drawingManager.SetSpacing);
        calligraphyAngleSlider.onValueChanged.AddListener(drawingManager.SetCalligraphyAngle);
        calligraphyAspectSlider.onValueChanged.AddListener(drawingManager.SetCalligraphyAspect);
    }
    
    private void Start()
    {
        UpdateVisibleSettings();
    }

private void UpdateVisibleSettings()
{
    var ps = ProjectSettingsManager.Instance.currentProjectSettings;

    ToolType currentTool = ps.currentTool;
    BrushShape currentShape = ps.brushShape;

    // Show/hide shape buttons
    bool showShapes = currentTool != ToolType.Fill;
    circleSelector.gameObject.SetActive(showShapes);
    squareSelector.gameObject.SetActive(showShapes);
    diamondSelector.gameObject.SetActive(showShapes);
    calligraphySelector.gameObject.SetActive(showShapes);

    // Show/hide sliders + set slider values to current settings
    SetSliderVisibility(brushSizeSlider, brushSizeText, currentTool != ToolType.Fill);
    if (brushSizeSlider.gameObject.activeSelf)
        brushSizeSlider.value = ps.brushSize;

    SetSliderVisibility(opacitySlider, opacityText, currentTool == ToolType.Brush || currentTool == ToolType.Eraser);
    if (opacitySlider.gameObject.activeSelf)
        opacitySlider.value = ps.brushOpacity;

    SetSliderVisibility(hardnessSlider, hardnessText, currentTool == ToolType.Brush || currentTool == ToolType.Eraser);
    if (hardnessSlider.gameObject.activeSelf)
        hardnessSlider.value = ps.brushHardness;

    SetSliderVisibility(spacingSlider, spacingText, currentTool == ToolType.Brush || currentTool == ToolType.Eraser);
    if (spacingSlider.gameObject.activeSelf)
        spacingSlider.value = ps.brushSpacing;

    bool isCalligraphy = currentShape == BrushShape.Calligraphy;
    SetSliderVisibility(calligraphyAngleSlider, calligraphyAngleText, isCalligraphy);
    if (calligraphyAngleSlider.gameObject.activeSelf)
        calligraphyAngleSlider.value = ps.calligraphyAngle;

    SetSliderVisibility(calligraphyAspectSlider, calligraphyAspectText, isCalligraphy);
    if (calligraphyAspectSlider.gameObject.activeSelf)
        calligraphyAspectSlider.value = ps.calligraphyAspect;
}

private void SetSliderVisibility(Slider slider, GameObject go, bool visible)
{
    slider.gameObject.SetActive(visible);
    go.SetActive(visible);
}


    // --- Brush type selection ---
    public void SelectBrush() => drawingManager.SetTool(ToolType.Brush);
    public void SelectEraser() => drawingManager.SetTool(ToolType.Eraser);
    public void SelectFill() => drawingManager.SetTool(ToolType.Fill);

    // --- Brush shape selection ---
    public void SelectCircle() => drawingManager.SetBrushShape(BrushShape.Circle);
    public void SelectSquare() => drawingManager.SetBrushShape(BrushShape.Square);
    public void SelectDiamond() => drawingManager.SetBrushShape(BrushShape.Diamond);
    public void SelectCalligraphy() => drawingManager.SetBrushShape(BrushShape.Calligraphy);

    // --- Slider setters ---
    public void SetBrushSize(float value) => drawingManager.SetBrushSize(Mathf.RoundToInt(value));
    public void SetOpacity(float value) => drawingManager.SetOpacity(value);
    public void SetHardness(float value) => drawingManager.SetHardness(value);
    public void SetSpacing(float value) => drawingManager.SetSpacing(value);
    public void SetCalligraphyAngle(float value) => drawingManager.SetCalligraphyAngle(value);
    public void SetCalligraphyAspect(float value) => drawingManager.SetCalligraphyAspect(value);
}

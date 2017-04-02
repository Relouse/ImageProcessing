using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour {

    #region Properties
    
    public Image originalImage, extraImage;
    [SerializeField] private Button saveButton, undoButton, redoButton, loadButton, binarizationApplyButton;
    [SerializeField] private Text spriteName,spriteSize;
    [SerializeField] private GameObject _binarizationWindowTools;
    [SerializeField] private Slider binarizationBorderSlider;

    public GameObject BinarizationWindowTools
    {
        get
        {
            return _binarizationWindowTools;
        }
    }
    public Color[] OriginalImageSpritePixels
    {
        get
        {
            return originalImagePixels;
        }
    }
    private Color[] originalImagePixels;
    [SerializeField]
    private Dropdown imageProcessingMethodsDropdown;
    private bool _mouseOnOriginalImage;
    public bool MouseOnOriginalImage
    {
        get
        {
            return _mouseOnOriginalImage;
        }
        set
        {
            _mouseOnOriginalImage = value;
        }
    }
    #region Singleton
    private static UIManager instance = null;
    public static UIManager Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion
    #endregion
    #region Methods
    #region UnityMethods
    private void Awake()
    {
        #region Singleton implementation
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
        #endregion
    }
    private void Start()
    {
        originalImagePixels = originalImage.sprite.texture.GetPixels();
        Texture2D temp = new Texture2D(originalImage.sprite.texture.width, originalImage.sprite.texture.height);
        temp.SetPixels(originalImagePixels);
        temp.Apply();

        extraImage.sprite = Sprite.Create(temp, new Rect(0, 0, temp.width, temp.height), new Vector2(0.5f, 0.5f));
        extraImage.preserveAspect = true;

        ChooseEffect(0);

        imageProcessingMethodsDropdown.onValueChanged.AddListener(ChooseEffect);
        saveButton.onClick.AddListener(SaveImage);
        undoButton.onClick.AddListener(ApplicationManager.Instance.Undo);
        redoButton.onClick.AddListener(ApplicationManager.Instance.Redo);
        binarizationApplyButton.onClick.AddListener(ApplyBinarizationButton);
        binarizationBorderSlider.onValueChanged.AddListener(ChangeBinarizationBorderSliderValueAndText);
        spriteName.text = "File: " + "<b><color=yellow>" + originalImage.sprite.name + ".png</color></b>";
        spriteSize.text = "Size: " + "<b><color=yellow>" + originalImage.sprite.texture.width + "x" + originalImage.sprite.texture.height + "</color></b>";
    }
    #endregion
    public void ChangeBinarizationBorderSliderValueAndText(float value)
    {
        ImageProcessingManager.Instance.ChangeBinarizationBorderValue(binarizationBorderSlider.value / binarizationBorderSlider.maxValue);
        binarizationBorderSlider.transform.FindChild("Handle Slide Area/Handle/Text").GetComponent<Text>().text = ((int)value).ToString();
    }
   private void ApplyBinarizationButton()
    {
        BinarizationWindowTools.SetActive(false);
        ImageProcessingManager.Instance.ApplyBinarization();
    }
    public void FillImageProcessingMethods()
    {
        imageProcessingMethodsDropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (var item in ImageProcessingManager.Instance.AvaibleProcessingMethods)
        {
            options.Add(new Dropdown.OptionData { text = item.Key });
        }
        imageProcessingMethodsDropdown.AddOptions(options);
    }
    private void ChooseEffect(int numb)
    {
        if (!ApplicationManager.Instance.DoingRedoOrUndo)
        {
            ImageProcessingManager.Instance.AvaibleProcessingMethods[imageProcessingMethodsDropdown.options[numb].text].DynamicInvoke(extraImage.sprite);
            ApplicationManager.Instance.SaveState(originalImage.sprite, extraImage.sprite, numb);
        }
    }
    private void SaveImage()
    {
        byte[] bytes = extraImage.sprite.texture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/Image.png", bytes);
    }
    public void SetEffectsDropdownCurrentValue(int i)
    {
        imageProcessingMethodsDropdown.value = i;
    }
    public void SetUndoButtonInteractable(bool interactable)
    {
        undoButton.interactable = interactable;
    }
    public void SetRedoButtonInteractable(bool interactable)
    {
        redoButton.interactable = interactable;
    }
    #endregion
}

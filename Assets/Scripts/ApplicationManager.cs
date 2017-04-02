using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour {

    #region Properties
    #region Singleton
    private static ApplicationManager instance = null;
    public static ApplicationManager Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    private int currentState, minState = 0, maxStates;
    
    /// <summary>
    /// Сохраняем состояния, каждое состояние это массив пикселей оригинального изображения и измененного + то, что было выбрано в дропдауне эффектов
    /// </summary>
    private struct State
    {
        public Color[] original, edited;
        public int effectsDropdownValue;
    }

    private List<State> states = new List<State>();

    private bool doingRedoOrUndo = false;//Нужно чтобы проверять, делали ли мы в данном шаге redo или undo
    public bool DoingRedoOrUndo
    {
        get
        {
            return doingRedoOrUndo;
        }
    }
    
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
        currentState = -1;
      
    }
    private void Update()
    {
       if(UIManager.Instance.MouseOnOriginalImage)
        {
            //Вывод координат

           
        }
    }
    #endregion

    public void SaveState(Sprite originalSprite, Sprite extraSprite, int dropDownValue)
    {

        states.Add(new State { original = originalSprite.texture.GetPixels(), edited = extraSprite.texture.GetPixels(), effectsDropdownValue = dropDownValue });
       
        maxStates = states.Count;
        currentState++;
        CheckStatesBordersAndSetUndoRedoInteractable();
    }
    private void CheckStatesBordersAndSetUndoRedoInteractable()
    {
        if (currentState == minState) UIManager.Instance.SetUndoButtonInteractable(false);
        else UIManager.Instance.SetUndoButtonInteractable(true);
        if (currentState == maxStates - 1) UIManager.Instance.SetRedoButtonInteractable(false);
        else UIManager.Instance.SetRedoButtonInteractable(true);
    }
    public void Undo()
    {
        
        if (currentState > minState)
        {
            doingRedoOrUndo = true;
            currentState--;
            SetSpritesByCurrentState();
            CheckStatesBordersAndSetUndoRedoInteractable();
        }
        
    }
    public void Redo()
    {
        
        if (currentState < maxStates - 1)
        {
            doingRedoOrUndo = true;
            currentState++;
            SetSpritesByCurrentState();
            CheckStatesBordersAndSetUndoRedoInteractable();
        }
    }
    private void SetSpritesByCurrentState()
    {
        UIManager.Instance.originalImage.sprite.texture.SetPixels(states[currentState].original);
        UIManager.Instance.originalImage.sprite.texture.Apply();
        UIManager.Instance.extraImage.sprite.texture.SetPixels(states[currentState].edited);
        UIManager.Instance.extraImage.sprite.texture.Apply();
        UIManager.Instance.SetEffectsDropdownCurrentValue(states[currentState].effectsDropdownValue);
        doingRedoOrUndo = false;//Присваивание нужно именно тут чтобы избежать лишнего вызова ChooseEffect, который вызывается из-за смены значения в Dropdown эффектов
    }
    #endregion
}

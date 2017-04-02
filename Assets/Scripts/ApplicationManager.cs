using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    #region Properties

    #region Singleton

    public static ApplicationManager Instance { get; private set; }

    #endregion

    private int _currentState;
    private readonly int minState = 0;
    private int _maxStates;

    /// <summary>
    ///     Сохраняем состояния, каждое состояние это массив пикселей оригинального изображения и измененного + то, что было
    ///     выбрано в дропдауне эффектов
    /// </summary>
    private struct State
    {
        public Color[] Original, Edited;
        public int EffectsDropdownValue;
    }

    private readonly List<State> _states = new List<State>();

    public bool DoingRedoOrUndo { get; private set; }

    #endregion

    #region Methods

    #region UnityMethods

    private void Awake()
    {
        #region Singleton implementation

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }

        #endregion
    }

    private void Start()
    {
        _currentState = -1; //Чтобы сохранить стартовое состояние как 0вое счетчик должен начинать с -1
    }

    #endregion

    /// <summary>
    ///     Сохранение состояния
    /// </summary>
    /// <param name="originalSprite">Оригинальное изображение</param>
    /// <param name="extraSprite">Измененное изображение</param>
    /// <param name="dropDownValue">Текущее значение в списке эффектов</param>
    public void SaveState(Sprite originalSprite, Sprite extraSprite, int dropDownValue)
    {
        _states.Add(new State
        {
            Original = originalSprite.texture.GetPixels(),
            Edited = extraSprite.texture.GetPixels(),
            EffectsDropdownValue = dropDownValue
        });

        _maxStates = _states.Count;
        _currentState++;
        CheckStatesBordersAndSetUndoRedoInteractable();
    }

    /// <summary>
    ///     Проверяет нужно ли включить/выключить стрелки шага назад и вперед
    /// </summary>
    private void CheckStatesBordersAndSetUndoRedoInteractable()
    {
        if (_currentState == minState) UIManager.Instance.SetUndoButtonInteractable(false);
        else UIManager.Instance.SetUndoButtonInteractable(true);
        if (_currentState == _maxStates - 1) UIManager.Instance.SetRedoButtonInteractable(false);
        else UIManager.Instance.SetRedoButtonInteractable(true);
    }

    /// <summary>
    ///     Реализация шага назад
    /// </summary>
    public void Undo()
    {
        if (_currentState > minState)
        {
            DoingRedoOrUndo = true;
            _currentState--;
            SetSpritesByCurrentState();
            CheckStatesBordersAndSetUndoRedoInteractable();
        }
    }

    /// <summary>
    ///     Реализация шага вперед
    /// </summary>
    public void Redo()
    {
        if (_currentState < _maxStates - 1)
        {
            DoingRedoOrUndo = true;
            _currentState++;
            SetSpritesByCurrentState();
            CheckStatesBordersAndSetUndoRedoInteractable();
        }
    }

    /// <summary>
    ///     Устанавливает спрайт, который был сохранен в текущем состоянии
    /// </summary>
    private void SetSpritesByCurrentState()
    {
        UIManager.Instance.OriginalImage.sprite.texture.SetPixels(_states[_currentState].Original);
        UIManager.Instance.OriginalImage.sprite.texture.Apply();
        UIManager.Instance.ExtraImage.sprite.texture.SetPixels(_states[_currentState].Edited);
        UIManager.Instance.ExtraImage.sprite.texture.Apply();

        UIManager.Instance.SetEffectsDropdownCurrentValue(_states[_currentState].EffectsDropdownValue);

        DoingRedoOrUndo = false;
            //Присваивание нужно именно тут чтобы избежать лишнего вызова ChooseEffect, который вызывается из-за смены значения в Dropdown эффектов
    }

    #endregion
}
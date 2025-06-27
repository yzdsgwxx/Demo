using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/**
 * Title:
 * Description:
 */
public class BindingPanel : MonoBehaviour
{
    private Dictionary<string, KeyBindingItem> keyBindingItemsMap = new Dictionary<string, KeyBindingItem>();
    private List<KeyBindingItem> keyBindingItemsList = new List<KeyBindingItem>();
    [SerializeField] private Color normalColor;
    [SerializeField] private Color conflictColor;

    private void Awake()
    {
        //ToDictionary(item => key,item => value)
        var list = GetComponentsInChildren<KeyBindingItem>();
        keyBindingItemsMap = list.ToDictionary(item => item.action, item => item);
        keyBindingItemsList = list.ToList<KeyBindingItem>();
    }

    private void Start()
    {
        InputManager.instance.confChanged += UpdatePanel;
        InputManager.instance.confCanceled += (string currentBindingAction) =>
        {
            keyBindingItemsMap[currentBindingAction].UpdateKeyText();
        };
        UpdatePanel(InputManager.instance.GetConflictActions());
    }

    private void UpdatePanel(List<string> conflictActions)
    {
        foreach (var item in keyBindingItemsList)
        {
            item.SetKeyText(InputManager.instance.GetKey(item.action).ToString());
            bool bConflict = conflictActions.Contains(item.action);
            item.SetKeyTextColor(bConflict ? conflictColor : normalColor);
        }
    }

    public void OnBtnResetClicked()
    {
        InputManager.instance.ResetBinding();
    }
}

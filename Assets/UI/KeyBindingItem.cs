
using System;
using TMPro;
using UnityEngine;

/**
 * Title:
 * Description:
 */
//破按钮点击之后被聚焦，按空格会再次触发。需要Navigation = none.
public class KeyBindingItem : MonoBehaviour
{
    [SerializeField] private MyInputAction myAction;
    [SerializeField] private TMP_Text text_key;
    [SerializeField] private TMP_Text text_dec;

    //编辑器中看到效果。
    private void OnValidate()
    {
        text_dec.text = myAction.ToString();
    }
    private void Start()
    {
        InputManager.instance.bindingChanged += UpdateText;
        UpdateText();
    }
    private void UpdateText()
    {
        text_key.text = InputManager.instance.GetKey(myAction).ToString();
    }

    public void OnBtnClicked()
    {
        text_key.text = "?";
        InputManager.instance.StartBinding(myAction);
    }
}

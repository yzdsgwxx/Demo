
using System;
using TMPro;
using UnityEngine;

/**
 * Title:
 * Description:
 */
//�ư�ť���֮�󱻾۽������ո���ٴδ�������ҪNavigation = none.
public class KeyBindingItem : MonoBehaviour
{
    [SerializeField] public string action = "action";
    [SerializeField] public TMP_Text text_key;
    [SerializeField] private TMP_Text text_dec;
    //�༭�����Զ�����Ϊaciton
    private void OnValidate()
    {
        text_dec.text = action;
    }

    public void SetKeyText(string text)
    {
        text_key.text = text;
    }

    public void SetKeyTextColor(Color color)
    {
        text_key.color = color;
    }

    public void UpdateKeyText()
    {
        text_key.text = InputManager.instance.GetKey(action).ToString();
    }

    public void OnBtnClicked()
    {
        text_key.text = "?";
        InputManager.instance.StartBinding(action);
    }
}

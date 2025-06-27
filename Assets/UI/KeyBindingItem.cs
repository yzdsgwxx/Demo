
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
    [SerializeField] private MyInputAction myAction;
    [SerializeField] private TMP_Text text_key;
    [SerializeField] private TMP_Text text_dec;

    //�༭���п���Ч����
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

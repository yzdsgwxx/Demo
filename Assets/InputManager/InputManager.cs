using System;
using System.Collections.Generic;
using UnityEngine;

/**
* Title:���������
* Description:������������֧�ְ���������ӳ�䰴�����ṩInputActionö�١�
* ȱ�㣺ÿ����Ŀ��InputAction���ǲ�һ���ģ���Ҫ������ļ���������ģ�黯��׼���Ժ���������Ŀ����������˵
* �ŵ㣺���õ���ģʽ��
* TODO:����ظ��������ڱ༭�����MyInputAction����������Ĭ�ϰ����������ܵ�����������
*/
public enum MyInputAction
{
    None,
    Jump,
    BackPack,
    Drop,
    Attack,
    Use,
    HideUI,
    TakePhoto,
}

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public Action bindingChanged;
    private MyInputAction currentBindingAction;
    private bool isRebinding = false;

    readonly Dictionary<MyInputAction, KeyCode> defaultInputMapping = new Dictionary<MyInputAction, KeyCode>()
    {
        {MyInputAction.Jump,KeyCode.Space},
        {MyInputAction.BackPack,KeyCode.E },
        {MyInputAction.Drop,KeyCode.Q },
        {MyInputAction.Attack,KeyCode.Mouse0 },
        {MyInputAction.Use,KeyCode.Mouse1 },
        {MyInputAction.HideUI,KeyCode.F1 },
        {MyInputAction.TakePhoto,KeyCode.F2 }
    };

    const string bUseDefaultKey = "bUseDefaultInputMapping";
    int bUseDefault = 1;

    private void Awake()
    {
        instance = this;
        Load();
        DontDestroyOnLoad(instance);
    }
    private void Update()
    {
        DetectBindingKey();
    }
    //���ڲ�ѯĳ��Action�İ����Ƿ񱻰��£�����ִ���߼�
    public bool IsActionTriggered(MyInputAction action)
    {
        return Input.GetKeyDown(currentInputMapping[action]);
    }

    //���ڿ�ʼ��ĳ��Action
    public void StartBinding(MyInputAction bindingAction)
    {
        Debug.Log(string.Format("��ʼ��:{0}", bindingAction));
        currentBindingAction = bindingAction;
        isRebinding = true;
    }

    //��ESCȡ���󶨡�
    public void CancelBinding()
    {
        currentBindingAction = MyInputAction.None;
        isRebinding = false;
        Debug.Log("ȡ����");
        bindingChanged.Invoke();
    }

    //���ڲ�ѯ��ǰAction��Ӧ�İ�������UI.
    public KeyCode GetKey(MyInputAction action)
    {
        return currentInputMapping[action];
    }

    private void Rebind(MyInputAction action, KeyCode keycode)
    {
        Debug.Log("���°󶨰���������");
        SetCurrentInputMapping(action, keycode);
        bUseDefault = 0;
        Save();
        ShowCurrent();
    }

    private Dictionary<MyInputAction, KeyCode> currentInputMapping = new Dictionary<MyInputAction, KeyCode>();
    void SetCurrentInputMapping(MyInputAction action, KeyCode key)
    {
        currentInputMapping[action] = key;
        bindingChanged?.Invoke();
    }

    void SetCurrentInputMapping(Dictionary<MyInputAction, KeyCode> otherMapping)
    {
        //������ǳ����.
        //currentInputMapping = otherMapping;
        currentInputMapping = new Dictionary<MyInputAction, KeyCode>(otherMapping); //���������
        bindingChanged?.Invoke();
    }
    private void Load()
    {
        bUseDefault = PlayerPrefs.GetInt(bUseDefaultKey);
        if (bUseDefault == 1)
        {
            Debug.Log("ʹ��Ĭ�ϼ�λ..");
            SetCurrentInputMapping(defaultInputMapping);
        }
        else
        {
            Debug.Log("���ؼ�λ..");
            foreach (MyInputAction action in Enum.GetValues(typeof(MyInputAction)))
            {
                int value = PlayerPrefs.GetInt(action.ToString());
                SetCurrentInputMapping(action, (KeyCode)value);
            }
            ShowCurrent();
        }
    }

    public void ResetBinding()
    {
        Debug.Log("���ð���..");
        bUseDefault = 1;
        SetCurrentInputMapping(defaultInputMapping);
        Save();
    }

    private void Save()
    {
        Debug.Log("���水���󶨡�����");
        foreach (KeyValuePair<MyInputAction, KeyCode> pair in currentInputMapping)
        {
            PlayerPrefs.SetInt(pair.Key.ToString(), (int)pair.Value);
        }
        PlayerPrefs.SetInt(bUseDefaultKey, bUseDefault);
        ShowSaved();
    }

    public void ShowCurrent()
    {
        Debug.Log("��ʾ��ǰ������");
        foreach (KeyValuePair<MyInputAction, KeyCode> pair in currentInputMapping)
        {
            Debug.Log(string.Format("action:{0},keyCode:{1}", pair.Key, pair.Value));
        }
        Debug.Log(string.Format("busedefault:{0}", bUseDefault));

    }

    public void ShowSaved()
    {
        Debug.Log("��ʾ����İ�����");
        foreach (MyInputAction action in Enum.GetValues(typeof(MyInputAction)))
        {
            KeyCode key = (KeyCode)(PlayerPrefs.GetInt(action.ToString()));
            Debug.Log(string.Format("action:{0},keyCode:{1}", action, key));
        }
        Debug.Log(string.Format("busedefault:{0}", PlayerPrefs.GetInt(bUseDefaultKey)));
    }

    private void DetectBindingKey()
    {
        if (isRebinding)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelBinding();
            }
            else if (Input.anyKeyDown)
            {
                KeyCode pressedKey = KeyCode.None;
                foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        pressedKey = keyCode;
                        break;
                    }
                }
                //����ģʽ����дStatic�ġ�Static���ǡ����������ࡣ
                Rebind(currentBindingAction, pressedKey);
                currentBindingAction = MyInputAction.None;
                isRebinding = false;
                Debug.Log("�󶨳ɹ�");
            }
        }
    }

}

using System;
using System.Collections.Generic;
using UnityEngine;

/**
* Title:输入管理器
* Description:这个输入管理器支持按键和重新映射按键。提供InputAction枚举。
* 缺点：每个项目的InputAction都是不一样的，需要动这个文件，不符合模块化标准。以后做其他项目遇到问题再说
* 优点：采用单例模式。
* TODO:检测重复按键。在编辑器添加MyInputAction。并且配置默认按键，不用跑到代码里来。
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
    //用于查询某个Action的按键是否被按下，用于执行逻辑
    public bool IsActionTriggered(MyInputAction action)
    {
        return Input.GetKeyDown(currentInputMapping[action]);
    }

    //用于开始绑定某个Action
    public void StartBinding(MyInputAction bindingAction)
    {
        Debug.Log(string.Format("开始绑定:{0}", bindingAction));
        currentBindingAction = bindingAction;
        isRebinding = true;
    }

    //按ESC取消绑定。
    public void CancelBinding()
    {
        currentBindingAction = MyInputAction.None;
        isRebinding = false;
        Debug.Log("取消绑定");
        bindingChanged.Invoke();
    }

    //用于查询当前Action对应的按键更新UI.
    public KeyCode GetKey(MyInputAction action)
    {
        return currentInputMapping[action];
    }

    private void Rebind(MyInputAction action, KeyCode keycode)
    {
        Debug.Log("重新绑定按键。。。");
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
        //产生了浅拷贝.
        //currentInputMapping = otherMapping;
        currentInputMapping = new Dictionary<MyInputAction, KeyCode>(otherMapping); //这是深拷贝。
        bindingChanged?.Invoke();
    }
    private void Load()
    {
        bUseDefault = PlayerPrefs.GetInt(bUseDefaultKey);
        if (bUseDefault == 1)
        {
            Debug.Log("使用默认键位..");
            SetCurrentInputMapping(defaultInputMapping);
        }
        else
        {
            Debug.Log("加载键位..");
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
        Debug.Log("重置按键..");
        bUseDefault = 1;
        SetCurrentInputMapping(defaultInputMapping);
        Save();
    }

    private void Save()
    {
        Debug.Log("保存按键绑定。。。");
        foreach (KeyValuePair<MyInputAction, KeyCode> pair in currentInputMapping)
        {
            PlayerPrefs.SetInt(pair.Key.ToString(), (int)pair.Value);
        }
        PlayerPrefs.SetInt(bUseDefaultKey, bUseDefault);
        ShowSaved();
    }

    public void ShowCurrent()
    {
        Debug.Log("显示当前按键绑定");
        foreach (KeyValuePair<MyInputAction, KeyCode> pair in currentInputMapping)
        {
            Debug.Log(string.Format("action:{0},keyCode:{1}", pair.Key, pair.Value));
        }
        Debug.Log(string.Format("busedefault:{0}", bUseDefault));

    }

    public void ShowSaved()
    {
        Debug.Log("显示保存的按键绑定");
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
                //单例模式不用写Static的。Static那是。。。工具类。
                Rebind(currentBindingAction, pressedKey);
                currentBindingAction = MyInputAction.None;
                isRebinding = false;
                Debug.Log("绑定成功");
            }
        }
    }

}

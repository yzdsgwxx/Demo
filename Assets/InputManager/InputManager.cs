using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
* Title:输入管理器
* Description:重新绑定按键，配置按键，重置按键，取消绑定，
* 三种按键状态，检测重复按键，键位变更通知、绑定取消通知。
* 优点：模块化。
* TODO:限定可绑定按键。
*/

//配置相关必须是Public
//用于序列化，与ScriptableObject结合使用。
[System.Serializable]
public struct KeyConfig
{
    public string action;
    public KeyCode defaultKey;
    public KeyCode currentKey;
}

public enum KeyState
{
    Down,
    Hold,
    Up
}
//可以在编辑器中配置。
[CreateAssetMenu(fileName = "InputMappingConfig", menuName = "InputManager/InputMappingConfig")]
public class InputMappingConfig : ScriptableObject
{
    //为了方便修改和查找改用Dictionary。 //配置只能用List.............啊啊啊啊啊
    public List<KeyConfig> InputMapping = new List<KeyConfig>();
}

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    [SerializeField] private InputMappingConfig _inputconfig;
    //方便点。。。
    public List<KeyConfig> InputConfig
    {
        get
        {
            return _inputconfig.InputMapping;
        }
    }
    /// <summary>
    /// 广播冲突action
    /// </summary>
    public Action<List<string>> confChanged;
    /// <summary>
    /// 广播取消绑定的按键
    /// </summary>
    public Action<string> confCanceled;
    private string currentBindingAction;

    private bool isRebinding = false;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(instance);
    }

    private void Start()
    {
    }
    private void Update()
    {
        DetectBindingKey();
    }
    //用于查询某个Action的按键是否被按下，用于执行逻辑
    public bool IsActionInState(string action, KeyState state = KeyState.Down)
    {
        KeyCode key = GetKey(action);
        if (state == KeyState.Down)
        {
            return Input.GetKeyDown(key);
        }
        else if (state == KeyState.Hold)
        {
            return Input.GetKey(key);
        }
        else if (state == KeyState.Up)
        {
            return Input.GetKeyUp(key);
        }
        return false;
    }

    //用于开始绑定某个Action
    public void StartBinding(string bindingAction)
    {
        Debug.Log(string.Format("开始绑定:{0}", bindingAction));
        currentBindingAction = bindingAction;
        isRebinding = true;
    }

    //按ESC取消绑定。
    public void CancelBinding()
    {
        confCanceled?.Invoke(currentBindingAction);
        currentBindingAction = "";
        isRebinding = false;
        Debug.Log("取消绑定");

    }

    //用于查询当前Action对应的按键更新UI.
    public KeyCode GetKey(string action)
    {
        foreach (KeyConfig keyconfig in InputConfig)
        {
            if (keyconfig.action == action)
            {
                return keyconfig.currentKey;
            }
        }
        return KeyCode.None;
    }

    public int FindKeyConfig(string action)
    {
        for (int i = 0; i < InputConfig.Count; i++)
        {
            if (InputConfig[i].action == action)
            {
                return i;
            }
        }
        return -1;
    }

    private void Rebind(string action, KeyCode keycode)
    {
        Debug.Log("重新绑定按键。。。");
        //修改结构体（值类型）
        int index = FindKeyConfig(action);
        if (index != -1)
        {
            KeyConfig conf = InputConfig[index];
            conf.currentKey = keycode;
            InputConfig[index] = conf;
            confChanged?.Invoke(GetConflictActions());
        }
        ShowConfig();
    }
    public void ResetBinding()
    {
        Debug.Log("重置按键..");
        for (int i = 0; i < InputConfig.Count; i++)
        {
            KeyConfig conf = InputConfig[i];
            conf.currentKey = conf.defaultKey;
            InputConfig[i] = conf;
        }
        confChanged?.Invoke(GetConflictActions());
    }

    public void ShowConfig()
    {
        Debug.Log("显示当前按键绑定");
        foreach (KeyConfig conf in InputConfig)
        {
            Debug.Log(string.Format("action:{0} - 默认key:{1} 当前key{2}", conf.action, conf.defaultKey, conf.currentKey));
        }

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
                currentBindingAction = "";
                isRebinding = false;
                Debug.Log("绑定成功");
            }
        }
    }

    /// <summary>
    /// 用于更新界面。
    /// </summary>
    /// <returns></returns>
    public List<string> GetConflictActions()
    {
        Dictionary<KeyCode, List<string>> map = new Dictionary<KeyCode, List<string>>();
        List<string> result = new List<string>();
        //将所有KeyCode取出，映射到绑定了他的action
        for (int i = 0; i < InputConfig.Count; i++)
        {
            KeyCode key = InputConfig[i].currentKey;
            if (!map.ContainsKey(key))
            {
                map[key] = new List<string>();
            }
            map[key].Add(InputConfig[i].action);
        }

        foreach (var pair in map)
        {
            if (pair.Value.Count > 1)
            {
                result.AddRange(pair.Value);
            }
        }

        Debug.Log(string.Format("冲突Actions:"));
        foreach (var action in result)
        {
            Debug.Log(action);
        }
        return result;
    }
}

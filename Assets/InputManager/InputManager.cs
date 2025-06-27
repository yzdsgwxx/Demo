using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
* Title:���������
* Description:���°󶨰��������ð��������ð�����ȡ���󶨣�
* ���ְ���״̬������ظ���������λ���֪ͨ����ȡ��֪ͨ��
* �ŵ㣺ģ�黯��
* TODO:�޶��ɰ󶨰�����
*/

//������ر�����Public
//�������л�����ScriptableObject���ʹ�á�
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
//�����ڱ༭�������á�
[CreateAssetMenu(fileName = "InputMappingConfig", menuName = "InputManager/InputMappingConfig")]
public class InputMappingConfig : ScriptableObject
{
    //Ϊ�˷����޸ĺͲ��Ҹ���Dictionary�� //����ֻ����List.............����������
    public List<KeyConfig> InputMapping = new List<KeyConfig>();
}

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    [SerializeField] private InputMappingConfig _inputconfig;
    //����㡣����
    public List<KeyConfig> InputConfig
    {
        get
        {
            return _inputconfig.InputMapping;
        }
    }
    /// <summary>
    /// �㲥��ͻaction
    /// </summary>
    public Action<List<string>> confChanged;
    /// <summary>
    /// �㲥ȡ���󶨵İ���
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
    //���ڲ�ѯĳ��Action�İ����Ƿ񱻰��£�����ִ���߼�
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

    //���ڿ�ʼ��ĳ��Action
    public void StartBinding(string bindingAction)
    {
        Debug.Log(string.Format("��ʼ��:{0}", bindingAction));
        currentBindingAction = bindingAction;
        isRebinding = true;
    }

    //��ESCȡ���󶨡�
    public void CancelBinding()
    {
        confCanceled?.Invoke(currentBindingAction);
        currentBindingAction = "";
        isRebinding = false;
        Debug.Log("ȡ����");

    }

    //���ڲ�ѯ��ǰAction��Ӧ�İ�������UI.
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
        Debug.Log("���°󶨰���������");
        //�޸Ľṹ�壨ֵ���ͣ�
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
        Debug.Log("���ð���..");
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
        Debug.Log("��ʾ��ǰ������");
        foreach (KeyConfig conf in InputConfig)
        {
            Debug.Log(string.Format("action:{0} - Ĭ��key:{1} ��ǰkey{2}", conf.action, conf.defaultKey, conf.currentKey));
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
                //����ģʽ����дStatic�ġ�Static���ǡ����������ࡣ
                Rebind(currentBindingAction, pressedKey);
                currentBindingAction = "";
                isRebinding = false;
                Debug.Log("�󶨳ɹ�");
            }
        }
    }

    /// <summary>
    /// ���ڸ��½��档
    /// </summary>
    /// <returns></returns>
    public List<string> GetConflictActions()
    {
        Dictionary<KeyCode, List<string>> map = new Dictionary<KeyCode, List<string>>();
        List<string> result = new List<string>();
        //������KeyCodeȡ����ӳ�䵽��������action
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

        Debug.Log(string.Format("��ͻActions:"));
        foreach (var action in result)
        {
            Debug.Log(action);
        }
        return result;
    }
}

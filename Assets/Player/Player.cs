using UnityEngine;

/**
 * Title:
 * Description:
 */
public class Player : MonoBehaviour
{
    //TODO ����߸ļ��ߴ���������ʵ������Ϸ������ͣ�ģ��������Ϸ���롣
    private void LateUpdate()
    {
        if (InputManager.instance.IsActionInState("jump", KeyState.Down))
        {
            Debug.Log("��Ծ");
        }
        if (InputManager.instance.IsActionInState("backpack", KeyState.Down))
        {
            Debug.Log("����");
        }
        if (InputManager.instance.IsActionInState("forward", KeyState.Hold))
        {
            Debug.Log("ǰ��");
        }
    }
}

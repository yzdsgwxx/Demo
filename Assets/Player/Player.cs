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
        if (InputManager.instance.IsActionTriggered(MyInputAction.Jump))
        {
            Debug.Log("Jump");
        }
    }
}

using UnityEngine;

/**
 * Title:
 * Description:
 */
public class Player : MonoBehaviour
{
    //TODO 这里边改键边触发。但其实真是游戏里是暂停的，会禁用游戏输入。
    private void LateUpdate()
    {
        if (InputManager.instance.IsActionTriggered(MyInputAction.Jump))
        {
            Debug.Log("Jump");
        }
    }
}

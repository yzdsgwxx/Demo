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
        if (InputManager.instance.IsActionInState("jump", KeyState.Down))
        {
            Debug.Log("跳跃");
        }
        if (InputManager.instance.IsActionInState("backpack", KeyState.Down))
        {
            Debug.Log("背包");
        }
        if (InputManager.instance.IsActionInState("forward", KeyState.Hold))
        {
            Debug.Log("前进");
        }
    }
}

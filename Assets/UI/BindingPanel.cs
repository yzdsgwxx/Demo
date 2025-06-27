using UnityEngine;
using UnityEngine.UI;

/**
 * Title:
 * Description:
 */
public class BindingPanel : MonoBehaviour
{
    public void OnBtnResetClicked()
    {
        InputManager.instance.ResetBinding();
    }
}

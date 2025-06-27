//可以在编辑器中配置。
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputMappingConfig", menuName = "InputManager/InputMappingConfig")]
public class InputMappingConfig : ScriptableObject
{
    //为了方便修改和查找改用Dictionary。 //配置只能用List.............啊啊啊啊啊
    public List<KeyConfig> InputMapping = new List<KeyConfig>();
}
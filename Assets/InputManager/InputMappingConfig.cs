//�����ڱ༭�������á�
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputMappingConfig", menuName = "InputManager/InputMappingConfig")]
public class InputMappingConfig : ScriptableObject
{
    //Ϊ�˷����޸ĺͲ��Ҹ���Dictionary�� //����ֻ����List.............����������
    public List<KeyConfig> InputMapping = new List<KeyConfig>();
}
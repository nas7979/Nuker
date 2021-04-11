using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어에 뭐 연결하고 그런것들 여러 스크립트에 하기 귀찮으니깐 
/// 뭐 받거나 그럴때는 그냥 여기서 연결하고 받으면 좋을 것 같음
/// </summary>
public class PlayerAdapter : MonoBehaviour
{
    public VirtualJoystick m_virtualJoystick = null;
    public Rigidbody2D m_rigidbody2D = null;
    public ParticleSystem m_Afterimage = null;
    public CameraMovement m_camera;

    public GameObject m_ArrowUI = null;
    public JoystickSprite m_JoystickSprite;
    public UnityEngine.UI.Image m_ChangeJoystickTarget = null;

    public ActionPoint m_actionPoint = null;
}

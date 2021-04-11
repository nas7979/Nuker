using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Controllers : MonoBehaviour
{
    [SerializeField] ControllerSet Set;

    [System.Serializable]
    public struct ControllerSet
    {
        [System.Serializable]
        public struct ControllerPreset
        {
            [SerializeField] Transform JumpPreset;
            public Transform GetJumpPreset { get => JumpPreset; set => JumpPreset = value; }

            [SerializeField] Transform VirtualJoystickPreset;
            public Transform GetVirtualJoystickPreset { get => VirtualJoystickPreset; set => VirtualJoystickPreset = value; }
        }

        [SerializeField] Transform Joystick;
        public Transform GetJoystick => Joystick;
        [SerializeField] Transform Jump;
        public Transform GetJump => Jump;

        [SerializeField] Transform Guide;
        public Transform GetGuide => Guide;

        [SerializeField] Transform TutorialRightJump;
        public Transform GetTutorialRightJump { get => TutorialRightJump; set => TutorialRightJump = value; }

        [SerializeField] Transform TutorialRightJoystick;
        public Transform GetTutorialRightJoystick { get => TutorialRightJoystick; set => TutorialRightJoystick = value; }

        [SerializeField] ControllerPreset LeftControllerPreset;
        public ControllerPreset GetLeftControllerPreset => LeftControllerPreset;

        [SerializeField] ControllerPreset RightControllerPreset;
        public ControllerPreset GetRightControllerPreset => RightControllerPreset;
    }

    private bool isRightHand;

    private void OnEnable()
    {
        StateListener.Instance.stateListener += Instance_stateListener;

        Menu_Buttons.OnClickStartButton += () =>
        {
            isRightHand = DataSystem.GetOption(DataSystem.Key_RightHand);
            Debug.Log("isRightHand : " + isRightHand);
            if (isRightHand)
            {
                var rightPreset = Set.GetRightControllerPreset;
                Set.GetJump.localPosition = rightPreset.GetJumpPreset.localPosition;
                Set.GetJoystick.localPosition = rightPreset.GetVirtualJoystickPreset.localPosition;
            }

            if (!isRightHand)
            {
                var leftPreset = Set.GetLeftControllerPreset;
                Set.GetJump.localPosition = leftPreset.GetJumpPreset.localPosition;
                Set.GetJoystick.localPosition = leftPreset.GetVirtualJoystickPreset.localPosition;
            }
        };
    }

    private void Instance_stateListener(Utilitys.BitFlag.BitFlag<EStateFlag> flag)
    {
        if (!isRightHand)
            return;

        if (flag.HasFlag(EStateFlag.OnTutorialJumpGuide))
            Set.GetGuide.localPosition = Set.GetTutorialRightJump.localPosition;
        if (flag.HasFlag(EStateFlag.OnTutorialDashGuide))
            Set.GetGuide.localPosition = Set.GetTutorialRightJoystick.localPosition;

    }
}

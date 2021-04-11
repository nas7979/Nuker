using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Toggle
{
    public class ToggleWithData : ToggleSystem
    {
        [SerializeField] private string Key;
        protected override void OnEnable()
        {
            bool state;
            base.OnEnable();
            ChangedState(state = DataSystem.GetOption(Key));
            DataSystem.UpdateLocalState(Key, state);
        }


        protected override void OnDisable()
        {
            base.OnDisable();
        }

        internal override void ChangedState(bool state)
        {
            base.ChangedState(state);
            DataSystem.SetOption(Key, state);
            //Debug.Log("Key : " + Key + ", State : " + state);
			SoundManager.Instance.PlaySound("SFX_Button13");
		}
	}

}
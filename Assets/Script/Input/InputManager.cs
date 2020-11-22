using System;
using Prototype.Utils;

namespace Prototype.Input
{
    public class InputManager : Singleton<InputManager>
    {
        private PlayerInputs _inputs;

        public static PlayerInputs Inputs => Instance._inputs;

        protected override void Awake()
        {
            base.Awake();
            
            _inputs = new PlayerInputs();
        }

        private void OnEnable()
        {
            Inputs.Enable();
        }

        private void OnDisable()
        {
            Inputs.Disable();
        }
    }
}
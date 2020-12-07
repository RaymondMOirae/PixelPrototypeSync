using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prototype.Gameplay.Player
{
    public class InputHandler : MonoBehaviour
    {
        private PlayerController _player;

        // Start is called before the first frame update
        void Start()
        {
            _player = transform.parent.GetComponent<PlayerController>();
            
        }
        public void MoveInput(InputAction.CallbackContext cxt)
        {
            Vector2 inputDir = cxt.ReadValue<Vector2>();
            _player.Move(inputDir);
            _player.Turn(inputDir);
        }

        public void AttackInputL(InputAction.CallbackContext cxt)
        {
            if (cxt.started)
                _player.Attack(AttackType.L);
        }
        public void AttackInputM(InputAction.CallbackContext cxt)
        {
            if(cxt.started)
                _player.Attack(AttackType.M);
        }

        public void AttackInputR(InputAction.CallbackContext cxt)
        {
            if(cxt.started)
                _player.Attack(AttackType.R);
        }

        public void Interact(InputAction.CallbackContext cxt)
        {
            if (cxt.started)
                _player.HandleInteraction();
        }

    }
}


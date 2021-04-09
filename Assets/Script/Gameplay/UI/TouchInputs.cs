using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Gameplay.Player;

namespace Prototype.Gameplay.UI
{
    public class TouchInputs : MonoBehaviour
    {
        private Dictionary<AttackType, bool> _cdStatus = new Dictionary<AttackType, bool>()
        {
            { AttackType.L, true },
            { AttackType.M, true },
            { AttackType.R, true },
            { AttackType.Rotate, true }
        };

        public void SetCDStatus(AttackType type, bool isReady)
        {
            _cdStatus[type] = isReady;
        }

        public bool GetCDStatus(AttackType type)
        {
            return _cdStatus[type];
        }

        public void TriggerCoolDownCheck(AttackType type)
        {
            BroadcastMessage("TriggerCoolDown", type);
        }
    }
}


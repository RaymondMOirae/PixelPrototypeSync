using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.RoomFacility
{
    public class PickableItem : InteractiveBase
    {
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            
        }

        // Update is called once per frame
        void Update()
        {
            base.CheckContact();
            
        }
    }

}


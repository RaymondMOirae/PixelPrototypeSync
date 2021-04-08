using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Prototype.Gameplay.Player;

namespace Prototype.Gameplay.UI
{
    public class SkillButton : MonoBehaviour
    {
        [SerializeField]private float _cd;
        [SerializeField] private AttackType _selfType;
        private Image _cdFill;
        private TouchInputs _touchInputs;


        private void Start()
        {
            _cdFill = GetComponent<Image>();
            _cdFill.fillAmount = 1.0f;
            _touchInputs = transform.parent.GetComponent<TouchInputs>();
        }

        public void TriggerCoolDown(AttackType type)
        {
            if(type == _selfType)
            {
                StartCoroutine(FillCoolDown());
            }

        }

        IEnumerator FillCoolDown()
        {
            _cdFill.fillAmount = 0.0f;
            _touchInputs.SetCDStatus(_selfType, false);
            while(_cdFill.fillAmount < 1.0f)
            {
                _cdFill.fillAmount += Time.deltaTime / _cd;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            _touchInputs.SetCDStatus(_selfType, true);
        }
    }
}


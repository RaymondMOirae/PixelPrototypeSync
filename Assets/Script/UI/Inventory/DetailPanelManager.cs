using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prototype.Input;
using Prototype.Inventory;
using Prototype.Utils;
using UnityEngine;

namespace Prototype.UI.Inventory
{
    public class DetailPanelManager : Singleton<DetailPanelManager>
    {
        private const float OffsetX = 30;
        private const float OffsetY = 30;
        [SerializeField] private GameObject Prefab;

        private readonly List<DetailPanel> _panels = new List<DetailPanel>();

        protected override void Awake()
        {
            base.Awake();
            
            GameObjectPool.PreAlloc(Prefab, 3);
        }

        public static void ShowPanel(ItemType itemType)
        {
            HideAllPanel();
            var panel = GameObjectPool.Get<DetailPanel>(Instance.Prefab);
            panel.transform.SetParent(Instance.transform, false);
            panel.ResetPivot();
            var pos = InputManager.Inputs.UI.Pointer.ReadValue<Vector2>();
            var uiScale = Vector2.one / Instance.transform.lossyScale;
            pos *= uiScale;
            var offsetPos = pos + new Vector2(OffsetX, -OffsetY);
            if (offsetPos.x + panel.Size.x > Screen.width * uiScale.x)
            {
                offsetPos.x = pos.x - OffsetX;
                panel.MovePivotRight();
            }

            if (offsetPos.y - panel.Size.y < 0)
            {
                offsetPos.y = pos.y + OffsetY;
                panel.MovePivotBottom();
            }

            panel.Show(itemType, offsetPos);
            Instance._panels.Add(panel);
        }

        public static async void HideAllPanel()
        {
            if(Instance._panels.Count <= 0)
                return;
            
            var panels = Instance._panels.ToArray();
            Instance._panels.Clear();
            
            await Task.WhenAll(panels.Select(p=>p.HideAsync()));
            
            foreach (var panel in panels)
            {
                GameObjectPool.Release(Instance.Prefab, panel);
            }
        }
    }
}
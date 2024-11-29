using System;
using Systems;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class BarrierUIController : MonoBehaviour
    {
        private ProgressBar barrierProgressBar;
        private PlayerBarrierSystem playerBarrierSystem;
        private UIDocument uiDocument;

        private void Awake()
        {
            playerBarrierSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerBarrierSystem>();

            if (playerBarrierSystem != null) playerBarrierSystem.OnBarrierChanged += OnBarrierChanged;
        }

        private void Start()
        {
            uiDocument = GetComponent<UIDocument>();
            VisualElement uiVisualELement = uiDocument.rootVisualElement;
            barrierProgressBar = uiVisualELement.Q<ProgressBar>("barrier");
        }

        private void OnDestroy()
        {
            if (playerBarrierSystem != null) playerBarrierSystem.OnBarrierChanged -= OnBarrierChanged;
        }

        private void OnBarrierChanged(float currentBarrierValue)
        {
            SetProgressBar(currentBarrierValue);
        }

        private void SetProgressBar(float currentBarrierValue)
        {
            barrierProgressBar.value = (float)Math.Round(currentBarrierValue, 2);

            if (barrierProgressBar.value > 0)
            {
                uiDocument.sortingOrder = 1;
            }
            else
            {
                uiDocument.sortingOrder = -1;
            }
        }
    }
}
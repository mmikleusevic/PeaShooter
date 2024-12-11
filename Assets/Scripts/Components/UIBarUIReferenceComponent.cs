using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    public class UIBarUIReferenceComponent : ICleanupComponentData
    {
        public Slider barrierSlider;
        public GameObject gameObject;
        public Slider hpSlider;
    }
}
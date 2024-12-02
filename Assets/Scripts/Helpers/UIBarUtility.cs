using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Helpers
{
    public static class UIBarUtility
    {
        public static void SetSliderValues(Slider slider, float minValue, float maxValue, float currentValue)
        {
            slider.minValue = minValue;
            slider.maxValue = maxValue;
            slider.value = currentValue;
        }

        public static void UpdateTransform(GameObject uiObject, float3 position, float3 offset)
        {
            uiObject.transform.position = position + offset;
        }
    }
}
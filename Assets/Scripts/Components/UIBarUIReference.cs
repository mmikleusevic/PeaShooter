using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class UIBarUIReference : ICleanupComponentData
{
    public Slider barrierSlider;
    public GameObject gameObject;
    public Slider hpSlider;
}
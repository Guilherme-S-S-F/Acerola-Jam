using Jam.Events;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChangeBar : MonoBehaviour
{
    [SerializeField]RectTransform bar;
    [SerializeField] Image image;
    float max;

    private void Start()
    {
        max = bar.rect.width;
    }

    private void OnEnable()
    {
        StaminaChangeEvent.StaminaChanged += onChangeBar;
    }

    private void OnDisable()
    {
        StaminaChangeEvent.StaminaChanged -= onChangeBar;
    }

    public void onChangeBar(float value, bool isCooldown)
    {
        Color c = (isCooldown ? Color.red : Color.white);

        image.color = c;
        bar.sizeDelta = new Vector2(Mathf.Lerp(1, max, value),bar.sizeDelta.y);
    }
}

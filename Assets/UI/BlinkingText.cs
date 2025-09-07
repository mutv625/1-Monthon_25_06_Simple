using UnityEngine;
using TMPro;

public class BlinkingText : MonoBehaviour
{
    public float blinkSpeed = 1.0f; // “_–Å‘¬“x
    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (text != null)
        {
            Color c = text.color;
            c.a = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
            text.color = c;
        }
    }
}
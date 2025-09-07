using UnityEngine;
using TMPro;

public class BlinkingText : MonoBehaviour
{
    public float blinkSpeed = 5.0f; // �_�ő��x
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
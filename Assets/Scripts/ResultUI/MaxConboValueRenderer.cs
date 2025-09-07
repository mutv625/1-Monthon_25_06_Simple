using UnityEngine;

public class MaxConboValueRenderer : MonoBehaviour
{
    private TMPro.TextMeshProUGUI text;
    [SerializeField] private SOResult result;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
        text.text = result.maxComboCounts[result.winnerPlayerID].ToString();
    }
}

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int player1CharacterIndex { get; private set; }
    public int player2CharacterIndex { get; private set; }

    void Awake()
    {
        // �V���O���g����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �V�[�����܂����ŕێ�
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // �L�����I�����ʂ��Z�b�g
    public void SetSelections(int p1Index, int p2Index)
    {
        player1CharacterIndex = p1Index;
        player2CharacterIndex = p2Index;
    }
}

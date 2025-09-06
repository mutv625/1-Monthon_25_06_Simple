using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class CharacterSelectManager : MonoBehaviour
{
    private int selectedIndex1P = 0;
    private int selectedIndex2P = 0;
    void Update()
    {
        // 決定ボタン（FightButton）は1PはJキー、2PはNum1キーで押せるように
        if (confirmButton != null && confirmButton.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.J)) // 1P用
                confirmButton.onClick.Invoke();
            if (Input.GetKeyDown(KeyCode.Keypad1)) // 2P用
                confirmButton.onClick.Invoke();
        }

            // 1P操作
            if (!player1Selection.HasValue) {
                if (Input.GetKeyDown(KeyCode.W)) // 上
                    MoveSelection(1, -1);
                if (Input.GetKeyDown(KeyCode.S)) // 下
                    MoveSelection(1, 1);
                if (Input.GetKeyDown(KeyCode.J)) // 決定
                    SelectCharacter(1, selectedIndex1P);
            }
            if (player1Selection.HasValue && Input.GetKeyDown(KeyCode.K)) // キャンセル
                CancelSelection(1, player1Selection.Value);

            // 2P操作
            if (!player2Selection.HasValue) {
                if (Input.GetKeyDown(KeyCode.UpArrow)) // 上
                    MoveSelection(2, -1);
                if (Input.GetKeyDown(KeyCode.DownArrow)) // 下
                    MoveSelection(2, 1);
                if (Input.GetKeyDown(KeyCode.Keypad1)) // 決定
                    SelectCharacter(2, selectedIndex2P);
            }
            if (player2Selection.HasValue && Input.GetKeyDown(KeyCode.Keypad2)) // キャンセル
                CancelSelection(2, player2Selection.Value);
    }

    void MoveSelection(int player, int delta)
    {
        if (player == 1)
        {
            selectedIndex1P = (selectedIndex1P + delta + 6) % 6;
            Highlight1P(selectedIndex1P);
        }
        else
        {
            selectedIndex2P = (selectedIndex2P + delta + 6) % 6;
            Highlight2P(selectedIndex2P);
        }
    }

    void Highlight1P(int index)
    {
        // 1Pの選択中キャラを強調表示する処理（例：一時的に色を変えるなど）
        for (int i = 0; i < 6; i++)
        {
            var img = player1Buttons[i].GetComponent<UnityEngine.UI.Image>();
            if (img != null)
                img.color = (i == index) ? Color.gray : Color.white;
        }
    }

    void Highlight2P(int index)
    {
        for (int i = 0; i < 6; i++)
        {
            var img = player2Buttons[i].GetComponent<UnityEngine.UI.Image>();
            if (img != null)
                img.color = (i == index) ? Color.gray : Color.white;
        }
    }

    public Button[] player1Buttons; // 1P用ボタン6個
    public Button[] player2Buttons; // 2P用ボタン6個
    public Button[] player1CancelButtons; // 1P用キャンセルボタン6個
    public Button[] player2CancelButtons; // 2P用キャンセルボタン6個
    public Button confirmButton;    // 決定ボタン
    public Image[] characterImages; // キャラ画像6個

    private int? player1Selection = null;
    private int? player2Selection = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        confirmButton.gameObject.SetActive(false);

        for (int i = 0; i < 6; i++)
        {
            int index = i;
            player1Buttons[i].onClick.AddListener(() => SelectCharacter(1, index));
            player2Buttons[i].onClick.AddListener(() => SelectCharacter(2, index));
            player1CancelButtons[i].onClick.AddListener(() => CancelSelection(1, index));
            player2CancelButtons[i].onClick.AddListener(() => CancelSelection(2, index));
            player1CancelButtons[i].gameObject.SetActive(false);
            player2CancelButtons[i].gameObject.SetActive(false);
        }

        confirmButton.onClick.AddListener(OnConfirm);
    }

    void SelectCharacter(int player, int charIndex)
    {
        if (player == 1)
        {
            player1Selection = charIndex;
            for (int i = 0; i < 6; i++)
            {
                player1Buttons[i].interactable = (i == charIndex);
                player1Buttons[i].gameObject.SetActive(i == charIndex);
                player1CancelButtons[i].gameObject.SetActive(i == charIndex);
                // 強調表示: 選択ボタンは黄色、それ以外は白
                var img = player1Buttons[i].GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                    img.color = (i == charIndex) ? Color.gray : Color.white;
            }
        }
        else
        {
            player2Selection = charIndex;
            for (int i = 0; i < 6; i++)
            {
                player2Buttons[i].interactable = (i == charIndex);
                player2Buttons[i].gameObject.SetActive(i == charIndex);
                player2CancelButtons[i].gameObject.SetActive(i == charIndex);
                var img = player2Buttons[i].GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                    img.color = (i == charIndex) ? Color.gray : Color.white;
            }
        }

        confirmButton.gameObject.SetActive(player1Selection.HasValue && player2Selection.HasValue);
    }

    void CancelSelection(int player, int charIndex)
    {
        if (player == 1 && player1Selection == charIndex)
        {
            player1Selection = null;
            for (int i = 0; i < 6; i++)
            {
                player1Buttons[i].gameObject.SetActive(true);
                player1Buttons[i].interactable = true;
                player1CancelButtons[i].gameObject.SetActive(false);
                // 色を元に戻す
                var img = player1Buttons[i].GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                    img.color = Color.white;
            }
        }
        else if (player == 2 && player2Selection == charIndex)
        {
            player2Selection = null;
            for (int i = 0; i < 6; i++)
            {
                player2Buttons[i].gameObject.SetActive(true);
                player2Buttons[i].interactable = true;
                player2CancelButtons[i].gameObject.SetActive(false);
                var img = player2Buttons[i].GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                    img.color = Color.white;
            }
        }
        confirmButton.gameObject.SetActive(player1Selection.HasValue && player2Selection.HasValue);
    }

    void OnConfirm()
    {
        GameManager.Instance.SetSelections(player1Selection.Value, player2Selection.Value);
        SceneManager.LoadScene("BattleScene"); 
    }
}

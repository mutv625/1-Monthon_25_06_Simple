using UnityEngine;

public class GenericChangeScene : MonoBehaviour
{
    [SerializeField] private string sceneName;
    
    public void ChangeScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}

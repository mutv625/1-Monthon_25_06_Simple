using UnityEngine;

public class AudioListenerManager : MonoBehaviour
{
    public void SetActiveListener()
    {
        var myListener = GetComponent<AudioListener>();
        // 自分に AudioListener がなければ何もしない
        if (myListener == null) return;

        // 既存のAudioListenerを無効化
        AudioListener[] existingListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        foreach (var listener in existingListeners)
        {
            if (listener == myListener) listener.enabled = true;
            else listener.enabled = false;
        }
    }
}

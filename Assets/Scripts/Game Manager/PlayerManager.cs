using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public CharacterStats player;

    #region Singletone
    public static PlayerManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Player Manager found.");
            return;
        }

        instance = this;
    }
    #endregion

    public delegate void OnPlayerDied();
    public OnPlayerDied onPlayerDied;

    public delegate void OnPlayerRevived();
    public OnPlayerRevived onPlayerRevived;

    public void Die()
    {
        if(onPlayerDied != null)
        {
            onPlayerDied.Invoke();
        }

        // Revive after 5 seconds.
        Invoke("Revive", 5f);

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Revive()
    {
        if(onPlayerRevived != null)
        {
            onPlayerRevived.Invoke();
        }

        player.Revive();
    }
}

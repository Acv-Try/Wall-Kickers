using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    private int coins;
    public int Coins => coins;
    #region
    private static EconomyManager _instance;
    public static EconomyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<EconomyManager>();
                if (_instance != null)
                {
                    Debug.LogWarning($"Economy Manager is not found in the scene!");
                }
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    #endregion
    public void AddCoins(int amount, SoundData data)
    {
        AudioManager.Instance.Play(data, EType_Gameplay_SFX.Coin_Collect);
        coins += amount;
        // Calls UI Manager for UI update 
    }
}

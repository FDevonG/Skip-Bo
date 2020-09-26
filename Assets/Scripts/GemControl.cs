using UnityEngine;

public class GemControl : MonoBehaviour
{

    public static GemControl Instance { get; private set; }

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void AddGems(int ammount)
    {
        LocalUser.locUser.gems += ammount;
        Database.UpdateUser("gems", LocalUser.locUser.gems);
        StartCoroutine(Notifications.Instance.SpawnNotification(ammount.ToString() +" Gems added"));
    }

    public void SubtractGems(int ammount)
    {
        LocalUser.locUser.gems -= ammount;
        Database.UpdateUser("gems", LocalUser.locUser.gems);
        StartCoroutine(Notifications.Instance.SpawnNotification(ammount.ToString() + " Gems Removed"));
    }

    public void GameGemPayOut(int cardAmmount, bool offline, bool playerWon)
    {
        if (offline)
        {
            switch (cardAmmount)
            {
                case 5:
                    if (playerWon)
                        AddGems(5);
                    else
                        AddGems(1);
                    break;

                case 10:
                    if (playerWon)
                        AddGems(6);
                    else
                        AddGems(2);
                    break;

                case 15:
                    if (playerWon)
                        AddGems(7);
                    else
                        AddGems(3);
                    break;

                case 20:
                    if (playerWon)
                        AddGems(8);
                    else
                        AddGems(4);
                    break;

                case 25:
                    if (playerWon)
                        AddGems(9);
                    else
                        AddGems(5);
                    break;

                case 30:
                    if (playerWon)
                        AddGems(10);
                    else
                        AddGems(6);
                    break;
            }
        }
        else
        {
            switch (cardAmmount)
            {
                case 5:
                    if (playerWon)
                        AddGems(7);
                    else
                        AddGems(2);
                    break;

                case 10:
                    if (playerWon)
                        AddGems(8);
                    else
                        AddGems(3);
                    break;

                case 15:
                    if (playerWon)
                        AddGems(9);
                    else
                        AddGems(4);
                    break;

                case 20:
                    if (playerWon)
                        AddGems(10);
                    else
                        AddGems(5);
                    break;

                case 25:
                    if (playerWon)
                        AddGems(11);
                    else
                        AddGems(6);
                    break;

                case 30:
                    if (playerWon)
                        AddGems(10);
                    else
                        AddGems(7);
                    break;
            }
        }
    }
}

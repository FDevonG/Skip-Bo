using UnityEngine;
using UnityEngine.UI;

public class BlockedPlayerPanel : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Button unBlockButton;

    public void SetUpPanel(User blockedUser)
    {
        nameText.text = blockedUser.userName;
        unBlockButton.onClick.AddListener(() => {
            Friends.UnblockPlayer(blockedUser.userID);
            BlockedPanel.blockedPlayers.Remove(gameObject);
            Destroy(gameObject);
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Victory : MonoBehaviour
{
    GameControl gameControl;
    public int[] playerStandings = new int[0];

    [SerializeField]
    GameObject playAgainButton;

    public GameObject panelsParent;
    public GameObject[] children;

    private void Start() {
        gameControl = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>();
    }

    public int[] GetPlayerStandings() {
        if (!PhotonNetwork.isMasterClient) {
            return null;
        }

        int[] playerStandingsToReturn = new int[PhotonNetwork.room.MaxPlayers];//this array will hold return the player order
        List<int> cardsLeft = new List<int>();
        for (int i = 0; i < gameControl.playerPanels.Length; i++) {
            cardsLeft.Add(gameControl.playerPanels[i].GetComponent<PanelControl>().deck.transform.childCount);
        }
        cardsLeft.Sort();

        //we will create a new array to hold the player panels so we are able to remove them
        GameObject[] panels = new GameObject[gameControl.playerPanels.Length];
        for (int x = 0; x <  gameControl.playerPanels.Length; x++) {
            panels[x] = gameControl.playerPanels[x];
        }

        for (int x = 0; x < cardsLeft.Count; x++) {
            for (int t = 0; t < panels.Length; t++) {
                if (panels[t] != null) {
                    if (cardsLeft[x] == panels[t].GetComponent<PanelControl>().deck.transform.childCount) {
                        playerStandingsToReturn[x] = t;
                        panels[t] = null;
                        break;
                    }
                }
            }
        }

        return playerStandingsToReturn;
    }

    public IEnumerator ShowStandings() {

        GetComponent<Image>().color = Color.white;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        GameObject.FindGameObjectWithTag("GameManager").GetComponent<InGameMenu>().activePanel = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InGameMenu>().victoryPanel;

        foreach (GameObject child in children) {
            child.SetActive(true);
        }

        //if (PhotonNetwork.offlineMode) {
        //    playAgainButton.SetActive(true);
        //}

        while (playerStandings.Length == 0) {
            yield return new WaitForSeconds(0.5f);
        }
        if (gameControl.localPlayerPanel.GetComponent<PanelControl>().deck.transform.childCount == 0) {
            StartCoroutine(GameObject.FindGameObjectWithTag("StatsController").GetComponent<PlayerStatsController>().AddGameWon());
        }
        for (int i = 0; i < playerStandings.Length; i++) {
            GameObject standingPanel = Instantiate(Resources.Load<GameObject>("PlayerStandingPanel") as GameObject);
            standingPanel.transform.SetParent(panelsParent.transform);
            standingPanel.transform.localScale = new Vector3(1,1,1);
            standingPanel.GetComponent<PlayerStandingPanel>().SetUpPanel(gameControl.playerPanels[playerStandings[i]].GetComponent<PanelControl>().photonPlayer, i);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(playerStandings);
        } else {
            playerStandings = (int[])stream.ReceiveNext();
        }
    }
}

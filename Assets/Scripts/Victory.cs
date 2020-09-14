using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Victory : MonoBehaviour
{
    GameControl gameControl;
    public int[] playerStandings = new int[0];

    public GameObject panelsParent;
    public GameObject[] children;

    [SerializeField] GameObject victoryPanel;
    [SerializeField] GameObject quitGamePanel;

    LevelSystem levelSystem;
    Achievments achievments;

    private void Start() {
        gameControl = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>();
        levelSystem = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelSystem>();
        achievments = GameObject.FindGameObjectWithTag("AchievementManager").GetComponent<Achievments>();
    }

    public void CloseQuitPanel()
    {
        quitGamePanel.SetActive(false);
        victoryPanel.SetActive(true);
    }

    public void QuitGamePanel()
    {
        victoryPanel.SetActive(false);
        quitGamePanel.SetActive(true);
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

        yield return new WaitUntil(() => playerStandings.Length == gameControl.playerPanels.Length);

        //while (playerStandings.Length == 0) {
        //    yield return new WaitForSeconds(0.5f);
        //}

        Announcer announcer = GameObject.FindGameObjectWithTag("Announcer").GetComponent<Announcer>();
        if (gameControl.localPlayerPanel.GetComponent<PanelControl>().deck.transform.childCount == 0) {
            GameObject.FindGameObjectWithTag("StatsController").GetComponent<PlayerStatsController>().AddGameWon();
            announcer.YouWon();
            if (PhotonNetwork.offlineMode) {
                StartCoroutine(levelSystem.AddExperience(25));
                StartCoroutine(achievments.UnlockAchievement("Champion"));
            } else {
                StartCoroutine(levelSystem.AddExperience(50));
                StartCoroutine(achievments.UnlockAchievement("Champion"));
                StartCoroutine(achievments.UnlockAchievement("Online champion"));
            }
            LocalUser.locUser.gamesWonInARow++;
        } else {
            announcer.YouLost();
            if (PhotonNetwork.offlineMode) {
                StartCoroutine(levelSystem.AddExperience(10));
            } else {
                StartCoroutine(levelSystem.AddExperience(15));
            }
            LocalUser.locUser.gamesWonInARow = 0;
        }
        StartCoroutine(achievments.GamesWon());
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

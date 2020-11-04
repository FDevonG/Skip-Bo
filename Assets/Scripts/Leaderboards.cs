using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using System.Linq;
using System;

public class Leaderboards : MonoBehaviour
{
    [SerializeField] Transform leadeboardInfoParent;
    List<GameObject> leaderboardPanels = new List<GameObject>();
    [SerializeField] Dropdown selection;
    [SerializeField] GameObject scrollHolder;
    Transform playersPanel;//this is used to store the users panel

    [SerializeField] GameObject loadingPanel;

    List<List<Leaderboard>> leaderboards = new List<List<Leaderboard>>();

    [SerializeField] Button[] pageButtons;
    [SerializeField] Button firstPageButton;
    [SerializeField] Button lastPageButton;

    int pageNumber = 0;

    List<int> buttonNumbers = new List<int>();

    private void OnEnable() {
        BuildLeaderboards();
    }

    private void OnDisable() {
        leaderboards.Clear();
        DestroyPanels();
        loadingPanel.SetActive(false);
        firstPageButton.interactable = false;
        lastPageButton.interactable = false;
        TurnOffPageButtons();
    }

    void TurnOffPageButtons()
    {
        foreach (Button button in pageButtons)
        {
            button.GetComponentInChildren<Text>().text = "";
            button.gameObject.SetActive(false);
        }
    }

    private void DestroyPanels() {
        foreach (GameObject child in leaderboardPanels) {
            Destroy(child);
        }
        TurnOffPageButtons();
        leaderboardPanels.Clear();
        loadingPanel.SetActive(true);
    }

    public void BuildLeaderboards() {

        //var pageNumberTask = BackendFunctions.LeaderBoardPageNumber();
        //yield return new WaitUntil(() => pageNumberTask.IsCompleted);
        //if (pageNumberTask.IsFaulted)
        //{
        //    Debug.Log("Shit Fucked Up");
        //}
        //else
        //{
        //    Debug.Log(pageNumberTask.Result);
        //}

        leaderboards.Clear();
        DestroyPanels();
        GetComponent<ErrorText>().ClearError();
        StartCoroutine(BuildingLeaderboards());
    }

    private IEnumerator BuildingLeaderboards() {
        var task = Database.GetUsers();
        yield return new WaitUntil(() => task.IsCompleted);
        loadingPanel.SetActive(false);
        if (task.IsFaulted) {
            GetComponent<ErrorText>().SetError("Failed to load leaderboards");
        } else {
            DivideArray(GetArray(task.Result));
        }
    }

    void DivideArray(List<Leaderboard> leaders)
    {
        int arrayCount = (leaders.Count / 100) + 1;
        int firstPageNumberToGoTo = 0;
        for (int i = 0; i < arrayCount; i++)
        {
            List<Leaderboard> newLeaderBoards = new List<Leaderboard>();
            for (int x = 0; x < 100; x++)
            {
                if (leaders.Count > 0)
                {
                    newLeaderBoards.Add(leaders[0]);
                    if (leaders[0].id == LocalUser.locUser.userID)
                        firstPageNumberToGoTo = i;

                    leaders.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }
            if(newLeaderBoards.Count > 0)
                leaderboards.Add(newLeaderBoards);
        }
            pageNumber = firstPageNumberToGoTo;
            SpawnLeaderPanels(leaderboards[firstPageNumberToGoTo]);
            SetUpPageButtons();
    }

    public void FirstPage()
    {
        DestroyPanels();
        pageNumber = 0;
        SpawnLeaderPanels(leaderboards[pageNumber]);
        firstPageButton.interactable = false;
        SetUpPageButtons();
    }

    public void LastPage()
    {
        DestroyPanels();
        pageNumber = leaderboards.Count - 1;
        SpawnLeaderPanels(leaderboards[pageNumber]);
        lastPageButton.interactable = false;
        SetUpPageButtons();
    }

    public void ChangePage(int buttonPressed)
    {
        DestroyPanels();
        pageNumber = buttonNumbers[buttonPressed];
        SpawnLeaderPanels(leaderboards[pageNumber]);
        SetUpPageButtons();
    }

    void SetUpPageButtons()
    {
        buttonNumbers.Clear();
        
        if (pageNumber <= 2)
        {
            buttonNumbers.Add(0);
            buttonNumbers.Add(1);
            buttonNumbers.Add(2);
            buttonNumbers.Add(3);
            buttonNumbers.Add(4);
        }
        else if (pageNumber >= leaderboards.Count - 2)
        {
            buttonNumbers.Add(leaderboards.Count - 4);
            buttonNumbers.Add(leaderboards.Count - 3);
            buttonNumbers.Add(leaderboards.Count - 2);
            buttonNumbers.Add(leaderboards.Count - 1);
            buttonNumbers.Add(leaderboards.Count);
        }
        else
        {
            buttonNumbers.Add(pageNumber - 2);
            buttonNumbers.Add(pageNumber - 1);
            buttonNumbers.Add(pageNumber);
            buttonNumbers.Add(pageNumber + 1);
            buttonNumbers.Add(pageNumber + 2);
        }
        
        for (int i = 0; i < pageButtons.Length; i++)
        {
            if (leaderboards.Count > buttonNumbers[i])
                pageButtons[i].gameObject.SetActive(true);
            pageButtons[i].GetComponentInChildren<Text>().text = (buttonNumbers[i] + 1).ToString();
            if(buttonNumbers[i] == pageNumber)
            {
                pageButtons[i].interactable = false;
            }
            else
            {
                pageButtons[i].interactable = true;
            }
        }

        if (pageNumber == 0)
            firstPageButton.interactable = false;
        else
            firstPageButton.interactable = true;

        if (pageNumber == leaderboards.Count)
            lastPageButton.interactable = false;
        else
            lastPageButton.interactable = true;

    }

    private void SpawnLeaderPanels(List<Leaderboard> leaders) {
        loadingPanel.SetActive(false);
        bool userInList = false;
        for (int i = 0; i < leaders.Count; i++) {
            GameObject leaderboardPanel = Instantiate(Resources.Load<GameObject>("LeaderboardInfoPanel"), leadeboardInfoParent);
            leaderboardPanel.transform.localScale = new Vector3(1,1,1);
            leaderboardPanels.Add(leaderboardPanel);
            LeaderboardInfoPanel leaderboardInfoPanel = leaderboardPanel.GetComponent<LeaderboardInfoPanel>();
            leaderboardInfoPanel.standingText.text = leaders[i].standing.ToString();

            leaderboardInfoPanel.nameText.text = leaders[i].name;
            if (selection.value == 0) {
                leaderboardInfoPanel.infoText.text = leaders[i].onlineWins.ToString();
            }
            if (selection.value == 1) {
                leaderboardInfoPanel.infoText.text = leaders[i].onlineWinPercentage.ToString() + "%";
            }
            if (selection.value == 2) {
                leaderboardInfoPanel.infoText.text = leaders[i].offlineWins.ToString();
            }
            if (selection.value == 3) {
                leaderboardInfoPanel.infoText.text = leaders[i].offlineWinPercentage.ToString() + "%";
            }
            if (leaders[i].id == LocalUser.locUser.userID) {
                userInList = true;
                playersPanel = leaderboardPanel.transform;
                leaderboardInfoPanel.standingText.color = new Color(0.3179769f, 0.008944447f, 0.6320754f, 1);
                leaderboardInfoPanel.nameText.color = new Color(0.3179769f, 0.008944447f, 0.6320754f, 1);
                leaderboardInfoPanel.infoText.color = new Color(0.3179769f, 0.008944447f, 0.6320754f, 1);
            }
        }
        if (userInList)
            ScrollToPlayerPanel();
        else
            ScrollToTop();
    }

    private void ScrollToTop()
    {
        Canvas.ForceUpdateCanvases();
        leadeboardInfoParent.GetComponentInParent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
    }

    private void ScrollToPlayerPanel() {
        Canvas.ForceUpdateCanvases();
        leadeboardInfoParent.GetComponent<RectTransform>().anchoredPosition = (Vector2)scrollHolder.GetComponent<ScrollRect>().transform.InverseTransformPoint(leadeboardInfoParent.GetComponent<RectTransform>().position) - (Vector2)scrollHolder.GetComponent<ScrollRect>().transform.InverseTransformPoint(playersPanel.position);
        leadeboardInfoParent.GetComponent<RectTransform>().localPosition = new Vector3(-25, leadeboardInfoParent.GetComponent<RectTransform>().localPosition.y, leadeboardInfoParent.GetComponent<RectTransform>().localPosition.z);
    }

    private List<Leaderboard> GetArray(DataSnapshot snap) {
        List<Leaderboard> arrayOfStats = new List<Leaderboard>();
        foreach (DataSnapshot snapShot in snap.Children) {
            User user = JsonUtility.FromJson<User>(snapShot.GetRawJsonValue());
            if (!string.IsNullOrEmpty(user.userName) && !string.IsNullOrWhiteSpace(user.userName)) {
                arrayOfStats.Add(new Leaderboard(user.userName, user.onlineGamesWon, GetPercentage(user.onlineGamesWon, user.onlineGamesPlayed), user.offlineGamesWon, GetPercentage(user.offlineGamesWon, user.offlineGamesPlayed), user.userID));
            }
        }
        return SortStandings(arrayOfStats);
    }

    private float GetPercentage(float numerator, float denominator) {
        float percentage = 0;
        if (numerator > 0) {
            percentage = (numerator / denominator) * 100;
            percentage = (float)Math.Round((decimal)percentage, 2);
        }
        return percentage;
    }

    private List<Leaderboard> SortStandings(List<Leaderboard> arrayOfStats) {
        List<Leaderboard> listToReturn = new List<Leaderboard>();
        if (selection.value == 0) {
            listToReturn = arrayOfStats.OrderBy(leaderboard => leaderboard.onlineWins).ToList();
        }
        if (selection.value == 1) {
            listToReturn = arrayOfStats.OrderBy(leaderboard => leaderboard.onlineWinPercentage).ToList();
        }
        if (selection.value == 2) {
            listToReturn = arrayOfStats.OrderBy(leaderboard => leaderboard.offlineWins).ToList();
        }
        if (selection.value == 3) {
            listToReturn = arrayOfStats.OrderBy(leaderboard => leaderboard.offlineWinPercentage).ToList();
        }
        listToReturn.Reverse();
        for (int i = 0; i < listToReturn.Count; i++)
        {
            listToReturn[i].standing = i + 1;
        }
        return listToReturn;
    }

    public class Leaderboard {
        public string name;
        public int onlineWins;
        public float onlineWinPercentage;
        public int offlineWins;
        public float offlineWinPercentage;
        public string id;
        public int standing;

        public Leaderboard(string n, int onw, float onwp, int offw, float offwp, string ID) {
            name = n;
            onlineWins = onw;
            onlineWinPercentage = onwp;
            offlineWins = offw;
            offlineWinPercentage = offwp;
            id = ID;
        }

    }

}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Leaderboards : MonoBehaviour
{
    [SerializeField] Transform leadeboardInfoParent;
    [SerializeField] Dropdown selection;
    [SerializeField] GameObject scrollHolder;
    Transform playersPanel;//this is used to store the users panel

    [SerializeField] Button[] pageButtons;
    [SerializeField] Button firstPageButton;
    [SerializeField] Button lastPageButton;

    int pageNumber;

    [Serializable]
    public class LoadedLeaderboards
    {
        public Leaderboard[] loadBoards = new Leaderboard[0];
    }

    [Serializable]
    public class Buttons
    {
        public int[] buttonNumbers = new int[0];
    }

    Buttons buttons = new Buttons();

    private void OnEnable() {
        StartCoroutine(LoadNewLeaderboard());
    }

    private void OnDisable() {
        DestroyPanels();
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
        foreach (Transform child in leadeboardInfoParent)
        {
            Destroy(child.gameObject);
        }
        GetComponent<ErrorText>().ClearError();
        TurnOffPageButtons();
    }

    public void ChangeLeaderboard()
    {
        StartCoroutine(LoadNewLeaderboard());
    }

    public IEnumerator LoadNewLeaderboard()
    {
        LoadingScreen.Instance.TurnOnLoadingScreen("Loading");
        DestroyPanels();
        var getLeaderboardsTask = BackendFunctions.LoadLeaderBoards(selection.value);
        yield return new WaitUntil(() => getLeaderboardsTask.IsCompleted);
        if (getLeaderboardsTask.IsFaulted)
        {
            GetComponent<ErrorText>().SetError("Failed to load");
            LoadingScreen.Instance.TurnOffLoadingScreen();
        }
        else
        {
            string[] strArr;
            strArr = getLeaderboardsTask.Result.Split('®');
            LoadedLeaderboards lb = JsonUtility.FromJson<LoadedLeaderboards>(strArr[0]);
            SpawnLeaderPanels(lb);
            buttons = JsonUtility.FromJson<Buttons>(strArr[1]);
            pageNumber = int.Parse(strArr[2]);
            SetUpPageButtons();
        }
    }

    private void SpawnLeaderPanels(LoadedLeaderboards leaders)
    {
        bool userInList = false;
        for (int i = 0; i < leaders.loadBoards.Length; i++)
        {
            GameObject leaderboardPanel = Instantiate(Resources.Load<GameObject>("LeaderboardInfoPanel"), leadeboardInfoParent);
            leaderboardPanel.transform.localScale = new Vector3(1, 1, 1);
            LeaderboardInfoPanel leaderboardInfoPanel = leaderboardPanel.GetComponent<LeaderboardInfoPanel>();
            leaderboardInfoPanel.standingText.text = leaders.loadBoards[i].standing.ToString();

            leaderboardInfoPanel.nameText.text = leaders.loadBoards[i].playerName;
            if (selection.value == 0 || selection.value == 2)
            {
                leaderboardInfoPanel.infoText.text = leaders.loadBoards[i].stat.ToString();
            }
            if (selection.value == 1 || selection.value == 3)
            {
                leaderboardInfoPanel.infoText.text = leaders.loadBoards[i].stat.ToString() + "%";
            }
            if (leaders.loadBoards[i].id == LocalUser.locUser.userID)
            {
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
        LoadingScreen.Instance.TurnOffLoadingScreen();
    }

    public IEnumerator ChangeLeaderboardPage()
    {
        LoadingScreen.Instance.TurnOnLoadingScreen("Loading");
        DestroyPanels();
        int[] intsToSend = new int[2];
        intsToSend[0] = selection.value;
        intsToSend[1] = pageNumber;
        var getLeaderboardsTask = BackendFunctions.LoadLeaderBoardPage(intsToSend);
        yield return new WaitUntil(() => getLeaderboardsTask.IsCompleted);
        if (getLeaderboardsTask.IsFaulted)
        {
            GetComponent<ErrorText>().SetError("Failed to load");
            LoadingScreen.Instance.TurnOffLoadingScreen();
        }
        else
        {
            string[] strArr;
            strArr = getLeaderboardsTask.Result.Split('®');
            LoadedLeaderboards lb = JsonUtility.FromJson<LoadedLeaderboards>(strArr[0]);
            SpawnLeaderPanels(lb);
            buttons = JsonUtility.FromJson<Buttons>(strArr[1]);
            SetUpPageButtons();
        }
    }

    public IEnumerator GetLastPage()
    {
        LoadingScreen.Instance.TurnOnLoadingScreen("Loading");
        DestroyPanels();
        var getLeaderboardsTask = BackendFunctions.GetLastPage(selection.value);
        yield return new WaitUntil(() => getLeaderboardsTask.IsCompleted);
        if (getLeaderboardsTask.IsFaulted)
        {
            GetComponent<ErrorText>().SetError("Failed to load");
            LoadingScreen.Instance.TurnOffLoadingScreen();
        }
        else
        {
            string[] strArr;
            strArr = getLeaderboardsTask.Result.Split('®');
            LoadedLeaderboards lb = JsonUtility.FromJson<LoadedLeaderboards>(strArr[0]);
            SpawnLeaderPanels(lb);
            buttons = JsonUtility.FromJson<Buttons>(strArr[1]);
            pageNumber = int.Parse(strArr[2]);
            SetUpPageButtons();
        }
    }

    void SetUpPageButtons()
    {
        for (int i = 0; i < pageButtons.Length; i++)
        {
            if (buttons.buttonNumbers.Length > i)
            {
                pageButtons[i].gameObject.SetActive(true);
                pageButtons[i].GetComponentInChildren<Text>().text = (buttons.buttonNumbers[i] + 1).ToString();
            }
            if (buttons.buttonNumbers[i] == pageNumber)
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

        if (pageNumber == buttons.buttonNumbers[4])
            lastPageButton.interactable = false;
        else
            lastPageButton.interactable = true;

    }

    public void FirstPage()
    {
        pageNumber = 0;
        StartCoroutine(ChangeLeaderboardPage());
    }

    public void LastPage()
    {
        StartCoroutine(GetLastPage());
    }

    public void ChangePage(int buttonPressed)
    {
        pageNumber = buttons.buttonNumbers[buttonPressed];
        StartCoroutine(ChangeLeaderboardPage());
    }

    private void ScrollToTop()
    {
        Canvas.ForceUpdateCanvases();
        leadeboardInfoParent.GetComponentInParent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
    }

    private void ScrollToPlayerPanel() {
        Canvas.ForceUpdateCanvases();
        RectTransform rt = leadeboardInfoParent.GetComponent<RectTransform>();
        rt.anchoredPosition = (Vector2)scrollHolder.GetComponent<ScrollRect>().transform.InverseTransformPoint(rt.position) - (Vector2)scrollHolder.GetComponent<ScrollRect>().transform.InverseTransformPoint(playersPanel.position);
        rt.anchoredPosition = new Vector3(rt.localPosition.x, rt.localPosition.y - 800, rt.localPosition.z);
        rt.localPosition = new Vector3(-25, rt.localPosition.y, rt.localPosition.z);
    }
}

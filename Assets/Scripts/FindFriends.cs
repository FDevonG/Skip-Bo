using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FindFriends : MonoBehaviour
{
    [SerializeField] GameObject nameInput;
    [SerializeField] Button searchButton;
    [SerializeField] Button addFriendButton;
    [SerializeField] GameObject addedText;

    [SerializeField] Text loadText;
 
    [SerializeField] GameObject freindPanel;
    [SerializeField] Text nameText;
    [SerializeField] Image hair;
    [SerializeField] Image face;
    [SerializeField] Image kit;
    [SerializeField] Image body;

    [SerializeField] GameObject searchingPanel;

    string id;//thjis is used to store the id of the found user

    void OnDisable()
    {
        nameInput.GetComponent<InputField>().text = "";
        GetComponent<ErrorText>().ClearError();
        freindPanel.SetActive(false);
        id = "";
        searchingPanel.SetActive(false);
        addedText.SetActive(false);
    }

    public void StartUserSearch()
    {
        freindPanel.SetActive(false);
        loadText.text = "Searching...";
        searchingPanel.SetActive(true);
        StartCoroutine(FindUser(nameInput.GetComponent<InputField>().text));
    }

    private IEnumerator FindUser(string playerName)
    {

        var task = BackendFunctions.FindUser(playerName);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted)
        {
            GetComponent<ErrorText>().SetError("Search failed");
        }
        else
        {
            if (task.Result != null)
            {
                User friend = JsonUtility.FromJson<User>(task.Result);
                id = friend.userID;
                FoundFriend(friend);
            }
            else
                GetComponent<ErrorText>().SetError("No users found");
        }
        searchingPanel.SetActive(false);
    }

    private void FoundFriend(User user)
    {
        freindPanel.SetActive(true);
        nameText.text = user.userName;
        body.sprite = Resources.Load<Sprite>("Faces/Bodies/" + user.body) as Sprite;
        face.sprite = Resources.Load<Sprite>("Faces/Faces/" + user.face) as Sprite;
        hair.sprite = Resources.Load<Sprite>("Faces/Hairs/" + user.hair) as Sprite;
        kit.sprite = Resources.Load<Sprite>("Faces/Kits/" + user.kit) as Sprite;
        if (Friends.FriendAlreadyAdded(user.userID))
        {
            addFriendButton.interactable = false;
        }
        else
        {
            addFriendButton.interactable = true;
        }
    }

    public void AddFriend()
    {
        if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id))
        {
            StartCoroutine(SaveFriend());
        }
    }

    private IEnumerator SaveFriend()
    {
        searchButton.interactable = false;
        nameInput.GetComponent<InputField>().interactable = false;
        freindPanel.SetActive(false);
        loadText.text = "Adding...";
        searchingPanel.SetActive(true);
        CoroutineWithData cd = new CoroutineWithData(this, Friends.AddFriend(id));
        yield return cd.coroutine;
        searchingPanel.SetActive(false);
        addedText.SetActive(true);
        nameInput.GetComponent<InputField>().interactable = true;
        searchButton.interactable = true;
    }

    public void CheckUserSearch()
    {
        GetComponent<ErrorText>().ClearError();
        freindPanel.SetActive(false);
        addedText.SetActive(false);
    }

}

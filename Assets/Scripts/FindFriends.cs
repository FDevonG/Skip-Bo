using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class FindFriends : MonoBehaviour
{
    [SerializeField] GameObject emailInput;
    [SerializeField] Button searchButton;

    [SerializeField] GameObject freindPanel;
    [SerializeField] Text nameText;
    [SerializeField] Image hair;
    [SerializeField] Image face;
    [SerializeField] Image kit;
    [SerializeField] Image body;

    [SerializeField] GameObject searchingPanel;

    string id;//thjis is used to store the id of the found user

    void OnDisable() {
        searchButton.interactable = false;
        emailInput.GetComponent<InputField>().text = "";
        GetComponent<ErrorText>().ClearError();
        freindPanel.SetActive(false);
        id = "";
        searchingPanel.SetActive(false);
    }

    public void StartUserSearch() {
        searchingPanel.SetActive(true);
        StartCoroutine(FindUser());
    }

    private IEnumerator FindUser() {
        var task = Database.GetUsers();
        yield return new WaitUntil(() => task.IsCompleted);
        searchingPanel.SetActive(false);
        if (task.IsFaulted) {
            GetComponent<ErrorText>().SetError("Search failed");
        } else {
            bool friendFound = false;
            foreach (DataSnapshot s in task.Result.Children) {
                User tempUser = JsonUtility.FromJson<User>(s.GetRawJsonValue());
                if (tempUser.email != null) {
                    if (tempUser.email.ToUpper() == emailInput.GetComponent<InputField>().text.ToUpper()) {
                        bool blocked = false;
                        foreach (string userID in tempUser.blocked) {
                            if (userID == FirebaseAuthentication.AuthenitcationKey()) {
                                blocked = true;
                                break;
                            }
                        }
                        if (!blocked) {
                            FoundFriend(tempUser);
                            id = tempUser.userID;
                            friendFound = true;
                            break;
                        }
                    }
                }
            }
            if (!friendFound) {
                GetComponent<ErrorText>().SetError("No users found");
            }
        }
    }

    private void FoundFriend(User user) {
        freindPanel.SetActive(true);
        nameText.text = user.userName;
        body.sprite = Resources.Load<Sprite>("Faces/Bodies/" + user.body) as Sprite;
        face.sprite = Resources.Load<Sprite>("Faces/Faces/" + user.face) as Sprite;
        hair.sprite = Resources.Load<Sprite>("Faces/Hairs/" + user.hair) as Sprite;
        kit.sprite = Resources.Load<Sprite>("Faces/Kits/" + user.kit) as Sprite;
    }

    public void AddFriend() {
        if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id)) {
            StartCoroutine(SaveFriend());
        }
    }

    private IEnumerator SaveFriend() {
        if (!Friends.FriendAlreadyAdded(id)) {
            CoroutineWithData cd = new CoroutineWithData(this, Friends.AddFriend(id));
            yield return cd.coroutine;
            GetComponent<ErrorText>().SetError((string)cd.result);
        } else {
            GetComponent<ErrorText>().SetError("Friend already added");
        }
    }

    public void CheckUserSearch() {
        GetComponent<ErrorText>().ClearError();
        freindPanel.SetActive(false);
        if (VerifyEmail.ValidateEmail(emailInput.GetComponent<InputField>().text)) {
            searchButton.interactable = true;
        } else {
            searchButton.interactable = false;
            freindPanel.SetActive(false);
        }
    }

}

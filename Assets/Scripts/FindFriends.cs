using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class FindFriends : MonoBehaviour
{
    [SerializeField] GameObject emailInput;
    [SerializeField] Button searchButton;
    [SerializeField] Text infoText;

    [SerializeField] GameObject freindPanel;
    [SerializeField] Text nameText;
    [SerializeField] Image hair;
    [SerializeField] Image face;
    [SerializeField] Image kit;
    [SerializeField] Image body;

    string id;//thjis is used to store the id of the found user

    void OnDisable() {
        searchButton.interactable = false;
        emailInput.GetComponent<InputField>().text = "";
        infoText.gameObject.SetActive(false);
        freindPanel.SetActive(false);
        id = "";
    }

    public void StartUserSearch() {
        StartCoroutine(FindUser());
    }

    private IEnumerator FindUser() {
        var task = FireBaseScript.GetUsers();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) {
            SetErrorMessage("Search failed");
        } else {
            bool friendFound = false;
            foreach (DataSnapshot s in task.Result.Children) {
                User tempUser = JsonUtility.FromJson<User>(s.GetRawJsonValue());
                if (tempUser.email.ToUpper() == emailInput.GetComponent<InputField>().text.ToUpper()) {
                    FoundFriend(tempUser);
                    id = tempUser.userID;
                    friendFound = true;
                    break;
                }
            }
            if (!friendFound) {
                SetErrorMessage("No users found");
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
        bool friendAlreadyAdded = false;
        for (int i = 0; i < LocalUser.locUser.friends.Count; i++) {
            if (LocalUser.locUser.friends[i].ToUpper() == id.ToUpper()) {
                friendAlreadyAdded = true;
                SetErrorMessage("Friend already added");
            }
        }
        if (!friendAlreadyAdded) {
            LocalUser.locUser.friends.Add(id);
            var usersTask = FireBaseScript.GetUsers();
            var addFriendTask = FireBaseScript.UpdateUserFriends(LocalUser.locUser.friends);
            yield return new WaitUntil(() => addFriendTask.IsCompleted && usersTask.IsCompleted);
            if (addFriendTask.IsFaulted || usersTask.IsFaulted) {
                SetErrorMessage("Failed to add " + nameText.text);
            } else {
                SetErrorMessage(nameText.text + " added");
                foreach (DataSnapshot snap in usersTask.Result.Children) {
                    User tempUser = JsonUtility.FromJson<User>(snap.GetRawJsonValue());
                    if (tempUser.userID == id) {
                        Friends.friends.Add(tempUser);
                    }
                }
                GameObject.FindGameObjectWithTag("Chat").GetComponent<Chat>().UpdateFriends();
            }
        }
    }

    public void CheckUserSearch() {
        infoText.gameObject.SetActive(false);
        freindPanel.SetActive(false);
        if (VerifyEmail.ValidateEmail(emailInput.GetComponent<InputField>().text)) {
            searchButton.interactable = true;
        } else {
            searchButton.interactable = false;
            freindPanel.SetActive(false);
        }
    }

    private void SetErrorMessage(string message) {
        infoText.gameObject.SetActive(true);
        infoText.text = message;
    }

}

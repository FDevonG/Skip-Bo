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

    string email;

    void OnDisable() {
        searchButton.interactable = false;
        emailInput.GetComponent<InputField>().text = "";
        infoText.gameObject.SetActive(false);
        freindPanel.SetActive(false);
        email = "";
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
                    email = tempUser.email;
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
        if (!string.IsNullOrEmpty(email) && !string.IsNullOrWhiteSpace(email)) {
            StartCoroutine(SaveFriend());
        }
    }

    private IEnumerator SaveFriend() {
        var task = FireBaseScript.GetCurrentUser();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) {
            SetErrorMessage("Failed to add friend");
        } else {
            User user = JsonUtility.FromJson<User>(task.Result);
            bool friendAlreadyAdded = false;
            for (int i = 0; i < user.friends.Count; i++) {
                if (user.friends[i].ToUpper() == email.ToUpper()) {
                    friendAlreadyAdded = true;
                    SetErrorMessage("Friend already added");
                }
            }
            if (!friendAlreadyAdded) {
                user.friends.Add(email);
                FireBaseScript.UpdateUser(user);
                SetErrorMessage(nameText.text + " added");
            }
        }
    }

    public void CheckUserSearch() {
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

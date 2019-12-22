using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class CharacterCreation : MonoBehaviour
{
    public Image cbody;//used to display the body
    public Image cface;//used to display the face
    public Image chair;//used to display the hair
    public Image ckit;//used to display the clothes
    public Sprite[] body;//stores the body types
    public Sprite[] face;//used to display the face types
    public Sprite[] hair;//used to display the hair types
    public Sprite[] kit;//used to display the clothes types

    public GameObject nameInput;

    [SerializeField] Text infoText;

    public int cbodyIndex = 0;
    public int cfaceIndex = 0;
    public int chairIndex = 0;
    public int ckitIndex = 0;

    public Button startMenuCancelButton;

    private float timeTakenDuringLerp = 1.0f;
    private float timeStartedLerping;
    private bool imagesLerping = false;
    private Color visibleColor = new Color(1,1,1,1);
    private Color invisibleColor = new Color(1,1,1,0);

    private void OnEnable() {
        StartCoroutine(BuildCharacter());
    }

    private void OnDisable() {
        nameInput.GetComponent<InputField>().text = "";
        nameInput.GetComponent<Outline>().enabled = false;
        infoText.gameObject.SetActive(false);
        chair.color = invisibleColor;
        cface.color = invisibleColor;
        ckit.color = invisibleColor;
        cbody.color = invisibleColor;
    }

    private IEnumerator BuildCharacter() {
        var task = FireBaseScript.GetCurrentUser();
        yield return new WaitUntil(() => task.IsCompleted);
        User user = new User();
        if (task.IsFaulted) {
            ErrorWithCharacterEdit("Failed to load profile");
        } else {
            user = JsonUtility.FromJson<User>(task.Result);
        }
        if (string.IsNullOrEmpty(user.userName)) {
            RandomizeCharacter();
            startMenuCancelButton.gameObject.SetActive(false);
        } else {
            BuildSavedCharacter(user);
            startMenuCancelButton.gameObject.SetActive(true);
        }
    }

    public void RandomizeCharacter() {
        //get random index of all the body parts and store them
        cbodyIndex = Random.Range(0, body.Length);
        cfaceIndex = Random.Range(0, face.Length);
        chairIndex = Random.Range(0, hair.Length);
        ckitIndex = Random.Range(0, kit.Length);

        //set the body parts
        cbody.sprite = body[cbodyIndex];

        cface.sprite = face[cfaceIndex];

        chair.sprite = hair[chairIndex];

        ckit.sprite = kit[ckitIndex];

        StartImagesLerp();
    }

    private void BuildSavedCharacter(User user) {
        //User user = FireBaseScript.GetCurrentUser();
        nameInput.GetComponent<InputField>().text = user.userName;
        
        ////set the body parts
        cbody.sprite = Resources.Load<Sprite>("Faces/Bodies/" + user.body) as Sprite;
        cface.sprite = Resources.Load<Sprite>("Faces/Faces/" + user.face) as Sprite;
        chair.sprite = Resources.Load<Sprite>("Faces/Hairs/" + user.hair) as Sprite;
        ckit.sprite = Resources.Load<Sprite>("Faces/Kits/" + user.kit) as Sprite;

        StartImagesLerp();
    }

    public void StartImagesLerp() {
        imagesLerping = true;
        timeStartedLerping = Time.time;//get the time this all started
    }

    //changes the hair to the next one
    public void NextHair() {
        if (chairIndex == hair.Length - 1) {
            chairIndex = 0;
        } else {
            chairIndex++;
        }
        chair.sprite = hair[chairIndex];
    }

    //changes the hair to the previous one
    public void PreviousHair() {
        if (chairIndex == 0) {
            chairIndex = hair.Length - 1;
        } else {
            chairIndex--;
        }
        chair.sprite = hair[chairIndex];
    }

    //changes the face to the next one
    public void NextFace() {
        if (cfaceIndex == face.Length - 1) {
            cfaceIndex = 0;
        } else {
            cfaceIndex++;
        }
        cface.sprite = face[cfaceIndex];
    }

    //changes the face to the previous face
    public void PreviousFace() {
        if (cfaceIndex == 0) {
            cfaceIndex = face.Length - 1;
        } else {
            cfaceIndex--;
        }
        cface.sprite = face[cfaceIndex];
    }

    //change the clothes to the next clothes
    public void NextClothes() {
        if (ckitIndex == kit.Length - 1) {
            ckitIndex = 0;
        } else {
            ckitIndex++;
        }
        ckit.sprite = kit[ckitIndex];
    }

    //changes the clothes to the previous clothes
    public void PreviousClothes() {
        if (ckitIndex == 0) {
            ckitIndex = kit.Length - 1;
        } else {
            ckitIndex--;
        }
        ckit.sprite = kit[ckitIndex];
    }

    //changes the body type to the next body
    public void NextBody() {
        if (cbodyIndex == body.Length - 1) {
            cbodyIndex = 0;
        } else {
            cbodyIndex++;
        }
        cbody.sprite = body[cbodyIndex];
    }

    //changes the body type to previous sprite
    public void PreviousBody() {
        if (cbodyIndex == 0) {
            cbodyIndex = body.Length - 1;
        } else {
            cbodyIndex--;
        }
        cbody.sprite = body[cbodyIndex];
    }

    public void SavePlayer() {
        string playerName = nameInput.GetComponent<InputField>().text;
        if (!string.IsNullOrEmpty(playerName) && !string.IsNullOrWhiteSpace(playerName)) {
            StartCoroutine(NameCheck(playerName));
        } else {
            ErrorWithCharacterEdit("Please enter a username");
            NameError();
        }
    }

    private IEnumerator NameCheck(string userName) {
        var task = FireBaseScript.GetUsers();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) {
            ErrorWithCharacterEdit("Failed to save character");
        } else {
            bool nameBool = false;
            foreach (DataSnapshot snap in task.Result.Children) {
                User tempUser = JsonUtility.FromJson<User>(snap.GetRawJsonValue());
                if (tempUser.userName == userName) {
                    if (tempUser.userID != FireBaseScript.AuthenitcationKey()) {
                        nameBool = true;
                        break;
                    }
                }
            }
            if (!nameBool) {
                var userNameTask = FireBaseScript.UpdateUser("userName", userName);
                var hairTask = FireBaseScript.UpdateUser("hair", hair[chairIndex].name);
                var faceTask = FireBaseScript.UpdateUser("face", face[cfaceIndex].name);
                var kitTask = FireBaseScript.UpdateUser("kit", kit[ckitIndex].name);
                var bodyTask = FireBaseScript.UpdateUser("body", body[cbodyIndex].name);
                yield return new WaitUntil(() => userNameTask.IsCompleted && hairTask.IsCompleted && faceTask.IsCompleted && kitTask.IsCompleted && bodyTask.IsCompleted);

                if (userNameTask.IsFaulted || hairTask.IsFaulted || faceTask.IsFaulted || kitTask.IsFaulted || bodyTask.IsFaulted) {
                    ErrorWithCharacterEdit("Failed to save character");
                } else {
                    yield return StartCoroutine(LocalUser.LoadUser());
                    PhotonPlayerSetup.BuildPhotonPlayer(PhotonNetwork.player, LocalUser.locUser);
                    GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<PhotonNetworking>().ConnectToPhoton();
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
                }
            } else {
                ErrorWithCharacterEdit("Username is taken");
                NameError();
            }
        }
    }

    public void ErrorWithCharacterEdit(string message) {
        infoText.gameObject.SetActive(true);
        infoText.text = message;
        GameObject.FindGameObjectWithTag("Announcer").GetComponent<Announcer>().AnnouncerAnError();
    }

    private void NameError() {
        nameInput.GetComponent<Outline>().enabled = true;
    }

    private void Update() {
        if (imagesLerping) {
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
            chair.color = Color.Lerp(invisibleColor, visibleColor, percentageComplete * 20);
            cface.color = Color.Lerp(invisibleColor, visibleColor, percentageComplete * 20);
            ckit.color = Color.Lerp(invisibleColor, visibleColor, percentageComplete * 20);
            cbody.color = Color.Lerp(invisibleColor, visibleColor, percentageComplete * 20);
            if (percentageComplete >= timeTakenDuringLerp) {
                imagesLerping = false;
            }
        }
    }

}

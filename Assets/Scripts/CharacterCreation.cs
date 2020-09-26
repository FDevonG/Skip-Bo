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
    public List<Sprite> face = new List<Sprite>();//used to display the face types
    public List<Sprite> hair = new List<Sprite>();//used to display the hair types
    public List<Sprite> kit = new List<Sprite>();//used to display the clothes types

    public GameObject nameInput;

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
        BuildCharacter();
    }

    private void OnDisable() {
        nameInput.GetComponent<InputField>().text = "";
        chair.color = invisibleColor;
        cface.color = invisibleColor;
        ckit.color = invisibleColor;
        cbody.color = invisibleColor;
    }

    private void BuildCharacter() {
        AddUnlockedAssets();
        if (string.IsNullOrEmpty(LocalUser.locUser.userName)) {
            RandomizeCharacter();
            startMenuCancelButton.gameObject.SetActive(false);
        } else {
            BuildSavedCharacter(LocalUser.locUser);
            startMenuCancelButton.gameObject.SetActive(true);
        }
    }

    void AddUnlockedAssets()
    {
        //Add the hairs
        foreach (string itemName in LocalUser.locUser.unlockedHair)
        {
            bool itemAdded = false;
            foreach (Sprite sprite in hair)
            {
                if (string.Equals(itemName, sprite.name))
                {
                    itemAdded = true;
                    break;
                }
            }
            if (!itemAdded)
            {
                hair.Add(Resources.Load<Sprite>(("Faces/Hairs/" + itemName)) as Sprite);
            }
        }

        //Add the Faces
        foreach (string itemName in LocalUser.locUser.unlockedFace)
        {
            bool itemAdded = false;
            foreach (Sprite sprite in face)
            {
                if (string.Equals(itemName, sprite.name))
                {
                    itemAdded = true;
                    break;
                }
            }
            if (!itemAdded)
            {
                hair.Add(Resources.Load<Sprite>(("Faces/Faces/" + itemName)) as Sprite);
            }
        }

        //add the kits
        foreach (string itemName in LocalUser.locUser.unlockedClothes)
        {
            bool itemAdded = false;
            foreach (Sprite sprite in kit)
            {
                if (string.Equals(itemName, sprite.name))
                {
                    itemAdded = true;
                    break;
                }
            }
            if (!itemAdded)
            {
                hair.Add(Resources.Load<Sprite>(("Faces/Kits/" + itemName)) as Sprite);
            }
        }
    }

    public void RandomizeCharacter() {
        //get random index of all the body parts and store them
        cbodyIndex = Random.Range(0, body.Length);
        cfaceIndex = Random.Range(0, face.Count);
        chairIndex = Random.Range(0, hair.Count);
        ckitIndex = Random.Range(0, kit.Count);

        //set the body parts
        cbody.sprite = body[cbodyIndex];
        cface.sprite = face[cfaceIndex];
        chair.sprite = hair[chairIndex];
        ckit.sprite = kit[ckitIndex];
        StartImagesLerp();
    }

    private void BuildSavedCharacter(User user) {
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
        if (chairIndex == hair.Count - 1) {
            chairIndex = 0;
        } else {
            chairIndex++;
        }
        chair.sprite = hair[chairIndex];
    }

    //changes the hair to the previous one
    public void PreviousHair() {
        if (chairIndex == 0) {
            chairIndex = hair.Count - 1;
        } else {
            chairIndex--;
        }
        chair.sprite = hair[chairIndex];
    }

    //changes the face to the next one
    public void NextFace() {
        if (cfaceIndex == face.Count - 1) {
            cfaceIndex = 0;
        } else {
            cfaceIndex++;
        }
        cface.sprite = face[cfaceIndex];
    }

    //changes the face to the previous face
    public void PreviousFace() {
        if (cfaceIndex == 0) {
            cfaceIndex = face.Count - 1;
        } else {
            cfaceIndex--;
        }
        cface.sprite = face[cfaceIndex];
    }

    //change the clothes to the next clothes
    public void NextClothes() {
        if (ckitIndex == kit.Count - 1) {
            ckitIndex = 0;
        } else {
            ckitIndex++;
        }
        ckit.sprite = kit[ckitIndex];
    }

    //changes the clothes to the previous clothes
    public void PreviousClothes() {
        if (ckitIndex == 0) {
            ckitIndex = kit.Count - 1;
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
            //StartCoroutine(SaveCharacter(playerName));
            StartCoroutine(NameCheck(playerName));
        } else {
            GetComponent<ErrorText>().SetError("Please enter a username");
        }
    }

    private IEnumerator NameCheck(string userName) {
        GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOnLoadingScreen();
        var task = Database.GetUsers();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) {
            GetComponent<ErrorText>().SetError("Failed to save character");
            GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
        } else {
            bool nameBool = false;
            foreach (DataSnapshot snap in task.Result.Children) {
                User tempUser = JsonUtility.FromJson<User>(snap.GetRawJsonValue());
                if (tempUser.userName == userName) {
                    if (tempUser.userID != FirebaseAuthentication.AuthenitcationKey()) {
                        nameBool = true;
                        break;
                    }
                }
            }
            if (!nameBool) {
                StartCoroutine(SaveCharacter(userName));
            } else {
                GetComponent<ErrorText>().SetError("Username is taken");
                GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
            }
        }
    }



    private IEnumerator SaveCharacter(string userName) {
        var userNameTask = Database.UpdateUser("userName", userName);
        var hairTask = Database.UpdateUser("hair", hair[chairIndex].name);
        var faceTask = Database.UpdateUser("face", face[cfaceIndex].name);
        var kitTask = Database.UpdateUser("kit", kit[ckitIndex].name);
        var bodyTask = Database.UpdateUser("body", body[cbodyIndex].name);
        yield return new WaitUntil(() => userNameTask.IsCompleted && hairTask.IsCompleted && faceTask.IsCompleted && kitTask.IsCompleted && bodyTask.IsCompleted);

        if (userNameTask.IsFaulted || hairTask.IsFaulted || faceTask.IsFaulted || kitTask.IsFaulted || bodyTask.IsFaulted) {
            GetComponent<ErrorText>().SetError("Failed to save character");
        } else {
            LocalUser.locUser.userName = userName;
            LocalUser.locUser.hair = hair[chairIndex].name;
            LocalUser.locUser.face = face[cfaceIndex].name;
            LocalUser.locUser.kit = kit[ckitIndex].name;
            LocalUser.locUser.body = body[cbodyIndex].name;
            PhotonPlayerSetup.BuildPhotonPlayer(PhotonNetwork.player, LocalUser.locUser);
            GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<PhotonNetworking>().ConnectToPhoton();
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
        }
        GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
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

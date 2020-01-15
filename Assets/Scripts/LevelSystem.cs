using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem : MonoBehaviour{

    private int levelCap = 100;
    Achievments achievments;

    public static LevelSystem Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        achievments = GameObject.FindGameObjectWithTag("AchievementManager").GetComponent<Achievments>();
    }

    public IEnumerator AddExperience(int amount) {
        if (LocalUser.locUser.experience != GetExperienceToNextLevel() && LocalUser.locUser.level != levelCap) {
            LocalUser.locUser.experience += amount;
            if (LocalUser.locUser.level == levelCap) {
                if (LocalUser.locUser.experience > GetExperienceToNextLevel()) {
                    LocalUser.locUser.experience = GetExperienceToNextLevel();
                }
            }
            var task = Database.UpdateUser("experience", LocalUser.locUser.experience);
            yield return new WaitUntil(() => task.IsCompleted);
            while (task.IsFaulted) {
                task = Database.UpdateUser("experience", LocalUser.locUser.experience);
                yield return new WaitUntil(() => task.IsCompleted);
            }
            if (LocalUser.locUser.level < levelCap) {
                while (LocalUser.locUser.experience >= GetExperienceToNextLevel()) {
                    // Enough experience to level up
                    yield return StartCoroutine(AddLevel());
                    yield return StartCoroutine(ChangeExperienceToLevelNeeded());
                }
            }
        }
    }

    private IEnumerator AddLevel() {
        LocalUser.locUser.experience -= GetExperienceToNextLevel();
        LocalUser.locUser.level++;

        while (GameObject.FindGameObjectWithTag("NotificationPanel") != null) {
            yield return new WaitForSeconds(0.5f);
        }

        GameObject notificationPanel = Instantiate(Resources.Load<GameObject>("NotificationPanel"));
        notificationPanel.GetComponent<NotificationPanel>().SetText("You have reached level " + LocalUser.locUser.level.ToString());

        achievments.CheckLevelAchievments(LocalUser.locUser.level);

        var levelTask = Database.UpdateUser("level", LocalUser.locUser.level);
        var experienceTask = Database.UpdateUser("experience", LocalUser.locUser.experience);
        yield return new WaitUntil(() => levelTask.IsCompleted && experienceTask.IsCompleted);
        while (levelTask.IsFaulted) {
            levelTask = Database.UpdateUser("level", LocalUser.locUser.level);
            yield return new WaitUntil(() => levelTask.IsCompleted);
        }
        while (experienceTask.IsFaulted) {
            experienceTask = Database.UpdateUser("experience", LocalUser.locUser.experience);
            yield return new WaitUntil(() => experienceTask.IsCompleted);
        }
    }

    private IEnumerator ChangeExperienceToLevelNeeded() {
        LocalUser.locUser.experienceToNextLevel = (int)Mathf.Ceil(LocalUser.locUser.experienceToNextLevel * 1.5f);
        var task = Database.UpdateUser("experienceToNextLevel", LocalUser.locUser.experienceToNextLevel);
        yield return new WaitUntil(() => task.IsCompleted);
        while (task.IsFaulted) {
            task = Database.UpdateUser("experienceToNextLevel", LocalUser.locUser.experienceToNextLevel);
            yield return new WaitUntil(() => task.IsCompleted);
        }
    }

    public static int GetLevelNumber() {
        return LocalUser.locUser.level;
    }

    public static int GetExperience() {
        return LocalUser.locUser.experience;
    }

    public static int GetExperienceToNextLevel() {
        return LocalUser.locUser.experienceToNextLevel;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem : MonoBehaviour{

    public IEnumerator AddExperience(int amount) {
        LocalUser.locUser.experience += amount;
        var task = FireBaseScript.UpdateUser("experience", LocalUser.locUser.experience);
        yield return new WaitUntil(() => task.IsCompleted);
        while (task.IsFaulted) {
            task = FireBaseScript.UpdateUser("experience", LocalUser.locUser.experience);
            yield return new WaitUntil(() => task.IsCompleted);
        }
        if (GetExperienceToNextLevel() == 0) {
            LocalUser.locUser.experienceToNextLevel = 100;
            var experienceNeededTask = FireBaseScript.UpdateUser("experienceToNextLevel", LocalUser.locUser.experienceToNextLevel);
            yield return new WaitUntil(() => experienceNeededTask.IsCompleted);
            while (experienceNeededTask.IsFaulted) {
                experienceNeededTask = FireBaseScript.UpdateUser("experienceToNextLevel", LocalUser.locUser.experienceToNextLevel);
                yield return new WaitUntil(() => experienceNeededTask.IsCompleted);
            }
        }
        while (LocalUser.locUser.experience >= GetExperienceToNextLevel()) {
            // Enough experience to level up
            yield return StartCoroutine(AddLevel());
            yield return StartCoroutine(ChangeExperienceToLevelNeeded());
        }
    }

    private IEnumerator AddLevel() {
        LocalUser.locUser.experience -= GetExperienceToNextLevel();
        LocalUser.locUser.level++;
        var levelTask = FireBaseScript.UpdateUser("level", LocalUser.locUser.level);
        var experienceTask = FireBaseScript.UpdateUser("experience", LocalUser.locUser.experience);
        yield return new WaitUntil(() => levelTask.IsCompleted && experienceTask.IsCompleted);
        while (levelTask.IsFaulted) {
            levelTask = FireBaseScript.UpdateUser("level", LocalUser.locUser.level);
            yield return new WaitUntil(() => levelTask.IsCompleted);
        }
        while (experienceTask.IsFaulted) {
            experienceTask = FireBaseScript.UpdateUser("experience", LocalUser.locUser.experience);
            yield return new WaitUntil(() => experienceTask.IsCompleted);
        }
    }

    private IEnumerator ChangeExperienceToLevelNeeded() {
        LocalUser.locUser.experienceToNextLevel = (int)Mathf.Ceil(LocalUser.locUser.experienceToNextLevel * 1.5f);
        var task = FireBaseScript.UpdateUser("experienceToNextLevel", LocalUser.locUser.experienceToNextLevel);
        yield return new WaitUntil(() => task.IsCompleted);
        while (task.IsFaulted) {
            task = FireBaseScript.UpdateUser("experienceToNextLevel", LocalUser.locUser.experienceToNextLevel);
            yield return new WaitUntil(() => task.IsCompleted);
        }
    }

    public int GetLevelNumber() {
        return LocalUser.locUser.level;
    }

    public int GetExperience() {
        return LocalUser.locUser.experience;
    }

    public int GetExperienceToNextLevel() {
        return LocalUser.locUser.experienceToNextLevel;
    }

}

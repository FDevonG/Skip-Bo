using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using Object = System.Object;

public class Database : MonoBehaviour
{

    public static Task<bool> WriteNewUser(User user) {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://skip-bo-52535022.firebaseio.com/");
        string json = JsonUtility.ToJson(user);
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        return databaseReference.Child("users").Child(FirebaseAuthentication.AuthenitcationKey()).SetRawJsonValueAsync(json).ContinueWith((task) => {
            return task.IsCompleted;
        });
    }

    public static Task<bool> UpdateUser(string saveName, Object varToSave) {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://skip-bo-52535022.firebaseio.com/");
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        return databaseReference.Child("users").Child(FirebaseAuthentication.AuthenitcationKey()).Child(saveName).SetValueAsync(varToSave).ContinueWith((task) => {
            return task.IsCompleted;
        });
    }

    public static void DeleteAccountData() {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://skip-bo-52535022.firebaseio.com/");
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(FirebaseAuthentication.AuthenitcationKey());
        databaseReference.RemoveValueAsync();
    }

    public static Task<string> GetCurrentUser() {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://skip-bo-52535022.firebaseio.com/");
        return FirebaseDatabase.DefaultInstance.GetReference("users").Child(FirebaseAuthentication.AuthenitcationKey()).GetValueAsync().ContinueWith(task => {
            return task.Result.GetRawJsonValue();
        });
    }

    public static Task<bool> SaveUserAchievments() {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://skip-bo-52535022.firebaseio.com/");
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        string json = "[";
        foreach (Achievment achievment in LocalUser.locUser.achievments) {
            json += JsonUtility.ToJson(achievment);
        }
        json += "]";
        return databaseReference.Child("users").Child(FirebaseAuthentication.AuthenitcationKey()).Child("achievments").SetRawJsonValueAsync(json).ContinueWith((task) => {
            return task.IsCompleted;
        });
    }

    public static Task<DataSnapshot> GetUserAchievments() {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://skip-bo-52535022.firebaseio.com/");
        return FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(FirebaseAuthentication.AuthenitcationKey()).Child("achievments").GetValueAsync().ContinueWith((task => {
            return task.Result;
        }));
    }

    public static Task<DataSnapshot> GetUsers() {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://skip-bo-52535022.firebaseio.com/");
        return FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync().ContinueWith(task => {
            return task.Result;
        });
    }
}

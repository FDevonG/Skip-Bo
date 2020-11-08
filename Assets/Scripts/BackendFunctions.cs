using System.Threading.Tasks;
using Firebase.Functions;
using System.Collections.Generic;

public static class BackendFunctions
{
    public static Task<List<string>> NameCheck(string playerName)
    {
        var functions = FirebaseFunctions.DefaultInstance;
        var function = functions.GetHttpsCallable("nameCheck");
        return function.CallAsync(playerName).ContinueWith((task) => {
            return (List<string>)task.Result.Data;
        });
    }

    public static Task<string>GetUser(string userID)
    {
        var functions = FirebaseFunctions.DefaultInstance;
        var function = functions.GetHttpsCallable("getUser");
        return function.CallAsync(userID).ContinueWith((task) => {
            return (string)task.Result.Data;
        });
    }

    public static Task<string> FindUser(string userName)
    {
        var functions = FirebaseFunctions.DefaultInstance;
        var function = functions.GetHttpsCallable("findUser");
        return function.CallAsync(userName).ContinueWith((task) => {
            return (string)task.Result.Data;
        });
    }

    public static Task<string> GetUsersArray(List<string> ids)
    {
        var functions = FirebaseFunctions.DefaultInstance;
        var function = functions.GetHttpsCallable("getUsersArray");
        return function.CallAsync(ids.ToArray()).ContinueWith((task) =>
        {
            return (string)task.Result.Data;
        });
    }

    public static Task<string> LoadLeaderBoards(int selection)
    {
        string[] dataToSend = new string[2];
        dataToSend[0] = selection.ToString();
        dataToSend[1] = FirebaseAuthentication.AuthenitcationKey();
        var functions = FirebaseFunctions.DefaultInstance;
        var function = functions.GetHttpsCallable("leaderboardsOpened");
        return function.CallAsync(dataToSend).ContinueWith((task) => {
            return (string)task.Result.Data;
        });
    }

    public static Task<string> LoadLeaderBoardPage(int[] data)
    {
        var functions = FirebaseFunctions.DefaultInstance;
        var function = functions.GetHttpsCallable("leaderboardPage");
        return function.CallAsync(data).ContinueWith((task) => {
            return (string)task.Result.Data;
        });
    }
    
    public static Task<string> GetLastPage(int selectionValue)
    {
        var functions = FirebaseFunctions.DefaultInstance;
        var function = functions.GetHttpsCallable("getLastPage");
        return function.CallAsync(selectionValue).ContinueWith((task) => {
            return (string)task.Result.Data;
        });
    }
}

using Firebase;
using UnityEngine;

public class AnaliticsTool : MonoBehaviour
{
    private FirebaseApp _firebaseApp;
    private static bool _isFirebaseInited;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                _firebaseApp = FirebaseApp.DefaultInstance;
                _isFirebaseInited = true;
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }


    /// <summary>
    /// Send analitic event with result
    /// </summary>
    /// <param name="towerHeight"></param>
    /// <param name="continuesTaken"></param>
    public static void LogGameResult(int towerHeight, int continuesTaken)
    {
        if (!_isFirebaseInited)
            return;

        Firebase.Analytics.FirebaseAnalytics.LogEvent(
          "GameEndResult",
          new Firebase.Analytics.Parameter(
            "TowerHeight", towerHeight),
          new Firebase.Analytics.Parameter(
            "ContinuesTaken", continuesTaken)
        );
    }

    public static void LogGPGSAuthenticateStatus(string status)
    {
        if (!_isFirebaseInited)
            return;

        Firebase.Analytics.FirebaseAnalytics.LogEvent("PGSAuthenticateStatus", "status", status);
    }

    public static void LogLeaderboardOpen()
    {
        if (!_isFirebaseInited)
            return;

        Firebase.Analytics.FirebaseAnalytics.LogEvent("LogLeaderboardOpen", "none", 0);
    }

    /// <summary>
    /// Send analitic for perfect moves statistic for game
    /// </summary>
    /// <param name="perfectMovesCount"></param>
    /// <param name="totalmovesCount"></param>
    /// <param name="bigestStreak"></param>
    public static void LogPerfectMovesStatistic(int perfectMovesCount, int totalmovesCount, int bigestStreak)
    {
        if (!_isFirebaseInited)
            return;

        var perfectMovePercent = perfectMovesCount / (float)totalmovesCount;

        Firebase.Analytics.FirebaseAnalytics.LogEvent(
          "PerfectMovesStatistic",
          new Firebase.Analytics.Parameter(
            "PerfectMovesCount", perfectMovesCount),
          new Firebase.Analytics.Parameter(
            "TotalmovesCount", totalmovesCount),
          new Firebase.Analytics.Parameter(
            "PerfectMovePercent", perfectMovePercent),
          new Firebase.Analytics.Parameter(
            "BigestStreak", bigestStreak)
        );
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameFlowManager
{
    private static bool matchEnding;

    public static bool IsMatchEnding => matchEnding;

    public static void ResetMatchState()
    {
        matchEnding = false;
    }

    public static void RequestGameOver()
    {
        if (matchEnding)
        {
            return;
        }

        matchEnding = true;
        SceneManager.LoadScene("CenaGameOver");
    }
}
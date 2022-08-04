 using System;
 using UnityEngine;
 using Steamworks;


 public class SteamLeaderboardController : MonoBehaviour
{
     // Steamworks Integration
     private CallResult<LeaderboardFindResult_t> _leaderboardFindResult;
     private CallResult<LeaderboardScoreUploaded_t> _leaderboardScoreUploadedResult;
     private SteamLeaderboard_t _steamLeaderboard;
     private int scoreToUpload = 0;

     public void UploadScoreToSteamLeaderboard(String leaderboardName, int score)
     {
         scoreToUpload = score;
         InitializeSteamLeaderboards();
     }
    
     private void OnEnable()
     {
         InitializeSteamLeaderboards();
     }
 
     private void InitializeSteamLeaderboards()
     {
         if (!SteamManager.Initialized) return;
         // Create Callback objects
         _leaderboardFindResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFindResult);
         _leaderboardScoreUploadedResult = CallResult<LeaderboardScoreUploaded_t>.Create(OnLeaderboardUploaded);
         
         // Find or create leaderboard
         SteamAPICall_t handle = SteamUserStats.FindOrCreateLeaderboard("AllTimeHighScores", ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
         _leaderboardFindResult.Set(handle);
     }
     
     private void OnLeaderboardFindResult(LeaderboardFindResult_t pCallback, bool bIOFailure)
     {
         if (!SteamManager.Initialized) return;
         if (!pCallback.m_bLeaderboardFound.Equals(1))
         {
             Debug.Log("Error Finding Leaderboard");
             return;
         }
         
         _steamLeaderboard = pCallback.m_hSteamLeaderboard;
         if (scoreToUpload != 0)
         {
             UploadScoreToSteam(scoreToUpload);
         }
     }
 
     private void OnLeaderboardUploaded(LeaderboardScoreUploaded_t pCallback, bool bIOFailure)
     {
         if (!bIOFailure)
         {
             Debug.Log("Steam Score Upload Success");
         }
         else
         {
             Debug.Log("Steam Score Upload Failure");
         }
     }
 
     private void UploadScoreToSteam(int score)
     {
         SteamAPICall_t handle = SteamUserStats.UploadLeaderboardScore(_steamLeaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, score, null, 0);
         _leaderboardScoreUploadedResult.Set(handle);
     }
}

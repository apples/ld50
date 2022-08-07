 using System;
 using System.Collections.Generic;
 using UnityEngine;
 using Steamworks;


 public class SteamLeaderboardController : MonoBehaviour
 {
     [SerializeField] private VoidEvent onSteamScoresLoaded;
    
     // Steamworks Integration
     private CallResult<LeaderboardFindResult_t> _leaderboardFindResult;
     private CallResult<LeaderboardScoreUploaded_t> _leaderboardScoreUploadedResult;
     private CallResult<LeaderboardScoresDownloaded_t> _leaderboardScoresDownloaded;
     private SteamLeaderboard_t _steamLeaderboard;
     
     private int scoreToUpload = 0;
     private List<PersistentData.ScoreEntry> scoreEntries = new();
     public List<PersistentData.ScoreEntry> ScoreEntries => scoreEntries;
     
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
         _leaderboardScoreUploadedResult = CallResult<LeaderboardScoreUploaded_t>.Create(OnLeaderboardScoreUploaded);
         _leaderboardScoresDownloaded = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloaded);

         // Find or create leaderboard
         SteamAPICall_t handle = SteamUserStats.FindOrCreateLeaderboard("AllTimeHighScores", ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
         _leaderboardFindResult.Set(handle);
     }

     private void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
     {
         if (!bIOFailure)
         {
             Debug.Log("Steam Score Download Success");
             for (int i = 0; i < pCallback.m_cEntryCount; i++)
             {
                 LeaderboardEntry_t leaderboardEntry;
                 SteamUserStats.GetDownloadedLeaderboardEntry(pCallback.m_hSteamLeaderboardEntries, i, out leaderboardEntry, null, 0);
                 string userNickname = SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser);
                 
                 PersistentData.ScoreEntry scoreEntry = new PersistentData.ScoreEntry();
                 scoreEntry.score = leaderboardEntry.m_nScore;
                 scoreEntry.globalRank = leaderboardEntry.m_nGlobalRank;
                 scoreEntry.persona = userNickname ?? "Steam User";
                 scoreEntries.Add(scoreEntry);
             }
             // we get 9 entries before and after the player's score
             // this will cut down the number of entries
             scoreEntries = scoreEntries.GetRange(0, Math.Min(scoreEntries.Count, 10));
             onSteamScoresLoaded.Raise();
             Debug.Log("Loaded all leaderboard entries");
         }
         else
         {
             Debug.Log("Steam Score Download Failure");
         }
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
         else
         {
             SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries(_steamLeaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, -9, 9);
             _leaderboardScoresDownloaded.Set(handle);
         }
     }
 
     private void OnLeaderboardScoreUploaded(LeaderboardScoreUploaded_t pCallback, bool bIOFailure)
     {
         if (!bIOFailure)
         {
             Debug.Log("Steam Score Upload Success");
             SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries(_steamLeaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, -10, 10);
             _leaderboardScoresDownloaded.Set(handle);
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

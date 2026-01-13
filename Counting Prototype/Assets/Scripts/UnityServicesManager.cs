using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using Unity.Services.RemoteConfig;
using Unity.Services.Analytics;
using Unity.Services.Analytics.Platform;
using Unity.Notifications;

namespace UnityServicesFramework
{
    public class UnityServicesManager : MonoBehaviour
    {
        private static UnityServicesManager _instance;
        public static UnityServicesManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("UnityServicesManager");
                    _instance = go.AddComponent<UnityServicesManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        public bool IsInitialized { get; private set; }
        public bool IsSignedIn => AuthenticationService.Instance.IsSignedIn;
        public string PlayerId => AuthenticationService.Instance.PlayerId;

        public event Action OnInitialized;
        public event Action<string> OnSignedIn;
        public event Action OnSignedOut;

        private struct UserAttributes { }
        private struct AppAttributes { }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ========== INITIALIZATION ==========
        
        public async Task<bool> InitializeAsync()
        {
            try
            {
                await UnityServices.InitializeAsync();
                IsInitialized = true;
                OnInitialized?.Invoke();
                Debug.Log("Unity Services initialized successfully");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Unity Services initialization failed: {e.Message}");
                return false;
            }
        }

        // ========== AUTHENTICATION ==========

        public async Task<bool> SignInAnonymouslyAsync()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                OnSignedIn?.Invoke(PlayerId);
                Debug.Log($"Signed in anonymously. Player ID: {PlayerId}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Anonymous sign-in failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> SignInWithUsernamePasswordAsync(string username, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
                OnSignedIn?.Invoke(PlayerId);
                Debug.Log($"Signed in with username. Player ID: {PlayerId}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Username/password sign-in failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> SignUpWithUsernamePasswordAsync(string username, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                Debug.Log($"Account created successfully. Player ID: {PlayerId}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Sign-up failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> SignInWithGoogleAsync(string idToken)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithGoogleAsync(idToken);
                OnSignedIn?.Invoke(PlayerId);
                Debug.Log($"Signed in with Google. Player ID: {PlayerId}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Google sign-in failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> SignInWithAppleAsync(string idToken)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithAppleAsync(idToken);
                OnSignedIn?.Invoke(PlayerId);
                Debug.Log($"Signed in with Apple. Player ID: {PlayerId}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Apple sign-in failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> SignInWithFacebookAsync(string accessToken)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithFacebookAsync(accessToken);
                OnSignedIn?.Invoke(PlayerId);
                Debug.Log($"Signed in with Facebook. Player ID: {PlayerId}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Facebook sign-in failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> LinkWithGoogleAsync(string idToken)
        {
            try
            {
                await AuthenticationService.Instance.LinkWithGoogleAsync(idToken);
                Debug.Log("Google account linked successfully");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Google link failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> LinkWithAppleAsync(string idToken)
        {
            try
            {
                await AuthenticationService.Instance.LinkWithAppleAsync(idToken);
                Debug.Log("Apple account linked successfully");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Apple link failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> LinkWithFacebookAsync(string accessToken)
        {
            try
            {
                await AuthenticationService.Instance.LinkWithFacebookAsync(accessToken);
                Debug.Log("Facebook account linked successfully");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Facebook link failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> UnlinkGoogleAsync()
        {
            try
            {
                await AuthenticationService.Instance.UnlinkGoogleAsync();
                Debug.Log("Google account unlinked");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Google unlink failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> UnlinkAppleAsync()
        {
            try
            {
                await AuthenticationService.Instance.UnlinkAppleAsync();
                Debug.Log("Apple account unlinked");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Apple unlink failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> UnlinkFacebookAsync()
        {
            try
            {
                await AuthenticationService.Instance.UnlinkFacebookAsync();
                Debug.Log("Facebook account unlinked");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Facebook unlink failed: {e.Message}");
                return false;
            }
        }

        public void SignOut()
        {
            AuthenticationService.Instance.SignOut();
            OnSignedOut?.Invoke();
            Debug.Log("Signed out successfully");
        }

        public async Task DeleteAccountAsync()
        {
            try
            {
                await AuthenticationService.Instance.DeleteAccountAsync();
                Debug.Log("Account deleted successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Account deletion failed: {e.Message}");
            }
        }

        // ========== CLOUD SAVE ==========

        public async Task<bool> SaveDataAsync(string key, object value)
        {
            try
            {
                var data = new Dictionary<string, object> { { key, value } };
                await CloudSaveService.Instance.Data.Player.SaveAsync(data);
                Debug.Log($"Data saved: {key}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Save failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> SaveMultipleDataAsync(Dictionary<string, object> data)
        {
            try
            {
                await CloudSaveService.Instance.Data.Player.SaveAsync(data);
                Debug.Log($"Multiple data saved: {data.Count} items");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Multiple save failed: {e.Message}");
                return false;
            }
        }

        public async Task<T> LoadDataAsync<T>(string key, T defaultValue = default)
        {
            try
            {
                var results = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { key });
                if (results.TryGetValue(key, out var item))
                {
                    return item.Value.GetAs<T>();
                }
                return defaultValue;
            }
            catch (Exception e)
            {
                Debug.LogError($"Load failed for {key}: {e.Message}");
                return defaultValue;
            }
        }

        public async Task<Dictionary<string, string>> LoadAllDataAsync()
        {
            try
            {
                var keys = await CloudSaveService.Instance.Data.Player.ListAllKeysAsync();
                var results = await CloudSaveService.Instance.Data.Player.LoadAllAsync();
                
                var data = new Dictionary<string, string>();
                foreach (var item in results)
                {
                    data[item.Key] = item.Value.Value.GetAsString();
                }
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"Load all failed: {e.Message}");
                return new Dictionary<string, string>();
            }
        }

        public async Task<bool> DeleteDataAsync(string key)
        {
            try
            {
                await CloudSaveService.Instance.Data.Player.DeleteAsync(key);
                Debug.Log($"Data deleted: {key}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Delete failed: {e.Message}");
                return false;
            }
        }

        // ========== LEADERBOARDS ==========

        public async Task<bool> AddLeaderboardScoreAsync(string leaderboardId, double score)
        {
            try
            {
                await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);
                Debug.Log($"Score {score} added to leaderboard {leaderboardId}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Add score failed: {e.Message}");
                return false;
            }
        }

        public async Task<LeaderboardScoresPage> GetLeaderboardScoresAsync(string leaderboardId, int limit = 10, int offset = 0)
        {
            try
            {
                var scores = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, new GetScoresOptions { Offset = offset, Limit = limit });
                Debug.Log($"Retrieved {scores.Results.Count} scores from {leaderboardId}");
                return scores;
            }
            catch (Exception e)
            {
                Debug.LogError($"Get leaderboard failed: {e.Message}");
                return null;
            }
        }

        public async Task<LeaderboardEntry> GetPlayerScoreAsync(string leaderboardId)
        {
            try
            {
                var entry = await LeaderboardsService.Instance.GetPlayerScoreAsync(leaderboardId);
                Debug.Log($"Player rank: {entry.Rank}, Score: {entry.Score}");
                return entry;
            }
            catch (Exception e)
            {
                Debug.LogError($"Get player score failed: {e.Message}");
                return null;
            }
        }

        public async Task<LeaderboardScores> GetPlayerRangeAsync(string leaderboardId, int rangeLimit = 10)
        {
            try
            {
                var scores = await LeaderboardsService.Instance.GetPlayerRangeAsync(leaderboardId, new GetPlayerRangeOptions { RangeLimit = rangeLimit });
                return scores;
            }
            catch (Exception e)
            {
                Debug.LogError($"Get player range failed: {e.Message}");
                return null;
            }
        }

        // ========== REMOTE CONFIG ==========

        public async Task FetchRemoteConfigAsync()
        {
            try
            {
                await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
                Debug.Log("Remote config fetched successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Fetch remote config failed: {e.Message}");
            }
        }

        public string GetRemoteConfigString(string key, string defaultValue = "")
        {
            return RemoteConfigService.Instance.appConfig.GetString(key, defaultValue);
        }

        public int GetRemoteConfigInt(string key, int defaultValue = 0)
        {
            return RemoteConfigService.Instance.appConfig.GetInt(key, defaultValue);
        }

        public float GetRemoteConfigFloat(string key, float defaultValue = 0f)
        {
            return RemoteConfigService.Instance.appConfig.GetFloat(key, defaultValue);
        }

        public bool GetRemoteConfigBool(string key, bool defaultValue = false)
        {
            return RemoteConfigService.Instance.appConfig.GetBool(key, defaultValue);
        }

        public bool HasRemoteConfigKey(string key)
        {
            return RemoteConfigService.Instance.appConfig.HasKey(key);
        }

        // ========== ANALYTICS ==========

        public void StartDataCollection()
        {
            try
            {
                AnalyticsService.Instance.StartDataCollection();
                Debug.Log("Analytics data collection started");
            }
            catch (Exception e)
            {
                Debug.LogError($"Start data collection failed: {e.Message}");
            }
        }

        public void StopDataCollection()
        {
            try
            {
                AnalyticsService.Instance.StopDataCollection();
                Debug.Log("Analytics data collection stopped");
            }
            catch (Exception e)
            {
                Debug.LogError($"Stop data collection failed: {e.Message}");
            }
        }

        public async Task RequestDataDeletion()
        {
            try
            {
                // await AnalyticsService.Instance.RequestDataDeletionAsync();
                AnalyticsService.Instance.RequestDataDeletion();

                Debug.Log("Analytics data deletion requested");
            }
            catch (Exception e)
            {
                Debug.LogError($"Data deletion request failed: {e.Message}");
            }
        }

        public void RecordCustomEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            try
            {
                CustomEvent customEvent = new CustomEvent(eventName);
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        customEvent.Add(param.Key, param.Value);
                    }
                }
                AnalyticsService.Instance.RecordEvent(customEvent);
                Debug.Log($"Analytics event recorded: {eventName}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Analytics event failed: {e.Message}");
            }
        }

        public void RecordLevelStart(string levelName)
        {
            RecordCustomEvent("level_start", new Dictionary<string, object> { { "level_name", levelName } });
        }

        public void RecordLevelComplete(string levelName, int score)
        {
            RecordCustomEvent("level_complete", new Dictionary<string, object> 
            { 
                { "level_name", levelName },
                { "score", score }
            });
        }

        public void RecordPurchase(string itemId, decimal price, string currency = "USD")
        {
            RecordCustomEvent("item_purchased", new Dictionary<string, object>
            {
                { "item_id", itemId },
                { "price", price },
                { "currency", currency }
            });
        }

        public void RecordAdImpression(string adType, string placementId)
        {
            RecordCustomEvent("ad_impression", new Dictionary<string, object>
            {
                { "ad_type", adType },
                { "placement_id", placementId }
            });
        }

        public void FlushAnalytics()
        {
            try
            {
                AnalyticsService.Instance.Flush();
                Debug.Log("Analytics events flushed");
            }
            catch (Exception e)
            {
                Debug.LogError($"Analytics flush failed: {e.Message}");
            }
        }

        // ========== PUSH NOTIFICATIONS (MOBILE) ==========

#if UNITY_ANDROID || UNITY_IOS
        public void InitializePushNotifications()
        {
            try
            {
#if UNITY_ANDROID
                var channel = new AndroidNotificationChannel
                {
                    Id = "default_channel",
                    Name = "Default Channel",
                    Importance = Importance.Default,
                    Description = "Generic notifications"
                };
                AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif
                Debug.Log("Push notifications initialized");
            }
            catch (Exception e)
            {
                Debug.LogError($"Push notification init failed: {e.Message}");
            }
        }

        public void ScheduleLocalNotification(string title, string body, int delaySeconds)
        {
            try
            {
#if UNITY_ANDROID
                var notification = new AndroidNotification
                {
                    Title = title,
                    Text = body,
                    FireTime = DateTime.Now.AddSeconds(delaySeconds),
                    SmallIcon = "icon_small",
                    LargeIcon = "icon_large"
                };
                AndroidNotificationCenter.SendNotification(notification, "default_channel");
#elif UNITY_IOS
                var notification = new iOSNotification
                {
                    Title = title,
                    Body = body,
                    ShowInForeground = true,
                    ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
                    Trigger = new iOSNotificationTimeIntervalTrigger
                    {
                        TimeInterval = TimeSpan.FromSeconds(delaySeconds),
                        Repeats = false
                    }
                };
                iOSNotificationCenter.ScheduleNotification(notification);
#endif
                Debug.Log($"Notification scheduled: {title}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Schedule notification failed: {e.Message}");
            }
        }

        public void CancelAllNotifications()
        {
#if UNITY_ANDROID
            AndroidNotificationCenter.CancelAllNotifications();
#elif UNITY_IOS
            iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif
            Debug.Log("All notifications cancelled");
        }
#endif

        // ========== UTILITY ==========

        public async Task<bool> InitializeAndSignInAsync()
        {
            bool initialized = await InitializeAsync();
            if (!initialized) return false;

            bool signedIn = await SignInAnonymouslyAsync();
            return signedIn;
        }
    }
}
/*
 * Offline Sync Manager for Pachinko Game
 * Handles syncing local database with remote server
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using PachinkoGame.Data;

namespace PachinkoGame.Sync
{
    public class OfflineSyncManager : MonoBehaviour
    {
        [Header("Sync Settings")]
        [SerializeField] private string serverUrl = "https://yourserver.com/api/pachinko";
        [SerializeField] private float autoSyncInterval = 300f; // Auto-sync every 5 minutes
        [SerializeField] private bool enableAutoSync = true;
        [SerializeField] private int batchSize = 50; // Sync 50 records at a time

        [Header("Network Detection")]
        [SerializeField] private bool requireWiFi = false;

        private float _lastSyncTime;
        private bool _isSyncing = false;

        public event Action OnSyncStarted;
        public event Action<int> OnSyncProgress;
        public event Action<int, int> OnSyncCompleted; // (successCount, failCount)
        public event Action<string> OnSyncFailed;

        void Start()
        {
            _lastSyncTime = Time.time;

            // Perform initial sync check
            if (IsNetworkAvailable())
            {
                StartCoroutine(SyncData());
            }
        }

        void Update()
        {
            // Auto-sync check
            if (enableAutoSync && !_isSyncing && IsNetworkAvailable())
            {
                if (Time.time - _lastSyncTime >= autoSyncInterval)
                {
                    StartCoroutine(SyncData());
                }
            }
        }

        /// <summary>
        /// Check if network is available for syncing
        /// </summary>
        public bool IsNetworkAvailable()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return false;
            }

            if (requireWiFi && Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Manually trigger sync
        /// </summary>
        public void TriggerSync()
        {
            if (_isSyncing)
            {
                Debug.LogWarning("Sync already in progress");
                return;
            }

            if (!IsNetworkAvailable())
            {
                Debug.LogWarning("Network not available for sync");
                OnSyncFailed?.Invoke("Network not available");
                return;
            }

            StartCoroutine(SyncData());
        }

        /// <summary>
        /// Main sync coroutine
        /// </summary>
        private IEnumerator SyncData()
        {
            _isSyncing = true;
            _lastSyncTime = Time.time;
            OnSyncStarted?.Invoke();

            Debug.Log("Starting sync process...");

            var dbManager = PachinkoDBManager.Instance;
            List<PachinkoData> unsyncedRecords = dbManager.GetUnsyncedRecords();

            Debug.Log($"Found {unsyncedRecords.Count} unsynced records");

            if (unsyncedRecords.Count == 0)
            {
                Debug.Log("No records to sync");
                _isSyncing = false;
                OnSyncCompleted?.Invoke(0, 0);
                yield break;
            }

            int successCount = 0;
            int failCount = 0;
            int totalRecords = unsyncedRecords.Count;

            // Sync in batches
            for (int i = 0; i < unsyncedRecords.Count; i += batchSize)
            {
                List<PachinkoData> batch = unsyncedRecords.GetRange(
                    i,
                    Mathf.Min(batchSize, unsyncedRecords.Count - i)
                );

                yield return StartCoroutine(SyncBatch(batch, (success, fail) =>
                {
                    successCount += success;
                    failCount += fail;
                }));

                // Update progress
                float progress = (float)(i + batch.Count) / totalRecords;
                OnSyncProgress?.Invoke(Mathf.RoundToInt(progress * 100));

                // Small delay between batches to avoid overwhelming the server
                yield return new WaitForSeconds(0.5f);
            }

            Debug.Log($"Sync completed: {successCount} successful, {failCount} failed");
            OnSyncCompleted?.Invoke(successCount, failCount);

            _isSyncing = false;
        }

        /// <summary>
        /// Sync a batch of records
        /// </summary>
        private IEnumerator SyncBatch(List<PachinkoData> batch, Action<int, int> callback)
        {
            int success = 0;
            int fail = 0;

            foreach (PachinkoData record in batch)
            {
                yield return StartCoroutine(SyncSingleRecord(record, (isSuccess) =>
                {
                    if (isSuccess)
                    {
                        success++;
                        // Mark as synced in local database
                        PachinkoDBManager.Instance.MarkAsSynced(record.Id);
                    }
                    else
                    {
                        fail++;
                    }
                }));
            }

            callback?.Invoke(success, fail);
        }

        /// <summary>
        /// Sync a single record to server
        /// </summary>
        private IEnumerator SyncSingleRecord(PachinkoData record, Action<bool> callback)
        {
            // Prepare JSON data
            string jsonData = JsonUtility.ToJson(record);

            using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"Successfully synced record {record.Id}");
                    callback?.Invoke(true);
                }
                else
                {
                    Debug.LogError($"Failed to sync record {record.Id}: {request.error}");
                    callback?.Invoke(false);
                }
            }
        }

        /// <summary>
        /// Get sync status information
        /// </summary>
        public SyncStatus GetSyncStatus()
        {
            var dbManager = PachinkoDBManager.Instance;
            int unsyncedCount = dbManager.GetUnsyncedCount();
            int totalCount = dbManager.GetTotalRecordCount();

            return new SyncStatus
            {
                IsSyncing = _isSyncing,
                UnsyncedCount = unsyncedCount,
                SyncedCount = totalCount - unsyncedCount,
                TotalCount = totalCount,
                LastSyncTime = _lastSyncTime,
                IsNetworkAvailable = IsNetworkAvailable(),
                NextAutoSync = enableAutoSync ? _lastSyncTime + autoSyncInterval : -1
            };
        }

        /// <summary>
        /// Force mark all records as unsynced (for testing or re-sync)
        /// </summary>
        public void MarkAllAsUnsynced()
        {
            var dbManager = PachinkoDBManager.Instance;
            int result = dbManager.ExecuteNonQuery("UPDATE PachinkoData SET IsSynced = 0");
            Debug.Log($"Marked {result} records as unsynced");
        }

        /// <summary>
        /// Download data from server and merge with local database
        /// </summary>
        public IEnumerator DownloadFromServer()
        {
            Debug.Log("Downloading data from server...");

            using (UnityWebRequest request = UnityWebRequest.Get(serverUrl))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        // Parse server response
                        string jsonResponse = request.downloadHandler.text;
                        ServerDataResponse response = JsonUtility.FromJson<ServerDataResponse>(jsonResponse);

                        if (response != null && response.data != null)
                        {
                            var dbManager = PachinkoDBManager.Instance;
                            int insertedCount = 0;

                            foreach (var serverRecord in response.data)
                            {
                                // Check if record already exists
                                try
                                {
                                    var existingRecord = dbManager.GetById(serverRecord.Id);
                                    if (existingRecord == null)
                                    {
                                        // Insert new record
                                        serverRecord.IsSynced = true; // Mark as synced since it came from server
                                        dbManager.Insert(serverRecord);
                                        insertedCount++;
                                    }
                                }
                                catch
                                {
                                    // Record doesn't exist, insert it
                                    serverRecord.IsSynced = true;
                                    dbManager.Insert(serverRecord);
                                    insertedCount++;
                                }
                            }

                            Debug.Log($"Downloaded and inserted {insertedCount} new records");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to parse server response: {ex.Message}");
                    }
                }
                else
                {
                    Debug.LogError($"Failed to download from server: {request.error}");
                }
            }
        }
    }

    #region Data Classes

    [Serializable]
    public class ServerDataResponse
    {
        public List<PachinkoData> data;
    }

    [Serializable]
    public class SyncStatus
    {
        public bool IsSyncing;
        public int UnsyncedCount;
        public int SyncedCount;
        public int TotalCount;
        public float LastSyncTime;
        public bool IsNetworkAvailable;
        public float NextAutoSync;

        public float SyncProgress => TotalCount > 0 ? (float)SyncedCount / TotalCount * 100f : 100f;
    }

    #endregion
}
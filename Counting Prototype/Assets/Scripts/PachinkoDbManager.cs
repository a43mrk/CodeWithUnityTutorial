/*
 * Copyright (c) 2025
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite;
using UnityEngine;

namespace PachinkoGame.Data
{
    public class PachinkoDBManager
    {
        private static PachinkoDBManager _instance;
        private static readonly object _lock = new object();
        private SQLiteConnection _connection;
        private readonly string _dbPath;

        // Cache for frequently accessed data
        private Dictionary<int, PachinkoData> _cache;
        private List<PachinkoData> _allRecordsCache;
        private DateTime _lastCacheUpdate;
        private readonly float _cacheExpirationSeconds = 60f; // Cache expires after 60 seconds
        private bool _cacheEnabled = true;

        public static PachinkoDBManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new PachinkoDBManager();
                        }
                    }
                }
                return _instance;
            }
        }

        private PachinkoDBManager()
        {
            _dbPath = Path.Combine(Application.persistentDataPath, "PachinkoGame.db");
            Debug.Log($"Database path: {_dbPath}");
            InitializeDatabase();
            InitializeCache();
        }

        private void InitializeCache()
        {
            _cache = new Dictionary<int, PachinkoData>();
            _allRecordsCache = null;
            _lastCacheUpdate = DateTime.MinValue;
        }

        private void InitializeDatabase()
        {
            try
            {
                _connection = new SQLiteConnection(_dbPath);
                _connection.CreateTable<PachinkoData>();
                Debug.Log("Database initialized successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize database: {ex.Message}");
                throw;
            }
        }

        public SQLiteConnection GetConnection()
        {
            return _connection;
        }

        #region Cache Management

        /// <summary>
        /// Enable or disable caching
        /// </summary>
        public void SetCacheEnabled(bool enabled)
        {
            _cacheEnabled = enabled;
            if (!enabled)
            {
                ClearCache();
            }
        }

        /// <summary>
        /// Check if cache is expired
        /// </summary>
        private bool IsCacheExpired()
        {
            return (DateTime.Now - _lastCacheUpdate).TotalSeconds > _cacheExpirationSeconds;
        }

        /// <summary>
        /// Clear all cache
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
            _allRecordsCache = null;
            _lastCacheUpdate = DateTime.MinValue;
            Debug.Log("Cache cleared");
        }

        /// <summary>
        /// Invalidate cache (forces refresh on next access)
        /// </summary>
        public void InvalidateCache()
        {
            _lastCacheUpdate = DateTime.MinValue;
            Debug.Log("Cache invalidated");
        }

        /// <summary>
        /// Add or update item in cache
        /// </summary>
        private void CacheItem(PachinkoData data)
        {
            if (_cacheEnabled && data != null)
            {
                _cache[data.Id] = data;
            }
        }

        /// <summary>
        /// Remove item from cache
        /// </summary>
        private void RemoveFromCache(int id)
        {
            if (_cache.ContainsKey(id))
            {
                _cache.Remove(id);
            }
        }

        /// <summary>
        /// Get cache statistics
        /// </summary>
        public CacheStatistics GetCacheStatistics()
        {
            return new CacheStatistics
            {
                CachedItemCount = _cache.Count,
                IsAllRecordsCached = _allRecordsCache != null,
                LastUpdateTime = _lastCacheUpdate,
                IsExpired = IsCacheExpired(),
                IsEnabled = _cacheEnabled
            };
        }

        #endregion

        #region CRUD Operations

        /// <summary>
        /// Insert a new PachinkoData record
        /// </summary>
        public int Insert(PachinkoData data)
        {
            try
            {
                int result = _connection.Insert(data);
                if (result > 0)
                {
                    CacheItem(data);
                    InvalidateCache(); // Invalidate list caches
                }
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Insert failed: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Update an existing PachinkoData record
        /// </summary>
        public int Update(PachinkoData data)
        {
            try
            {
                int result = _connection.Update(data);
                if (result > 0)
                {
                    CacheItem(data);
                    InvalidateCache(); // Invalidate list caches
                }
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Update failed: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Delete a PachinkoData record by ID
        /// </summary>
        public int Delete(int id)
        {
            try
            {
                int result = _connection.Delete<PachinkoData>(id);
                if (result > 0)
                {
                    RemoveFromCache(id);
                    InvalidateCache(); // Invalidate list caches
                }
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Delete failed: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Delete a PachinkoData record
        /// </summary>
        public int Delete(PachinkoData data)
        {
            try
            {
                int result = _connection.Delete(data);
                if (result > 0)
                {
                    RemoveFromCache(data.Id);
                    InvalidateCache(); // Invalidate list caches
                }
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Delete failed: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Get a PachinkoData record by ID
        /// </summary>
        public PachinkoData GetById(int id)
        {
            try
            {
                // Check cache first
                if (_cacheEnabled && _cache.ContainsKey(id))
                {
                    return _cache[id];
                }

                // Fetch from database
                PachinkoData data = _connection.Get<PachinkoData>(id);
                CacheItem(data);
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetById failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get all PachinkoData records
        /// </summary>
        public List<PachinkoData> GetAll()
        {
            try
            {
                // Check cache first
                if (_cacheEnabled && _allRecordsCache != null && !IsCacheExpired())
                {
                    return new List<PachinkoData>(_allRecordsCache);
                }

                // Fetch from database
                List<PachinkoData> records = _connection.Table<PachinkoData>().ToList();
                
                // Update cache
                if (_cacheEnabled)
                {
                    _allRecordsCache = new List<PachinkoData>(records);
                    _lastCacheUpdate = DateTime.Now;
                    
                    // Also cache individual items
                    foreach (var record in records)
                    {
                        CacheItem(record);
                    }
                }
                
                return records;
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetAll failed: {ex.Message}");
                return new List<PachinkoData>();
            }
        }

        #endregion

        #region Useful Queries

        /// <summary>
        /// Get all unsynced records (for offline sync)
        /// </summary>
        public List<PachinkoData> GetUnsyncedRecords()
        {
            try
            {
                return _connection.Table<PachinkoData>()
                    .Where(p => p.IsSynced == false)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetUnsyncedRecords failed: {ex.Message}");
                return new List<PachinkoData>();
            }
        }

        /// <summary>
        /// Get all synced records
        /// </summary>
        public List<PachinkoData> GetSyncedRecords()
        {
            try
            {
                return _connection.Table<PachinkoData>()
                    .Where(p => p.IsSynced == true)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetSyncedRecords failed: {ex.Message}");
                return new List<PachinkoData>();
            }
        }

        /// <summary>
        /// Mark a record as synced
        /// </summary>
        public int MarkAsSynced(int id)
        {
            try
            {
                var record = GetById(id);
                if (record != null)
                {
                    record.IsSynced = true;
                    return Update(record);
                }
                return 0;
            }
            catch (Exception ex)
            {
                Debug.LogError($"MarkAsSynced failed: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Mark multiple records as synced
        /// </summary>
        public int MarkMultipleAsSynced(List<int> ids)
        {
            try
            {
                int count = 0;
                _connection.BeginTransaction();
                
                foreach (int id in ids)
                {
                    var record = GetById(id);
                    if (record != null)
                    {
                        record.IsSynced = true;
                        count += _connection.Update(record);
                    }
                }
                
                _connection.Commit();
                InvalidateCache(); // Invalidate cache after bulk update
                return count;
            }
            catch (Exception ex)
            {
                _connection.Rollback();
                Debug.LogError($"MarkMultipleAsSynced failed: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Mark all unsynced records as synced
        /// </summary>
        public int MarkAllAsSynced()
        {
            try
            {
                int result = _connection.Execute("UPDATE PachinkoData SET IsSynced = 1 WHERE IsSynced = 0");
                InvalidateCache(); // Invalidate cache after bulk update
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"MarkAllAsSynced failed: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Get count of unsynced records
        /// </summary>
        public int GetUnsyncedCount()
        {
            try
            {
                return _connection.Table<PachinkoData>()
                    .Where(p => p.IsSynced == false)
                    .Count();
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetUnsyncedCount failed: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Mark a record as unsynced (e.g., after modification)
        /// </summary>
        public int MarkAsUnsynced(int id)
        {
            try
            {
                var record = GetById(id);
                if (record != null)
                {
                    record.IsSynced = false;
                    return Update(record);
                }
                return 0;
            }
            catch (Exception ex)
            {
                Debug.LogError($"MarkAsUnsynced failed: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Get records by machine name
        /// </summary>
        public List<PachinkoData> GetByMachineName(string machineName)
        {
            try
            {
                return _connection.Table<PachinkoData>()
                    .Where(p => p.MachineName == machineName)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetByMachineName failed: {ex.Message}");
                return new List<PachinkoData>();
            }
        }

        /// <summary>
        /// Get records by username
        /// </summary>
        public List<PachinkoData> GetByUserName(string userName)
        {
            try
            {
                return _connection.Table<PachinkoData>()
                    .Where(p => p.UserName == userName)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetByUserName failed: {ex.Message}");
                return new List<PachinkoData>();
            }
        }

        /// <summary>
        /// Get records by game status
        /// </summary>
        public List<PachinkoData> GetByGameStatus(string gameStatus)
        {
            try
            {
                return _connection.Table<PachinkoData>()
                    .Where(p => p.GameStatus == gameStatus)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetByGameStatus failed: {ex.Message}");
                return new List<PachinkoData>();
            }
        }

        /// <summary>
        /// Get records by difficulty
        /// </summary>
        public List<PachinkoData> GetByDifficulty(string difficulty)
        {
            try
            {
                return _connection.Table<PachinkoData>()
                    .Where(p => p.Difficulty == difficulty)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetByDifficulty failed: {ex.Message}");
                return new List<PachinkoData>();
            }
        }

        /// <summary>
        /// Get records within a date range
        /// </summary>
        public List<PachinkoData> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                return _connection.Table<PachinkoData>()
                    .Where(p => p.DateTime >= startDate && p.DateTime <= endDate)
                    .OrderByDescending(p => p.DateTime)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetByDateRange failed: {ex.Message}");
                return new List<PachinkoData>();
            }
        }

        /// <summary>
        /// Get top N records by balls won
        /// </summary>
        public List<PachinkoData> GetTopByBallsWon(int count)
        {
            try
            {
                return _connection.Table<PachinkoData>()
                    .OrderByDescending(p => p.BallsWon)
                    .Take(count)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetTopByBallsWon failed: {ex.Message}");
                return new List<PachinkoData>();
            }
        }

        /// <summary>
        /// Get top N records by total bonus
        /// </summary>
        public List<PachinkoData> GetTopByTotalBonus(int count)
        {
            try
            {
                return _connection.Table<PachinkoData>()
                    .OrderByDescending(p => p.BigBonus + p.SmallBonus)
                    .Take(count)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetTopByTotalBonus failed: {ex.Message}");
                return new List<PachinkoData>();
            }
        }

        /// <summary>
        /// Get the most recent N records
        /// </summary>
        public List<PachinkoData> GetRecentRecords(int count)
        {
            try
            {
                return _connection.Table<PachinkoData>()
                    .OrderByDescending(p => p.DateTime)
                    .Take(count)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetRecentRecords failed: {ex.Message}");
                return new List<PachinkoData>();
            }
        }

        /// <summary>
        /// Get statistics for a specific machine
        /// </summary>
        public MachineStatistics GetMachineStatistics(string machineName)
        {
            try
            {
                var records = GetByMachineName(machineName);
                if (records.Count == 0)
                    return null;

                return new MachineStatistics
                {
                    MachineName = machineName,
                    TotalGames = records.Count,
                    TotalBallShoots = records.Sum(r => r.BallShoots),
                    TotalBallsWon = records.Sum(r => r.BallsWon),
                    TotalBallsDeath = records.Sum(r => r.BallsDeath),
                    TotalBigBonus = records.Sum(r => r.BigBonus),
                    TotalSmallBonus = records.Sum(r => r.SmallBonus),
                    AverageBallsWon = records.Average(r => r.BallsWon),
                    AverageBallsDeath = records.Average(r => r.BallsDeath)
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetMachineStatistics failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get statistics for a specific user
        /// </summary>
        public UserStatistics GetUserStatistics(string userName)
        {
            try
            {
                var records = GetByUserName(userName);
                if (records.Count == 0)
                    return null;

                return new UserStatistics
                {
                    UserName = userName,
                    TotalGames = records.Count,
                    TotalBallShoots = records.Sum(r => r.BallShoots),
                    TotalBallsWon = records.Sum(r => r.BallsWon),
                    TotalBallsDeath = records.Sum(r => r.BallsDeath),
                    TotalBigBonus = records.Sum(r => r.BigBonus),
                    TotalSmallBonus = records.Sum(r => r.SmallBonus),
                    BestGame = records.OrderByDescending(r => r.BallsWon).First(),
                    AverageBallsWon = records.Average(r => r.BallsWon)
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetUserStatistics failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Search records with custom query
        /// </summary>
        public List<PachinkoData> ExecuteQuery(string query, params object[] args)
        {
            try
            {
                return _connection.Query<PachinkoData>(query, args);
            }
            catch (Exception ex)
            {
                Debug.LogError($"ExecuteQuery failed: {ex.Message}");
                return new List<PachinkoData>();
            }
        }

        /// <summary>
        /// Execute a non-query command (UPDATE, DELETE, etc.)
        /// </summary>
        public int ExecuteNonQuery(string query, params object[] args)
        {
            try
            {
                return _connection.Execute(query, args);
            }
            catch (Exception ex)
            {
                Debug.LogError($"ExecuteNonQuery failed: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Get total count of records
        /// </summary>
        public int GetTotalRecordCount()
        {
            try
            {
                return _connection.Table<PachinkoData>().Count();
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetTotalRecordCount failed: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Delete all records (use with caution!)
        /// </summary>
        public int DeleteAll()
        {
            try
            {
                return _connection.DeleteAll<PachinkoData>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"DeleteAll failed: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Vacuum the database to reclaim space
        /// </summary>
        public void VacuumDatabase()
        {
            try
            {
                _connection.Execute("VACUUM");
                Debug.Log("Database vacuumed successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError($"VacuumDatabase failed: {ex.Message}");
            }
        }

        #endregion

        /// <summary>
        /// Close the database connection
        /// </summary>
        public void Close()
        {
            try
            {
                _connection?.Close();
                _connection?.Dispose();
                Debug.Log("Database connection closed");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Close failed: {ex.Message}");
            }
        }
    }

    #region Statistics Classes

    public class MachineStatistics
    {
        public string MachineName { get; set; }
        public int TotalGames { get; set; }
        public int TotalBallShoots { get; set; }
        public int TotalBallsWon { get; set; }
        public int TotalBallsDeath { get; set; }
        public int TotalBigBonus { get; set; }
        public int TotalSmallBonus { get; set; }
        public double AverageBallsWon { get; set; }
        public double AverageBallsDeath { get; set; }
    }

    public class UserStatistics
    {
        public string UserName { get; set; }
        public int TotalGames { get; set; }
        public int TotalBallShoots { get; set; }
        public int TotalBallsWon { get; set; }
        public int TotalBallsDeath { get; set; }
        public int TotalBigBonus { get; set; }
        public int TotalSmallBonus { get; set; }
        public PachinkoData BestGame { get; set; }
        public double AverageBallsWon { get; set; }
    }

    public class CacheStatistics
    {
        public int CachedItemCount { get; set; }
        public bool IsAllRecordsCached { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public bool IsExpired { get; set; }
        public bool IsEnabled { get; set; }
    }

    #endregion
}
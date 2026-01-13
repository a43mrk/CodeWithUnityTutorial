/*
 * Example usage of PachinkoDBManager
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using PachinkoGame.Data;

public class PachinkoDBUsageExample : MonoBehaviour
{
    void Start()
    {
        // Access the singleton instance
        var dbManager = PachinkoDBManager.Instance;

        // Example 1: Insert a new record
        InsertExampleRecord();

        // Example 2: Get all records
        GetAllRecordsExample();

        // Example 3: Query by machine name
        QueryByMachineExample();

        // Example 4: Get statistics
        GetStatisticsExample();

        // Example 5: Get top performers
        GetTopPerformersExample();

        // Example 6: Sync operations (NEW)
        SyncOperationsExample();

        // Example 7: Cache management (NEW)
        CacheManagementExample();
    }

    void InsertExampleRecord()
    {
        var newGame = new PachinkoData
        {
            DateTime = DateTime.Now,
            MachineName = "Lucky Dragon",
            BallShoots = 1000,
            BallsDeath = 600,
            BallsWon = 400,
            BigBonus = 5,
            SmallBonus = 12,
            Difficulty = "Medium",
            GameStatus = "Completed",
            UserName = "Player1"
        };

        int result = PachinkoDBManager.Instance.Insert(newGame);
        
        if (result > 0)
        {
            Debug.Log($"Game record inserted successfully! ID: {newGame.Id}");
            Debug.Log($"Win Rate: {newGame.WinRate:F2}%");
            Debug.Log($"Total Bonus: {newGame.TotalBonus}");
        }
    }

    void GetAllRecordsExample()
    {
        List<PachinkoData> allGames = PachinkoDBManager.Instance.GetAll();
        Debug.Log($"Total games in database: {allGames.Count}");

        foreach (var game in allGames)
        {
            Debug.Log($"Game {game.Id}: {game.UserName} played {game.MachineName} on {game.DateTime}");
        }
    }

    void QueryByMachineExample()
    {
        List<PachinkoData> dragonGames = PachinkoDBManager.Instance.GetByMachineName("Lucky Dragon");
        Debug.Log($"Total games on Lucky Dragon: {dragonGames.Count}");

        // Get games from the last 7 days
        DateTime weekAgo = DateTime.Now.AddDays(-7);
        List<PachinkoData> recentGames = PachinkoDBManager.Instance.GetByDateRange(weekAgo, DateTime.Now);
        Debug.Log($"Games in the last 7 days: {recentGames.Count}");
    }

    void GetStatisticsExample()
    {
        // Get machine statistics
        MachineStatistics machineStats = PachinkoDBManager.Instance.GetMachineStatistics("Lucky Dragon");
        
        if (machineStats != null)
        {
            Debug.Log($"=== Statistics for {machineStats.MachineName} ===");
            Debug.Log($"Total Games: {machineStats.TotalGames}");
            Debug.Log($"Total Balls Won: {machineStats.TotalBallsWon}");
            Debug.Log($"Average Balls Won: {machineStats.AverageBallsWon:F2}");
            Debug.Log($"Total Big Bonus: {machineStats.TotalBigBonus}");
        }

        // Get user statistics
        UserStatistics userStats = PachinkoDBManager.Instance.GetUserStatistics("Player1");
        
        if (userStats != null)
        {
            Debug.Log($"=== Statistics for {userStats.UserName} ===");
            Debug.Log($"Total Games Played: {userStats.TotalGames}");
            Debug.Log($"Best Game Balls Won: {userStats.BestGame?.BallsWon}");
            Debug.Log($"Average Balls Won: {userStats.AverageBallsWon:F2}");
        }
    }

    void GetTopPerformersExample()
    {
        // Get top 5 games by balls won
        List<PachinkoData> topGames = PachinkoDBManager.Instance.GetTopByBallsWon(5);
        
        Debug.Log("=== Top 5 Games by Balls Won ===");
        for (int i = 0; i < topGames.Count; i++)
        {
            var game = topGames[i];
            Debug.Log($"{i + 1}. {game.UserName} - {game.BallsWon} balls won on {game.MachineName}");
        }

        // Get top 5 by bonus
        List<PachinkoData> topBonus = PachinkoDBManager.Instance.GetTopByTotalBonus(5);
        
        Debug.Log("=== Top 5 Games by Total Bonus ===");
        for (int i = 0; i < topBonus.Count; i++)
        {
            var game = topBonus[i];
            Debug.Log($"{i + 1}. {game.UserName} - {game.TotalBonus} bonus on {game.MachineName}");
        }
    }

    void UpdateRecordExample(int gameId)
    {
        // Get the record
        PachinkoData game = PachinkoDBManager.Instance.GetById(gameId);
        
        if (game != null)
        {
            // Modify some values
            game.GameStatus = "Updated";
            game.BallsWon += 100;

            // Update in database
            int result = PachinkoDBManager.Instance.Update(game);
            
            if (result > 0)
            {
                Debug.Log($"Game {gameId} updated successfully!");
            }
        }
    }

    void DeleteRecordExample(int gameId)
    {
        int result = PachinkoDBManager.Instance.Delete(gameId);
        
        if (result > 0)
        {
            Debug.Log($"Game {gameId} deleted successfully!");
        }
    }

    void CustomQueryExample()
    {
        // Execute a custom query
        string query = "SELECT * FROM PachinkoData WHERE BallsWon > ? AND Difficulty = ? ORDER BY BallsWon DESC";
        List<PachinkoData> results = PachinkoDBManager.Instance.ExecuteQuery(query, 500, "Hard");
        
        Debug.Log($"Found {results.Count} hard games with more than 500 balls won");
    }

    void OnApplicationQuit()
    {
        // Optional: Close the database connection when the application quits
        PachinkoDBManager.Instance.Close();
    }

    void SyncOperationsExample()
    {
        Debug.Log("=== Sync Operations Example ===");
        
        // Get unsynced records count
        int unsyncedCount = PachinkoDBManager.Instance.GetUnsyncedCount();
        Debug.Log($"Unsynced records: {unsyncedCount}");

        // Get all unsynced records
        List<PachinkoData> unsyncedRecords = PachinkoDBManager.Instance.GetUnsyncedRecords();
        Debug.Log($"Found {unsyncedRecords.Count} records to sync");

        if (unsyncedRecords.Count > 0)
        {
            // Simulate syncing to server
            Debug.Log("Syncing to server...");
            
            // After successful sync, mark records as synced
            List<int> syncedIds = new List<int>();
            foreach (var record in unsyncedRecords)
            {
                syncedIds.Add(record.Id);
                Debug.Log($"Synced record {record.Id}: {record.UserName} on {record.MachineName}");
            }

            // Mark multiple records as synced
            int markedCount = PachinkoDBManager.Instance.MarkMultipleAsSynced(syncedIds);
            Debug.Log($"Marked {markedCount} records as synced");

            // Alternative: Mark all as synced at once
            // int result = PachinkoDBManager.Instance.MarkAllAsSynced();
            // Debug.Log($"Marked {result} records as synced");
        }

        // Verify sync status
        unsyncedCount = PachinkoDBManager.Instance.GetUnsyncedCount();
        Debug.Log($"Remaining unsynced records: {unsyncedCount}");
    }

    void CacheManagementExample()
    {
        Debug.Log("=== Cache Management Example ===");
        
        var dbManager = PachinkoDBManager.Instance;

        // Get cache statistics
        CacheStatistics stats = dbManager.GetCacheStatistics();
        Debug.Log($"Cache enabled: {stats.IsEnabled}");
        Debug.Log($"Cached items: {stats.CachedItemCount}");
        Debug.Log($"All records cached: {stats.IsAllRecordsCached}");
        Debug.Log($"Cache expired: {stats.IsExpired}");
        Debug.Log($"Last update: {stats.LastUpdateTime}");

        // Test cache performance
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        // First call (cache miss - reads from DB)
        sw.Start();
        List<PachinkoData> records1 = dbManager.GetAll();
        sw.Stop();
        Debug.Log($"First GetAll (cache miss): {sw.ElapsedMilliseconds}ms - {records1.Count} records");

        // Second call (cache hit - reads from memory)
        sw.Reset();
        sw.Start();
        List<PachinkoData> records2 = dbManager.GetAll();
        sw.Stop();
        Debug.Log($"Second GetAll (cache hit): {sw.ElapsedMilliseconds}ms - {records2.Count} records");

        // Clear cache
        dbManager.ClearCache();
        Debug.Log("Cache cleared");

        // Third call (cache miss again)
        sw.Reset();
        sw.Start();
        List<PachinkoData> records3 = dbManager.GetAll();
        sw.Stop();
        Debug.Log($"Third GetAll (after clear): {sw.ElapsedMilliseconds}ms - {records3.Count} records");

        // Disable cache
        dbManager.SetCacheEnabled(false);
        Debug.Log("Cache disabled");

        // Enable cache again
        dbManager.SetCacheEnabled(true);
        Debug.Log("Cache enabled");
    }
}
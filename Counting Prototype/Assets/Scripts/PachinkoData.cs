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
using SQLite;

namespace PachinkoGame.Data
{
    [Table("PachinkoData")]
    public class PachinkoData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public DateTime DateTime { get; set; }

        [NotNull, MaxLength(100)]
        public string MachineName { get; set; }

        public int BallShoots { get; set; }

        public int BallsDeath { get; set; }

        public int BallsWon { get; set; }

        public int BigBonus { get; set; }

        public int SmallBonus { get; set; }

        [MaxLength(50)]
        public string Difficulty { get; set; }

        [MaxLength(50)]
        public string GameStatus { get; set; }

        [MaxLength(100)]
        public string UserName { get; set; }

        // Computed properties for convenience
        [Ignore]
        public int TotalBalls => BallsWon - BallsDeath;

        [Ignore]
        public int TotalBonus => BigBonus + SmallBonus;

        [Ignore]
        public float WinRate => BallShoots > 0 ? (float)BallsWon / BallShoots * 100f : 0f;
    }
}
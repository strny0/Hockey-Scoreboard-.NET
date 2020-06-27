using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace HockeyScoreboard
{
    public static partial class CustomTypes
    {
        public struct PlayerType : IEquatable<PlayerType>
        {
            private TimeSpan penaltyTimeLeft;
            private TimeSpan penaltyTimeSet;
            private TimeSpan penaltyOffset;
            public string Number { get; set; }
            public bool PenaltyRunning { get; set; }
            public TimeSpan PenaltyTimeLeft { get => penaltyTimeLeft; set => penaltyTimeLeft = value; }
            public TimeSpan PenaltyTimeSet { get => penaltyTimeSet; set => penaltyTimeSet = value; }
            public TimeSpan PenaltyOffset { get => penaltyOffset; set => penaltyOffset = value; }
            public bool PeriodIs2plus2 { get; set; }
            public int ScoreAtPeriodStart { get; set; }

            public override bool Equals(object obj) => Equals(obj);

            public override int GetHashCode() => GetHashCode();

            public static bool operator ==(PlayerType left, PlayerType right) => left.Equals(right);

            public static bool operator !=(PlayerType left, PlayerType right) => !(left == right);

            public bool Equals(PlayerType other) => Equals(other);
        }
        public enum PeriodState
        {
            First = 1,
            Second = 2,
            Third = 3,
            Extension = 4,
            SN = 5,
        }
        public enum GameState
        {
            Regular = 0,
            Break = 1,
            Timeout = 2
        }
    }
}

using ChessDotNet;
using ChessDotNet.Variants.Atomic;
using System;

namespace AtomicChessPuzzles.Models
{
    public class TimedTrainingSession
    {
        public string SessionID { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime EndsAt { get; private set; }
        public string CurrentFen { get; set; }
        public AtomicChessGame AssociatedGame { get; set; }
        public bool Ended
        { 
            get
            {
                return DateTime.UtcNow >= EndsAt;
            }
        }
        public bool RecordedInDb { get; set; }
        public TimedTrainingScore Score { get; set; }


        public TimedTrainingSession(string sessionId, DateTime startedAt, DateTime endsAt, string owner, string type)
        {
            SessionID = sessionId;
            StartedAt = startedAt;
            EndsAt = endsAt;
            RecordedInDb = false;
            Score = new TimedTrainingScore(0, type, owner);
        }

        public bool VerifyMove(string origin, string destination)
        {
            bool correctMove = false;
            MoveType moveType = AssociatedGame.ApplyMove(new Move(origin, destination, AssociatedGame.WhoseTurn), false);
            if (moveType != MoveType.Invalid)
            {
                correctMove = AssociatedGame.IsCheckmated(AssociatedGame.WhoseTurn) || AssociatedGame.KingIsGone(AssociatedGame.WhoseTurn);
            }
            else
            {
                correctMove = false;
            }
            if (correctMove)
            {
                Score.Score++;
            }
            return correctMove;
        }

        public void SetPosition(TrainingPosition position)
        {
            AssociatedGame = new AtomicChessGame(position.FEN);
            CurrentFen = position.FEN;
        }
    }
}
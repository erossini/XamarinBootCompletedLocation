using System;
namespace LocationTest.Droid
{
    public class UtcTimestamper
    {
        DateTime startTime;

        public UtcTimestamper()
        {
            startTime = DateTime.UtcNow;
        }

        public string GetFormattedTimestamp()
        {
            TimeSpan duration = DateTime.UtcNow.Subtract(startTime);
            return $"({duration:c} ago) Service started at {startTime}.";
        }
    }
}
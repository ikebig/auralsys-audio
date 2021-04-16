using System;

namespace Resony.Server
{
    public static class DateTimeExtensions
    {
        public static DateTime GetNextMinuteSchedule(this DateTime currentSchedule, int periodMinutes)
        {
            //var cronSchedule = NCrontab.CrontabSchedule.Parse($"*/{(int)_serverOptions.RecordingLengthMinutes} * * * *");
            //nextScheduledUtc = cronSchedule.GetNextOccurrence(currentScheduleUtc);
            var cronSchedule = NCrontab.CrontabSchedule.Parse($"*/{(int)periodMinutes} * * * *");
            var nextSchedule = cronSchedule.GetNextOccurrence(currentSchedule);
            return nextSchedule;
        }
    }
}

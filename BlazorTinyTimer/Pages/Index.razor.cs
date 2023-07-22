using Microsoft.JSInterop;
using System.Timers;

namespace BlazorTinyTimer.Pages;

public partial class Index
{
    #region Tiny timer
    const string DEFAULT_TIME = "00:00:00";
    string elapsedTime = DEFAULT_TIME;
    System.Timers.Timer timer = new System.Timers.Timer(1);
    DateTime startTime = DateTime.Now;
    bool isRunning = false;

    private void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        DateTime currentTime = e.SignalTime;
        elapsedTime = $"{currentTime.Subtract(startTime)}";
        StateHasChanged();
    }

    void StartTimer()
    {
        startTime = DateTime.Now;
        timer = new System.Timers.Timer(1);
        timer.Elapsed += OnTimedEvent;
        timer.AutoReset = true;
        timer.Enabled = true;
        isRunning = true;
    }

    void StopTimer()
    {
        isRunning = false;
        Console.WriteLine($"Elapsed Time: {elapsedTime}");
        timer.Enabled = false;
        elapsedTime = DEFAULT_TIME;
    }

    void OnTimerChanged()
    {
        if (!isRunning)
            StartTimer();
        else
            StopTimer();
    }
    #endregion

    #region Alarm
    private DateTime? alarmDateTime;
    private TimeSpan timeRemaining = TimeSpan.Zero;
    private System.Timers.Timer clockTimer;
    private System.Timers.Timer alarmTimer;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            clockTimer = new System.Timers.Timer(1000);
            clockTimer.Elapsed += UpdateClock;
            clockTimer.Start();
        }
    }

    private void SetAlarm()
    {
        DateTime now = DateTime.Now;
        DateTime alarmTime = new DateTime(now.Year, now.Month, now.Day, alarmDateTime?.Hour ?? 0, alarmDateTime?.Minute ?? 0, alarmDateTime?.Second ?? 0);
        if (alarmTime < now)
        {
            alarmTime = alarmTime.AddDays(1);
        }
        timeRemaining = alarmTime - now;

        alarmTimer?.Stop();
        alarmTimer = new System.Timers.Timer(timeRemaining.TotalMilliseconds);
        alarmTimer.Elapsed += AlarmElapsed;
        alarmTimer.Start();
    }

    private async void AlarmElapsed(object sender, ElapsedEventArgs e)
    {
        await JSRuntime.InvokeVoidAsync("alert", "¡La alarma está sonando!");
        timeRemaining = TimeSpan.Zero;
        alarmTimer.Stop();
    }

    private void UpdateClock(object sender, ElapsedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    #endregion
}

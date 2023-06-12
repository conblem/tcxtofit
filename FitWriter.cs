using Dynastream.Fit;
using Activity = tcxboi.TCX.Activity;

namespace tcxboi;

public class FitWriter: IDisposable
{
    private readonly Encode _encode;
    private readonly Stream _stream;

    public FitWriter(Stream stream)
    {
        _stream = stream;
        _encode = new Encode(ProtocolVersion.V20);
    }

    public void Write(Activity activity)
    {
        _encode.Open(_stream);
        
        WriteFileId(activity);
        WriteRecord(activity);
        WriteLap(activity);
        WriteSession(activity);
        WriteActivity(activity);
    }

    private void WriteFileId(Activity activity)
    {
        var fileIdMsg = new FileIdMesg(); // Every FIT file MUST contain a 'File ID' message as the first message
        
        fileIdMsg.SetType(Dynastream.Fit.File.Activity);
        fileIdMsg.SetManufacturer(0);
        fileIdMsg.SetProduct(0);
        fileIdMsg.SetTimeCreated(activity.StartTime.ToDynastream());
        fileIdMsg.SetSerialNumber(10000);
        
        _encode.Write(fileIdMsg);
    }

    private void WriteActivity(Activity activity)
    {
        var activityMsg = new ActivityMesg();
        
        activityMsg.SetTimestamp(activity.StopTime.ToDynastream());
        activityMsg.SetTotalTimerTime(activity.TotalTimeSeconds);
        activityMsg.SetNumSessions(1);
        activityMsg.SetType(Dynastream.Fit.Activity.Manual);
        activityMsg.SetEvent(Event.Activity);
        activityMsg.SetEventType(EventType.Stop);
        
        _encode.Write(activityMsg);
    }

    private void WriteRecord(Activity activity)
    {
        var records = activity.Trackpoints.Select(trackpoint =>
        {
            var record = new RecordMesg();

            record.SetTimestamp(trackpoint.Time.ToDynastream());
            record.SetPositionLat((int) (trackpoint.Position.Latitude * 11930465.1487));
            record.SetPositionLong((int) (trackpoint.Position.Longitude * 11930465.1487));
            record.SetAltitude(trackpoint.AltitudeMeters);
            record.SetHeartRate(trackpoint.BPM);
            record.SetCadence(trackpoint.RunCadence);
            record.SetDistance(trackpoint.DistanceMeters);
    
            return record;
        });
        
        _encode.Write(records);
    }
    

    private void WriteLap(Activity activity)
    {
        var lapMsg = new LapMesg();
        
        lapMsg.SetTimestamp(activity.StopTime.ToDynastream());
        lapMsg.SetMessageIndex(0);
        lapMsg.SetEvent(Event.Lap);
        lapMsg.SetEventType(EventType.Stop);
        lapMsg.SetStartTime(activity.StartTime.ToDynastream());
        lapMsg.SetLapTrigger(LapTrigger.SessionEnd);
        lapMsg.SetTotalDistance(activity.DistanceMeters);
        lapMsg.SetTotalElapsedTime(activity.TotalTimeSeconds);
        
        _encode.Write(lapMsg);
    }

    private void WriteSession(Activity activity)
    {
        var sessionMesg = new SessionMesg();
        
        sessionMesg.SetTimestamp(activity.StopTime.ToDynastream());
        sessionMesg.SetEvent(Event.Lap);
        sessionMesg.SetEventType(EventType.Stop);
        sessionMesg.SetStartTime(activity.StartTime.ToDynastream());
        sessionMesg.SetSport(Sport.Running);
        sessionMesg.SetSubSport(SubSport.Generic);
        sessionMesg.SetTotalElapsedTime(activity.TotalTimeSeconds);
        sessionMesg.SetTotalDistance(activity.DistanceMeters);
        sessionMesg.SetTrigger(SessionTrigger.ActivityEnd);

        var zones = new ZoneCalculator(activity).Calculate();
        for (var i = 0; i < zones.Length; i++)
        {
            sessionMesg.SetTimeInHrZone(i + 1, zones[i]);
        }
        
        _encode.Write(sessionMesg);
    }

    public void Dispose()
    {
        _encode.Close();
        _stream.Close();
    }
}
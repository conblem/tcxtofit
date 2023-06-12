using System.Xml.Linq;

namespace tcxboi.TCX;

public class Training
{
    private static readonly XNamespace Ns = XNamespace.Get("http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2");
    public IEnumerable<Activity> Activities { get; }
    public Training(XDocument document)
    {
        Activities = document
            .Element(Ns + "TrainingCenterDatabase")!
            .Element(Ns + "Activities")!
            .Elements(Ns + "Activity")
            .Select(element => Activity.Parse(element, Ns));
    }
    
}
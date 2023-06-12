using System.Xml.Linq;
using tcxboi;
using tcxboi.TCX;

await using var stream = new FileStream(Directory.GetCurrentDirectory() + "/input.tcx", FileMode.Open);
var document = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);

var training = new Training(document);
var activity = training.Activities.First();

var interpolatedTrackpoints = new TrackpointInterpolator(training.Activities.First()).Interpolate();
activity = activity.WithTrackpoints(interpolatedTrackpoints);

var fitDest = new FileStream("ExampleActivity.fit", FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
using var writer = new FitWriter(fitDest);
writer.Write(activity);
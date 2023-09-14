using SpeakingTimeTracker;

namespace Tests;

public class SegmentParserTests
{
    [TestCase("James Bond: Sorry. Next time I'll shoot the camera first", "James Bond")]
    [TestCase("M: I knew it was too early to promote you", "M")]
    [TestCase("Bond! BOND!", null)]
    public async Task Should_try_parse_speaker(string line, string speaker)
    {
        var segment = $"""
        00:00:01.450 --> 00:00:11.160
        {line}

        """;

        var result = await ZoomTranscriptParser.ParseSegment(segment.Split(Environment.NewLine).ToAsyncEnumerable().GetAsyncEnumerator());

        Assert.That(result.SpeakerName, Is.EqualTo(speaker));
    }
}
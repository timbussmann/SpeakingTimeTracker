namespace SpeakingTimeTracker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 1 || !args[0].EndsWith(".vtt"))
            {
                Console.WriteLine("Provide a file path to a Zoom WebVTT file as the first parameter.");
                return;
            }

            List<TranscriptSegment> segments = await ZoomTranscriptParser.ParseTranscript(File.ReadLinesAsync(args[0]));

            Console.WriteLine($"Total speaking time: {segments.Aggregate(TimeSpan.Zero, (totalDuration, segment) => totalDuration + segment.Duration)}");
            var speakingTimePerSpeaker = segments
                .GroupBy(s => s.SpeakerName)
                .Select(group => (
                    speaker: group.Key,
                    duration: group.Aggregate(TimeSpan.Zero, (speakerTime, segment) => speakerTime + segment.Duration)))
                .OrderByDescending(tuple => tuple.duration);
            foreach (var speaker in speakingTimePerSpeaker)
            {
                Console.WriteLine($"{speaker.speaker ?? "<unknown>"}: {speaker.duration}");
            }

        }
    }
}
namespace Helpers
{
    public static class TestDirectories
    {
        public static readonly string testDir = "..\\..\\..\\..\\TestDocuments\\";
        public static readonly string inputDir = testDir + "Input\\";
        public static readonly string outputDir = testDir + "Output\\";
        public static readonly string expectedOutputDir = testDir + "ExpectedOutput\\";
    }

    public static class Equality
    {
        public static bool StreamsAreEqual(Stream stream1, Stream stream2)
        {
            int b;
            while ((b = stream1.ReadByte()) == stream2.ReadByte())
                if (b == -1)
                    return true;
            return false;
        }

        public static bool FilesAreEqual(string path1, string path2)
        {
            using var stream1 = File.OpenRead(path1);
            using var stream2 = File.OpenRead(path2);
            return StreamsAreEqual(stream1, stream2);
        }
    }
}

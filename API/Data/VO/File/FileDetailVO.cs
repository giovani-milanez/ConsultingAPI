using System;

namespace API.Data.VO
{
    public class FileDetailVO
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public long Size { get; set; }
        public string Url { get; set; }
    }
}

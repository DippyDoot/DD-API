namespace Dippy.DDApi.DomainModels {
    public class ResourceFile {
        public long Id { get; set; }
        public string RelativePath { get; set; }
        public long Hash { get; set; }
        public long? SourceInfoId { get; set; }
    }
}

namespace Dippy.DDApi.DomainModels {
    public class SourceInfo {
        public long Id { get; set; }
        public long? DateTimeCreated { get; set; }
        public long? DateToPosted { get; set; }
        public long? DateTimeConverted { get; set; }
        public long? DateTimeAdded { get; set; }
        public long? DateTimeLastTimeEdited { get; set; }
        public string Url { get; set; }
        public string CustomInfo { get; set; }
        public long? PosterSourceGroupId { get; set; }
        public long? CreatorSourceGroupId { get; set; }
        public long? HostSourceGroupId { get; set; }
    }
}

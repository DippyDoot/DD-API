namespace Dippy.DDApi.DomainModels {
    public class SourceGroup {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public long? TagId { get; set; }
    }
}

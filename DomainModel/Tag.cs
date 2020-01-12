namespace Dippy.DDApi.DomainModels {
    public class Tag {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameCultureInfo { get; set; }
        public long TagTypeId { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public long? ResourceFileId { get; set; }
        public long? Rating { get; set; }
    }
}

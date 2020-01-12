namespace Dippy.DDApi.DomainModels {
    public class Gallery {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long GalleryTypeId { get; set; }
        public long ResourceFileId { get; set; }
        public long? Length { get; set; }
        public long? Rating { get; set; }
        public long? DateTimeRated { get; set; }
        public long? DateTimeEdited { get; set; }
        public long? DateTimeLastTagged { get; set; }
    }
}

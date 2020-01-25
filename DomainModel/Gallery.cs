using Dippy.DDApi.Attributes;

namespace Dippy.DDApi.DomainModels {
    public class Gallery {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long GalleryTypeId { get; set; }
        public long ResourceFileId { get; set; }
        public long? Length { get; set; }
        public long? Rating { get; set; }
        [ReadOnlyColumn]
        public long? DateTimeRated { get; set; }
        [ReadOnlyColumn]
        public long? DateTimeEdited { get; set; }
        [ReadOnlyColumn]
        public long? DateTimeLastTagged { get; set; }
    }
}

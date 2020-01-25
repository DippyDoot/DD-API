using Dippy.DDApi.Attributes;

namespace Dippy.DDApi.DomainModels {
    public class Collection {
        [Key(false, 0)]
        public long CollectionTagId { get; set; }
        [Key(false, 1)]
        public long GalleryId { get; set; }
        public long? Order { get; set; }
    }
}

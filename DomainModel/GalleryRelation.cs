using Dippy.DDApi.Attributes;

namespace Dippy.DDApi.DomainModels {
    public class GalleryRelation {
        [Key(false, 0)]
        public long FromGalleryId { get; set; }
        [Key(false, 1)]
        public long ToGalleryId { get; set; }
    }
}

using Dippy.DDApi.Attributes;

namespace Dippy.DDApi.DomainModels {
    public class GalleryTag {
        [Key(false, 0)]
        public long GalleryId { get; set; }
        [Key(false, 1)]
        public long TagId { get; set; }
        public long? TagGroupId { get; set; }
    }
}

using Dippy.DDApi.Attributes;

namespace Dippy.DDApi.DomainModels {
    public class FavoriteGalleries {
        [Key]
        public long GalleryId { get; set; }
        [ReadOnlyColumn]
        public long? DateTimeFavorited { get; set; }
    }
}

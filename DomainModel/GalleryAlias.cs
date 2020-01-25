using Dippy.DDApi.Attributes;

namespace Dippy.DDApi.DomainModels {
    public class GalleryAlias {
        [Key]
        public long Id { get; set; }
        public long GalleryId { get; set; }
        public string Name { get; set; }
        public string NameCultureInfo { get; set; }
    }
}

using Dippy.DDApi.Attributes;

namespace Dippy.DDApi.DomainModels {
    public class FavoriteTag {
        [Key]
        public long TagId { get; set; }
        [ReadOnlyColumn]
        public long? DateTimeFavorited { get; set; }
    }
}

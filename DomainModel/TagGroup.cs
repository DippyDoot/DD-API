using Dippy.DDApi.Attributes;

namespace Dippy.DDApi.DomainModels {
    public class TagGroup {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public long? TagTypeId { get; set; }
    }
}

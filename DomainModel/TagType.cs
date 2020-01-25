using Dippy.DDApi.Attributes;

namespace Dippy.DDApi.DomainModels {
    public class TagType {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
    }
}

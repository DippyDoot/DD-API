using Dippy.DDApi.Attributes;

namespace Dippy.DDApi.DomainModels {
    public class TagRelations {
        [Key(false, 0)]
        public long FromTagId { get; set; }
        [Key(false, 1)]
        public long ToTagId { get; set; }
    }
}

using Dippy.DDApi.Attributes;

namespace Dippy.DDApi.DomainModels {
    public class TagAlias {
        [Key]
        public long Id { get; set; }
        public long TagId { get; set; }
        public string Name { get; set; }
        public string NameCultureInfo { get; set; }
    }
}

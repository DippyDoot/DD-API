using System;
using System.Collections.Generic;
using System.Text;

namespace Dippy.DDApi.Entities {
    public class Tag : IEntity<Tag> {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameCultureInfo { get; set; }
        public IEntity<TagType> Type { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public IEntity<ResourceFile> ResourceFile { get; set; }
        public long? Rating { get; set; }
        public DateTime? FavoritedOn { get; set; }
        public IEnumerable<IEntity<TagAlias>> Aliases { get; set; }
    }
}

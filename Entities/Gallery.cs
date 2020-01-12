using System;
using System.Collections.Generic;
using System.Text;

namespace Dippy.DDApi.Entities {
    public class Gallery : IEntity<Gallery> {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameCultureInfo { get; set; }
        public string Description { get; set; }
        public IEntity<GalleryType> Type { get; set; }
        public IEntity<ResourceFile> ResourceFile { get; set; }
        public long? Length { get; set; }
        public long? Rating { get; set; }
        public DateTime? RatedOn { get; set; }
        public DateTime? LastEditedOn { get; set; }
        public DateTime? LastTaggedOn { get; set; }
        public DateTime? FavoritedOn { get; set; }
        public IEnumerable<(IEntity<Tag> tag, IEntity<TagGroup> group)> Tags { get; set; }
        public IEnumerable<IEntity<GalleryAlias>> Aliases { get; set; }
    }
}

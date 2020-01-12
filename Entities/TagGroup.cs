using System;
using System.Collections.Generic;
using System.Text;

namespace Dippy.DDApi.Entities {
    public class TagGroup : IEntity<TagGroup> {
        public long Id { get; set; }
        public string Name { get; set; }
        public IEntity<TagType> TagType { get; set; }
    }
}

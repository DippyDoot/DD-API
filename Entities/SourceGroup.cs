using System;
using System.Collections.Generic;
using System.Text;

namespace Dippy.DDApi.Entities {
    public class SourceGroup : IEntity<SourceGroup> {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public IEntity<Tag> AssociatedTag { get; set; }
    }
}

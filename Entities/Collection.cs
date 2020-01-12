using System;
using System.Collections.Generic;
using System.Text;

namespace Dippy.DDApi.Entities {
    public class Collection {
        public IEntity<Tag> CollectionTag { get; set; }
        public IEnumerable<(IEntity<Gallery> gallery, int? order)> Galleries { get; set; }
    }
}

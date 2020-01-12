using System;
using System.Collections.Generic;
using System.Text;

namespace Dippy.DDApi.Entities {
    public class GalleryAlias : IEntity<GalleryAlias> {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameCultureInfo { get; set; }
    }
}

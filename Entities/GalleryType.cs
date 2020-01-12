using System;
using System.Collections.Generic;
using System.Text;

namespace Dippy.DDApi.Entities {
    public class GalleryType : IEntity<GalleryType> {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}

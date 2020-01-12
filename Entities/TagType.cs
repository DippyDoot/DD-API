using System;
using System.Collections.Generic;
using System.Text;

namespace Dippy.DDApi.Entities {
    public class TagType : IEntity<TagType> {
        public long Id { get; set; }
        public string Name{ get; set; }
    }
}

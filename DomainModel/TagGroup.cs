using System;
using System.Collections.Generic;
using System.Text;

namespace Dippy.DDApi.DomainModels {
    public class TagGroup {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? TagTypeId { get; set; }
    }
}

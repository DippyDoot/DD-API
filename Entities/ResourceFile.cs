using System;
using System.Collections.Generic;
using System.Text;

namespace Dippy.DDApi.Entities {
    public class ResourceFile : IEntity<ResourceFile> {
        public long Id { get; set; }
        public long RelativePath { get; set; }
        public uint Hash { get; set; }
        public IEntity<SourceInfo> SourceInfo { get; set; }
    }
}

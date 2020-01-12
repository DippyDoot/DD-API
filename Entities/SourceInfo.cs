using System;
using System.Collections.Generic;
using System.Text;

namespace Dippy.DDApi.Entities {
    public class SourceInfo : IEntity<SourceInfo> {
        public long Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? PostedOn { get; set; }
        public DateTime? ConvertedOn { get; set; }
        public DateTime? AddedOn { get; set; }
        public DateTime? LastEditedOn { get; set; }
        public string Url { get; set; }
        public string CustomInfo { get; set; }
        public IEntity<SourceGroup> Poster { get; set; }
        public IEntity<SourceGroup> Creator { get; set; }
        public IEntity<SourceGroup> Host { get; set; }
    }
}

using System.Collections.Generic;

namespace CardMaker
{
    class TemplateRefList
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public List<TemplateRef> angles { get; set; }
    }
}

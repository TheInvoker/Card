namespace CardMaker
{
    public class TemplateRef
    {
        public string grid { get; set; }
        public string warp { get; set; }
        public string template { get; set; }
        public string result { get; set; }
        public string mapping { get; set; }
        public string transformer { get; set; }
        public string filter { get; set; }
        public string[] extra { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int w { get; set; }
        public int h { get; set; }
    }
}

using System;

namespace SldWorksLogic.Infrastructure
{
    public class Element
    {
        public string Path { get; set; }
        public int Count { get; set; }
        public ElementType ElementType { get; set; }

        public Element()
        {
            Path = string.Empty;
            Count = 0;
            ElementType = ElementType.Support;
        }

        public Element(string pathDir, int count, ElementType elementType)
        {
            if (pathDir == null) throw new ArgumentNullException("pathDir");
            if (count < 0) throw new ArgumentOutOfRangeException("count");

            Path = pathDir;
            Count = count;
            ElementType = elementType;
        }
    }
}
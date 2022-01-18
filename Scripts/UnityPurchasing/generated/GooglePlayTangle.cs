// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("HX6ZTN0mktwxTLpVdh0Qy9nFmlyvZQvx3sMQgtPczPIVPo+e6StTu0myHrlr6ncK3DEyBeEDEDZGFhv2KJoZOigVHhEynlCe7xUZGRkdGBvvcksI4e48WC9fqKkJv1TuPFcPIlQZzHU+5yVvIcUu0D6tY7JUWsRGyX5GMlxYJvYUpT7/J20CdDSLtOj8B2Lz7cuzQlCyRBNwYRam2Bk5Bt45BmOHHfXC9zR2+vacoi111CzqIc7kfl9Q0+bkGiFkAY2tqPjGzuP8DfemOREegnscn/iBRnfL/aB8sz4N6LmyOpkyea8+uiWGeC4brMY9TakpUOO7nXDKUOwtjg2HGFSUAlSaGRcYKJoZEhqaGRkYsLg/tBJ5BemEQEa3MRwLjxobGRgZ");
        private static int[] order = new int[] { 13,6,12,13,11,10,10,7,12,9,11,12,13,13,14 };
        private static int key = 24;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}

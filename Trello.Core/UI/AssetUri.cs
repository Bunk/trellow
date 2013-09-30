using System;

namespace Trellow.UI
{
    public class AssetUri : Uri
    {
        public AssetUri(string path)
            : base("/Assets/" + path, UriKind.Relative) { }
    }
}

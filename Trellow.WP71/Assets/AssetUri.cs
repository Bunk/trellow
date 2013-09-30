using System;

namespace trello.Assets
{
    public class AssetUri : Uri
    {
        public AssetUri(string path)
            : base("/Assets/" + path, UriKind.Relative) { }
    }
}

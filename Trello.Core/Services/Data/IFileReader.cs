using System;
using System.Collections.Generic;
using Strilanc.Value;

namespace Trellow.Services.Data
{
    public interface IFileReader
    {
        May<T> Read<T>(Uri uri);

        IList<T> ReadList<T>(Uri uri);
    }
}
using System;
using System.Collections.Generic;

namespace trello.Services
{
    public interface IBoardService
    {
        IObservable<IEnumerable<Board>> Mine();
    }
}
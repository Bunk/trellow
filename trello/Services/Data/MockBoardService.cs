using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using trello.Services.Models;
using trello.ViewModels;

namespace trello.Services.Data
{
    public class MockBoardService : MockServiceBase, IBoardService
    {
        public MockBoardService(IProgressService progressService) : base(progressService)
        {
        }

        public Task<IEnumerable<Board>> Mine()
        {
            IEnumerable<Board> boards = new List<Board>
            {
                new Board
                {
                    Id = "BOARD-1",
                    Name = "Work @ Home",
                    Desc = "This is a simple description for this board.",
                    Prefs = new Preferences {PermissionLevel = "private"}
                },
                new Board
                {
                    Id = "BOARD-2",
                    Name = "Trellow",
                    Desc = "This is a short description.",
                    Prefs = new Preferences {PermissionLevel = "private"}
                },
                new Board
                {
                    Id = "BOARD-3",
                    Name = "Cardboard",
                    Desc = "This should be a really, really, really, really long description for this board.",
                    Prefs = new Preferences {PermissionLevel = "public"}
                }
            };
            return Run(() => boards);
        }

        public Task<Board> WithId(string id)
        {
            var board = new Board
            {
                Id = id,
                Name = "Mock Board",
                Desc = "This is a temporary mock board that will always be the same no matter what you do.",
                Prefs = new Preferences {PermissionLevel = "public"},
                Lists = new List<List>
                {
                    new List
                    {
                        Id = "LIST-1",
                        Name = "To Do",
                        Pos = 0
                    },
                    new List
                    {
                        Id = "LIST-2",
                        Name = "Doing",
                        Pos = 1
                    },
                    new List
                    {
                        Id = "LIST-3",
                        Name = "Done",
                        Pos = 2
                    }
                }
            };
            return Run(() => board);
        }
    }
}
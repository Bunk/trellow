using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using trello.Services.Models;
using trello.ViewModels;

namespace trello.Services
{
    public class BoardService : ServiceBase, IBoardService
    {
        public BoardService(IRequestProcessor processor) : base(processor)
        {
        }

        public async Task<IEnumerable<Board>> Mine()
        {
            return await Processor.Execute<List<Board>>(
                Request("members/my/boards")
                    .AddParameter("filter", "open"));
        }
    }

    public abstract class MockServiceBase
    {
        private readonly IProgressService _progressService;

        public MockServiceBase(IProgressService progressService)
        {
            _progressService = progressService;
        }

        public async Task<T> Execute<T>(Func<Task<T>> action)
        {
            _progressService.Show();

            var results = await action();

            _progressService.Hide();

            return results;
        }
    }

    public class MockBoardService : MockServiceBase, IBoardService
    {
        public MockBoardService(IProgressService progressService) : base(progressService)
        {
        }

        public Task<IEnumerable<Board>> Mine()
        {
            return Execute(() => Task.Run(() =>
            {
                var boards = new List<Board>
                {
                    new Board
                    {
                        Name = "Work @ Home",
                        Desc = "This is a simple description for this board.",
                        Prefs = new Preferences {PermissionLevel = "private"}
                    },
                    new Board
                    {
                        Name = "Trellow",
                        Desc = "This is a short description.",
                        Prefs = new Preferences {PermissionLevel = "private"}
                    },
                    new Board
                    {
                        Name = "Cardboard",
                        Desc = "This should be a really, really, really, really long description for this board.",
                        Prefs = new Preferences {PermissionLevel = "public"}
                    },
                    new Board
                    {
                        Name = "Cardboard",
                        Desc = "This should be a really, really, really, really long description for this board.",
                        Prefs = new Preferences {PermissionLevel = "public"}
                    },
                    new Board
                    {
                        Name = "Cardboard",
                        Desc = "This should be a really, really, really, really long description for this board.",
                        Prefs = new Preferences {PermissionLevel = "public"}
                    },
                    new Board
                    {
                        Name = "Cardboard",
                        Desc = "This should be a really, really, really, really long description for this board.",
                        Prefs = new Preferences {PermissionLevel = "public"}
                    },
                    new Board
                    {
                        Name = "Cardboard",
                        Desc = "This should be a really, really, really, really long description for this board.",
                        Prefs = new Preferences {PermissionLevel = "public"}
                    },
                    new Board
                    {
                        Name = "Cardboard",
                        Desc = "This should be a really, really, really, really long description for this board.",
                        Prefs = new Preferences {PermissionLevel = "public"}
                    }
                };

                Thread.Sleep(4000);

                return (IEnumerable<Board>) boards;
            }));
        }
    }
}
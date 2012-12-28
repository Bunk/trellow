using System.Diagnostics;
using System.Linq;
using Microsoft.Phone.Testing;
using trello.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tests.integration.Tests.Services
{
    [TestClass]
    public class BoardServiceTests : WorkItemTest
    {
        [TestMethod, Asynchronous, Tag("Integration")]
        public async void can_authenticate_with_trello()
        {
            var factory = new TrelloOAuthClient();
            var loginUrl = (await factory.GetLoginUri()).ToString();

            Debugger.Break();

            var verifier = "PASTE_ME";
            var token = await factory.GetAccessToken(verifier);

            var repo = new BoardService(factory);

            var boards = await repo.Mine();

            Assert.IsNotNull(boards);
            Assert.IsTrue(boards.Any());

            EnqueueTestComplete();
        }
    }
}

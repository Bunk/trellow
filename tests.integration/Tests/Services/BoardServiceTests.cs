using System.Diagnostics;
using System.Linq;
using Microsoft.Phone.Testing;
using trello.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using trello.Services.Data;
using trello.Services.OAuth;

namespace tests.integration.Tests.Services
{
    [TestClass]
    public class BoardServiceTests : WorkItemTest
    {
//        [TestMethod, Asynchronous, Tag("Integration")]
//        public async void can_authenticate_with_trello()
//        {
//            // todo: Mock the settings object(s)
//            var factory = new TrelloOAuthClient();
//            var loginUrl = (await factory.GetLoginUri()).ToString();
//
//            Debugger.Break();
//
//            var verifier = "PASTE_ME";
//            var token = await factory.GetAccessToken(verifier);
//            var processor = new RequestProcessor(factory);
//            var repo = new BoardService(processor);
//
//            var boards = await repo.Mine();
//
//            Assert.IsNotNull(boards);
//            Assert.IsTrue(boards.Any());
//
//            EnqueueTestComplete();
//        }
    }
}

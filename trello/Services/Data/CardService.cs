using System.Collections.Generic;
using System.Threading.Tasks;
using trello.Services.Models;

namespace trello.Services.Data
{
    public class CardService : ServiceBase, ICardService
    {
        public CardService(IRequestProcessor processor) : base(processor)
        {
        }

        public async Task<IEnumerable<Card>> Mine()
        {
            return await Processor.Execute<List<Card>>(
                Request("members/my/cards/open")
                    .AddParameter("members", "false"));
        }
    }
}
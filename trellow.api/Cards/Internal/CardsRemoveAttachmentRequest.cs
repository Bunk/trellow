using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Cards.Internal
{
	internal class CardsRemoveAttachmentRequest : CardsRequest
	{
		public CardsRemoveAttachmentRequest(ICardId card, IAttachmentId attachment)
			: base(card, "attachments/{idAttachment}", Method.DELETE)
		{
			Guard.NotNull(attachment, "attachment");
			AddParameter("idAttachment", attachment.GetAttachmentId(), ParameterType.UrlSegment);
		}
	}
}
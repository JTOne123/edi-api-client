﻿using KonturEdi.Api.Types.BoxEvents;
using KonturEdi.Api.Types.Messages.BoxEventsContents.Outbox;

namespace KonturEdi.Api.Types.Messages.BoxEventsContents.Inbox
{
    public class MessageDiadocRevocationAcceptedForBuyerEventContent : IBoxEventContent, IMessageDiadocEventContent
    {
        public BasicMessageMeta InboxMessageMeta { get; set; }
        public AcceptedRevocationInfo AcceptedRevocationInfo { get; set; }

        public string DiadocBoxId { get; set; }
        public string InvoiceId { get; set; }
        public string MessageId { get; set; }
        public string Torg12Id { get; set; }
        public string InvoiceCorrectionId { get; set; }

        public DiadocUrls DiadocUrls { get; set; }
    }
}
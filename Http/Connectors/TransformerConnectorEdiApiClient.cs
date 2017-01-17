﻿using System;
using System.Globalization;
using System.Net;

using JetBrains.Annotations;

using KonturEdi.Api.Client.Http.Helpers;
using KonturEdi.Api.Types.BoxEvents;
using KonturEdi.Api.Types.Common;
using KonturEdi.Api.Types.Connectors;
using KonturEdi.Api.Types.Connectors.Transformer;
using KonturEdi.Api.Types.Serialization;

namespace KonturEdi.Api.Client.Http.Connectors
{
    public class TransformerConnectorEdiApiClient : BaseEdiApiHttpClient, ITransformerConnectorEdiApiClient
    {
        public TransformerConnectorEdiApiClient(string apiClientId, Uri baseUri, int timeoutInMilliseconds = DefaultTimeout, IWebProxy proxy = null)
            : this(apiClientId, baseUri, new JsonEdiApiTypesSerializer(), timeoutInMilliseconds, proxy)
        {
        }

        public TransformerConnectorEdiApiClient(string apiClientId, Uri baseUri, IEdiApiTypesSerializer serializer, int timeoutInMilliseconds = DefaultTimeout, IWebProxy proxy = null)
            : base(apiClientId, baseUri, serializer, timeoutInMilliseconds, proxy)
        {
        }

        public void TransformationStarted([NotNull] string authToken, [NotNull] string connectorBoxId, [NotNull] string connectorInteractionId)
        {
            var url = new UrlBuilder(BaseUri, relativeUrl + "TransformationStarted")
                .AddParameter(boxIdUrlParameterName, connectorBoxId)
                .AddParameter(connectorInteractionIdUrlParameterName, connectorInteractionId)
                .ToUri();
            MakePostRequest(url, authToken, content : null);
        }

        public void TransformationPaused([NotNull] string authToken, [NotNull] string connectorBoxId, [NotNull] string connectorInteractionId, [CanBeNull] string reason)
        {
            var url = new UrlBuilder(BaseUri, relativeUrl + "TransformationPaused")
                .AddParameter(boxIdUrlParameterName, connectorBoxId)
                .AddParameter(connectorInteractionIdUrlParameterName, connectorInteractionId)
                .ToUri();
            MakePostRequest(url, authToken, reason);
        }

        public void TransformationResumed([NotNull] string authToken, [NotNull] string connectorBoxId, [NotNull] string connectorInteractionId)
        {
            var url = new UrlBuilder(BaseUri, relativeUrl + "TransformationResumed")
                .AddParameter(boxIdUrlParameterName, connectorBoxId)
                .AddParameter(connectorInteractionIdUrlParameterName, connectorInteractionId)
                .ToUri();
            MakePostRequest(url, authToken, content : null);
        }

        [NotNull]
        public MessageMeta TransformedSuccessfully([NotNull] string authToken, [NotNull] string connectorBoxId, [NotNull] string connectorInteractionId, [NotNull] MessageData resultMessageData)
        {
            var url = new UrlBuilder(BaseUri, relativeUrl + "TransformedSuccessfully")
                .AddParameter(boxIdUrlParameterName, connectorBoxId)
                .AddParameter(connectorInteractionIdUrlParameterName, connectorInteractionId)
                .AddParameter("messageFileName", resultMessageData.MessageFileName)
                .ToUri();
            return MakePostRequest<MessageMeta>(url, authToken, resultMessageData.MessageBody);
        }

        public void TransformedUnsuccessfully([NotNull] string authToken, [NotNull] string connectorBoxId, [NotNull] string connectorInteractionId, [CanBeNull] string[] errors)
        {
            var url = new UrlBuilder(BaseUri, relativeUrl + "TransformedUnsuccessfully")
                .AddParameter(boxIdUrlParameterName, connectorBoxId)
                .AddParameter(connectorInteractionIdUrlParameterName, connectorInteractionId)
                .ToUri();
            MakePostRequest(url, authToken, errors);
        }

        public void StopProcessing([NotNull] string authToken, [NotNull] string connectorBoxId, [NotNull] string connectorInteractionId, [NotNull] ServiceMessageData serviceMessageData)
        {
            var url = new UrlBuilder(BaseUri, relativeUrl + "StopProcessing")
                .AddParameter(boxIdUrlParameterName, connectorBoxId)
                .AddParameter(connectorInteractionIdUrlParameterName, connectorInteractionId)
                .AddParameter("messageId", serviceMessageData.MessageId)
                .AddParameter("messageDetails", serviceMessageData.MessageId)
                .AddParameter("recipientGln", serviceMessageData.RecipientGln)
                .ToUri();
            MakePostRequest(url, authToken, serviceMessageData.MessageBody);
        }

        [NotNull]
        public TransformerConnectorBoxEventBatch GetEvents([NotNull] string authToken, [NotNull] string connectorBoxId, [CanBeNull] string exclusiveEventId, uint? count = null)
        {
            var url = new UrlBuilder(BaseUri, relativeUrl + "GetEvents")
                .AddParameter(boxIdUrlParameterName, connectorBoxId)
                .AddParameter("exclusiveEventId", exclusiveEventId);
            if(count.HasValue)
                url.AddParameter("count", count.Value.ToString(CultureInfo.InvariantCulture));
            return GetEvents(authToken, url);
        }

        [NotNull]
        public TransformerConnectorBoxEventBatch GetEvents([NotNull] string authToken, [NotNull] string connectorBoxId, DateTime fromDateTime, uint? count = null)
        {
            var url = new UrlBuilder(BaseUri, relativeUrl + "GetEventsFrom")
                .AddParameter(boxIdUrlParameterName, connectorBoxId)
                .AddParameter("fromDateTime", DateTimeUtils.ToString(fromDateTime));
            if(count.HasValue)
                url.AddParameter("count", count.Value.ToString(CultureInfo.InvariantCulture));
            return GetEvents(authToken, url);
        }

        [NotNull]
        private TransformerConnectorBoxEventBatch GetEvents([NotNull] string authToken, [NotNull] UrlBuilder url)
        {
            var boxEventBatch = MakeGetRequest<TransformerConnectorBoxEventBatch>(url.ToUri(), authToken);
            boxEventBatch.Events = boxEventBatch.Events ?? new TransformerConnectorBoxEvent[0];
            foreach(var boxEvent in boxEventBatch.Events)
            {
                boxEvent.EventContent =
                    boxEventTypeRegistry.IsSupportedEventType(boxEvent.EventType)
                        ? Serializer.NormalizeDeserializedObjectToType(boxEvent.EventContent, boxEventTypeRegistry.GetEventContentType(boxEvent.EventType))
                        : null;
            }
            return boxEventBatch;
        }

        [NotNull]
        public MessageEntity GetMessage([NotNull] string authToken, [NotNull] string connectorBoxId, [NotNull] string messageId)
        {
            var url = new UrlBuilder(BaseUri, relativeUrl + "GetMessage")
                .AddParameter(boxIdUrlParameterName, connectorBoxId)
                .AddParameter("messageId", messageId)
                .ToUri();
            return MakeGetRequest<MessageEntity>(url, authToken);
        }

        [NotNull]
        public ConnectorBoxesInfo GetConnectorBoxesInfo([NotNull] string authToken)
        {
            var url = new UrlBuilder(BaseUri, "V1/Boxes/GetConnectorBoxesInfo").ToUri();
            return MakeGetRequest<ConnectorBoxesInfo>(url, authToken);
        }

        private const string relativeUrl = "V1/Connectors/Transformers/";
        private const string boxIdUrlParameterName = "connectorBoxId";
        private const string connectorInteractionIdUrlParameterName = "connectorInteractionId";
        private readonly IBoxEventTypeRegistry<TransformerConnectorBoxEventType> boxEventTypeRegistry = new TransformerConnectorBoxEventTypeRegistry();
    }
}
﻿using Lime.Protocol;
using System;
using System.Threading;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client.Host;
using Takenet.MessagingHub.Client.Sender;

namespace Takenet.MessagingHub.Client.Extensions.Tunnel
{
    public class TunnelExtension : ITunnelExtension
    {
        public readonly Node TunnelAddress;

        private readonly IMessagingHubSender _sender;

        public TunnelExtension(IMessagingHubSender sender, Application application)
        {
            _sender = sender;
            var domain = string.IsNullOrWhiteSpace(application.Domain) ? Constants.DEFAULT_DOMAIN : application.Domain;
            TunnelAddress = Node.Parse($"postmaster@tunnel.{domain}");
        }

        public Task<Node> ForwardMessageAsync(Message message, Identity destination, CancellationToken cancellationToken)
            => ForwardAsync(message, destination, _sender.SendMessageAsync, cancellationToken);

        public Task<Node> ForwardNotificationAsync(Notification notification, Identity destination, CancellationToken cancellationToken)
            => ForwardAsync(notification, destination, _sender.SendNotificationAsync, cancellationToken);

        private async Task<Node> ForwardAsync<TEnvelope>(TEnvelope envelope, Identity destination, Func<TEnvelope, CancellationToken, Task> senderFunc, CancellationToken cancellationToken)
            where TEnvelope : Envelope, new()
        {
            if (envelope == null) throw new ArgumentNullException(nameof(envelope));
            if (envelope.From == null) throw new ArgumentException("The envelope 'from' value must be provided", nameof(envelope));
            if (destination == null) throw new ArgumentNullException(nameof(destination));

            var tunnelAddress = new Node(
                Uri.EscapeDataString(destination.ToString()),
                TunnelAddress.Domain,
                Uri.EscapeDataString(envelope.From.ToString()));

            var tunnelEnvelope = envelope.ShallowCopy();
            tunnelEnvelope.From = null;
            tunnelEnvelope.To = tunnelAddress;

            await senderFunc(tunnelEnvelope, cancellationToken);
            return tunnelAddress;
        }
    }
}

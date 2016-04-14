﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;

namespace Playground
{
    /// <summary>
    /// Example of a notification receiver
    /// </summary>
    public class PrintNotificationReceiver : INotificationReceiver
    {
        public Task ReceiveAsync(Notification notification, IMessagingHubSender sender, CancellationToken cancellationToken)
        {
            Console.WriteLine("Notification of {0} event received. Reason: {1}", notification.Event, notification.Reason);
            return Task.FromResult(0);
        }
    }
}

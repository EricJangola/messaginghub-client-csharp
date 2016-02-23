﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Messaging.Resources;
using Lime.Protocol;
using Lime.Protocol.Security;
using Takenet.MessagingHub.Client.LimeProtocol;
using Takenet.MessagingHub.Client.Receivers;
using Lime.Protocol.Listeners;

namespace Takenet.MessagingHub.Client
{
    /// <summary>
    ///     Default implementation for <see cref="IMessagingHubClient" /> class.
    /// </summary>
    public class MessagingHubClient : IMessagingHubClient
    {
        private readonly Identity _identity;
        private readonly Authentication _authentication;
        private readonly Uri _endpoint;
        private readonly IPersistentLimeSessionFactory _persistentClientFactory;
        private readonly IClientChannelFactory _clientChannelFactory;
        private readonly ICommandProcessorFactory _commandProcessorFactory;
        private readonly ILimeSessionProvider _limeSessionProvider;
        private readonly SemaphoreSlim _semaphore;
        private readonly TimeSpan _sendTimeout;
        private readonly EnvelopeListenerRegistrar _listenerRegistrar;

        private ICommandProcessor _commandProcessor;
        private IPersistentLimeSession _persistentLimeSession;
        private ChannelListener _channelListener;

        internal MessagingHubClient(Identity identity, Authentication authentication, Uri endPoint, TimeSpan sendTimeout,
            IPersistentLimeSessionFactory persistentChannelFactory, IClientChannelFactory clientChannelFactory,
            ICommandProcessorFactory commandProcessorFactory, ILimeSessionProvider limeSessionProvider, EnvelopeListenerRegistrar listenerRegistrar)
        {
            _identity = identity;
            _authentication = authentication;
            _endpoint = endPoint;
            _persistentClientFactory = persistentChannelFactory;
            _clientChannelFactory = clientChannelFactory;
            _commandProcessorFactory = commandProcessorFactory;
            _limeSessionProvider = limeSessionProvider;
            _sendTimeout = sendTimeout;
            _semaphore = new SemaphoreSlim(1);
            _listenerRegistrar = listenerRegistrar;
        }

        internal MessagingHubClient(Identity identity, Authentication authentication, Uri endPoint, TimeSpan sendTimeout, EnvelopeListenerRegistrar listenerRegistrar)
            : this(identity, authentication, endPoint, sendTimeout)
        {
            _listenerRegistrar = listenerRegistrar;
        }

        public MessagingHubClient(Identity identity, Authentication authentication, Uri endPoint) :
            this(identity, authentication, endPoint, TimeSpan.FromSeconds(20))
        { }

        public MessagingHubClient(Identity identity, Authentication authentication, Uri endPoint, TimeSpan sendTimeout) :
            this(identity, authentication, endPoint, sendTimeout, new PersistentLimeSessionFactory(), new ClientChannelFactory(),
                new CommandProcessorFactory(), new LimeSessionProvider(), new EnvelopeListenerRegistrar())
        { }

        public bool Started { get; private set; }

        public virtual async Task<Command> SendCommandAsync(Command command)
        {
            if (!Started)
                throw new InvalidOperationException("Client must be started before to proceed with this operation");

            return await _commandProcessor.SendAsync(command, _sendTimeout).ConfigureAwait(false);
        }

        public virtual async Task SendMessageAsync(Message message)
        {
            if (!Started)
                throw new InvalidOperationException("Client must be started before to proceed with this operation");

            await _persistentLimeSession.SendMessageAsync(message).ConfigureAwait(false);
        }

        public virtual async Task SendNotificationAsync(Notification notification)
        {
            if (!Started)
                throw new InvalidOperationException("Client must be started before to proceed with this operation!");

            await _persistentLimeSession.SendNotificationAsync(notification).ConfigureAwait(false);
        }

        public virtual Task<Message> ReceiveMessageAsync(CancellationToken cancellationToken)
        {
            return _persistentLimeSession.ReceiveMessageAsync(cancellationToken);
        }

        public virtual Task<Notification> ReceiveNotificationAsync(CancellationToken cancellationToken)
        {
            return _persistentLimeSession.ReceiveNotificationAsync(cancellationToken);
        }

        public virtual async Task StartAsync()
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                if (Started) throw new InvalidOperationException("The client is already started");

                await InstantiateClientChannelAsync().ConfigureAwait(false);
                await _persistentLimeSession.StartAsync().ConfigureAwait(false);

                StartEnvelopeProcessors();

                Started = true;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public virtual async Task StopAsync()
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                if (!Started) throw new InvalidOperationException("The client is not started");

                await _commandProcessor.StopReceivingAsync().ConfigureAwait(false);
                _channelListener.Stop();
                await Task.WhenAll(_channelListener.NotificationListenerTask, _channelListener.MessageListenerTask, _channelListener.NotificationListenerTask);
                _persistentLimeSession.SessionEstablished -= OnSessionEstabilished;
                await _persistentLimeSession.StopAsync().ConfigureAwait(false);
                Started = false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void StartEnvelopeProcessors()
        {
            _commandProcessor = _commandProcessorFactory.Create(_persistentLimeSession);
            _commandProcessor.StartReceiving();

            var handler = new EnvelopeReceivedHandler(this, _listenerRegistrar);
            _channelListener = new ChannelListener(
                handler.Handle,
                handler.Handle,
                c => true.AsCompletedTask());
            _channelListener.Start(_persistentLimeSession.ClientChannel);
        }


        private async Task InstantiateClientChannelAsync()
        {
            _persistentLimeSession =
                await
                    _persistentClientFactory.CreatePersistentClientChannelAsync(_endpoint, _sendTimeout, _identity,
                        _authentication, _clientChannelFactory, _limeSessionProvider)
                            .ConfigureAwait(false);

            _persistentLimeSession.SessionEstablished += OnSessionEstabilished;
        }

        private async void OnSessionEstabilished(object sender, EventArgs e)
        {
            await SetPresenceAsync().ConfigureAwait(false);
        }

        private async Task SetPresenceAsync()
        {
            using (var cancellationToken = new CancellationTokenSource(_sendTimeout))
            {
                await _persistentLimeSession.SetResourceAsync(
                    LimeUri.Parse(UriTemplates.PRESENCE),
                    new Presence { Status = PresenceStatus.Available, RoutingRule = RoutingRule.Identity },
                    cancellationToken.Token)
                    .ConfigureAwait(false);
            }
        }
    }
}

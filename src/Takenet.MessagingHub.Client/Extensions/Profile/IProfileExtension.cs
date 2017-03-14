﻿using Lime.Messaging.Contents;
using Lime.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Takenet.MessagingHub.Client.Extensions.Profile
{
    /// <summary>
    /// Allows the management of the application's public profile.
    /// </summary>
    public interface IProfileExtension
    {
        /// <summary>
        /// Gets the current persistent menu.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DocumentSelect> GetPersistentMenuAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Sets the persistent menu.
        /// </summary>
        /// <param name="persistentMenu"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SetPersistentMenuAsync(DocumentSelect persistentMenu, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the current get started document.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Document> GetGetStartedAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Sets the get started document.
        /// </summary>
        /// <param name="getStarted"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Document> SetGetStartedAsync(Document getStarted, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the current greeting text.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<PlainText> GetGreetingAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Sets the greeting text.
        /// </summary>
        /// <param name="greeting"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SetGreetingAsync(PlainText greeting, CancellationToken cancellationToken);
    }
}

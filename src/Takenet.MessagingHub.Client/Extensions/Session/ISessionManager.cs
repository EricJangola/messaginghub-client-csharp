﻿using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;

namespace Takenet.MessagingHub.Client.Extensions.Session
{
    /// <summary>
    /// Defines a session management service.
    /// </summary>
    public interface ISessionManager
    {
        /// <summary>
        /// Gets an existing session for the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<NavigationSession> GetSessionAsync(Node node, CancellationToken cancellationToken);

        /// <summary>
        /// Clears an existing session for a node, removing all associated variable and states.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task ClearSessionAsync(Node node, CancellationToken cancellationToken);

        /// <summary>
        /// Adds an variable to a node session. If the session doesn't exists, it will be created.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task AddVariableAsync(Node node, string key, string value, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an existing variable from a node session.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="key">The key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<string> GetVariableAsync(Node node, string key, CancellationToken cancellationToken);

        /// <summary>
        /// Removes an existing variable from a node session.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="key">The key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task RemoveVariableAsync(Node node, string key, CancellationToken cancellationToken);

        /// <summary>
        /// Adds an state to a node session. If the session doesn't exists, it will be created.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="state">The state.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task AddStateAsync(Node node, string state, CancellationToken cancellationToken);

        /// <summary>
        /// Determines whether a state exists in a node session.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="state">The state.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> HasStateAsync(Node node, string state, CancellationToken cancellationToken);

        /// <summary>
        /// Removes an existing state from a node session.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="state">The state.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task RemoveStateAsync(Node node, string state, CancellationToken cancellationToken);
    }
}

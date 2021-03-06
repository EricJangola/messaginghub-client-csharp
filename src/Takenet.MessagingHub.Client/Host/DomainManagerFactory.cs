﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Lime.Protocol.Serialization;
using Newtonsoft.Json;
using Takenet.MessagingHub.Client.Extensions;
using Takenet.MessagingHub.Client.Extensions.Broadcast;
using Takenet.MessagingHub.Client.Extensions.Bucket;
using Takenet.MessagingHub.Client.Extensions.Delegation;
using Takenet.MessagingHub.Client.Extensions.Session;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;
using System.Security.Permissions;

namespace Takenet.MessagingHub.Client.Host
{
    public class DomainManagerFactory : IDomainManagerFactory
    {
        public IDomainManager Create(string domainName, string assemblyPath)
        {
            var domaininfo = new AppDomainSetup
            {
                ApplicationBase = assemblyPath,
                ApplicationName = domainName,
                PrivateBinPath = assemblyPath
            };

            var evidence = AppDomain.CurrentDomain.Evidence;
            var permission = AppDomain.CurrentDomain.PermissionSet;
            var appDomain = AppDomain.CreateDomain(domainName, evidence, domaininfo, permission);
            var domainManager = appDomain.CreateInstanceFromAndUnwrap(Path.Combine(assemblyPath, "Takenet.MessagingHub.Client.dll"), "Takenet.MessagingHub.Client.Host.DomainManager") as IDomainManager;
            return domainManager;
        }
    }
}

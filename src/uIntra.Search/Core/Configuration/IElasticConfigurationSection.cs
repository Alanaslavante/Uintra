﻿namespace uIntra.Search.Core.Configuration
{
    public interface IElasticConfigurationSection
    {
        string Url { get; }
        int LimitBulkOperation { get; }
        int NumberOfShards { get; }
        int NumberOfReplicas { get; }
        string IndexPrefix { get; }
    }
}
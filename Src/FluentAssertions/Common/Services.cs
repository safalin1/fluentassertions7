﻿using System;
using FluentAssertions.Execution;

namespace FluentAssertions.Common;

/// <summary>
/// Maintains the framework-specific services.
/// </summary>
public static class Services
{
    private static readonly object Lockable = new();
    private static Configuration configuration;

    static Services()
    {
        ResetToDefaults();
    }

    public static IConfigurationStore ConfigurationStore { get; set; }

    public static Configuration Configuration
    {
        get
        {
            lock (Lockable)
            {
                return configuration ??= new Configuration(ConfigurationStore);
            }
        }
    }

    public static Action<string> ThrowException { get; set; }

    public static IReflector Reflector { get; set; }

    public static void ResetToDefaults()
    {
        Reflector = new FullFrameworkReflector();
#if NETFRAMEWORK || NET6_0_OR_GREATER
        ConfigurationStore = new ConfigurationStoreExceptionInterceptor(new AppSettingsConfigurationStore());
#else
        ConfigurationStore = new NullConfigurationStore();
#endif
        ThrowException = new TestFrameworkProvider(Configuration).Throw;
    }
}

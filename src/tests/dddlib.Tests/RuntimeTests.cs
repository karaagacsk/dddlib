﻿// <copyright file="RuntimeTests.cs" company="dddlib contributors">
//  Copyright (c) dddlib contributors. All rights reserved.
// </copyright>

namespace dddlib.Tests.Acceptance
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using dddlib.Runtime;
    using dddlib.Sdk.Configuration.Model;
    using Xbehave;

    public partial class RuntimeTests
    {
        [Background]
        public void Background()
        {
            "Given a new ambient application"
                .Given(context => new Application().Using(context));
        }

        [Scenario]
        [Example(typeof(NaturalKeyTest))]
        [Example(typeof(NaturalKeySerializerTest))]
        public void AggregateRootTests(Type type)
        {
            var aggregateRootType = default(AggregateRootType);

            "Given a type of {0}"
                .Given(() => { });

            "When the ambient application is used to get the aggregate root type"
                .When(() => aggregateRootType = (AggregateRootType)Application.Current.GetAggregateRootType(type));

            "Then {0} should be represented by the aggregate root type"
                .Then(() => type.ShouldBeRepresentedBy(aggregateRootType));
        }

        [Scenario]
        [Example(typeof(NaturalKeyTest))]
        public void EntityTests(Type type)
        {
            var entityType = default(EntityType);

            "Given a type of {0}"
                .Given(() => { });

            "When the ambient application is used to get the entity type"
                .When(() => entityType = (EntityType)Application.Current.GetEntityType(type));

            "Then {0} should be represented by the entity type"
                .Then(() => type.ShouldBeRepresentedBy(entityType));
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Hmm.")]
    internal static class TypeAssertions
    {
        // NOTE (Cameron): This invokes a static method in this class with the name 'Assert' followed by the name of the aggregate under test.
        public static void ShouldBeRepresentedBy<T>(this Type type, T runtimeType)
        {
            var methodInfo = typeof(RuntimeTests).GetMethod(
                string.Concat("Assert", type.Name),
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new[] { typeof(T) },
                null);

            if (methodInfo == null)
            {
                throw new Exception("Aggregate root type under test is missing static method AssertRepresentedBy.");
            }

            methodInfo.Invoke(null, new object[] { runtimeType });
        }
    }
}
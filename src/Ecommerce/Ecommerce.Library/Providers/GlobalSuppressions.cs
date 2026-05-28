// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "I do not like primary constructors and do not use them. They are mutable, which can lead to unintended side effects.", Scope = "member", Target = "~M:Ecommerce.Library.Providers.OrderProvider.#ctor(ILogger{Ecommerce.Library.Providers.OrderProvider},IAmazonDynamoDB)")]

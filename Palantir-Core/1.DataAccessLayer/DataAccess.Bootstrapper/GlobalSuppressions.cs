// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Scope = "member", Target = "Ix.Palantir.DataAccess.Bootstrapper.DataAccessRegistry.#ConfigureRepositories(StructureMap.ConfigurationExpression)", Justification = "It is a registry, so it is fine to have many references here")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Scope = "member", Target = "Ix.Palantir.DataAccess.Bootstrapper.DataAccessRegistry.#InstantiateInStructureMap()", Justification = "It is a registry, so it is fine to have many references here")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Scope = "type", Target = "Ix.Palantir.DataAccess.Bootstrapper.DataAccessRegistry", Justification = "It is a registry, so it is fine to have many references here")]

// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Scope = "member", Target = "Ix.Palantir.Services.ConcurrentAnalysisService.#RankModels(Ix.Palantir.Services.API.Analytics.RankedModel,System.Collections.Generic.IList`1<Ix.Palantir.Services.API.Analytics.RankedModel>)", Justification = "It is simple but LINQ is too complicated for CodeAnalysis")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Ix.Palantir.Services.ProjectService.#DeleteProject(System.Int32,System.Int32)", Justification = " ")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Scope = "type", Target = "Ix.Palantir.Services.MetricsService", Justification = " ")]

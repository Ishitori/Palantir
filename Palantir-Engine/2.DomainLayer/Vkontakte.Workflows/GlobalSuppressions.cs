// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Scope = "member", Target = "Ix.Palantir.Vkontakte.Workflows.Processes.GetFeedsFromVkProcess.#.ctor(Ix.Palantir.Vkontakte.API.Access.IVkConnectionBuilder,Ix.Palantir.DataAccess.API.Repositories.IVkGroupRepository,Ix.Palantir.Logging.ILog,Ix.Palantir.Configuration.API.IConfigurationProvider,Ix.Palantir.Localization.API.IDateTimeHelper,Ix.Palantir.Utilities.IWebUtilities)", Justification = "Need to investigate it futher")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Ix.Palantir.Vkontakte.Workflows.Processes.GetFeedsFromVkProcess.#ProcessQueueItem(Ix.Palantir.DomainModel.FeedQueueItem,Ix.Palantir.Vkontakte.API.Access.IVkDataProvider,Ix.Palantir.Configuration.FeedProcessingConfig)", Justification = "Exception got logged, so it is ok")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Ix.Palantir.Vkontakte.Workflows.Processes.VkDataFeedsParserProcess.#ProcessSpecificGroupFeeds(Ix.Palantir.Configuration.FeedProcessingConfig)", Justification = "Exception got logged, so it is ok")]

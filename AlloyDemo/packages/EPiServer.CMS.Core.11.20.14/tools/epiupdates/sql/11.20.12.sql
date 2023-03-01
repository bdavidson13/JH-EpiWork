--beginvalidatingquery
	if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_DatabaseVersion]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
		begin
			declare @ver int
			exec @ver = sp_DatabaseVersion
			if (@ver >= 7070)
				select 0, 'Already correct database version'
			else if (@ver = 7069)
				select 1, 'Upgrading database'
			else
				select -1, 'Invalid database version detected'
		end
	else
		select -1, 'Not an EPiServer database'
--endvalidatingquery
GO



PRINT N'Altering Procedure [dbo].[netApprovalAdd]...';
GO

ALTER PROCEDURE [dbo].[netApprovalAdd](
	@StartedBy NVARCHAR(255),
	@Started DATETIME2,
	@Approvals [dbo].[AddApprovalTable] READONLY)
AS
BEGIN
	DELETE t FROM [dbo].[tblApproval] t  WITH (HOLDLOCK)
	JOIN @Approvals a ON t.ApprovalKey = a.ApprovalKey

	DECLARE @StepCounts AS TABLE(VersionID INT, StepCount INT, RequireCommentOnApprove BIT, RequireCommentOnReject BIT, RequireCommentOnStart BIT)

	INSERT INTO @StepCounts
	SELECT VersionID, COUNT(*) AS StepCount, RequireCommentOnApprove, RequireCommentOnReject, RequireCommentOnStart FROM (
		SELECT DISTINCT adv.pkID AS VersionID, ads.pkID AS StepID, adv.RequireCommentOnApprove, adv.RequireCommentOnReject, adv.RequireCommentOnStart FROM [dbo].[tblApprovalDefinitionVersion] adv
		JOIN [dbo].[tblApprovalDefinitionStep] ads ON adv.pkID = ads.fkApprovalDefinitionVersionID
		JOIN @Approvals approvals ON approvals.ApprovalDefinitionVersionID = adv.pkID
	) X	GROUP BY VersionID, RequireCommentOnApprove, RequireCommentOnReject, RequireCommentOnStart

	INSERT INTO [dbo].[tblApproval]([fkApprovalDefinitionVersionID], [ApprovalKey], [fkLanguageBranchID], [ActiveStepIndex], [ActiveStepStarted], [StepCount], [StartedBy], [Started], [Completed], [ApprovalStatus], [RequireCommentOnApprove], [RequireCommentOnReject], [RequireCommentOnStart])
	SELECT a.ApprovalDefinitionVersionID, a.ApprovalKey, a.LanguageBranchID, 0, @Started, sc.StepCount, @StartedBy, @Started, NULL, 0, sc.RequireCommentOnApprove, sc.RequireCommentOnReject, sc.RequireCommentOnStart FROM @Approvals a
	JOIN @StepCounts sc ON a.ApprovalDefinitionVersionID = sc.VersionID

	SELECT t.ApprovalKey, t.pkID AS ApprovalID, t.StepCount, t.RequireCommentOnApprove, t.RequireCommentOnReject, t.RequireCommentOnStart FROM [dbo].[tblApproval] t
	JOIN @Approvals a ON t.ApprovalKey = a.ApprovalKey
END
GO


PRINT N'Altering Procedure [dbo].[sp_DatabaseVersion]...';
GO

ALTER PROCEDURE [dbo].[sp_DatabaseVersion]
AS
	RETURN 7070
GO


PRINT N'Update complete.';
GO

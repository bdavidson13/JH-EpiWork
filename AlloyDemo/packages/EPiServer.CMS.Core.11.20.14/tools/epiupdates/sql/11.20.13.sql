--beginvalidatingquery
	if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_DatabaseVersion]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
    begin
            declare @ver int
            exec @ver=sp_DatabaseVersion
            if (@ver >= 7071)
				select 0, 'Already correct database version'
            else if (@ver = 7070)
                 select 1, 'Upgrading database'
            else
                 select -1, 'Invalid database version detected'
    end
    else
            select -1, 'Not an EPiServer database'
--endvalidatingquery
GO

PRINT N'Altering Procedure [dbo].[netMappedIdentityDelete]...';

GO
ALTER PROCEDURE [dbo].[netMappedIdentityDelete]
	@Provider NVARCHAR(255),
	@ProviderUniqueId NVARCHAR(2048)
AS
BEGIN
	SET NOCOUNT ON;
    DECLARE @returnVal TABLE ([ContentGuid] [UNIQUEIDENTIFIER] NOT NULL)

	DELETE
	FROM tblMappedIdentity
    OUTPUT deleted.ContentGuid INTO @returnVal
	WHERE tblMappedIdentity.Provider = @Provider AND tblMappedIdentity.ProviderUniqueId = @ProviderUniqueId

    SELECT TOP 1 ContentGuid FROM @returnVal
END
GO
PRINT N'Altering Procedure [dbo].[sp_DatabaseVersion]...';


GO
ALTER PROCEDURE [dbo].[sp_DatabaseVersion]
AS
	RETURN 7071
GO
PRINT N'Update complete.';


GO

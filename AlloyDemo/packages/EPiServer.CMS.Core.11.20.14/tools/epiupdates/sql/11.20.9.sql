--beginvalidatingquery
	if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_DatabaseVersion]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
		begin
			declare @ver int
			exec @ver = sp_DatabaseVersion
			if (@ver >= 7069)
				select 0, 'Already correct database version'
			else if (@ver = 7068)
				select 1, 'Upgrading database'
			else
				select -1, 'Invalid database version detected'
		end
	else
		select -1, 'Not an EPiServer database'
--endvalidatingquery
GO

GO
PRINT N'Altering Procedure [dbo].[netContentTypeSave]...';


GO
ALTER PROCEDURE [dbo].[netContentTypeSave]
(
	@ContentTypeID			INT,
	@ContentTypeGUID		UNIQUEIDENTIFIER,
	@Saved					DATETIME		= NULL,
	@SavedBy				NVARCHAR(255)	= NULL,
	@Name					NVARCHAR(50),
	@Base					NVARCHAR(50)	= NULL,
	@Version				NVARCHAR(50)	= NULL,
	@DisplayName			NVARCHAR(50)	= NULL,
	@Description			NVARCHAR(255)	= NULL,
	@DefaultWebFormTemplate	NVARCHAR(1024)	= NULL,
	@DefaultMvcController	NVARCHAR(1024)	= NULL,
	@DefaultMvcPartialView	NVARCHAR(255)	= NULL,
	@Filename				NVARCHAR(255)	= NULL,
	@Available				BIT				= NULL,
	@SortOrder				INT				= NULL,
	@ModelType				NVARCHAR(1024)	= NULL,
	
	@DefaultID				INT				= NULL,
	@DefaultName 			NVARCHAR(100)	= NULL,
	@StartPublishOffset		INT				= NULL,
	@StopPublishOffset		INT				= NULL,
	@VisibleInMenu			BIT				= NULL,
	@PeerOrder 				INT				= NULL,
	@ChildOrderRule 		INT				= NULL,
	@ArchiveContentID 		INT				= NULL,
	@FrameID 				INT				= NULL,
	@ACL					NVARCHAR(MAX)	= NULL,
	@ContentType			INT				= 0,
	@Created				DATETIME
)
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON

	DECLARE @IdString NVARCHAR(255)
	
	IF @ContentTypeID <= 0
	BEGIN
        --We want to block parallel inserts which can happen if several applications initializes in parallel, hence we read all table with update lock
        DECLARE @NotUsed INT
        SELECT @NotUsed = pkID FROM tblContentType WITH (UPDLOCK, HOLDLOCK)
        
		SET @ContentTypeID = ISNULL((SELECT pkID FROM tblContentType where Name = @Name), @ContentTypeID)
	END

	IF (@ContentTypeID <= 0)
	BEGIN
		SELECT TOP 1 @IdString = IdString FROM tblContentType
		INSERT INTO tblContentType
			(Saved,
			SavedBy,
			Name,
			DisplayName,
			Base,
			Version,
			DefaultMvcController,
			DefaultWebFormTemplate,
			DefaultMvcPartialView,
			Description,
			Available,
			SortOrder,
			ModelType,
			Filename,
			IdString,
			ContentTypeGUID,
			ACL,
			ContentType,
			Created)
		VALUES
			(@Saved,
			@SavedBy,
			@Name,
			@DisplayName,
			@Base,
			@Version,
			@DefaultMvcController,
			@DefaultWebFormTemplate,
			@DefaultMvcPartialView,
			@Description,
			@Available,
			@SortOrder,
			@ModelType,
			@Filename,
			@IdString,
			@ContentTypeGUID,
			@ACL,
			@ContentType,
			@Created)

		SET @ContentTypeID = SCOPE_IDENTITY() 
		
	END
	ELSE
	BEGIN
		UPDATE tblContentType
		SET
			Saved = @Saved,
			SavedBy = @SavedBy,
			Name=@Name,
			Base=@Base,
			Version=@Version,
			DisplayName=@DisplayName,
			Description=@Description,
			DefaultWebFormTemplate=@DefaultWebFormTemplate,
			DefaultMvcController=@DefaultMvcController,
			DefaultMvcPartialView=@DefaultMvcPartialView,
			Available=@Available,
			SortOrder=@SortOrder,
			ModelType = @ModelType,
			Filename = @Filename,
			ACL=@ACL,
			ContentType = @ContentType,
			@ContentTypeGUID = ContentTypeGUID
		WHERE
			pkID = @ContentTypeID
	END

    IF (@DefaultID IS NULL)
	BEGIN
		DELETE FROM tblContentTypeDefault WHERE fkContentTypeID=@ContentTypeID
	END
	ELSE
	BEGIN
		IF (EXISTS (SELECT pkID FROM tblContentTypeDefault WHERE fkContentTypeID=@ContentTypeID))
		BEGIN
			UPDATE tblContentTypeDefault SET
				Name 				= @DefaultName,
				StartPublishOffset 	= @StartPublishOffset,
				StopPublishOffset 	= @StopPublishOffset,
				VisibleInMenu 		= @VisibleInMenu,
				PeerOrder 			= @PeerOrder,
				ChildOrderRule 		= @ChildOrderRule,
				fkArchiveContentID 	= @ArchiveContentID,
				fkFrameID 			= @FrameID
			WHERE fkContentTypeID = @ContentTypeID
		END
		ELSE
		BEGIN
			INSERT INTO tblContentTypeDefault 
				(fkContentTypeID,
				Name,
				StartPublishOffset,
				StopPublishOffset,
				VisibleInMenu,
				PeerOrder,
				ChildOrderRule,
				fkArchiveContentID,
				fkFrameID)
			VALUES
				(@ContentTypeID,
				@DefaultName,
				@StartPublishOffset,
				@StopPublishOffset,
				@VisibleInMenu,
				@PeerOrder,
				@ChildOrderRule,
				@ArchiveContentID,
				@FrameID)
		END
	END
    
	SELECT @ContentTypeID AS "ID", @ContentTypeGUID AS "GUID"
END
GO
PRINT N'Altering Procedure [dbo].[netPageDefinitionSave]...';


GO
ALTER PROCEDURE dbo.netPageDefinitionSave
(
	@PageDefinitionID      INT OUTPUT,
	@PageTypeID            INT,
	@Name                  NVARCHAR(100),
	@PageDefinitionTypeID  INT,
	@Required              BIT = NULL,
	@Advanced              INT = NULL,
	@Searchable            BIT = NULL,
	@DefaultValueType      INT = NULL,
	@EditCaption           NVARCHAR(255) = NULL,
	@HelpText              NVARCHAR(2000) = NULL,
	@ObjectProgID          NVARCHAR(255) = NULL,
	@LongStringSettings    INT = NULL,
	@SettingsID            UNIQUEIDENTIFIER = NULL,
	@FieldOrder            INT = NULL,
	@Type                  INT = NULL OUTPUT,
	@OldType               INT = NULL OUTPUT,
	@LanguageSpecific      INT = 0,
	@DisplayEditUI         BIT = NULL,
	@ExistsOnModel         BIT = 0,
    @EditorHint               NVARCHAR(255) = NULL
)
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON

	SELECT @OldType = tblPropertyDefinitionType.Property 
	FROM tblPropertyDefinition
	INNER JOIN tblPropertyDefinitionType ON tblPropertyDefinitionType.pkID=tblPropertyDefinition.fkPropertyDefinitionTypeID
	WHERE tblPropertyDefinition.pkID=@PageDefinitionID

	SELECT @Type = Property FROM tblPropertyDefinitionType WHERE pkID=@PageDefinitionTypeID
	IF @Type IS NULL
		RAISERROR('Cannot find data type',16,1)
	IF @PageTypeID=0
		SET @PageTypeID = NULL

	IF @PageDefinitionID = 0 AND @ExistsOnModel = 1
	BEGIN
        --We want to block parallel inserts which can happen if several applications initializes in parallel, hence we read all table with update lock
        DECLARE @NotUsed INT
        SELECT @NotUsed = pkID FROM tblPropertyDefinition WITH (UPDLOCK, HOLDLOCK)
		
        SET @PageDefinitionID = ISNULL((SELECT pkID FROM tblPropertyDefinition where Name = @Name AND fkContentTypeID = @PageTypeID), @PageDefinitionID)
	END

	IF @PageDefinitionID=0
	BEGIN	
		INSERT INTO tblPropertyDefinition
		(
			fkContentTypeID,
			fkPropertyDefinitionTypeID,
			Name,
			Property,
			Required,
			Advanced,
			Searchable,
			DefaultValueType,
			EditCaption,
			HelpText,
			ObjectProgID,
			LongStringSettings,
			SettingsID,
			FieldOrder,
			LanguageSpecific,
			DisplayEditUI,
			ExistsOnModel,
            EditorHint
		)
		VALUES
		(
			@PageTypeID,
			@PageDefinitionTypeID,
			@Name,
			@Type,
			@Required,
			@Advanced,
			@Searchable,
			@DefaultValueType,
			@EditCaption,
			@HelpText,
			@ObjectProgID,
			@LongStringSettings,
			@SettingsID,
			@FieldOrder,
			@LanguageSpecific,
			@DisplayEditUI,
			@ExistsOnModel,
            @EditorHint
		)
		SET @PageDefinitionID =  SCOPE_IDENTITY() 
	END
	ELSE
	BEGIN
		UPDATE tblPropertyDefinition SET
			Name 		= @Name,
			fkPropertyDefinitionTypeID	= @PageDefinitionTypeID,
			Property 	= @Type,
			Required 	= @Required,
			Advanced 	= @Advanced,
			Searchable 	= @Searchable,
			DefaultValueType = @DefaultValueType,
			EditCaption 	= @EditCaption,
			HelpText 	= @HelpText,
			ObjectProgID 	= @ObjectProgID,
			LongStringSettings = @LongStringSettings,
			SettingsID = @SettingsID,
			LanguageSpecific = @LanguageSpecific,
			FieldOrder = @FieldOrder,
			DisplayEditUI = @DisplayEditUI,
			ExistsOnModel = @ExistsOnModel,
            EditorHint = @EditorHint
		WHERE pkID=@PageDefinitionID

        DELETE FROM tblPropertyDefault WHERE fkPageDefinitionID=@PageDefinitionID
	    IF @LanguageSpecific<3
	    BEGIN
		    /* NOTE: Here we take into consideration that language neutral dynamic properties are always stored on language 
			    with id 1 (which perhaps should be changed and in that case the special handling here could be removed). */
		    IF @PageTypeID IS NULL
		    BEGIN
			    DELETE tblProperty
			    FROM tblProperty
			    INNER JOIN tblPage ON tblPage.pkID=tblProperty.fkPageID
			    WHERE fkPageDefinitionID=@PageDefinitionID AND tblProperty.fkLanguageBranchID<>1
		    END
		    ELSE
		    BEGIN
			    DELETE tblProperty
			    FROM tblProperty
			    INNER JOIN tblPage ON tblPage.pkID=tblProperty.fkPageID
			    WHERE fkPageDefinitionID=@PageDefinitionID AND tblProperty.fkLanguageBranchID<>tblPage.fkMasterLanguageBranchID
		    END
		    DELETE tblWorkProperty
		    FROM tblWorkProperty
		    INNER JOIN tblWorkPage ON tblWorkProperty.fkWorkPageID=tblWorkPage.pkID
		    INNER JOIN tblPage ON tblPage.pkID=tblWorkPage.fkPageID
		    WHERE fkPageDefinitionID=@PageDefinitionID AND tblWorkPage.fkLanguageBranchID<>tblPage.fkMasterLanguageBranchID

		    DELETE 
			    tblCategoryPage
		    FROM
			    tblCategoryPage
		    INNER JOIN
			    tblPage
		    ON
			    tblPage.pkID = tblCategoryPage.fkPageID
		    WHERE
			    CategoryType = @PageDefinitionID
		    AND
			    tblCategoryPage.fkLanguageBranchID <> tblPage.fkMasterLanguageBranchID

		    DELETE 
			    tblWorkCategory
		    FROM
			    tblWorkCategory
		    INNER JOIN 
			    tblWorkPage
		    ON
			    tblWorkCategory.fkWorkPageID = tblWorkPage.pkID
		    INNER JOIN
			    tblPage
		    ON
			    tblPage.pkID = tblWorkPage.fkPageID
		    WHERE
			    CategoryType = @PageDefinitionID
		    AND
			    tblWorkPage.fkLanguageBranchID <> tblPage.fkMasterLanguageBranchID
	    END
	END
END
GO
PRINT N'Altering Procedure [dbo].[netPropertyDefinitionTypeSave]...';


GO

ALTER PROCEDURE [dbo].[netPropertyDefinitionTypeSave]
(
	@ID 			INT OUTPUT,
	@Property 		INT,
	@Name 			NVARCHAR(255),
    @GUID           uniqueidentifier = NULL,
	@TypeName 		NVARCHAR(255) = NULL,
	@AssemblyName 	NVARCHAR(255) = NULL,
	@BlockTypeID	uniqueidentifier = NULL
)
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON

	/* In case several sites start up at sametime, e.g. in enterprise it may occour that both sites tries to insert at same time. 
	Therefore a check is made to see it it already exist an entry with same guid, and if so an update is performed instead of insert.*/
	IF @ID <= 0
	BEGIN
        --We want to block parallel inserts which can happen if several applications initializes in parallel, hence we read all table with update lock
        DECLARE @NotUsed INT
        SELECT @NotUsed = pkID FROM tblPropertyDefinitionType WITH (UPDLOCK, HOLDLOCK)

        IF (@BlockTypeID IS NOT NULL)
            SET @ID = ISNULL((SELECT pkID FROM tblPropertyDefinitionType WHERE fkContentTypeGUID = @BlockTypeID), @ID)
        ELSE
            SET @ID = ISNULL((SELECT pkID FROM tblPropertyDefinitionType WHERE TypeName = @TypeName AND AssemblyName = @AssemblyName), @ID)
	END

	IF @ID<0
	BEGIN
		IF @AssemblyName='EPiServer'
			SELECT @ID = Max(pkID)+1 FROM tblPropertyDefinitionType WHERE pkID<1000
		ELSE
			SELECT @ID = CASE WHEN Max(pkID)<1000 THEN 1000 ELSE Max(pkID)+1 END FROM tblPropertyDefinitionType
		INSERT INTO tblPropertyDefinitionType
		(
			pkID,
			Property,
			Name,
            GUID,
			TypeName,
			AssemblyName,
			fkContentTypeGUID
		)
		VALUES
		(
			@ID,
			@Property,
			@Name,
            @GUID,
			@TypeName,
			@AssemblyName,
			@BlockTypeID
		)
	END
	ELSE
		UPDATE tblPropertyDefinitionType SET
			Name 		= @Name,
			Property		= @Property,
            GUID        = @GUID,
			TypeName 	= @TypeName,
			AssemblyName 	= @AssemblyName,
			fkContentTypeGUID = @BlockTypeID
		WHERE pkID=@ID
		
END
GO
PRINT N'Altering Procedure [dbo].[netSchedulerSave]...';


GO
ALTER PROCEDURE [dbo].netSchedulerSave
(
@pkID		UNIQUEIDENTIFIER = NULL OUTPUT,
@Name		NVARCHAR(50),
@Enabled	BIT = 0,
@NextExec 	DATETIME,
@DatePart	NCHAR(2) = NULL,
@Interval	INT = 0,
@MethodName NVARCHAR(100),
@fStatic 	BIT,
@TypeName 	NVARCHAR(1024),
@AssemblyName NVARCHAR(100),
@InstanceData   VARBINARY(MAX) = NULL,
@IsStoppable BIT = 0,
@Restartable    INT = 0
)
AS
BEGIN

--We want to block parallel inserts which can happen if several applications initializes in parallel, hence we read all table with update lock
DECLARE @NotUsed UNIQUEIDENTIFIER
SELECT @NotUsed = pkID FROM tblScheduledItem WITH (UPDLOCK, HOLDLOCK)

DECLARE @Exists BIT
IF (@pkID IS NULL)
BEGIN
    SELECT @pkID = pkID FROM tblScheduledItem WHERE TypeName = @TypeName AND AssemblyName = @AssemblyName
    IF (@pkID is NULL)
    BEGIN
        SET @pkID = NEWID()
        SET @Exists = 0
    END
    ELSE
        SET @Exists = 1
END
ELSE
    SET @Exists = ISNULL((SELECT TOP 1 1 FROM tblScheduledItem WHERE pkID = @pkID), 0)

IF (@Exists = 1)
    UPDATE tblScheduledItem SET
		    Name 		= @Name,
		    Enabled 	= @Enabled,
		    NextExec 	= @NextExec,
		    [DatePart] 	= @DatePart,
		    Interval 		= @Interval,
		    MethodName 	= @MethodName,
		    fStatic 		= @fStatic,
		    TypeName 	= @TypeName,
		    AssemblyName 	= @AssemblyName,
		    InstanceData	= @InstanceData,
		    IsStoppable = @IsStoppable,
		    Restartable		= @Restartable
	    WHERE pkID = @pkID
ELSE
    INSERT INTO tblScheduledItem(pkID,Name,Enabled,NextExec,[DatePart],Interval,MethodName,fStatic,TypeName,AssemblyName,InstanceData,IsStoppable,Restartable)
	    VALUES(@pkID,@Name,@Enabled,@NextExec,@DatePart,@Interval, @MethodName,@fStatic,@TypeName,@AssemblyName,@InstanceData, @IsStoppable, @Restartable)
END
GO
PRINT N'Altering Procedure [dbo].[sp_DatabaseVersion]...';


GO
ALTER PROCEDURE [dbo].[sp_DatabaseVersion]
AS
	RETURN 7069
GO
PRINT N'Update complete.';


GO

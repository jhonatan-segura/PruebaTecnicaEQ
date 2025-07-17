-- DocKeywords

CREATE OR ALTER PROCEDURE GetDocKeywords
AS
BEGIN
   SET NOCOUNT ON;

   SELECT *
   FROM DocKey;
END

GO

CREATE OR ALTER PROCEDURE GetDocKeywordById
   @Id INT,
   @ResultCode INT OUTPUT
AS
BEGIN
   SET NOCOUNT ON;

   IF NOT EXISTS (SELECT 1
   FROM DocKey
   WHERE Id = @Id)   
   BEGIN
      SET @ResultCode = -1;
      -- No encontrado
      RETURN;
   END

   SELECT *
   FROM DocKey
   WHERE Id = @Id;
   SET @ResultCode = 0;
-- No encontrado
END

GO

CREATE OR ALTER PROCEDURE CreateDocKeyword
   @DocName VARCHAR(200),
   @Keyword VARCHAR(200),
   @ResultCode INT OUTPUT
AS
BEGIN
   SET NOCOUNT ON;

   BEGIN TRY
      BEGIN TRANSACTION;

         -- Validación simple (puedes expandirla si lo deseas)
         IF @Keyword IS NULL OR @DocName IS NULL
         BEGIN
            SET @ResultCode = -1;
            ROLLBACK TRANSACTION;
            RETURN;
         END

        INSERT INTO DocKey
        (KeyWord, DocName)
        VALUES
        (@Keyword, @DocName);

        COMMIT TRANSACTION;
        SET @ResultCode = 0;  -- Éxito
   END TRY
   BEGIN CATCH
      IF @@TRANCOUNT > 0
         ROLLBACK TRANSACTION;
         SET @ResultCode = -1; -- Error
   END CATCH
END

GO

CREATE OR ALTER PROCEDURE UpdateDocKeyword
   @Id INT,
   @DocName VARCHAR(200),
   @Keyword VARCHAR(200),
   @ResultCode INT OUTPUT
AS
BEGIN
   SET NOCOUNT ON;

   IF NOT EXISTS (SELECT 1
   FROM DocKey
   WHERE Id = @Id)   
   BEGIN
      SET @ResultCode = -1;
      -- Not found
      RETURN;
   END

   UPDATE DocKey
   SET DocName = @DocName,
       KeyWord = @Keyword
   WHERE Id = @Id;

   SET @ResultCode = 0;
END

GO

CREATE OR ALTER PROCEDURE DeleteDocKeyword
   @Id INT,
   @ResultCode INT OUTPUT
AS
BEGIN
   SET NOCOUNT ON;

   IF NOT EXISTS (SELECT 1
   FROM DocKey
   WHERE Id = @Id)
   BEGIN
      SET @ResultCode = -1;
      -- Not found
      RETURN;
   END

   DELETE FROM DocKey
   WHERE Id = @Id;

   SET @ResultCode = 0;
END

GO

-- LogProcess

CREATE OR ALTER PROCEDURE GetLogProcesses
AS
BEGIN
   SET NOCOUNT ON;

   SELECT *
   FROM LogProcess;
END

GO

CREATE OR ALTER PROCEDURE GetLogProcessById
   @Id INT,
   @ResultCode INT OUTPUT
AS
BEGIN
   SET NOCOUNT ON;

   IF NOT EXISTS (SELECT 1
   FROM LogProcess
   WHERE Id = @Id)   
   BEGIN
      SET @ResultCode = -1;
      -- No encontrado
      RETURN;
   END

   SELECT *
   FROM LogProcess
   WHERE Id = @Id;
   SET @ResultCode = 0;
-- No encontrado
END

GO

CREATE OR ALTER PROCEDURE CreateLogProcess
   @OriginalFileName VARCHAR(200),
   @Status VARCHAR(200),
   @NewFileName VARCHAR(200),
   @DateProcessed DATETIME,
   @ResultCode INT OUTPUT
AS
BEGIN
   SET NOCOUNT ON;

   BEGIN TRY
      BEGIN TRANSACTION;

         -- Validación simple (puedes expandirla si lo deseas)
         IF @OriginalFileName IS NULL OR @Status IS NULL OR @DateProcessed IS NULL
         BEGIN
            SET @ResultCode = -1;
            ROLLBACK TRANSACTION;
            RETURN;
         END

        INSERT INTO LogProcess
        (OriginalFileName, Status, NewFileName, DateProcessed)
        VALUES
        (@OriginalFileName, @Status, @NewFileName, @DateProcessed);

        COMMIT TRANSACTION;
        SET @ResultCode = 0;  -- Éxito
   END TRY
   BEGIN CATCH
      IF @@TRANCOUNT > 0
         ROLLBACK TRANSACTION;
         SET @ResultCode = -1; -- Error
   END CATCH
END

GO

CREATE OR ALTER PROCEDURE UpdateLogProcess
   @Id INT,
   @OriginalFileName VARCHAR(200),
   @Status VARCHAR(200),
   @NewFileName VARCHAR(200),
   @ResultCode INT OUTPUT
AS
BEGIN
   SET NOCOUNT ON;

   IF NOT EXISTS (SELECT 1
   FROM LogProcess
   WHERE Id = @Id)   
   BEGIN
      SET @ResultCode = -1;
      -- Not found
      RETURN;
   END

   UPDATE LogProcess
   SET OriginalFileName = @OriginalFileName,
       Status = @Status,
       NewFileName = @NewFileName
   WHERE Id = @Id;

   SET @ResultCode = 0;
END

GO

CREATE OR ALTER PROCEDURE DeleteLogProcess
   @Id INT,
   @ResultCode INT OUTPUT
AS
BEGIN
   SET NOCOUNT ON;

   IF NOT EXISTS (SELECT 1
   FROM LogProcess
   WHERE Id = @Id)
   BEGIN
      SET @ResultCode = -1;
      RETURN;
   END

   DELETE FROM LogProcess
   WHERE Id = @Id;

   SET @ResultCode = 0;
END

GO
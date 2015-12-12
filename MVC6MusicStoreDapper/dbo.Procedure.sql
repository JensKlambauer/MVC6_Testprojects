USE [Mvc6Dapper]
GO

CREATE PROC [dbo].[HoleAlleAlben]
AS 	
	SELECT [Album].[AlbumId],[Album].[Title],[Album].[GenreId],[Album].[ArtistId],[Album].[Price],[Album].[AlbumArtUrl],
		    [Genre].[GenreId], [Genre].[Name], [Genre].[Description],
			[Artist].[ArtistId], [Artist].[ArtistName]
	FROM [dbo].[Album] INNER JOIN [dbo].[Artist] ON
		[dbo].[Album].[ArtistId] = [dbo].[Artist].[ArtistId]
		INNER JOIN [dbo].[Genre] ON
		[dbo].[Genre].[GenreId] = [dbo].[Album].[GenreId]
GO

USE [master]
GO
/****** Object:  Database [Apollo.AIM.SNAP]    Script Date: 04/01/2010 13:20:13 ******/
CREATE DATABASE [Apollo.AIM.SNAP] ON  PRIMARY 
( NAME = N'Apollo.AIM.SNAP', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\Apollo.AIM.SNAP.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Apollo.AIM.SNAP_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\Apollo.AIM.SNAP_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
 COLLATE SQL_Latin1_General_CP1_CI_AS
GO
/****** CHANGE PASSWORD TO MATCH ENVIRONMENT ******/
CREATE LOGIN [svc_snap] WITH PASSWORD=N'qPD9j4Mv', DEFAULT_DATABASE=[Apollo.AIM.SNAP], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
EXEC dbo.sp_dbcmptlevel @dbname=N'Apollo.AIM.SNAP', @new_cmptlevel=90
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Apollo.AIM.SNAP].[dbo].[sp_fulltext_database] @action = 'disable'
end
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET ARITHABORT OFF 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET  ENABLE_BROKER 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET  READ_WRITE 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET RECOVERY FULL 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET  MULTI_USER 
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Apollo.AIM.SNAP] SET DB_CHAINING OFF 
GO

USE [Apollo.AIM.SNAP]
GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'svc_snap')
CREATE USER [svc_snap] FOR LOGIN [svc_snap] WITH DEFAULT_SCHEMA=[dbo]
GO
EXEC sp_addrolemember db_datareader, [svc_snap]  
GO 
EXEC sp_addrolemember db_datawriter , [svc_snap]  
GO 


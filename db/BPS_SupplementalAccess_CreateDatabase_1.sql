USE [master]
GO
/****** Object:  Database [BPS_SupplementalAccess] ******/
CREATE DATABASE [BPS_SupplementalAccess] ON  PRIMARY 
( NAME = N'BPS_SupplementalAccess', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\BPS_SupplementalAccess.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'BPS_SupplementalAccess_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\BPS_SupplementalAccess_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
 COLLATE SQL_Latin1_General_CP1_CI_AS
GO
/****** CHANGE PASSWORD TO MATCH ENVIRONMENT ******/
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'svc_snap')
CREATE LOGIN [svc_snap] WITH PASSWORD=N'qPD9j4Mv', DEFAULT_DATABASE=[BPS_SupplementalAccess], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
EXEC dbo.sp_dbcmptlevel @dbname=N'BPS_SupplementalAccess', @new_cmptlevel=90
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BPS_SupplementalAccess].[dbo].[sp_fulltext_database] @action = 'disable'
end
GO
ALTER DATABASE [BPS_SupplementalAccess] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET ARITHABORT OFF 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET  ENABLE_BROKER 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET  READ_WRITE 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET RECOVERY FULL 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET  MULTI_USER 
GO
ALTER DATABASE [BPS_SupplementalAccess] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [BPS_SupplementalAccess] SET DB_CHAINING OFF 
GO

USE [BPS_SupplementalAccess]
GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'svc_snap')
CREATE USER [svc_snap] FOR LOGIN [svc_snap] WITH DEFAULT_SCHEMA=[dbo]
GO
EXEC sp_addrolemember db_datareader, [svc_snap]  
GO 
EXEC sp_addrolemember db_datawriter , [svc_snap]  
GO 


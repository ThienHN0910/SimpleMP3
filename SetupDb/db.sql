-- Xoá DB nếu đã tồn tại (Cẩn thận khi chạy!)
IF DB_ID('MusicPlayerDb') IS NOT NULL
BEGIN
    ALTER DATABASE MusicPlayerDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MusicPlayerDb;
END;
GO

-- Tạo database mới
CREATE DATABASE MusicPlayerDb;
GO

USE MusicPlayerDb;
GO

-- ================================
-- 1. Bảng Users
-- ================================
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- ================================
-- 2. Bảng Artists
-- ================================
CREATE TABLE Artists (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Bio NVARCHAR(MAX),
    AvatarUrl NVARCHAR(500),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- ================================
-- 3. Bảng Tracks (liên kết Artists)
-- ================================
CREATE TABLE Tracks (
    Id INT PRIMARY KEY IDENTITY(1,1),
    YouTubeId NVARCHAR(20) NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    Album NVARCHAR(255),
    DurationSeconds INT,
    FilePath NVARCHAR(500),
    ArtistId INT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (ArtistId) REFERENCES Artists(Id) ON DELETE SET NULL
);
GO

-- ================================
-- 4. Bảng Playlists
-- ================================
CREATE TABLE Playlists (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    IsSystem BIT NOT NULL DEFAULT 0, -- true nếu là playlist hệ thống như "Favorites"
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
GO

-- ================================
-- 5. Bảng PlaylistTracks (nhiều-nhiều)
-- ================================
CREATE TABLE PlaylistTracks (
    PlaylistId INT NOT NULL,
    TrackId INT NOT NULL,
    PRIMARY KEY (PlaylistId, TrackId),
    FOREIGN KEY (PlaylistId) REFERENCES Playlists(Id) ON DELETE CASCADE,
    FOREIGN KEY (TrackId) REFERENCES Tracks(Id) ON DELETE CASCADE
);
GO

-- ================================
-- 6. Bảng PlayHistory (lịch sử phát)
-- ================================
CREATE TABLE PlayHistory (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    TrackId INT NOT NULL,
    PlayedAt DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (TrackId) REFERENCES Tracks(Id) ON DELETE CASCADE
);
GO

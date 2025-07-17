CREATE DATABASE DocumentProcessing;

USE DocumentProcessing;

CREATE TABLE DocKey (
   Id INT IDENTITY(1,1) PRIMARY KEY,
   DocName varchar(200),
   KeyWord varchar(200), -- Key es una palabra reservada así que se reemplazó por KeyWord
)

CREATE TABLE LogProcess (
   Id INT IDENTITY(1,1) PRIMARY KEY,
   OriginalFileName VARCHAR(200),
   Status VARCHAR(200),
   NewFileName VARCHAR(200),
   DateProcessed DATETIME -- Se reemplazó DateProcces por DateProcessed
)

INSERT INTO DocKey (DocName, KeyWord) VALUES ('PruebaTecnica', 'tecnica');
INSERT INTO DocKey (DocName, KeyWord) VALUES ('DocumentoSalud', 'salud');
INSERT INTO DocKey (DocName, KeyWord) VALUES ('DocumentoBiblioteca', 'libro');
-- LogProcess
INSERT INTO LogProcess (OriginalFileName, [Status], NewFileName, DateProcessed) VALUES ('documento','unknown', NULL, '2024-07-16 14:30:00');
INSERT INTO LogProcess (OriginalFileName, [Status], NewFileName, DateProcessed) VALUES ('documento2','processed','PruebaTecnica', '2024-07-16 14:30:00');
INSERT INTO LogProcess (OriginalFileName, [Status], NewFileName, DateProcessed) VALUES ('formularioSalud','processed','DocumentoSalud', '2024-07-16 14:30:00');
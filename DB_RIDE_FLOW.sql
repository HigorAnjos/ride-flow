-- Apagar o banco de dados se já existir
DROP DATABASE IF EXISTS RIDE_FLOW;
CREATE DATABASE RIDE_FLOW;

-- Criar a tabela de motos
CREATE TABLE Motos (
    Identificador TEXT PRIMARY KEY, -- Agora aceita identificadores alfanuméricos personalizados
    Ano INT NOT NULL,
    Modelo VARCHAR(100) NOT NULL,
    Placa VARCHAR(10) NOT NULL UNIQUE
);

-- Verificar se a tabela foi criada
SELECT 'Tabela Motos criada com sucesso!' AS status;

-- Criar a tabela de entregadores
CREATE TABLE Entregadores (
    Identificador TEXT PRIMARY KEY, -- Agora aceita identificadores alfanuméricos personalizados
    Nome VARCHAR(100) NOT NULL,
    CNPJ VARCHAR(14) NOT NULL UNIQUE,
    DataNascimento DATE NOT NULL,
    NumeroCNH VARCHAR(11) NOT NULL UNIQUE,
    TipoCNH VARCHAR(5) NOT NULL
);

-- Criar a tabela de locações
CREATE TABLE Locacoes (
    Identificador TEXT PRIMARY KEY, -- Agora aceita identificadores alfanuméricos personalizados
    EntregadorId TEXT NOT NULL,
    MotoId TEXT NOT NULL,
    DataInicio DATE NOT NULL,
    DataTermino DATE NOT NULL,
    DataPrevisaoTermino DATE NOT NULL,
    Plano INT NOT NULL,
    DataDevolucao DATE,
    FOREIGN KEY (EntregadorId) REFERENCES Entregadores(Identificador) ON DELETE CASCADE,
    FOREIGN KEY (MotoId) REFERENCES Motos(Identificador) ON DELETE CASCADE
);

-- Inserir dados de teste na tabela de motos
INSERT INTO Motos (Identificador, Ano, Modelo, Placa)
VALUES
('moto123', 2020, 'Mottu Sport', 'ABC-1234'),
('moto456', 2024, 'Mottu Classic', 'DEF-5678'),
('moto789', 2021, 'Mottu Premium', 'GHI-9101');

-- Inserir dados de teste na tabela de entregadores
INSERT INTO Entregadores (Identificador, Nome, CNPJ, DataNascimento, NumeroCNH, TipoCNH)
VALUES
('entregador123', 'João da Silva', '12345678901234', '1990-01-01', '12345678900', 'A'),
('entregador456', 'Maria Oliveira', '98765432109876', '1985-05-15', '98765432100', 'B'),
('entregador789', 'Carlos Pereira', '45678912345678', '1992-09-20', '45678912300', 'A+B');

-- Inserir dados de teste na tabela de locações
INSERT INTO Locacoes (Identificador, EntregadorId, MotoId, DataInicio, DataTermino, DataPrevisaoTermino, Plano)
VALUES
('locacao123', 'entregador123', 'moto123', '2024-01-01', '2024-01-07', '2024-01-07', 7),
('locacao456', 'entregador456', 'moto456', '2024-01-02', '2024-01-16', '2024-01-16', 15),
('locacao789', 'entregador789', 'moto789', '2024-01-05', '2024-02-04', '2024-02-04', 30);

-- Consultar as tabelas para verificar os dados inseridos
SELECT * FROM Motos;
SELECT * FROM Entregadores;
SELECT * FROM Locacoes;

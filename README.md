# Projeto Ride Flow

Este é o projeto **Ride Flow**, uma aplicação que utiliza **.NET**.

---

## Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)
- [Git](https://git-scm.com/)

---

## Como configurar e rodar o projeto

### 1. Configure o banco de dados

Os bancos de dados utilizam PostgreSQL e MongoDB, além do RabbitMQ para mensageria. Eles serão configurados automaticamente ao subir o **Docker Compose**. Certifique-se de que o arquivo `docker-compose.yml` já está configurado corretamente.

O script SQL `DB_RIDE_FLOW.sql` será usado para criar e popular o banco.

---

### 2. Subindo o Docker Compose

Para inicializar o ambiente Docker, execute:

```bash
docker-compose up -d
```

Esse comando iniciará os serviços definidos no arquivo `docker-compose.yml`.


### Como parar o ambiente

Para parar os contêineres, execute:

```bash
docker-compose down
```


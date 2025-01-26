# Projeto Ride Flow

Este � o projeto **Ride Flow**, uma aplica��o que utiliza **.NET**.

---

## Pr�-requisitos

Antes de come�ar, certifique-se de ter instalado:

- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)
- [Git](https://git-scm.com/)

---

## Como configurar e rodar o projeto

### 1. Configure o banco de dados

Os bancos de dados utilizam PostgreSQL e MongoDB, al�m do RabbitMQ para mensageria. Eles ser�o configurados automaticamente ao subir o **Docker Compose**. Certifique-se de que o arquivo `docker-compose.yml` j� est� configurado corretamente.

O script SQL `DB_RIDE_FLOW.sql` ser� usado para criar e popular o banco.

---

### 2. Subindo o Docker Compose

Para inicializar o ambiente Docker, execute:

```bash
docker-compose up -d
```

Esse comando iniciar� os servi�os definidos no arquivo `docker-compose.yml`.


### Como parar o ambiente

Para parar os cont�ineres, execute:

```bash
docker-compose down
```


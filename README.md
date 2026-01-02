# 🛒 Rappi Orders Service

Microservicio de **Pedidos (Orders)** desarrollado como prueba técnica, utilizando **.NET 9**, **DDD**, **EF Core**, **Worker Service** y **Docker**.

Este README cumple con los siguientes puntos solicitados:

* Arquitectura
* Estructura del proyecto
* Cómo ejecutar con Docker
* Endpoints disponibles

---

## 🧠 Arquitectura

La solución sigue el enfoque **Domain-Driven Design (DDD)**, separando claramente responsabilidades en capas:

* **Domain**
  Contiene la lógica de negocio pura: entidades, value objects, agregados y reglas del dominio.

* **Application**
  Contiene los casos de uso, DTOs y contratos. Orquesta la lógica del dominio sin conocer detalles de infraestructura.

* **Infrastructure**
  Implementa la persistencia con **EF Core**, repositorios y configuración de base de datos.

* **API**
  Expone los endpoints HTTP, validaciones, CORS, Rate Limiting y documentación OpenAPI.

* **Worker**
  Servicio en segundo plano encargado de ejecutar procesos automáticos (cancelación de pedidos antiguos).

---

## 🗂️ Estructura del proyecto

```
Rappi.Orders
├── Domain
│   ├── Entities
│   ├── ValueObjects
│   ├── Aggregates
│   └── Rules
│
├── Application
│   ├── DTOs
│   ├── UseCases
│   └── Common (ApiResponse)
│
├── Infrastructure
│   ├── Persistence (EF Core)
│   ├── Configurations
│   └── Repositories
│
├── Api
│   ├── Controllers
│   ├── Program.cs
│   └── appsettings.*.json
│
├── Worker
│   ├── Jobs
│   ├── Program.cs
│   └── appsettings.*.json
```

---

## 🐳 Cómo ejecutar con Docker

La solución está dockerizada usando **Docker Compose** y Dockerfiles **multi-stage (Build + Runtime)**.

### Requisitos

* Docker
* Docker Compose

### Levantar el sistema completo

Desde la raíz del repositorio:

```bash
docker compose up --build
```

### Servicios levantados

* **API**: [http://localhost:8080](http://localhost:8080)
* **Swagger / OpenAPI (Scalar)**: [http://localhost:8080/scalar/v1](http://localhost:8080/scalar/v1)
* **Worker**: se ejecuta en segundo plano

> La API y el Worker leen la configuración desde `appsettings.Development.json`.

---

## 🔌 Endpoints disponibles

### Crear pedido

**POST** `/api/orders`

```json
{
  "aggregatorOrder": "ORD-100",
  "items": [
    {
      "orderCode": "BG-100",
      "description": "item 1",
      "value": 2.09
    }
  ]
}
```

---

### Obtener total por pedido

**GET** `/api/orders/{aggregatorOrder}/total`

---

### Cambiar estado de un pedido

**PATCH** `/api/orders/status`

```json
{
  "aggregatorOrder": "ORD-100",
  "newStatus": "Paid"
}
```

---

## 🔁 Worker Service

El Worker ejecuta un proceso automático que:

* Se ejecuta **3 veces al día** en producción
* En Development corre cada pocos segundos
* Cancela pedidos en estado `Created` con más de **14 días**

---

## ✅ Estado del proyecto

✔️ Arquitectura DDD
✔️ API REST
✔️ Worker Service
✔️ Configuración por entornos
✔️ Docker Compose
✔️ Documentación de endpoints
